using System.Collections.Generic;
using System.Linq;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="IEnumerable{T}"/> as <see cref="T:IEnumerable&lt;string&gt;"/>  
    /// </summary>
    public static class IEnumerableStringExtensions
    {
        /// <summary>
        /// checks if ALL provided values are present in source enumerable, ignoring string case
        /// </summary>
        /// <param name="source"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAllInvariant(this IEnumerable<string> source, IEnumerable<string> values)
        {
            if (source.IsNullOrEmpty() && values.HasValues())
            {
                return false;
            }
            if (values.IsNullOrEmpty())
            {
                return true;//empty or null guarantees it's always contained in source
            }
            return
                values.All(
                    co =>
                        source.Select(v => v.ToUpperInvariant())
                              .Contains(co.ToUpperInvariant())
                        );
        }
    }
}