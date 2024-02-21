using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the Collection data type
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Defines whether a Collection is empty or contains elements within it/ Define si una colleccion es empty or no contiene elementos dentro de ella
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsEmptyAndNotContainsItems<T>(this Collection<T> source)
    {
        bool result = false;
        if (source == null)
        {
            return true;
        }
        if (!(source.Count != 0))
        {
            return true;
        }
        return result;
    }

    /// <summary>
    /// Creates a string separated by the character indicated property values that difinio/Crea un string separado por el carácter que se indique de  los valores de la propiedad que se difinio
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static string CreateConcatenation<T>(this Collection<T> source, string separator, string propertyName)
    {
        string result = string.Empty;
        if (!(source == null) && source.Count != 0)
        {
            object temporalObject = source.FirstOrDefault();
            if (!(temporalObject == null))
            {
                bool isExistProperty = temporalObject.ExistsProperty(propertyName);
                if (isExistProperty)
                {
                    var listaVector = new List<string>();
                    foreach (var ItemSource in source)
                        listaVector.Add(Conversions.ToString(ItemSource.GetType().GetProperty(propertyName).GetValue(ItemSource, null)));
                    if (!listaVector.IsEmptyAndNotContainsItems())
                    {
                        result = string.Join(separator, listaVector);
                    }
                }
            }
        }
        return result;
    }
}