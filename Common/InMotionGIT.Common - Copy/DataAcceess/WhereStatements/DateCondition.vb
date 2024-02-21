Namespace DataAccess.WhereStatements

    Public Class DateCondition(Of t)

        Protected Friend ReadOnly fieldName As String
        Protected Friend ReadOnly ConnectionName As String
        Protected Friend ReadOnly tableType As IWhereStatement

        Public Sub New(name As String, base As IWhereStatement)
            fieldName = name
            tableType = base
        End Sub

        Public Sub New(name As String, connectionName As String, base As IWhereStatement)
            fieldName = name
            tableType = base
            Me.ConnectionName = connectionName
        End Sub

        Public Function EqualTo(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} = '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function NotEqualTo(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <> '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThan(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} > '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThan(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} < '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function GreaterThanEqualTo(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} >= '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function LessThanEqualTo(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <= '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function IsNull() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function IsNotNull() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NOT NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

    End Class

End Namespace