namespace Examples.AspNetMvcCode.CodeUtility
{
    /// <summary>
    /// Common regex patterns
    /// </summary>
    public static class RegexPatterns
    {
        //IMPORTANT: DO NOT USE regex shorthands like "d" for "[0-9]" or "s" for [a-z]. Not supported


        /// <summary>
        /// Literals ONLY
        /// used for querystring get parameters validation, preventing submit of malicious url or html text
        /// </summary>
        public const string LiteralsOnly = @"^[a-zA-Z]+$";

        /// <summary>
        /// Literals, number, dash
        /// Used for querystring get parameters validation, preventing submit of malicious url or html text
        /// </summary>
        public const string SimpleString = @"^[a-zA-Z0-9-]+$";

        //https://www.w3.org/QA/Tips/iso-date
        /// <summary>
        /// Checks if date is in orderable (ISO) format.
        /// Necessary for date representation regardless of localization
        /// </summary>
        public const string DateOrderableInvariant = @"^(([0-9]{4}-((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01])|(0[469]|11)-(0[1-9]|[12][0-9]|30)|02-(0[1-9]|1[0-9]|2[0-8])))|(([0-9]{2}(0[48]|[2468][048]|[13579][26])|([02468][048]|[1359][26])00)-02-29))$";

        //W3C standard html validation, strangely it allows email without ending dot part (Ex .com)
        /// <summary>
        /// single email pattern using W3C specification
        /// </summary>
        public const string Email = @"^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

        /// <summary>
        /// line break match in a string
        /// </summary>
        public const string HtmlLineBreak = @"<\s*(br|BR|bR|Br)\s*\/?>";
        //we give all case options becase case insensitive must be set in regex init
        //and people might forget to set it

        /// <summary>
        /// pattern to match both types of newlines  
        /// </summary>
        public static readonly string TextNewLine = $@"(\n|\r\n)";
    }
}