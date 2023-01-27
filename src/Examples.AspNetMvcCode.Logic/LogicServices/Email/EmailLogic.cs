namespace Examples.AspNetMvcCode.Logic;

public class EmailLogic : IEmailLogic
{
    private readonly ILogger<EmailLogic> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;

    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly IMainLocalizer _localizer;
    private readonly ITemplateLocalizer _localizerTemplate;

    private readonly IParametersQueries _queryParameters;
    private readonly IUserNotificationQueries _queryUserWithContext;
    private readonly IItemAggregationQueries _queryItemAggregation;
    private readonly IEmailQueries _queryEmail;




    public EmailLogic(
        ILogger<EmailLogic> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextApp contextApp
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IMainLocalizer localizer
        , ITemplateLocalizer localizerTemplate
        , IParametersQueries queryParameters
        , IUserNotificationQueries queryUserWithContext
        , IItemAggregationQueries queryItemAggregation
        , IEmailQueries queryEmail
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _localizer = localizer;
        _localizerTemplate = localizerTemplate;
        _queryParameters = queryParameters;
        _queryUserWithContext = queryUserWithContext;
        _queryItemAggregation = queryItemAggregation;
        _queryEmail = queryEmail;
    }


    private record EmailNotificationData(
        List<string> ToEmaiAddressList
        , bool HasOnlyBasicUserEmails
        , ItemDetailQr ItemDetail
        ) : IEmailForNotificationLgc;


    private EmailNotificationData GetItemAndRecipients()
    {
        ItemDetailQr itemDetail =
            _queryItemAggregation.GetItemStepDetail(
                _contextApp.CurrentCultureIsoCode
                , _contextUser.AssignedSupervisorRolesFound.HasValues()
                , _contextUser.ItemIdCurrentlyManagedByUser
                );

        Guard.Against.Null(itemDetail, nameof(ItemDetailQr));

        IEnumerable<string> emailAddressNotificationsFound =
            _queryUserWithContext.GetRecipientsForMailNotification(
                companyGroupId: _contextTenant.CompanyGroupId
                , itemId: _contextUser.ItemIdCurrentlyManagedByUser
                , phase: itemDetail.Phase
                , state: itemDetail.State
                );

        if (emailAddressNotificationsFound.IsNullOrEmpty())
        {
            return
                new EmailNotificationData(
                    ToEmaiAddressList: new List<string>()
                    , HasOnlyBasicUserEmails: true
                    , ItemDetail: itemDetail
                    );
        }

        return
            new EmailNotificationData(
                    ToEmaiAddressList: emailAddressNotificationsFound.ToList()
                    , HasOnlyBasicUserEmails: false
                    , ItemDetail: itemDetail
                    );
    }


    /// <summary>
    /// get email data for email notification after step changing
    /// </summary>
    /// <returns>oggetto, template contenuto email, mail per l'invio, necessario per verifica se serve l'invio email o no</returns>
    public EmailForStepChangeLgc GetDataForChangeNotification()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetDataForChangeNotification) }
                });

        _logger.LogDebug("CALL");



        EmailNotificationData emailNotificationData = GetItemAndRecipients();

        return
            new EmailForStepChangeLgc(
                ToEmaiAddressList: emailNotificationData.ToEmaiAddressList
                , HasOnlyBasicUserEmails: emailNotificationData.HasOnlyBasicUserEmails

                , Subject:
                    _queryParameters.GetEmailSubjectWithReplacements(
                        emailNotificationData.ItemDetail.ItemDescriptiveCode
                        , emailNotificationData.ItemDetail.StepDescription
                        , emailNotificationData.ItemDetail.ProcessDescription
                        )

                , Content:
                    _queryParameters.GetEmailBodyWithReplacements(
                        emailNotificationData.ItemDetail.ItemDescriptiveCode
                        , emailNotificationData.ItemDetail.StepDescription
                        , emailNotificationData.ItemDetail.ProcessDescription
                        )

                , IsOpen: emailNotificationData.ItemDetail.StepStateGroup.IsOpen()
                );
    }


    /// <summary>
    /// get data for email notification after inclusion. Only user with modify permission will be notified. 
    /// If setting is enabled, user with view permission will also be notified
    /// </summary>
    public EmailForInclusionLgc GetDataForInclusionNotification()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetDataForInclusionNotification) }
                });

        _logger.LogDebug("CALL");



        EmailNotificationData emailNotificationData = GetItemAndRecipients();

        return
            new EmailForInclusionLgc(
                ToEmaiAddressList: emailNotificationData.ToEmaiAddressList
                , HasOnlyBasicUserEmails: emailNotificationData.HasOnlyBasicUserEmails
                , ProcessDescription: emailNotificationData.ItemDetail.ProcessDescription
                , ItemDescriptiveCode: emailNotificationData.ItemDetail.ItemDescriptiveCode
                );
    }



    /// <summary>
    /// get parameters for email sending, as configured for current tenant database
    /// </summary>
    /// <returns></returns>
    public EmailConfigTenantLgc GetBaseEmailParams()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetBaseEmailParams) }
                });

        _logger.LogDebug("CALL");



        EmailConfigTenantQr emailConfigTenant = _queryParameters.GetEmailSmtpTenantConfig();

        return
            emailConfigTenant.MapFromDataToLogicWithNullCheck();
    }



    public bool ForceNotificationEmailForItemChangeStep()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ForceNotificationEmailForItemChangeStep) }
                });

        _logger.LogDebug("CALL");



        return
            _queryParameters.ForceNotificationEmailForItemChangeStep();
    }


}