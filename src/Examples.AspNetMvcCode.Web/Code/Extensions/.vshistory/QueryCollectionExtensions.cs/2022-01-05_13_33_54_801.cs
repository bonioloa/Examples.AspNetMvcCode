namespace Comunica.ProcessManager.Web.Code;

public static class QueryCollectionExtensions
{
    public static string GetQueryStringValue(this IQueryCollection querystring, string key)
    {
        return
            querystring.HasValues()
            && querystring.TryGetValue(key
                , out Microsoft.Extensions.Primitives.StringValues queryVal)
            && queryVal.HasValues()
            && queryVal.FirstOrDefault().StringHasValue()
            ? queryVal.FirstOrDefault()
            : string.Empty;
    }


    public static IDictionary<string, string> ToDictionary(this IQueryCollection querystring)
    {
        IDictionary<string, string> routeVars = new Dictionary<string, string>();
        if (querystring != null)
        {
            foreach (string querystringKey in querystring.Keys)
            {
                string paramValue = querystring.GetQueryStringValue(querystringKey);
                if (paramValue.StringHasValue()
                    && !paramValue.EqualsInvariant(ParamsNames.ReturnUrl)) //this parameter must be discarded
                {
                    routeVars.Add(querystringKey, paramValue);
                }
            }
        }
        return routeVars;
    }


    public static IDictionary<string, string> ToDictionaryApp(this IQueryCollection querystring)
    {
        IDictionary<string, string> temp =
            querystring.IsNullOrEmpty()
            ? new Dictionary<string, string>() : querystring.ToDictionary();

        temp.Remove(ParamsNames.ReturnUrl);
        temp.Remove(ParamsNames.ReturnUrl.ToLower());
        temp.Remove(ParamsNames.ReturnUrl.ToUpper());
        return temp;
    }
}
