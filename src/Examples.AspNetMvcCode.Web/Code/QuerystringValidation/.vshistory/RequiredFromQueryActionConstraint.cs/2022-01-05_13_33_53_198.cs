namespace Comunica.ProcessManager.Web.Code;

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
        if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            Log.Error($"{nameof(RequiredFromQueryActionConstraint)}: missing mandatory parameter '{_parameter}' ");
            return false;
        }
        return true;
    }
}
