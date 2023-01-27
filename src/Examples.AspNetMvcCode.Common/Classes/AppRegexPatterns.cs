namespace Examples.AspNetMvcCode.Common;

public static class AppRegexPatterns
{
    //IMPORTANT: DO NOT USE regex shorthands like "d" for "[0-9]" or "s" for [a-z]. Not supported

    public const string LoginCode = @"^([A-Z]{4}\-[A-Z]{6})$";

    public const string Culture = @"^[a-z]{2}(-[A-Z]{2})*$";
}