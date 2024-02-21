#Region "using"

Imports System.IO
Imports System.Runtime.CompilerServices

#End Region

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the DataRow type
    ''' </summary>
    Public Module DataRowExtensions

#Region "Binary (BLOB) Extension"

        <Extension()>
        Public Function FileContent(value As DataRow, name As String, path As String, filename As String, extension As String) As String
            Dim result As String = String.Empty

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                Dim localfilename As String = path
                If filename.IsEmpty Then
                    localfilename &= "\" & Guid.NewGuid().ToString()
                Else
                    localfilename &= "\" & filename
                End If
                If extension.StartsWith(".") Then
                    localfilename &= extension
                Else
                    localfilename &= "." & extension
                End If
                localfilename = localfilename.Replace("\\", "\")

                Using FS As New FileStream(localfilename, FileMode.Create)
                    Dim blob As Byte() = DirectCast(value(name), Byte())
                    FS.Write(blob, 0, blob.Length)
                    FS.Close()
                End Using

                result = localfilename

            End If

            Return result
        End Function

#End Region

#Region "Numeric Extension"

        <Extension()>
        Public Function NumericValue(value As DataRow, name As String) As Decimal
            Dim result As Decimal = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

        <Extension()>
        Public Function IntegerValue(value As DataRow, name As String) As Integer
            Dim result As Integer = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

        <Extension()>
        Public Function ByteValue(value As DataRow, name As String) As Byte
            Dim result As Byte = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

        <Extension()>
        Public Function NumericValueWithFormatDefault(value As DataRow, name As String, format As String, defaultValue As Object, ByRef specified As Boolean) As Object
            Dim result As Object = defaultValue

            If value.IsNotNull(name) Then
                Dim internal As Decimal = NumericValue(value, name)

                If format.IsNotEmpty Then
                    result = internal.ToString(format, New System.Globalization.CultureInfo("en-US", False))
                Else
                    result = internal
                End If

                specified = True
            Else
                specified = False
            End If

            Return result
        End Function

#End Region

#Region "Double Extension"

        <Extension()>
        Public Function DoubleValue(value As DataRow, name As String) As Double
            Dim result As Double = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

#End Region

#Region "DateTime Extension"

        <Extension()>
        Public Function DateTimeValue(value As DataRow, name As String) As Date
            Dim result As Date = Date.MinValue

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

        <Extension()>
        Public Function DateTimeValueWithFormatDefault(value As DataRow, name As String, format As String, defaultValue As Object) As Object
            Dim result As Object = defaultValue

            If value.IsNotNull(name) Then
                Dim internal As Date = DateTimeValue(value, name)

                If format.IsNotEmpty Then
                    result = internal.ToString(format)
                Else
                    result = internal
                End If
            End If

            Return result
        End Function

#End Region

#Region "Boolean Extension"

        <Extension()>
        Public Function BooleanValue(value As DataRow, name As String) As Boolean
            Dim result As Boolean = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = (value.Item(name) = 1)
            End If

            Return result
        End Function

        <Extension()>
        Public Function BooleanCharValue(value As DataRow, name As String) As Boolean
            Dim result As Boolean = 0

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = (value.Item(name) = "1")
            End If

            Return result
        End Function

#End Region

#Region "String Extension"

        <Extension()>
        Public Function StringValue(value As DataRow, name As String) As String
            Dim result As String = String.Empty

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
                result = result.Trim
            End If

            Return result
        End Function

        <Extension()>
        Public Function StringValueWithDefault(value As DataRow, name As String, defaultValue As Object, ByRef specified As Boolean) As String
            Dim result As String = defaultValue

            If value.IsNotNull(name) Then
                result = StringValue(value, name)
                specified = True
            Else
                specified = False
            End If

            Return result
        End Function

        <Extension()>
        Public Function StringHourValue(value As DataRow, name As String) As Date
            Dim result As Date = Date.MinValue
            Dim currentHour As Integer = 0
            Dim currentMinute As Integer = 0
            Dim currentValue As String

            If Not IsNothing(value.Item(name)) AndAlso
           Not IsDBNull(value.Item(name)) Then
                currentValue = value.Item(name)
                currentValue = currentValue.Trim
                If currentValue.IndexOf(":") > 0 Then
                    currentHour = currentValue.Split(":")(0)
                    currentMinute = currentValue.Split(":")(1)
                End If
            End If

            result = result.AddHours(currentHour)
            result = result.AddMinutes(currentMinute)

            Return result
        End Function

#End Region

#Region "Specials Extension"

        <Extension()>
        Public Function GuidValue(value As DataRow, name As String) As Guid
            Dim result As Guid = Guid.NewGuid

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = value.Item(name)
            End If

            Return result
        End Function

        <Extension()>
        Public Function XmlValue(value As DataRow, name As String) As XDocument
            Dim result As New XDocument

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = XDocument.Parse(value.StringValue(name))
            End If

            Return result
        End Function

        <Extension()>
        Public Function SwitchValue(value As DataRow, name As String) As Boolean
            Dim result As Boolean = False

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = (value.Item(name) = 1)
            End If

            Return result
        End Function

        <Extension()>
        Public Function SwitchCharValue(value As DataRow, name As String) As Boolean
            Dim result As Boolean = False

            If Not IsNothing(value.Item(name)) AndAlso Not IsDBNull(value.Item(name)) Then
                result = (value.Item(name) = "1")
            End If

            Return result
        End Function

#End Region

#Region "Behavior Functions"

        <Extension()>
        Public Function IsNotNull(value As DataRow, name As String) As Boolean
            Return Not IsDBNull(value.Item(name))
        End Function

        <Extension()>
        Public Function EnumValue(Of T)(ByVal value As DataRow, name As String, ByVal enumType As T, ByRef specified As Boolean, ByRef witherror As Boolean) As T

            Dim result As T = Nothing

            If value.IsNotNull(name) Then
                Try
                    result = CType([Enum].Parse(enumType.GetType, value.Item(name)), T)
                    specified = True
                Catch ex As ArgumentException
                    result = Nothing
                    specified = False
                    witherror = True
                End Try
            Else
                result = Nothing
                specified = False
            End If

            Return result
        End Function

#End Region

    End Module

End Namespace