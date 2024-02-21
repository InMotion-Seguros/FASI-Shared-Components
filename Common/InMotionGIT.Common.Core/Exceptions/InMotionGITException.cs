using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.VisualBasic;

namespace InMotionGIT.Common.Core.Exceptions
{

    [Serializable()]
    public class InMotionGITException : Exception, ISerializable
    {

        private Collection _InvalidFields;
        public static InMotionGIT.FASI.Trace.Logic.Connection mefLog = new InMotionGIT.FASI.Trace.Logic.Connection();

        protected InMotionGITException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public InMotionGITException() : base()
        {
        }

        public InMotionGITException(string message) : base(message)
        {
            
            mefLog.ErrorLog("InMotionGITException", message);
        }

        public InMotionGITException(string message, Exception inner) : base(message, inner)
        { 
            mefLog.ErrorLog("InMotionGITException", message, inner);
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
            mefLog.ErrorLog("ShowError", ex.Message, ex); 

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

        

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Implements ISerializable.GetObjectData
            base.GetObjectData(info, context);
            throw new ArgumentNullException("info");

        }

    }

}