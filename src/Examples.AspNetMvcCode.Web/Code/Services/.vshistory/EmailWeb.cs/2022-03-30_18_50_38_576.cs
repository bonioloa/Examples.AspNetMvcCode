namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// apart from injection a mail client, in this class we prepare mail content
/// for the various app notifications.
/// Replacements are done here because we need localization, 
/// and replacement placeholders are strictly related to localized messages
/// </summary>
public class EmailWeb : IEmailWeb
{
    //shared constants. The other ones used for only one method are placed just above it
    private const string PlhUserLogin = "#userlogin#";
    private const string PlhPassword = "#password#";
    //private const string PlhGenericUrl = "#url#";
    private const string PlhName = "#nome#";

    private readonly ILogger<EmailWeb> _logger;
    private readonly IOptionsSnapshot<EmailClientSettings> _optEmailClient;
    private readonly IOptionsSnapshot<EmailDefaultSettings> _optEmailDefault;
    private readonly IOptionsSnapshot<EmailUfSupportSettings> _optEmailUfSupport;
    private readonly IOptionsSnapshot<SchedulingSettings> _optScheduling;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly IUserConfiguratorLogic _logicUserConfigurator;
    private readonly IEmailLogic _logicEmail;

    private readonly IUrlHelper _urlHelper;
    private readonly MainLocalizer _localizer;

    public EmailWeb(
        ILogger<EmailWeb> logger
        , IOptionsSnapshot<EmailClientSettings> optEmailClient
        , IOptionsSnapshot<EmailDefaultSettings> optEmailDefault
        , IOptionsSnapshot<EmailUfSupportSettings> optEmailUfSupport
        , IOptionsSnapshot<SchedulingSettings> optScheduling
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IUserConfiguratorLogic logicUserConfigurator
        , IEmailLogic logicEmail
        , IUrlHelper urlHelper
        , MainLocalizer localizer
        )
    {
        _logger = logger;
        _optEmailClient = optEmailClient;
        _optEmailDefault = optEmailDefault;
        _optEmailUfSupport = optEmailUfSupport;
        _optScheduling = optScheduling;
        _optProduct = optProduct;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _logicUserConfigurator = logicUserConfigurator;
        _logicEmail = logicEmail;
        _urlHelper = urlHelper;
        _localizer = localizer;
    }





    #region architecture emails (this email will always be sent if email address is set for the logged user)
    /// <summary>
    /// Send notification to configured app support address (should be used in case of application errors)
    /// </summary>
    /// <param name="mailType">to check if specific email is enabled</param>
    /// <param name="subject"></param> 
    /// <param name="content"></param> 
    public void SendWbSupportNotificationEmail(
        SupportMailType mailType
        , string subject
        , string content
        )
    {
        bool enabled = mailType switch
        {
            SupportMailType.GenericError =>
                _optEmailUfSupport.Value.EnableForGenericError,

            SupportMailType.WrongBrowser =>
              _optEmailUfSupport.Value.EnableForErrorNotSupportedBrowser,

            SupportMailType.MissingSupervisorsEmail =>
                _optEmailUfSupport.Value.EnableForCodePages,

            SupportMailType.WrongSchedulingIp =>
                _optEmailUfSupport.Value.EnableForWrongSchedulingIp,

            SupportMailType.CodePage => true,
            _ => false,
        };
        if (enabled)
        {
            SendAppEmails(
              tenantCustomSmtp: string.Empty
              , addConfigBcc: false
              , urlReplacementRules: null
              , new List<EmailNotificationModel>()
                  {
                            new EmailNotificationModel()
                            {
                                EmailToList = _optEmailUfSupport.Value.ToEmailAddress ,
                                Subject = _optEmailUfSupport.Value.PrefixSubject + subject,
                                Body = new HtmlString(content),
                            }
                  }
              );
        }
    }



