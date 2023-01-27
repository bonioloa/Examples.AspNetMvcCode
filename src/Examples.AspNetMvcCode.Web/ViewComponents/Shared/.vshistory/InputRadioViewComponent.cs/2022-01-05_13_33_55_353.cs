namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputRadioViewComponent : ViewComponent
{
    private readonly ILogger<InputRadioViewComponent> _logger;


    public InputRadioViewComponent(
        ILogger<InputRadioViewComponent> logger
        )
    {
        _logger = logger;
    }




    public async Task<IViewComponentResult> InvokeAsync(InputControlViewModel inputModel)
    {
        if (inputModel.ControlType != FormControlType.OptionsRadio)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.OptionsRadio}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputRadio, inputModel)
               ).ConfigureAwait(false);
    }
}
