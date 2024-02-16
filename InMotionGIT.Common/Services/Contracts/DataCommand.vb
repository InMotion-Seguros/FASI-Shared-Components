﻿Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports InMotionGIT.Common.DataType

Namespace Services.Contracts

    <DataContract()>
    Public Class DataCommand

        <DataMember()>
        Public ConnectionStringName As String

        <DataMember()>
        Public Property TableName As String

        <DataMember()>
        Public Property Statement As String

        <DataMember()>
        Public Property Operation As String

        <DataMember()>
        <XmlArray(ElementName:="Fields")>
        <XmlArrayItem(GetType(Dictionary(Of String, String)))>
        Public Property Fields As Dictionary(Of String, String) = New Dictionary(Of String, String)

        <DataMember()>
        Public Property Parameters As List(Of DataParameter)

        <DataMember()>
        Public Property Owner As String

        <DataMember()>
        Public Property ObjectType As String

        <DataMember()>
        Public Property CompanyId As Integer

        <DataMember()>
        Public Property ConnectionStringsRaw As Services.Contracts.ConnectionString

        <DataMember()>
        Public Property MaxNumberOfRecord As Integer

        <DataMember()>
        Public Property IgnoreMaxNumberOfRecords As Boolean

        <DataMember()>
        Public Property QueryCount As String

        <DataMember()>
        Public Property QueryCountResult As Integer

        <DataMember()>
        Public Property LookUp As LookUpValue

    End Class

End Namespace