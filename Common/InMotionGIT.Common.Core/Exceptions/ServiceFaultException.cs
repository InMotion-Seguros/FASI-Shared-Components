using InMotionGIT.Common.Core.Extensions;
using System;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace InMotionGIT.Common.Core.Exceptions
{
    public sealed class ServiceFaultException
    {
        public static InMotionGIT.FASI.Trace.Logic.Connection mefLog = new InMotionGIT.FASI.Trace.Logic.Connection();
        /// <summary>
        /// Returns the instance of 'FaultException' with the original text of the exception / Retorna la instancia de 'FaultException' con el texto original de la exception
        /// </summary>
        /// <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory(Exception inner)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            string code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            string message = string.Empty;
            bool IsDeveloperMode = false;

            if ("Working.Mode".AppSettings("Development").Equals("Development"))
            {
                IsDeveloperMode = true;
            }

            if ("FaultExceptionMessage".AppSettingsExist())
            {
                message = "FaultExceptionMessage".AppSettings();
            }
            else
            {
                message = string.Format("The service encountered a internal error. Error Code: {0}", code);
            }
            FaultException fault;

            switch (inner.GetType())
            {
                case var @case when @case == typeof(DataAccessException):
                    {
                        message = "An error occurred in the data layer";
                        if (IsDeveloperMode)
                        {
                            message += string.Format(". {0}.", inner.Message);
                        }

                        fault = new FaultException(new FaultReason(message), new FaultCode(code, source), source);
                        break;
                    }
                case var case1 when case1 == typeof(InMotionGITException):
                    {
                        message = inner.Message;
                        fault = new FaultException(new FaultReason(message), new FaultCode(code, source), source);
                        break;
                    }

                default:
                    {
                        if (IsDeveloperMode)
                        {
                            message += string.Format(". {0}.", inner.Message);
                        }
                        fault = new FaultException(new FaultReason(message), new FaultCode(code, source), source);
                        mefLog.ErrorLog(source, message, inner, string.Empty, false, code);
                        break;
                    }
            }

            fault.Source = source;

            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'Reason', 'Code' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' , 'Code' y 'Exception'
        /// </summary>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory(string reason, Exception inner)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            string code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            var fault = new FaultException(new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
           mefLog.ErrorLog(source, reason, inner, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'Reason', 'Code' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' , 'Code' y 'Exception'
        /// </summary>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        /// <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory(string reason, string code, Exception inner)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            var fault = new FaultException(new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
           mefLog.ErrorLog(source, reason, inner, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'Reason' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason' y 'Code'
        /// </summary>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory(string reason, string code)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            var fault = new FaultException(new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
           mefLog.ErrorLog(source, reason, null, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'Reason'  / Metodo que realiza un throw de 'FaultException', con los parametros de 'Reason'
        /// </summary>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory(string reason)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            string code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            var fault = new FaultException(new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
           mefLog.ErrorLog(source, reason, null, string.Empty, false, code);
            return fault;
        }

        // '''Generic Implementations

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'TDetail' and 'Exception' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail' y 'Exception'
        /// </summary>
        /// <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        /// <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory<TDetail>(TDetail container, Exception inner)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            string code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            string entry = string.Empty;
            string Message = string.Empty;
            if ("FaultExceptionMessage".AppSettingsExist())
            {
                Message = "FaultExceptionMessage".AppSettings();
            }
            else
            {
                Message = string.Format("The service encountered a internal error. Error Code: {0}", code);
            }
            if ((container.GetType().FullName ?? "") != (container.ToString() ?? ""))
            {
                entry = container.ToString();
            }
            else
            {
                entry = container.GetType().Name;
            }
            var fault = new FaultException<TDetail>(container, new FaultReason(Message), new FaultCode(code, source),source);
            fault.Source = source;
            mefLog.ErrorLog(source, entry, inner, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'TDetail' , 'Reason', 'Exception' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail', 'Reason', 'Exception' y 'Code'
        /// </summary>
        /// <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        /// <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        /// <param name="inner">Exception that perform devel tracking/Exception que se desa realizar tracking.</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory<TDetail>(TDetail container, string reason, string code, Exception inner)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            var fault = new FaultException<TDetail>(container, new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
           mefLog.ErrorLog(source, reason, inner, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'TDetail' , 'Reason' and 'Code' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail', 'Reason' y 'Code'
        /// </summary>
        /// <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        /// <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <param name="Code">The exception identifier for referencing in database or file/Identificador  de la excepcion para posteriores consultas en base de datos o archivos</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory<TDetail>(TDetail container, string reason, string code)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            var fault = new FaultException<TDetail>(container, new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source; 
           mefLog.ErrorLog(source, reason, null, string.Empty, false, code);
            return fault;
        }

        /// <summary>
        /// Method that takes a throw of 'FaultException' with parameters 'TDetail' and 'Reason' / Metodo que realiza un throw de 'FaultException', con los parametros de 'TDetail' y 'Reason'
        /// </summary>
        /// <typeparam name="TDetail">Generic type parameter ' TDetail ' , which allows you to send personalized information about the exception/ Parametro generico de tipo 'TDetail', que permite enviar informacion personalizada de la excepcion </typeparam>
        /// <param name="Container">Custum object containing information for exception / Objeto que contiene informacion custum para la exception</param>
        /// <param name="Reason">Errror message user friendly / Mensaje de errror amigable con el usuario</param>
        /// <returns>Return instance of ' FaultException'/ Retorna Instancia de 'FaultException'</returns>
        /// <remarks></remarks>
        public static FaultException Factory<TDetail>(TDetail container, string reason)
        {
            string source = Assembly.GetCallingAssembly().FullName;
            string code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            var fault = new FaultException<TDetail>(container, new FaultReason(reason), new FaultCode(code, source), source);
            fault.Source = source;
            mefLog.ErrorLog(source, reason, null, string.Empty, false, code);
            return fault;
        }
    }
}