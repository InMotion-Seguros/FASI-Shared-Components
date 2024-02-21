using System;
using System.Collections.Generic;
using System.Linq;

namespace InMotionGIT.Common.Extensions
{
    public static class IEnumerableExtensions
    {

        /// <summary>
        /// This method allows to make a 'String.Join' of an object that allows the IEnumerable / Este método permite hacer un 'String.Join' de un objeto que permite el IEnumerable
        /// </summary>
        /// <typeparam name="T">Type of object/ Tipo del objeto</typeparam>
        /// <typeparam name="TProp">Type property for which you want to join/Tipo propiedad por la que se desea hacer el join</typeparam>
        /// <param name="source">Colección o objeto que heredan de IEnumerable/Collection or object inheriting from IEnumerable</param>
        /// <param name="[property]">Property for which you want to join/Propiedad por la que se desea hacer el join</param>
        /// <param name="separator">Value with which it is desired to carry out the separation of the values/ vValor con los que se desea realizar la separación de los valores</param>
        /// <returns></returns>
        public static string JoinToString<T, TProp>(this IEnumerable<T> source, Func<T, TProp> property, string separator)
        {

            string result = string.Empty;
            if (separator is null)
            {
                throw new ArgumentNullException("separator");
            }

            if (source.IsEmpty())
            {
                throw new ArgumentNullException("source");
            }

            if (source.Count() == 0)
            {
                throw new ArgumentNullException("source");
            }

            var list = source.Select(property).ToList();

            result = string.Join(separator, list);

            return result;
        }

        public static List<TProp> GetProperties<T, TProp>(this IEnumerable<T> seq, Func<T, TProp> selector)
        {
            return seq.Select(selector).ToList();
        }

        // Public Sub ForEachToJoin(Of T)(col As IEnumerable(Of T), action As Action(Of T))
        // Dim contet As Integer
        // If action Is Nothing Then
        // Throw New ArgumentNullException("action")
        // End If
        // For Each item In col
        // contet
        // action(item)
        // Next
        // End Sub

    }

}