Namespace DataAccess.WhereStatements

    Public Class EffectiveDateCondition(Of t)
        Inherits DateCondition(Of t)

        Protected Friend ReadOnly nullfieldName As String

        Public Sub New(ByVal name As String, ByVal nulldate As String, connectionName As String, ByVal base As IWhereStatement)
            MyBase.New(name, connectionName, base)
            nullfieldName = nulldate
        End Sub

        Public Sub New(ByVal name As String, ByVal nulldate As String, ByVal base As IWhereStatement)
            MyBase.New(name, base)
            nullfieldName = nulldate
        End Sub

        Public Sub New(ByVal name As String, ByVal base As IWhereStatement)
            MyBase.New(name, base)
        End Sub

        Public Function ValidAt(value As Date) As LogicalOperator(Of t)
            tableType.command &= String.Format("{0} <= '{2}' AND ({1} IS NULL OR {1} > '{2}')", fieldName, nullfieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value))
            Return New LogicalOperator(Of t)(tableType)
        End Function

    End Class

End Namespace