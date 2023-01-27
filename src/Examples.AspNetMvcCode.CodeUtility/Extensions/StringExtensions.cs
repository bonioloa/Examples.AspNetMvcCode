using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// <para>Custom extensions for <see cref="string"/></para>
    /// <para>DateTime conversions are handled in <see cref="StringDateTimeExtensions"/> and <see cref="StringDateTimeNullableExtensions"/></para>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// returns the opposite of <see cref="string.IsNullOrWhiteSpace(string)"/>. Null safe.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringHasValue(this string str)
        {
            return !Empty(str);
        }


        /// <summary>
        /// Shorthand for <see cref="string.IsNullOrWhiteSpace(string)"/> and also null safe
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Empty(this string str) => string.IsNullOrWhiteSpace(str);


        /// <summary>
        /// Implementation for string of generic method <see cref="IEnumerableExtension.IsNullOrEmpty{T}(IEnumerable{T})"/>,
        /// because the generic method must not be used for string.
        /// Use <see cref="Empty"/> instead.
        /// </summary>
        /// <param name="text"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(error: true, message: "A string is a sequence of characters, but is not intended to be shown as a list")]
        public static void IsNullOrEmpty(this string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>Implementation for string of generic method <see cref="IEnumerableExtension.HasValues{T}(IEnumerable{T})"/>,
        /// because the generic method must not be used for string. </para> 
        /// <para>Use <see cref="StringHasValue"/></para>
        /// </summary>
        /// <param name="text"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(error: true, message: "A string is a sequence of characters, but is not intended to be shown as a list")]
        public static void HasValues(this string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// use this method when you are not sure if it's null, has trailing spaces or newlines.
        /// Can happen with db configuration done with excel
        /// </summary>
        /// <param name="str"></param>
        /// <returns>a non null string</returns>
        public static string Clean(this string str) => str.Empty() ? string.Empty : str.Trim();


        /// <summary>
        /// cleans and replace newlines with <paramref name="replacementText"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacementText"></param>
        /// <returns></returns>
        public static string CleanReplaceTextNewLines(this string str, string replacementText)
        {
            return
                str.StringHasValue()
                ? new Regex(RegexPatterns.TextNewLine).Replace(str.Clean(), replacementText)
                : string.Empty;
        }


        /// <summary>
        /// replace <see cref="CodeConstants.Tab"/> char with <paramref name="replacementText"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacementText"></param>
        /// <returns></returns>
        public static string CleanReplaceTextTabs(this string str, string replacementText)
        {
            return
                str.StringHasValue()
                ? new Regex("\t").Replace(str.Clean(), replacementText)
                : string.Empty;
        }

        /// <summary>
        /// trims input string and replaces new lines and tabs characters with <see cref="string.Empty"/> for all encoding and trim.
        /// </summary>
        /// <param name="str">a string with possible newlines and tabs</param>
        /// <returns></returns>
        public static string CleanRemoveNewLinesAndTabs(this string str)
        {
            return
                str.StringHasValue()
                ? str.Clean()
                      .CleanReplaceTextNewLines(string.Empty)
                      .CleanReplaceTextTabs(string.Empty)
                : string.Empty;
        }

        /// <summary>
        /// cleans and replace html line breaks tags with <paramref name="replacementText"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacementText"></param>
        /// <returns></returns>
        public static string CleanReplaceHtmlNewLines(this string str, string replacementText)
        {
            return
                str.StringHasValue()
                ? new Regex(RegexPatterns.HtmlLineBreak).Replace(str.Clean(), replacementText)
                : string.Empty;
        }

        /// <summary>
        /// cleans and replace html entity non-breakable space with <paramref name="replacementText"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacementText"></param>
        /// <returns></returns>
        public static string CleanReplaceHtmlNonBreakableSpaces(this string str, string replacementText)
        {
            return
                str.StringHasValue()
                ? str.Clean()
                     .ReplaceInvariant("&nbsp;", replacementText)
                : string.Empty; ;
        }


        /// <summary>
        /// https://stackoverflow.com/questions/6275980/string-replace-ignoring-case
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another 
        /// specified string according the type of search to use for the specified string.
        /// </summary>
        /// <param name="str">The string performing the replace method.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string replace all occurrences of <paramref name="oldValue"/>. 
        /// If value is equal to <c>null</c>, than all occurrences of <paramref name="oldValue"/> will be removed from the <paramref name="str"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. 
        /// If <paramref name="oldValue"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        internal static string ReplaceWithComparison(
             this string str
             , string oldValue
             , string newValue
             , StringComparison comparisonType
             )
        {
            if (str is null)
            {
                return string.Empty;//prevent exception
            }
            if (str.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return str;
            }
            if (oldValue.Length == 0)
            {
                //Log.Logger.Error($"{nameof(ReplaceWithComparison)} String cannot be of zero length.");
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length");
            }

            // Prepare string builder for storing the processed string.
            // Note: StringBuilder has a better performance than String by 30-40%.
            StringBuilder resultStringBuilder = new StringBuilder(str.Length);

            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(newValue);

            // Replace all values.
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != NumericsConstants.IndexOfNotFound)
            {
                // Append all characters until the found replacement.
                int charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append(str, startSearchFromIndex, charsUntilReplacment);
                }

                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append(newValue);
                }

                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }

            // Append the last part to the result.
            int charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append(str, startSearchFromIndex, charsUntilStringEnd);

            return resultStringBuilder.ToString();
        }


        /// <summary>
        /// Replace a string with another string using <see cref="StringComparison.InvariantCultureIgnoreCase"/> as comparison.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceInvariant(
            this string str
             , string oldValue
             , string newValue
            )
        {
            return str.ReplaceWithComparison(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
        }



        /// <summary>
        /// capitalize first letter invariant
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CleanAndFirstCharToUppercase(this string str)
        {
            str = str.Clean();
            return
                str.Empty()
                ? string.Empty
                : char.ToUpperInvariant(str[0]) + str.Substring(1);
        }


        /// <summary>
        /// first letter to lowercase invariant
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CleanAndFirstCharToLowercase(this string str)
        {
            str = str.Clean();
            return
                str.Empty()
                ? string.Empty
                : $"{char.ToLowerInvariant(str[0])}{str.Substring(1)}";
        }


        /// <summary>
        /// compare string without casing (<see cref="StringComparison.InvariantCultureIgnoreCase"/>)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strToConfront"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">same as Equals</exception>
        public static bool EqualsInvariant(this string str, string strToConfront)
        {
            return str.Equals(
                strToConfront
                , StringComparison.InvariantCultureIgnoreCase
                );
        }
        /// <summary>
        /// check if a substring is present a string with comparison <see cref="StringComparison.InvariantCultureIgnoreCase"/>
        /// </summary>
        /// <param name="str">where to search</param>
        /// <param name="subStrToSearch">substring to search inside str</param>
        /// <returns></returns>
        public static bool ContainsInvariant(this string str, string subStrToSearch)
        {
            return str.IndexOf(
                subStrToSearch
                , StringComparison.InvariantCultureIgnoreCase
                ) > NumericsConstants.IndexOfNotFound;
        }
        /// <summary>
        /// check if <paramref name="subStrToSearchAtStart"/> starts with input substring with comparison <see cref="StringComparison.InvariantCultureIgnoreCase"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="subStrToSearchAtStart"></param>
        /// <returns></returns>
        public static bool StartsWithInvariant(this string str, string subStrToSearchAtStart)
        {
            return str.StartsWith(
                subStrToSearchAtStart
                , StringComparison.InvariantCultureIgnoreCase
                );
        }
        /// <summary>
        /// check if <paramref name="subStrToSearchAtEnd"/> starts with input substring with comparison <see cref="StringComparison.InvariantCultureIgnoreCase"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="subStrToSearchAtEnd"></param>
        /// <returns></returns>
        public static bool EndsWithInvariant(this string str, string subStrToSearchAtEnd)
        {
            return str.EndsWith(
                subStrToSearchAtEnd
                , StringComparison.InvariantCultureIgnoreCase
                );
        }



        //https://stackoverflow.com/questions/14988691/how-to-check-if-something-equals-any-of-a-list-of-values-in-c-sharp


        /// <summary>
        /// true if target contains at least one element of <paramref name="values"/> 
        /// Equality defined by <paramref name="comparer"/>
        /// <seealso href="https://stackoverflow.com/questions/14988691/how-to-check-if-something-equals-any-of-a-list-of-values-in-c-sharp"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="comparer"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool EqualsAny(this string target, StringComparer comparer, IEnumerable<string> values)
        {
            return values.Contains(target, comparer);
        }

        /// <summary>
        /// true if target contains at least one element of <paramref name="values"/>, ignoring case for all elements comparison
        /// <seealso href="https://stackoverflow.com/questions/14988691/how-to-check-if-something-equals-any-of-a-list-of-values-in-c-sharp"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAnyInvariant(this string target, IEnumerable<string> values)
        {
            return EqualsAny(target, StringComparer.InvariantCultureIgnoreCase, values);
        }

        /// <summary>
        /// convert string to given Enum T ignoring case. 
        /// String must match with a label of provided enum or Exception will be thrown
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str)
            where T : struct, IConvertible //note using the old constraint types and Enum class, 
                                           //because Enum class is nullable and Enum.Parse<T> does not accept it
        {
            try
            {
                T res = (T)Enum.Parse(typeof(T), str, ignoreCase: true);

                if (!Enum.IsDefined(typeof(T), res))
                {
                    //Log.Logger.Error($"{nameof(ToEnum)} - Value '{str}' is not defined in Enum {nameof(T)}");
                    // Same as original .NET C# string.Replace behaviour.
                    throw new ArgumentException($" '{str}' does not exist in enum {typeof(T)} ");
                }
                return res;
            }
            catch
            {
                //Log.Logger.Error($"{nameof(ToEnum)} - Value '{str}' parse threw exception for Enum {nameof(T)}");

                throw;
            }
        }

        /// <summary>
        /// convert string to given Enum T ignoring case. If string does not match with any of defined labels.
        /// Default will be returned (the first defined label of provided enum)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnumOrDefault<T>(this string str) where T : struct, IConvertible
        {
            T res = default;
            try
            {
                Enum.TryParse(str, ignoreCase: true, out res);
            }
            catch
            {
            }
            return res;
        }
    }
}