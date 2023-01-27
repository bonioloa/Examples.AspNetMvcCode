using System;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for Numerics types
    /// </summary>
    public static class NumericsExtensions
    {
        /// <summary>
        /// true if value is less or equal to 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool NumberInvalid<T>(this T value) where T : struct, IComparable<T>
        {
            return value.CompareTo(default) <= 0;
        }
        /// <summary>
        /// true if nullable value is null or equal/less to 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool NumberInvalid<T>(this T? value) where T : struct, IComparable<T>
        {
            if (value is null)
            {
                return true;
            }
            return NumberInvalid((T)value);
        }


        #region sbyte

        /// <summary>
        /// true if input is equal or less than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this sbyte input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or equal or less than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this sbyte? input) => input.NumberInvalid();


        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this sbyte input) => !input.NumberInvalid();
        /// <summary>
        /// true if nullable input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this sbyte? input) => !input.NumberInvalid();
        #endregion

        #region byte

        /// <summary>
        /// true if input is 0 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this byte input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this byte? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this byte input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this byte? input) => !input.NumberInvalid();
        #endregion


        #region short
        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this short input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this short? input) => input.NumberInvalid();
        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this short input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this short? input) => !input.NumberInvalid();
        #endregion

        #region ushort
        /// <summary>
        /// true if input is 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this ushort input) => input.NumberInvalid();
        /// <summary>
        /// true if input is null or 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this ushort? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this ushort input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this ushort? input) => !input.NumberInvalid();
        #endregion


        #region int
        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this int input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this int? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this int input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this int? input) => !input.NumberInvalid();
        #endregion

        #region uint
        /// <summary>
        /// true if input is 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this uint input) => input.NumberInvalid();
        /// <summary>
        /// true if input is null or 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this uint? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this uint input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this uint? input) => !input.NumberInvalid();
        #endregion


        #region long
        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this long input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this long? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this long input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this long? input) => !input.NumberInvalid();
        #endregion

        #region ulong
        /// <summary>
        /// true if input is 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this ulong input) => input.NumberInvalid();
        /// <summary>
        /// true if input is null or 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this ulong? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this ulong input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this ulong? input) => !input.NumberInvalid();
        #endregion


        #region float

        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this float input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this float? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this float input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this float? input) => !input.NumberInvalid();
        #endregion


        #region double

        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this double input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this double? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this double input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this double? input) => !input.NumberInvalid();
        #endregion


        #region decimal

        /// <summary>
        /// convert decimal to string, using format "G" (no thousands separator)
        /// </summary>
        /// <param name="number"></param>
        /// <param name="culture">Optional, mainly used to determine what decimal separator to use</param>
        /// <returns></returns>
        public static string ToStringGeneral(this decimal number, CultureInfo culture = null)
        {
            if (culture is null)
            {
                culture = CultureInfo.InvariantCulture;
            }
            return number.ToString(NumericsFormats.StandardGeneral, culture);
        }

        /// <summary>
        /// true if input is 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this decimal input) => input.NumberInvalid();
        /// <summary>
        /// true if nullable input is null or 0 or less
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Invalid(this decimal? input) => input.NumberInvalid();

        /// <summary>
        /// true if input is more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this decimal input) => !input.NumberInvalid();
        /// <summary>
        /// true if input is not null and more than 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Valid(this decimal? input) => !input.NumberInvalid();
        #endregion
    }
}