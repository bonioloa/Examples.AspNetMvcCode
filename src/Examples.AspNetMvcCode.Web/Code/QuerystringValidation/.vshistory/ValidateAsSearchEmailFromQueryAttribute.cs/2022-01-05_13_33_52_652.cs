namespace Comunica.ProcessManager.Web.Code;

public sealed class ValidateAsSearchEmailFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
{
    public void Apply(ParameterModel parameter)
    {
        if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
        {
            parameter.Action.Selectors.Last().ActionConstraints.Add(
                new ValidateAsSearchEmailFromQueryActionConstraint(
                    parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName)
                );
        }
    }
}
