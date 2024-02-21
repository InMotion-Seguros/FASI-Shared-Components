#Region "using"

Imports System.Globalization

#End Region

Namespace Helpers

    Public Class Utility

#Region "Methods"

        ''' <summary>
        ''' Execute a Application of the suite
        ''' </summary>
        ''' <param name="appName">Application Name</param>
        ''' <param name="modelID">Model Identification</param>
        ''' <param name="release">Model Release</param>
        ''' <remarks>
        ''' Form Designer,Workflow Designer,Query Designer,CRUD Form Designer
        '''
        ''' Arguments samples:
        ''' http://robindotnet.wordpress.com/2010/03/21/how-to-pass-arguments-to-an-offline-clickonce-application/
        ''' </remarks>
        Public Shared Sub ExecuteSuiteApp(appName As String, modelID As String, release As Integer)
            Dim exePath As String = String.Format("{0}\InMotionGIT\Ease Designer Workbench\{1}.appref-ms",
                                                  Environment.GetFolderPath(Environment.SpecialFolder.Programs), appName)
            If IO.File.Exists(exePath) Then
                Dim startInfo As New ProcessStartInfo() With {.FileName = exePath,
                                                              .Arguments = String.Format(CultureInfo.InvariantCulture, "?modelid={0}&release={1}", modelID, release)}

                Using processToRun As Process = New Process() With {.StartInfo = startInfo}
                    processToRun.Start()
                End Using
            End If
        End Sub

#End Region

    End Class

End Namespace