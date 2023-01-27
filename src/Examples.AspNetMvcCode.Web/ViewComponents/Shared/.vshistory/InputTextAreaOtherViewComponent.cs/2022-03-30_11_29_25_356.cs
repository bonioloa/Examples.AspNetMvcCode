namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputTextAreaOtherViewComponent : ViewComponent
{
    private readonly ILogger<InputTextAreaOtherViewComponent> _logger;


    public InputTextAreaOtherViewComponent(
        ILogger<InputTextAreaOtherViewComponent> logger
        )
    {
        _logger = logger;
    }




    public async Task<IViewComponentResult> InvokeAsync(
        InputControlViewModel inputModel
        )
    {
        _logger.LogAppDebug("CALL");

        if (inputModel.RelatedFieldHtmlLabel.Invalid())
        {
            return await Task.FromResult<IViewComponentResult>(
                     Content(string.Empty)
                     ).ConfigureAwait(false);
        }
        else
        {
            InputControlViewModel outputModel = new()
            {
                ControlType = FormControlType.InputTextArea,
                InputSimpleType = InputSimpleType.Text,
                FieldName = inputModel.FieldName + AppConstants.HtmlNameOtherFieldSuffix,
                HideMandatorySymbol = true,
                Description = inputModel.RelatedFieldHtmlLabel,
                CharactersLimit = WebAppConstants.OtherTextAreaMaxLenth,
                UseFixedLabel = true, //always, because it only appears for customer configurable controls
                                      //this must be handled more gracefully
            };

            return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputTextAreaOther, outputModel)
               ).ConfigureAwait(false);
        }
    }
}