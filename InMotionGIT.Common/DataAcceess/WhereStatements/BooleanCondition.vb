Namespace DataAccess.WhereStatements

    Public Class BooleanCondition(Of t)

        Private ReadOnly fieldName As String
        Private ReadOnly tableType As IWhereStatement

        Public Sub New(name As String, base As IWhereStatement)
            fieldName = name
            tableType = base
        End Sub

        Public Function IsTrue() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function NotIsTrue() As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} IS NOT NULL", fieldName)
            Return New LogicalOperator(Of t)(tableType)
        End Function

    End Class

End Namespace