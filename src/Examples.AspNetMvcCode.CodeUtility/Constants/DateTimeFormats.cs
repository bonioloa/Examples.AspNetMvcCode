namespace Examples.AspNetMvcCode.CodeUtility
{
    /// <summary>
    /// <para>
    /// standard and custom date/string formats used in applications
    /// </para>
    /// <para>
    /// For all standard format codes check
    /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
    /// </para>
    /// <para>
    /// For all custom format syntax check
    /// <see href="https://docs.microsoft.com/it-it/dotnet/standard/base-types/custom-date-and-time-format-strings"/><br />
    /// </para>
    /// </summary>
    public static class DateTimeFormats
    {
        /* GUIDANCE: these constants must remain internal 
         * and should not be made public and used outside this assembly 
         */

        /// <summary>
        /// "d" Standard Short date pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)<br />
        /// 2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)<br />
        /// </summary>
        /// <remarks>
        /// Localized, needs culture<br />
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-short-date-d-format-specifier"/>
        /// </remarks>
        internal const string StandardShortDateLocalized = "d";


        /// <summary>
        /// "g" Standard General date time pattern (short time).<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45 (zh-CN)<br />
        /// </summary>
        /// <remarks>
        /// Localized, needs culture<br />
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-general-date-short-time-g-format-specifier"/>
        /// </remarks>
        internal const string StandardDateTimeLocalizedGeneralTimeShort = "g";


        /// <summary>
        /// "G" Standard General date time pattern (long time)<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45:30 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45:30 (zh-CN)<br />
        /// </summary>
        /// <remarks>
        /// Localized, needs culture<br />
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-general-date-long-time-g-format-specifier"/>
        /// </remarks>
        internal const string StandardDateTimeLocalizedGeneralTimeLong = "G";


        /// <summary>
        /// "s" Standard Sortable date time pattern<br />
        /// Invariant format: "yyyy-MM-ddTHH:mm:ss". Note: it also contains a "T" between date and time
        /// </summary>
        /// <remarks>
        /// Culture invariant<br />
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-sortable-s-format-specifier"/>
        /// </remarks>
        internal const string StandardDateTimeSortable = "s";


        /// <summary>
        /// Custom Format "yyyy-MM-dd" (example 2021-09-13)<br />
        /// ISO date representation
        /// </summary>
        /// <remarks>
        /// Culture invariant<br />
        /// </remarks>
        public const string CustomDateSortable = "yyyy-MM-dd";


        /// <summary>
        /// Custom Format "dd MMMM yyyy" (example 13 Maggio 2022, needs to be localized)
        /// </summary>
        /// <remarks>
        /// Depends on culture<br />
        /// </remarks>
        internal const string CustomDateWithLiteralMonth = "dd MMMM yyyy";


        /// <summary>
        /// Format as "yyyyMMdd" (date only).<br />
        /// Can be used to read write dates on databases.
        /// </summary>
        /// <remarks>
        /// Culture invariant<br />
        /// </remarks>
        internal const string CustomDbDateOnly = "yyyyMMdd";

        /// <summary>
        /// Format as "HHmmss" (time only)<br />
        /// Can be used to read write time on databases.
        /// </summary>
        internal const string CustomDbTimeOnlyLong = "HHmmss";

        /// <summary>
        /// Format "HHmm" (time only, no seconds)
        /// </summary>
        internal const string CustomDbTimeOnlyShort = "HHmm";

        /// <summary>
        /// Format as "HH:mm:ss" (time only)
        /// </summary>
        internal const string CustomTimeSortable = "HH:mm:ss";

        /// <summary>
        /// Format as "HH.mm.ss" (time only)
        /// </summary>
        internal const string CustomTimeSortableForFileName = "HH.mm.ss";

        /// <summary>
        /// Format as "yyyyMMddHHmmss" (date and time)
        /// </summary>
        internal const string CustomDbDateTimeLong = CustomDbDateOnly + CustomDbTimeOnlyLong;

        /// <summary>
        /// Format as "yyyyMMddHHmm" (date and time, no seconds)
        /// </summary>
        internal const string CustomDbDateTimeShort = CustomDbDateOnly + CustomDbTimeOnlyShort;


        /// <summary>
        /// Format as "yyyy-MM-dd HH:mm:ss" <br />
        /// Similar to standard pattern "s", but it contains a space instead of "T" as DateTime separator
        /// </summary>
        internal readonly static string CustomDateTimeSortable =
            CustomDateSortable + " " + CustomTimeSortable;

        /// <summary>
        /// Format "yyyy-MM-dd_HH.mm.ss", suited for use in file name.
        /// </summary>
        internal readonly static string CustomDateTimeSortableForFileName =
            CustomDateSortable + "_" + CustomTimeSortableForFileName;

    }
}