    private const string PlhUrlValidateRegistrationWithParams = "#UrlValidateRegistrationWithParams#";
    private const string PlhUrlValidateRegistrationSimple = "#UrlValidateRegistrationSimple#";
    private const string PlhValidateRegistrationCode = "#ValidateRegistrationCode#";
    public void SendEmailRegistration(
        string userName
        , string userLogin
        , string password
        , string email
        , string validationCode
        , Uri completeUrlValidationWithParamenter
        , Uri completeUrlValidationSimple
        )
    {
        _logger.LogAppDebug("CALL");

        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();

        //we need to encode the encrypted confirmation code 
        //to prevent errors in link clicking

        string registrationContentTemplate = _localizer[nameof(LocalizedStr.EmailRegisterContentTemplate)];

        string tmpContent = registrationContentTemplate
            .Replace(PlhName, userName)
            .Replace(PlhUserLogin, userLogin)
            .Replace(PlhPassword, password)
            .Replace(PlhValidateRegistrationCode, validationCode);


        SendAppEmails(
          tenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
          , addConfigBcc: false
          , urlReplacementRules: new Dictionary<string, Uri>()
                {
                        { PlhUrlValidateRegistrationWithParams, completeUrlValidationWithParamenter},
                        { PlhUrlValidateRegistrationSimple,completeUrlValidationSimple }
                }
          , new List<EmailNotificationModel>()
              {
                        new EmailNotificationModel()
                        {
                            EmailToList = new List<string> { email },
                            Subject = _localizer[nameof(LocalizedStr.EmailRegisterSubject)],
                            Body = new HtmlString(tmpContent),
                        }
              }
          );
    }



    public void SendEmailRegistrationValidated(string email)
    {
        _logger.LogAppDebug("CALL");


        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();


        SendAppEmails(
          tenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
          , addConfigBcc: false
          , urlReplacementRules: null
          , new List<EmailNotificationModel>()
              {
                        new EmailNotificationModel()
                        {
                            EmailToList = new List<string> { email },
                            Subject = _localizer[nameof(LocalizedStr.EmailValidateRegistrationSubject)],
                            Body = new HtmlString(_localizer[nameof(LocalizedStr.EmailValidateRegistrationContentTemplate)]),
                        }
              }
          );
    }


    public void SendEmailCredentialRecover(
        RecoverType recoverType
        , string password
        , string userLogin
        , string email
        )
    {
        _logger.LogAppDebug("CALL");

        EmailConfigTenantLgc emailConfigTenantLgc = _logicEmail.GetBaseEmailParams();

        string tmpContent;
        switch (recoverType)
        {
            case RecoverType.Password:
                string recoverPasswordContentTemplate =
                    _localizer[nameof(LocalizedStr.EmailRecoverPasswordContentTemplate)];
                tmpContent = recoverPasswordContentTemplate.Replace(PlhPassword, password);
                break;

            case RecoverType.UserLogin:
                string recoverUserLoginContentTemplate =
                    _localizer[nameof(LocalizedStr.EmailRecoverUserLoginContentTemplate)];
                tmpContent = recoverUserLoginContentTemplate.Replace(PlhUserLogin, userLogin);
                break;

            default:
                tmpContent = null;
                break;
        }

        if (tmpContent is null)
        {
            _logger.LogAppError("missing recovertype for mail sending");
            return;
        }

        SendAppEmails(
           tenantCustomSmtp: emailConfigTenantLgc.TenantCustomSmtpServer
           , addConfigBcc: false
           , urlReplacementRules: null
           , new List<EmailNotificationModel>()
               {
                        new EmailNotificationModel()
                        {
                            EmailToList = new List<string> { email },
                            Subject = _localizer[nameof(LocalizedStr.EmailRecoverSubject)],
                            Body = new HtmlString(tmpContent),
                        }
               }
           );
    }


    private const string PlhCode2fa = "#code2fa#";
    public void SendEmail2faCode(string userEmail, string code)
    {
        _logger.LogAppDebug("CALL");


        if (userEmail.Empty())
        {
            _logger.LogAppError($"{nameof(userEmail)} required");
            throw new WebAppException();
        }

        if (code.Empty())
        {
            _logger.LogAppError($"{nameof(code)} required");
            throw new WebAppException();
        }

        (bool _, string tenantCustomSmtp)
            = _logicEmail.GetBaseEmailParams();

        string tmpContent = _localizer[nameof(LocalizedStr.EmailLogin2faContent)];
        tmpContent = tmpContent.Replace(PlhCode2fa, code);

        SendAppEmails(
            tenantCustomSmtp: tenantCustomSmtp
            , addConfigBcc: false
            , urlReplacementRules: null
            , new List<EmailNotificationModel>()
            {
                    new EmailNotificationModel()
                    {
                        EmailToList = new List<string> { userEmail },
                        Subject = _localizer[nameof(LocalizedStr.EmailLogin2faSubject)],
                        Body = new HtmlString(tmpContent),
                    }
            }
            );
    }


