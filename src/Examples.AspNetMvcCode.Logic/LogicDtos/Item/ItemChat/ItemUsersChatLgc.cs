namespace Examples.AspNetMvcCode.Logic;

public record ItemUsersChatLgc(
    bool UserHasAccess
    , long ItemSubmitterUserId
    , IList<ItemUserMessageLgc> Messages
    , bool EnableMessageCompose
    , string Phase
    , string State
    );