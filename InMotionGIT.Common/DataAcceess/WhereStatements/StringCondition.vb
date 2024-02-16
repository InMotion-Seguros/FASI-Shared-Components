Namespace DataAccess.WhereStatements

    Public Class StringCondition(Of t)

        Private ReadOnly fieldName As String
        Private ReadOnly tableType As IWhereStatement

        Public Sub New(name As String, base As IWhereStatement)
            fieldName = name
            tableType = base
        End Sub

        Public Function EqualTo(value As String) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} = '{1}'", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function NotEqualTo(value As String) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <> '{1}'", fieldName, value)
            Return New LogicalOperator(Of t)(tableType)
        End Function

        Public Function [Like](value As String) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} LIKE '{1}'", fieldName, value)
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

        'StartsWith
        'EndsWith

    End Class

End Namespace