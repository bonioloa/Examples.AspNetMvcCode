namespace Comunica.ProcessManager.Web.Code;

public class ValidateAsDateStringFromQueryActionConstraint : IActionConstraint
{
    private readonly string _parameter;

    public ValidateAsDateStringFromQueryActionConstraint(string parameter)
    {
        _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
        if (context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            string regexStr = RegexPatterns.DateOrderableInvariant;
            Regex regex = new(regexStr);
            string paramValue = context.RouteContext.HttpContext.Request.Query[_parameter];
            if (paramValue.Empty())
            {
                return true;
            }
            Match match = regex.Match(paramValue);
            if (match == null || !match.Success)
            {
                Log.Error($"{nameof(ValidateAsDateStringFromQueryActionConstraint)}: parameter '{_parameter}' value '{paramValue}' does not match validation '{regexStr}' ");
                return false;
            }
        }
        return true;
    }
}
