namespace Comunica.ProcessManager.Web.Code;

public sealed class ValidateAsStringSimpleFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
{
    public void Apply(ParameterModel parameter)
    {
        if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
        {
            parameter.Action.Selectors.Last().ActionConstraints.Add(
                new ValidateAsStringSimpleFromQueryActionConstraint(
                    parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName)
                );
        }
    }
}