    private const string PlhUrlMainAccessWithToken = "#urlMainAccessWithToken#";
    public void SendCredentialsForSupervisorsNewConfigRelease()
    {
        //TODO va parametrizzato per gestire anche registri
        string emailTemplate = @$"
Gentile #nome#,
<br>
di seguito troverai le credenziali per l'accesso all'area riservata dell' applicativo <strong>Comunica Whistleblowing</strong><br>
da cui potrai consultare e gestire le segnalazioni per il profilo a te assegnato:<br>
<br>
User Login: <strong>#userlogin#</strong> <br>
Password: <strong>#password#</strong> <br>
<br>
Per motivi di sicurezza al primo accesso ti sarà richiesto di modificare la password.
<br>
<br>
Clicca sul link per cominciare:<br>
<a href='#urlMainAccessWithToken#'>Whistleblowing - Pagina di accesso</a>
<br>
<br>
<br>
Cordiali saluti,<br>
<i>Team UF</i>
<br>
<br>
";
        string tmpEmail;
        IList<UserInfoForCredentialsEmailLgc> infoForEmailList = _logicUserConfigurator.GetInfoForMassEmailCredentials();
        foreach (UserInfoForCredentialsEmailLgc info in infoForEmailList)
        {
            tmpEmail =
                emailTemplate.Replace(PlhName, info.Name)
                .Replace(PlhUserLogin, info.Login)
                .Replace(PlhPassword, info.Password)
                .Replace(PlhUrlMainAccessWithToken, _urlHelper.AbsoluteActionAccessPage(_contextTenant.Token).ToString());


            SendAppEmails(
                tenantCustomSmtp: string.Empty
                , addConfigBcc: false
                , urlReplacementRules: null
                , new List<EmailNotificationModel>() {
                        new EmailNotificationModel()
                        {
                            EmailToList = new List<string>() { info.Email },
                            Subject = "Comunica Whistleblowing - Accesso",
                            Body = new HtmlString(tmpEmail),
                        }
                }
            );
        }
    }


    public void SendEmailSupportForScheduling()
    {
        SendWbSupportNotificationEmail(
            SupportMailType.WrongSchedulingIp
            , subject: "Errore Schedulazioni"
            , content: @$"E' stata lanciata una schedulazione tramite url, 
ma l' IP della richiesta non corrisponde a quello configurato '{_optScheduling.Value.SchedulingIpConstraint}'. 

Fare una verifica sulla macchina web e verificare i log applicativi cercando il parametro '{nameof(_optScheduling.Value.SchedulingIpConstraint)}' 
Nel caso l' IP della macchina web è corretto, significa che qualcuno sta facendo chiamate dall'esterno.
Se capita questa cose bisogna cominciare a fare una blacklist degli ip esterni.
Non disabilitare l'invio di queste email, perché è necessario essere al corrente di eventuali cambi IP delle macchine"
            );
    }
    #endregion



    /*deve essere verificato su tabella sys parametri che invio email sia abilitato, 
     * per evitare invii email alle utenze o ai sistemi ticketing durante possibili test*/
    #region email applicative 


