namespace Examples.AspNetMvcCode.Data;

public interface IMessageWriteQueries
{
    void EnqueueInsertItemUserChatMessage(ItemUserMessageWriteQr itemUserMessageNewToSave);
}