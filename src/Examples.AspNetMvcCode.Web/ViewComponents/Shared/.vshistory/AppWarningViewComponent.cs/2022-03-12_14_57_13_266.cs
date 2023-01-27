namespace Comunica.ProcessManager.Web.ViewComponents;

public class AppWarningViewComponent : ViewComponent
{
    private readonly ILogger<AppWarningViewComponent> _logger;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    private readonly IResultMessageMapperWeb _webResultMessageMapper;

    public AppWarningViewComponent(
        ILogger<AppWarningViewComponent> logger
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IResultMessageMapperWeb webResultMessageMapper
        )
    {
        _logger = logger;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _webResultMessageMapper = webResultMessageMapper;
    }



    public async Task<IViewComponentResult> InvokeAsync()
    {
        _logger.LogAppDebug("CALL");

        //from temp data
        OperationResultViewModel operationResult = _httpContextAccessorCustomWeb.SessionOperationResult;
        if (operationResult is null)
        {
            return await Task.FromResult<IViewComponentResult>(
                                 Content(string.Empty)
                                 ).ConfigureAwait(false);
        }
        else
        {
            _httpContextAccessorCustomWeb.SessionRemoveOperationResult();//clean message so it will show only on next action
        }

        _logger.LogAppWarning("found message data for display");

        AppWarningViewModel messagesModel = _webResultMessageMapper.GetLocalized(operationResult);

        _logger.LogAppWarning($"title: '{messagesModel.Title}'. Message :'{JsonSerializer.Serialize(messagesModel.MessageLines)}'");

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompAppWarning, messagesModel)
               ).ConfigureAwait(false);
    }
}
