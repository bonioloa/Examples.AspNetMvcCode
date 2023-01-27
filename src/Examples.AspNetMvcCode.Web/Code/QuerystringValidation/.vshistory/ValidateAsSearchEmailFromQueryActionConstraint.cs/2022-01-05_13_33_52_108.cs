namespace Comunica.ProcessManager.Web.Code;

public class ValidateAsSearchEmailFromQueryActionConstraint : IActionConstraint
{
    private readonly string _parameter;

    public ValidateAsSearchEmailFromQueryActionConstraint(string parameter)
    {
        _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
        if (context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            string regexEmail = AppConstants.RegexEmailSearch;
            Regex regex = new(regexEmail);
            string paramValue = context.RouteContext.HttpContext.Request.Query[_parameter];
            if (paramValue.Empty())
            {
                return true;
            }
            Match match = regex.Match(paramValue);
            if (match == null || !match.Success)
            {
                Log.Error($"{nameof(ValidateAsSearchEmailFromQueryActionConstraint)}: parameter '{_parameter}' value '{paramValue}' does not match validation '{regexEmail}' ");
                return false;
            }
        }
        return true;
    }
}
