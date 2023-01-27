namespace Comunica.ProcessManager.Web.ViewComponents;

public class ModalTriggerViewComponent : ViewComponent
{
    private readonly ILogger<ModalTriggerViewComponent> _logger;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly MainLocalizer _localizer;

    public ModalTriggerViewComponent(
        ILogger<ModalTriggerViewComponent> logger
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , MainLocalizer localizer
        )
    {
        _logger = logger;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _localizer = localizer;
    }




    public async Task<IViewComponentResult> InvokeAsync(ModalTriggerViewModel model)
    {
        _logger.LogAppDebug("CALL");

        if (model is null)
        {
            _logger.LogAppError($"'{nameof(ModalTriggerViewComponent)}' input model empty");
            throw new WebAppException();
        }
        if (model.ModalId.Empty())
        {
            _logger.LogAppError($"'{nameof(ModalTriggerViewComponent)}' input model '{nameof(model.ModalId)}' empty ");
            throw new WebAppException();
        }
        //do not validate Description because it can be empty for icon trigger

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompModalTrigger, model)
               ).ConfigureAwait(false);
    }
}
