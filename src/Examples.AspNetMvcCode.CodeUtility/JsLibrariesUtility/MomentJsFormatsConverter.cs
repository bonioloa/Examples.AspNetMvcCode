using System;
using System.Globalization;
using System.Text;

namespace Examples.AspNetMvcCode.CodeUtility.JsLibrariesUtility
{
    //credits to
    //https://stackoverflow.com/questions/51664866/convert-date-and-time-format-string-from-moment-js-to-c-sharp-format/51669864
    //https://stackoverflow.com/questions/30572317/how-can-i-convert-date-time-format-string-used-by-c-sharp-to-the-format-used-by/41653618#41653618

    /// <summary>
    /// Class to help convert moment.js date and time format to C# format
    /// </summary>
    public static class MomentJsFormatsConverter
    {
        /* GUIDANCE:
         * do not make public the generic conversion methods, but create a wrapping public method
         * for each .net format / moment.js format that is needed by referencing projects
         */

        /// <summary>
        /// Converts to moment.js the .net format "d" standard Short date pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)<br />
        /// 2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)<br />
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetForStandardShortDate(CultureInfo culture)
        {
            return GetMomentJsFormat(DateTimeFormats.StandardShortDateLocalized, true, culture);
        }


        /// <summary>
        /// Converts to moment.js the .net format "G" standard long time pattern<br />
        /// 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)<br />
        /// 2009-06-15T13:45:30 -> 15/06/2009 13:45:30 (es-ES)<br />
        /// 2009-06-15T13:45:30 -> 2009/6/15 13:45:30 (zh-CN)<br />
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetForStandardDateTime(CultureInfo culture)
        {
            return GetMomentJsFormat(DateTimeFormats.StandardDateTimeLocalizedGeneralTimeLong, true, culture);
        }


        /// <summary>
        /// Converts to moment.js invariant format "yyyy-MM-dd_HH.mm.ss", suited for use in file name.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetInvariantForFileName(CultureInfo culture)
        {
            return GetMomentJsFormat(DateTimeFormats.CustomDateTimeSortableForFileName, true, culture);
        }





        /// <summary>
        /// State of StateMachine while scanning Format 
        /// </summary>
        private enum StateMomentToDotNet
        {
            None,
            LowerA,
            CapitalA,
            LowerD1,
            LowerD2,
            LowerD3,
            LowerD4,
            CapitalD1,
            CapitalD2,
            LowerH1,
            LowerH2,
            CapitalH1,
            CapitalH2,
            LowerM1,
            LowerM2,
            CapitalM1,
            CapitalM2,
            CapitalM3,
            CapitalM4,
            LowerS1,
            LowerS2,
            CapitalS1,
            CapitalS2,
            CapitalS3,
            CapitalS4,
            CapitalS5,
            CapitalS6,
            CapitalS7,
            CapitalY1,
            CapitalY2,
            CapitalY3,
            CapitalY4,
            CapitalZ
        }

