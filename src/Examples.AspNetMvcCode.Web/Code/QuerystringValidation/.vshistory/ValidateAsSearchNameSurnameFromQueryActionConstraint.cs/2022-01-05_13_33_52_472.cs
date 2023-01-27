namespace Comunica.ProcessManager.Web.Code;

public class ValidateAsSearchNameSurnameFromQueryActionConstraint : IActionConstraint
{
    private readonly string _parameter;

    public ValidateAsSearchNameSurnameFromQueryActionConstraint(string parameter)
    {
        _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
        if (context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            string paramValue = context.RouteContext.HttpContext.Request.Query[_parameter];
            if (paramValue.Empty())
            {
                return true;
            }
            Match match = AppConstants.RegexObjNameSurnameSearch.Match(paramValue);
            if (match == null || !match.Success)
            {
                Log.Error($"{nameof(ValidateAsSearchNameSurnameFromQueryActionConstraint)}: parameter '{_parameter}' value '{paramValue}' does not match validation '{AppConstants.RegexObjNameSurnameSearch}' ");
                return false;
            }
        }
        return true;
    }
}
