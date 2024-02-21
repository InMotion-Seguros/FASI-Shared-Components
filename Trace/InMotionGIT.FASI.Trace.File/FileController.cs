using InMotionGIT.Common.Core;
using InMotionGIT.FASI.Trace.Logic.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace InMotionGIT.FASI.Trace.File
{
    /// <summary>
    /// Elasticsearch controller.
    /// </summary>
    [Export(typeof(IController))]
    [ExportMetadata("Name", "FILE")]
    public class FileController : IController
    {
        public void Setup()
        {
        }

        #region TraceLog

        public void TraceLog(string entry)
        {
            TraceLog(GetFrameProcessFullName(2), entry, "Trace");
        }

        public void TraceLog(string source, string entry)
        {
            TraceLog(source, entry, "Trace");
        }

        public void TraceLog(string source, string entry, string prefix)
        {
            TraceLog(source, entry, prefix, null);
        }

        public void TraceLog(string source, string entry, string prefix, object customData)
        {
            TraceLog(source, entry, prefix, customData, true);
        }

        public void TraceLog(string source, string entry, string prefix, object customData, bool async)
        {
            try
            {
                string logPrefix = "Logs.Prefix".AppSettings();
                string filename = string.Empty;
                string DebugMode = "File";
                string format = "log";
                bool _logDebug = true;
                bool modeMultiThread = true;

                modeMultiThread = "FASI.Log.MultiThread".AppSettings<bool>(true);
                _logDebug = "Log.Debug".AppSettings<bool>(true);
                format = "Log.Format".AppSettings();

                string rootPath = GetPath();

                if (_logDebug)
                {
                    if (string.IsNullOrEmpty(logPrefix))
                        filename = string.Format(@"{0}\{1:yyyyMMdd}.{2}", rootPath, DateTime.Now, prefix);
                    else
                        filename = string.Format(@"{0}\{1:yyyyMMdd}.{2}.{3}", rootPath, DateTime.Now, prefix, logPrefix);

                    filename = filename + string.Format(".{0}", format);

                    DateTime _dateEfective = DateTime.Now;
                    string sessionId = string.Empty;
                    if (HttpContext.Current != null && HttpContext.Current.Session != null)
                        sessionId = HttpContext.Current.Session.SessionID;
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"DateEfective", _dateEfective.ToString("hh:mm:ss.fff")},
                        {"DateEfectiveRaw", _dateEfective },
                        {"FileName", filename },
                        {"Source", source },
                        {"Entry", entry },
                        {"Format", format },
                        {"ThreadId", Thread.CurrentThread.ManagedThreadId },
                        {"DebugMode", DebugMode },
                        {"CustomData", customData },
                        {"SessionID", sessionId }
                    };
                    if (modeMultiThread)
                    {
                        if (async)
                        {
                            var AddUsersSecurityTraceAsyn = new Task(ActionTraceLog, parameters);
                            AddUsersSecurityTraceAsyn.Start();
                        }
                        else
                        {
                            while (TraceLogInternal(parameters) == false)
                            {
                                Thread.Sleep(800);
                            }
                        }
                    }
                    else
                    {
                        TraceLogInternal(parameters);
                    }
                }
            }
            catch (Exception ex)
            {
                var IsWrited = false;
                while (IsWrited == false)
                {
                    try
                    {
                        var folder = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                        folder = Path.GetDirectoryName(folder);
                        string fileName = string.Format(@"{0}\error.text", folder);
                        StringBuilder body = new StringBuilder();
                        body.AppendLine(string.Format("Error:{0}", DateTime.Now.ToString("{1:yyyyMMdd}")));
                        body.AppendLine(string.Format("      Message:{0}", ex.Message));
                        body.AppendLine(string.Format("      Stack:{0}", ex.StackTrace));
                        if (!System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Create(fileName);
                        }
                        System.IO.File.AppendAllText(fileName, body.ToString());
                        IsWrited = true;
                    }
                    catch (IOException exIO)
                    {
                        if (Marshal.GetHRForException(exIO) == -2147024864)
                        {
                            IsWrited = false;
                        }
                        else
                        {
                            IsWrited = true;
                        }
                    }
                    catch (Exception)
                    {
                        IsWrited = true;
                    }
                    Thread.Sleep(800);
                }
            }
        }

        #endregion TraceLog

        #region WarningLog

        public void WarningLog(string source, string entry)
        {
            WarningLog(source, entry, string.Empty);
        }

        public void WarningLog(string source, string entry, string prefix)
        {
            WarningLog(source, entry, prefix, null);
        }

        public void WarningLog(string source, string entry, string prefix, object customData)
        {
            WarningLog(source, entry, prefix, customData, true);
        }

        public void WarningLog(string source, string entry, string prefix, object customData, bool async)
        {
            string logprefix ="Logs.Prefix".AppSettings();
            string DebugMode = "File";
            string rootPath = GetPath();

            string filename;
            if (string.IsNullOrEmpty(logprefix))
                filename = string.Format(@"{0}\{1:yyyyMMdd}.Warning.log", rootPath, DateTime.Now);
            else
                filename = string.Format(@"{0}\{1:yyyyMMdd}.Warning.{2}.log", rootPath, DateTime.Now, logprefix);

            DateTime _DateEfective = DateTime.Now;
            string sessionId = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                sessionId = HttpContext.Current.Session.SessionID;

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"DateEfective", _DateEfective.ToString("hh:mm:ss.fff") },
                {"FileName", filename },
                {"Source", source },
                {"Entry", entry },
                {"ThreadId", Thread.CurrentThread.ManagedThreadId },
                {"DebugMode", DebugMode },
                {"CustomData", customData },
                {"SessionID", sessionId }
            };

            if (async)
            {
                var AddUsersSecurityTraceAsyn = new Task(ActionWarningLog, parameters);
                AddUsersSecurityTraceAsyn.Start();
            }
            else
            {
                while (WarningLogInternal(parameters) == false)
                {
                    Thread.Sleep(800);
                }
            }
        }

        #endregion WarningLog

        #region ErrorLog

        public void ErrorLog(Exception currentException)
        {
            ErrorLog(InMotionGIT.Common.Core.Helpers.Assembly.GetFrameProcessFullName(2), string.Empty, currentException, string.Empty);
        }

        public void ErrorLog(string source, string entry)
        {
            ErrorLog(source, entry, null, string.Empty);
        }

        public void ErrorLog(string source, string entry, string prefix)
        {
            ErrorLog(source, entry, null, prefix);
        }

        public void ErrorLog(string source, string entry, Exception currentException)
        {
            ErrorLog(source, entry, currentException, string.Empty);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix)
        {
            ErrorLog(source, entry, currentException, prefix, true, string.Empty);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async)
        {
            ErrorLog(source, entry, currentException, prefix, false, string.Empty);
        }

        public void ErrorLog(string source, string entry, Exception currentException, string prefix, bool async, string code)
        {
            string rootPath = GetPath();
            string logPrefix =  "Logs.Prefix".AppSettings();

            if (string.IsNullOrEmpty(code))
                code = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff");

            if (string.IsNullOrEmpty(prefix))
                prefix = logPrefix;

            string filename;
            if (string.IsNullOrEmpty(prefix))
                filename = string.Format("{0}\\{1:yyyyMMdd}.error.log", rootPath, DateTime.Now);
            else
                filename = string.Format("{0}\\{1:yyyyMMdd}.error.{2}.log", rootPath, DateTime.Now, prefix);

            StringBuilder ServerVariables = new StringBuilder();

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                {
                    if (HttpContext.Current.Request.ServerVariables != null)
                    {
                        string value = string.Empty;
                        ServerVariables.AppendLine("RawUrl: " + HttpContext.Current.Request.Url.LocalPath);
                        ServerVariables.AppendLine("   Query: " + HttpContext.Current.Request.QueryString.ToString());
                        ServerVariables.AppendLine("    Form: " + HttpContext.Current.Request.Form.ToString());
                        ServerVariables.Append(" Session: ");
                        if (HttpContext.Current.Session != null)
                        {
                            //foreach (string key in HttpContext.Current.Session.Keys)
                            //{
                            //    if (HttpContext.Current.Session[key] != null)
                            //    {
                            //        value = HttpContext.Current.Session[key].ToString();
                            //        if (!string.IsNullOrEmpty(value) && (value.Length > 20))
                            //            value = (value.Substring(0, 20) + "...");

                            //        ServerVariables.Append(string.Format("{0}={1}&", key, value));
                            //    }
                            //    else
                            //        ServerVariables.Append(string.Format("{0}={1}&", key, "Null"));
                            //}
                        }
                        else
                            ServerVariables.Append("Not Enable Session");
                    }
                }
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Code", code);
            parameters.Add("DateEfective", DateTime.Now.ToString("hh:mm:ss.fff"));
            parameters.Add("FileName", filename);
            parameters.Add("Source", source);
            parameters.Add("Entry", entry);
            parameters.Add("CurrentException", currentException);
            parameters.Add("ThreadId", Thread.CurrentThread.ManagedThreadId);
            parameters.Add("ExceptionStack", new StackTrace(true));
            parameters.Add("ServerVariables", ServerVariables);
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
            else
                parameters.Add("SessionID", "");
            parameters.Add("DebugMode", "File");

            if (async)
            {
                var AddUsersSecurityTraceAsyn = new Task(ActionErrorLog, parameters);
                AddUsersSecurityTraceAsyn.Start();
            }
            else
            {
                while (ErrorLogInternal(parameters) == false)
                {
                    Thread.Sleep(800);
                }
            }
        }

        public bool ErrorLogInternal(Dictionary<string, object> parameterInternal)
        {
            bool result = false;
            FileStream fs = null;
            string _fileName = parameterInternal.ContainsKey("FileName") ? parameterInternal["FileName"].ToString() : string.Empty;
            string _DateEfective = parameterInternal.ContainsKey("DateEfective") ? parameterInternal["DateEfective"].ToString() : string.Empty;
            string _IP = parameterInternal.ContainsKey("IP") ? parameterInternal["IP"].ToString() : string.Empty;
            string _Source = parameterInternal.ContainsKey("Source") ? parameterInternal["Source"].ToString() : string.Empty;
            Exception _CurrentException = parameterInternal.ContainsKey("CurrentException") ? (Exception)parameterInternal["CurrentException"] : null;
            string _Entry = parameterInternal.ContainsKey("Entry") ? parameterInternal["Entry"].ToString() : string.Empty;
            StackTrace _ExceptionStack = parameterInternal.ContainsKey("ExceptionStack") ? (StackTrace)parameterInternal["ExceptionStack"] : null;
            StringBuilder _ServerVariables = parameterInternal.ContainsKey("ServerVariables") ? (StringBuilder)parameterInternal["ServerVariables"] : null;
            string _SessionID = parameterInternal.ContainsKey("SessionID") ? parameterInternal["SessionID"].ToString() : string.Empty;
            string _Code = parameterInternal.ContainsKey("Code") ? parameterInternal["Code"].ToString() : string.Empty;
            string _DebugMode = parameterInternal.ContainsKey("DebugMode") ? parameterInternal["DebugMode"].ToString() : string.Empty;

            try
            {
                if (System.IO.File.Exists(_fileName))
                    fs = System.IO.File.Open(_fileName, FileMode.Append);
                else
                    fs = System.IO.File.Create(_fileName);

                StreamWriter sw = new StreamWriter(fs);
                //string stringTemporal = string.Format("{0} {1} {2}", _IP, Common.Helpers.Connection.NameHostFromIp(_IP), _Source);
                string stringTemporal = string.Format("{0} {1} {2}", _IP, _IP, _Source);
                sw.WriteLine(stringTemporal);

                if (_CurrentException != null)
                {
                    TraceInnerExceptionMessage2(_CurrentException, 1, sw);
                    TraceInnerExceptionData(_CurrentException, 1, sw);
                }

                sw.WriteLine(string.Format(" Code:{0}", _Code));
                if (!string.IsNullOrEmpty(_Entry))
                {
                    sw.WriteLine(" Entry: " + _Entry);
                }
                sw.WriteLine(" Session Id:" + _SessionID);

                if (_ServerVariables != null && !string.IsNullOrEmpty(_ServerVariables.ToString()))
                    sw.WriteLine(_ServerVariables.ToString());

                if (_CurrentException != null)
                {
                    StackTrace ExceptionStack = new StackTrace(_CurrentException, true);
                    StackFrame[] _VectorStack = ExceptionStack.GetFrames();

                    if (_VectorStack != null)
                    {
                        int _Index = _VectorStack.Length;
                        if (_Index - (_Index - 1) > 0)
                        {
                            var ExceptionFrame = ExceptionStack.GetFrame(_Index - 1);
                            sw.WriteLine("Method: " + ExceptionFrame.GetMethod().ToString());
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
                    sw.Write(("Method: " + ExceptionFrame.GetMethod().ToString()));
                    sw.WriteLine(string.Format(" at {0},{1}", ExceptionFrame.GetFileLineNumber(), ExceptionFrame.GetFileColumnNumber()));
                    sw.WriteLine(string.Format(" Class: {0}", ExceptionFrame.GetMethod().DeclaringType));
                    sw.WriteLine(string.Format("  File: {0}", ExceptionFrame.GetFileName()));
                    sw.WriteLine(" Stack:");
                    sw.WriteLine(_ExceptionStack.ToString().Replace("   at ", "  "));
                }
                sw.Close();
                fs.Close();
                result = true;
            }
            catch (IOException exIO)
            {
                if ((Marshal.GetHRForException(exIO) == -2147024864))
                    result = false;
                else
                    result = true;
            }
            catch (Exception)
            {
                result = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return result;
        }

        #endregion ErrorLog

        #region PerformanceLog

        public Stopwatch PerformanceLogBegin(string source, string nameMethod, Dictionary<string, object> parameters)
        {
            Stopwatch result = new Stopwatch();
            result.Start();
            PerformanceLog(source, nameMethod, true, parameters, null);
            return result;
        }

        public void PerformanceLogFinish(string source, string nameMethod, Stopwatch control)
        {
            PerformanceLog(source, nameMethod, false, null, control);
        }

        public void PerformanceLog(string source, string nameMethod, bool way, Dictionary<string, object> parametersByData, Stopwatch control)
        {
            string prefix = "Performance.Prefix".AppSettings();
            string rootPath = GetPath();
            string fileName = string.Format(@"{0}\{1:yyyyMMdd}.{2}.log", rootPath, DateTime.Now, prefix);
            if (!string.IsNullOrEmpty(prefix))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("DateEfective", DateTime.Now);
                parameters.Add("Source", source);
                parameters.Add("Method", nameMethod);
                parameters.Add("FileName", fileName);
                parameters.Add("Way", way);
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                    parameters.Add("SessionID", HttpContext.Current.Session.SessionID);
                else
                    parameters.Add("SessionID", "without session id");
                if (!way || control != null)
                {
                    control.Stop();
                    parameters.Add("Elapsed", control.Elapsed);
                }
                int attempts = 1;
                while (PerformanceLogInternal(parameters, parametersByData) == false && attempts < 4)
                {
                    Thread.Sleep(800);
                    attempts++;
                }
            }
        }

        public bool PerformanceLogInternal(Dictionary<string, object> parameterInternal, Dictionary<string, object> parametersByData)
        {
            bool result = false;
            FileStream fs = null;
            try
            {
                string fileName = parameterInternal.ContainsKey("FileName") ? parameterInternal["FileName"].ToString() : string.Empty;
                string sessionID = parameterInternal.ContainsKey("SessionID") ? parameterInternal["SessionID"].ToString() : string.Empty;
                bool way = parameterInternal.ContainsKey("Way") ? bool.Parse(parameterInternal["Way"].ToString()) : false;
                string source = parameterInternal.ContainsKey("Source") ? parameterInternal["Source"].ToString() : string.Empty;
                string timeElapse = string.Empty;
                if (!way)
                {
                    timeElapse = parameterInternal.ContainsKey("Elapsed") ? parameterInternal["Elapsed"].ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(timeElapse))
                        timeElapse = string.Format("{0}Time elapsed in milliseconds:{0},{0}{1}{0}", "'", timeElapse);
                }
                DateTime dateEffective = parameterInternal.ContainsKey("DateEfective") ? (DateTime)parameterInternal["DateEfective"] : DateTime.Now;
                string method = parameterInternal.ContainsKey("Method") ? parameterInternal["Method"].ToString() : string.Empty;
                if (System.IO.File.Exists(fileName))
                    fs = System.IO.File.Open(fileName, FileMode.Append);
                else
                    fs = System.IO.File.Create(fileName);
                StreamWriter sw = new StreamWriter(fs);
                string values = string.Empty;
                if (parametersByData != null)
                {
                    List<string> valuesArray = new List<string>();
                    foreach (string key in parametersByData.Keys)
                    {
                        if (parametersByData[key] != null)
                            valuesArray.Add(string.Format("{0}='{1}'", key, "Empty"));
                        else
                            valuesArray.Add(string.Format("{0}='{1}'", key, parametersByData[key].ToString()));
                    }
                    values = string.Join("; ", valuesArray);
                }
                if (way)
                    sw.WriteLine(string.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{0}{6}{0}", "'", dateEffective.ToString("hh:mm:ss.fff"), sessionID, "Begin ", source, method, string.IsNullOrEmpty(values) ? "" : values));
                else
                    sw.WriteLine(string.Format("{0}{1}{0},{0}{2}{0},{0}{3}{0},{0}{4}{0},{0}{5}{0},{6}", "'", dateEffective.ToString("hh:mm:ss.fff"), sessionID, "Finish", source, method, timeElapse));
                sw.Close();
                fs.Close();
                result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                    result = false;
                else
                    result = true;
            }
            catch (Exception ex)
            {
                result = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return result;
        }

        #endregion PerformanceLog

        public string GetPath()
        {
            string rootPath = "Path.Logs".AppSettings();
            if (string.IsNullOrEmpty(rootPath))
                rootPath = string.Format("{0}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            return rootPath;
        }

        #region private methods

        private static Action<object> ActionTraceLog = (object parameterContainer) =>
              {
                  Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
                  int attempts = 1;
                  while (TraceLogInternal(parameterInternal) == false && attempts < 4)
                  {
                      Thread.Sleep(800);
                      attempts += 1;
                  }
              };

        private static bool TraceLogInternal(Dictionary<string, object> pParamters)
        {
            bool Result = false;
            FileStream fs = null;
            string _FileName = pParamters.ContainsKey("FileName") ? pParamters["FileName"].ToString() : string.Empty;
            string _IP = pParamters.ContainsKey("IP") ? pParamters["IP"].ToString() : string.Empty;
            string _Source = pParamters.ContainsKey("Source") ? pParamters["Source"].ToString() : string.Empty;
            string _Entry = pParamters.ContainsKey("Entry") ? pParamters["Entry"].ToString() : string.Empty;
            string _DateEfective = pParamters.ContainsKey("DateEfective") ? pParamters["DateEfective"].ToString() : string.Empty;
            string _ThreadId = pParamters.ContainsKey("ThreadId") ? pParamters["ThreadId"].ToString() : string.Empty;
            string _SessionID = pParamters.ContainsKey("SessionID") ? pParamters["SessionID"].ToString() : string.Empty;
            string _Format = pParamters.ContainsKey("Format") ? pParamters["Format"].ToString() : string.Empty;
            DateTime _DataEfectiveRaw = pParamters.ContainsKey("DateEfectiveRaw") ? (DateTime)pParamters["DateEfectiveRaw"] : DateTime.MinValue;

            try
            {
                if (System.IO.File.Exists(_FileName))
                    fs = System.IO.File.Open(_FileName, FileMode.Append);
                else
                    fs = System.IO.File.Create(_FileName);

                StreamWriter sw = new StreamWriter(fs);
                if (_Format.Equals("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    _Source = _Source.Replace(Environment.NewLine, string.Empty);
                    _Entry = _Entry.Replace(Environment.NewLine, string.Empty);

                    //sw.WriteLine(string.Format("{0},{3},{4},{5},{1},{2} ", string.Format("{0:h:mm:ss.fff tt}", _DataEfectiveRaw), _Source, _Entry, Common.Helpers.Connection.NameHostFromIp(_IP), _ThreadId, _SessionID));
                    sw.WriteLine(string.Format("{0},{3},{4},{5},{1},{2} ", string.Format("{0:h:mm:ss.fff tt}", _DataEfectiveRaw), _Source, _Entry,_IP, _ThreadId, _SessionID));
                }
                else
                {
                    //sw.WriteLine(string.Format("{0} {3} {4} {5} {1} {2} ", _DateEfective, _Source, _Entry, Common.Helpers.Connection.NameHostFromIp(_IP), _ThreadId, _SessionID));
                    sw.WriteLine(string.Format("{0},{3},{4},{5},{1},{2} ", string.Format("{0:h:mm:ss.fff tt}", _DataEfectiveRaw), _Source, _Entry, _IP, _ThreadId, _SessionID));
                }
                sw.Close();
                fs.Close();
                Result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                    Result = false;
                else
                    Result = true;
            }
            catch (Exception)
            {
                Result = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return Result;
        }

        private static bool WarningLogInternal(Dictionary<string, object> parameterInternal)
        {
            bool result = false;
            string _FileName = parameterInternal.ContainsKey("FileName") ? parameterInternal["FileName"].ToString() : string.Empty;
            string _IP = parameterInternal.ContainsKey("IP") ? parameterInternal["IP"].ToString() : string.Empty;
            string _Source = parameterInternal.ContainsKey("Source") ? parameterInternal["Source"].ToString() : string.Empty;
            string _Entry = parameterInternal.ContainsKey("Entry") ? parameterInternal["Entry"].ToString() : string.Empty;
            string _DateEfective = parameterInternal.ContainsKey("DateEfective") ? parameterInternal["DateEfective"].ToString() : string.Empty;
            string _ThreadId = parameterInternal.ContainsKey("ThreadId") ? parameterInternal["ThreadId"].ToString() : string.Empty;
            string _SessionID = parameterInternal.ContainsKey("SessionID") ? parameterInternal["SessionID"].ToString() : string.Empty;
            string _DebugMode = parameterInternal.ContainsKey("DebugMode") ? parameterInternal["DebugMode"].ToString() : string.Empty;
            object _CustomData = parameterInternal.ContainsKey("CustomData") ? parameterInternal["CustomData"]?.ToString() : string.Empty;
            FileStream fs = null;

            try
            {
                if (System.IO.File.Exists(_FileName))
                    fs = System.IO.File.Open(_FileName, FileMode.Append);
                else
                    fs = System.IO.File.Create(_FileName);

                StreamWriter sw = new StreamWriter(fs);
                DateTime thisDate = DateTime.Now;

                sw.WriteLine(string.Format("{0} {3} {4} {5} {1} {2}", _DateEfective, _Source, _Entry,  _IP, _ThreadId, _SessionID));
                //sw.WriteLine(string.Format("{0} {3} {4} {5} {1} {2}", _DateEfective, _Source, _Entry, Common.Helpers.Connection.NameHostFromIp(_IP), _ThreadId, _SessionID));
                sw.Close();
                fs.Close();
                result = true;
            }
            catch (IOException exIO)
            {
                if (Marshal.GetHRForException(exIO) == -2147024864)
                    result = false;
                else
                    result = true;
            }
            catch (Exception)
            {
                result = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return result;
        }

        private static Action<object> ActionWarningLog = (object parameterContainer) =>
              {
                  Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
                  int attempts = 1;
                  while (WarningLogInternal(parameterInternal) == false && attempts < 4)
                  {
                      Thread.Sleep(800);
                      attempts += 1;
                  }
              };

        private void TraceInnerExceptionMessage2(Exception ex, int level, StreamWriter sw)
        {
            level += 1;
            if (ex.InnerException != null)
                TraceInnerExceptionMessage2(ex.InnerException, level, sw);
            sw.WriteLine(string.Format(" {1}({0}) {2}", ex.GetType().Name, " ".PadLeft(level), ex.Message));
        }

        private void TraceInnerExceptionData(Exception ex, int level, StreamWriter sw)
        {
            level += 1;
            if (ex.InnerException != null)
                TraceInnerExceptionData(ex.InnerException, level, sw);
            if (ex.Data.Count > 0)
            {
                sw.WriteLine(string.Format("  {1}({0}) Details:", ex.GetType().Name, " ".PadLeft(level)));
                foreach (DictionaryEntry de in ex.Data)
                    sw.WriteLine("   Key: {0,-20}  Value: {1}", de.Key, de.Value);
            }
        }

        private static Action<object> ActionErrorLog = (object parameterContainer) =>
              {
                  FileController fileController = new FileController();
                  Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
                  int attempts = 1;
                  while (fileController.ErrorLogInternal(parameterInternal) == false && attempts < 4)
                  {
                      Thread.Sleep(800);
                      attempts = attempts + 1;
                  }
              };

        private static string GetFrameProcessFullName(int Index)
        {
            string result2 = string.Empty;
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames != null && stackFrames.Length >= Index)
            {
                StackFrame stackCall = stackFrames[Index];
                StackFrame stackFrame = stackCall;
                if (stackFrame.GetMethod() != null && !string.IsNullOrEmpty(stackFrame.GetMethod().Name))
                {
                    result2 = stackFrame.GetMethod().Name;
                    string nameSpace = string.Empty;
                    if (stackFrame.GetMethod().ReflectedType != null && !string.IsNullOrEmpty(stackFrame.GetMethod().ReflectedType.FullName))
                    {
                        string className = stackFrame.GetMethod().ReflectedType.FullName;
                        nameSpace = $"{className}.{stackFrame.GetMethod().Name}";
                    }
                    if (string.IsNullOrEmpty(nameSpace))
                    {
                        nameSpace = "Empty";
                    }
                    result2 = nameSpace;
                }
                stackFrame = null;
            }
            return result2;
        }

        #endregion private methods
    }
}