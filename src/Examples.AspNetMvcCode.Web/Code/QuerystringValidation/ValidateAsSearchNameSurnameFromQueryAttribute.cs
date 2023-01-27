namespace Examples.AspNetMvcCode.Web.Code;

public sealed class ValidateAsSearchNameSurnameFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
{
    public void Apply(ParameterModel parameter)
    {
        if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
        {
            parameter.Action.Selectors.Last().ActionConstraints.Add(
                new ValidateAsSearchNameSurnameFromQueryActionConstraint(
                    parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName)
                );
        }
    }
}