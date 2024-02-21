using System;
using System.Diagnostics;
#region using

using System.Globalization;

#endregion

namespace InMotionGIT.Common.Helpers
{

    public class Utility
    {

        #region Methods

        /// <summary>
        /// Execute a Application of the suite
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="modelID">Model Identification</param>
        /// <param name="release">Model Release</param>
        /// <remarks>
        /// Form Designer,Workflow Designer,Query Designer,CRUD Form Designer
        /// 
        /// Arguments samples:
        /// http://robindotnet.wordpress.com/2010/03/21/how-to-pass-arguments-to-an-offline-clickonce-application/
        /// </remarks>
        public static void ExecuteSuiteApp(string appName, string modelID, int release)
        {
            string exePath = string.Format(@"{0}\InMotionGIT\Ease Designer Workbench\{1}.appref-ms", Environment.GetFolderPath(Environment.SpecialFolder.Programs), appName);
            if (System.IO.File.Exists(exePath))
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = exePath,
                    Arguments = string.Format(CultureInfo.InvariantCulture, "?modelid={0}&release={1}", modelID, release)
                };

                using (var processToRun = new Process() { StartInfo = startInfo })
                {
                    processToRun.Start();
                }
            }
        }

        #endregion

    }

}