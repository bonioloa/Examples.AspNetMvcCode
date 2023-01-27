namespace Examples.AspNetMvcCode.Logic;

public class EmailSendBaseLogic : IEmailSendBaseLogic
{
    private readonly ILogger<EmailSendBaseLogic> _logger;
    private readonly IOptionsSnapshot<EmailClientSettings> _optEmailClient;
    private readonly IOptionsSnapshot<EmailDefaultSettings> _optEmailDefault;
    private readonly IOptionsSnapshot<EmailUfSupportSettings> _optEmailUfSupport;

    private readonly IMainLocalizer _localizer;
    private readonly ITemplateLocalizer _localizerTemplate;

    public EmailSendBaseLogic(
        ILogger<EmailSendBaseLogic> logger
        , IOptionsSnapshot<EmailClientSettings> optEmailClient
        , IOptionsSnapshot<EmailDefaultSettings> optEmailDefault
        , IOptionsSnapshot<EmailUfSupportSettings> optEmailUfSupport
        , IMainLocalizer localizer
        , ITemplateLocalizer localizerTemplate
        )
    {
        _logger = logger;
        _optEmailClient = optEmailClient;
        _optEmailDefault = optEmailDefault;
        _optEmailUfSupport = optEmailUfSupport;
        _localizer = localizer;
        _localizerTemplate = localizerTemplate;
    }








    private const string MailDefaultCredentials = "DEF";

    /// <summary>
    /// base method for mail sending
    /// </summary>
    /// <param name="tenantCustomSmtp">OPTIONAL; if empty, application SMTP will be used</param>
    /// <param name="urlReplacementRules">OPTIONAL;placeholder is the key</param>
    /// <param name="emailModelList">MANDATORY:</param>
    private void SendAppEmails(EmailSendBaseLgc emailSendBase)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SendAppEmails) }
                });




        //if no specified, use default smtp
        string tenantCustomSmtp =
            emailSendBase.TenantCustomSmtp.Empty()
            ? _optEmailClient.Value.DefaultSmtp
            : emailSendBase.TenantCustomSmtp;

        if (emailSendBase.EmailNotificationList.IsNullOrEmpty())
        {
            return;
        }


        using SmtpClient client = new(tenantCustomSmtp, _optEmailClient.Value.SmtpPort);

        if (_optEmailClient.Value.Credentials.EqualsInvariant(MailDefaultCredentials))
        {
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        string emailBody = string.Empty;
        bool errorsHappened = false;
        foreach (EmailNotificationLgc email in emailSendBase.EmailNotificationList)
        {
            emailBody =
                SharedEmailContentReplacements(
                    email.Body
                    , emailSendBase.UrlReplacementRules
                    );

            using MailMessage mailMessage =
                new()
                {
                    From =
                        new MailAddress(
                            email.EmailFromAddress
                            , email.EmailFromDisplayName
                            ),
                    Subject = email.Subject,
                    IsBodyHtml = true,
                    Body = emailBody,
                };

            //no method available to preventively check if email is invalid
            //so we need to use try catch, log the wrong email and let the code go on
            if (email.EmailToList.HasValues())
            {
                foreach (string mail in email.EmailToList)
                {
                    try
                    {
                        mailMessage.To.Add(mail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "error while adding To email, value {Mail} ", mail);
                    }
                }
            }


            if (email.EmailCCList.HasValues())
            {
                foreach (string mail in email.EmailCCList)
                {
                    try
                    {
                        mailMessage.CC.Add(mail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "error while adding CC email, value {Mail} ", mail);
                    }
                }
            }


            if (email.EmailBCCList.HasValues())
            {
                foreach (string mail in email.EmailBCCList)
                {
                    try
                    {
                        mailMessage.Bcc.Add(mail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "error while adding BCC email, value {Mail} ", mail);
                    }
                }
            }


            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                errorsHappened = true;
                //TODO backup email content for send later
                _logger.LogError(ex, "error while sending email. Try again after making sure that site is hosted from a UF network server");
            }


            if (!errorsHappened)
            {
                _logger.LogInformation("all mails sent successfully");
            }
        }
    }


    private const string PlhMailText = "#testoemail#";
    private const string PlhNewLine = "#acapo#";
    private string SharedEmailContentReplacements(
        string content
        , Dictionary<string, Uri> urlReplacementRules
        )
    {
        //insert content to localized standard template
        string standardTemplate = _localizerTemplate[nameof(TemplateLocalized.EmailNoReplyTemplate)];

        string tmpContent = standardTemplate.ReplaceInvariant(PlhMailText, content);

        if (urlReplacementRules.HasValues())
        {
            foreach (string replacementKey in urlReplacementRules.Keys)
            {
                string url = urlReplacementRules[replacementKey].ToString();

                url = $@"<a href=""{url}"">LINK</a>";//this line must not be used if user request text email

                tmpContent = tmpContent.ReplaceInvariant(replacementKey, url);
            }
        }

        tmpContent = tmpContent.ReplaceInvariant(PlhNewLine, CodeConstants.HtmlNewLineTag);
        //add email standard font
        tmpContent = $@"<font face=""Calibri, Arial"">{tmpContent}</font>";
        //inject allowed html bits
        tmpContent = tmpContent.CleanReplaceWithAllowedHtmlElements();

        return tmpContent;
    }



    //warning: if needed add a flag as argument here to determine if BCC settings must be ignored because the emails contain sensible data
    private List<EmailNotificationLgc> RemapWithSettingsOverrides(
        List<EmailNotificationLgc> emailList
        , bool ignoreSettingsBcc
        )
    {
        if (emailList.IsNullOrEmpty())
        {
            return new List<EmailNotificationLgc>();
        }


        List<EmailNotificationLgc> emailOverridedWithSettingsList = new();

        EmailNotificationLgc tmpEmail;
        foreach (EmailNotificationLgc email in emailList)
        {
            tmpEmail =
                email with
                {
                    EmailFromAddress =
                        email.EmailFromAddress.Empty()
                        ? _optEmailDefault.Value.FromAddress
                        : email.EmailFromAddress,

                    EmailFromDisplayName =
                         email.EmailFromDisplayName.Empty()
                            ? _optEmailDefault.Value.FromDisplayName
                            : email.EmailFromDisplayName,

                    EmailBCCList =
                        ignoreSettingsBcc
                        ? email.EmailBCCList
                        : IncludeBccMailsFromSettings(email.EmailBCCList),
                };

            emailOverridedWithSettingsList.Add(tmpEmail);
        }


        return emailOverridedWithSettingsList;
    }


    /// <summary>
    /// do not use this method with emails that contain sensible data
    /// </summary>
    /// <returns></returns>
    private List<string> IncludeBccMailsFromSettings()
    {
        return IncludeBccMailsFromSettings(new List<string>());
    }


    /// <summary>
    /// do not use this method with emails that contain sensible data
    /// </summary>
    /// <param name="emailBCCList"></param>
    /// <returns></returns>
    private List<string> IncludeBccMailsFromSettings(List<string> emailBCCList)
    {
        emailBCCList ??= new List<string>();

        //not included if not enabled in settings
        if (!_optEmailDefault.Value.IncludeBccMail
                || _optEmailDefault.Value.BccAddresses.IsNullOrEmpty())
        {
            return emailBCCList;
        }

        List<string> emailsBCC = emailBCCList.ToList();

        emailsBCC.AddRange(_optEmailDefault.Value.BccAddresses);

        return emailsBCC;
    }
}