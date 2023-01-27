namespace Examples.AspNetMvcCode.Logic;

public interface IItemUsersChatLogic
{
    ItemUsersChatLgc GetItemUsersChat(bool itemIsOpen, string phase, string state);
    OperationResultLgc SaveUsersChatMessage(ItemUserMessageSubmitLgc inputMessage);
}