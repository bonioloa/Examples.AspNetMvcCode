namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputFileViewComponent : ViewComponent
{
    private readonly ILogger<InputFileViewComponent> _logger;

    public InputFileViewComponent(
        ILogger<InputFileViewComponent> logger
        )
    {
        _logger = logger;
    }


    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        if (inputModel.ControlType != FormControlType.InputMultipleFile)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.InputMultipleFile}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(
                   MvcComponents.SharedViewCompInputFile
                   , inputModel
                   )
               ).ConfigureAwait(false);
    }
}
