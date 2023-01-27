namespace Examples.AspNetMvcCode.Logic;

public interface IEmailSendAppNotificationLogic
{
    void SendEmailNotificationChange(Uri linkToItem);
    void SendEmailNotificationInclusion();
    void SendEmailProblemReport(ProblemReportLgc problemReport);
}