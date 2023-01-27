namespace Examples.AspNetMvcCode.Data;

internal static class MessageQrUtility
{
    //attachment for message has a "peculiar" saving convention
    //because there was no way to separate them from Item attachment
    //so messageId is saved in phase and this constant in state
    /// <summary>
    /// code to use for "state" column when saving attachments to messages
    /// </summary>
    internal const string StateForMessageAttachment = "MSG";
}