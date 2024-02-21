using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace InMotionGIT.Utils.Reflection
{
    /// <summary>
    ///   <para>Static class containing Extension methods for Reflection Query</para>
    /// </summary>
    public static class Query
    {
        /// <summary>
        ///   <para>Regex to find filters in a query</para>
        ///   <para>Matches operators to check for values in sub objects e.g. Path=Value</para>
        ///   <para>Matches numeric values which are considered indexers</para>
        /// </summary>
        private static readonly Regex FilterRE = new Regex(
            @"(?<path>\w+)(\[((?<subPath>\w+)(?<operator>=)(?<value>.*)|(?<indexer>\d+))\])$");

        /// <summary>
        ///   <para>Get the first value given a query</para>
        /// </summary>
        /// <typeparam name = "T">Value type</typeparam>
        /// <param name = "data">Data</param>
        /// <param name = "query">Query</param>
        /// <returns>First or throws sequence exception</returns>
        public static T GetValue<T>(this object data, string query)
        {
            var values = GetValues<T>(data, query);
            return values.FirstOrDefault<T>();
        }

        /// <summary>
        ///   <para>Get the first value given a query</para>
        /// </summary>
        /// <typeparam name = "T">Value type</typeparam>
        /// <param name = "data">Data</param>
        /// <param name = "query">Query</param>
        /// <param name = "def">Default value</param>
        /// <returns>First or default</returns>
        public static T GetValue<T>(this object data, string query, T def)
        {
            var values = GetValues<T>(data, query);
            switch (values.Count())
            {
                case 0:
                    return def;
                default:
                    return values.FirstOrDefault<T>();
            }
        }

        /// <summary>
        ///   <para>Query an object using reflection</para>
        /// </summary>
        public static IEnumerable<T> GetValues<T>(this object data, string query)
        {
            var delimPos = query.LastIndexOf('.');
            string prevquery = query;
            IEnumerable<object> root;
            if (delimPos == -1)
            {
                root = new[] { data };
            }
            else
            {
                // recurse to get the root value
                prevquery = query.Substring(0, delimPos);
                root = GetValues<object>(data, prevquery);
                query = query.Substring(delimPos + 1);
            }

            // check for filter, and get the path without filter part
            var filterMatch = FilterRE.Match(query);
            if (filterMatch.Success)
            {
                query = filterMatch.Groups["path"].Value;
            }

             
            if (root.ElementAt(0) == null)
                throw new ArgumentNullException( string.Format("Member '{0}' is null", prevquery));
                

            // get the property or field by name, public or not
            var type = root.ElementAt(0).GetType();
            var prop = default(MemberInfo);
            while (prop == null && type != typeof(Object))
            {
                prop = type.GetMember(query, BindingFlags.Instance
                                           | BindingFlags.Public 
                                           | BindingFlags.NonPublic
                                           | BindingFlags.GetField 
                                           | BindingFlags.GetProperty ).FirstOrDefault();
                type = type.BaseType;
            }

            if (prop == null)
                throw new ArgumentException(string.Format("Member '{0}' not found", query));

            // set up the indexer or filter predicate
            var indexer = default(Func<IEnumerable<T>, T>);
            var filter = default(Func<object, bool>);

            if (filterMatch.Groups["indexer"].Success)
            {
                indexer = os => os.ElementAt(int.Parse(filterMatch.Groups["indexer"].Value));
            }
            else if (filterMatch.Groups["subPath"].Success)
            {
                switch (filterMatch.Groups["operator"].Value)
                {
                    case "=":
                        filter = o => string.Equals(o.ToString(), filterMatch.Groups["value"].Value.ToString(), StringComparison.CurrentCultureIgnoreCase);
                        break;
                }
            }

            foreach (var value in
                root.Select(rootItem => prop is FieldInfo
                                            ? (prop as FieldInfo).GetValue(rootItem)
                                            : (prop as PropertyInfo).GetValue(rootItem, null)))
            {
                if (indexer != null)
                {
                    if (!(value is IEnumerable)) throw new InvalidCastException();

                    yield return indexer((value as IEnumerable).Cast<T>());
                }
                else if (filter != null)
                {
                    // if enumerable (not a string) filterMatch each item
                    foreach (var rootItemItem in
                        value is IEnumerable && !(value is string)
                            ? (IEnumerable)value
                            : new[] { value })
                    {
                        // recurse to get value to check
                        var subValue = GetValue(rootItemItem,
                                                filterMatch.Groups["subPath"].Value, default(object));

                        if (filter(subValue))
                        {
                            yield return (T)rootItemItem;
                        }
                    }
                }
                else
                {
                    yield return (T)value;
                }        
            }
       //     yield return default(T);
        }
    }
}
