#Region "using"

Imports System.ComponentModel
Imports System.Data.Common
Imports System.Globalization
Imports System.Xml.Serialization

#End Region

Namespace DataType

    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    Public Class Parameter

#Region "Public properties, to expose the state of the entity"

        <XmlAttribute()>
        Public Property Name() As String = String.Empty

        <XmlAttribute()>
        Public Property Type() As String = String.Empty

        <XmlAttribute()>
        Public Property SpecialKind() As Boolean

        <XmlIgnore()>
        Public Property ValueToExecute() As Object

        <XmlAttribute(), DefaultValue(0)>
        Public Property Size As Integer

        <XmlAttribute(), DefaultValue(0)>
        Public Property Scale As Integer

#End Region

#Region "ReadOnly Properties"

        <XmlIgnore()>
        <BrowsableAttribute(False)>
        Public ReadOnly Property FormatDBType() As String
            Get
                Dim kind As String = Type

                If kind.Contains("System.") Then
                    kind = kind.Split(".")(1)
                End If

                Select Case kind.ToUpper
                    Case "STRING", "BOOLEAN"
                        Return "VARCHAR(1)"

                    Case "INT32", "DECIMAL"
                        Return "NUMBER"

                    Case "DATETIME"
                        Return "DATE"

                    Case Else
                        Return "VARCHAR(1)"
                End Select
            End Get
        End Property

        <XmlIgnore()>
        <BrowsableAttribute(False)>
        Public ReadOnly Property IsStringType() As Boolean
            Get
                Dim realType As String = Type

                If realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase) Then
                    realType = realType.Split(".")(1)
                End If

                Return realType = "Char" OrElse realType = "NChar" OrElse
                       realType = "VarChar" OrElse realType = "NVarChar" OrElse
                       realType = "String"
            End Get
        End Property

        <XmlIgnore()>
        <BrowsableAttribute(False)>
        Public ReadOnly Property IsNumericType() As Boolean
            Get
                Dim realType As String = Type

                If realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase) Then
                    realType = realType.Split(".")(1)
                End If

                Return realType = "Byte" OrElse realType = "Currency" OrElse
                       realType = "Decimal" OrElse realType = "Integer" OrElse
                       realType = "Long" OrElse realType = "Numeric" OrElse
                       realType = "Short" OrElse realType = "Number" OrElse
                       realType = "Double" OrElse realType = "Int32"
            End Get
        End Property

        Public ReadOnly Property ParameterKindDB() As DbType
            Get
                Dim realType As String = Type

                If realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase) Then
                    realType = Type.Split(".")(1)
                End If

                Select Case realType.ToUpper(CultureInfo.CurrentCulture)
                    Case "BOOLEAN"
                        Return DbType.Boolean

                    Case "CHAR"
                        Return DbType.AnsiStringFixedLength

                    Case "DATE", "DATETIME"
                        Return DbType.DateTime

                    Case "NUMBER", "DECIMAL", "NUMERIC"
                        Return DbType.Decimal

                    Case "DOUBLE"
                        Return DbType.Currency

                    Case "INTEGER", "INT32"
                        Return DbType.Int32

                    Case "VARCHAR", "STRING"
                        Return DbType.AnsiString

                    Case Else
                        Return DbType.AnsiString
                End Select
            End Get
        End Property

        Public ReadOnly Property ValueNullCondition(expression As String) As String
            Get
                Dim result As String = String.Format(CultureInfo.InvariantCulture, "({0}", expression)

                If String.Equals(expression, "Nothing", StringComparison.CurrentCultureIgnoreCase) Then
                    result = "True"

                ElseIf IsNumeric(expression) Then
                    result = "False"
                Else
                    Select Case ParameterKindDB
                        Case DbType.Decimal, DbType.Currency, DbType.Byte, DbType.Double,
                             DbType.Int16, DbType.Int64, DbType.Int32

                            result = "False"

                        Case DbType.AnsiString, DbType.AnsiStringFixedLength
                            If String.Equals(expression, "Session(""nUsercode"")",
                                        StringComparison.CurrentCultureIgnoreCase) Then

                                result += ".ToString = String.Empty)"
                            Else
                                result += " = String.Empty)"
                            End If

                        Case DbType.DateTime
                            result += " = Date.MinValue)"

                        Case DbType.Boolean
                            result += " = False)"

                        Case Else
                            result = "False"
                    End Select
                End If

                Return result
            End Get
        End Property

        Public ReadOnly Property GetDefaultValue() As String
            Get
                Dim defaultValue As String = String.Empty

                If IsStringType Then
                    defaultValue = "String.Empty"

                ElseIf IsNumericType Then
                    defaultValue = "0"
                Else
                    Dim realType As String = Type

                    If realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase) Then
                        realType = realType.Split(".")(1)
                    End If

                    Select Case realType.ToUpper(CultureInfo.CurrentCulture)
                        Case "BOOLEAN"
                            defaultValue = "False"

                        Case "DATE", "DATETIME"
                            defaultValue = "Date.MinValue"

                        Case Else
                            defaultValue = "String.Empty"
                    End Select
                End If

                Return defaultValue
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function CreateCommandParameter(command As DbCommand) As DbParameter
            Return Helpers.DataAccessLayer.CommandParameter(command, Name, ParameterKindDB, Size, True, ValueToExecute)
        End Function

#End Region

    End Class

End Namespace