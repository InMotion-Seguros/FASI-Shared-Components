Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json

Namespace Helpers

    'Dim dcs As New DataContractSerializer(GetType(Contract.IndividualRisk))
    'Dim writer As New MemoryStream
    'Dim xdw As XmlDictionaryWriter = _
    '    XmlDictionaryWriter.CreateTextWriter(writer, Encoding.UTF8)
    'dcs.WriteObject(xdw, risk)
    'writer.Flush()

    'writer.Position = 0
    'Dim xx As String = UTF8Encoding.UTF8.GetString(writer.ToArray())
    'Dim reader As New StreamReader(writer)
    'Dim str = reader.ReadToEnd()

    ''Dim x As String = writer.Read
    ''http://msdn.microsoft.com/en-us/library/ms731073.aspx

    Public NotInheritable Class Serialize

        Private Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' JSON Serialization
        ''' </summary>
        Public Shared Function JsonSerializer(Of T)(ByVal obj As T) As String
            Dim ser As New Runtime.Serialization.Json.DataContractJsonSerializer(GetType(T))
            Dim ms As New IO.MemoryStream()
            ser.WriteObject(ms, obj)
            Dim jsonString As String = Encoding.UTF8.GetString(ms.ToArray())
            ms.Close()
            'Replace Json Date String
            Dim p As String = "\\/Date\((\d+)\+\d+\)\\/"
            Dim matchEvaluator As New MatchEvaluator(AddressOf ConvertJsonDateToDateString)
            Dim reg As New Regex(p)
            jsonString = reg.Replace(jsonString, matchEvaluator)
            Return jsonString
        End Function

        ''' <summary>
        ''' Convert Serialization Time /Date(1319266795390+0800) as String
        ''' </summary>
        Private Shared Function ConvertJsonDateToDateString(ByVal m As Match) As String
            Dim result As String = String.Empty
            Dim dt As New DateTime(1970, 1, 1)
            dt = dt.AddMilliseconds(Long.Parse(m.Groups(1).Value))
            dt = dt.ToLocalTime()
            result = dt.ToString("yyyy-MM-dd HH:mm:ss")
            Return result
        End Function

        ''' <summary>
        ''' JSON Deserialization
        ''' </summary>
        Public Shared Function JsonDeserialize(Of T)(ByVal jsonString As String) As T
            'Convert "yyyy-MM-dd HH:mm:ss" String as "\/Date(1319266795390+0800)\/"
            Dim p As String = "\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}"
            Dim matchEvaluator As New MatchEvaluator(AddressOf ConvertDateStringToJsonDate)
            Dim reg As New Regex(p)
            jsonString = reg.Replace(jsonString, matchEvaluator)
            Dim ser As New Runtime.Serialization.Json.DataContractJsonSerializer(GetType(T))
            Dim ms As New MemoryStream(Encoding.UTF8.GetBytes(jsonString))
            Dim obj As T = DirectCast(ser.ReadObject(ms), T)
            Return obj
        End Function

        ''' <summary>
        ''' Convert Date String as Json Time
        ''' </summary>
        Private Shared Function ConvertDateStringToJsonDate(ByVal m As Match) As String
            Dim result As String = String.Empty
            Dim dt As DateTime = DateTime.Parse(m.Groups(0).Value)
            dt = dt.ToUniversalTime()
            Dim ts As TimeSpan = dt - DateTime.Parse("1970-01-01")
            result = String.Format("\/Date({0}+0800)\/", ts.TotalMilliseconds)
            Return result
        End Function

        Public Shared Function SerializeJSON(Of T)(current As T) As String
            Dim result As String = ""
            Return JsonConvert.SerializeObject(current, Newtonsoft.Json.Formatting.Indented, New JsonSerializerSettings With {.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, .PreserveReferencesHandling = PreserveReferencesHandling.Objects, .TypeNameHandling = TypeNameHandling.All})
            Return result
        End Function

        Public Shared Function DeserializeJSON(Of T)(ByVal body As String) As T
            Dim result As T
            result = JsonConvert.DeserializeObject(Of T)(body, New JsonSerializerSettings With {
                .PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                .StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                .TypeNameHandling = TypeNameHandling.All})

            Return result
        End Function

        Public Shared Function Serialize(Of T)(current As T) As String
            Dim serializedObj As String
            Dim serializer As New XmlSerializer(GetType(T))
            Using writer As New StringWriterWithEncoding(Encoding.UTF8)
                serializer.Serialize(writer, current)
                serializedObj = writer.ToString()
                Return serializedObj
            End Using
        End Function

        Public Shared Sub SerializeToFile(Of T)(current As T,
                                                fullFileName As String)
            SerializeToFile(Of T)(current, fullFileName, False)
        End Sub

        Public Shared Function IsSerializable(serializableObject As Object) As Boolean
            Dim typeObjec As Type = serializableObject.GetType()
            Return ((TypeOf typeObjec Is ISerializable) OrElse (Attribute.IsDefined(typeObjec, GetType(SerializableAttribute))))
        End Function

        Public Shared Function SerializarObject(serializerObject As Object) As String
            Dim result As New StringBuilder(String.Empty)
            Try
                Using writer As New StringWriter(result)
                    Dim xs As New XmlSerializer(serializerObject.GetType)
                    xs.Serialize(writer, serializerObject)
                End Using
            Catch ex As Exception
                result.AppendLine(String.Format("This object type '{0}' can not by serialized", serializerObject.GetType))
            End Try
            Return result.ToString()
        End Function

        Public Shared Sub SerializeToFile(Of T)(current As T,
                                                fullFileName As String,
                                                withFormat As Boolean)
            Dim xmlSerialiazerItem As New XmlSerializer(GetType(T))
            Dim fileStreamItem As New FileStream(fullFileName, FileMode.Create)
            Dim xmlTextWriterItem As New XmlTextWriter(fileStreamItem, Encoding.UTF8)

            If withFormat Then
                With xmlTextWriterItem
                    .Formatting = Xml.Formatting.Indented
                    .Indentation = 2
                    .IndentChar = " "
                End With
            End If
            xmlSerialiazerItem.Serialize(xmlTextWriterItem, current)
            xmlTextWriterItem.Close()
            fileStreamItem.Close()
            fileStreamItem = Nothing
        End Sub

        Public Shared Function Deserialize(Of T)(ByVal xmlDocument As String) As T
            Using vlcFileStream As New StringReader(xmlDocument)
                Dim vloXmlSerializer As New XmlSerializer(GetType(T))
                Dim metadataItem As T = vloXmlSerializer.Deserialize(vlcFileStream)
                Return metadataItem
            End Using
        End Function

        Public Shared Function DeserializeFromFile(Of T)(ByVal fullFileName As String) As T
            Dim xmlSerialiazerItem As New XmlSerializer(GetType(T))
            Dim fileStreamItem As New FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim xmlTextReaderItem As New XmlTextReader(fileStreamItem)
            Dim metadataItem As T = CType(xmlSerialiazerItem.Deserialize(xmlTextReaderItem), T)

            xmlTextReaderItem.Close()
            fileStreamItem.Close()
            fileStreamItem = Nothing

            Return metadataItem
        End Function

        Public Shared Function BinaryDeserializeFromFile(Of T)(ByVal fullFileName As String) As T
            Dim formatter As IFormatter = New BinaryFormatter()
            Dim fileStreamItem As New FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim metadataItem As T = CType(formatter.Deserialize(fileStreamItem), T)

            fileStreamItem.Close()
            fileStreamItem = Nothing

            Return metadataItem
        End Function

        Public Shared Sub BinarySerializeToFile(Of T)(current As T, ByVal fullFileName As String)
            Dim formatter As IFormatter = New BinaryFormatter()
            Dim writer As New FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None)
            formatter.Serialize(writer, current)
            writer.Close()
        End Sub

        Public Shared Sub DataContractSerializeToFile(Of T)(current As T, ByVal fullFileName As String)
            Dim writer As New FileStream(fullFileName, FileMode.Create)
            Dim ser As New DataContractSerializer(GetType(T))
            ser.WriteObject(writer, current)
            writer.Close()
        End Sub

        Public Shared Function DataContractDeserializeFromFile(Of T)(fullFileName As String) As T
            Dim fs As New FileStream(fullFileName, FileMode.OpenOrCreate)
            Dim ser As New DataContractSerializer(GetType(T))
            Dim metadataItem As T = ser.ReadObject(fs)
            fs.Close()

            Return metadataItem
        End Function

        Public Shared Function Clone(Of T)(currentObject As T) As T
            Return Deserialize(Of T)(Serialize(Of T)(currentObject))
        End Function

    End Class

    Friend Class StringWriterWithEncoding
        Inherits StringWriter
        Private ReadOnly _encoding As Encoding

        Public Sub New()
        End Sub

        Public Sub New(formatProvider As IFormatProvider)
            MyBase.New(formatProvider)
        End Sub

        Public Sub New(sb As StringBuilder)
            MyBase.New(sb)
        End Sub

        Public Sub New(sb As StringBuilder, formatProvider As IFormatProvider)
            MyBase.New(sb, formatProvider)
        End Sub

        Public Sub New(encoding As Encoding)
            _encoding = encoding
        End Sub

        Public Sub New(formatProvider As IFormatProvider, encoding As Encoding)
            MyBase.New(formatProvider)
            _encoding = encoding
        End Sub

        Public Sub New(sb As StringBuilder, encoding As Encoding)
            MyBase.New(sb)
            _encoding = encoding
        End Sub

        Public Sub New(sb As StringBuilder, formatProvider As IFormatProvider, encoding As Encoding)
            MyBase.New(sb, formatProvider)
            _encoding = encoding
        End Sub

        Public Overrides ReadOnly Property Encoding() As Encoding
            Get
                Return If((_encoding Is Nothing), MyBase.Encoding, _encoding)
            End Get
        End Property

    End Class

End Namespace