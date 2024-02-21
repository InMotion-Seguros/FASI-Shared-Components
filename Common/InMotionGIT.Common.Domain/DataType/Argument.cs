using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Domain.DataType;


[DataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
[Serializable()]
[XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
[XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
public class Argument
{

    [DataMember()]
    [XmlAttribute()]
    public string Alias { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public string Type { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public string Assemblies { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public string Name { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public bool IsCollection { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public bool FileContent { get; set; }

    public List<Argument> Arguments { get; set; }

    [XmlIgnore()]
    public string FullName
    {
        get
        {
            return string.Format("{0}.{1}", Type, Name);
        }
    }

    [XmlIgnore()]
    public Type RealType
    {
        get
        {
            Type result = null;

            string currentFullName = FullName;

            if (Type.IndexOf("/") > -1)
            {
                currentFullName = Type.Split('/')[Type.Split('/').Length - 1];
            }

            foreach (System.Reflection.Assembly AssemblyItem in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!AssemblyItem.FullName.StartsWith("inrule.", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (Type typeItem in AssemblyItem.GetTypes())
                    {
                        if ((currentFullName ?? "") == (typeItem.FullName ?? ""))
                        {
                            result = typeItem;
                            break;
                        }
                    }

                    if (!(result == null))
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }

    [XmlIgnore()]
    [Browsable(false)]
    public string FormatDBType
    {
        get
        {
            string kind = FullName;

            if (kind.Contains("System."))
            {
                kind = kind.Split('.')[1];
            }

            switch (kind.ToUpper() ?? "")
            {
                case "STRING":
                case "BOOLEAN":
                    {
                        return "VARCHAR(1)";
                    }

                case "INT16":
                case "INT32":
                case "DECIMAL":
                case "INT64":
                    {
                        return "NUMBER";
                    }

                case "DATETIME":
                    {
                        return "DATE";
                    }

                default:
                    {
                        return "VARCHAR(1)";
                    }
            }
        }
    }

}