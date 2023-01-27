namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputTextAreaViewComponent : ViewComponent
{
    private readonly ILogger<InputTextAreaViewComponent> _logger;


    public InputTextAreaViewComponent(
        ILogger<InputTextAreaViewComponent> logger
        )
    {
        _logger = logger;
    }




    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        if (inputModel.ControlType != FormControlType.InputTextArea)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.InputTextArea}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputTextArea, inputModel)
               ).ConfigureAwait(false);
    }
}
