namespace Comunica.ProcessManager.Web.ViewComponents;

/// <summary>
/// can be considered like an extension of text simple
/// </summary>
public class InputDateViewComponent : ViewComponent
{
    private readonly ILogger<InputDateViewComponent> _logger;

    private readonly MainLocalizer _localizer;

    public InputDateViewComponent(
        ILogger<InputDateViewComponent> logger
        , MainLocalizer localizer
        )
    {
        _logger = logger;
        _localizer = localizer;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        _logger.LogAppDebug("CALL");
        if (inputModel.ControlType != FormControlType.InputDate)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.InputDate}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        //this properties must be overridden to transform a simple text input in a date picker
        inputModel.AdditionalClasses = inputModel.AdditionalClasses + CodeConstants.Space + "datepicker";
        inputModel.HasPlaceholder = true;
        inputModel.Placeholder = _localizer[nameof(LocalizedStr.SharedPlaceholderDate)];
        inputModel.CharactersLimit = _localizer[nameof(LocalizedStr.SharedPlaceholderDate)].ToString().Length;

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputDate, inputModel)
               ).ConfigureAwait(false);
    }
}
