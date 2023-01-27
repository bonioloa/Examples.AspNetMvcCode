using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="IDictionary{TKey, TValue}"/>
    /// </summary>
    public static class IDictionaryExtension
    {
        /// <summary>
        /// check if dictionary collection is null or is empty
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="me"></param>
        /// <returns></returns>
        public static bool HasValues<TK, TV>(this IDictionary<TK, TV> me) => !IsNullOrEmpty(me);

        /// <summary>
        /// check if dictionary collection is not null and has at least one element
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="me"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TK, TV>(this IDictionary<TK, TV> me) => !me?.Any() ?? true;

        /// <summary>
        /// change all keys of provided dictionary to lowercase
        /// </summary>
        /// <remarks>throws exception if dictionary contains a key that once minimized is equal to another key</remarks>
        /// <param name="me"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToLowerInvariant(this IDictionary<string, string> me)
        {
            IDictionary<string, string> toReturn = new Dictionary<string, string>();
            if (me is null || me.Keys.IsNullOrEmpty())
            {
                return toReturn;
            }

            string tmpKey;
            foreach (string key in me.Keys)
            {
                tmpKey = key.ToLowerInvariant();
                if (me.ContainsKey(tmpKey))
                {
                    throw new ArgumentException($"key {key} already exists minimized in provided IDictionary");
                }
                toReturn.Add(key.ToLowerInvariant(), me[key].ToLowerInvariant());
            }
            return toReturn;
        }
    }
}