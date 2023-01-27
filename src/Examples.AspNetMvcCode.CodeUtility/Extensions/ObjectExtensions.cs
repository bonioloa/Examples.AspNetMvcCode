using System;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Examples.AspNetMvcCode.CodeUtility.Tests")]//necessary for testing without making methods public
namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="object"/>. Only internal for now
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Tries to convert the object to decimal. <br/>
        /// In case object is a string, try parsing with <see cref="CultureInfo.InvariantCulture"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static (
            bool success
            , decimal numericValue
            ) TryParseToNumericInvariant(this object obj)
        {
            bool success = false;
            decimal numericValue = decimal.MinValue;
            if (obj is null || obj is DBNull)
            {
                return (success, numericValue);
            }

            string s = Convert.ToString(obj, CultureInfo.InvariantCulture);
            success = decimal.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out numericValue);
            if (!success)
            {
                numericValue = decimal.MinValue;
            }
            return (success, numericValue);
        }

        /// <summary>
        /// checks if type is a explicit numeric type, excludes all other types even if they can be parsed to numeric (like string)
        /// </summary>
        /// <remarks>null returns false</remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool IsNumericStructType(this object obj)
        {
            if (obj is null || obj is DBNull || obj is string || obj is char)
            {
                return false;
            }

            switch (obj)
            {
                case sbyte _:
                case byte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case float _:
                case double _:
                case decimal _:
                    return true;

                default:
                    return false;
            }
        }
    }
}