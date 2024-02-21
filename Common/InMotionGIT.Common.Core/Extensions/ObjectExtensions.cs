 
using System.Reflection;
using Newtonsoft.Json;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the object data type
/// </summary>
public static class ObjectExtensions
{
    public static T Performance<T>(this T item, Action<T> work)
    {
        work(item);
        return item;
    }

    public static T With<T>(this T item, Action<T> work)
    {
        work(item);
        return item;
    }

    public static bool IsEmpty(this object value)
    {
        return (value == null);
    }

    

    public static T Clone<T>(this object value)
    {
        string json = JsonConvert.SerializeObject(value);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static bool CheckPropertyExists(this object obj, string name)
    {
        Type objectType = obj.GetType();
        PropertyInfo propertyInfo = objectType.GetProperty(name);

        return propertyInfo != null;
    }
    public static bool IsNotEmpty(this Dictionary<string, object> source)
    {
        var result = default(bool);
        if (source is not null && source.Count != 0)
        {
            result = true;
        }
        return result;
    }

    public static string ToStringExtended(this Dictionary<string, string> source)
    {
        return string.Join(", ", source.Select(kvp => string.Format("{0}= {1}", kvp.Key, kvp.Value)).ToArray());
    }

    /// <summary>
    /// Este método permite realizar una clonación completa por medio de memberwiseclone
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T CloneObject<T>(this T source) where T : class
    {
        if (source is null)
        {
            return null;
        }
        var inst = source.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (inst is not null)
        {
            T result = (T)inst.Invoke(source, null);
            CloneObjectBase(source, result);
            return result;
        }
        else
        {
            return null;
        }
    }

    private static void CloneObjectBase<T>(T source, T result) where T : class
    {
        foreach (System.Reflection.PropertyInfo itemPropertyes in source.GetType().GetProperties())
        {
            if (IsNotCoreType(itemPropertyes.PropertyType))
            {
                var instanceObjectFromBase = GetPropValue(source, itemPropertyes.Name);
                var inntanceObjectCloned = CloneInternal(instanceObjectFromBase);
                if (result.IsNotEmpty())
                {
                    var propertyResult = result.GetType().GetProperty(itemPropertyes.Name);
                    if (propertyResult.IsNotEmpty())
                    {
                        propertyResult.SetValue(result, Convert.ChangeType(inntanceObjectCloned, propertyResult.PropertyType), null);
                    }
                }
                CloneObjectBase(instanceObjectFromBase, inntanceObjectCloned);
            }
        }
    }

    private static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    private static T CloneInternal<T>(T source) where T : class
    {
        if (source is null)
        {
            return null;
        }
        var inst = source.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (inst is not null)
        {
            T result = (T)inst.Invoke(source, null);
            return result;
        }
        else
        {
            return null;
        }
    }

    public static bool IsNotCoreType(Type type)
    {
        return type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object;
    }

     

    public static bool IsNotEmpty(this object value)
    {
        return !(value == null);
    }

    /// <summary>
    /// Checks whether the property in the object/ Verifica si existe la propiedad en el objeto
    /// </summary>
    /// <param name="srcObject"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool ExistsProperty(this object srcObject, string propertyName)
    {
        if (srcObject is null)
        {
            throw new ArgumentNullException("srcObject");
        }

        if (propertyName is null || string.IsNullOrEmpty(propertyName) || propertyName.Length == 0)
        {
            throw new ArgumentException("Property name cannot be empty or null.");
        }

        var propInfoSrcObj = srcObject.GetType().GetProperty(propertyName);

        return propInfoSrcObj is not null;
    }

    public static Type GetTypeProperty(this object srcObject, string propertyName)
    {
        if (srcObject is null)
        {
            throw new ArgumentNullException("srcObject");
        }

        if (propertyName is null || string.IsNullOrEmpty(propertyName) || propertyName.Length == 0)
        {
            throw new ArgumentException("Property name cannot be empty or null.");
        }

        var propInfoSrcObj = srcObject.GetType().GetProperty(propertyName);

        return propInfoSrcObj.PropertyType;
    }
}