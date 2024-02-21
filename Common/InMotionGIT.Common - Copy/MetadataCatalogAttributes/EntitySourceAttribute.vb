Namespace Attributes

    ''' <summary>
    '''Establece la procedencia del la entidad a nivel de modelo
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class)>
    Public NotInheritable Class EntitySourceAttribute
        Inherits Attribute

        Private _source As String

        ''' <summary>
        ''' List of properties that define the primary key
        ''' </summary>
        Public ReadOnly Property Source() As String
            Get
                Return _source
            End Get
        End Property

        ''' <summary>
        ''' Initializes a new instance of the EntityPrimaryKeyMembers class
        ''' </summary>
        ''' <param name="source">List of properties that define the primary key</param>
        ''' <remarks>Ejemplo: &lt;EntityPrimaryKeyMembers("RecordOwner,KeyToAddressRecord,RecordEffectiveDate")&gt;</remarks>
        Public Sub New(ByVal source As String)
            _source = source
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace