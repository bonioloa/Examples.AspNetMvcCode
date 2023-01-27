namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputTextSimpleViewComponent : ViewComponent
{
    private readonly ILogger<InputTextSimpleViewComponent> _logger;


    public InputTextSimpleViewComponent(
        ILogger<InputTextSimpleViewComponent> logger
        )
    {
        _logger = logger;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        if (inputModel.ControlType != FormControlType.InputTextSimple
            && inputModel.ControlType != FormControlType.InputDate
            && inputModel.ControlType != FormControlType.InputNumeric)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.InputTextSimple}' or '{FormControlType.InputDate}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputTextSimple, inputModel)
               ).ConfigureAwait(false);
    }
}
