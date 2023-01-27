using System.Text;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="StringBuilder"/>
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// converts current content to string and capitalize first letter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string ToStringWithFirstCharToUpper(this StringBuilder builder)
        {
            return builder is null || builder.Length.Invalid()
                ? string.Empty
                : builder.ToString().CleanAndFirstCharToUppercase();
        }
    }
}