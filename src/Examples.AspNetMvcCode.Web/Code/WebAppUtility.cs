namespace Examples.AspNetMvcCode.Web.Code;

public static class WebAppUtility
{
    internal static object GetRouteDefaults()
    {
        IDictionary<string, object> result = new ExpandoObject();

        result[RouteParams.Language] = SupportedCulturesConstants.IsoCodeDefault;
        result[RouteParams.Controller] = MvcComponents.CtrlFallback;
        result[RouteParams.Action] = MvcComponents.ActRedirectToDefaultLanguage;

        return result;
    }




    public static string ToSerializationCase(string toSerialize)
    {
        return toSerialize.CleanAndFirstCharToLowercase();
    }
    public static string ToSerializationCase(SearchResultItemField toSerialize)
    {
        return ToSerializationCase(toSerialize.ToString());
    }
    public static string ToSerializationCase(DynamicFormFieldForLogic toSerialize)
    {
        return ToSerializationCase(toSerialize.ToString());
    }
}