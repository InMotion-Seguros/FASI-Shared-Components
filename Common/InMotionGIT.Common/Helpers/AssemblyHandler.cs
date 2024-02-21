using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Helpers
{

    public class AssemblyHandler
    {

        /// <summary>
        /// Initializes a new instance of the AssemblyInfoCalling class.
        /// </summary>
        /// <param name="traceLevel">The trace level needed to get correct assembly
        /// - will need to adjust based on where you put these classes in your project(s).</param>
        public AssemblyHandler(int traceLevel = 4)
        {
            // ----------------------------------------------------------------------
            // Default to "3" as the number of levels back in the stack trace to get the
            // correct assembly for "calling" assembly
            // ----------------------------------------------------------------------
            StackTraceLevel = traceLevel;
        }

        // ----------------------------------------------------------------------
        // Standard assembly attributes
        // ----------------------------------------------------------------------
        public string Company
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company);
            }
        }

        public string Product
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);
            }
        }

        public string Copyright
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
            }
        }

        public string Trademark
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark);
            }
        }

        public string Title
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
            }
        }

        public string Description
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
            }
        }

        public string Configuration
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
            }
        }

        public string FileVersion
        {
            get
            {
                return GetCallingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
            }
        }

        // ----------------------------------------------------------------------
        // Version attributes
        // ----------------------------------------------------------------------
        public static Version Version
        {
            get
            {
                // ----------------------------------------------------------------------
                // Get the assembly, return empty if null
                // ----------------------------------------------------------------------
                var assembly = GetAssembly(StackTraceLevel);
                return assembly is null ? new Version() : assembly.GetName().Version;
            }
        }

        public string VersionFull
        {
            get
            {
                return Version.ToString();
            }
        }

        public string VersionMajor
        {
            get
            {
                return Version.Major.ToString();
            }
        }

        public string VersionMinor
        {
            get
            {
                return Version.Minor.ToString();
            }
        }

        public string VersionBuild
        {
            get
            {
                return Version.Build.ToString();
            }
        }

        public string VersionRevision
        {
            get
            {
                return Version.Revision.ToString();
            }
        }

        // ----------------------------------------------------------------------
        // Set how deep in the stack trace we're looking - allows for customized changes
        // ----------------------------------------------------------------------
        public static int StackTraceLevel
        {
            get
            {
                return m_StackTraceLevel;
            }
            set
            {
                m_StackTraceLevel = value;
            }
        }

        private static int m_StackTraceLevel;

        /// <summary>
        /// Gets the calling assembly attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <example>return GetCallingAssemblyAttribute&lt;AssemblyCompanyAttribute&gt;(a => a.Company);</example>
        /// <returns></returns>
        private string GetCallingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            // ----------------------------------------------------------------------
            // Get the assembly, return empty if null
            // ----------------------------------------------------------------------
            var assembly = GetAssembly(StackTraceLevel);
            if (assembly is null)
            {
                return string.Empty;
            }

            // ----------------------------------------------------------------------
            // Get the attribute value
            // ----------------------------------------------------------------------
            T attribute__1 = (T)Attribute.GetCustomAttribute(assembly, typeof(T));
            return value.Invoke(attribute__1);
        }

        /// <summary>
        /// Go through the stack and gets the assembly
        /// </summary>
        /// <param name="stackTraceLevel">The stack trace level.</param>
        /// <returns></returns>
        private static Assembly GetAssembly(int stackTraceLevel)
        {
            // ----------------------------------------------------------------------
            // Get the stack frame, returning null if none
            // ----------------------------------------------------------------------
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames is null)
            {
                return null;
            }

            // ----------------------------------------------------------------------
            // Get the declaring type from the associated stack frame, returning null if nonw
            // ----------------------------------------------------------------------
            var declaringType = stackFrames[stackTraceLevel].GetMethod().DeclaringType;
            if (declaringType is null)
            {
                return null;
            }

            // ----------------------------------------------------------------------
            // Return the assembly
            // ----------------------------------------------------------------------
            var assembly = declaringType.Assembly;
            return assembly;
        }

        internal static string MethodName(string currentMethod)
        {
            var result = new StringBuilder();
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            int Index = stackFrames.Select((elem, indexInternal) => new { elem, indexInternal }).First(p => p.elem.GetMethod().Name.ToLower().Equals(currentMethod)).indexInternal + 1;
            if (stackFrames.IsNotEmpty() && stackFrames.Length >= Index)
            {
                var stackCall = stackFrames[Index];
                if (stackCall.GetMethod().IsNotEmpty() && stackCall.GetMethod().Name.IsNotEmpty())
                {
                    result.AppendLine(string.Format("<<I>>Method:'{0}'", stackCall.GetMethod().Name));
                    string nameSpace = string.Empty;
                    if (stackCall.GetMethod().ReflectedType.IsNotEmpty() && stackCall.GetMethod().ReflectedType.FullName.IsNotEmpty())
                    {
                        string className = stackCall.GetMethod().ReflectedType.FullName;
                        nameSpace = string.Format("{0}.{1}", className, stackCall.GetMethod().Name);
                    }
                    if (nameSpace.IsEmpty())
                    {
                        nameSpace = "Empty";
                    }
                    result.AppendLine(string.Format("<<I>>Namespace:'{0}'", nameSpace));
                }
                else
                {
                    result.AppendLine(string.Format("<<I>>Method:'{0}'", "Empty"));
                }
                result.AppendLine(string.Format("<<I>>Row/Column:'{0}'/'{1}'", stackCall.GetFileLineNumber(), stackCall.GetFileColumnNumber()));
            }
            return result.ToString();
        }

        public static string GetFrameProcess(int Index)
        {
            var result = new StringBuilder();
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames.IsNotEmpty() && stackFrames.Length >= Index)
            {
                var stackCall = stackFrames[Index];
                if (stackCall.GetMethod().IsNotEmpty() && stackCall.GetMethod().Name.IsNotEmpty())
                {
                    result.AppendLine(string.Format("<<I>>Method:'{0}'", stackCall.GetMethod().Name));
                    string nameSpace = string.Empty;
                    if (stackCall.GetMethod().ReflectedType.IsNotEmpty() && stackCall.GetMethod().ReflectedType.FullName.IsNotEmpty())
                    {
                        string className = stackCall.GetMethod().ReflectedType.FullName;
                        nameSpace = string.Format("{0}.{1}", className, stackCall.GetMethod().Name);
                    }
                    if (nameSpace.IsEmpty())
                    {
                        nameSpace = "Empty";
                    }
                    result.AppendLine(string.Format("<<I>>Namespace:'{0}'", nameSpace));
                }
                else
                {
                    result.AppendLine(string.Format("<<I>>Method:'{0}'", "Empty"));
                }
                result.AppendLine(string.Format("<<I>>Row/Column:'{0}'/'{1}'", stackCall.GetFileLineNumber(), stackCall.GetFileColumnNumber()));
            }
            return result.ToString();
        }

        public static string GetFrameProcessFullName(int Index)
        {
            string result = string.Empty;
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames.IsNotEmpty() && stackFrames.Length >= Index)
            {
                var stackCall = stackFrames[Index];
                if (stackCall.GetMethod().IsNotEmpty() && stackCall.GetMethod().Name.IsNotEmpty())
                {
                    result = stackCall.GetMethod().Name;
                    string nameSpace = string.Empty;
                    if (stackCall.GetMethod().ReflectedType.IsNotEmpty() && stackCall.GetMethod().ReflectedType.FullName.IsNotEmpty())
                    {
                        string className = stackCall.GetMethod().ReflectedType.FullName;
                        nameSpace = string.Format("{0}.{1}", className, stackCall.GetMethod().Name);
                    }
                    if (nameSpace.IsEmpty())
                    {
                        nameSpace = "Empty";
                    }
                    result = nameSpace;
                }
            }
            return result;
        }

        /// <summary>
        /// Obtiene el nombre del usuario de windows.
        /// </summary>
        /// <returns></returns>
        public static string GetUserNameWindows()
        {
            string result = string.Empty;
            if (!HostingEnvironment.IsHosted)
            {
                result = Environment.UserName;
            }
            return result;
        }

        /// <summary>
        /// Obtiene el nombre de la maquina.
        /// </summary>
        /// <returns></returns>
        public static string GetUserNameMachine()
        {
            string result = string.Empty;
            if (!HostingEnvironment.IsHosted)
            {
                result = Environment.MachineName;
            }
            return result;
        }

        /// <summary>
        /// Obtiene el nombre de la sistema operativo.
        /// </summary>
        /// <returns></returns>
        public static string GetOS()
        {
            string result = string.Empty;
            try
            {
                if (!HostingEnvironment.IsHosted)
                {
                    result = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
                }
            }
            catch (Exception ex)
            {
                LogHandler.ErrorLog("InMotionGIT.Common.Helpers.AssemblyHandler", "GetOS", ex);
                result = "Unknown";
            }

            return result;
        }

        /// <summary>
        /// Obtiene el nombre de la sistema operativo.
        /// </summary>
        /// <returns></returns>
        public static string GetNameApplication()
        {
            string result = string.Empty;
            if (!HostingEnvironment.IsHosted)
            {
                try
                {
                    result = new AssemblyHandler(5).Title;
                    if (result.IsEmpty())
                    {
                        result = Assembly.GetCallingAssembly().GetName().Name;
                    }
                }
                catch (Exception ex)
                {
                    result = "Name not found";
                }
            }
            return result;
        }

        public static string GetVersionApplication()
        {
            string result = string.Empty;
            if (!HostingEnvironment.IsHosted)
            {
                try
                {
                    result = new AssemblyHandler(5).VersionFull;
                    if (result.IsEmpty())
                    {
                        result = Assembly.GetCallingAssembly().GetName().Version.ToString();
                    }
                }
                catch (Exception ex)
                {
                    result = "-1";
                }
            }
            return result;
        }

    }

}