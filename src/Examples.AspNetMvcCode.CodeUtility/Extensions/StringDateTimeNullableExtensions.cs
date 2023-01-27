using System;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="string"/> to <see cref="Nullable{DateTime}"/> as <see cref="T:Nullable&lt;DateTime&gt;"/>  
    /// </summary>
    public static class StringDateTimeNullableExtensions
    {
        /// <summary>
        /// Converts string to nullable DateTime using <see cref="CultureInfo.InvariantCulture"/>. Does not throw exception.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="format">Can be a explicit format (like" yyyyMMdd") or a standard pattern (like "d" or "s" </param>
        /// <param name="dateTime">out</param>
        /// <returns></returns>
        internal static bool TryParseToNullableDateTimeInvariant(this string dateStr, string format, out DateTime? dateTime)
        {
            bool success;
            try
            {
                if (dateStr.Empty())
                {
                    success = true;//false only on conversion error, empty is an allowed value because it sets date value to null
                    dateTime = null;
                }
                else
                {
                    dateTime =
                        DateTime.ParseExact(
                            dateStr.ToString(),
                            format
                            , CultureInfo.InvariantCulture
                            );
                    success = true;
                }
            }
            catch (Exception)
            {
                //Log.Logger.Warning($"{nameof(AppTypeConvert)} {nameof(StringToNullableDateTime)} conversion failed. Value {dateStr}, format {format}, exception {ex.Message}");
                //throw new CommonException();//do not throw !!!
                success = false;
                dateTime = null;
            }
            return success;
        }


        /// <summary>
        /// Try to convert string using format as "yyyyMMdd" (date only).<br />
        /// Can be used to read write dates on databases.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="date">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateInvariantToNullable(this string dateStr, out DateTime? date)
        {
            return dateStr.TryParseToNullableDateTimeInvariant(DateTimeFormats.CustomDbDateOnly, out date);
        }


        /// <summary>
        /// Try to convert string using standard format "s" (format "yyyy-MM-ddTHH:mm:ss")
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool TryParseSortableDateTimeInvariantStandardToNullable(this string dateStr, out DateTime? dateTime)
        {
            return dateStr.TryParseToNullableDateTimeInvariant(DateTimeFormats.StandardDateTimeSortable, out dateTime);
        }


        /// <summary>
        /// Try to convert string using custom format "yyyy-MM-dd" (example 2021-09-13)<br />
        /// ISO date representation
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="date">out</param>
        /// <returns></returns>
        public static bool TryParseSortableDateInvariantToNullable(this string dateStr, out DateTime? date)
        {
            return dateStr.TryParseToNullableDateTimeInvariant(DateTimeFormats.CustomDateSortable, out date);
        }


        /// <summary>
        /// Try to convert string using format as "yyyyMMddHHmm" (date and time, no seconds)
        /// </summary>
        /// <param name="dateTimeStr"></param>
        /// <param name="dateTime">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateTimeShortInvariantToNullable(this string dateTimeStr, out DateTime? dateTime)
        {
            return dateTimeStr.TryParseToNullableDateTimeInvariant(DateTimeFormats.CustomDbDateTimeShort, out dateTime);
        }


        /// <summary>
        /// Try to convert string using format as "yyyyMMddHHmmss" (date and time)
        /// </summary>
        /// <param name="dateTimeStr"></param>
        /// <param name="dateTime">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateTimeLongInvariantToNullable(this string dateTimeStr, out DateTime? dateTime)
        {
            return dateTimeStr.TryParseToNullableDateTimeInvariant(DateTimeFormats.CustomDbDateTimeLong, out dateTime);
        }
    }
}