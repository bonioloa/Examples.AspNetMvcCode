namespace Examples.AspNetMvcCode.Web.Code;

public sealed class ValidateAsLiteralStringFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
{
    public void Apply(ParameterModel parameter)
    {
        if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
        {
            parameter.Action.Selectors.Last().ActionConstraints.Add(
                new ValidateAsLiteralStringFromQueryActionConstraint(
                    parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName)
                );
        }
    }
}