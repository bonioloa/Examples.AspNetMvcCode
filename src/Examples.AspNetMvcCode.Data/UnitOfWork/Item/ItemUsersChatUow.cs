namespace Examples.AspNetMvcCode.Data;

public class ItemUsersChatUow : IItemUsersChatUow
{
    private readonly ILogger<ItemUsersChatUow> _logger;

    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;

    private readonly IProgressiveWriteQueries _queryProgressiveWrite;
    private readonly IMessageWriteQueries _queryMessageWrite;

    private readonly IFileAttachmentUow _uowFileAttachment;

    public ItemUsersChatUow(
        ILogger<ItemUsersChatUow> logger
        , IDataCommandManagerTenant dataCommandManagerTenant
        , IProgressiveWriteQueries queryProgressiveWrite
        , IMessageWriteQueries queryMessageWrite
        , IFileAttachmentUow uowFileAttachment
        )
    {
        _logger = logger;
        _dataCommandManagerTenant = dataCommandManagerTenant;
        _queryProgressiveWrite = queryProgressiveWrite;
        _queryMessageWrite = queryMessageWrite;
        _uowFileAttachment = uowFileAttachment;
    }



    public void SaveNewMessageWithAttachmentsAndCommit(ItemUserMessageSubmitQr itemUserMessageSubmit)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(SaveNewMessageWithAttachmentsAndCommit) }
               });



        //create new id for message
        long messageNewId = _queryProgressiveWrite.CreateAndCommitNewIdForMessage();

        //add new id to attachments
        if (itemUserMessageSubmit.AttachmentList.HasValues())
        {
            foreach (FileAttachmentQr attachment in itemUserMessageSubmit.AttachmentList)
            {
                attachment.Id = _queryProgressiveWrite.CreateAndCommitNewIdForAttachment();
            }
        }

        EnqueueNewMessageWithAttachments(
            messageNewId
            , itemUserMessageSubmit
            , operationDateTime: itemUserMessageSubmit.OperationDateTime
            );

        _dataCommandManagerTenant.CommitCommandsInCurrentBatch();


        _logger.LogInformation(
            "new message '{MessageNewId}' saved for item '{ItemId}' "
            , messageNewId
            , itemUserMessageSubmit.ItemId
            );
    }



    private void EnqueueNewMessageWithAttachments(
        long messageNewId
        , ItemUserMessageSubmitQr itemUserMessageSubmit
        , DateTime operationDateTime
        )
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(EnqueueNewMessageWithAttachments) }
               });



        _queryMessageWrite.EnqueueInsertItemUserChatMessage(
            new ItemUserMessageWriteQr(
                UserId: itemUserMessageSubmit.UserId
                , ItemId: itemUserMessageSubmit.ItemId
                , ProcessId: itemUserMessageSubmit.ProcessId
                , Phase: itemUserMessageSubmit.Phase
                , State: itemUserMessageSubmit.State
                , NewIdMessage: messageNewId
                , Subject: itemUserMessageSubmit.Subject
                , Text: itemUserMessageSubmit.Text
                , OperationDateTime: operationDateTime
                )
            );

        _uowFileAttachment.EnqueueUserMessageAttachments(
            messageId: messageNewId
            , itemUserMessageSubmit: itemUserMessageSubmit
            , operationDateTime: operationDateTime
            );


        _logger.LogInformation(
            "new message '{MessageNewId}' enqueued for item '{ItemId}' "
            , messageNewId
            , itemUserMessageSubmit.ItemId
            );
    }
}