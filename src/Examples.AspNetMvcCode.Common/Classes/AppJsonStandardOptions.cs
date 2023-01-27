namespace Examples.AspNetMvcCode.Common;

public static class AppJsonStandardOptions
{
    public static readonly JsonSerializerOptions Indented =
        new() { WriteIndented = true };
}