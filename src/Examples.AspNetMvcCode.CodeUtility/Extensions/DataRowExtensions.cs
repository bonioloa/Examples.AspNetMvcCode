using System;
using System.Data;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="DataRow"/>
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Requires that column type is string<br/>
        /// Does the same of sql coalesce, trims all empty characters<br/>
        /// Null safe.
        /// </summary>
        /// <remarks>VERY IMPORTANT: do not use to read encrypted columns, use directly row.Field&lt;string&gt;</remarks>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static string CoalesceAndClean(this DataRow row, string columnName)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(CoalesceAndClean)} null, column '{columnName}' ");
            }
            string tmpValue = row.Field<string>(columnName);
            return tmpValue.Clean();
        }


        /// <summary>
        /// Trim characters and remove internal newlines and tabs. Spaces and other empty characters will be preserved
        /// Null safe.
        /// </summary>
        /// <remarks>VERY IMPORTANT: use with caution and only on fields that are results of a prefix/suffix  
        /// where internal empty characters can create bad strings to display in a html context </remarks>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static string CoalesceCleanInAllString(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(CoalesceCleanInAllString)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(CoalesceCleanInAllString)} null, column '{columnName}' ");
            }
            return row.CoalesceAndClean(columnName)
                      .CleanRemoveNewLinesAndTabs();
        }


        /// <summary>
        /// Requires that column base type is decimal<br/>
        /// This is a dangerous method but we need it to clearly express when a db value
        /// is stored with the wrong type and we need to use the correct data type
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static long GetLongFromFakeDecimal(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetLongFromFakeDecimal)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetLongFromFakeDecimal)} null, column '{columnName}' ");
            }
            return (long)row.Field<decimal>(columnName);
        }


        /// <summary>
        /// this is a dangerous method but we need it to clearly express when a db value
        /// is stored with the wrong type and we need to use the correct data type
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static int GetIntFromFakeDecimal(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetIntFromFakeDecimal)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetIntFromFakeDecimal)} null, column '{columnName}' ");
            }
            return (int)row.Field<decimal>(columnName);
        }


        /// <summary>
        /// retrieves bool value indifferent to underlying numeric type. True if num > 0.<br/>
        /// Sometimes sql converts calculated values to int instead of bit, so this method prevent possible errors
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        /// <exception cref="InvalidCastException"><see cref="DBNull" />not allowed</exception>
        public static bool GetBoolFromNumOrBit(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetBoolFromNumOrBit)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetBoolFromNumOrBit)} null, column '{columnName}' ");
            }
            if (row[columnName] is DBNull)
            {
                throw new InvalidCastException($"{nameof(GetBoolFromNumOrBit)} DBNull per tipo dati bit/int non consentito, colonna '{columnName}'");
            }

            if (row[columnName] is bool rowBool)
            {
                return rowBool;
            }

            (bool success, decimal numericValue) = row[columnName].TryParseToNumericInvariant();
            if (success)
            {
                return numericValue > 0;
            }

            throw new InvalidCastException($"{nameof(GetBoolFromNumOrBit)} tipo dati non riconosciuto (necessario numerico o bool), colonna '{columnName}', tipo '{row[columnName].GetType()}'");
        }


        /// <summary>
        /// Converts the underlying string flag to boolean. <br/>
        /// Accepted true values can be "TRUE", "VERO". "S". All other values will be considered false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        /// <exception cref="InvalidCastException"><see cref="DBNull" />not allowed</exception>
        public static bool GetBoolFromFlagString(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetBoolFromFlagString)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetBoolFromFlagString)} null, column '{columnName}' ");
            }
            if (row[columnName] is DBNull)
            {
                throw new InvalidCastException($"{nameof(GetBoolFromFlagString)} DBNull per tipo dati varchar non consentito, colonna '{columnName}'");
            }

            bool? toReturn = row.GetNullableBoolFromFlagString(columnName);

            return toReturn.HasValue && (bool)toReturn;
        }


        /// <summary>
        /// Converts the underlying string flag to Nullable boolean. <br/>
        /// Accepted true values can be "TRUE", "VERO". "S". (case invariant)<br/>
        /// Accepted false values are "FALSE", "FALSE", "N" (case invariant)<br/>
        /// All other values will return null
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static bool? GetNullableBoolFromFlagString(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetNullableBoolFromFlagString)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetNullableBoolFromFlagString)} argument is empty, column '{columnName}' ");
            }
            if (row[columnName] is null || row[columnName] is DBNull)
            {
                return null;
            }

            string flagStr = row.CoalesceAndClean(columnName);

            bool? toReturn = null;
            if (flagStr.Empty())
            {
                return toReturn;
            }

            string tmp = flagStr.Clean().ToUpperInvariant();
            if (tmp == "TRUE"
                || tmp == "VERO"
                || tmp == "S")
            {
                return true;
            }

            if (tmp == "FALSE"
                || tmp == "FALSO"
                || tmp == "N")
            {
                return false;
            }
            return toReturn;
        }


        /// <summary>
        /// Reads a date from a row, where value is saved as varchar in format yyyyMMdd.<br/>
        /// Null or empty varchar will output <see cref="DateTime.MinValue"/>.Date<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DateTime GetDateFromString(this DataRow row, string columnName)
        {
            DateTime? date = row.GetNullableDateFromString(columnName);
            return date.HasValue ? (DateTime)date : DateTime.MinValue.Date;
        }


        /// <summary>
        /// Reads a date from a row, where value is saved as varchar in format yyyyMMdd.<br/>
        /// Null or empty varchar will output null value<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static DateTime? GetNullableDateFromString(this DataRow row, string columnName)
        {
            if (columnName.Empty())
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(GetNullableDateFromString)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetNullableDateFromString)} null for column '{columnName}' ");
            }
            if (row[columnName] is DBNull || row[columnName] is null)
            {
                return null;
            }

            string dateStr = row.CoalesceAndClean(columnName);

            bool success = dateStr.TryParseDbDateInvariantToNullable(out DateTime? date);
            if (!success)
            {
                throw new FormatException($"{nameof(GetNullableDateFromString)} - {columnName} is not a date '{dateStr}' ");
            }

            return date;
        }




        /// <summary>
        /// reads date time from two columns content (one for date part yyyyMMdd, one for time part HHmmss).<br/>
        /// Null or empty in just ONE of the two columns will output <see cref="DateTime.MinValue"/>.Date<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnNameDate"></param>
        /// <param name="columnNameTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromStrings(
            this DataRow row
            , string columnNameDate
            , string columnNameTime
            )
        {
            DateTime? dateTime = row.GetNullableDateTimeFromStrings(columnNameDate, columnNameTime);
            return dateTime.HasValue ? (DateTime)dateTime : DateTime.MinValue.Date;
        }


        /// <summary>
        /// reads date time from two columns content (one for date part yyyyMMdd, one for time part HHmmss).<br/>
        /// Null or empty in just ONE of the two columns will output null value<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnNameDate"></param>
        /// <param name="columnNameTime"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static DateTime? GetNullableDateTimeFromStrings(
            this DataRow row
            , string columnNameDate
            , string columnNameTime
            )
        {
            if (columnNameDate.Empty())
            {
                throw new ArgumentNullException(nameof(columnNameDate), $"{nameof(GetNullableDateTimeFromStrings)} argument is empty");
            }
            if (columnNameTime.Empty())
            {
                throw new ArgumentNullException(nameof(columnNameTime), $"{nameof(GetNullableDateTimeFromStrings)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetNullableDateTimeFromStrings)} row is null, column '{columnNameDate}' and  column '{columnNameTime}' ");
            }

            //if one of the cells is null return null anyway ignoring value of the other
            if (row[columnNameDate] is DBNull || row[columnNameDate] is null
                || row[columnNameTime] is DBNull || row[columnNameTime] is null)
            {
                return null;
            }

            string dateStr = row.CoalesceAndClean(columnNameDate);
            string timeStr = row.CoalesceAndClean(columnNameTime);

            bool success = (dateStr + timeStr).TryParseDbDateTimeLongInvariantToNullable(out DateTime? dateTime);
            if (!success)
            {
                throw new FormatException($"{nameof(GetNullableDateTimeFromStrings)} - {columnNameDate} is not a date '{dateStr}' OR  {columnNameTime} is not a date '{timeStr}' ");
            }

            return dateTime;
        }


        /// <summary>
        /// reads date time from a column content (format yyyyMMddHHmmss).<br/>
        /// Null or empty value columns will output <see cref="DateTime.MinValue"/>.Data<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnNameDateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromString(
           this DataRow row
           , string columnNameDateTime
           )
        {
            DateTime? dateTime = row.GetNullableDateTimeFromString(columnNameDateTime);
            return dateTime.HasValue ? (DateTime)dateTime : DateTime.MinValue.Date;
        }


        /// <summary>
        /// reads date time from a column content (format yyyyMMddHHmmss).<br/>
        /// Null or empty value columns will output null value<br/>
        /// A non-empty string with wrong format will throw exception
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnNameDateTime"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException">null arguments not allowed</exception>
        public static DateTime? GetNullableDateTimeFromString(
            this DataRow row
            , string columnNameDateTime
            )
        {
            if (columnNameDateTime.Empty())
            {
                throw new ArgumentNullException(nameof(columnNameDateTime), $"{nameof(GetNullableDateTimeFromString)} argument is empty");
            }
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row), $"{nameof(GetNullableDateTimeFromString)} null, column '{columnNameDateTime}' ");
            }
            if (row[columnNameDateTime] is DBNull)
            {
                return null;
            }

            string dateTimeStr = row.CoalesceAndClean(columnNameDateTime);

            bool success = dateTimeStr.TryParseDbDateTimeLongInvariantToNullable(out DateTime? dateTime);
            if (!success)
            {
                throw new FormatException($"{nameof(GetNullableDateTimeFromStrings)} - {columnNameDateTime} is not a date '{dateTimeStr}' ");
            }

            return dateTime;
        }
    }
}