Imports System.Data.Common
Imports System.Runtime.Serialization

Namespace Exceptions

    <Serializable()>
    Public Class DataAccessException
        Inherits System.Exception
        Implements ISerializable

#Region "Consructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
            Helpers.LogHandler.ErrorLog("DataAccessException", message)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
            Helpers.LogHandler.ErrorLog("DataAccessException", message, innerException)
        End Sub

        Public Sub New(ByVal message As String, ByVal InnerException As Exception, command As DbCommand, nameObject As String, commandKind As String, Optional isSaveLog As Boolean = True)
            MyBase.New(message)
            If isSaveLog Then
                MessageDetail(InnerException, message, command, nameObject, commandKind)
                Helpers.LogHandler.ErrorLog("DataAccessException", message, InnerException)
            End If
        End Sub

        Public Sub MessageDetail(InnerException As Exception, ByRef message As String, command As DbCommand, nameObject As String, commandKind As String)
            Dim _commandText As String = String.Empty
            Dim _parameters As Dictionary(Of String, String) = Nothing

            If command.IsNotEmpty Then
                Dim constraintKey As String = InnerException.Message

                message &= vbCrLf & "Command:" & Helpers.DataAccessLayer.MakeCommandSummary(command, nameObject, commandKind, _commandText, _parameters, True)
                message = message.Trim
                Dim internalMessage As String = String.Empty
                If constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase) Then
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1)
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"))

                    Dim owner As String = constraintKey.Split(".")(0)
                    Dim constraint As String = constraintKey.Split(".")(1)
                    Dim relation As String = Helpers.DataAccessLayer.QueryExecuteScalar(Of String)(String.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS")

                    internalMessage = String.Format(" Integrity constraint with table '{0}' violated", relation.Split(".")(1))
                End If

                If ("	   " & String.Format("{0}.", internalMessage)).Replace(".", String.Empty).Trim.IsNotEmpty Then
                    message &= vbCrLf & "	   " & String.Format("{0}.", internalMessage)
                End If

            End If
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

#End Region

        Public Property Command As String

        Public Property Parameters As Dictionary(Of String, String)

        Public Shared Function Factory(innerException As Exception,
                                       command As DbCommand,
                                       table As String,
                                       commandKind As String) As DataAccessException
            Dim _message As String = String.Format("Failure the {1} on the table '{0}'.", table, commandKind)
            Dim _commandText As String = String.Empty
            Dim _parameters As Dictionary(Of String, String) = Nothing

            If command.IsNotEmpty Then
                Dim constraintKey As String = innerException.Message

                _message &= vbCrLf & Helpers.DataAccessLayer.MakeCommandSummary(command, table, commandKind, _commandText, _parameters, True)
                _message = _message.Trim
                Dim internalMessage As String = String.Empty
                If constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase) Then
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1)
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"))

                    Dim owner As String = constraintKey.Split(".")(0)
                    Dim constraint As String = constraintKey.Split(".")(1)
                    Dim relation As String = Helpers.DataAccessLayer.QueryExecuteScalar(Of String)(String.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS")

                    internalMessage = String.Format(" Integrity constraint with table '{0}' violated", relation.Split(".")(1))
                ElseIf innerException.Message.StartsWith("ORA-") Or
                   innerException.Message.IndexOf(":") > -1 Then
                    Dim internalCode As String = innerException.Message.Substring(0, innerException.Message.IndexOf(":")).Trim
                    internalMessage = innerException.Message.Substring(innerException.Message.IndexOf(":") + 1).Trim
                    If internalMessage.Length > 1 Then
                        internalMessage = internalMessage.Substring(0, 1).ToUpper & internalMessage.Substring(1)
                    End If
                Else
                    internalMessage = innerException.Message
                End If
                _message &= String.Format(" {0}.", internalMessage)
            End If

            Return New DataAccessException(_message, innerException) With {.Command = _commandText,
                                                                          .Parameters = _parameters}
        End Function

        Public Shared Function Factory(innerException As Exception,
                                       message As String,
                                       command As DbCommand,
                                       table As String,
                                       commandKind As String) As DataAccessException
            Dim _commandText As String = String.Empty
            Dim _parameters As Dictionary(Of String, String) = Nothing

            If command.IsNotEmpty Then
                Dim constraintKey As String = innerException.Message

                message &= vbCrLf & "        " & Helpers.DataAccessLayer.MakeCommandSummary(command, table, commandKind, _commandText, _parameters, True)
                message = message.Trim
                Dim internalMessage As String = String.Empty
                If constraintKey.StartsWith("ORA-02291:", StringComparison.CurrentCultureIgnoreCase) Then
                    constraintKey = constraintKey.Substring(constraintKey.IndexOf("(") + 1)
                    constraintKey = constraintKey.Substring(0, constraintKey.IndexOf(")"))

                    Dim owner As String = constraintKey.Split(".")(0)
                    Dim constraint As String = constraintKey.Split(".")(1)
                    Dim relation As String = Helpers.DataAccessLayer.QueryExecuteScalar(Of String)(String.Format("SELECT ALL_CONSTRAINTS.TABLE_NAME || '.' || REL.TABLE_NAME TABLE_NAME_REL FROM ALL_CONSTRAINTS LEFT JOIN ALL_CONSTRAINTS REL ON REL.OWNER = ALL_CONSTRAINTS.R_OWNER AND REL.CONSTRAINT_NAME = ALL_CONSTRAINTS.R_CONSTRAINT_NAME WHERE ALL_CONSTRAINTS.OWNER = '{0}' AND ALL_CONSTRAINTS.CONSTRAINT_NAME = '{1}'", owner, constraint), command.Connection, "ALL_CONSTRAINTS")

                    internalMessage = String.Format(" Integrity constraint with table '{0}' violated", relation.Split(".")(1))
                ElseIf innerException.Message.StartsWith("ORA-") Or
                   innerException.Message.IndexOf(":") > -1 Then
                    Dim internalCode As String = innerException.Message.Substring(0, innerException.Message.IndexOf(":")).Trim
                    internalMessage = innerException.Message.Substring(innerException.Message.IndexOf(":") + 1).Trim
                    If internalMessage.Length > 1 Then
                        internalMessage = internalMessage.Substring(0, 1).ToUpper & internalMessage.Substring(1)
                    End If
                Else
                    internalMessage = innerException.Message
                End If
                message &= String.Format(" {0}.", internalMessage)
            End If

            Return New DataAccessException(message, innerException) With {.Command = _commandText,
                                                                          .Parameters = _parameters}
        End Function

    End Class

End Namespace