Namespace DataAccess

    Public MustInherit Class ModelMapper(Of T)

        Protected Sub New()
        End Sub

        Public MustOverride Function Map(ByVal r As IDataReader) As T

    End Class

End Namespace