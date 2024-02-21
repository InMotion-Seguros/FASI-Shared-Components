Imports System.Configuration
Imports System.Reflection
Imports System.ServiceModel

Namespace Exceptions

    Public NotInheritable Class ServiceFaultException

        ''' <summary>
        ''' Returns the instance of 'FaultException' with the original text of the exception / Retorna la instancia de 'FaultException' con el texto original de la exception
        ''' </summary>
        ''' <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(inner As Exception) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim code As String = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            Dim message As String = String.Empty
            Dim IsDeveloperMode As Boolean = False

            If ConfigurationManager.AppSettings("Working.Mode").IsNotEmpty AndAlso
               ConfigurationManager.AppSettings("Working.Mode").Equals("Development", StringComparison.CurrentCultureIgnoreCase) Then
                IsDeveloperMode = True
            End If

            If ConfigurationManager.AppSettings("FaultExceptionMessage").IsNotEmpty Then
                message = ConfigurationManager.AppSettings("FaultExceptionMessage")
            Else
                message = String.Format("The service encountered a internal error. Error Code: {0}", code)
            End If
            Dim fault As FaultException

            Select Case inner.GetType
                Case GetType(DataAccessException)
                    message = "An error occurred in the data layer"
                    If IsDeveloperMode Then
                        message &= String.Format(". {0}.", inner.Message)
                    End If

                    fault = New FaultException(message, New System.ServiceModel.FaultCode(code, source))
                Case GetType(Exceptions.InMotionGITException)
                    message = inner.Message
                    fault = New FaultException(message, New System.ServiceModel.FaultCode(code, source))
                Case Else
                    If IsDeveloperMode Then
                        message &= String.Format(". {0}.", inner.Message)
                    End If
                    fault = New FaultException(message, New System.ServiceModel.FaultCode(code, source))
                    Helpers.LogHandler.ErrorLog(source, message, inner, String.Empty, False, code)
            End Select

            fault.Source = source

            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'Reason', 'Code' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' , 'Code' y 'Exception'
        ''' </summary>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(reason As String, inner As Exception) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim code As String = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            Dim fault As New FaultException(New System.ServiceModel.FaultReason(reason), New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, inner, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'Reason', 'Code' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' , 'Code' y 'Exception'
        ''' </summary>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        ''' <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(reason As String, code As String, inner As Exception) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim fault As New FaultException(New System.ServiceModel.FaultReason(reason), New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, inner, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'Reason' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' y 'Code'
        ''' </summary>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(reason As String, code As String) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim fault As New FaultException(New System.ServiceModel.FaultReason(reason), New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, Nothing, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'Reason'  / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason'
        ''' </summary>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(reason As String) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim code As String = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            Dim fault As New FaultException(reason, New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, Nothing, String.Empty, False, code)
            Return fault
        End Function

        ''''Generic Implementations

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'TDetail' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail' y 'Exception'
        ''' </summary>
        ''' <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        ''' <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(Of TDetail)(container As TDetail, inner As Exception) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim code As String = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            Dim entry As String = String.Empty
            Dim Message As String = String.Empty
            If ConfigurationManager.AppSettings("FaultExceptionMessage").IsNotEmpty Then
                Message = ConfigurationManager.AppSettings("FaultExceptionMessage")
            Else
                Message = String.Format("The service encountered a internal error. Error Code: {0}", code)
            End If
            If container.GetType.FullName <> container.ToString Then
                entry = container.ToString
            Else
                entry = container.GetType.Name
            End If
            Dim fault As New FaultException(Of TDetail)(container, Message, New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, entry, inner, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'TDetail' , 'Reason', 'Exception' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail', 'Reason', 'Exception' y 'Code'
        ''' </summary>
        ''' <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        ''' <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        ''' <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(Of TDetail)(container As TDetail, reason As String, code As String, inner As Exception) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim fault As New FaultException(Of TDetail)(container, New System.ServiceModel.FaultReason(reason), New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, inner, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'TDetail' , 'Reason' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail', 'Reason' y 'Code'
        ''' </summary>
        ''' <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        ''' <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(Of TDetail)(container As TDetail, reason As String, code As String) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim fault As New FaultException(Of TDetail)(container, New System.ServiceModel.FaultReason(reason), New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, Nothing, String.Empty, False, code)
            Return fault
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'FaultException' with parameters 'TDetail' and 'Reason' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail' y 'Reason'
        ''' </summary>
        ''' <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        ''' <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        ''' <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        ''' <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        ''' <remarks></remarks>
        Public Shared Function Factory(Of TDetail)(container As TDetail, reason As String) As FaultException
            Dim source As String = Assembly.GetCallingAssembly().FullName
            Dim code As String = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff")
            Dim fault As New FaultException(Of TDetail)(container, reason, New System.ServiceModel.FaultCode(code, source))
            fault.Source = source
            Helpers.LogHandler.ErrorLog(source, reason, Nothing, String.Empty, False, code)
            Return fault
        End Function

    End Class

End Namespace