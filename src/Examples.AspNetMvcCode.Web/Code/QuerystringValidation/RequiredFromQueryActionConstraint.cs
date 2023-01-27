namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// https://www.strathweb.com/2016/09/required-query-string-parameters-in-asp-net-core-mvc/
/// 
/// attention using this constraint/attribute.
/// Works only for querystrings, so posted forms are excluded 
/// </summary>
public class RequiredFromQueryActionConstraint : IActionConstraint
{
    private readonly string _parameter;

    public RequiredFromQueryActionConstraint(string parameter)
    {
        _parameter = parameter;
    }


    public int Order => 998;

    public bool Accept(ActionConstraintContext context)
    {
        using IDisposable logScopeCurrentClass =
            LogContext.PushProperty(AppLogPropertiesKeys.ClassName, nameof(RequiredFromQueryActionConstraint));

        using IDisposable logScopeCurrentMethod =
            LogContext.PushProperty(AppLogPropertiesKeys.MethodName, nameof(Accept));



        if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            Log.Logger.Error(
                "missing mandatory parameter '{Parameter}' "
                , _parameter
                );
            return false;
        }


        return true;
    }
}