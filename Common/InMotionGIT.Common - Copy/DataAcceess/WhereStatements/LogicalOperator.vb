Namespace DataAccess.WhereStatements

    Public Class LogicalOperator(Of t)

        Private ReadOnly tableType As IWhereStatement

        Public Sub New(base As IWhereStatement)
            tableType = base
        End Sub

        Public Function [And]() As t
            tableType.command &= " AND "
            Return tableType
        End Function

        Public Function [Or]() As t
            tableType.command &= " OR "
            Return tableType
        End Function

        Public Function Prepare() As t
            tableType.command = String.Format(" WHERE {0}", tableType.command)

            Return tableType
        End Function

    End Class

End Namespace