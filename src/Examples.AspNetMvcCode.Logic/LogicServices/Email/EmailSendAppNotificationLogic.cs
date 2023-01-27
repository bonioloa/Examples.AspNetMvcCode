namespace Examples.AspNetMvcCode.Logic;

public class EmailSendAppNotificationLogic : IEmailSendAppNotificationLogic
{
    private readonly ILogger<EmailSendAppNotificationLogic> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextUser _contextUser;

    private readonly IEmailLogic _logicEmail;
    private readonly IEmailSendBaseLogic _logicEmailSendBase;

    private readonly IMainLocalizer _localizer;
    private readonly ITemplateLocalizer _localizerTemplate;

    public EmailSendAppNotificationLogic(
        ILogger<EmailSendAppNotificationLogic> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextUser contextUser
        , IEmailLogic logicEmail
        , IEmailSendBaseLogic logicEmailSendBase
        , IMainLocalizer localizer
        , ITemplateLocalizer localizerTemplate
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextUser = contextUser;
        _logicEmail = logicEmail;
        _logicEmailSendBase = logicEmailSendBase;
        _localizer = localizer;
        _localizerTemplate = localizerTemplate;
    }




    /*deve essere verificato su tabella sys parametri che invio email sia abilitato, 
     * per evitare invii email alle utenze o ai sistemi ticketing durante possibili test*/

    public void SendEmailNotificationChange(Uri linkToItem)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SendEmailNotificationChange) }
                });

        _logger.LogDebug("CALL");



        if (!_logicEmail.ForceNotificationEmailForItemChangeStep()
                && _optProduct.Value.DisableEmailNotificationsChangeStep)
        {
            _logger.LogInformation("app notification disabled by appsettings and not overridden (CHSTEP_OVR)");
            return;
        }


        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();

        if (!emailConfigTenantLgc.MailSendEnabled)
        {
            _logger.LogInformation("app notification disabled by tenant config (EML)");
            return;
        }

        EmailForStepChangeLgc emailForStepChange = _logicEmail.GetDataForChangeNotification();

        if (emailForStepChange.ToEmaiAddressList.IsNullOrEmpty())
        {
            if (emailForStepChange.IsOpen
                && !emailForStepChange.HasOnlyBasicUserEmails)
            {
                /*if this happens in production environment it's a problem
                because this means that no users can modify item when it's in current step
                If email send is activated and work progress is not at ending step (open), 
                all notification must be sent to at least one address
                so we send a warning email, without throwing error for user*/
                _logger.LogError("no mails found for mail send");

                _logicEmailSendBase.SendSupportEmail(
                    SupportMailType.MissingSupervisorsEmail
                    , subject: "Errore email responsabili non trovate"
                    , content:
                        @$"
                            Attenzione!!! 
                            Invio notifica cambio stato/fase fallito perché non sono state trovate email per l'invio. 
                            User '{JsonSerializer.Serialize(_contextUser)}' "
                    );
                return;
            }

            if (emailForStepChange.HasOnlyBasicUserEmails)
            {
                _logger.LogInformation("this notification has only basic user email addresses. We can't send notification to whistleblower to prevent a privacy law fine");
            }

            if (!emailForStepChange.IsOpen)
            {
                _logger.LogInformation("config has no email address for step change notification, but we also detected that item is in 'Chiuso' step. By construction normally no emails should be sent when item is changed to closed step");
            }


            return;
        }


        _logicEmailSendBase.SendEmailsOverrideWithSettings(
            new EmailSendBaseLgc(
                TenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
                , UrlReplacementRules:
                    new Dictionary<string, Uri>()
                    {
                        {
                            EmailQrUtility.PlhUrlItem,
                            linkToItem
                        }
                    }
                , EmailNotificationList:
                    new List<EmailNotificationLgc>()
                    {
                        new EmailNotificationLgc(
                            EmailFromAddress: string.Empty
                            , EmailFromDisplayName: string.Empty
                            , EmailToList: emailForStepChange.ToEmaiAddressList
                            , EmailCCList: new List<string>()
                            , EmailBCCList: new List<string>()
                            , Subject: emailForStepChange.Subject
                            , Body: emailForStepChange.Content
                            )
                    }
                )
            );
        return;
    }


    /// <summary>
    /// send a email to included users with same logic as step change
    /// </summary>
    public void SendEmailNotificationInclusion()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SendEmailNotificationInclusion) }
                });

        _logger.LogDebug("CALL");



        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();

        if (!emailConfigTenantLgc.MailSendEnabled)
        {
            _logger.LogInformation("app notification disabled by tenant config");
            return;
        }


        EmailForInclusionLgc emailForInclusion =
            _logicEmail.GetDataForInclusionNotification();

        if (emailForInclusion.ToEmaiAddressList.IsNullOrEmpty())
        {
            //for inclusion is not mandatory to send notification if no user with modify permission was included
            return;
        }


        GetEmailNotificationInclusionTextByProduct(out string subject, out string tmpContent);

        tmpContent =
            tmpContent
            .ReplaceInvariant(AppEmailPlaceholders.ItemDescriptiveCode, emailForInclusion.ItemDescriptiveCode)
            .ReplaceInvariant(AppEmailPlaceholders.ProcessDescription, emailForInclusion.ProcessDescription.GetStringContent());

        _logicEmailSendBase.SendEmailsOverrideWithSettings(
            new EmailSendBaseLgc(
                TenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
                , UrlReplacementRules: new Dictionary<string, Uri>()
                , EmailNotificationList:
                    new List<EmailNotificationLgc>()
                    {
                        new EmailNotificationLgc(
                            EmailFromAddress: string.Empty
                            , EmailFromDisplayName: string.Empty
                            , EmailToList: emailForInclusion.ToEmaiAddressList
                            , EmailCCList: new List<string>()
                            , EmailBCCList: new List<string>()
                            , Subject: subject
                            , Body: tmpContent
                            )
                    }
                )
            );
        return;
    }


    private void GetEmailNotificationInclusionTextByProduct(out string subject, out string tmpContent)
    {
        switch (_optProduct.Value.Product)
        {
            case Product.:
                subject = _localizer[nameof(LocalizedStr.EmailInclusionSubject)];
                tmpContent = _localizerTemplate[nameof(TemplateLocalized.EmailInclusionContent)];
                break;

            case Product.:
                subject = _localizer[nameof(LocalizedStr.EmailInclusionSubjectAltro)];
                tmpContent = _localizerTemplate[nameof(TemplateLocalized.EmailInclusionContentAltro)];
                break;

            default:
                throw new PmLogicException("unknown product");
        }
    }



    public void SendEmailProblemReport(
        ProblemReportLgc problemReport
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SendEmailProblemReport) }
                });

        _logger.LogDebug("CALL");



        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();

        if (!emailConfigTenantLgc.MailSendEnabled)
        {
            _logger.LogInformation("app notification disabled by tenant config");

            return;
        }


        EmailNotificationLgc emailNotificationLgc =
            _logicEmail.GetProblemReportEmail(problemReport);


        _logicEmailSendBase.SendEmailsOverrideWithSettings(
            new EmailSendBaseLgc(
                TenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
                , UrlReplacementRules: new Dictionary<string, Uri>()
                , EmailNotificationList:
                    new List<EmailNotificationLgc>()
                    {
                        emailNotificationLgc
                    }
                )
            );
    }
}