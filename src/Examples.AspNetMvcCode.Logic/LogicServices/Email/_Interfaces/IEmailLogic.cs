namespace Examples.AspNetMvcCode.Logic;

public interface IEmailLogic
{
    EmailNotificationLgc GetProblemReportEmail(ProblemReportLgc input);
    EmailConfigTenantLgc GetBaseEmailParams();
    EmailForInclusionLgc GetDataForInclusionNotification();
    EmailForStepChangeLgc GetDataForChangeNotification();
    bool ForceNotificationEmailForItemChangeStep();
    EmailNotificationLgc GetEmailSupervisorCredentials(UserSupervisorDataLgc userSupervisorData, string password, Uri loginPageWithToken);
}