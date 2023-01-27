namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// https://www.strathweb.com/2016/09/required-query-string-parameters-in-asp-net-core-mvc/
/// WARNING: use only for GET calls
/// </summary>
public sealed class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
{
    public void Apply(ParameterModel parameter)
    {
        if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
        {
            parameter.Action.Selectors.Last().ActionConstraints.Add(
                new RequiredFromQueryActionConstraint(
                    parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName)
                );
        }
    }
}
