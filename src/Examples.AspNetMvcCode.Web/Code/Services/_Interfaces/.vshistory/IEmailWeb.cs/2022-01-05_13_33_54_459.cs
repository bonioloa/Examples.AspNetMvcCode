namespace Comunica.ProcessManager.Web.Code;

public interface IEmailWeb
{
    void SendEmailCredentialRecover(RecoverType recoverType, string password, string userLogin, string email);
    void SendEmailNotificationChange();
    void SendEmailRegistrationValidated(string email);
    void SendEmail2faCode(string userEmail, string code);
    void SendEmailNotificationInclusion();
    void SendEmailRegistration(string userName, string userLogin, string password, string email, string validationCode, Uri completeUrlValidationWithParamenter, Uri completeUrlValidationSimple);
    void SendWbSupportNotificationEmail(SupportMailType mailType, string subject, string content);
    void SendCredentialsForSupervisorsNewConfigRelease();
    void SendEmailProblemReport(ProblemReportViewModel problemReport);
    void SendScheduledNotifications(IList<EmailNotificationModel> emailNotificationList);
    void SendEmailSupportForScheduling();
}
