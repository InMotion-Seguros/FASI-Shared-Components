Imports System.Xml.Serialization
Imports System.Collections.ObjectModel

Namespace Data

    <Serializable()> _
    Partial Public Class ParameterCollection
        Inherits Collection(Of Parameter)

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace