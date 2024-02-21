Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web

Namespace Helpers

    Public NotInheritable Class LogHandler

#Region "Properties"

        Private Shared _NameFile As String

        Public Shared Property NameFile() As String
            Get
                Return _NameFile
            End Get
            Set(ByVal value As String)
                _NameFile = value
            End Set
        End Property

#End Region

#Region "WarningLog"

        ''' <summary>
        ''' Overload method
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="entry"></param>
        ''' <remarks></remarks>
        Public Shared Sub WarningLog(source As String, entry As String)
            WarningLog(source, entry, String.Empty)
        End Sub

        ''' <summary>
        ''' Overload of method
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="entry"></param>
        ''' <param name="prefix"></param>
        ''' <remarks></remarks>
        Public Shared Sub WarningLog(source As String, entry As String, prefix As String)
            WarningLog(source, entry, prefix, Nothing)
        End Sub

        ''' <summary>
        ''' Overrable
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="entry"></param>
        ''' <param name="prefix"></param>
        ''' <param name="customData"></param>
        ''' <remarks></remarks>
        Public Shared Sub WarningLog(source As String, entry As String, prefix As String, customData As Object)
            WarningLog(source, entry, prefix, customData, True)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="entry"></param>
        ''' <param name="prefix"></param>
        ''' <param name="customData"></param>
        ''' <param name="Async"></param>
        ''' <remarks></remarks>
        Public Shared Sub WarningLog(source As String, entry As String, prefix As String, customData As Object, Async As Boolean)
            Dim logprefix As String = "Logs.Prefix".StringValue
            Dim filename As String = String.Empty
            Dim DebugMode As String = String.Empty

            Dim rootPath As String = GetPath()

            If String.IsNullOrEmpty(logprefix) Then
                filename = String.Format("{0}\{1:yyyyMMdd}.Warning.log", rootPath, Date.Now)
            Else
                filename = String.Format("{0}\{1:yyyyMMdd}.Warning.{2}.log", rootPath, Date.Now, logprefix)
            End If

            NameFile = filename

            DebugMode = "FrontOffice.Debug.Mode".StringValue("File")

            Dim parameters As New Dictionary(Of String, Object)
            With parameters
                Dim _DateEfective As DateTime = DateTime.Now
                .Add("DateEfective", _DateEfective.ToString("hh:mm:ss.fff"))
                .Add("FileName", filename)
                .Add("Source", source)
                .Add("Entry", entry)
                .Add("ThreadId", Thread.CurrentThread.ManagedThreadId)
                .Add("DebugMode", DebugMode)
                .Add("CustomData", customData)
                If Not IsNothing(HttpContext.Current) AndAlso
                   Not IsNothing(HttpContext.Current.Session) Then
                    .Add("SessionID", HttpContext.Current.Session.SessionID)
                Else
                    .Add("SessionID", "without session id")
                End If
            End With
            If Async Then
                Dim AddUsersSecurityTraceAsyn As New Task(actionWarningLog, parameters)
                AddUsersSecurityTraceAsyn.Start()
            Else
                While (WarningLogInternal(parameters) = False)
                    Thread.Sleep(800)
                End While
            End If
        End Sub

        Public Shared actionWarningLog As Action(Of Object) =
                         Sub(parameterContainer As Object)
                             Dim parameterInternal As Dictionary(Of String, Object) = DirectCast(parameterContainer, Dictionary(Of String, Object))
                             Dim attempts As Integer = 1
                             While (WarningLogInternal(parameterInternal) = False AndAlso attempts < 4)
                                 Thread.Sleep(800)
                                 attempts += 1
                             End While
                         End Sub

        ''' <summary>
        ''' Method of the Internal WarnningLog
        ''' </summary>
        ''' <param name="parameterInternal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function WarningLogInternal(parameterInternal As Dictionary(Of String, Object)) As Boolean
            Dim Result As Boolean = False
            Dim _FileName As String = parameterInternal.Item("FileName").ToString
            Dim _IP As String = Connection.GetIPRequest()
            Dim _Source As String = parameterInternal.Item("Source").ToString
            Dim _Entry As String = parameterInternal.Item("Entry").ToString
            Dim _DateEfective As String = parameterInternal.Item("DateEfective").ToString
            Dim _ThreadId As String = parameterInternal.Item("ThreadId").ToString
            Dim _SessionID As String = parameterInternal.Item("SessionID").ToString
            Dim _DebugMode As String = parameterInternal.Item("DebugMode").ToString()
            Dim _CustomData As Object = parameterInternal.Item("CustomData")
            Dim fs As FileStream = Nothing
            Try

                If File.Exists(_FileName) Then
                    fs = File.Open(_FileName, FileMode.Append)
                Else
                    fs = File.Create(_FileName)
                End If

                Dim sw As StreamWriter = New StreamWriter(fs)
                Dim thisDate As DateTime = DateTime.Now

                sw.WriteLine(String.Format("{0} {3} {4} {5} {1} {2}", _DateEfective, _Source, _Entry, Connection.NameHostFromIp(_IP), _ThreadId, _SessionID))
                sw.Close()

                fs.Close()

                If Not _DebugMode.Equals("File") Then

                    Dim temporalLog As New EventLogClient.EventLog
                    With temporalLog
                        .FactTime = DateTime.Now
                        .HostSource = Connection.NameHostFromIp(_IP)
                        .TypeTrace = Convert.ToInt32(Enumerations.EnumTraceType.Warrning)
                        .Source = _Source
                        .Entry = String.Format("{0}{1} Seccion Id:{2}", _Entry, vbLf, _SessionID)
                        If Not IsNothing(_CustomData) AndAlso Serialize.IsSerializable(_CustomData) Then
                            Dim dataCustom As String = Serialize.SerializarObject(_CustomData)
                            .EventLogDetail = New EventLogClient.EventLogDetail With {.Detail = dataCustom}
                        End If
                    End With
                    Using EventLogClient As New EventLogClient.ManagerClient
                        EventLogClient.LogSave(temporalLog)
                    End Using
                End If
                Result = True
            Catch exIO As IOException
                If Marshal.GetHRForException(exIO) = -2147024864 Then
                    Result = False
                Else
                    Result = True
                End If
            Catch ex As Exception
                Result = True
            Finally
                If fs IsNot Nothing Then
                    fs.Close()
                End If
            End Try
            Return Result
        End Function

