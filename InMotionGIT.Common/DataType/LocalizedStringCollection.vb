Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <CollectionDataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    <TypeConverter(GetType(ExpandableObjectConverter))>
    Public Class LocalizedStringCollection
        Inherits Collection(Of LocalizedString)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(enumBased As Boolean)
            MyBase.New()
            If enumBased Then
                SetValue(Enumerations.EnumLanguage.English, String.Empty)
                SetValue(Enumerations.EnumLanguage.Spanish, String.Empty)
            End If
        End Sub

        Public Sub New(ByVal list As IList(Of LocalizedString))
            MyBase.New(list)
        End Sub

        Public Sub SetUpValues(language As Integer, values As Dictionary(Of Integer, String))
            Dim languages As Dictionary(Of Integer, String) = Helpers.Caching.GetItem("Languages")
            Dim textresourceInstance As LocalizedString
            Dim existLanguage As Boolean
            Dim defaultValue As String = String.Empty

            For Each itemValue As KeyValuePair(Of Integer, String) In values
                If itemValue.Key = language Then
                    defaultValue = itemValue.Value
                End If

                textresourceInstance = Nothing

                For Each Item As LocalizedString In Me

                    If Item.Language = itemValue.Key Then
                        textresourceInstance = Item
                        textresourceInstance.Value = itemValue.Value

                        Exit For
                    End If
                Next

                If IsNothing(textresourceInstance) Then
                    textresourceInstance = New LocalizedString(itemValue.Key, itemValue.Value)
                    Add(textresourceInstance)
                End If
            Next

            If Not IsNothing(languages) Then

                For Each languageItem As KeyValuePair(Of Integer, String) In languages
                    existLanguage = False

                    For Each Item As LocalizedString In Me
                        If Item.Language = languageItem.Key Then
                            existLanguage = True

                            Exit For
                        End If
                    Next

                    If Not existLanguage Then
                        textresourceInstance = New LocalizedString(languageItem.Key, defaultValue)
                        Add(textresourceInstance)
                    End If
                Next
            End If
        End Sub

        Public Function SetValue(language As Integer, value As String) As LocalizedString
            Dim textresourceInstance As LocalizedString = Nothing

            For Each item As LocalizedString In Me
                If item.Language = language Then
                    textresourceInstance = item
                    textresourceInstance.Value = value
                    Exit For
                End If
            Next

            If IsNothing(textresourceInstance) Then
                textresourceInstance = New LocalizedString(language, value)
                Add(textresourceInstance)
            End If

            Return textresourceInstance
        End Function

        Public Sub SetAllValue(value As String)
            Dim languages As Dictionary(Of Integer, String) = Helpers.Caching.GetItem("Languages")

            For Each languageItem As KeyValuePair(Of Integer, String) In languages
                SetValue(languageItem.Key, value)
            Next
        End Sub

        Public Function GetUpValue(language As Integer, defaultLanguage As Integer) As String
            Dim result As String = String.Empty

            Try
                result = GetValue(language)

                If String.IsNullOrEmpty(result) Then
                    result = GetValue(defaultLanguage)
                End If
            Catch ex As Exception
                Throw New InvalidEnumArgumentException(String.Format(CultureInfo.InvariantCulture, "The language '{0}' not is valid", language))
            End Try

            If IsNothing(result) Then
                result = String.Empty
            End If

            Return result
        End Function

        Public Function GetValue(language As Integer) As String
            Dim result As String = String.Empty

            For Each item As LocalizedString In Me
                If item.Language = language Then
                    result = item.Value
                    Exit For
                End If
            Next

            If IsNothing(result) Then
                result = String.Empty
            End If
            If result.IsNotEmpty Then
                result = result.Trim
                'Esta línea fue comentada ya que la misma provocaba problemas en el generador de persistencia.
                'result = result.Replace("_", String.Empty)
            End If
            Return result
        End Function

        Public Function GetValue(language As Integer, mode As Enumerations.EnumFriendlyMode) As String
            Return Helpers.Names.Standard(GetValue(language), mode)
        End Function

        Public Function GetValue(ByVal language As String, ByVal mode As Enumerations.EnumFriendlyMode) As String
            Return Helpers.Names.Standard(GetValue(language), mode)
        End Function

        Public Function Clone() As LocalizedStringCollection
            Dim newLocalizedStrings As New LocalizedStringCollection

            For Each _item As LocalizedString In Me
                newLocalizedStrings.Add(_item.Clone)
            Next

            Return newLocalizedStrings
        End Function

        Public Overrides Function ToString() As String
            Dim result As String

            If Count > 0 Then
                result = "Text"
            Else
                result = String.Empty
            End If

            Return result
        End Function

    End Class

End Namespace