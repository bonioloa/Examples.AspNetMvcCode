using Examples.AspNetMvcCode.CodeUtility.Extensions;
using System.Globalization;

namespace Examples.AspNetMvcCode.CodeUtility
{
    /// <summary>
    /// this class is necessary to obtain explicit date formats by culture for standard patterns
    /// because some of this patterns must be built manually, 
    /// there is no way with .NET classes to directly get the globalized formats
    /// </summary>
    public static class GlobalizedPattern
    {
        /// <summary>
        /// "G" Standard General date time pattern (long time)<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45:30 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45:30 (zh-CN)<br />
        /// </summary>
        /// <param name="cultureCode">OPTIONAL: Invariant culture if not provided</param>
        /// <returns></returns>
        public static string GetDateTimeGeneralLong(string cultureCode = null)
        {
            //there is not a way to get directly pattern associated with "G", must be built
            CultureInfo culture = InitCulture(cultureCode);
            return
                culture.DateTimeFormat.ShortDatePattern
                + " "
                + culture.DateTimeFormat.LongTimePattern;
        }


        /// <summary>
        /// "g" Standard General date time pattern (short time).<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45 (zh-CN)<br />
        /// </summary>
        /// <param name="cultureCode">OPTIONAL: Invariant culture if not provided</param>
        /// <returns></returns>
        public static string GetDateTimeGeneralShort(string cultureCode = null)
        {
            //there is not a way to get directly pattern associated with "g", must be built
            CultureInfo culture = InitCulture(cultureCode);
            return
                culture.DateTimeFormat.ShortDatePattern
                + " "
                + culture.DateTimeFormat.ShortTimePattern;
        }



        /// <summary>
        /// "d" Standard Short date pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)<br />
        /// 2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)<br />
        /// </summary>
        /// <param name="cultureCode">OPTIONAL: Invariant culture if not provided</param>
        /// <returns></returns>
        public static string GetDateShort(string cultureCode = null)
        {
            //just for coherence with the other methods, wrap here the property related to this format 
            CultureInfo culture = InitCulture(cultureCode);
            return culture.DateTimeFormat.ShortDatePattern;
        }

        private static CultureInfo InitCulture(string cultureCode)
        {
            CultureInfo culture =
                cultureCode.Empty()
                ? CultureInfo.InvariantCulture
                : new CultureInfo(cultureCode);

            return culture;
        }
    }
}