#End Region

#Region "PerformanceLog"

        ''' <summary>
        ''' Writes an entry in the logbook file for trace
        ''' </summary>
        ''' <param name="nameMethod">Nombre del método que llama</param>
        Public Shared Function PerformanceLogBegin(source As String, nameMethod As String, parameters As Dictionary(Of String, Object)) As Stopwatch
            Dim result As New Stopwatch()
            result.Start()

            PerformanceLog(source, nameMethod, True, parameters, Nothing)
            Return result
        End Function

        Public Shared Sub PerformanceLogFinish(source As String, nameMethod As String, control As Stopwatch)
            PerformanceLog(source, nameMethod, False, Nothing, control)
        End Sub

        Friend Shared Sub PerformanceLog(source As String, nameMethod As String, way As Boolean, parametersByData As Dictionary(Of String, Object), control As Stopwatch)

            Dim prefix As String = "Performance.Prefix".StringValue
            Dim filename As String
            Dim parameters As New Dictionary(Of String, Object)
            If prefix.IsNotEmpty Then
                Dim rootPath As String = GetPath()
                With parameters
                    .Add("DateEfective", Date.Now)
                    .Add("Source", source)
                    .Add("Method", nameMethod)
                    filename = String.Format("{0}\{1:yyyyMMdd}.{2}.log", rootPath, Date.Now, prefix)
                    .Add("FileName", filename)
                    .Add("Way", way)
                    If Not IsNothing(HttpContext.Current) AndAlso
                       Not IsNothing(HttpContext.Current.Session) Then
                        .Add("SessionID", HttpContext.Current.Session.SessionID)
                    Else
                        .Add("SessionID", "without session id")
                    End If
                    If Not way Or control.IsNotEmpty Then
                        control.Stop()
                        .Add("Elapsed", control.Elapsed)
                    End If
                End With

                Dim attempts As Integer = 1
                While (PerformanceLogInternal(parameters, parametersByData) = False AndAlso attempts < 4)
                    Thread.Sleep(800)
                    attempts += 1
                End While
            End If

        End Sub

        Private Shared Function PerformanceLogInternal(parameterInternal As Dictionary(Of String, Object), parametersByData As Dictionary(Of String, Object)) As Boolean
            Dim result As Boolean = False

            Dim fs As FileStream = Nothing

            Try
                Dim _FileName = parameterInternal.Item("FileName").ToString
                Dim _SessionID = parameterInternal.Item("SessionID").ToString
                Dim _way As Boolean = Boolean.Parse(parameterInternal.Item("Way").ToString())
                Dim _source As String = parameterInternal.Item("Source").ToString()
                Dim _time_elapsed As String = String.Empty
                If Not _way Then
                    _time_elapsed = parameterInternal.Item("Elapsed").ToString
                    If _time_elapsed.IsNotEmpty() Then
                        _time_elapsed = String.Format("{0}Time elapsed in milliseconds:{0},{0}{1}{0}", Strings.Chr(34), _time_elapsed)
                    End If
                End If

                Dim _DateEfective As Date = parameterInternal.Item("DateEfective")
                Dim _Method = parameterInternal.Item("Method").ToString
                If File.Exists(_FileName) Then
                    fs = File.Open(_FileName, FileMode.Append)
                Else
                    fs = File.Create(_FileName)
                End If

                Dim sw As StreamWriter = New StreamWriter(fs)
                Dim values As String = String.Empty
                If parametersByData.IsNotEmpty() Then
                    Dim valuesArray As New List(Of String)
                    For Each key As String In parametersByData.Keys
                        If parametersByData.Item(key) Is Nothing Then
                            valuesArray.Add(String.Format("{0}='{1}'", key, "Empty"))
                        Else
                            valuesArray.Add(String.Format("{0}='{1}'", key, parametersByData.Item(key).ToString()))
                        End If
                    Next

                    values = String.Join("; ", valuesArray)
                End If
                If _way Then
                    sw.WriteLine(String.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{0}{6}{0}", Strings.Chr(34), _DateEfective.ToString("hh:mm:ss.fff"), _SessionID, "Begin ", _source, _Method, IIf(values.IsEmpty(), "", values)))
                Else
                    sw.WriteLine(String.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{6}", Strings.Chr(34), _DateEfective.ToString("hh:mm:ss.fff"), _SessionID, "Finish", _source, _Method, _time_elapsed))
                End If
                sw.Close()
                fs.Close()
                result = True
            Catch exIO As IOException
                If Marshal.GetHRForException(exIO) = -2147024864 Then
                    result = False
                Else
                    result = True
                End If
            Catch ex As Exception

                result = True
            Finally
                If fs IsNot Nothing Then
                    fs.Close()
                End If
            End Try

            Return result
        End Function

#End Region

#Region "TraceLog"

        ''' <summary>
        ''' Writes an entry in the logbook file for trace
        ''' </summary>
        ''' <param name="entry">Information to be recorded</param>
        Public Shared Sub TraceLog(entry As String)
            TraceLog(AssemblyHandler.GetFrameProcessFullName(2), entry, "Trace")
        End Sub

        ''' <summary>
        ''' Writes an entry in the logbook file for trace
        ''' </summary>
        ''' <param name="source">Key associated with the originating source registration</param>
        ''' <param name="entry">Information to be recorded</param>
        Public Shared Sub TraceLog(source As String, entry As String)
            TraceLog(source, entry, "Trace")
        End Sub

        Public Shared Sub TraceLog(source As String, entry As String, prefix As String)
            TraceLog(source, entry, prefix, Nothing)
        End Sub

        ''' <summary>
        ''' Overload method
        ''' </summary>
        ''' <param name="source">Key associated with the originating source registration</param>
        ''' <param name="entry">Information to be recorded</param>
        ''' <param name="prefix"></param>
        ''' <param name="customData"></param>
        ''' <remarks></remarks>
        Public Shared Sub TraceLog(source As String, entry As String, prefix As String, customData As Object)
            TraceLog(source, entry, prefix, customData, True)
        End Sub

        ''' <summary>
        ''' Method Base of the TraceLog,
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="entry"></param>
        ''' <param name="prefix"></param>
        ''' <param name="customData"></param>
        ''' <param name="Async"></param>
        ''' <remarks></remarks>
        Public Shared Sub TraceLog(source As String, entry As String, prefix As String, customData As Object, ByVal Async As Boolean)
            Dim logprefix As String = "Logs.Prefix".StringValue
            Dim filename As String = String.Empty
            Dim DebugMode As String = String.Empty

            Dim rootPath As String = GetPath()

            If "FrontOffice.Debug".IsTrue Then

                If String.IsNullOrEmpty(logprefix) Then
                    filename = String.Format("{0}\{1:yyyyMMdd}.{2}.log", rootPath, Date.Now, prefix)
                Else
                    filename = String.Format("{0}\{1:yyyyMMdd}.{2}.{3}.log", rootPath, Date.Now, prefix, logprefix)
                End If

                NameFile = filename

                DebugMode = "FrontOffice.Debug.Mode".StringValue("File")

                Dim parameters As New Dictionary(Of String, Object)
                With parameters
                    Dim _DateEfective As DateTime = DateTime.Now
                    .Add("DateEfective", _DateEfective.ToString("hh:mm:ss.fff"))
                    .Add("FileName", filename)
                    .Add("Source", source)
                    .Add("Entry", entry)
                    .Add("ThreadId", Thread.CurrentThread.ManagedThreadId)
                    .Add("DebugMode", DebugMode)
                    .Add("CustomData", customData)
                    If Not IsNothing(HttpContext.Current) AndAlso
                       Not IsNothing(HttpContext.Current.Session) Then
                        .Add("SessionID", HttpContext.Current.Session.SessionID)
                    Else
                        .Add("SessionID", "without session id")
                    End If
                End With

                Dim modeMultiThread As Boolean = True
                If "FrontOffice.Trace.Mode".StringValue = "Single" Then
                    modeMultiThread = False
                End If
                If modeMultiThread Then
                    If Async Then
                        Dim AddUsersSecurityTraceAsyn As New Task(actionTraceLog, parameters)
                        AddUsersSecurityTraceAsyn.Start()
                    Else
                        While (TraceLogInternal(parameters) = False)
                            Thread.Sleep(800)
                        End While
                    End If
                Else
                    TraceLogInternal(parameters)
                End If

            End If
        End Sub

        Public Shared actionTraceLog As Action(Of Object) =
                        Sub(parameterContainer As Object)
                            Dim parameterInternal As Dictionary(Of String, Object) = DirectCast(parameterContainer, Dictionary(Of String, Object))
                            Dim attempts As Integer = 1
                            While (TraceLogInternal(parameterInternal) = False AndAlso attempts < 4)
                                Thread.Sleep(800)
                                attempts += 1
                            End While
                        End Sub

        Public Shared Function GetFileName() As String
            Dim result As String = String.Empty
            result = GetPath()
            Return result
        End Function

        Private Shared Function GetPath() As String
            Dim rootPath As String = "Path.Logs".StringValue

            If String.IsNullOrEmpty(rootPath) Then
                rootPath = Helpers.Directory.GetPathRoot
            Else
                If rootPath.ToLower.Contains("special") Then
                    If rootPath.ToLower.Equals("special") Then
                        rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Else
                        Dim temporal = String.Format("{0}{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), rootPath.Replace("special", String.Empty).Replace("@", "\"))
                        rootPath = temporal
                    End If
                End If
            End If

            Return rootPath
        End Function

        ''' <summary>
        ''' Internal method of the Tracelog
        ''' </summary>
        ''' <param name="pParamters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function TraceLogInternal(pParamters As Dictionary(Of String, Object)) As Boolean
            Dim Result As Boolean = False
            Dim parameterInternal As Dictionary(Of String, Object) = pParamters
            Dim fs As FileStream = Nothing
            Dim _FileName As String
            Dim _IP As String
            Dim _Source As String
            Dim _Entry As String
            Dim _DateEfective As String
            Dim _ThreadId As String
            Dim _SessionID As String
            Dim _DebugMode As String
            Dim _CustomData As Object
            With parameterInternal
                _FileName = parameterInternal.Item("FileName").ToString
                _IP = Connection.GetIPRequest()
                _Source = parameterInternal.Item("Source").ToString
                _Entry = parameterInternal.Item("Entry").ToString
                _DateEfective = parameterInternal.Item("DateEfective")
                _ThreadId = parameterInternal.Item("ThreadId").ToString
                _SessionID = parameterInternal.Item("SessionID").ToString
                _DebugMode = parameterInternal.Item("DebugMode").ToString
                _CustomData = parameterInternal.Item("CustomData")
            End With

            Try
                If File.Exists(_FileName) Then
                    fs = File.Open(_FileName, FileMode.Append)
                Else
                    fs = File.Create(_FileName)
                End If

                Dim sw As StreamWriter = New StreamWriter(fs)
                sw.WriteLine(String.Format("{0} {3} {4} {5} {1} {2} ", _DateEfective, _Source, _Entry, Connection.NameHostFromIp(_IP), _ThreadId, _SessionID))
                sw.Close()
                fs.Close()
                If Not _DebugMode.Equals("File") Then
                    Using EventLogClient As New EventLogClient.ManagerClient
                        Dim temporalLog As New EventLogClient.EventLog
                        With temporalLog
                            .FactTime = DateTime.Now
                            .HostSource = Connection.NameHostFromIp(_IP)
                            .TypeTrace = Convert.ToInt32(Enumerations.EnumTraceType.Trace)
                            .Source = _Source
                            .Entry = String.Format("{0}{1} Seccion Id:{2}", _Entry, vbLf, _SessionID)
                            If Not IsNothing(_CustomData) AndAlso Serialize.IsSerializable(_CustomData) Then
                                Dim dataCustom As String = Serialize.SerializarObject(_CustomData)
                                .EventLogDetail = New EventLogClient.EventLogDetail With {.Detail = dataCustom}
                            End If
                        End With
                        EventLogClient.LogSave(temporalLog)
                    End Using

                End If
                Result = True
            Catch exIO As IOException
                If Marshal.GetHRForException(exIO) = -2147024864 Then
                    Result = False
                Else
                    Result = True
                End If
            Catch ex As Exception

                Result = True
            Finally
                If fs IsNot Nothing Then
                    fs.Close()
                End If
            End Try
            Return Result
        End Function

#End Region

#Region "ErrorLog"

        Public Shared Function ErrorLogInternal(parameterInternal As Dictionary(Of String, Object)) As Boolean
            Dim Result As Boolean = False
            Dim fs As FileStream = Nothing
            Dim _fileName As String = String.Empty
            Dim _DateEfective As String
            Dim _IP As String = String.Empty
            Dim _Source As String = String.Empty
            Dim _CurrentException As Exception
            Dim _Entry As String = String.Empty
            Dim _ExceptionStack As StackTrace
            Dim _ServerVariables As StringBuilder
            Dim _DebugMode As String = String.Empty
            Dim _SessionID As String = String.Empty
            Dim _Code As String = String.Empty
            With parameterInternal
                _Code = .Item("Code")
                _fileName = .Item("FileName")
                _DateEfective = .Item("DateEfective")
                _IP = Connection.GetIPOnlyRequest
                _Source = .Item("Source")
                _CurrentException = .Item("CurrentException")
                _Entry = .Item("Entry")
                _ExceptionStack = .Item("ExceptionStack")
                _ServerVariables = .Item("ServerVariables")
                _DebugMode = parameterInternal.Item("DebugMode").ToString
                _SessionID = parameterInternal.Item("SessionID")
            End With

            Try

                If File.Exists(_fileName) Then
                    fs = File.Open(_fileName, FileMode.Append)
                Else
                    fs = File.Create(_fileName)
                End If

                Dim sw As StreamWriter = New StreamWriter(fs)
                Dim stringTemporal As String = String.Format("{0} {1} {2}", _IP, Connection.NameHostFromIp(_IP), _Source)
                With sw
                    .WriteLine(stringTemporal)
                End With
                If Not IsNothing(_CurrentException) Then
                    TraceInnerExceptionMessage2(_CurrentException, 1, sw)
                    TraceInnerExceptionData(_CurrentException, 1, sw)
                End If
                With sw
                    .WriteLine(String.Format(" Code:{0}", _Code))
                    If _Entry.IsNotEmpty Then
                        .WriteLine(" Entry: " + _Entry)
                    End If
                    .WriteLine(" Session Id:" + _SessionID)
                End With

                If Not IsNothing(_ServerVariables) AndAlso Not String.IsNullOrEmpty(_ServerVariables.ToString()) Then
                    With sw
                        .Write(_ServerVariables.ToString())
                    End With
                End If

                If _CurrentException IsNot Nothing Then
                    Dim ExceptionStack As New StackTrace(_CurrentException, True)
                    Dim _VectorStack As StackFrame() = ExceptionStack.GetFrames()
                    If _VectorStack IsNot Nothing Then
                        Dim _Index As Integer = _VectorStack.Length
                        If _Index - (_Index - 1) > 0 Then
                            Dim ExceptionFrame = ExceptionStack.GetFrame(_Index - 1)
                            With sw
                                .Write("Method: " + ExceptionFrame.GetMethod.ToString)
                                .WriteLine(String.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()))

                                .WriteLine(" Class: " + ExceptionFrame.GetMethod.DeclaringType.ToString)
                                .WriteLine("  File: " + ExceptionFrame.GetFileName())
                                .WriteLine(" Stack:")
                                .WriteLine(ExceptionStack.ToString.Replace("   at ", "  "))
                            End With
                        End If
                    End If
                Else
                    Dim stat As StackFrame() = _ExceptionStack.GetFrames()
                    Dim ExceptionFrame = stat(2)
                    With sw
                        .Write("Method: " + ExceptionFrame.GetMethod.ToString)
                        .WriteLine(String.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()))
                        .WriteLine(String.Format(" Class: {0}", ExceptionFrame.GetMethod().DeclaringType))
                        .WriteLine(String.Format("  File: {0}", ExceptionFrame.GetFileName()))
                        .WriteLine(" Stack:")
                        .WriteLine(_ExceptionStack.ToString.Replace("   at ", "  "))
                    End With
                End If
                sw.Close()
                fs.Close()

                If Not _DebugMode.Equals("File") Then
                    Dim sr As New StreamReader(_fileName)
                    Using EventLogClient As New EventLogClient.ManagerClient
                        Dim temporalLog As New EventLogClient.EventLog
                        With temporalLog
                            .Code = _Code
                            .FactTime = DateTime.Now
                            .HostSource = Connection.NameHostFromIp(_IP)
                            .TypeTrace = Convert.ToInt32(Enumerations.EnumTraceType.Error)
                            .Source = _Source
                            .Entry = String.Format("{0}{1} Seccion Id:{2}", _Entry, vbLf, _SessionID)
                            .EventLogDetail = New EventLogClient.EventLogDetail With {.Detail = sr.ReadToEnd}
                        End With
                        sr.Close()
                        EventLogClient.LogSave(temporalLog)
                    End Using
                End If
                Result = True
            Catch exIO As IOException
                If Marshal.GetHRForException(exIO) = -2147024864 Then
                    Result = False
                Else
                    Result = True
                End If
            Catch ex As Exception
                Result = True
            Finally
                If fs IsNot Nothing Then
                    fs.Close()
                End If
            End Try
            Return Result
        End Function

        Public Shared Sub ErrorLog(currentException As Exception)
            ErrorLog(AssemblyHandler.GetFrameProcessFullName(2), String.Empty, currentException, String.Empty)
        End Sub

        ''' <summary>
        ''' Writes an entry in the logbook file for errors
        ''' </summary>
        ''' <param name="source">Key associated with the originating source registration</param>
        ''' <param name="entry">Information to be recorded</param>
        Public Shared Sub ErrorLog(source As String, entry As String)
            ErrorLog(source, entry, Nothing, String.Empty)
        End Sub

        Public Shared Sub ErrorLog(source As String, entry As String, currentException As Exception)
            ErrorLog(source, entry, currentException, String.Empty)
        End Sub

        Public Shared Sub ErrorLog(source As String, entry As String, currentException As Exception, prefix As String)
            ErrorLog(source, entry, currentException, prefix, True, String.Empty)
        End Sub

        ''' <summary>
        ''' Writes an entry in the logbook file for errors
        ''' </summary>
        ''' <param name="source">Key associated with the originating source registration</param>
        ''' <param name="entry">Information to be recorded</param>
        ''' <param name="currentException">Exception that gives rise to record</param>
        Public Shared Sub ErrorLog(source As String, entry As String, currentException As Exception, prefix As String, Async As Boolean)
            ErrorLog(source, entry, currentException, prefix, False, String.Empty)
        End Sub

        ''' <summary>
        ''' Writes an entry in the logbook file for errors
        ''' </summary>
        ''' <param name="source">Key associated with the originating source registration</param>
        ''' <param name="entry">Information to be recorded</param>
        ''' <param name="currentException">Exception that gives rise to record</param>
        ''' <param name="prefix"></param>
        ''' <param name="Async"></param>
        ''' <param name="Code"></param>
        ''' <remarks></remarks>
        Public Shared Sub ErrorLog(source As String, entry As String, currentException As Exception, prefix As String, Async As Boolean, Code As String)

            Dim rootPath As String = GetPath()

            Dim fs As FileStream = Nothing
            Dim filename As String = String.Empty
            Dim DebugMode As String = String.Empty

            If Code.IsEmpty Then
                Code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            End If

            If String.IsNullOrEmpty(prefix) Then
                prefix = "Logs.Prefix".StringValue
            End If

            If String.IsNullOrEmpty(prefix) Then
                filename = String.Format("{0}\{1:yyyyMMdd}.error.log", rootPath, Date.Now)
            Else
                filename = String.Format("{0}\{1:yyyyMMdd}.error.{2}.log", rootPath, Date.Now, prefix)
            End If

            NameFile = filename

            DebugMode = "FrontOffice.Debug.Mode".StringValue("File")

            Dim ServerVariables As New StringBuilder

            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    If HttpContext.Current.Request.ServerVariables IsNot Nothing Then
                        Dim value As String
                        If "FrontOffice.Debug.Detail".IsTrue Then
                            With ServerVariables
                                .AppendLine("RawUrl: " + HttpContext.Current.Request.Url.LocalPath)
                                .AppendLine("   Query: " & HttpContext.Current.Request.QueryString.ToString)
                                .AppendLine("    Form: " & HttpContext.Current.Request.Form.ToString)
                                .Append(" Session: ")
                                If Not IsNothing(HttpContext.Current.Session) Then
                                    For Each key As String In HttpContext.Current.Session.Keys
                                        If Not IsNothing(HttpContext.Current.Session(key)) Then
                                            value = HttpContext.Current.Session(key).ToString
                                            If Not String.IsNullOrEmpty(value) AndAlso value.Length > 20 Then
                                                value = value.Substring(0, 20) & "..."
                                            End If
                                            .Append(String.Format("{0}={1}&", key, value))
                                        Else
                                            .Append(String.Format("{0}={1}&", key, "Null"))
                                        End If
                                    Next
                                Else
                                    .Append("Not Enable Session")
                                End If
                            End With
                        End If
                    End If
                End If
            End If

            Dim parameters As New Dictionary(Of String, Object)
            With parameters
                .Add("Code", Code)
                .Add("DateEfective", DateTime.Now.ToString("hh:mm:ss.fff"))
                .Add("FileName", filename)
                .Add("Source", source)
                .Add("Entry", entry)
                .Add("CurrentException", currentException)
                .Add("ThreadId", Thread.CurrentThread.ManagedThreadId)
                .Add("ExceptionStack", New StackTrace(True))
                .Add("ServerVariables", ServerVariables)
                If Not IsNothing(HttpContext.Current) AndAlso
                   Not IsNothing(HttpContext.Current.Session) Then
                    .Add("SessionID", HttpContext.Current.Session.SessionID)
                Else
                    .Add("SessionID", "without session id")
                End If
                .Add("DebugMode", DebugMode)
            End With

            If Async Then
                Dim AddUsersSecurityTraceAsyn As New Task(actionErrorLog, parameters)
                AddUsersSecurityTraceAsyn.Start()
            Else
                While (ErrorLogInternal(parameters) = False)
                    Thread.Sleep(800)
                End While
            End If

        End Sub

        Public Shared actionErrorLog As Action(Of Object) =
                        Sub(parameterContainer As Object)
                            Dim parameterInternal As Dictionary(Of String, Object) = DirectCast(parameterContainer, Dictionary(Of String, Object))
                            Dim attempts As Integer = 1
                            While (ErrorLogInternal(parameterInternal) = False AndAlso attempts < 4)
                                Thread.Sleep(800)
                                attempts += 1
                            End While
                        End Sub

#End Region

#Region "Tools"

        ''' <summary>
        ''' Gets all the detail of exception
        ''' </summary>
        ''' <param name="ex">Exception that gives rise to record</param>
        ''' <param name="level">level of exception</param>
        ''' <remarks></remarks>
        Private Shared Sub TraceInnerExceptionMessage2(ByVal ex As Exception, ByVal level As Integer, sw As StreamWriter)

            level += 1
            If Not IsNothing(ex.InnerException) Then

                TraceInnerExceptionMessage2(ex.InnerException, level, sw)
            End If

            With sw
                .WriteLine(String.Format(" {1}({0}) {2}", TypeName(ex), " ".PadLeft(level), ex.Message))
            End With

        End Sub

        Private Shared Sub TraceInnerExceptionData(ByVal ex As Exception, ByVal level As Integer, sw As StreamWriter)

            level += 1
            If Not IsNothing(ex.InnerException) Then

                TraceInnerExceptionData(ex.InnerException, level, sw)
            End If

            With sw
                If ex.Data.Count > 0 Then
                    .WriteLine(String.Format(" {1}({0}) Details:", TypeName(ex), " ".PadLeft(level)))
                    For Each de As DictionaryEntry In ex.Data
                        .WriteLine("   Key: {0,-20}  Value: {1}", de.Key, de.Value)
                    Next
                End If

            End With

        End Sub

        Public Shared Function ExistInTrace(stacks As StackFrame(), name As String) As Boolean
            Dim result As Boolean = False
            If stacks.IsNotEmpty Then
                For Each item In stacks
                    If item.GetType.IsNotEmpty Then
                        If item.GetMethod().Name.Equals(name) Then
                            result = True
                            Exit For
                        End If
                    End If

                Next
            End If
            Return result
        End Function

#End Region

        Public Shared Function GetMessage(exception As Exception) As String
            Dim result As String = String.Empty
            Dim body As New StringBuilder
            Dim exceptionCode = Marshal.GetHRForException(exception)
            If TypeOf exception Is HttpException Then
                Dim con = HttpContext.Current
                If DirectCast(exception, HttpException).GetHttpCode = 404 Then
                    body.AppendLine(String.Format("Not found URL '{0}'", con.Request.Url.ToString()))
                Else
                    body.AppendLine(String.Format("Message:'{0}'", exception.Message))
                End If
            Else
                Dim ExceptionStack As New StackTrace(exception, True)
                Dim _VectorStack As StackFrame() = ExceptionStack.GetFrames()
                If _VectorStack IsNot Nothing Then
                    Dim _Index As Integer = _VectorStack.Length
                    If _Index - (_Index - 1) > 0 Then
                        Dim ExceptionFrame = ExceptionStack.GetFrame(2)
                        With body
                            .Append("Method: " + ExceptionFrame.GetMethod.ToString)
                            .AppendLine(String.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()))

                            .AppendLine(" Class: " + ExceptionFrame.GetMethod.DeclaringType.ToString)
                            .AppendLine("  File: " + ExceptionFrame.GetFileName())
                            .AppendLine(" Stack:")
                            .AppendLine(ExceptionStack.ToString.Replace("   at ", "  "))
                        End With
                    End If
                End If
            End If
            result = body.ToString

            Return result
        End Function

    End Class

End Namespace