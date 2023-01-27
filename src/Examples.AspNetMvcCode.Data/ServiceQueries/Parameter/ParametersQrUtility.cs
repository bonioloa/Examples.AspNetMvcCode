namespace Examples.AspNetMvcCode.Data;

public static class ParametersQrUtility
{
    internal const char CfgValueSeparator = CodeConstants.Dash;
    internal const char CfgGroupSeparator = CodeConstants.Comma;

    internal const string MonthCode = "M";
    internal const string DayCode = "D";
    private const string YearCode = "Y";

    public static DateTime AddDateByType(
        DateTime startingDate
        , string type
        , int quantity
        )
    {
        DateTime toReturn = DateTime.MinValue;
        switch (type)
        {
            case DayCode:
                toReturn = startingDate.AddDays(quantity);
                break;
            case MonthCode:
                toReturn = startingDate.AddMonths(quantity);
                break;
            case YearCode:
                toReturn = startingDate.AddYears(quantity);
                break;
        }
        return toReturn;
    }



    internal const string ItemDescriptiveCodeDefaultFormat = $"{ItemCodePlhCodeBase}_{ItemCodePlhProgressiveProcSimple}";
    internal const string ItemCodePlhCodeBase = "#PREF#";//this is mandatory to have a valid format
    internal const string ItemCodePlhYear = "#yyyy#";
    internal const string ItemCodePlhProgressiveProcSimple = "#progbyproc#";
    internal const string ItemCodePlhProgressiveProcYear = "#progbyprocbyyear#";

    internal static ItemDescriptiveCodeProgressiveType Get(string format)
    {
        if (format.ContainsInvariant(ItemCodePlhProgressiveProcSimple))
        {
            return ItemDescriptiveCodeProgressiveType.ByProcessOnly;
        }
        //if (format.ContainsInvariant(ItemCodePlhProgressiveProcYear))
        //{
        //    return ItemDescriptiveCodeProgressiveType.ByProcessAndYear;
        //}
        return ItemDescriptiveCodeProgressiveType.Missing;
    }



    public static string ItemCodeFormat(
        string format
        , string codeBase
        , ItemDescriptiveCodeProgressiveType progressiveType
        , long progressive
        )
    {
        Guard.Against.NullOrWhiteSpace(format, nameof(format));

        Guard.Against.InvalidInput(
            format
            , nameof(format)
            , (input) => input.ContainsInvariant(ItemCodePlhCodeBase)
            , $"{nameof(format)} value '{format}' does not contain item code base placeholder '{ItemCodePlhCodeBase}'"
            );

        Guard.Against.NegativeOrZero(progressive, nameof(progressive));


        string tmpCode = format.ReplaceInvariant(ItemCodePlhCodeBase, codeBase);

        if (format.ContainsInvariant(ItemCodePlhYear))
        {
            tmpCode = tmpCode.ReplaceInvariant(ItemCodePlhYear, DateTime.Now.Year.ToString());
        }

        string progressivePlaceholder =
            progressiveType switch
            {
                ItemDescriptiveCodeProgressiveType.ByProcessOnly => ItemCodePlhProgressiveProcSimple,
                //ItemDescriptiveCodeProgressiveType.ByProcessAndYear => ItemCodePlhProgressiveProcYear,
                _ => string.Empty,
            };


        return
            tmpCode.ReplaceInvariant(progressivePlaceholder, progressive.ToString());
    }
}