using Elastic.Apm;
using Elastic.Apm.Api;
//using Elastic.Apm.AspNetFullFramework;
using InMotionGIT.FASI.Trace.Logic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;

namespace InMotionGIT.FASI.Trace.Elasticsearch
{
    /// <summary>
    /// Elasticsearch controller..
    /// </summary>
    [Export(typeof(IController))]
    [ExportMetadata("Name", "ELASTICSEARCH")]
    public class ElasticsearchController : IController
    {
        public void Setup()
        {
        //    var agentComponents = ElasticApmModule.CreateAgentComponents();
        //    Agent.Setup(agentComponents);

        //    // add transaction filter
        //    Agent.AddFilter((ITransaction t) =>
        //    {
        //        t.SetLabel("foo", "bar");
        //        return t;
        //    });
        }

        private Serilog.Core.Logger InicializeLogger()
        {
            Serilog.Core.Logger logger = new LoggerConfiguration().WriteTo.Elasticsearch(ConfigurationManager.AppSettings["Elasticsearch.Uri"], "elk-fasi-index-{0:yyyy.MM}"
            , "fasiTemplate", "fasiLogEventType", 50, 2000, true, Serilog.Events.LogEventLevel.Verbose, string.Concat(ConfigurationManager.AppSettings["Path.Logs"], @"\elasticsearch-logs\elk-serilog"), 5242880)
            .CreateLogger();
            return logger;
        }

        #region TraceLog

        public void TraceLog(string entry)
        {
            Serilog.Core.Logger logger = InicializeLogger();
            logger.Information("Entry: {0}", entry);
        }

        public void TraceLog(string source, string entry)
        {
            Serilog.Core.Logger logger = InicializeLogger();
            logger.Information("Source: {0} Entry: {1}", source, entry);
        }

        public void TraceLog(string source, string entry, string prefix)
        {
            TraceLog(source, entry);
        }

        public void TraceLog(string source, string entry, string prefix, object customData)
        {
            TraceLog(source, entry);
        }

        public void TraceLog(string source, string entry, string prefix, object customData, bool async)
        {
            TraceLog(source, entry);
        }

        #endregion TraceLog

        #region WarningLog

        public void WarningLog(string source, string entry)
        {
            Serilog.Core.Logger logger = InicializeLogger();
            logger.Warning("Source: {0} Entry: {1}", source, entry);
        }

        public void WarningLog(string source, string entry, string prefix)
        {
            WarningLog(source, entry);
        }

        public void WarningLog(string source, string entry, string prefix, object customData)
        {
            WarningLog(source, entry);
        }

        public void WarningLog(string source, string entry, string prefix, object customData, bool async)
        {
            WarningLog(source, entry);
        }

        #endregion WarningLog

        #region ErrorLog

        public void ErrorLog(Exception currentException)
        {
            Agent.Tracer.CaptureException(currentException);
        }

        public void ErrorLog(string source, string entry)
        {
            ErrorLog(source, entry, string.Empty);
        }

        public void ErrorLog(string source, string entry, string prefix)
        {
            ErrorLog errorLog = new ErrorLog(source);
            List<CapturedStackFrame> capturedExceptions = new List<CapturedStackFrame>()
            {  new CapturedStackFrame{
                   FileName = entry
            }};
            errorLog.StackTrace = capturedExceptions;
            Agent.Tracer.CaptureErrorLog(errorLog);
        }

        public void ErrorLog(string source, string entry, Exception currentException)
        {
            ErrorLog(currentException);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix)
        {
            ErrorLog(currentException);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async)
        {
            ErrorLog(currentException);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async, string code)
        {
            ErrorLog(currentException);
        }

        public bool ErrorLogInternal(Dictionary<string, object> parameterInternal)
        { return false; }

        #endregion ErrorLog

        #region PerformanceLog

        public Stopwatch PerformanceLogBegin(string source, string nameMethod, Dictionary<string, object> parameters)
        { return null; }

        public void PerformanceLogFinish(string source, string nameMethod, Stopwatch control)
        { }

        public void PerformanceLog(string source, string nameMethod, bool way, Dictionary<string, object> parametersByData, Stopwatch control)
        { }

        public bool PerformanceLogInternal(Dictionary<string, object> parameterInternal, Dictionary<string, object> parametersByData)
        { return false; }

        #endregion PerformanceLog

        public string GetPath()
        {
            return string.Empty;
        }
    }
}