Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports InMotionGIT.Common.Services.Contracts

Namespace Helpers

    Public Class GeoLocationHelper

        Public Shared ReadOnly ImageList As String() = {"ad", "ae", "af", "ag", "ai", "al",
                                                    "am", "an", "ao", "ar", "as", "at",
                                                    "au", "aw", "ax", "az", "ba", "bb",
                                                    "bd", "be", "bf", "bg", "bh", "bi",
                                                    "bj", "bm", "bn", "bo", "br", "bs",
                                                    "bt", "bv", "bw", "by", "bz", "ca",
                                                    "catalonia", "cc", "cd", "cf", "cg",
                                                    "ch", "ci", "ck", "cl", "cm", "cn",
                                                    "co", "cr", "cs", "cu", "cv", "cx",
                                                    "cz", "de", "dj", "dk", "dm", "do",
                                                    "dz", "ec", "ee", "eg", "eh", "england",
                                                    "er", "es", "et", "europeanunion",
                                                    "fam", "fi", "fj", "fk", "fm", "fo",
                                                    "fr", "ga", "gb", "gd", "ge", "gf",
                                                    "gl", "gm", "gn", "gp", "gq", "gr",
                                                    "gs", "gt", "gu", "gw", "gy", "hk",
                                                    "hm", "hn", "hr", "ht", "hu", "id",
                                                    "ie", "il", "in", "io", "iq", "ir",
                                                    "is", "it", "jm", "jo", "jp", "ke",
                                                    "kg", "kh", "ki", "km", "kn", "kp",
                                                    "kr", "kw", "ky", "kz", "la", "lb",
                                                    "lc", "li", "lk", "lr", "ls", "lt",
                                                    "lu", "lv", "ly", "ma", "mc", "md",
                                                    "me", "mg", "mh", "mk", "ml", "mm",
                                                    "mn", "mo", "mp", "mq", "mr", "ms",
                                                    "mt", "mu", "mv", "mw", "mx", "my",
                                                    "mz", "na", "nc", "ne", "nf", "ng",
                                                    "ni", "nl", "no", "np", "nr", "nu",
                                                    "nz", "om", "pa", "pe", "pf", "pg",
                                                    "ph", "pk", "pl", "pm", "pn", "pr",
                                                    "ps", "pt", "pw", "py", "qa", "re",
                                                    "ro", "rs", "ru", "rw", "sa", "sb",
                                                    "sc", "scotland", "sd", "se", "sg",
                                                    "si", "sj", "sk", "sl", "sm", "sn",
                                                    "so", "sr", "st", "sv", "sy", "sz",
                                                    "tc", "td", "tf", "tg", "th", "tj",
                                                    "tk", "tl", "tm", "tn", "to", "tr",
                                                    "tt", "tv", "tw", "tz", "ua", "ug",
                                                    "um", "us", "uy", "uz", "va", "vc",
                                                    "ve", "vg", "vi", "vn", "vu", "wales",
                                                    "wf", "ws", "ye", "yt", "za", "zm",
                                                    "zw", "gh", "gi", "sh", "cy"}

        Public Shared Property ImageIndex() As Integer
            Get
                Return m_ImageIndex
            End Get
            Set
                m_ImageIndex = Value
            End Set
        End Property

        Private Shared m_ImageIndex As Integer

        Public Shared Function IsEnableLocator()
            Return ConfigurationManager.AppSettings("FrontOffice.Debug.Locator") = Nothing Or (ConfigurationManager.AppSettings("FrontOffice.Debug.Locator") <> Nothing AndAlso ConfigurationManager.AppSettings("FrontOffice.Debug.Locator").ToString().ToLower().Equals("true"))
        End Function

        Public Shared Function Located(Optional Ip As String = "") As Services.Contracts.GeoInformation
            If IsEnableLocator() Then
                Dim result As Services.Contracts.GeoInformation
                Try
                    Dim jsonSerializer As New DataContractJsonSerializer(GetType(GeoInformation))

                    Dim request As HttpWebRequest = DirectCast(WebRequest.Create("http://ip-api.com/json/" + IIf(Ip.IsNotEmpty, Ip, String.Empty)), HttpWebRequest)
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0"
                    request.Proxy = Nothing
                    request.Timeout = 10000

                    Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                        Using dataStream As Stream = response.GetResponseStream()
                            Using reader As New StreamReader(dataStream)
                                Dim responseString As String = reader.ReadToEnd()

                                Using ms As New MemoryStream(Encoding.UTF8.GetBytes(responseString))
                                    result = DirectCast(jsonSerializer.ReadObject(ms), GeoInformation)
                                End Using
                            End Using
                        End Using
                    End Using

                    Return result
                Catch
                    Return TryLocateFallback(Ip)
                End Try
            Else
                Return New GeoInformation With {.Ip = "Disable-Unknown", .Country = "Disable-Unknown", .CountryCode = "Disable-", .Region = "Disable-Unknown", .City = "Disable-Unknown", .Timezone = "Disable-Unknown", .Isp = "Disable-Unknown"}
            End If
        End Function

        Private Shared Function TryLocateFallback(Optional Ip As String = "") As Services.Contracts.GeoInformation
            Dim result As New GeoInformation()
            If IsEnableLocator() Then
                Try
                    Dim request As HttpWebRequest = DirectCast(WebRequest.Create("http://freegeoip.net/xml/" + IIf(Ip.IsNotEmpty, Ip, String.Empty)), HttpWebRequest)
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0"
                    request.Proxy = Nothing
                    request.Timeout = 10000

                    Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                        Using dataStream As Stream = response.GetResponseStream()
                            Using reader As New StreamReader(dataStream)
                                Dim responseString As String = reader.ReadToEnd()

                                Dim doc As New XmlDocument()
                                doc.LoadXml(responseString)

                                Dim xmlIp As String = doc.SelectSingleNode("Response//IP").InnerXml
                                Dim xmlCountry As String = doc.SelectSingleNode("Response//CountryName").InnerXml
                                Dim xmlCountryCode As String = doc.SelectSingleNode("Response//CountryCode").InnerXml
                                Dim xmlRegion As String = doc.SelectSingleNode("Response//RegionName").InnerXml
                                Dim xmlCity As String = doc.SelectSingleNode("Response//City").InnerXml
                                Dim timeZone As String = doc.SelectSingleNode("Response//TimeZone").InnerXml

                                result.Ip = If((Not String.IsNullOrEmpty(xmlIp)), xmlIp, "-")
                                result.Country = If((Not String.IsNullOrEmpty(xmlCountry)), xmlCountry, "Unknown")
                                result.CountryCode = If((Not String.IsNullOrEmpty(xmlCountryCode)), xmlCountryCode, "-")
                                result.Region = If((Not String.IsNullOrEmpty(xmlRegion)), xmlRegion, "Unknown")
                                result.City = If((Not String.IsNullOrEmpty(xmlCity)), xmlCity, "Unknown")
                                result.Timezone = If((Not String.IsNullOrEmpty(timeZone)), timeZone, "Unknown")

                                ' freegeoip does not support ISP detection
                                result.Isp = "Unknown"
                            End Using
                        End Using
                    End Using
                Catch
                    result.Country = "Unknown"
                    result.CountryCode = "-"
                    result.Region = "Unknown"
                    result.City = "Unknown"
                    result.Timezone = "Unknown"
                    result.Isp = "Unknown"

                End Try

                If String.IsNullOrEmpty(result.Ip) Then
                    TryGetWanIp(result)
                End If
            Else
                result.Country = "Disable-Unknown"
                result.CountryCode = "Disable-"
                result.Region = "Disable-Unknown"
                result.City = "Disable-Unknown"
                result.Timezone = "Disable-Unknown"
                result.Isp = "Disable-Unknown"
            End If

            Return result
        End Function

        Private Shared Sub TryGetWanIp(ByRef item As Services.Contracts.GeoInformation)
            Dim wanIp As String = "-"
            If IsEnableLocator() Then
                Try
                    Dim request As HttpWebRequest = DirectCast(WebRequest.Create("http://api.ipify.org/"), HttpWebRequest)
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0"
                    request.Proxy = Nothing
                    request.Timeout = 5000

                    Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                        Using dataStream As Stream = response.GetResponseStream()
                            Using reader As New StreamReader(dataStream)
                                wanIp = reader.ReadToEnd()
                            End Using
                        End Using
                    End Using
                Catch generatedExceptionName As Exception
                End Try

                item.Ip = wanIp
            Else
                item.Ip = "Disable-Unknown"
            End If
        End Sub

    End Class

End Namespace