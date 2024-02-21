using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;

namespace InMotionGIT.Common.Helpers
{

    public class ExceptionHandlers
    {

        public static string TraceInnerExceptionMessage(Exception ex, bool includeStackInfo)
        {
            return TraceInnerExceptionMessage(ex, 0) + Constants.vbCrLf + ex.StackTrace;
        }

        public static string TraceInnerExceptionMessage(Exception ex)
        {
            return TraceInnerExceptionMessage(ex, 0);
        }

        public static string TraceInnerExceptionMessage(Exception ex, int level)
        {
            string result = string.Empty;

            result += ex.Message + Constants.vbCrLf;
            Trace.IndentLevel = level;
            Trace.WriteLine(ex.Message);
            if (!(ex.InnerException == null))
            {
                result += TraceInnerExceptionMessage(ex.InnerException, +level);
            }
            return result;
        }

        public static void LogInnerException(string key, string message, Exception ex)
        {
            string result = TraceInnerExceptionMessage(ex);

            ErrorLog(key, message, 0);
            ErrorLog(string.Empty, result, 0);
        }

        public static void ErrorLog(string key, string message, int count)
        {
            ErrorLog(key, message, count, "");
        }

        public static void ErrorLog(string key, string message, int count, string logFile)
        {
            FileStream fs;
            string filename = string.Empty;

            if (string.IsNullOrEmpty(logFile))
            {
                filename = ConfigurationManager.AppSettings["Path.Logs"] + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            else
            {
                filename = ConfigurationManager.AppSettings["Path.Logs"] + @"\" + logFile;
            }
            try
            {

                if (File.Exists(filename))
                {
                    fs = File.Open(filename, FileMode.Append);
                }
                else
                {
                    fs = File.Create(filename);
                }

                var sw = new StreamWriter(fs);
                var thisDate = DateTime.Now;
                if (string.IsNullOrEmpty(key))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        sw.WriteLine(message);
                    }
                }
                else
                {
                    sw.WriteLine(string.Format("{0} {1}{2} {3}", thisDate.ToString("hh:mm:ss.fff"), "".PadLeft(count, ' '), key, message));
                }

                sw.Close();

                fs.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }

}