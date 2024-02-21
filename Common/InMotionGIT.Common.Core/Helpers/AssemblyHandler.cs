using InMotionGIT.Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InMotionGIT.Common.Core.Helpers
{
    /// <summary>
    /// Manager assembly
    /// </summary>
    public class AssemblyHandler
    {
        /// <summary>
        /// Overload for backwards compatibility which only tries to load
        /// assemblies that are already loaded in memory.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type GetTypeFromName(string typeName)
        {
            return (new AssemblyHandler()).GetTypeFromName(typeName, null);
        }


        public static string GetLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }
        /// <summary>
        /// Helper routine that looks up a type name and tries to retrieve the
        /// full type reference using GetType() and if not found looking
        /// in the actively executing assemblies and optionally loading
        /// the specified assembly name.
        /// </summary>
        /// <param name="typeName">type to load</param>
        /// <param name="assemblyName">
        /// Optional assembly name to load from if type cannot be loaded initially.
        /// Use for lazy loading of assemblies without taking a type dependency.
        /// </param>
        /// <returns>null</returns>
        // Token: 0x06000144 RID: 324 RVA: 0x000080EC File Offset: 0x000062EC
        public Type GetTypeFromName(string typeName, string assemblyName)
        {
            Type type = Type.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly asm in assemblies)
            {
                type = asm.GetType(typeName, false);
                if (type != null)
                {
                    break;
                }
            }
            if (type != null)
            {
                return type;
            }
            if (!string.IsNullOrEmpty(assemblyName))
            {
                System.Reflection.Assembly a = AssemblyHandler.LoadAssembly(assemblyName);
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static System.Reflection.Assembly LoadAssembly(string assemblyName)
        {
            System.Reflection.Assembly assembly = null;
            try
            {
                assembly = System.Reflection.Assembly.Load(assemblyName);
            }
            catch
            {
            }
            if (assembly != null)
            {
                return assembly;
            }
            if (File.Exists(assemblyName))
            {
                assembly = System.Reflection.Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts a .NET type into an XML compatible type - roughly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string MapTypeToXmlType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type == typeof(string) || type == typeof(char))
            {
                return "string";
            }
            if (type == typeof(int) || type == typeof(int))
            {
                return "integer";
            }
            if (type == typeof(short) || type == typeof(byte))
            {
                return "short";
            }
            if (type == typeof(long) || type == typeof(long))
            {
                return "long";
            }
            if (type == typeof(bool))
            {
                return "boolean";
            }
            if (type == typeof(DateTime))
            {
                return "datetime";
            }
            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(decimal))
            {
                return "decimal";
            }
            if (type == typeof(double))
            {
                return "double";
            }
            if (type == typeof(float))
            {
                return "single";
            }
            if (type == typeof(byte))
            {
                return "byte";
            }
            if (type == typeof(byte[]))
            {
                return "base64Binary";
            }
            return null;
        }

        public static System.Type FindByName(object AssemblyRoot, string PropertieName, Object Name)
        {
            System.Type result = null;

            foreach (var item in AssemblyRoot.GetType().GetProperties())
            {
                if (string.Equals(PropertieName, item.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    var rr = item;
                }
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cubeCodeSettings"></param>
        /// <param name="v"></param>
        /// <param name="Path"></param>
        public static void PropertieSet(Object Root, string Path, Object Value)
        {
            string[] vector = Path.Split(new string[] { "][" }, StringSplitOptions.RemoveEmptyEntries);
            PropertyInfo PropertieInfoFind = null;
            for (int i = 0; i < vector.Length; i++)
            {
                foreach (var item in Root.GetType().GetProperties())
                {
                    var name = vector[i].Replace("[", string.Empty).Replace("]", string.Empty);

                    if (string.Equals(name, item.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        PropertieInfoFind = item;
                        if (vector.Length == 1)
                        {
                            PropertieInfoFind.SetValue(Root, Convert.ChangeType(Value, PropertieInfoFind.PropertyType));
                            break;
                        }
                        else
                        {
                            var value = vector[i];
                            if (!value.Contains("["))
                            {
                                value = string.Format("[{0}", value);
                            }
                            if (!value.Contains("]"))
                            {
                                value = string.Format("{0}]", value);
                            }
                            PropertieSet(PropertieInfoFind.GetValue(Root, new object[] { }), Path.Replace(value, string.Empty), Value);
                        }
                        break;
                    }
                }
                if (PropertieInfoFind.IsEmpty())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoCalling"/> class.
        /// </summary>
        /// <param name="traceLevel">The trace level needed to get correct assembly
        /// - will need to adjust based on where you put these classes in your project(s).</param>
        public AssemblyHandler(int traceLevel = 4)
        {
            //----------------------------------------------------------------------
            // Default to "3" as the number of levels back in the stack trace to get the
            //  correct assembly for "calling" assembly
            //----------------------------------------------------------------------
            StackTraceLevel = traceLevel;
        }

        //----------------------------------------------------------------------
        // Standard assembly attributes
        //----------------------------------------------------------------------
        public string Company
        {
            get { return GetCallingAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company); }
        }

        public string Product
        {
            get { return GetCallingAssemblyAttribute<AssemblyProductAttribute>(a => a.Product); }
        }

        public string Copyright
        {
            get { return GetCallingAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        public string Trademark
        {
            get { return GetCallingAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark); }
        }

        public string Title
        {
            get { return GetCallingAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title); }
        }

        public string Description
        {
            get { return GetCallingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); }
        }

        public string Configuration
        {
            get { return GetCallingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); }
        }

        public string FileVersion
        {
            get { return GetCallingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version); }
        }

        //----------------------------------------------------------------------
        // Version attributes
        //----------------------------------------------------------------------
        public static Version Version
        {
            get
            {
                //----------------------------------------------------------------------
                // Get the assembly, return empty if null
                //----------------------------------------------------------------------
                System.Reflection.Assembly assembly = GetAssembly(StackTraceLevel);
                return assembly == null ? new Version() : assembly.GetName().Version;
            }
        }

        public string VersionFull
        {
            get { return Version.ToString(); }
        }

        public string VersionMajor
        {
            get { return Version.Major.ToString(); }
        }

        public string VersionMinor
        {
            get { return Version.Minor.ToString(); }
        }

        public string VersionBuild
        {
            get { return Version.Build.ToString(); }
        }

        public string VersionRevision
        {
            get { return Version.Revision.ToString(); }
        }

        //----------------------------------------------------------------------
        // Set how deep in the stack trace we're looking - allows for customized changes
        //----------------------------------------------------------------------
        public static int StackTraceLevel
        {
            get { return m_StackTraceLevel; }
            set { m_StackTraceLevel = value; }
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
            //----------------------------------------------------------------------
            // Get the assembly, return empty if null
            //----------------------------------------------------------------------
            System.Reflection.Assembly assembly = GetAssembly(StackTraceLevel);
            if (assembly == null)
            {
                return string.Empty;
            }

            //----------------------------------------------------------------------
            // Get the attribute value
            //----------------------------------------------------------------------
            T attribute__1 = (T)Attribute.GetCustomAttribute(assembly, typeof(T));
            return value.Invoke(attribute__1);
        }

        /// <summary>
        /// Go through the stack and gets the assembly
        /// </summary>
        /// <param name="stackTraceLevel">The stack trace level.</param>
        /// <returns></returns>
        private static System.Reflection.Assembly GetAssembly(int stackTraceLevel)
        {
            //----------------------------------------------------------------------
            // Get the stack frame, returning null if none
            //----------------------------------------------------------------------
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames == null)
            {
                return null;
            }

            //----------------------------------------------------------------------
            // Get the declaring type from the associated stack frame, returning null if nonw
            //----------------------------------------------------------------------
            Type declaringType = stackFrames[stackTraceLevel].GetMethod().DeclaringType;
            if (declaringType == null)
            {
                return null;
            }

            //----------------------------------------------------------------------
            // Return the assembly
            //----------------------------------------------------------------------
            System.Reflection.Assembly assembly = declaringType.Assembly;
            return assembly;
        }

        public static string GetMethodDetalleCall(string namespacefilter = "")
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            var itemsFilter = (from itemDb in stackFrames
                               where itemDb.GetMethod().IsNotEmpty() &&
                                     itemDb.GetMethod().ReflectedType.IsNotEmpty() &&
                                     !itemDb.GetMethod().ReflectedType.FullName.Contains(typeof(Assembly).Assembly.GetName().Name)
                               select itemDb);
            if (namespacefilter.IsNotEmpty())
                itemsFilter = itemsFilter.Where(c => !c.GetMethod().ReflectedType.FullName.Contains(namespacefilter));

            var item = itemsFilter.ToList().FirstOrDefault();
            var index = (new List<StackFrame>(stackFrames)).FindIndex(a => a == item);
            return GetFrameProcess(index + 1);
        }

        public static string GetFrameProcess(int Index)
        {
            StringBuilder result = new StringBuilder();
            StackTrace stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames.IsNotEmpty() && stackFrames.Length >= Index)
            {
                StackFrame stackCall = stackFrames[Index];
                var _with1 = stackCall;
                if (_with1.GetMethod().IsNotEmpty() && _with1.GetMethod().Name.IsNotEmpty())
                {
                    //result.AppendLine(string.Format("<<I>>Method:'{0}'", _with1.GetMethod().Name));
                    string nameSpace = string.Empty;
                    if (_with1.GetMethod().ReflectedType.IsNotEmpty() && _with1.GetMethod().ReflectedType.FullName.IsNotEmpty())
                    {
                        string className = _with1.GetMethod().ReflectedType.FullName;
                        nameSpace = string.Format("{0}.{1}", className, _with1.GetMethod().Name);
                    }
                    if (nameSpace.IsEmpty())
                    {
                        nameSpace = "Empty";
                    }
                    result.AppendLine(string.Format("<<I>>Call from={0} (line {1}, column {2})", nameSpace, _with1.GetFileLineNumber(), _with1.GetFileColumnNumber()));
                }
                else
                {
                    result.AppendLine(string.Format("<<I>>Method:'{0}'", "Empty"));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Obtiene el nombre del usuario de windows.
        /// </summary>
        /// <returns></returns>
        public static string GetUserNameWindows()
        {
            return Environment.UserName;
        }

        /// <summary>
        /// Obtiene el nombre de la maquina.
        /// </summary>
        /// <returns></returns>
        public static string GetUserNameMachine()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// Obtiene el nombre de la sistema operativo.
        /// </summary>
        /// <returns></returns>
        public static string GetOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Windows"
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? "Linux"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? "macOS"
                    : "Desconocido";
        }

        /// <summary>
        /// Obtiene el nombre de la sistema operativo.
        /// </summary>
        /// <returns></returns>
        public static string GetNameApplication()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        }

        public static string GetVersionApplication()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public static string GetFrameProcessFullName(int Index)
        {
            string result = String.Empty;
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames.IsNotEmpty() && stackFrames.Length >= Index)
            {
                StackFrame stackCall = stackFrames[Index];
                stackCall.With(f =>
                {
                    if (f.GetMethod().IsNotEmpty() && f.GetMethod().Name.IsNotEmpty())
                    {
                        result = f.GetMethod().Name;
                        string nameSpace = String.Empty;
                        if (f.GetMethod().ReflectedType.IsNotEmpty() && f.GetMethod().ReflectedType.FullName.IsNotEmpty())
                        {
                            string className = f.GetMethod().ReflectedType.FullName;
                            nameSpace = string.Format("{0}.{1}", className, f.GetMethod().Name);
                        }
                        if (nameSpace.IsEmpty())
                        {
                            nameSpace = "Empty";
                        }
                        result = nameSpace;
                    }
                });
            }
            return result;
        }
    }
}