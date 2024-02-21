using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    public sealed class LogHandler
    {

        #region Properties

        private static string _NameFile;

        public static string NameFile
        {
            get
            {
                return _NameFile;
            }
            set
            {
                _NameFile = value;
            }
        }

        #endregion

        #region WarningLog

        /// <summary>
        /// Overload method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <remarks></remarks>
        public static void WarningLog(string source, string entry)
        {
            WarningLog(source, entry, string.Empty);
        }

        /// <summary>
        /// Overload of method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <param name="prefix"></param>
        /// <remarks></remarks>
        public static void WarningLog(string source, string entry, string prefix)
        {
            WarningLog(source, entry, prefix, null);
        }

        /// <summary>
        /// Overrable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <remarks></remarks>
        public static void WarningLog(string source, string entry, string prefix, object customData)
        {
            WarningLog(source, entry, prefix, customData, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="Async"></param>
        /// <remarks></remarks>
        public static void WarningLog(string source, string entry, string prefix, object customData, bool Async)
        {
            string logprefix = "Logs.Prefix".StringValue();
            string filename = string.Empty;
            string DebugMode = string.Empty;

            string rootPath = GetPath();

            if (string.IsNullOrEmpty(logprefix))
            {
                filename = string.Format(@"{0}\{1:yyyyMMdd}.Warning.log", rootPath, DateTime.Now);
            }
            else
            {
                filename = string.Format(@"{0}\{1:yyyyMMdd}.Warning.{2}.log", rootPath, DateTime.Now, logprefix);
            }

            NameFile = filename;

            DebugMode = "FrontOffice.Debug.Mode".StringValue("File");

            var parameters = new Dictionary<string, object>();
            var _DateEfective = DateTime.Now;
            parameters.Add("DateEfective", _DateEfective.ToString("hh:mm:ss.fff"));
            parameters.Add("FileName", filename);
            parameters.Add("Source", source);
            parameters.Add("Entry", entry);
            parameters.Add("ThreadId", Thread.CurrentThread.ManagedThreadId);
            parameters.Add("DebugMode", DebugMode);
            parameters.Add("CustomData", customData);
            if (!(HttpContext.Current == null) && !(HttpContext.Current.Session == null))
            {
                parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
            }
            else
            {
                parameters.Add("SessionID", "without session id");
            }
            if (Async)
            {
                var AddUsersSecurityTraceAsyn = new Task(actionWarningLog, parameters);
                AddUsersSecurityTraceAsyn.Start();
            }
            else
            {
                while (WarningLogInternal(parameters) == false)
                    Thread.Sleep(800);
            }
        }

        public static Action<object> actionWarningLog = (parameterContainer) =>
{
Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
int attempts = 1;
while (WarningLogInternal(parameterInternal) == false && attempts < 4)
{
Thread.Sleep(800);
attempts += 1;
}
};

        /// <summary>
        /// Method of the Internal WarnningLog
        /// </summary>
        /// <param name="parameterInternal"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool WarningLogInternal(Dictionary<string, object> parameterInternal)
        {
            bool Result = false;
            string _FileName = parameterInternal["FileName"].ToString();
            string _IP = Connection.GetIPRequest();
            string _Source = parameterInternal["Source"].ToString();
            string _Entry = parameterInternal["Entry"].ToString();
            string _DateEfective = parameterInternal["DateEfective"].ToString();
            string _ThreadId = parameterInternal["ThreadId"].ToString();
            string _SessionID = parameterInternal["SessionID"].ToString();
            string _DebugMode = parameterInternal["DebugMode"].ToString();
            var _CustomData = parameterInternal["CustomData"];
            FileStream fs = null;
            try
            {

                if (File.Exists(_FileName))
                {
                    fs = File.Open(_FileName, FileMode.Append);
                }
                else
                {
                    fs = File.Create(_FileName);
                }

                var sw = new StreamWriter(fs);
                var thisDate = DateTime.Now;

                sw.WriteLine(string.Format("{0} {3} {4} {5} {1} {2}", _DateEfective, _Source, _Entry, Connection.NameHostFromIp(_IP), _ThreadId, _SessionID));
                sw.Close();

                fs.Close();

                if (!_DebugMode.Equals("File"))
                {

                    var temporalLog = new EventLogClient.EventLog();
                    temporalLog.FactTime = DateTime.Now;
                    temporalLog.HostSource = Connection.NameHostFromIp(_IP);
                    temporalLog.TypeTrace = Convert.ToInt32((int)Enumerations.EnumTraceType.Warrning);
                    temporalLog.Source = _Source;
                    temporalLog.Entry = string.Format("{0}{1} Seccion Id:{2}", _Entry, Constants.vbLf, _SessionID);
                    if (!(_CustomData == null) && Serializer.IsSerializable(_CustomData))
                    {
                        string dataCustom = Serializer.SerializarObject(_CustomData);
                        temporalLog.EventLogDetail = new EventLogClient.EventLogDetail() { Detail = dataCustom };
                    }
                    using (var EventLogClient = new EventLogClient.ManagerClient())
                    {
                        EventLogClient.LogSave(temporalLog);
                    }
                }
                Result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                {
                    Result = false;
                }
                else
                {
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Result = true;
            }
            finally
            {
                if (fs is not null)
                {
                    fs.Close();
                }
            }
            return Result;
        }

        #endregion

        #region PerformanceLog

        /// <summary>
        /// Writes an entry in the logbook file for trace
        /// </summary>
        /// <param name="nameMethod">Nombre del método que llama</param>
        public static Stopwatch PerformanceLogBegin(string source, string nameMethod, Dictionary<string, object> parameters)
        {
            var result = new Stopwatch();
            result.Start();

            PerformanceLog(source, nameMethod, true, parameters, null);
            return result;
        }

        public static void PerformanceLogFinish(string source, string nameMethod, Stopwatch control)
        {
            PerformanceLog(source, nameMethod, false, null, control);
        }

        internal static void PerformanceLog(string source, string nameMethod, bool way, Dictionary<string, object> parametersByData, Stopwatch control)
        {

            string prefix = "Performance.Prefix".StringValue();
            string filename;
            var parameters = new Dictionary<string, object>();
            if (prefix.IsNotEmpty())
            {
                string rootPath = GetPath();
                parameters.Add("DateEfective", DateTime.Now);
                parameters.Add("Source", source);
                parameters.Add("Method", nameMethod);
                filename = string.Format(@"{0}\{1:yyyyMMdd}.{2}.log", rootPath, DateTime.Now, prefix);
                parameters.Add("FileName", filename);
                parameters.Add("Way", way);
                if (!(HttpContext.Current == null) && !(HttpContext.Current.Session == null))
                {
                    parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
                }
                else
                {
                    parameters.Add("SessionID", "without session id");
                }
                if (!way | control.IsNotEmpty())
                {
                    control.Stop();
                    parameters.Add("Elapsed", control.Elapsed);
                }

                int attempts = 1;
                while (PerformanceLogInternal(parameters, parametersByData) == false && attempts < 4)
                {
                    Thread.Sleep(800);
                    attempts += 1;
                }
            }

        }

        private static bool PerformanceLogInternal(Dictionary<string, object> parameterInternal, Dictionary<string, object> parametersByData)
        {
            bool result = false;

            FileStream fs = null;

            try
            {
                string _FileName = parameterInternal["FileName"].ToString();
                string _SessionID = parameterInternal["SessionID"].ToString();
                bool _way = bool.Parse(parameterInternal["Way"].ToString());
                string _source = parameterInternal["Source"].ToString();
                string _time_elapsed = string.Empty;
                if (!_way)
                {
                    _time_elapsed = parameterInternal["Elapsed"].ToString();
                    if (_time_elapsed.IsNotEmpty())
                    {
                        _time_elapsed = string.Format("{0}Time elapsed in milliseconds:{0},{0}{1}{0}", '"', _time_elapsed);
                    }
                }

                DateTime _DateEfective = Conversions.ToDate(parameterInternal["DateEfective"]);
                string _Method = parameterInternal["Method"].ToString();
                if (File.Exists(_FileName))
                {
                    fs = File.Open(_FileName, FileMode.Append);
                }
                else
                {
                    fs = File.Create(_FileName);
                }

                var sw = new StreamWriter(fs);
                string values = string.Empty;
                if (parametersByData.IsNotEmpty())
                {
                    var valuesArray = new List<string>();
                    foreach (string key in parametersByData.Keys)
                    {
                        if (parametersByData[key] is null)
                        {
                            valuesArray.Add(string.Format("{0}='{1}'", key, "Empty"));
                        }
                        else
                        {
                            valuesArray.Add(string.Format("{0}='{1}'", key, parametersByData[key].ToString()));
                        }
                    }

                    values = string.Join("; ", valuesArray);
                }
                if (_way)
                {
                    sw.WriteLine(string.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{0}{6}{0}", '"', _DateEfective.ToString("hh:mm:ss.fff"), _SessionID, "Begin ", _source, _Method, Interaction.IIf(values.IsEmpty(), "", values)));
                }
                else
                {
                    sw.WriteLine(string.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{6}", '"', _DateEfective.ToString("hh:mm:ss.fff"), _SessionID, "Finish", _source, _Method, _time_elapsed));
                }
                sw.Close();
                fs.Close();
                result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {

                result = true;
            }
            finally
            {
                if (fs is not null)
                {
                    fs.Close();
                }
            }

            return result;
        }

        #endregion

        #region TraceLog

        /// <summary>
        /// Writes an entry in the logbook file for trace
        /// </summary>
        /// <param name="entry">Information to be recorded</param>
        public static void TraceLog(string entry)
        {
            TraceLog(AssemblyHandler.GetFrameProcessFullName(2), entry, "Trace");
        }

        /// <summary>
        /// Writes an entry in the logbook file for trace
        /// </summary>
        /// <param name="source">Key associated with the originating source registration</param>
        /// <param name="entry">Information to be recorded</param>
        public static void TraceLog(string source, string entry)
        {
            TraceLog(source, entry, "Trace");
        }

        public static void TraceLog(string source, string entry, string prefix)
        {
            TraceLog(source, entry, prefix, null);
        }

        /// <summary>
        /// Overload method
        /// </summary>
        /// <param name="source">Key associated with the originating source registration</param>
        /// <param name="entry">Information to be recorded</param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <remarks></remarks>
        public static void TraceLog(string source, string entry, string prefix, object customData)
        {
            TraceLog(source, entry, prefix, customData, true);
        }

        /// <summary>
        /// Method Base of the TraceLog,
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <param name="prefix"></param>
        /// <param name="customData"></param>
        /// <param name="Async"></param>
        /// <remarks></remarks>
        public static void TraceLog(string source, string entry, string prefix, object customData, bool Async)
        {
            string logprefix = "Logs.Prefix".StringValue();
            string filename = string.Empty;
            string DebugMode = string.Empty;

            string rootPath = GetPath();

            if ("FrontOffice.Debug".IsTrue())
            {

                if (string.IsNullOrEmpty(logprefix))
                {
                    filename = string.Format(@"{0}\{1:yyyyMMdd}.{2}.log", rootPath, DateTime.Now, prefix);
                }
                else
                {
                    filename = string.Format(@"{0}\{1:yyyyMMdd}.{2}.{3}.log", rootPath, DateTime.Now, prefix, logprefix);
                }

                NameFile = filename;

                DebugMode = "FrontOffice.Debug.Mode".StringValue("File");

                var parameters = new Dictionary<string, object>();
                var _DateEfective = DateTime.Now;
                parameters.Add("DateEfective", _DateEfective.ToString("hh:mm:ss.fff"));
                parameters.Add("FileName", filename);
                parameters.Add("Source", source);
                parameters.Add("Entry", entry);
                parameters.Add("ThreadId", Thread.CurrentThread.ManagedThreadId);
                parameters.Add("DebugMode", DebugMode);
                parameters.Add("CustomData", customData);
                if (!(HttpContext.Current == null) && !(HttpContext.Current.Session == null))
                {
                    parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
                }
                else
                {
                    parameters.Add("SessionID", "without session id");
                }

                bool modeMultiThread = true;
                if ("FrontOffice.Trace.Mode".StringValue() == "Single")
                {
                    modeMultiThread = false;
                }
                if (modeMultiThread)
                {
                    if (Async)
                    {
                        var AddUsersSecurityTraceAsyn = new Task(actionTraceLog, parameters);
                        AddUsersSecurityTraceAsyn.Start();
                    }
                    else
                    {
                        while (TraceLogInternal(parameters) == false)
                            Thread.Sleep(800);
                    }
                }
                else
                {
                    TraceLogInternal(parameters);
                }

            }
        }

        public static Action<object> actionTraceLog = (parameterContainer) =>
{
Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
int attempts = 1;
while (TraceLogInternal(parameterInternal) == false && attempts < 4)
{
Thread.Sleep(800);
attempts += 1;
}
};

        public static string GetFileName()
        {
            string result = string.Empty;
            result = GetPath();
            return result;
        }

        private static string GetPath()
        {
            string rootPath = "Path.Logs".StringValue();

            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Directory.GetPathRoot();
            }
            else if (rootPath.ToLower().Contains("special"))
            {
                if (rootPath.ToLower().Equals("special"))
                {
                    rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    string temporal = string.Format("{0}{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), rootPath.Replace("special", string.Empty).Replace("@", @"\"));
                    rootPath = temporal;
                }
            }

            return rootPath;
        }

        /// <summary>
        /// Internal method of the Tracelog
        /// </summary>
        /// <param name="pParamters"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool TraceLogInternal(Dictionary<string, object> pParamters)
        {
            bool Result = false;
            var parameterInternal = pParamters;
            FileStream fs = null;
            string _FileName;
            string _IP;
            string _Source;
            string _Entry;
            string _DateEfective;
            string _ThreadId;
            string _SessionID;
            string _DebugMode;
            object _CustomData;
            _FileName = parameterInternal["FileName"].ToString();
            _IP = Connection.GetIPRequest();
            _Source = parameterInternal["Source"].ToString();
            _Entry = parameterInternal["Entry"].ToString();
            _DateEfective = Conversions.ToString(parameterInternal["DateEfective"]);
            _ThreadId = parameterInternal["ThreadId"].ToString();
            _SessionID = parameterInternal["SessionID"].ToString();
            _DebugMode = parameterInternal["DebugMode"].ToString();
            _CustomData = parameterInternal["CustomData"];

            try
            {
                if (File.Exists(_FileName))
                {
                    fs = File.Open(_FileName, FileMode.Append);
                }
                else
                {
                    fs = File.Create(_FileName);
                }

                var sw = new StreamWriter(fs);
                sw.WriteLine(string.Format("{0} {3} {4} {5} {1} {2} ", _DateEfective, _Source, _Entry, Connection.NameHostFromIp(_IP), _ThreadId, _SessionID));
                sw.Close();
                fs.Close();
                if (!_DebugMode.Equals("File"))
                {
                    using (var EventLogClient = new EventLogClient.ManagerClient())
                    {
                        var temporalLog = new EventLogClient.EventLog();
                        temporalLog.FactTime = DateTime.Now;
                        temporalLog.HostSource = Connection.NameHostFromIp(_IP);
                        temporalLog.TypeTrace = Convert.ToInt32((int)Enumerations.EnumTraceType.Trace);
                        temporalLog.Source = _Source;
                        temporalLog.Entry = string.Format("{0}{1} Seccion Id:{2}", _Entry, Constants.vbLf, _SessionID);
                        if (!(_CustomData == null) && Serializer.IsSerializable(_CustomData))
                        {
                            string dataCustom = Serializer.SerializarObject(_CustomData);
                            temporalLog.EventLogDetail = new EventLogClient.EventLogDetail() { Detail = dataCustom };
                        }
                        EventLogClient.LogSave(temporalLog);
                    }

                }
                Result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                {
                    Result = false;
                }
                else
                {
                    Result = true;
                }
            }
            catch (Exception ex)
            {

                Result = true;
            }
            finally
            {
                if (fs is not null)
                {
                    fs.Close();
                }
            }
            return Result;
        }

        #endregion

        #region ErrorLog

        public static bool ErrorLogInternal(Dictionary<string, object> parameterInternal)
        {
            bool Result = false;
            FileStream fs = null;
            string _fileName = string.Empty;
            string _DateEfective;
            string _IP = string.Empty;
            string _Source = string.Empty;
            Exception _CurrentException;
            string _Entry = string.Empty;
            StackTrace _ExceptionStack;
            StringBuilder _ServerVariables;
            string _DebugMode = string.Empty;
            string _SessionID = string.Empty;
            string _Code = string.Empty;
            _Code = Conversions.ToString(parameterInternal["Code"]);
            _fileName = Conversions.ToString(parameterInternal["FileName"]);
            _DateEfective = Conversions.ToString(parameterInternal["DateEfective"]);
            _IP = Connection.GetIPOnlyRequest();
            _Source = Conversions.ToString(parameterInternal["Source"]);
            _CurrentException = (Exception)parameterInternal["CurrentException"];
            _Entry = Conversions.ToString(parameterInternal["Entry"]);
            _ExceptionStack = (StackTrace)parameterInternal["ExceptionStack"];
            _ServerVariables = (StringBuilder)parameterInternal["ServerVariables"];
            _DebugMode = parameterInternal["DebugMode"].ToString();
            _SessionID = Conversions.ToString(parameterInternal["SessionID"]);

            try
            {

                if (File.Exists(_fileName))
                {
                    fs = File.Open(_fileName, FileMode.Append);
                }
                else
                {
                    fs = File.Create(_fileName);
                }

                var sw = new StreamWriter(fs);
                string stringTemporal = string.Format("{0} {1} {2}", _IP, Connection.NameHostFromIp(_IP), _Source);
                sw.WriteLine(stringTemporal);
                if (!(_CurrentException == null))
                {
                    TraceInnerExceptionMessage2(_CurrentException, 1, sw);
                    TraceInnerExceptionData(_CurrentException, 1, sw);
                }
                sw.WriteLine(string.Format(" Code:{0}", _Code));
                if (_Entry.IsNotEmpty())
                {
                    sw.WriteLine(" Entry: " + _Entry);
                }
                sw.WriteLine(" Session Id:" + _SessionID);

                if (!(_ServerVariables == null) && !string.IsNullOrEmpty(_ServerVariables.ToString()))
                {
                    sw.Write(_ServerVariables.ToString());
                }

                if (_CurrentException is not null)
                {
                    var ExceptionStack = new StackTrace(_CurrentException, true);
                    StackFrame[] _VectorStack = ExceptionStack.GetFrames();
                    if (_VectorStack is not null)
                    {
                        int _Index = _VectorStack.Length;
                        if (_Index - (_Index - 1) > 0)
                        {
                            var ExceptionFrame = ExceptionStack.GetFrame(_Index - 1);
                            sw.Write("Method: " + ExceptionFrame.GetMethod().ToString());
                            sw.WriteLine(string.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()));

                            sw.WriteLine(" Class: " + ExceptionFrame.GetMethod().DeclaringType.ToString());
                            sw.WriteLine("  File: " + ExceptionFrame.GetFileName());
                            sw.WriteLine(" Stack:");
                            sw.WriteLine(ExceptionStack.ToString().Replace("   at ", "  "));
                        }
                    }
                }
                else
                {
                    StackFrame[] stat = _ExceptionStack.GetFrames();
                    var ExceptionFrame = stat[2];
                    sw.Write("Method: " + ExceptionFrame.GetMethod().ToString());
                    sw.WriteLine(string.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()));
                    sw.WriteLine(string.Format(" Class: {0}", ExceptionFrame.GetMethod().DeclaringType));
                    sw.WriteLine(string.Format("  File: {0}", ExceptionFrame.GetFileName()));
                    sw.WriteLine(" Stack:");
                    sw.WriteLine(_ExceptionStack.ToString().Replace("   at ", "  "));
                }
                sw.Close();
                fs.Close();

                if (!_DebugMode.Equals("File"))
                {
                    var sr = new StreamReader(_fileName);
                    using (var EventLogClient = new EventLogClient.ManagerClient())
                    {
                        var temporalLog = new EventLogClient.EventLog();
                        temporalLog.Code = _Code;
                        temporalLog.FactTime = DateTime.Now;
                        temporalLog.HostSource = Connection.NameHostFromIp(_IP);
                        temporalLog.TypeTrace = Convert.ToInt32((int)Enumerations.EnumTraceType.Error);
                        temporalLog.Source = _Source;
                        temporalLog.Entry = string.Format("{0}{1} Seccion Id:{2}", _Entry, Constants.vbLf, _SessionID);
                        temporalLog.EventLogDetail = new EventLogClient.EventLogDetail() { Detail = sr.ReadToEnd() };
                        sr.Close();
                        EventLogClient.LogSave(temporalLog);
                    }
                }
                Result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                {
                    Result = false;
                }
                else
                {
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Result = true;
            }
            finally
            {
                if (fs is not null)
                {
                    fs.Close();
                }
            }
            return Result;
        }

        public static void ErrorLog(Exception currentException)
        {
            ErrorLog(AssemblyHandler.GetFrameProcessFullName(2), string.Empty, currentException, string.Empty);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors
        /// </summary>
        /// <param name="source">Key associated with the originating source registration</param>
        /// <param name="entry">Information to be recorded</param>
        public static void ErrorLog(string source, string entry)
        {
            ErrorLog(source, entry, null, string.Empty);
        }

        public static void ErrorLog(string source, string entry, Exception currentException)
        {
            ErrorLog(source, entry, currentException, string.Empty);
        }

        public static void ErrorLog(string source, string entry, Exception currentException, string prefix)
        {
            ErrorLog(source, entry, currentException, prefix, true, string.Empty);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors
        /// </summary>
        /// <param name="source">Key associated with the originating source registration</param>
        /// <param name="entry">Information to be recorded</param>
        /// <param name="currentException">Exception that gives rise to record</param>
        public static void ErrorLog(string source, string entry, Exception currentException, string prefix, bool Async)
        {
            ErrorLog(source, entry, currentException, prefix, false, string.Empty);
        }

        /// <summary>
        /// Writes an entry in the logbook file for errors
        /// </summary>
        /// <param name="source">Key associated with the originating source registration</param>
        /// <param name="entry">Information to be recorded</param>
        /// <param name="currentException">Exception that gives rise to record</param>
        /// <param name="prefix"></param>
        /// <param name="Async"></param>
        /// <param name="Code"></param>
        /// <remarks></remarks>
        public static void ErrorLog(string source, string entry, Exception currentException, string prefix, bool Async, string Code)
        {

            string rootPath = GetPath();

            FileStream fs = null;
            string filename = string.Empty;
            string DebugMode = string.Empty;

            if (Code.IsEmpty())
            {
                Code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");
            }

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "Logs.Prefix".StringValue();
            }

            if (string.IsNullOrEmpty(prefix))
            {
                filename = string.Format(@"{0}\{1:yyyyMMdd}.error.log", rootPath, DateTime.Now);
            }
            else
            {
                filename = string.Format(@"{0}\{1:yyyyMMdd}.error.{2}.log", rootPath, DateTime.Now, prefix);
            }

            NameFile = filename;

            DebugMode = "FrontOffice.Debug.Mode".StringValue("File");

            var ServerVariables = new StringBuilder();

            if (HttpContext.Current is not null)
            {
                if (HttpContext.Current.Request is not null)
                {
                    if (HttpContext.Current.Request.ServerVariables is not null)
                    {
                        string value;
                        if ("FrontOffice.Debug.Detail".IsTrue())
                        {
                            ServerVariables.AppendLine("RawUrl: " + HttpContext.Current.Request.Url.LocalPath);
                            ServerVariables.AppendLine("   Query: " + HttpContext.Current.Request.QueryString.ToString());
                            ServerVariables.AppendLine("    Form: " + HttpContext.Current.Request.Form.ToString());
                            ServerVariables.Append(" Session: ");
                            if (!(HttpContext.Current.Session == null))
                            {
                                foreach (string key in HttpContext.Current.Session.Keys)
                                {
                                    if (!(HttpContext.Current.Session[key] == null))
                                    {
                                        value = HttpContext.Current.Session[key].ToString();
                                        if (!string.IsNullOrEmpty(value) && value.Length > 20)
                                        {
                                            value = value.Substring(0, 20) + "...";
                                        }
                                        ServerVariables.Append(string.Format("{0}={1}&", key, value));
                                    }
                                    else
                                    {
                                        ServerVariables.Append(string.Format("{0}={1}&", key, "Null"));
                                    }
                                }
                            }
                            else
                            {
                                ServerVariables.Append("Not Enable Session");
                            }
                        }
                    }
                }
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("Code", Code);
            parameters.Add("DateEfective", DateTime.Now.ToString("hh:mm:ss.fff"));
            parameters.Add("FileName", filename);
            parameters.Add("Source", source);
            parameters.Add("Entry", entry);
            parameters.Add("CurrentException", currentException);
            parameters.Add("ThreadId", Thread.CurrentThread.ManagedThreadId);
            parameters.Add("ExceptionStack", new StackTrace(true));
            parameters.Add("ServerVariables", ServerVariables);
            if (!(HttpContext.Current == null) && !(HttpContext.Current.Session == null))
            {
                parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
            }
            else
            {
                parameters.Add("SessionID", "without session id");
            }
            parameters.Add("DebugMode", DebugMode);

            if (Async)
            {
                var AddUsersSecurityTraceAsyn = new Task(actionErrorLog, parameters);
                AddUsersSecurityTraceAsyn.Start();
            }
            else
            {
                while (ErrorLogInternal(parameters) == false)
                    Thread.Sleep(800);
            }

        }

        public static Action<object> actionErrorLog = (parameterContainer) =>
{
Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
int attempts = 1;
while (ErrorLogInternal(parameterInternal) == false && attempts < 4)
{
Thread.Sleep(800);
attempts += 1;
}
};

        #endregion

        #region Tools

        /// <summary>
        /// Gets all the detail of exception
        /// </summary>
        /// <param name="ex">Exception that gives rise to record</param>
        /// <param name="level">level of exception</param>
        /// <remarks></remarks>
        private static void TraceInnerExceptionMessage2(Exception ex, int level, StreamWriter sw)
        {

            level += 1;
            if (!(ex.InnerException == null))
            {

                TraceInnerExceptionMessage2(ex.InnerException, level, sw);
            }

            sw.WriteLine(string.Format(" {1}({0}) {2}", Information.TypeName(ex), " ".PadLeft(level), ex.Message));

        }

        private static void TraceInnerExceptionData(Exception ex, int level, StreamWriter sw)
        {

            level += 1;
            if (!(ex.InnerException == null))
            {

                TraceInnerExceptionData(ex.InnerException, level, sw);
            }

            if (ex.Data.Count > 0)
            {
                sw.WriteLine(string.Format(" {1}({0}) Details:", Information.TypeName(ex), " ".PadLeft(level)));
                foreach (DictionaryEntry de in ex.Data)
                    sw.WriteLine("   Key: {0,-20}  Value: {1}", de.Key, de.Value);

            }

        }

        public static bool ExistInTrace(StackFrame[] stacks, string name)
        {
            bool result = false;
            if (stacks.IsNotEmpty())
            {
                foreach (var item in stacks)
                {
                    if (item.GetType().IsNotEmpty())
                    {
                        if (item.GetMethod().Name.Equals(name))
                        {
                            result = true;
                            break;
                        }
                    }

                }
            }
            return result;
        }

        #endregion

        public static string GetMessage(Exception exception)
        {
            string result = string.Empty;
            var body = new StringBuilder();
            int exceptionCode = Marshal.GetHRForException(exception);
            if (exception is HttpException)
            {
                var con = HttpContext.Current;
                if (((HttpException)exception).GetHttpCode() == 404)
                {
                    body.AppendLine(string.Format("Not found URL '{0}'", con.Request.Url.ToString()));
                }
                else
                {
                    body.AppendLine(string.Format("Message:'{0}'", exception.Message));
                }
            }
            else
            {
                var ExceptionStack = new StackTrace(exception, true);
                StackFrame[] _VectorStack = ExceptionStack.GetFrames();
                if (_VectorStack is not null)
                {
                    int _Index = _VectorStack.Length;
                    if (_Index - (_Index - 1) > 0)
                    {
                        var ExceptionFrame = ExceptionStack.GetFrame(2);
                        body.Append("Method: " + ExceptionFrame.GetMethod().ToString());
                        body.AppendLine(string.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()));

                        body.AppendLine(" Class: " + ExceptionFrame.GetMethod().DeclaringType.ToString());
                        body.AppendLine("  File: " + ExceptionFrame.GetFileName());
                        body.AppendLine(" Stack:");
                        body.AppendLine(ExceptionStack.ToString().Replace("   at ", "  "));
                    }
                }
            }
            result = body.ToString();

            return result;
        }

    }

}