    private const string PlhUrlItem = "#url#";
    public void SendEmailNotificationChange()
    {
        _logger.LogAppDebug("CALL");

        if (_optProduct.Value.DisableEmailNotificationsChangeStep)
        {
            _logger.LogAppInformation($"app notification disabled by appsettings");
            return;
        }

        (bool mailSendEnabled, string tenantCustomSmtp) = _logicEmail.GetBaseEmailParams();
        if (!mailSendEnabled)
        {
            _logger.LogAppInformation($"app notification disabled by tenant config");
            return;
        }

        (
            string subject
            , string tmpContent
            , IList<string> toEmails
            , bool isOpen
            ) = _logicEmail.GetDataForChangeNotification();

        if (isOpen && toEmails.IsNullOrEmpty())
        {
            /*if this happens in production environment it's a problem
                because this means that no users can modify item when it's in current step
                If email send is activated and work progress is not at ending step (open), 
                all notification must be sent to at least one address
                so we send a warning email, without throwing error for user*/
            _logger.LogAppError("no mails found for mail send");

            this.SendWbSupportNotificationEmail(
                SupportMailType.MissingSupervisorsEmail
                , subject: "Errore email responsabili non trovate"
                , content:
                    @$"
                            Attenzione!!! 
                            Invio notifica cambio stato/fase fallito perché non sono state trovate email per l'invio. 
                            User '{JsonSerializer.Serialize(_contextUser)}' "
                );
        }


        SendAppEmails(
            tenantCustomSmtp
            , addConfigBcc: true
            , urlReplacementRules: new Dictionary<string, Uri>()
                {
                        {
                            PlhUrlItem,
                            _urlHelper.AbsoluteActionItemManagement(
                                _contextUser.IdItemCurrentlyManagedByUser)
                        }
                }
            , new List<EmailNotificationModel>()
                {
                        new EmailNotificationModel()
                        {
                            EmailToList = toEmails,
                            Subject = subject,
                            Body = new HtmlString(tmpContent),
                        }
                }
            );
        return;
    }


    /// <summary>
    /// send a email to included users with same logic as step change
    /// </summary>
    public void SendEmailNotificationInclusion()
    {
        _logger.LogAppDebug("CALL");

        (bool mailSendEnabled, string tenantCustomSmtp) = _logicEmail.GetBaseEmailParams();
        if (!mailSendEnabled)
        {
            _logger.LogAppInformation($"app notification disabled by tenant config");
            return;
        }


        (
            IList<string> toEmails
            , IHtmlContent processDescription
            , string itemDescriptiveCode
            ) = _logicEmail.GetDataForInclusionNotification();

        if (toEmails.IsNullOrEmpty())
        {
            //for inclusion is not mandatory to send notification if no user with modify permission was included
            return;
        }

        string subject = _localizer[nameof(LocalizedStr.EmailInclusionSubject)];
        string tmpContent = _localizer[nameof(LocalizedStr.EmailInclusionContent)];
        tmpContent = tmpContent
            .Replace(AppConstants.MailPlhItemDescriptiveCode, itemDescriptiveCode)
            .Replace(AppConstants.MailPlhProcessDescription, processDescription.GetStringContent());

        SendAppEmails(
            tenantCustomSmtp
            , addConfigBcc: true
            , urlReplacementRules: null
            , new List<EmailNotificationModel>()
                {
                        new EmailNotificationModel()
                        {
                            EmailToList = toEmails,
                            Subject = subject,
                            Body = new HtmlString(tmpContent),
                        }
                }
            );
        return;
    }



    public void SendEmailProblemReport(
        ProblemReportViewModel problemReport
        )
    {
        _logger.LogAppDebug("CALL");

        (bool mailSendEnabled, string tenantCustomSmtp) = _logicEmail.GetBaseEmailParams();
        if (!mailSendEnabled)
        {
            _logger.LogAppInformation($"app notification disabled by tenant config");
            return;
        }

        EmailNotificationLgc emailNotificationLgc =
            _logicEmail.GetProblemReportEmail(
                problemReport.MapFromWebToLogic()
                );


        SendAppEmails(
            tenantCustomSmtp: tenantCustomSmtp
            , addConfigBcc: false
            , urlReplacementRules: null
            , new List<EmailNotificationModel>()
                {
                    emailNotificationLgc.MapFromLogicToWeb()
                }
            );
    }

    public void SendScheduledNotifications(IList<EmailNotificationModel> emailNotificationList)
    {
        _logger.LogAppDebug("CALL");

        (bool mailSendEnabled, string tenantCustomSmtp) = _logicEmail.GetBaseEmailParams();
        if (!mailSendEnabled)
        {
            _logger.LogAppInformation($"app notification disabled by tenant config");
            return;
        }

        SendAppEmails(
            tenantCustomSmtp: tenantCustomSmtp
            , addConfigBcc: false
            , urlReplacementRules: null
            , emailNotificationList
            );
    }
    #endregion







