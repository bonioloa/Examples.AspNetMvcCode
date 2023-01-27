﻿namespace Examples.AspNetMvcCode.Web.Code;

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
        using IDisposable logScopeCurrentClass =
            LogContext.PushProperty(AppLogPropertiesKeys.ClassName, nameof(ValidateAsDateStringFromQueryActionConstraint));

        using IDisposable logScopeCurrentMethod =
            LogContext.PushProperty(AppLogPropertiesKeys.MethodName, nameof(Accept));



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
                Log.Logger.Error(
                    "parameter '{Parameter}' value '{ParamValue}' does not match validation '{RegexStr}' "
                    , _parameter
                    , paramValue
                    , regexStr
                    );

                return false;
            }
        }


        return true;
    }
}