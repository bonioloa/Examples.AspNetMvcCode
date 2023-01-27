using System;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="string"/> with numerics output
    /// </summary>
    public static class StringNumericsExtensions
    {
        private static bool TryParseSafe(
            this string decimalStr
            , bool allowThousands
            , out decimal number
            , CultureInfo culture = null
            )
        {
            bool success = false;
            number = decimal.MinValue;

            if (culture is null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            NumberStyles style =
                allowThousands

                ? NumberStyles.AllowDecimalPoint
                    | NumberStyles.AllowThousands
                    | NumberStyles.AllowLeadingSign

                : NumberStyles.AllowDecimalPoint
                    | NumberStyles.AllowLeadingSign;


            try
            {
                number = decimal.Parse(decimalStr, style, culture);
                success = true;
            }
#pragma warning disable IDE0059 // Unnecessary assignment of a value (debug reasons)
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
            {
                //by construction, don't rethrow
            }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return success;
        }



        /// <summary>
        /// Tries to convert a string to decimal.<br/>
        /// Sign and decimal separator all allowed.<br/>
        /// Thousands separators will fail the conversion
        /// </summary>
        /// <param name="decimalStr"></param>
        /// <param name="number"></param>
        /// <param name="culture"></param>
        /// <returns>Conversion result</returns>
        /// <remarks>Does not throw exception</remarks>
        public static bool TryParseSafeNoThousandsSeparator(
            this string decimalStr
            , out decimal number
            , CultureInfo culture = null
            )
        {
            return TryParseSafe(decimalStr, allowThousands: false, out number, culture);
        }



        /// <summary>
        /// Tries to convert a string to decimal.<br/>
        /// Sign, thousands and decimal separator all allowed.<br/>
        /// </summary>
        /// <param name="decimalStr"></param>
        /// <param name="number"></param>
        /// <param name="culture"></param>
        /// <returns>Conversion result</returns>
        /// <remarks>
        /// Does not throw exception.<br/>
        /// Be cautious with this method. A decimal separator can be mistaken as thousands separator for difference cultures
        /// </remarks>
        public static bool TryParseSafe(
            this string decimalStr
            , out decimal number
            , CultureInfo culture = null
            )
        {
            return TryParseSafe(decimalStr, allowThousands: true, out number, culture);
        }
    }
}