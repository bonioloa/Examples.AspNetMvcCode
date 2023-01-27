using Ganss.Xss;

namespace Examples.AspNetMvcCode.Web.Code;

public static class StringInputExtensions
{
    //https://github.com/mganss/HtmlSanitizer
    //to be used optimistically
    //apply only on user inputs that can't be rendered as normal string (Example: modal title)
    public static string HtmlInputSanitize(this string input)
    {
        //note: for now we use sanitizer default configuration, it will allow anyway a lot of tags.
        //for default lists check package source.
        //If we block in sanitizer all possible tags, all content with tags will be completely omitted by sanitizer
        //so we do a compromise, we leave the default configuration to remove the most dangerous tags (i.e.<script>)
        //while the allowed tags with their content will be preserved, just uglified
        HtmlSanitizer sanitizer = new();

        string sanitized = sanitizer.Sanitize(input.Clean());


        sanitized =
            sanitized.ReplaceInvariant("&", "&amp;")
                     .ReplaceInvariant("<", "&lt;")
                     .ReplaceInvariant(">", "&gt;");
        //.Replace("\"", "&quot;")
        //.Replace("'", "&#x27;");

        return sanitized;
    }
}