    /// <summary>
    /// base method for mail sending
    /// </summary>
    /// <param name="tenantCustomSmtp">OPTIONAL; if empty, application SMTP will be used</param>
    /// <param name="addConfigBcc">OPTIONAL;</param>
    /// <param name="urlReplacementRules">OPTIONAL;placeholder is the key</param>
    /// <param name="emailNotificationModelList">MANDATORY:</param>
    private void SendAppEmails(
        string tenantCustomSmtp
        , bool addConfigBcc
        , IDictionary<string, Uri> urlReplacementRules
        , IList<EmailNotificationModel> emailNotificationModelList
        )
    {
        //if no specified, use default smtp
        if (tenantCustomSmtp.Empty())
        {
            tenantCustomSmtp = _optEmailClient.Value.DefaultSmtp;
        }
        if (emailNotificationModelList.IsNullOrEmpty())
        {
            return;
        }


        using SmtpClient client = new(tenantCustomSmtp, _optEmailClient.Value.SmtpPort);
        if (_optEmailClient.Value.Credentials == WebAppConstants.MailDefaultCredentials)
        {
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        string emailBody = string.Empty;
        foreach (EmailNotificationModel email in emailNotificationModelList)
        {
            if (email.EmailFromAddress.Empty())
            {
                email.EmailFromAddress = _optEmailDefault.Value.FromAddress;
            }
            if (email.EmailFromDisplayName.Empty())
            {
                email.EmailFromDisplayName = _optEmailDefault.Value.FromDisplayName;
            }

            emailBody =
                SharedEmailContentReplacements(
                    email.Body.GetStringContent()
                    , urlReplacementRules
                    , email.SendMailBodyAsText
                    );

            using MailMessage mailMessage = new()
            {
                From = new MailAddress(
                    email.EmailFromAddress
                    , email.EmailFromDisplayName
                    ),
                Subject = email.Subject,
                IsBodyHtml = !email.SendMailBodyAsText,
                Body = emailBody,
            };

            if (email.EmailToList.HasValues())
            {
                foreach (string mail in email.EmailToList)
                {
                    mailMessage.To.Add(mail);
                }
            }

            if (email.EmailCCList.HasValues())
            {
                foreach (string mail in email.EmailCCList)
                {
                    mailMessage.CC.Add(mail);
                }
            }

            if (email.EmailBCCList.HasValues())
            {
                foreach (string mail in email.EmailBCCList)
                {
                    mailMessage.Bcc.Add(mail);
                }
            }
            if (addConfigBcc
                && _optEmailDefault.Value.IncludeBccMail
                && _optEmailDefault.Value.BccAddresses.StringHasValue())
            {
                mailMessage.Bcc.Add(_optEmailDefault.Value.BccAddresses); //value can be multiple addresses separated by ";"
            }

            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogAppError(ex, "error while sending email. Try again after making sure that site is hosted from a UF network server");
            }

            _logger.LogAppInformation("mail sent");
        }
    }


    private const string PlhMailText = "#testoemail#";
    private const string PlhNewLine = "#acapo#";
    private string SharedEmailContentReplacements(
        string content
        , IDictionary<string, Uri> urlReplacementRules
        , bool sendEmailBodyAsText
        )
    {
        //insert content to localized standard template
        string standardTemplate = _localizer[nameof(LocalizedStr.EmailNoReplyTemplate)];
        string tmpContent = standardTemplate.Replace(PlhMailText, content);

        if (urlReplacementRules.HasValues())
        {
            foreach (string replacementKey in urlReplacementRules.Keys)
            {
                string url = urlReplacementRules[replacementKey].ToString();

                if (!sendEmailBodyAsText)
                {
                    url = $@"<a href=""{url}"">LINK</a>";//this line must not be used if user request text email
                }

                tmpContent = tmpContent.Replace(replacementKey, url);
            }
        }


        if (sendEmailBodyAsText)
        {
            tmpContent = tmpContent.Replace(PlhNewLine, Environment.NewLine);
        }
        else
        {
            tmpContent = tmpContent.Replace(PlhNewLine, "<br>");
            //add email standard font
            tmpContent = $@"<font face=""Calibri, Arial"">{tmpContent}</font>";
            //inject allowed html bits
            tmpContent = tmpContent.CleanReplaceWithAllowedHtmlElements();
        }

        return tmpContent;
    }
}