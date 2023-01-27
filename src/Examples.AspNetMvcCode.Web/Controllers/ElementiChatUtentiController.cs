namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.IfUserHasLoginCodeMustMatchCurrentItem)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
[ServiceFilter(typeof(RedirectIfAccessSimpleAnonymousFilter), Order = 3)]
[ServiceFilter(typeof(RequiresManagedItemIdFilter), Order = 4)]
public class ElementiChatUtentiController : BaseContextController
{
    private readonly ContextUser _contextUser;

    private readonly IItemUsersChatLogic _logicItemUsersChat;

    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;
    private readonly IResultMessageMapperWeb _webResultMessageMapper;

    public ElementiChatUtentiController(
        ContextUser contextUser
        , IItemUsersChatLogic logicItemUsersChat
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        , IResultMessageMapperWeb webResultMessageMapper
        ) : base(webHttpContextAccessor)
    {
        _contextUser = contextUser;
        _logicItemUsersChat = logicItemUsersChat;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
        _webResultMessageMapper = webResultMessageMapper;
    }


    [HttpPost]
    public IActionResult InviaMessaggio(IFormCollection form)
    {
        ItemUserMessageSubmitViewModel messageViewModel =
            _webHtmlFormToModelMapper.MapItemUserMessageSubmit(form);


        ItemUserMessageSubmitLgc message = messageViewModel.MapFromWebToLogic();


        OperationResultLgc result = _logicItemUsersChat.SaveUsersChatMessage(message);


        OperationResultViewModel modelMessage = result.MapFromLogicToWeb();

        modelMessage = _webResultMessageMapper.SetResultForItemUserMessageSubmit(modelMessage);


        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        //reload current item confirmation message
        return
            RedirectToAction(
                MvcComponents.ActViewAndManage
                , MvcComponents.CtrlItemManagement
                , new Dictionary<string, string>()
                    {
                        {ParamsNames.ItemId, _contextUser.ItemIdCurrentlyManagedByUser.ToString() }
                    }
                );
    }
}