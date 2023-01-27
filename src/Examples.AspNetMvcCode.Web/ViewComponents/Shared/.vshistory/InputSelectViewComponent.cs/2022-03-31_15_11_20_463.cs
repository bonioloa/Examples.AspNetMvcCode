﻿namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputSelectViewComponent : ViewComponent
{
    private readonly ILogger<InputSelectViewComponent> _logger;

    public InputSelectViewComponent(
        ILogger<InputSelectViewComponent> logger
        )
    {
        _logger = logger;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        if (inputModel.ControlType != FormControlType.OptionsSelect
            || inputModel.ControlType != FormControlType.OptionsSelectMultiple)
        {
            _logger.LogAppError($"wrong input control type: requires '{FormControlType.OptionsSelect}' or '{FormControlType.OptionsSelectMultiple}'; provided '{inputModel.ControlType}' ");
            throw new WebAppException();
        }

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputSelect, inputModel)
               ).ConfigureAwait(false);
    }
}