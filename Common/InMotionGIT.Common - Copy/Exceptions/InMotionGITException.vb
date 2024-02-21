Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Security.Permissions

Namespace Exceptions

    <Serializable()>
    Public Class InMotionGITException
        Inherits Exception
        Implements ISerializable

        Private _InvalidFields As Collection

        Protected Sub New(ByVal serializationInfo As SerializationInfo, ByVal streamingContext As StreamingContext)
            MyBase.New(serializationInfo, streamingContext)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
            Helpers.LogHandler.ErrorLog("InMotionGITException", message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
            Helpers.LogHandler.ErrorLog("InMotionGITException", message, inner)
        End Sub

        Public ReadOnly Property InvalidFields() As Collection
            Get
                InvalidFields = _InvalidFields
            End Get
        End Property

        Public Shared Function ShowError(ByVal ex As Exception) As String
            MakeLog(ex)

            Select Case ex.GetType.Name
                Case "InvalidCastException"
                    Return "Error: Input string was not in a correct format."
                Case "NullReferenceException"
                    Return "Error: Object reference not set to an instance of an object."
                Case Else
                    Return "Unexpected error in the system. Verify with your administrator"
            End Select
        End Function

        Friend Shared Sub MakeLog(ByVal ex As Exception)
            Dim file As System.IO.StreamWriter
            file = My.Computer.FileSystem.OpenTextFileWriter("c:\test.txt", True)
            file.WriteLine(String.Format("|-------------------{0}---------------------------|{1}", Date.Now.ToLocalTime.ToString(CultureInfo.CurrentCulture), vbCrLf))
            file.WriteLine(String.Format("Message: {0}{1}", ex.Message, vbCrLf))
            If Not ex.InnerException Is Nothing Then
                file.WriteLine(String.Format("InnerException: {0}{1}", ex.InnerException.Message, vbCrLf))
            End If
            file.WriteLine(String.Format("Data: {0}{1}", ex.Data, vbCrLf))
            file.WriteLine(String.Format("StackTrace: {0}{1}", ex.StackTrace, vbCrLf))
            file.WriteLine(String.Format("Source: {0}{1}", ex.Source, vbCrLf))
            If ex.HelpLink <> Nothing Then
                file.WriteLine(String.Format("HelpLink: {0}{1}", ex.HelpLink, vbCrLf))
            End If
            file.WriteLine("|----------------------------------------------------------------------|" + vbCrLf)
            file.Close()
        End Sub

        <SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.SerializationFormatter)>
        Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) _
            'Implements ISerializable.GetObjectData
            MyBase.GetObjectData(info, context)
            Throw New ArgumentNullException("info")

        End Sub

    End Class

End Namespace