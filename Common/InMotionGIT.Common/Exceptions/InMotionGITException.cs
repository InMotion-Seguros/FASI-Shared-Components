using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.VisualBasic;

namespace InMotionGIT.Common.Exceptions
{

    [Serializable()]
    public class InMotionGITException : Exception, ISerializable
    {

        private Collection _InvalidFields;

        protected InMotionGITException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public InMotionGITException() : base()
        {
        }

        public InMotionGITException(string message) : base(message)
        {
            Helpers.LogHandler.ErrorLog("InMotionGITException", message);
        }

        public InMotionGITException(string message, Exception inner) : base(message, inner)
        {
            Helpers.LogHandler.ErrorLog("InMotionGITException", message, inner);
        }

        public Collection InvalidFields
        {
            get
            {
                Collection InvalidFieldsRet = default;
                InvalidFieldsRet = _InvalidFields;
                return InvalidFieldsRet;
            }
        }

        public static string ShowError(Exception ex)
        {
            MakeLog(ex);

            switch (ex.GetType().Name ?? "")
            {
                case "InvalidCastException":
                    {
                        return "Error: Input string was not in a correct format.";
                    }
                case "NullReferenceException":
                    {
                        return "Error: Object reference not set to an instance of an object.";
                    }

                default:
                    {
                        return "Unexpected error in the system. Verify with your administrator";
                    }
            }
        }

        internal static void MakeLog(Exception ex)
        {
            System.IO.StreamWriter file;
            file = My.MyProject.Computer.FileSystem.OpenTextFileWriter(@"c:\test.txt", true);
            file.WriteLine(string.Format("|-------------------{0}---------------------------|{1}", DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture), Constants.vbCrLf));
            file.WriteLine(string.Format("Message: {0}{1}", ex.Message, Constants.vbCrLf));
            if (ex.InnerException is not null)
            {
                file.WriteLine(string.Format("InnerException: {0}{1}", ex.InnerException.Message, Constants.vbCrLf));
            }
            file.WriteLine(string.Format("Data: {0}{1}", ex.Data, Constants.vbCrLf));
            file.WriteLine(string.Format("StackTrace: {0}{1}", ex.StackTrace, Constants.vbCrLf));
            file.WriteLine(string.Format("Source: {0}{1}", ex.Source, Constants.vbCrLf));
            if (!string.IsNullOrEmpty(ex.HelpLink))
            {
                file.WriteLine(string.Format("HelpLink: {0}{1}", ex.HelpLink, Constants.vbCrLf));
            }
            file.WriteLine("|----------------------------------------------------------------------|" + Constants.vbCrLf);
            file.Close();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Implements ISerializable.GetObjectData
            base.GetObjectData(info, context);
            throw new ArgumentNullException("info");

        }

    }

}