using System.Collections.Generic;
using System.Linq;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// check if collection is null or is empty.
        /// Don't use it for a single string!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => !source?.Any() ?? true;

        /// <summary>
        /// check if collection is not null and has at least one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <returns></returns>
        public static bool HasValues<T>(this IEnumerable<T> me) => !IsNullOrEmpty(me);


        /// <summary>
        /// checks if ALL provided values are present in source enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            if (source.IsNullOrEmpty() && values.HasValues())
            {
                return false;
            }
            if (values.IsNullOrEmpty())
            {
                return true;//empty or null guarantees it's always contained in source
            }
            //NOTE there is also a string invariant version
            return values.All(value => source.Contains(value));
        }





        /// <summary>
        /// Checks whether all items in the enumerable are same (Uses <see cref="object.Equals(object)" /> to check for equality)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// Returns true if there is 0 or 1 item in the enumerable <br/>
        /// or if all items in the enumerable are same (equal to each other) <br/>
        /// otherwise false.
        /// </returns>
        public static bool AreAllSame<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.IsNullOrEmpty())
            {
                return true;
            }

            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                T toCompare = default;
                if (enumerator.MoveNext())
                {
                    toCompare = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    if (toCompare != null && !toCompare.Equals(enumerator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// checks if in this <see cref="IEnumerable{T}"/> are present duplicates. <br/> 
        /// If null or empty are provided, returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        /// <remarks>Null safe</remarks>
        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.IsNullOrEmpty())
            {
                return false;
            }
            return enumerable.Distinct().Count() != enumerable.Count();
        }

        /// <summary>
        /// finds the duplicated values in this <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        /// <remarks>Null safe. returns Empty list if no duplicates were found</remarks>
        public static IList<T> GetDuplicates<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.IsNullOrEmpty() || !enumerable.HasDuplicates())
            {
                return new List<T>();
            }
            return enumerable.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(y => y.Key)
                             .ToList();
        }
    }
}