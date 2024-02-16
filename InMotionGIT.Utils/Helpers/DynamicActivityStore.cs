//-----------------------------------------------------------------------
// <copyright file="DynamicActivityStore.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
//-----------------------------------------------------------------------
namespace InMotionGIT.Utils.Helpers
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implements a cache of DynamicActivity instances.
    /// </summary>
    public static class DynamicActivityStore
    {
        /// <summary>
        /// The regular expression that matches the first occurence of an environment variable.
        /// </summary>
        private static Regex envVarSearchExpression = new Regex(@"%([^%]+)%", RegexOptions.Compiled);

        /// <summary>
        /// A thread safe dictionary holding DynamicActivity instances keyed by their file path (environment vars unresolved).
        /// </summary>
        private static IDictionary<string, DynamicActivity> activityDictionary = new ThreadSafeDictionary<string, DynamicActivity>();

        /// <summary>
        /// Returns the requested activity from the cache. If not found, loads the activity from the supplied path, caches it and returns the activity.
        /// </summary>
        /// <param name="path">The path which is the unique identifier of the required activity.</param>
        /// <param name="key"></param>
        /// <returns>The requested activity.</returns>
        public static DynamicActivity GetActivity(string path, string key)
        {
            DynamicActivity dynamicActivity = null;
            if (!activityDictionary.TryGetValue(key, out dynamicActivity))
            {
                dynamicActivity = ActivityXamlServices.Load(ReplaceEnvironmentVariables(path)) as DynamicActivity;
                activityDictionary[key] = dynamicActivity;
            }

            return dynamicActivity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveKey(string key)
        {
            if (activityDictionary.ContainsKey(key))
                activityDictionary.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            activityDictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
            return activityDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Resolves all environement variables in the supplied path. Nested environment variables are recursively resolved.
        /// </summary>
        /// <param name="path">The file path that requires resolving.</param>
        /// <returns>A string with the environment variables delimited with % resolved to their values.</returns>
        public static string ReplaceEnvironmentVariables(string path)
        {
            Match match = envVarSearchExpression.Match(path);
            if (match.Groups.Count > 1)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    string varName = match.Groups[i].Value;
                    string varValue = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Process);
                    if (varValue == null)
                    {
                        varValue = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.User);
                        if (varValue == null)
                        {
                            varValue = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Machine);
                            if (varValue == null)
                            {
                                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Environment variable '{0}' not defined", varName));
                            }
                        }
                    }

                    path = ReplaceEnvironmentVariables(path.Replace("%" + varName + "%", varValue));
                }

                return path;
            }
            else
            {
                return path;
            }
        }
    }
}
