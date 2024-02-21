#Region "Imports"

Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates

#End Region

Namespace Helpers

    ''' <summary>
    '''Class allows certified operations - Clase permite realizar operaciones sobre certificados
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Certificate

        ''' <summary>
        '''Handler that allows a self-certification, in this case self-signed cerificados - Manejador que permite realizar una auto certificación, en este caso de cerificados autofirmados
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub OverrideCertificateValidation()
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf Certificate.RemoteCertValidate)
        End Sub

        ''' <summary>
        ''' Method that allows the validation of 'ServicePointManager' - Metodo que permite la validación sobre 'ServicePointManager'
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="cert"></param>
        ''' <param name="chain"></param>
        ''' <param name="error"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function RemoteCertValidate(ByVal sender As Object, ByVal cert As X509Certificate, ByVal chain As X509Chain, ByVal [error] As SslPolicyErrors) As Boolean
            Return True
        End Function

    End Class

End Namespace