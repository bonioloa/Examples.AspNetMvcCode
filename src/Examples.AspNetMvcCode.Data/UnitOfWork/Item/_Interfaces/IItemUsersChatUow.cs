namespace Examples.AspNetMvcCode.Data;

public interface IItemUsersChatUow
{
    void SaveNewMessageWithAttachmentsAndCommit(ItemUserMessageSubmitQr itemUserMessageSubmit);
}