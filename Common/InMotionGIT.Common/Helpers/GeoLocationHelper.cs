using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using InMotionGIT.Common.Extensions;
using InMotionGIT.Common.Services.Contracts;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{
    public class GeoLocationHelper
    {
        public static readonly string[] ImageList = new[] { "ad", "ae", "af", "ag", "ai", "al", "am", "an", "ao", "ar", "as", "at", "au", "aw", "ax", "az", "ba", "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bm", "bn", "bo", "br", "bs", "bt", "bv", "bw", "by", "bz", "ca", "catalonia", "cc", "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr", "cs", "cu", "cv", "cx", "cz", "de", "dj", "dk", "dm", "do", "dz", "ec", "ee", "eg", "eh", "england", "er", "es", "et", "europeanunion", "fam", "fi", "fj", "fk", "fm", "fo", "fr", "ga", "gb", "gd", "ge", "gf", "gl", "gm", "gn", "gp", "gq", "gr", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht", "hu", "id", "ie", "il", "in", "io", "iq", "ir", "is", "it", "jm", "jo", "jp", "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw", "ky", "kz", "la", "lb", "lc", "li", "lk", "lr", "ls", "lt", "lu", "lv", "ly", "ma", "mc", "md", "me", "mg", "mh", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np", "nr", "nu", "nz", "om", "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt", "pw", "py", "qa", "re", "ro", "rs", "ru", "rw", "sa", "sb", "sc", "scotland", "sd", "se", "sg", "si", "sj", "sk", "sl", "sm", "sn", "so", "sr", "st", "sv", "sy", "sz", "tc", "td", "tf", "tg", "th", "tj", "tk", "tl", "tm", "tn", "to", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "um", "us", "uy", "uz", "va", "vc", "ve", "vg", "vi", "vn", "vu", "wales", "wf", "ws", "ye", "yt", "za", "zm", "zw", "gh", "gi", "sh", "cy" };

        public static int ImageIndex
        {
            get
            {
                return m_ImageIndex;
            }
            set
            {
                m_ImageIndex = value;
            }
        }

        private static int m_ImageIndex;

        public static bool IsEnableLocator()
        {
            return string.IsNullOrEmpty(ConfigurationManager.AppSettings["FrontOffice.Debug.Locator"]) | (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FrontOffice.Debug.Locator"]) && ConfigurationManager.AppSettings["FrontOffice.Debug.Locator"].ToString().ToLower().Equals("true"));
        }

        public static Services.Contracts.GeoInformation Located(string Ip = "")
        {
            if (IsEnableLocator())
            {
                Services.Contracts.GeoInformation result;
                try
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoInformation));

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/" + (Ip.IsNotEmpty() ? Ip : string.Empty));
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                    request.Proxy = null;
                    request.Timeout = 10000;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                string responseString = reader.ReadToEnd();

                                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                                {
                                    result = (GeoInformation)jsonSerializer.ReadObject(ms);
                                }
                            }
                        }
                    }

                    return result;
                }
                catch
                {
                    return TryLocateFallback(Ip);
                }
            }
            else
                return new GeoInformation() { Ip = "Disable-Unknown", Country = "Disable-Unknown", CountryCode = "Disable-", Region = "Disable-Unknown", City = "Disable-Unknown", Timezone = "Disable-Unknown", Isp = "Disable-Unknown" };
        }

        private static Services.Contracts.GeoInformation TryLocateFallback(string Ip = "")
        {
            GeoInformation result = new GeoInformation();
            if (IsEnableLocator())
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://freegeoip.net/xml/" + (Ip.IsNotEmpty() ? Ip : string.Empty));
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                    request.Proxy = null;
                    request.Timeout = 10000;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                string responseString = reader.ReadToEnd();

                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(responseString);

                                string xmlIp = doc.SelectSingleNode("Response//IP").InnerXml;
                                string xmlCountry = doc.SelectSingleNode("Response//CountryName").InnerXml;
                                string xmlCountryCode = doc.SelectSingleNode("Response//CountryCode").InnerXml;
                                string xmlRegion = doc.SelectSingleNode("Response//RegionName").InnerXml;
                                string xmlCity = doc.SelectSingleNode("Response//City").InnerXml;
                                string timeZone = doc.SelectSingleNode("Response//TimeZone").InnerXml;

                                result.Ip = (!string.IsNullOrEmpty(xmlIp)) ? xmlIp : "-";
                                result.Country = (!string.IsNullOrEmpty(xmlCountry)) ? xmlCountry : "Unknown";
                                result.CountryCode = (!string.IsNullOrEmpty(xmlCountryCode)) ? xmlCountryCode : "-";
                                result.Region = (!string.IsNullOrEmpty(xmlRegion)) ? xmlRegion : "Unknown";
                                result.City = (!string.IsNullOrEmpty(xmlCity)) ? xmlCity : "Unknown";
                                result.Timezone = (!string.IsNullOrEmpty(timeZone)) ? timeZone : "Unknown";

                                // freegeoip does not support ISP detection
                                result.Isp = "Unknown";
                            }
                        }
                    }
                }
                catch
                {
                    result.Country = "Unknown";
                    result.CountryCode = "-";
                    result.Region = "Unknown";
                    result.City = "Unknown";
                    result.Timezone = "Unknown";
                    result.Isp = "Unknown";
                }

                if (string.IsNullOrEmpty(result.Ip))
                    TryGetWanIp(ref result);
            }
            else
            {
                result.Country = "Disable-Unknown";
                result.CountryCode = "Disable-";
                result.Region = "Disable-Unknown";
                result.City = "Disable-Unknown";
                result.Timezone = "Disable-Unknown";
                result.Isp = "Disable-Unknown";
            }

            return result;
        }


        private static void TryGetWanIp(ref GeoInformation item)
        {
            string wanIp = "-";
            if (Conversions.ToBoolean(IsEnableLocator()))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.ipify.org/");
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                    request.Proxy = null;
                    request.Timeout = 5000;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var dataStream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                wanIp = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception generatedExceptionName)
                {
                }

                item.Ip = wanIp;
            }
            else
            {
                item.Ip = "Disable-Unknown";
            }
        }
    }
}