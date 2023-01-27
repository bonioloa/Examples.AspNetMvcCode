using System;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="string"/> to <see cref="DateTime"/>
    /// </summary>
    public static class StringDateTimeExtensions
    {
        /// <summary>
        /// Try to convert string using format as "yyyyMMdd" (date only).<br />
        /// Can be used to read write dates on databases.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="date">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateInvariant(this string dateStr, out DateTime date)
        {
            bool success = dateStr.TryParseDbDateInvariantToNullable(out DateTime? dateResult);
            date = dateResult.SafeCastWithMinDateAsDefault();
            return success;
        }


        /// <summary>
        /// Try to convert string using standard format "s" (format "yyyy-MM-ddTHH:mm:ss")
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool TryParseSortableDateTimeInvariantStandard(this string dateStr, out DateTime dateTime)
        {
            bool success = dateStr.TryParseSortableDateTimeInvariantStandardToNullable(out DateTime? dateResult);
            dateTime = dateResult.SafeCastWithMinDateAsDefault();
            return success;
        }


        /// <summary>
        /// Try to convert string using custom Format "yyyy-MM-dd" (example 2021-09-13)<br />
        /// ISO date representation
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="date">out</param>
        /// <returns></returns>
        public static bool TryParseSortableDateInvariant(this string dateStr, out DateTime date)
        {
            bool success = dateStr.TryParseSortableDateInvariantToNullable(out DateTime? result);
            date = result.SafeCastWithMinDateAsDefault();
            return success;
        }


        /// <summary>
        /// Try to convert string using format as "yyyyMMddHHmm" (date and time, no seconds)
        /// </summary>
        /// <param name="dateTimeStr"></param>
        /// <param name="dateTime">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateTimeShortInvariant(this string dateTimeStr, out DateTime dateTime)
        {
            bool success = dateTimeStr.TryParseDbDateTimeShortInvariantToNullable(out DateTime? result);
            dateTime = result.SafeCastWithMinDateAsDefault();
            return success;
        }


        /// <summary>
        /// Try to convert string using format as "yyyyMMddHHmmss" (date and time)
        /// </summary>
        /// <param name="dateTimeStr"></param>
        /// <param name="dateTime">out</param>
        /// <returns></returns>
        public static bool TryParseDbDateTimeLongInvariant(this string dateTimeStr, out DateTime dateTime)
        {
            bool success = dateTimeStr.TryParseDbDateTimeLongInvariantToNullable(out DateTime? result);
            dateTime = result.SafeCastWithMinDateAsDefault();
            return success;
        }
    }
}