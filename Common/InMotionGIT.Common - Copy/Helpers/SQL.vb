Namespace Helpers

    Public Class SQL

        Public Shared Function SafeSqlLikeClauseLiteral(ByVal inputSQL As String) As String
            Dim s As String = inputSQL
            s = inputSQL.Replace("'", "''")
            s = s.Replace("[", "[[]")
            s = s.Replace("%", "[%]")
            s = s.Replace("_", "[_]")
            Return s
        End Function

    End Class

End Namespace