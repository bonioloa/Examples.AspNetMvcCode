namespace Examples.AspNetMvcCode.Logic;

public class ItemUsersChatLogic : IItemUsersChatLogic
{
    private readonly ILogger<ItemUsersChatLogic> _logger;
    private readonly ContextApp _contextApp;
    private readonly ContextUser _contextUser;

    private readonly IPermissionItemReadQueries _queryPermissionItemRead;
    private readonly IMessageReadQueries _queryMessageRead;
    private readonly IItemAggregationQueries _queryItemAggregation;

    private readonly IItemUsersChatUow _uowItemUsersChat;


    public ItemUsersChatLogic(
        ILogger<ItemUsersChatLogic> logger
        , ContextApp contextApp
        , ContextUser contextUser
        , IItemAggregationQueries queryItemAggregation
        , IPermissionItemReadQueries queryPermissionItemRead
        , IMessageReadQueries queryMessageRead
        , IItemUsersChatUow uowItemUsersChat
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _contextUser = contextUser;
        _queryItemAggregation = queryItemAggregation;
        _queryPermissionItemRead = queryPermissionItemRead;
        _queryMessageRead = queryMessageRead;
        _uowItemUsersChat = uowItemUsersChat;
    }



    public OperationResultLgc SaveUsersChatMessage(ItemUserMessageSubmitLgc inputMessage)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SaveUsersChatMessage) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.Null(inputMessage, nameof(inputMessage));


        OperationResultLgc result =
            new()
            {
                FieldToWarnList = new List<MessageField>(),
            };

        if (inputMessage.Subject.Empty())
        {
            result.FieldToWarnList.Add(MessageField.MessageSubject);
        }

        if (inputMessage.Text.Empty())
        {
            result.FieldToWarnList.Add(MessageField.MessageText);
        }

        if (result.FieldToWarnList.HasValues())
        {
            result.WarningType = WarningType.InvalidOrEmpty;
            return result;
        }

        if (!_queryPermissionItemRead.UserCanViewAndSendMessages())
        {
            result.WarningType = WarningType.InsufficientPermissions;
            return result;
        }


        ItemDetailQr itemDetail =
            _queryItemAggregation.GetItemStepDetail(
                 _contextApp.CurrentCultureIsoCode
                , _contextUser.AssignedSupervisorRolesFound.HasValues()
                , _contextUser.ItemIdCurrentlyManagedByUser
                );

        if (itemDetail.Phase != inputMessage.Phase
            || itemDetail.State != inputMessage.State)
        {
            result.WarningType = WarningType.PageNeedsRefresh;
            return result;
        }


        _uowItemUsersChat.SaveNewMessageWithAttachmentsAndCommit(
            new ItemUserMessageSubmitQr(
                UserId: _contextUser.UserIdLoggedIn
                , ItemId: _contextUser.ItemIdCurrentlyManagedByUser
                , ProcessId: itemDetail.ProcessId
                , Phase: inputMessage.Phase
                , State: inputMessage.State
                , Subject: inputMessage.Subject
                , Text: inputMessage.Text
                , AttachmentList: inputMessage.AttachmentList.MapIListFromLogicToData()
                , OperationDateTime: DateTime.Now
                )
            );

        result.Success = true;
        return result;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIsOpen"></param>
    /// <param name="phase"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public ItemUsersChatLgc GetItemUsersChat(
        bool itemIsOpen
        , string phase
        , string state
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetItemUsersChat) }
                });

        _logger.LogDebug("CALL");



        IList<ItemUserMessageQr> itemUserMessageList = _queryMessageRead.GetItemUserChatMessageList();

        return
            new ItemUsersChatLgc(
                UserHasAccess: _queryPermissionItemRead.UserCanViewAndSendMessages()
                , ItemSubmitterUserId: _contextUser.UserIdLoggedIn
                //If is closed, messages are only displayed and sending is not enabled
                , EnableMessageCompose: itemIsOpen
                //message list can be null
                , Messages: itemUserMessageList.MapIListFromDataToLogic()
                , Phase: phase
                , State: state
                );
    }
}