Imports System.Reflection
Imports System.Text
Imports System.Web.Hosting

Namespace Helpers

    Public Class AssemblyHandler

        ''' <summary>
        ''' Initializes a new instance of the AssemblyInfoCalling class.
        ''' </summary>
        ''' <param name="traceLevel">The trace level needed to get correct assembly
        ''' - will need to adjust based on where you put these classes in your project(s).</param>
        Public Sub New(Optional traceLevel As Integer = 4)
            '----------------------------------------------------------------------
            ' Default to "3" as the number of levels back in the stack trace to get the
            '  correct assembly for "calling" assembly
            '----------------------------------------------------------------------
            StackTraceLevel = traceLevel
        End Sub

        '----------------------------------------------------------------------
        ' Standard assembly attributes
        '----------------------------------------------------------------------
        Public ReadOnly Property Company() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyCompanyAttribute)(Function(a) a.Company)
            End Get
        End Property

        Public ReadOnly Property Product() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyProductAttribute)(Function(a) a.Product)
            End Get
        End Property

        Public ReadOnly Property Copyright() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyCopyrightAttribute)(Function(a) a.Copyright)
            End Get
        End Property

        Public ReadOnly Property Trademark() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyTrademarkAttribute)(Function(a) a.Trademark)
            End Get
        End Property

        Public ReadOnly Property Title() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyTitleAttribute)(Function(a) a.Title)
            End Get
        End Property

        Public ReadOnly Property Description() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyDescriptionAttribute)(Function(a) a.Description)
            End Get
        End Property

        Public ReadOnly Property Configuration() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyDescriptionAttribute)(Function(a) a.Description)
            End Get
        End Property

        Public ReadOnly Property FileVersion() As String
            Get
                Return GetCallingAssemblyAttribute(Of AssemblyFileVersionAttribute)(Function(a) a.Version)
            End Get
        End Property

        '----------------------------------------------------------------------
        ' Version attributes
        '----------------------------------------------------------------------
        Public Shared ReadOnly Property Version() As Version
            Get
                '----------------------------------------------------------------------
                ' Get the assembly, return empty if null
                '----------------------------------------------------------------------
                Dim assembly As Assembly = GetAssembly(StackTraceLevel)
                Return If(assembly Is Nothing, New Version(), assembly.GetName().Version)
            End Get
        End Property

        Public ReadOnly Property VersionFull() As String
            Get
                Return Version.ToString()
            End Get
        End Property

        Public ReadOnly Property VersionMajor() As String
            Get
                Return Version.Major.ToString()
            End Get
        End Property

        Public ReadOnly Property VersionMinor() As String
            Get
                Return Version.Minor.ToString()
            End Get
        End Property

        Public ReadOnly Property VersionBuild() As String
            Get
                Return Version.Build.ToString()
            End Get
        End Property

        Public ReadOnly Property VersionRevision() As String
            Get
                Return Version.Revision.ToString()
            End Get
        End Property

        '----------------------------------------------------------------------
        ' Set how deep in the stack trace we're looking - allows for customized changes
        '----------------------------------------------------------------------
        Public Shared Property StackTraceLevel() As Integer
            Get
                Return m_StackTraceLevel
            End Get
            Set
                m_StackTraceLevel = Value
            End Set
        End Property

        Private Shared m_StackTraceLevel As Integer

        ''' <summary>
        ''' Gets the calling assembly attribute.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="value">The value.</param>
        ''' <example>return GetCallingAssemblyAttribute&lt;AssemblyCompanyAttribute&gt;(a => a.Company);</example>
        ''' <returns></returns>
        Private Function GetCallingAssemblyAttribute(Of T As Attribute)(value As Func(Of T, String)) As String
            '----------------------------------------------------------------------
            ' Get the assembly, return empty if null
            '----------------------------------------------------------------------
            Dim assembly As Assembly = GetAssembly(StackTraceLevel)
            If assembly Is Nothing Then
                Return String.Empty
            End If

            '----------------------------------------------------------------------
            ' Get the attribute value
            '----------------------------------------------------------------------
            Dim attribute__1 As T = DirectCast(Attribute.GetCustomAttribute(assembly, GetType(T)), T)
            Return value.Invoke(attribute__1)
        End Function

        ''' <summary>
        ''' Go through the stack and gets the assembly
        ''' </summary>
        ''' <param name="stackTraceLevel">The stack trace level.</param>
        ''' <returns></returns>
        Private Shared Function GetAssembly(stackTraceLevel As Integer) As Assembly
            '----------------------------------------------------------------------
            ' Get the stack frame, returning null if none
            '----------------------------------------------------------------------
            Dim stackTrace As New StackTrace()
            Dim stackFrames As StackFrame() = stackTrace.GetFrames()
            If stackFrames Is Nothing Then
                Return Nothing
            End If

            '----------------------------------------------------------------------
            ' Get the declaring type from the associated stack frame, returning null if nonw
            '----------------------------------------------------------------------
            Dim declaringType = stackFrames(stackTraceLevel).GetMethod().DeclaringType
            If declaringType Is Nothing Then
                Return Nothing
            End If

            '----------------------------------------------------------------------
            ' Return the assembly
            '----------------------------------------------------------------------
            Dim assembly = declaringType.Assembly
            Return assembly
        End Function

        Friend Shared Function MethodName(currentMethod As String) As String
            Dim result As New StringBuilder
            Dim stackTrace As New StackTrace()
            Dim stackFrames As StackFrame() = stackTrace.GetFrames()
            Dim Index = stackFrames.[Select](Function(elem, indexInternal) New With {elem, indexInternal}).First(Function(p) (CType(p.elem, StackFrame)).GetMethod().Name.ToLower().Equals(currentMethod)).indexInternal + 1
            If stackFrames.IsNotEmpty AndAlso stackFrames.Length >= Index Then
                Dim stackCall As StackFrame = stackFrames(Index)
                With stackCall
                    If .GetMethod().IsNotEmpty AndAlso .GetMethod().Name.IsNotEmpty Then
                        result.AppendLine(String.Format("<<I>>Method:'{0}'", .GetMethod().Name))
                        Dim [nameSpace] As String = String.Empty
                        If .GetMethod().ReflectedType.IsNotEmpty AndAlso .GetMethod().ReflectedType.FullName.IsNotEmpty Then
                            Dim className As String = .GetMethod().ReflectedType.FullName
                            [nameSpace] = String.Format("{0}.{1}", className, .GetMethod().Name)
                        End If
                        If [nameSpace].IsEmpty Then
                            [nameSpace] = "Empty"
                        End If
                        result.AppendLine(String.Format("<<I>>Namespace:'{0}'", [nameSpace]))
                    Else
                        result.AppendLine(String.Format("<<I>>Method:'{0}'", "Empty"))
                    End If
                    result.AppendLine(String.Format("<<I>>Row/Column:'{0}'/'{1}'", .GetFileLineNumber, .GetFileColumnNumber))
                End With
            End If
            Return result.ToString
        End Function

        Public Shared Function GetFrameProcess(Index As Integer) As String
            Dim result As New StringBuilder
            Dim stackTrace As New StackTrace()
            Dim stackFrames As StackFrame() = stackTrace.GetFrames()
            If stackFrames.IsNotEmpty AndAlso stackFrames.Length >= Index Then
                Dim stackCall As StackFrame = stackFrames(Index)
                With stackCall
                    If .GetMethod().IsNotEmpty AndAlso .GetMethod().Name.IsNotEmpty Then
                        result.AppendLine(String.Format("<<I>>Method:'{0}'", .GetMethod().Name))
                        Dim [nameSpace] As String = String.Empty
                        If .GetMethod().ReflectedType.IsNotEmpty AndAlso .GetMethod().ReflectedType.FullName.IsNotEmpty Then
                            Dim className As String = .GetMethod().ReflectedType.FullName
                            [nameSpace] = String.Format("{0}.{1}", className, .GetMethod().Name)
                        End If
                        If [nameSpace].IsEmpty Then
                            [nameSpace] = "Empty"
                        End If
                        result.AppendLine(String.Format("<<I>>Namespace:'{0}'", [nameSpace]))
                    Else
                        result.AppendLine(String.Format("<<I>>Method:'{0}'", "Empty"))
                    End If
                    result.AppendLine(String.Format("<<I>>Row/Column:'{0}'/'{1}'", .GetFileLineNumber, .GetFileColumnNumber))
                End With
            End If
            Return result.ToString
        End Function

        Public Shared Function GetFrameProcessFullName(Index As Integer) As String
            Dim result As String = String.Empty
            Dim stackTrace As New StackTrace()
            Dim stackFrames As StackFrame() = stackTrace.GetFrames()
            If stackFrames.IsNotEmpty AndAlso stackFrames.Length >= Index Then
                Dim stackCall As StackFrame = stackFrames(Index)
                With stackCall
                    If .GetMethod().IsNotEmpty AndAlso .GetMethod().Name.IsNotEmpty Then
                        result = .GetMethod().Name
                        Dim [nameSpace] As String = String.Empty
                        If .GetMethod().ReflectedType.IsNotEmpty AndAlso .GetMethod().ReflectedType.FullName.IsNotEmpty Then
                            Dim className As String = .GetMethod().ReflectedType.FullName
                            [nameSpace] = String.Format("{0}.{1}", className, .GetMethod().Name)
                        End If
                        If [nameSpace].IsEmpty Then
                            [nameSpace] = "Empty"
                        End If
                        result = [nameSpace]
                    End If
                End With
            End If
            Return result
        End Function

        ''' <summary>
        ''' Obtiene el nombre del usuario de windows.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetUserNameWindows() As String
            Dim result As String = String.Empty
            If Not HostingEnvironment.IsHosted Then
                result = Environment.UserName
            End If
            Return result
        End Function

        ''' <summary>
        ''' Obtiene el nombre de la maquina.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetUserNameMachine() As String
            Dim result As String = String.Empty
            If Not HostingEnvironment.IsHosted Then
                result = Environment.MachineName
            End If
            Return result
        End Function

        ''' <summary>
        ''' Obtiene el nombre de la sistema operativo.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetOS() As String
            Dim result As String = String.Empty
            Try
                If Not HostingEnvironment.IsHosted Then
                    result = New Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName
                End If
            Catch ex As Exception
                LogHandler.ErrorLog("InMotionGIT.Common.Helpers.AssemblyHandler", "GetOS", ex)
                result = "Unknown"
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Obtiene el nombre de la sistema operativo.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetNameApplication() As String
            Dim result As String = String.Empty
            If Not HostingEnvironment.IsHosted Then
                Try
                    result = (New AssemblyHandler(5)).Title
                    If result.IsEmpty Then
                        result = Assembly.GetCallingAssembly.GetName().Name
                    End If
                Catch ex As Exception
                    result = "Name not found"
                End Try
            End If
            Return result
        End Function

        Public Shared Function GetVersionApplication() As String
            Dim result As String = String.Empty
            If Not HostingEnvironment.IsHosted Then
                Try
                    result = (New AssemblyHandler(5)).VersionFull
                    If result.IsEmpty Then
                        result = Assembly.GetCallingAssembly.GetName().Version.ToString
                    End If
                Catch ex As Exception
                    result = "-1"
                End Try
            End If
            Return result
        End Function

    End Class

End Namespace