namespace Comunica.ProcessManager.Web.Code;

public class ValidateAsStringSimpleFromQueryActionConstraint : IActionConstraint
{
    private readonly string _parameter;

    public ValidateAsStringSimpleFromQueryActionConstraint(string parameter)
    {
        _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
        if (context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            string regexStr = RegexPatterns.SimpleString;
            Regex regex = new(regexStr);
            string paramValue = context.RouteContext.HttpContext.Request.Query[_parameter];
            if (paramValue.Empty())
            {
                return true;
            }
            Match match = regex.Match(paramValue);
            if (match == null || !match.Success)
            {
                Log.Error($"{nameof(ValidateAsStringSimpleFromQueryActionConstraint)}: parameter '{_parameter}' value '{paramValue}' does not match validation '{regexStr}' ");
                return false;
            }
        }
        return true;
    }
}
