using InMotionGIT.FASI.Trace.Logic.Interfaces; 
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;

namespace InMotionGIT.FASI.Trace.Logic
{
    /// <summary>
    /// Connection interface.
    /// </summary>
    public class Connection : IConnection
    {
        private readonly CompositionContainer Container;

        /// <summary>
        /// List of controller.
        /// </summary>
        [ImportMany]
        public List<Lazy<IController, IControllerMetadata>> Controllers { get; set; }

        /// <summary>
        /// Define el provider que se va usar en el proceso
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">Controller provider type.</param>
        public Connection(string provider = "")
        {
            if (provider == string.Empty)
                provider = ConfigurationManager.AppSettings["FASI.Debug.Mode"];
            this.Provider = provider;
            var catalogo = new AggregateCatalog();
            var path = ConfigurationManager.AppSettings["FASI.Path.Drivers"];
            catalogo.Catalogs.Add(new DirectoryCatalog(path));
            Container = new CompositionContainer(catalogo);
            Container.ComposeParts(this);
        }

        private Lazy<IController, IControllerMetadata> Find(string provider)
        {
            Lazy<IController, IControllerMetadata> result = null;
            foreach (Lazy<IController, IControllerMetadata> controller in Controllers)
            {
                if (controller.Metadata.Name.Equals(provider.ToUpper()))
                {
                    result = controller;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Method that initializes log handling.
        /// </summary>
        public void Setup(string provider)
        {
            Lazy<IController, IControllerMetadata> result = Find(provider);
            result?.Value?.Setup();
        }

        #region WarningLog

        /// <summary>
        /// Overload method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <remarks></remarks>
        public void WarningLog(string source, string entry)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.WarningLog(source, entry);
        }

        /// <summary>
        /// Overload of method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <remarks></remarks>
        public void WarningLog(string source, string entry, string prefix)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.WarningLog(source, entry, prefix);
        }

        /// <summary>
        /// Overload of method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <remarks></remarks>
        public void WarningLog(string source, string entry, string prefix, object customData)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.WarningLog(source, entry, prefix, customData);
        }

        /// <summary>
        /// Warning Log
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="async"></param>
        public void WarningLog(string source, string entry, string prefix, object customData, bool async)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.WarningLog(source, entry, prefix, customData, async);
        }

        #endregion WarningLog

        #region TraceLog

        /// <summary>
        /// Writes an entry in the logbook file for trace.
        /// </summary>
        /// <param name="entry">Information to be recorded.</param>
        public void TraceLog(string entry)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.TraceLog(entry);
        }

        /// <summary>
        /// Writes an entry in the logbook file for trace.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        public void TraceLog(string source, string entry)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.TraceLog(source, entry);
        }

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        public void TraceLog(string source, string entry, string prefix)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.TraceLog(source, entry, prefix);
        }

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        public void TraceLog(string source, string entry, string prefix, object customData)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.TraceLog(source, entry, prefix, customData);
        }

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="async"></param>
        public void TraceLog(string source, string entry, string prefix, object customData, bool async)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.TraceLog(source, entry, prefix, customData, async);
        }

        #endregion TraceLog

        #region ErrorLog

        /// <summary>
        /// Error log internal.
        /// </summary>

        /// <param name="parameterInternal">Dictionary with internal parameters.</param>
        /// <returns>Boolean value.</returns>
        public bool ErrorLogInternal(Dictionary<string, object> parameterInternal)
        {
            bool result = false;
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLogInternal(parameterInternal);
            return result;
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="currentException">Exception that gives rise to record.</param>
        public void ErrorLog(Exception currentException)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(currentException);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        public void ErrorLog(string source, string entry)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        public void ErrorLog(string source, string entry, string prefix)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry, prefix);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        public void ErrorLog(string source, string entry, Exception currentException)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry, currentException);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>

        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        public void ErrorLog(string source, string entry, Exception currentException, string prefix)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry, currentException, prefix);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        /// <param name="async"></param>
        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry, currentException, prefix, async);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        /// <param name="async"></param>
        /// <param name="code"></param>
        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async, string code)
        {
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            resultController?.Value?.ErrorLog(source, entry, currentException, prefix, async, code);
        }

        #endregion ErrorLog

        #region PerformanceLog

        /// <summary>
        /// Start logbook file for trace performance.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="parameters">Parameters for trace performance.</param>
        /// <returns></returns>
        public Stopwatch PerformanceLogBegin(string source, string nameMethod, Dictionary<string, object> parameters)
        {
            Stopwatch result = null;
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            result = resultController?.Value?.PerformanceLogBegin(source, nameMethod, parameters);
            return result;
        }

        /// <summary>
        /// Stop logbook file for trace performance.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="control"></param>
        public void PerformanceLogFinish(string source, string nameMethod, Stopwatch control)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.PerformanceLogFinish(source, nameMethod, control);
        }

        /// <summary>
        /// New record in performance log
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="way"></param>
        /// <param name="parametersByData"></param>
        /// <param name="control"></param>
        public void PerformanceLog(string source, string nameMethod, bool way, Dictionary<string, object> parametersByData, Stopwatch control)
        {
            Lazy<IController, IControllerMetadata> result = Find(Provider);
            result?.Value?.PerformanceLog(source, nameMethod, way, parametersByData, control);
        }

        /// <summary>
        /// New record in internal performance log
        /// </summary>
        /// <param name="parameterInternal">Internal parameter.</param>
        /// <param name="parametersByData"></param>
        public bool PerformanceLogInternal(Dictionary<string, object> parameterInternal, Dictionary<string, object> parametersByData)
        {
            bool result = false;
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            result = (bool)(resultController?.Value?.PerformanceLogInternal(parameterInternal, parametersByData));
            return result;
        }

        #endregion PerformanceLog

        /// <summary>
        /// Get path as files are stored.
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string result = string.Empty;
            Lazy<IController, IControllerMetadata> resultController = Find(Provider);
            result = resultController?.Value?.GetPath();
            return result;
        }
    }
}