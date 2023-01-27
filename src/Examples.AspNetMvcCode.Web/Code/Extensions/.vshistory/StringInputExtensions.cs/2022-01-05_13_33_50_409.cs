using Ganss.XSS;

namespace Comunica.ProcessManager.Web.Code;

public static class StringInputExtensions
{
    //https://github.com/mganss/HtmlSanitizer
    //to be used optimistically
    //apply only on user inputs that can't be rendered as normal string (Example: modal title)
    public static string InputSanitize(this string input)
    {
        HtmlSanitizer sanitizer = new();
        string sanitized = sanitizer.Sanitize(input.Clean());
        sanitized =
            sanitized.Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
        //.Replace("\"", "&quot;")
        //.Replace("'", "&#x27;");

        return sanitized;
    }
}
