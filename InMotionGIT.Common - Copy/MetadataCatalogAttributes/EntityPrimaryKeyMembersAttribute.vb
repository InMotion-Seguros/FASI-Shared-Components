Namespace Attributes

    ''' <summary>
    ''' Allows you to indicate the properties of the class to be used as primary key to be used to persist information in a table.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class)>
    Public NotInheritable Class EntityPrimaryKeyMembersAttribute
        Inherits Attribute

        Private _primaryKeyMembers As String

        ''' <summary>
        ''' List of properties that define the primary key
        ''' </summary>
        Public ReadOnly Property PrimaryKeyMembers() As String
            Get
                Return _primaryKeyMembers
            End Get
        End Property

        ''' <summary>
        ''' Initializes a new instance of the EntityPrimaryKeyMembers class
        ''' </summary>
        ''' <param name="primaryKeyMembers">List of properties that define the primary key</param>
        ''' <remarks>Ejemplo: &lt;EntityPrimaryKeyMembers("RecordOwner,KeyToAddressRecord,RecordEffectiveDate")&gt;</remarks>
        Public Sub New(ByVal primaryKeyMembers As String)
            _primaryKeyMembers = primaryKeyMembers
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace