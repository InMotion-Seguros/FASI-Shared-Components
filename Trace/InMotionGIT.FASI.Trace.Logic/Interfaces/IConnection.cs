using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InMotionGIT.FASI.Trace.Logic.Interfaces
{
    /// <summary>
    /// Connection interface..
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Method that initializes log handling.
        /// </summary>
        void Setup(string provider);

        #region WarningLog

        /// <summary>
        /// Overload method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <remarks></remarks>
        void WarningLog(string source, string entry);

        /// <summary>
        /// Overload of method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <remarks></remarks>
        void WarningLog(string source, string entry, string prefix);

        /// <summary>
        /// Overload of method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <remarks></remarks>
        void WarningLog(string source, string entry, string prefix, object customData);

        /// <summary>
        /// Warning Log
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="async"></param>
        void WarningLog(string source, string entry, string prefix, object customData, bool async);

        #endregion WarningLog

        #region TraceLog

        /// <summary>
        /// Writes an entry in the logbook file for trace.
        /// </summary>
        /// <param name="entry">Information to be recorded.</param>
        void TraceLog(string entry);

        /// <summary>
        /// Writes an entry in the logbook file for trace.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        void TraceLog(string source, string entry);

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        void TraceLog(string source, string entry, string prefix);

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        void TraceLog(string source, string entry, string prefix, object customData);

        /// <summary>
        /// Overload of method.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="async"></param>
        void TraceLog(string source, string entry, string prefix, object customData, bool async);

        #endregion TraceLog

        #region ErrorLog

        /// <summary>
        /// Error log internal.
        /// </summary>
        /// <param name="parameterInternal">Dictionary with internal parameters.</param>
        /// <returns>Boolean value.</returns>
        bool ErrorLogInternal(Dictionary<string, object> parameterInternal);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="currentException">Exception that gives rise to record.</param>
        void ErrorLog(Exception currentException);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        void ErrorLog(string source, string entry);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="prefix"></param>
        void ErrorLog(string source, string entry, string prefix);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        void ErrorLog(string source, string entry, Exception currentException);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        void ErrorLog(string source, string entry, Exception currentException, string prefix);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        /// <param name="async"></param>
        void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async);

        /// <summary>
        /// Writes an entry in the logbook file for errors.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="entry">Information to be recorded.</param>
        /// <param name="currentException">Exception that gives rise to record.</param>
        /// <param name="prefix"></param>
        /// <param name="async"></param>
        /// <param name="code"></param>
        void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async, string code);

        #endregion ErrorLog

        #region PerformanceLog

        /// <summary>
        /// Writes an entry in the logbook file for trace performance.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="parameters">Parameters for trace performance.</param>
        /// <returns></returns>
        Stopwatch PerformanceLogBegin(string source, string nameMethod, Dictionary<string, object> parameters);

        /// <summary>
        /// Stop logbook file for trace performance.
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="control"></param>
        void PerformanceLogFinish(string source, string nameMethod, Stopwatch control);

        /// <summary>
        /// New record in performance log
        /// </summary>
        /// <param name="source">Key associated with the originating source registration.</param>
        /// <param name="nameMethod">Name of method for registration.</param>
        /// <param name="way"></param>
        /// <param name="parametersByData"></param>
        /// <param name="control"></param>
        void PerformanceLog(string source, string nameMethod, bool way, Dictionary<string, object> parametersByData, Stopwatch control);

        /// <summary>
        /// New record in internal performance log
        /// </summary>
        /// <param name="parameterInternal">Internal parameter.</param>
        /// <param name="parametersByData"></param>
        bool PerformanceLogInternal(Dictionary<string, object> parameterInternal, Dictionary<string, object> parametersByData);

        #endregion PerformanceLog

        /// <summary>
        /// Get path as files are stored.
        /// </summary>
        /// <param name="provider">Controller provider type.</param>
        /// <returns></returns>
        string GetPath();
    }
}