 
using System.Data.Common;
using System.Runtime.Serialization; 
using Microsoft.VisualBasic;
using InMotionGIT.Common.Core.Extensions;

namespace InMotionGIT.Common.Core.Exceptions
{

    [Serializable()]
    public class DataAccessException : Exception, ISerializable
    {
        public static InMotionGIT.FASI.Trace.Logic.Connection mefLog = new InMotionGIT.FASI.Trace.Logic.Connection();

        #region Consructors

        public DataAccessException() : base()
        {
        }

        public DataAccessException(string message) : base(message)
        {
            mefLog.ErrorLog("DataAccessException", message);
        }

        public DataAccessException(string message, Exception innerException) : base(message, innerException)
        {
            mefLog.ErrorLog("DataAccessException", message, innerException);
        }

        public DataAccessException(string message, Exception InnerException, DbCommand command, string nameObject, string commandKind, bool isSaveLog = true) : base(message)
        {
            if (isSaveLog)
            {
                MessageDetail(InnerException, ref message, command, nameObject, commandKind);
                mefLog.ErrorLog("DataAccessException", message, InnerException);
            }
        }

        public void MessageDetail(Exception InnerException, ref string message, DbCommand command, string nameObject, string commandKind)
        {
            string _commandText = string.Empty;
            Dictionary<string, string> _parameters = null;

            if (command.IsNotEmpty())
            {
                string constraintKey = InnerException.Message;

                //message += Constants.vbCrLf + "Command:" + Helpers.DataAccessLayer.MakeCommandSummary(command, nameObject, commandKind, ref _commandText, ref _parameters, true);
                message = message.Trim();
                string internalMessage = string.Empty;
                if (constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase))
                {
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1);
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"));

                    string owner = constraintKey.Split('.')[0];
                    string constraint = constraintKey.Split('.')[1];
                    //string relation = Helpers.DataAccessLayer.QueryExecuteScalar<string>(string.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS");

                    //internalMessage = string.Format(" Integrity constraint with table '{0}' violated", relation.Split('.')[1]);
                }

                if (("	   " + string.Format("{0}.", internalMessage)).Replace(".", string.Empty).Trim().IsNotEmpty())
                {
                    message += Constants.vbCrLf + "	   " + string.Format("{0}.", internalMessage);
                }

            }
        }

        protected DataAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion

        public string Command { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public static DataAccessException Factory(Exception innerException, DbCommand command, string table, string commandKind)
        {
            string _message = string.Format("Failure the {1} on the table '{0}'.", table, commandKind);
            string _commandText = string.Empty;
            Dictionary<string, string> _parameters = null;

            if (command.IsNotEmpty())
            {
                string constraintKey = innerException.Message;

                //TODO: Isaac Cambiar
                //_message += Constants.vbCrLf + Helpers.DataAccessLayer.MakeCommandSummary(command, table, commandKind, ref _commandText, ref _parameters, true);
                _message = _message.Trim();
                string internalMessage = string.Empty;
                if (constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase))
                {
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1);
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"));

                    string owner = constraintKey.Split('.')[0];
                    string constraint = constraintKey.Split('.')[1];
                    //TODO: Isaac Cambiar
                    //string relation = Helpers.DataAccessLayer.QueryExecuteScalar<string>(string.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS");

                    //internalMessage = string.Format(" Integrity constraint with table '{0}' violated", relation.Split('.')[1]);
                }
                else if (innerException.Message.StartsWith("ORA-") | innerException.Message.IndexOf(":") > -1)
                {
                    string internalCode = innerException.Message.Substring(0, innerException.Message.IndexOf(":")).Trim();
                    internalMessage = innerException.Message.Substring(innerException.Message.IndexOf(":") + 1).Trim();
                    if (internalMessage.Length > 1)
                    {
                        internalMessage = internalMessage.Substring(0, 1).ToUpper() + internalMessage.Substring(1);
                    }
                }
                else
                {
                    internalMessage = innerException.Message;
                }
                _message += string.Format(" {0}.", internalMessage);
            }

            return new DataAccessException(_message, innerException)
            {
                Command = _commandText,
                Parameters = _parameters
            };
        }

        public static DataAccessException Factory(Exception innerException, string message, DbCommand command, string table, string commandKind)
        {
            string _commandText = string.Empty;
            Dictionary<string, string> _parameters = null;

            if (command.IsNotEmpty())
            {
                string constraintKey = innerException.Message;
                //TODO: Isaac Cambiar
                //message += Constants.vbCrLf + "        " + Helpers.DataAccessLayer.MakeCommandSummary(command, table, commandKind, ref _commandText, ref _parameters, true);
                message = message.Trim();
                string internalMessage = string.Empty;
                if (constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase))
                {
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1);
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"));

                    string owner = constraintKey.Split('.')[0];
                    string constraint = constraintKey.Split('.')[1];
                    //TODO: Isaac Cambiar
                    //string relation = Helpers.DataAccessLayer.QueryExecuteScalar<string>(string.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS");

                    //internalMessage = string.Format(" Integrity constraint with table '{0}' violated", relation.Split('.')[1]);
                }
                else if (innerException.Message.StartsWith("ORA-") | innerException.Message.IndexOf(":") > -1)
                {
                    string internalCode = innerException.Message.Substring(0, innerException.Message.IndexOf(":")).Trim();
                    internalMessage = innerException.Message.Substring(innerException.Message.IndexOf(":") + 1).Trim();
                    if (internalMessage.Length > 1)
                    {
                        internalMessage = internalMessage.Substring(0, 1).ToUpper() + internalMessage.Substring(1);
                    }
                }
                else
                {
                    internalMessage = innerException.Message;
                }
                message += string.Format(" {0}.", internalMessage);
            }

            return new DataAccessException(message, innerException)
            {
                Command = _commandText,
                Parameters = _parameters
            };
        }

    }

}