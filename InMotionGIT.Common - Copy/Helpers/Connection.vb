Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.Web

Namespace Helpers

    Public Class Connection

        ''' <summary>
        ''' Determina si la ip es local o no
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsLocalIpAddress() As Boolean
            If GeoLocationHelper.IsEnableLocator() Then
                If Not IsNothing(HttpContext.Current) Then
                    If Not IsNothing(HttpContext.Current.Request) Then
                        If Not IsNothing(HttpContext.Current.Request.Url) Then
                            If Not String.IsNullOrEmpty(HttpContext.Current.Request.Url.Host) Then
                                Return IsLocalIpAddress(HttpContext.Current.Request.Url.Host)
                            Else
                                Return False
                            End If
                        Else
                            Return False
                        End If
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Determina si la ip es local o no
        ''' </summary>
        ''' <param name="host"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsLocalIpAddress(host As String) As Boolean
            If GeoLocationHelper.IsEnableLocator() Then
                Try
                    Dim hostIPs As IPAddress() = Dns.GetHostAddresses(host)
                    ' get local IP addresses
                    Dim localIPs As IPAddress() = Dns.GetHostAddresses(Dns.GetHostName())

                    If Not HttpContext.Current.Request.IsLocal Then
                        Return False
                    End If

                    For Each hostIP As IPAddress In hostIPs
                        ' is local-host
                        If IPAddress.IsLoopback(hostIP) Then
                            Return True
                        End If

                        ' is local address
                        For Each localIP As IPAddress In localIPs
                            If hostIP.Equals(localIP) Then
                                Return True
                            End If
                        Next
                    Next
                Catch
                    Return False
                End Try
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Method que extrae el nombre de la maquina que hace el requests
        ''' </summary>
        ''' <param name="ipSource">Ip de origen</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function NameHostFromIp(ipSource As String) As String
            Try
                If GeoLocationHelper.IsEnableLocator() Then
                    Dim hostName As String = Dns.GetHostEntry(ipSource).HostName
                    If Not String.IsNullOrEmpty(hostName) Then
                        Return hostName.ToUpper
                    Else
                        Return ipSource
                    End If
                Else
                    Return ipSource
                End If
            Catch ex As Exception
                Return ipSource
            End Try
        End Function

        ''' <summary>
        ''' Trying to get the public IP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIPPublic() As String
            Try
                Dim ExternalIP As String = Helpers.GeoLocationHelper.Located.Ip
                Return ExternalIP
            Catch
                Return GetIPRequest()
            End Try
        End Function

        ''' <summary>
        ''' Trying to get the public IP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIPEnviroment() As String
            Try
                If IsLocalIpAddress() Then
                    Return InMotionGIT.Common.Helpers.Connection.NameHostFromIp(InMotionGIT.Common.Helpers.Connection.GetIPOnlyRequest())
                Else
                    Return GetIPOnlyRequest()
                End If
            Catch
                Return GetIPRequest()
            End Try
        End Function

        Public Shared Function GetIPRequestsWithOutHostName() As String
            Return GetIPOnlyRequest()
        End Function

        ''' <summary>
        ''' Get ip request.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIPRequest() As String
            Return NameHostFromIp(GetIPOnlyRequest)
        End Function

        Public Shared Function GetIPOnlyRequest() As String
            Dim Result As String = String.Empty
            If GeoLocationHelper.IsEnableLocator() Then
                If System.Web.Hosting.HostingEnvironment.IsHosted Then
                    Try

                        If HttpContext.Current IsNot Nothing Then
                            If HttpContext.Current.Request IsNot Nothing Then
                                If HttpContext.Current.Request.ServerVariables IsNot Nothing Then
                                    If HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR") IsNot Nothing Then
                                        Result = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
                                    End If
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(Result) Then
                            If HttpContext.Current IsNot Nothing Then
                                If HttpContext.Current.Request IsNot Nothing Then
                                    If HttpContext.Current.Request.ServerVariables IsNot Nothing Then
                                        If HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") IsNot Nothing Then
                                            Result = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(Result) AndAlso Not IsNothing(OperationContext.Current) Then
                            Dim clientEndpoint As RemoteEndpointMessageProperty = TryCast(OperationContext.Current.IncomingMessageProperties(RemoteEndpointMessageProperty.Name),
                                                                           RemoteEndpointMessageProperty)
                            Result = clientEndpoint.Address
                        End If

                        If String.Equals(Result, "::1", StringComparison.CurrentCultureIgnoreCase) Then
                            Result = "127.0.0.1"
                        End If

                        If String.IsNullOrEmpty(Result) Then
                            Result = "127.0.0.1"
                        End If

                        Return Result
                    Catch ex As Exception
                        Return NameHostFromIp("127.0.0.1")
                    End Try
                Else
                    Try
                        Dim SystemAC As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())
                        For Each item In SystemAC.AddressList
                            If item.AddressFamily = AddressFamily.InterNetwork Then
                                Result = item.ToString
                                Exit For
                            End If
                        Next
                        Return Result
                    Catch ex As Exception
                        Return NameHostFromIp("127.0.0.1")
                    End Try
                End If
            Else
                Return "Disable"
            End If

        End Function

        Public Shared Function GetBroadcastAddress(address As IPAddress, subnetMask As IPAddress) As IPAddress
            Dim ipAdressBytes As Byte() = address.GetAddressBytes()
            Dim subnetMaskBytes As Byte() = subnetMask.GetAddressBytes()

            If ipAdressBytes.Length <> subnetMaskBytes.Length Then
                Throw New ArgumentException("Lengths of IP address and subnet mask do not match.")
            End If

            Dim broadcastAddress As Byte() = New Byte(ipAdressBytes.Length - 1) {}
            For i As Integer = 0 To broadcastAddress.Length - 1
                broadcastAddress(i) = CByte(ipAdressBytes(i) Or (subnetMaskBytes(i) Xor 255))
            Next
            Return New IPAddress(broadcastAddress)
        End Function

        Public Shared Function GetNetworkAddress(address As IPAddress, subnetMask As IPAddress) As IPAddress
            Dim ipAdressBytes As Byte() = address.GetAddressBytes()
            Dim subnetMaskBytes As Byte() = subnetMask.GetAddressBytes()

            If ipAdressBytes.Length <> subnetMaskBytes.Length Then
                Throw New ArgumentException("Lengths of IP address and subnet mask do not match.")
            End If

            Dim broadcastAddress As Byte() = New Byte(ipAdressBytes.Length - 1) {}
            For i As Integer = 0 To broadcastAddress.Length - 1
                broadcastAddress(i) = CByte(ipAdressBytes(i) And (subnetMaskBytes(i)))
            Next
            Return New IPAddress(broadcastAddress)
        End Function

        Public Shared Function IsInSameSubnet(ipServer As String, ipHost As String, subnetMask As String) As Boolean
            Return IsInSameSubnet(IPAddress.Parse(ipServer), IPAddress.Parse(ipHost), IPAddress.Parse(subnetMask))
        End Function

        Public Shared Function IsInSameSubnet(ipServer As IPAddress, ipHost As IPAddress, subnetMask As IPAddress) As Boolean
            Dim network1 As IPAddress = GetNetworkAddress(ipHost, subnetMask)
            Dim network2 As IPAddress = GetNetworkAddress(ipServer, subnetMask)

            Return network1.Equals(network2)
        End Function

        Public Shared Function GetSubnetMask() As IPAddress
            Return GetSubnetMask(IPAddress.Parse(GetIPOnlyRequest()))
        End Function

        Public Shared Function GetSubnetMask(address As IPAddress) As IPAddress
            For Each adapter As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
                For Each unicastIPAddressInformation As UnicastIPAddressInformation In adapter.GetIPProperties().UnicastAddresses
                    If unicastIPAddressInformation.Address.AddressFamily = AddressFamily.InterNetwork Then
                        If address.Equals(unicastIPAddressInformation.Address) Then
                            Return unicastIPAddressInformation.IPv4Mask
                        End If
                    End If
                Next
            Next
            Throw New ArgumentException(String.Format("Can't find subnetmask for IP address '{0}'", address))
        End Function

    End Class

End Namespace