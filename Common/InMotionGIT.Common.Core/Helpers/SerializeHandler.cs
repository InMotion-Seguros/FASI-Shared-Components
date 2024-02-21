using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using InMotionGIT.Common.Core.Extensions;
using System.Xml;

namespace InMotionGIT.Common.Core.Helpers
{
    public static class SerializeHandler<T>
    {
        public static string Serialize(T current)
        {
            string serializedObj = null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8))
            {
                serializer.Serialize(writer, current);
                serializedObj = writer.ToString();
            }
            return serializedObj;
        }

        public static void SerializeToFile(T current, string fullFileName)
        {
            SerializeToFile(current, fullFileName, false);
        }

        public static void SerializeJSONToFile(T current, string fullFileName, bool withFormat = false, bool PreserveReferences = true, bool IgnoreNull = false)
        {
            string body = SerializeJSON(current, withFormat, PreserveReferences, IgnoreNull);
            System.IO.File.WriteAllText(fullFileName, body, Encoding.ASCII);
        }

        public static string SerializeJSON(T current, bool withFormat, bool PreserveReferences = true, bool IgnoreNull = false, TypeNameHandling typeNameHandling = TypeNameHandling.All)
        {
            var config = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            if (IgnoreNull)
            {
                config.NullValueHandling = NullValueHandling.Ignore;
            }

            var varIdented = Newtonsoft.Json.Formatting.Indented;
            if (!withFormat)
            {
                config.Formatting = Newtonsoft.Json.Formatting.None;
                varIdented = Newtonsoft.Json.Formatting.None;
            }
            else
            {
                varIdented = Newtonsoft.Json.Formatting.Indented;
            }

            if (typeNameHandling != TypeNameHandling.All)
            {
                config.TypeNameHandling = typeNameHandling;
                config.PreserveReferencesHandling = PreserveReferencesHandling.None;
            }

            return JsonConvert.SerializeObject(current, varIdented, config);
        }

        public static JObject SerializeToJObject(T item)
        {
            JObject data = JObject.FromObject(item,
                                               new Newtonsoft.Json.JsonSerializer
                                               {
                                                   PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                   StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                                                   TypeNameHandling = TypeNameHandling.All
                                               });
            return data;
        }

        public static string SerializeJSONToArrayBytes(T current, bool withFormat)
        {
            return JsonConvert.SerializeObject(current, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public static T DeserializeJSONFromFile(string fullFileName, string encoding = "")
        {
            string body = "";
            if (encoding.IsEmpty())
                body = System.IO.File.ReadAllText(fullFileName);
            else
                body = System.IO.File.ReadAllText(fullFileName, System.Text.Encoding.GetEncoding(encoding));

            body = body.Replace("Gears.Commons.Entity.SSH.ServerSSH", "Gears.Commons.Entity.SSH.SFTPServer");
            body = body.Replace("Gears.Commons.Entity.SSH.FileDownloadSSH", "Gears.Commons.Entity.SSH.SFTPFileDownload");
            body = body.Replace("Gears.Commons.Entity.SSH.CommandSSH", "Gears.Commons.Entity.SSH.SSHCommand");

            T result = JsonConvert.DeserializeObject<T>(body, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.All });

            return result;
        }

        public static T DeserializeJSONArrayBytes(string body)
        {
            T result = JsonConvert.DeserializeObject<T>(body, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.All });

            return result;
        }

        public static T DeserializeJSON(string body)
        {
            body = body.Replace("CubeCode.AppFactory.Entity.Entities.UI.Tab", "CubeCode.AppFactory.Entity.Entities.UI.Layout.Tab");
            T result = JsonConvert.DeserializeObject<T>(body, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                TypeNameHandling = TypeNameHandling.All,
                Error = (sender, ErrorEventArgs) =>
                {
                    var rrrr = sender;
                    var rm = ErrorEventArgs;
                }
            });

            return result;
        }

        public static JObject DeserializeJSONByJObject(string body)
        {
            JObject result = JObject.Parse(body);
            return result;
        }

        public static void SerializeToFile(T current, string fullFileName, bool withFormat)
        {
            XmlSerializer xmlSerialiazerItem = new XmlSerializer(typeof(T));
            FileStream fileStreamItem = new FileStream(fullFileName, FileMode.Create);
            XmlTextWriter xmlTextWriterItem = new XmlTextWriter(fileStreamItem, Encoding.UTF8);

            if (withFormat)
            {
                var _with1 = xmlTextWriterItem;
                _with1.Formatting = System.Xml.Formatting.Indented;
                _with1.Indentation = 2;
                _with1.IndentChar = ' ';
            }
            xmlSerialiazerItem.Serialize(xmlTextWriterItem, current);
            xmlTextWriterItem.Close();
            fileStreamItem.Close();
            fileStreamItem = null;
        }

        public static bool IsSerializable(object serializableObject)
        {
            Type typeObjec = serializableObject.GetType();
            return ((typeObjec is ISerializable) || (Attribute.IsDefined(typeObjec, typeof(SerializableAttribute))));
        }

        public static string SerializarObject(object serializerObject)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            try
            {
                using (StringWriter writer = new StringWriter(result))
                {
                    XmlSerializer xs = new XmlSerializer(serializerObject.GetType());
                    xs.Serialize(writer, serializerObject);
                }
            }
            catch (Exception ex)
            {
                result.AppendLine(string.Format("This object type '{0}' can not by serialized. {1}", serializerObject.GetType(), ex.Message));
            }
            return result.ToString();
        }

        public static T Deserialize(string xmlDocument)
        {
            using (StringReader vlcFileStream = new StringReader(xmlDocument))
            {
                XmlSerializer vloXmlSerializer = new XmlSerializer(typeof(T));
                T metadataItem = (T)vloXmlSerializer.Deserialize(vlcFileStream);
                return metadataItem;
            }
        }

        public static T DeserializeFromFile(string fullFileName)
        {
            XmlSerializer xmlSerialiazerItem = new XmlSerializer(typeof(T));
            FileStream fileStreamItem = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader xmlTextReaderItem = new XmlTextReader(fileStreamItem);
            T metadataItem = (T)xmlSerialiazerItem.Deserialize(xmlTextReaderItem);

            xmlTextReaderItem.Close();
            fileStreamItem.Close();
            fileStreamItem = null;

            return metadataItem;
        }

        public static T BinaryDeserializeFromFile(string fullFileName)
        {
            try
            {
                using (StreamReader fileStream = File.OpenText(fullFileName))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

                    // Deserializar directamente desde el stream
                    T item = (T)serializer.Deserialize(fileStream, typeof(T));
                    return item;
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                // Por ejemplo, puedes registrar el error o lanzar una excepción personalizada
                throw new Exception($"Error during deserialization: {ex.Message}", ex);
            }
        }

        public static void SerializeToFile<T>(T current, string fullFileName)
        {
            try
            {
                // Serializar el objeto a JSON
                string jsonString = System.Text.Json.JsonSerializer.Serialize(current);

                // Escribir el JSON a un archivo
                File.WriteAllText(fullFileName, jsonString);
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                throw new Exception($"Error during serialization: {ex.Message}", ex);
            }
        }

        public static T Clone(T currentObject)
        {
            return Deserialize(Serialize(currentObject));
        }
    }

    internal class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb) : base(sb)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider)
        {
        }

        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding) : base(formatProvider)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding) : base(sb, formatProvider)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return (_encoding == null) ? base.Encoding : _encoding; }
        }
    }
}