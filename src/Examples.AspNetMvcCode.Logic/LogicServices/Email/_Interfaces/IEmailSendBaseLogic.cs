namespace Examples.AspNetMvcCode.Logic;

public interface IEmailSendBaseLogic
{
    void SendEmailsOverrideWithSettings(EmailSendBaseLgc emailSendBase);
    void SendEmailsWithDefaultFromAddress(EmailSendBaseLgc emailSendBase);
    void SendSupportEmail(SupportMailType mailType, string subject, string content);
}