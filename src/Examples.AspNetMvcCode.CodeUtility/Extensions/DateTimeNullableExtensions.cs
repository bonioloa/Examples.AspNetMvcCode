using System;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="Nullable{DateTime}"/> as <see cref="T:Nullable&lt;DateTime&gt;"/>  
    /// </summary>
    public static class DateTimeNullableExtensions
    {
        /// <summary>
        /// casts a DateTime? to DateTime class using as default <see cref="DateTime.MinValue"/>.Date part
        /// </summary>
        /// <param name="nullableDateTime"></param>
        /// <returns><see cref="DateTime"/></returns>
        public static DateTime SafeCastWithMinDateAsDefault(this DateTime? nullableDateTime)
        {
            return nullableDateTime.HasValue ? (DateTime)nullableDateTime : DateTime.MinValue.Date;
        }

        /* GUIDANCE
         * keep ToStringSafe method internal and wrap it with static format methods
         */

        /// <summary>
        /// convert DateTime? to provided string format 
        /// <br/>
        /// (can be standard or custom <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
        /// </summary>
        /// <remarks>Do not use this method if in this class the string format is already handled by another method</remarks>
        /// <exception cref="DateTime.ToString(string)"></exception>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <param name="culture"></param>
        /// <returns>If value is null, returns <see cref="string.Empty"/>, else the formatted string dateTime</returns>
        internal static string ToStringSafe(this DateTime? dateTime, string format, CultureInfo culture = null)
        {
            if (culture is null)
            {
                culture = CultureInfo.InvariantCulture;
            }
            string toReturn = string.Empty;
            if (dateTime.HasValue)
            {
                toReturn = ((DateTime)dateTime).ToString(format, culture);
            }
            return toReturn;
        }


        /// <summary>
        /// Convert to string "d" Standard Short date pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)<br />
        /// 2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)<br />
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringShortDate(this DateTime? dateTime, CultureInfo culture)
        {
            return dateTime.ToStringSafe(DateTimeFormats.StandardShortDateLocalized, culture);
        }


        /// <summary>
        /// Convert to string "G" Standard General date time pattern (long time)<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45:30 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45:30 (zh-CN)<br />
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringDateTimeGeneralLong(this DateTime? dateTime, CultureInfo culture)
        {
            return dateTime.ToStringSafe(DateTimeFormats.StandardDateTimeLocalizedGeneralTimeLong, culture);
        }


        /// <summary>
        /// Convert string using standard culture invariant format "s".<br/>
        /// Will result in format "yyyy-MM-ddTHH:mm:ss"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeSortableInvariantStandard(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.StandardDateTimeSortable);
        }


        /// <summary>
        /// Convert to string with custom format "yyyy-MM-dd". Culture invariant
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateSortableInvariant(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDateSortable);
        }


        /// <summary>
        /// Convert to string custom Format "dd MMMM yyyy" (example 13 Maggio 2022, needs to be localized)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringDateWithLiteralMonth(this DateTime? dateTime, CultureInfo culture)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDateWithLiteralMonth, culture);
        }


        /// <summary>
        /// Convert to string format as "yyyy-MM-dd HH:mm:ss" <br />
        /// Similar to standard pattern "s", but it contains a space instead of "T" as DateTime separator
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeInvariant(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDateTimeSortable);
        }


        /// <summary>
        /// Convert to string format "yyyy-MM-dd_HH.mm.ss", suited for use in file name.
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeInvariantForFileName(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDateTimeSortableForFileName);
        }


        /// <summary>
        /// Convert to string format as "yyyyMMdd" (date only).<br />
        /// Can be used to read write dates on databases.
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringDateInvariant(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDbDateOnly);
        }


        /// <summary>
        /// Convert to string format as "yyyyMMddHHmm" (date and time, no seconds)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringDateTimeShortInvariant(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDbDateTimeShort);
        }

        /// <summary>
        /// Convert to string format as "HHmmss" (time 24 hours invariant format, no separators)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringTimeInvariant(this DateTime? dateTime)
        {
            return dateTime.ToStringSafe(DateTimeFormats.CustomDbTimeOnlyLong);
        }
    }
}