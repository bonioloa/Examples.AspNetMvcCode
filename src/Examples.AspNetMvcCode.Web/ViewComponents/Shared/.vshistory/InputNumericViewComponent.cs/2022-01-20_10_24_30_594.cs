namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputNumericViewComponent : ViewComponent
{
    private readonly ILogger<InputNumericViewComponent> _logger;


    public InputNumericViewComponent(
        ILogger<InputNumericViewComponent> logger
        )
    {
        _logger = logger;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        if (inputModel.ControlType != FormControlType.InputNumeric)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.InputNumeric}' or '{FormControlType.InputDate}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        //this properties must be overridden to transform a simple text input in a date picker
        inputModel.AdditionalClasses = inputModel.AdditionalClasses + CodeConstants.Space + "input-numeric";
        inputModel.HasPlaceholder = true;
        inputModel.Placeholder = "0";
        inputModel.CharactersLimit = NumericsConstants.NumericMaxLength;


        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputNumeric, inputModel)
               ).ConfigureAwait(false);
    }
}
