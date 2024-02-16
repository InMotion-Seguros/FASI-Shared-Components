Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports InMotionGIT.Common.Attributes

Namespace DataType

    <EntityCommonlyUsed()>
    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    <DebuggerDisplay("Notify {ErrorId} {Message}")>
    Public Class Notify

#Region "Public properties, to expose the state of the entity"

        ''' <summary>
        ''' Nombre de la propiedad o campo que da origen a la notificación
        ''' </summary>
        <DataMember()>
        <XmlAttribute()>
        Public Property Source As String

        ''' <summary>
        ''' Severidad de la notificación
        ''' </summary>
        <DataMember()>
        <XmlAttribute()>
        Public Property Severity As Enumerations.EnumNotifySeverity

        ''' <summary>
        ''' Número que identifica de la notificación
        ''' </summary>
        <DataMember()>
        <XmlAttribute()>
        Public Property Id As Long

        ''' <summary>
        ''' Mensaje a ser desplegado por la notificación
        ''' </summary>
        <DataMember()>
        <XmlAttribute()>
        Public Property Message As String

#End Region

        ''' <summary>
        ''' Constructor base.
        ''' </summary>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Permite saber si existe un error en la colección de notificación.
        ''' </summary>
        ''' <param name="notifyList">colección de notificación</param>
        ''' <returns>Verdadero en caso de existir un error, falso en caso contrario</returns>
        Public Shared Function HasErrors(notifyList As List(Of DataType.Notify)) As Boolean
            Dim result As Boolean = False
            If Not IsNothing(notifyList) Then
                For Each Item As Notify In notifyList
                    If Item.Severity = Enumerations.EnumNotifySeverity.Error Then
                        result = True
                        Exit For
                    End If
                Next
            End If
            Return result
        End Function

        ''' <summary>
        ''' Permite saber si existe una advertencia en la colección de notificación.
        ''' </summary>
        ''' <param name="notifyList">colección de notificación</param>
        ''' <returns>Verdadero en caso de existir una advertencia, falso en caso contrario</returns>
        Public Shared Function HasWarnings(notifyList As List(Of DataType.Notify)) As Boolean
            Dim result As Boolean = False
            If Not IsNothing(notifyList) Then
                For Each Item As Notify In notifyList
                    If Item.Severity = Enumerations.EnumNotifySeverity.Warning Then
                        result = True
                        Exit For
                    End If
                Next
            End If
            Return result
        End Function

        ''' <summary>
        ''' Permite saber si existe un mensaje en la colección de notificación.
        ''' </summary>
        ''' <param name="notifyList">colección de notificación</param>
        ''' <returns>Verdadero en caso de existir un mensaje, falso en caso contrario</returns>
        Public Shared Function HasMessages(notifyList As List(Of DataType.Notify)) As Boolean
            Dim result As Boolean = False
            If Not IsNothing(notifyList) Then
                For Each Item As Notify In notifyList
                    If Item.Severity = Enumerations.EnumNotifySeverity.Message Then
                        result = True
                        Exit For
                    End If
                Next
            End If
            Return result
        End Function

    End Class

End Namespace