        /// <summary>
        /// Convert a Moment.js format to .Net
        /// </summary>
        /// <param name="momentJsFormat"></param>
        /// <returns></returns>
        internal static string GetDotNetFormat(string momentJsFormat)
        {
            StringBuilder resultBuilder = new StringBuilder();
            StateMomentToDotNet resultState = StateMomentToDotNet.None;
            StringBuilder tokenBuffer = new StringBuilder();

            var ChangeState = new Action<StateMomentToDotNet>((StateMomentToDotNet fNewState) =>
            {
                switch (resultState)
                {
                    case StateMomentToDotNet.LowerA:
                    case StateMomentToDotNet.CapitalA:
                        resultBuilder.Append("tt");
                        break;
                    case StateMomentToDotNet.LowerD3:
                        resultBuilder.Append("ddd");
                        break;
                    case StateMomentToDotNet.LowerD4:
                        resultBuilder.Append("dddd");
                        break;
                    case StateMomentToDotNet.CapitalD1:
                        resultBuilder.Append('d');
                        break;
                    case StateMomentToDotNet.CapitalD2:
                        resultBuilder.Append("dd");
                        break;
                    case StateMomentToDotNet.LowerH1:
                        resultBuilder.Append('h');
                        break;
                    case StateMomentToDotNet.LowerH2:
                        resultBuilder.Append("hh");
                        break;
                    case StateMomentToDotNet.CapitalH1:
                        resultBuilder.Append('H');
                        break;
                    case StateMomentToDotNet.CapitalH2:
                        resultBuilder.Append("HH");
                        break;
                    case StateMomentToDotNet.LowerM1:
                        resultBuilder.Append('m');
                        break;
                    case StateMomentToDotNet.LowerM2:
                        resultBuilder.Append("mm");
                        break;
                    case StateMomentToDotNet.CapitalM1:
                        resultBuilder.Append('M');
                        break;
                    case StateMomentToDotNet.CapitalM2:
                        resultBuilder.Append("MM");
                        break;
                    case StateMomentToDotNet.CapitalM3:
                        resultBuilder.Append("MMM");
                        break;
                    case StateMomentToDotNet.CapitalM4:
                        resultBuilder.Append("MMMM");
                        break;
                    case StateMomentToDotNet.LowerS1:
                        resultBuilder.Append('s');
                        break;
                    case StateMomentToDotNet.LowerS2:
                        resultBuilder.Append("ss");
                        break;
                    case StateMomentToDotNet.CapitalS1:
                        resultBuilder.Append('f');
                        break;
                    case StateMomentToDotNet.CapitalS2:
                        resultBuilder.Append("ff");
                        break;
                    case StateMomentToDotNet.CapitalS3:
                        resultBuilder.Append("fff");
                        break;
                    case StateMomentToDotNet.CapitalS4:
                        resultBuilder.Append("ffff");
                        break;
                    case StateMomentToDotNet.CapitalS5:
                        resultBuilder.Append("fffff");
                        break;
                    case StateMomentToDotNet.CapitalS6:
                        resultBuilder.Append("ffffff");
                        break;
                    case StateMomentToDotNet.CapitalS7:
                        resultBuilder.Append("fffffff");
                        break;
                    case StateMomentToDotNet.CapitalY2:
                        resultBuilder.Append("yy");
                        break;
                    case StateMomentToDotNet.CapitalY4:
                        resultBuilder.Append("yyyy");
                        break;
                    case StateMomentToDotNet.CapitalZ:
                        resultBuilder.Append("zzz");
                        break;
                }

                tokenBuffer.Clear();
                resultState = fNewState;
            });

            foreach (var character in momentJsFormat)
            {
                switch (character)
                {
                    case 'a':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.LowerA:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.LowerA);
                                break;
                        }
                        break;
                    case 'A':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalA:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalA);
                                break;
                        }
                        break;
                    case 'd':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.LowerD1:
                                resultState = StateMomentToDotNet.LowerD2;
                                break;
                            case StateMomentToDotNet.LowerD2:
                                resultState = StateMomentToDotNet.LowerD3;
                                break;
                            case StateMomentToDotNet.LowerD3:
                                resultState = StateMomentToDotNet.LowerD4;
                                break;
                            case StateMomentToDotNet.LowerD4:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.LowerD1);
                                break;
                        }
                        break;
                    case 'D':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalD1:
                                resultState = StateMomentToDotNet.CapitalD2;
                                break;
                            case StateMomentToDotNet.CapitalD2:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalD1);
                                break;
                        }
                        break;
                    case 'h':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.LowerH1:
                                resultState = StateMomentToDotNet.LowerH2;
                                break;
                            case StateMomentToDotNet.LowerH2:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.LowerH1);
                                break;
                        }
                        break;
                    case 'H':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalH1:
                                resultState = StateMomentToDotNet.CapitalH2;
                                break;
                            case StateMomentToDotNet.CapitalH2:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalH1);
                                break;
                        }
                        break;
                    case 'm':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.LowerM1:
                                resultState = StateMomentToDotNet.LowerM2;
                                break;
                            case StateMomentToDotNet.LowerM2:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.LowerM1);
                                break;
                        }
                        break;
                    case 'M':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalM1:
                                resultState = StateMomentToDotNet.CapitalM2;
                                break;
                            case StateMomentToDotNet.CapitalM2:
                                resultState = StateMomentToDotNet.CapitalM3;
                                break;
                            case StateMomentToDotNet.CapitalM3:
                                resultState = StateMomentToDotNet.CapitalM4;
                                break;
                            case StateMomentToDotNet.CapitalM4:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalM1);
                                break;
                        }
                        break;
                    case 's':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.LowerS1:
                                resultState = StateMomentToDotNet.LowerS2;
                                break;
                            case StateMomentToDotNet.LowerS2:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.LowerS1);
                                break;
                        }
                        break;
                    case 'S':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalS1:
                                resultState = StateMomentToDotNet.CapitalS2;
                                break;
                            case StateMomentToDotNet.CapitalS2:
                                resultState = StateMomentToDotNet.CapitalS3;
                                break;
                            case StateMomentToDotNet.CapitalS3:
                                resultState = StateMomentToDotNet.CapitalS4;
                                break;
                            case StateMomentToDotNet.CapitalS4:
                                resultState = StateMomentToDotNet.CapitalS5;
                                break;
                            case StateMomentToDotNet.CapitalS5:
                                resultState = StateMomentToDotNet.CapitalS6;
                                break;
                            case StateMomentToDotNet.CapitalS6:
                                resultState = StateMomentToDotNet.CapitalS7;
                                break;
                            case StateMomentToDotNet.CapitalS7:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalS1);
                                break;
                        }
                        break;
                    case 'Y':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalY1:
                                resultState = StateMomentToDotNet.CapitalY2;
                                break;
                            case StateMomentToDotNet.CapitalY2:
                                resultState = StateMomentToDotNet.CapitalY3;
                                break;
                            case StateMomentToDotNet.CapitalY3:
                                resultState = StateMomentToDotNet.CapitalY4;
                                break;
                            case StateMomentToDotNet.CapitalY4:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalY1);
                                break;
                        }
                        break;
                    case 'Z':
                        switch (resultState)
                        {
                            case StateMomentToDotNet.CapitalZ:
                                break;
                            default:
                                ChangeState(StateMomentToDotNet.CapitalZ);
                                break;
                        }
                        break;
                    default:
                        ChangeState(StateMomentToDotNet.None);
                        resultBuilder.Append(character);
                        break;
                }
            }

            ChangeState(StateMomentToDotNet.None);
            return resultBuilder.ToString();
        }



        /// <summary>
        /// State of StateMachine while scanning Format 
        /// </summary>
        private enum StateDotNetToMoment
        {
            None,
            LowerD1,
            LowerD2,
            LowerD3,
            LowerD4,
            LowerF1,
            LowerF2,
            LowerF3,
            LowerF4,
            LowerF5,
            LowerF6,
            LowerF7,
            CapitalF1,
            CapitalF2,
            CapitalF3,
            CapitalF4,
            CapitalF5,
            CapitalF6,
            CapitalF7,
            LowerG,
            LowerH1,
            LowerH2,
            CapitalH1,
            CapitalH2,
            CapitalK,
            LowerM1,
            LowerM2,
            CapitalM1,
            CapitalM2,
            CapitalM3,
            CapitalM4,
            LowerS1,
            LowerS2,
            LowerT1,
            LowerT2,
            LowerY1,
            LowerY2,
            LowerY3,
            LowerY4,
            LowerY5,
            LowerZ1,
            LowerZ2,
            LowerZ3,
            InSingleQuoteLiteral,
            InDoubleQuoteLiteral,
            EscapeSequence
        }


        /// <summary>
        /// Convert a dotnet datetime format string to a Moment.js format string<br/>
        /// Restriction:<br/>
        /// Fractional Seconds Lowercase F and Uppercase F are difficult to translate to MomentJS, so closest 
        /// translation used
        /// </summary>
        /// <param name="dotNetformat">The dotnet datetime format string to convert. 
        /// Can be a custom exact format like "yyyyMMdd" or a standard shorthand "d"
        /// </param>
        /// <param name="laxConversion">If true, some cases where there is not exact equivalent in MomentJs is 
        /// handled by generating a similar format instead of throwing a UnsupportedFormatException exception.</param>
        /// <param name="culture">The Culture to use. If none, the current thread Culture is used</param>
        /// <returns>A format string to be used in MomentJS</returns>
        /// <exception cref="UnsupportedFormatException">Conversion fails because we have an element in the format string
        /// that has no equivalent in MomentJS</exception>
        /// <exception cref="FormatException"><paramref name="dotNetformat"/> is no valid DateTime format string in dotnet</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dotNetformat"/> is null</exception>
        internal static string GetMomentJsFormat(
            string dotNetformat
            , bool laxConversion = true
            , CultureInfo culture = null
            )
        {
            culture = culture ?? CultureInfo.CurrentCulture;

            if (dotNetformat is null)
            {
                throw new ArgumentNullException(nameof(dotNetformat));
            }

            if (dotNetformat.Length == 0)
            {
                return string.Empty;
            }

            if (dotNetformat.Length == 1)
            {
                return GetMomentJSFormatFromStandard(dotNetformat, laxConversion, culture);
            }

            return GetMomentJSFormat(dotNetformat, laxConversion, culture);
        }


        /// <summary>
        /// decodes a shorthand conventional format to an explicit format
        /// </summary>
        /// <param name="dotNetStandardFormatKey"></param>
        /// <param name="laxConversion"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="UnsupportedFormatException">throws when trying to convert a dotnet datetime format string
        /// to a MomentJS format string and this fails because we have an element in the format string
        /// that has no equivalent in MomentJS</exception>
        /// <exception cref="FormatException"></exception>
        private static string GetMomentJSFormatFromStandard(
            string dotNetStandardFormatKey
            , bool laxConversion
            , CultureInfo culture
            )
        {
            string result;

            //if this shorthands are used in more places, incapsulate them in constants
            switch (dotNetStandardFormatKey)
            {
                case DateTimeFormats.StandardShortDateLocalized:
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.ShortDatePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "D":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.LongDatePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "f":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.LongDatePattern + " " + culture.DateTimeFormat.ShortTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "F":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.FullDateTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "g":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.ShortTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "G":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "M":
                case "m":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.MonthDayPattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "O":
                case "o":
                    result =
                        GetMomentJSFormat(
                            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"
                            , laxConversion
                            , culture
                            );
                    break;

                case "R":
                case "r":
                    throw new UnsupportedFormatException("RFC 1123 not supported  in MomentJS");

                case "s":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.SortableDateTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "t":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.ShortTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "T":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.LongTimePattern
                            , laxConversion
                            , culture
                            );
                    break;

                case "u":
                    throw new UnsupportedFormatException("Universal Sortable Format not supported in MomentJS");

                case "U":
                    throw new UnsupportedFormatException("Universal Full Format not supported in MomentJS");

                case "Y":
                case "y":
                    result =
                        GetMomentJSFormat(
                            culture.DateTimeFormat.YearMonthPattern
                            , laxConversion
                            , culture
                            );
                    break;

                default:
                    throw new FormatException("Unknown Standard DateTime Format");
            }

            return result;
        }


        private static string GetMomentJSFormat(
            string dotNetformat
            , bool laxConversion
            , CultureInfo culture
            )
        {
            StringBuilder formatBuilder = new StringBuilder();

            StateDotNetToMoment state = StateDotNetToMoment.None;
            StringBuilder tokenBuilder = new StringBuilder();

            Action<StateDotNetToMoment> changeState =
                new Action<StateDotNetToMoment>((StateDotNetToMoment tmpState) =>
            {
                switch (state)
                {
                    case StateDotNetToMoment.LowerD1:
                        formatBuilder.Append('D');
                        break;
                    case StateDotNetToMoment.LowerD2:
                        formatBuilder.Append("DD");
                        break;
                    case StateDotNetToMoment.LowerD3:
                        formatBuilder.Append("ddd");
                        break;
                    case StateDotNetToMoment.LowerD4:
                        formatBuilder.Append("dddd");
                        break;
                    case StateDotNetToMoment.LowerF1:
                    case StateDotNetToMoment.CapitalF1:
                        formatBuilder.Append('S');
                        break;
                    case StateDotNetToMoment.LowerF2:
                    case StateDotNetToMoment.CapitalF2:
                        formatBuilder.Append("SS");
                        break;
                    case StateDotNetToMoment.LowerF3:
                    case StateDotNetToMoment.CapitalF3:
                        formatBuilder.Append("SSS");
                        break;
                    case StateDotNetToMoment.LowerF4:
                    case StateDotNetToMoment.CapitalF4:
                        formatBuilder.Append("SSSS");
                        break;
                    case StateDotNetToMoment.LowerF5:
                    case StateDotNetToMoment.CapitalF5:
                        formatBuilder.Append("SSSSS");
                        break;
                    case StateDotNetToMoment.LowerF6:
                    case StateDotNetToMoment.CapitalF6:
                        formatBuilder.Append("SSSSSS");
                        break;
                    case StateDotNetToMoment.LowerF7:
                    case StateDotNetToMoment.CapitalF7:
                        formatBuilder.Append("SSSSSSS");
                        break;
                    case StateDotNetToMoment.LowerG:
                        throw new UnsupportedFormatException("Era not supported in MomentJS");
                    case StateDotNetToMoment.LowerH1:
                        formatBuilder.Append('h');
                        break;
                    case StateDotNetToMoment.LowerH2:
                        formatBuilder.Append("hh");
                        break;
                    case StateDotNetToMoment.CapitalH1:
                        formatBuilder.Append('H');
                        break;
                    case StateDotNetToMoment.CapitalH2:
                        formatBuilder.Append("HH");
                        break;
                    case StateDotNetToMoment.LowerM1:
                        formatBuilder.Append('m');
                        break;
                    case StateDotNetToMoment.LowerM2:
                        formatBuilder.Append("mm");
                        break;
                    case StateDotNetToMoment.CapitalM1:
                        formatBuilder.Append('M');
                        break;
                    case StateDotNetToMoment.CapitalM2:
                        formatBuilder.Append("MM");
                        break;
                    case StateDotNetToMoment.CapitalM3:
                        formatBuilder.Append("MMM");
                        break;
                    case StateDotNetToMoment.CapitalM4:
                        formatBuilder.Append("MMMM");
                        break;
                    case StateDotNetToMoment.LowerS1:
                        formatBuilder.Append('s');
                        break;
                    case StateDotNetToMoment.LowerS2:
                        formatBuilder.Append("ss");
                        break;
                    case StateDotNetToMoment.LowerT1:
                        if (laxConversion)
                        {
                            formatBuilder.Append('A');
                        }
                        else
                        {
                            throw new UnsupportedFormatException("Single Letter AM/PM not supported in MomentJS");
                        }
                        break;
                    case StateDotNetToMoment.LowerT2:
                        formatBuilder.Append('A');
                        break;
                    case StateDotNetToMoment.LowerY1:
                        if (laxConversion)
                        {
                            formatBuilder.Append("YY");
                        }
                        else
                        {
                            throw new UnsupportedFormatException("Single Letter Year not supported in MomentJS");
                        }
                        break;
                    case StateDotNetToMoment.LowerY2:
                        formatBuilder.Append("YY");
                        break;
                    case StateDotNetToMoment.LowerY3:
                        if (laxConversion)
                        {
                            formatBuilder.Append("YYYY");
                        }
                        else
                        {
                            throw new UnsupportedFormatException("Three Letter Year not supported in MomentJS");
                        }
                        break;
                    case StateDotNetToMoment.LowerY4:
                        formatBuilder.Append("YYYY");
                        break;
                    case StateDotNetToMoment.LowerY5:
                        if (laxConversion)
                        {
                            formatBuilder.Append('Y');
                        }
                        else
                        {
                            throw new UnsupportedFormatException("Five or more Letter Year not supported in MomentJS");
                        }
                        break;
                    case StateDotNetToMoment.LowerZ1:
                    case StateDotNetToMoment.LowerZ2:
                        if (laxConversion)
                        {
                            formatBuilder.Append("ZZ");
                        }
                        else
                        {
                            throw new UnsupportedFormatException("Hours offset not supported in MomentJS");
                        }
                        break;
                    case StateDotNetToMoment.LowerZ3:
                        formatBuilder.Append('Z');
                        break;
                    case StateDotNetToMoment.InSingleQuoteLiteral:
                    case StateDotNetToMoment.InDoubleQuoteLiteral:
                    case StateDotNetToMoment.EscapeSequence:
                        foreach (var lCharacter in tokenBuilder.ToString())
                        {
                            formatBuilder.Append("[" + lCharacter + "]");
                        }
                        break;
                }

                tokenBuilder.Clear();
                state = tmpState;
            }); // End ChangeState

            foreach (char character in dotNetformat)
            {
                if (state == StateDotNetToMoment.EscapeSequence)
                {
                    tokenBuilder.Append(character);
                    changeState(StateDotNetToMoment.None);
                }
                else if (state == StateDotNetToMoment.InDoubleQuoteLiteral)
                {
                    if (character == '\"')
                    {
                        changeState(StateDotNetToMoment.None);
                    }
                    else
                    {
                        tokenBuilder.Append(character);
                    }
                }
                else if (state == StateDotNetToMoment.InSingleQuoteLiteral)
                {
                    if (character == '\'')
                    {
                        changeState(StateDotNetToMoment.None);
                    }
                    else
                    {
                        tokenBuilder.Append(character);
                    }
                }
                else
                {
                    switch (character)
                    {
                        case 'd':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerD1:
                                    state = StateDotNetToMoment.LowerD2;
                                    break;
                                case StateDotNetToMoment.LowerD2:
                                    state = StateDotNetToMoment.LowerD3;
                                    break;
                                case StateDotNetToMoment.LowerD3:
                                    state = StateDotNetToMoment.LowerD4;
                                    break;
                                case StateDotNetToMoment.LowerD4:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerD1);
                                    break;
                            }
                            break;

                        case 'f':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerF1:
                                    state = StateDotNetToMoment.LowerF2;
                                    break;
                                case StateDotNetToMoment.LowerF2:
                                    state = StateDotNetToMoment.LowerF3;
                                    break;
                                case StateDotNetToMoment.LowerF3:
                                    state = StateDotNetToMoment.LowerF4;
                                    break;
                                case StateDotNetToMoment.LowerF4:
                                    state = StateDotNetToMoment.LowerF5;
                                    break;
                                case StateDotNetToMoment.LowerF5:
                                    state = StateDotNetToMoment.LowerF6;
                                    break;
                                case StateDotNetToMoment.LowerF6:
                                    state = StateDotNetToMoment.LowerF7;
                                    break;
                                case StateDotNetToMoment.LowerF7:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerF1);
                                    break;
                            }
                            break;

                        case 'F':
                            switch (state)
                            {
                                case StateDotNetToMoment.CapitalF1:
                                    state = StateDotNetToMoment.CapitalF2;
                                    break;
                                case StateDotNetToMoment.CapitalF2:
                                    state = StateDotNetToMoment.CapitalF3;
                                    break;
                                case StateDotNetToMoment.CapitalF3:
                                    state = StateDotNetToMoment.CapitalF4;
                                    break;
                                case StateDotNetToMoment.CapitalF4:
                                    state = StateDotNetToMoment.CapitalF5;
                                    break;
                                case StateDotNetToMoment.CapitalF5:
                                    state = StateDotNetToMoment.CapitalF6;
                                    break;
                                case StateDotNetToMoment.CapitalF6:
                                    state = StateDotNetToMoment.CapitalF7;
                                    break;
                                case StateDotNetToMoment.CapitalF7:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.CapitalF1);
                                    break;
                            }
                            break;

                        case 'g':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerG:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerG);
                                    break;
                            }
                            break;

                        case 'h':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerH1:
                                    state = StateDotNetToMoment.LowerH2;
                                    break;
                                case StateDotNetToMoment.LowerH2:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerH1);
                                    break;
                            }
                            break;

                        case 'H':
                            switch (state)
                            {
                                case StateDotNetToMoment.CapitalH1:
                                    state = StateDotNetToMoment.CapitalH2;
                                    break;
                                case StateDotNetToMoment.CapitalH2:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.CapitalH1);
                                    break;
                            }
                            break;

                        case 'K':
                            changeState(StateDotNetToMoment.None);
                            if (laxConversion)
                            {
                                formatBuilder.Append('Z');
                            }
                            else
                            {
                                throw new UnsupportedFormatException("TimeZoneInformation not supported in MomentJS");
                            }
                            break;

                        case 'm':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerM1:
                                    state = StateDotNetToMoment.LowerM2;
                                    break;
                                case StateDotNetToMoment.LowerM2:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerM1);
                                    break;
                            }
                            break;

                        case 'M':
                            switch (state)
                            {
                                case StateDotNetToMoment.CapitalM1:
                                    state = StateDotNetToMoment.CapitalM2;
                                    break;
                                case StateDotNetToMoment.CapitalM2:
                                    state = StateDotNetToMoment.CapitalM3;
                                    break;
                                case StateDotNetToMoment.CapitalM3:
                                    state = StateDotNetToMoment.CapitalM4;
                                    break;
                                case StateDotNetToMoment.CapitalM4:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.CapitalM1);
                                    break;
                            }
                            break;

                        case 's':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerS1:
                                    state = StateDotNetToMoment.LowerS2;
                                    break;
                                case StateDotNetToMoment.LowerS2:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerS1);
                                    break;
                            }
                            break;

                        case 't':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerT1:
                                    state = StateDotNetToMoment.LowerT2;
                                    break;
                                case StateDotNetToMoment.LowerT2:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerT1);
                                    break;
                            }
                            break;

                        case 'y':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerY1:
                                    state = StateDotNetToMoment.LowerY2;
                                    break;
                                case StateDotNetToMoment.LowerY2:
                                    state = StateDotNetToMoment.LowerY3;
                                    break;
                                case StateDotNetToMoment.LowerY3:
                                    state = StateDotNetToMoment.LowerY4;
                                    break;
                                case StateDotNetToMoment.LowerY4:
                                    state = StateDotNetToMoment.LowerY5;
                                    break;
                                case StateDotNetToMoment.LowerY5:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerY1);
                                    break;
                            }
                            break;

                        case 'z':
                            switch (state)
                            {
                                case StateDotNetToMoment.LowerZ1:
                                    state = StateDotNetToMoment.LowerZ2;
                                    break;
                                case StateDotNetToMoment.LowerZ2:
                                    state = StateDotNetToMoment.LowerZ3;
                                    break;
                                case StateDotNetToMoment.LowerZ3:
                                    break;
                                default:
                                    changeState(StateDotNetToMoment.LowerZ1);
                                    break;
                            }
                            break;

                        case ':':
                            changeState(StateDotNetToMoment.None);
                            formatBuilder.Append("[" + culture.DateTimeFormat.TimeSeparator + "]");
                            break;

                        case '/':
                            changeState(StateDotNetToMoment.None);
                            formatBuilder.Append("[" + culture.DateTimeFormat.DateSeparator + "]");
                            break;

                        case '\"':
                            changeState(StateDotNetToMoment.InDoubleQuoteLiteral);
                            break;

                        case '\'':
                            changeState(StateDotNetToMoment.InSingleQuoteLiteral);
                            break;

                        case '%':
                            changeState(StateDotNetToMoment.None);
                            break;

                        case '\\':
                            changeState(StateDotNetToMoment.EscapeSequence);
                            break;

                        default:
                            changeState(StateDotNetToMoment.None);
                            formatBuilder.Append("[" + character + "]");
                            break;
                    }
                }
            }

            if (state == StateDotNetToMoment.EscapeSequence
                || state == StateDotNetToMoment.InDoubleQuoteLiteral
                || state == StateDotNetToMoment.InSingleQuoteLiteral)
            {
                throw new FormatException("Invalid Format String");
            }

            changeState(StateDotNetToMoment.None);


            return formatBuilder.ToString();
        }
    }
}