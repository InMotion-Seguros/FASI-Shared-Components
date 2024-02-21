Namespace DataAccess.WhereStatements

    Public Class NumericCondition(Of t)

        Private ReadOnly fieldName As String
        Private ReadOnly tableType As IWhereStatement

        Public Sub New(name As String, base As IWhereStatement)
            fieldName = name
            tableType = base
        End Sub

#Region "EqualTo"

        Public Function EqualTo(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} = {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function EqualTo(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} = {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function EqualTo(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} = {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "NotEqualTo"

        Public Function NotEqualTo(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <> {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function NotEqualTo(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <> {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function NotEqualTo(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <> {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "GreaterThan"

        Public Function GreaterThan(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} > {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThan(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} > {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThan(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} > {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "LessThan"

        Public Function LessThan(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} < {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThan(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} < {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThan(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} < {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "GreaterThanEqualTo"

        Public Function GreaterThanEqualTo(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} >= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThanEqualTo(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} >= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThanEqualTo(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} >= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "LessThanEqualTo"

        Public Function LessThanEqualTo(value As Integer) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThanEqualTo(value As Double) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThanEqualTo(value As Decimal) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <= {1}", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "IsNull"

        Public Function IsNull() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

#Region "IsNotNull"

        Public Function IsNotNull() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NOT NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

#End Region

    End Class

End Namespace