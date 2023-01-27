namespace Examples.AspNetMvcCode.Web.Code;

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
        using IDisposable logScopeCurrentClass =
            LogContext.PushProperty(AppLogPropertiesKeys.ClassName, nameof(ValidateAsSearchNameSurnameFromQueryActionConstraint));

        using IDisposable logScopeCurrentMethod =
            LogContext.PushProperty(AppLogPropertiesKeys.MethodName, nameof(Accept));



        if (context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
        {
            string paramValue = context.RouteContext.HttpContext.Request.Query[_parameter];
            if (paramValue.Empty())
            {
                return true;
            }

            if (paramValue.Length < AppConstants.PersonalNameSurnameMinimumCharacters
                || paramValue.Length > AppConstants.PersonalNameSurnameMaximumCharactersSearch)
            {
                Log.Logger.Error(
                    "parameter '{Parameter}' value '{ParamValue}' is outside allowed length: '{Minimum}'-'{Maximum}' "
                    , _parameter
                    , paramValue
                    , AppConstants.PersonalNameSurnameMinimumCharacters
                    , AppConstants.PersonalNameSurnameMaximumCharactersSearch
                    );

                return false;
            }
        }


        return true;
    }
}