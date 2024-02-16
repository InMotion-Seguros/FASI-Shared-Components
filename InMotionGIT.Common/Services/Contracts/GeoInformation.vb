Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract>
    Public Class GeoInformation

        <DataMember(Name:="as")>
        Public Property [As]() As String
            Get
                Return m_As
            End Get
            Set
                m_As = Value
            End Set
        End Property

        Private m_As As String

        <DataMember(Name:="city")>
        Public Property City() As String
            Get
                Return m_City
            End Get
            Set
                m_City = Value
            End Set
        End Property

        Private m_City As String

        <DataMember(Name:="country")>
        Public Property Country() As String
            Get
                Return m_Country
            End Get
            Set
                m_Country = Value
            End Set
        End Property

        Private m_Country As String

        <DataMember(Name:="countryCode")>
        Public Property CountryCode() As String
            Get
                Return m_CountryCode
            End Get
            Set
                m_CountryCode = Value
            End Set
        End Property

        Private m_CountryCode As String

        <DataMember(Name:="isp")>
        Public Property Isp() As String
            Get
                Return m_Isp
            End Get
            Set
                m_Isp = Value
            End Set
        End Property

        Private m_Isp As String

        <DataMember(Name:="lat")>
        Public Property Lat() As Double
            Get
                Return m_Lat
            End Get
            Set
                m_Lat = Value
            End Set
        End Property

        Private m_Lat As Double

        <DataMember(Name:="lon")>
        Public Property Lon() As Double
            Get
                Return m_Lon
            End Get
            Set
                m_Lon = Value
            End Set
        End Property

        Private m_Lon As Double

        <DataMember(Name:="org")>
        Public Property Org() As String
            Get
                Return m_Org
            End Get
            Set
                m_Org = Value
            End Set
        End Property

        Private m_Org As String

        <DataMember(Name:="query")>
        Public Property Ip() As String
            Get
                Return m_Ip
            End Get
            Set
                m_Ip = Value
            End Set
        End Property

        Private m_Ip As String

        <DataMember(Name:="region")>
        Public Property Region() As String
            Get
                Return m_Region
            End Get
            Set
                m_Region = Value
            End Set
        End Property

        Private m_Region As String

        <DataMember(Name:="regionName")>
        Public Property RegionName() As String
            Get
                Return m_RegionName
            End Get
            Set
                m_RegionName = Value
            End Set
        End Property

        Private m_RegionName As String

        <DataMember(Name:="status")>
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set
                m_Status = Value
            End Set
        End Property

        Private m_Status As String

        <DataMember(Name:="timezone")>
        Public Property Timezone() As String
            Get
                Return m_Timezone
            End Get
            Set
                m_Timezone = Value
            End Set
        End Property

        Private m_Timezone As String

        <DataMember(Name:="zip")>
        Public Property Zip() As String
            Get
                Return m_Zip
            End Get
            Set
                m_Zip = Value
            End Set
        End Property

        Private m_Zip As String
    End Class

End Namespace