namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class AppWarningComp : ViewComponent
{
    private readonly ILogger<AppWarningComp> _logger;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    private readonly IResultMessageMapperWeb _webResultMessageMapper;

    public AppWarningComp(
        ILogger<AppWarningComp> logger
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IResultMessageMapperWeb webResultMessageMapper
        )
    {
        _logger = logger;
        _webHttpContextAccessor = webHttpContextAccessor;
        _webResultMessageMapper = webResultMessageMapper;
    }



    public async Task<IViewComponentResult> InvokeAsync()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });



        //from temp data
        OperationResultViewModel operationResult = _webHttpContextAccessor.SessionOperationResult;

        if (operationResult is null)
        {
            return
                await Task.FromResult<IViewComponentResult>(
                                 Content(string.Empty)
                                 ).ConfigureAwait(false);
        }

        _logger.LogWarning("found message data for display");


        _webHttpContextAccessor.SessionRemoveOperationResult();//clean message so it will show only on next action

        AppWarningViewModel messagesModel = _webResultMessageMapper.GetLocalized(operationResult);


        string title = messagesModel.Title;
        _logger.LogWarning(
            "title: '{Title}'. Message :'{MessageLines}' "
            , title
            , messagesModel.MessageLines
            );


        return
            await Task.FromResult<IViewComponentResult>(
                   View(MvcComponents.SharedViewCompAppWarning, messagesModel)
                   ).ConfigureAwait(false);
    }
}