using System;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="DateTime"/>. 
    /// </summary>
    public static class DateTimeExtensions
    {
        /*INTERNAL NOTE FOR THIS CLASS
         * 
         * If possible just write the code for every new method in DateTimeNullableExtensions
         * and here call the method casting the DateTime object to DateTime?
         */


        /// <summary>
        /// checks if date equals <see cref="DateTime.MinValue"/> Date part. (time will not be considered)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsMinDateValue(this DateTime date) => date.Date.Equals(DateTime.MinValue.Date);


        /// <summary>
        /// checks if data equal <see cref="DateTime.MaxValue"/> Date part. (time will not be considered)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsMaxDateValue(this DateTime date) => date.Date.Equals(DateTime.MaxValue.Date);


        /* GUIDANCE
         * for this methods, just call the nullable DateTime versions and simply cast output
         */

        /// <summary>
        /// Convert to string "d" Standard Short date pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)<br />
        /// 2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)<br />
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringShortDate(this DateTime dateTime, CultureInfo culture)
        {
            return ((DateTime?)dateTime).ToStringShortDate(culture);
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
        public static string ToStringDateTimeGeneralLong(this DateTime dateTime, CultureInfo culture)
        {
            return ((DateTime?)dateTime).ToStringDateTimeGeneralLong(culture);
        }


        /// <summary>
        /// Convert to string custom Format "dd MMMM yyyy" (example 13 Maggio 2022, needs to be localized)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringDateWithLiteralMonth(this DateTime dateTime, CultureInfo culture)
        {
            return ((DateTime?)dateTime).ToStringDateWithLiteralMonth(culture);
        }


        /// <summary>
        /// Convert string using standard culture invariant format "s".<br/>
        /// Will result in format "yyyy-MM-ddTHH:mm:ss"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeSortableInvariantStandard(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToStringDateTimeSortableInvariantStandard();
        }


        /// <summary>
        /// Convert to string with custom format "yyyy-MM-dd". Culture invariant
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateSortableInvariant(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToStringDateSortableInvariant();
        }


        /// <summary>
        /// Convert to string format as "yyyy-MM-dd HH:mm:ss" <br />
        /// Similar to standard pattern "s", but it contains a space instead of "T" as DateTime separator
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeInvariant(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToStringDateTimeInvariant();
        }


        /// <summary>
        /// Convert to string format "yyyy-MM-dd_HH.mm.ss", suited for use in file name.
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringDateTimeInvariantForFileName(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToStringDateTimeInvariantForFileName();
        }


        /// <summary>
        /// Convert to string Format as "yyyyMMdd" (date only).<br />
        /// Can be used to read write dates on databases.
        /// </summary>
        /// <remarks>Culture invariant</remarks>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringDateInvariant(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToDbStringDateInvariant();
        }


        /// <summary>
        /// Convert to string format as "yyyyMMddHHmm" (date and time, no seconds)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringDateTimeShortInvariant(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToDbStringDateTimeShortInvariant();
        }


        /// <summary>
        /// Convert to string format as "HHmmss" (time 24 hours invariant format, no separators)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDbStringTimeInvariant(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).ToStringSafe(DateTimeFormats.CustomDbTimeOnlyLong);
        }

    }
}