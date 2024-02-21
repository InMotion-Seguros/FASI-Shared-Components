using System.Runtime.Serialization;

namespace InMotionGIT.Common.Domain.DataAccess;


[DataContract]
public class GeoInformation
{

    [DataMember(Name = "as")]
    public string As
    {
        get
        {
            return m_As;
        }
        set
        {
            m_As = value;
        }
    }

    private string m_As;

    [DataMember(Name = "city")]
    public string City
    {
        get
        {
            return m_City;
        }
        set
        {
            m_City = value;
        }
    }

    private string m_City;

    [DataMember(Name = "country")]
    public string Country
    {
        get
        {
            return m_Country;
        }
        set
        {
            m_Country = value;
        }
    }

    private string m_Country;

    [DataMember(Name = "countryCode")]
    public string CountryCode
    {
        get
        {
            return m_CountryCode;
        }
        set
        {
            m_CountryCode = value;
        }
    }

    private string m_CountryCode;

    [DataMember(Name = "isp")]
    public string Isp
    {
        get
        {
            return m_Isp;
        }
        set
        {
            m_Isp = value;
        }
    }

    private string m_Isp;

    [DataMember(Name = "lat")]
    public double Lat
    {
        get
        {
            return m_Lat;
        }
        set
        {
            m_Lat = value;
        }
    }

    private double m_Lat;

    [DataMember(Name = "lon")]
    public double Lon
    {
        get
        {
            return m_Lon;
        }
        set
        {
            m_Lon = value;
        }
    }

    private double m_Lon;

    [DataMember(Name = "org")]
    public string Org
    {
        get
        {
            return m_Org;
        }
        set
        {
            m_Org = value;
        }
    }

    private string m_Org;

    [DataMember(Name = "query")]
    public string Ip
    {
        get
        {
            return m_Ip;
        }
        set
        {
            m_Ip = value;
        }
    }

    private string m_Ip;

    [DataMember(Name = "region")]
    public string Region
    {
        get
        {
            return m_Region;
        }
        set
        {
            m_Region = value;
        }
    }

    private string m_Region;

    [DataMember(Name = "regionName")]
    public string RegionName
    {
        get
        {
            return m_RegionName;
        }
        set
        {
            m_RegionName = value;
        }
    }

    private string m_RegionName;

    [DataMember(Name = "status")]
    public string Status
    {
        get
        {
            return m_Status;
        }
        set
        {
            m_Status = value;
        }
    }

    private string m_Status;

    [DataMember(Name = "timezone")]
    public string Timezone
    {
        get
        {
            return m_Timezone;
        }
        set
        {
            m_Timezone = value;
        }
    }

    private string m_Timezone;

    [DataMember(Name = "zip")]
    public string Zip
    {
        get
        {
            return m_Zip;
        }
        set
        {
            m_Zip = value;
        }
    }

    private string m_Zip;
}