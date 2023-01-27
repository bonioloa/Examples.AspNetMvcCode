namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputCheckViewComponent : ViewComponent
{
    private readonly ILogger<InputCheckViewComponent> _logger;

    public InputCheckViewComponent(
        ILogger<InputCheckViewComponent> logger
        )
    {
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(InputControlViewModel inputModel)
    {
        if (inputModel.ControlType != FormControlType.OptionsCheckBox)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.OptionsCheckBox}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputCheck, inputModel)
               ).ConfigureAwait(false);
    }
}
