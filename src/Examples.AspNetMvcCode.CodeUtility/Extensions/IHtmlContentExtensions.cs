using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Encodings.Web;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="IHtmlContent"/>
    /// </summary>
    public static class IHtmlContentExtensions
    {
        /// <summary>
        /// use this method to convert this type to string, not the default ToString() !!!
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string GetStringContent(this IHtmlContent htmlContent)
        {
            if (htmlContent is null)
            {
                return string.Empty;
            }

            string output = string.Empty;
            using (StringWriter writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                output = writer.ToString();
            }
            return output;
        }

        /// <summary>
        /// true if null or does not contain only white characters
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static bool Invalid(this IHtmlContent htmlContent) => htmlContent.GetStringContent().Empty();

        /// <summary>
        /// true if not null and has significative characters
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static bool HasValue(this IHtmlContent htmlContent) => !htmlContent.Invalid();

        /// <summary>
        /// true if it contains the string "&lt;div"
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static bool ContainsDivTags(this IHtmlContent htmlContent)
            => htmlContent.GetStringContent().ContainsInvariant("<div");

        /// <summary>
        /// true if compared with <paramref name="toHtmlContent"/> regardless the case
        /// </summary>
        /// <param name="fromHtmlContent"></param>
        /// <param name="toHtmlContent"></param>
        /// <returns></returns>
        public static bool IsEqualCaseInvariant(
            this IHtmlContent fromHtmlContent
            , IHtmlContent toHtmlContent
            )
        {
            return fromHtmlContent.GetStringContent()
                                  .EqualsInvariant(toHtmlContent.GetStringContent());
        }

        /// <summary>
        /// obsolete, because <see cref="object.ToString"/> for this object does not guarantee to return the correct value 
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <exception cref="NotSupportedException"></exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(error: true, message: "Use GetStringContent method to get the underlying string ")]
        public static void ToString(this IHtmlContent htmlContent)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// obsolete, because <see cref="object.ToString"/> for this object does not guarantee to return the correct value 
        /// </summary>
        /// <param name="htmlString"></param>
        /// <exception cref="NotSupportedException"></exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(error: true, message: "Use GetStringContent method to get the underlying string ")]
        public static void ToString(this HtmlString htmlString)
        {
            throw new NotSupportedException();
        }

    }
}