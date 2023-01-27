namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// queries in this class depend only from context language (login invariant)
/// </summary>
public partial class ParametersQueries : IParametersQueries
{
    private readonly ILogger<ParametersQueries> _logger;
    private readonly ContextApp _contextApp;
    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;

    public ParametersQueries(
        ILogger<ParametersQueries> logger
        , ContextApp contextApp
        , IDataCommandManagerTenant dataCommandManagerTenant
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _dataCommandManagerTenant = dataCommandManagerTenant;
    }



    //only query from table Z_SYS_SPARAMETRI for now
    //additional tables used will be added to method comments

    #region email parameters

    public EmailConfigTenantQr GetEmailSmtpTenantConfig()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetEmailSmtpTenantConfig) }
                });



        DataTable smtpTenantConfigTb =
            _dataCommandManagerTenant.ReadData(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT TOP 1 
                            s_valore
                            , b_valore
                        FROM Z_SYS_SPARAMETRI 
                        WHERE s_tipo_param='EML'
                        ",
                });

        if (smtpTenantConfigTb.IsNullOrEmpty())
        {
            throw new PmDataException("tenant smtp configuration not found, configuration row mandatory in database");
        }


        bool mailSendEnabled = smtpTenantConfigTb.Rows[0].GetBoolFromFlagString("b_valore");
        if (!mailSendEnabled)
        {
            _logger.LogWarning("Email sending is DISABLED by database configuration");
        }


        return
            new EmailConfigTenantQr(
                MailSendEnabled: mailSendEnabled
                , TenantCustomSmtpServer: smtpTenantConfigTb.Rows[0].CoalesceAndClean("s_valore")
                );
    }



    private enum EmailTemplateCode
    {
        Body = 10007,
        Subject = 10008,
    }


    public string GetEmailSubjectWithReplacements(
        string itemDescriptiveCode
        , IHtmlContent currentStepDescription
        , IHtmlContent processDescription
        )
    {
        string emailSubjectTemplate = GetEmailTemplate(EmailTemplateCode.Subject);

        return
            ReplaceWithStandardPlaceholders(
                template: emailSubjectTemplate
                , itemDescriptiveCode: itemDescriptiveCode
                , currentStepDescription: currentStepDescription
                , processDescription: processDescription
                );
    }

    public string GetEmailBodyWithReplacements(
        string itemDescriptiveCode
        , IHtmlContent currentStepDescription
        , IHtmlContent processDescription
        )
    {
        string emailBodyTemplate = GetEmailTemplate(EmailTemplateCode.Body);

        return
            ReplaceWithStandardPlaceholders(
                template: emailBodyTemplate
                , itemDescriptiveCode: itemDescriptiveCode
                , currentStepDescription: currentStepDescription
                , processDescription: processDescription
                );
    }


    /// <summary>
    /// for now used only for email change state placeholder replacements.
    /// </summary>
    /// <param name="template">MANDATORY</param>
    /// <param name="itemDescriptiveCode">OPTIONAL</param>
    /// <param name="currentStepDescription">OPTIONAL</param>
    /// <param name="processDescription">OPTIONAL</param>
    /// <returns></returns>
    private static string ReplaceWithStandardPlaceholders(
        string template
        , string itemDescriptiveCode
        , IHtmlContent currentStepDescription
        , IHtmlContent processDescription
        )
    {
        Guard.Against.NullOrWhiteSpace(template, nameof(template));

        //do coalescing of provided data just to prevent errors,
        //because for some configurations they are not present, neither as data, nor as placeholder in template
        return
            template
               .ReplaceInvariant(AppEmailPlaceholders.ItemDescriptiveCode, itemDescriptiveCode.Clean())
               .ReplaceInvariant(AppEmailPlaceholders.StepDescription, currentStepDescription.GetStringContent().Clean())
               .ReplaceInvariant(AppEmailPlaceholders.ProcessDescription, processDescription.GetStringContent().Clean());
    }


    private string GetEmailTemplate(EmailTemplateCode parameter)
    {
        decimal numericParam = (decimal)parameter;

        string emailPartTemplate =
            _dataCommandManagerTenant.ExecuteToGetCleanString(
                new CommandExecutionDb()
                {
                    Parameters =
                        new HashSet<CommandParameterDb>
                        {
                            new () { Name =  SqlParamsNames.ParamsCode, Value = numericParam},
                        },
                    CommandText = $@"
                        SELECT TOP 1
                            s_des_estesa
                        FROM Z_SYS_SPARAMETRI{_contextApp.LanguageSuffix()}
                        WHERE n_cod_param = {SqlParamsNames.ParamsCode}
                            AND b_valore='TRUE'
                            AND s_tipo_param ='SPE'        
                        ",
                });
        if (emailPartTemplate.Empty())
        {
            throw new PmDataException(
                $"parameter 'SPE', '{parameter}', value '{numericParam}' not found in Z_SYS_SPARAMETRI{_contextApp.LanguageSuffix()}"
                );
        }

        return emailPartTemplate;
    }



    /// <summary>
    /// search allowed email registration domains 
    /// -multiple rows
    /// -multiple values on the same row separated by ';' character
    /// </summary>
    /// <returns></returns>
    public IList<string> GetEmailDomainRestriction()
    {
        DataTable dt =
            _dataCommandManagerTenant.ReadData(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT 
                            s_valore 
                        FROM Z_SYS_SPARAMETRI  
                        WHERE s_tipo_param = 'REG' 
                            AND b_valore='TRUE' 
                            AND COALESCE(s_valore,'') <> ''
                        ",
                });

        if (dt.IsNullOrEmpty())
            return null;

        List<string> emailRestrictedDomains = new();
        string tmpEmailRow;
        foreach (DataRow row in dt.Rows)
        {
            tmpEmailRow = row.CoalesceAndClean("s_valore");
            emailRestrictedDomains.AddRange(
                tmpEmailRow.Split(
                                DbConstants.EmailSeparator
                                , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                                )
                           .Select(d => d.Clean())
                );
        }
        return emailRestrictedDomains;
    }
    #endregion



    /// <summary>
    /// local class to model a generic parameter key with values associated with it
    /// </summary>
    /// <param name="ParamKey"></param>
    /// <param name="Text"></param>
    /// <param name="Amount"></param>
    /// <param name="Enabled"></param>
    private record ParameterValueDb(
        string ParamKey
        , string Text
        , decimal Amount
        , bool Enabled
        );

    private const string DisableRegistrationForUsers = "NOREG_USER";
    private const string ParameterConfiguredLanguages = "LANGS";
    /// <summary>
    /// use this method to recover tenant encryption configuration data 
    /// </summary>
    /// <returns></returns>
    public TenantOwnConfigurationQr GetTenantOwnConfiguration()
    {
        List<string> tenantConfigurationParams =
            new()
            {
                DisableRegistrationForUsers
                , ParameterConfiguredLanguages
            };

        IEnumerable<ParameterValueDb> parameters =
            _dataCommandManagerTenant.Query<ParameterValueDb>(
                new CommandExecutionDb()
                {
                    CommandText = @$"
                        SELECT
                            s_tipo_param AS ParamKey
                            , COALESCE(s_valore, '') AS Text
                            , n_valore AS Amount
                            , CONVERT(BIT, 
                                        CASE WHEN b_valore = 'TRUE' 
                                                    OR b_valore = 'VERO' 
                                                    OR b_valore = 'S'
                                            THEN 1
                                            ELSE 0
                                        END
                                    ) AS Enabled

                        FROM Z_SYS_SPARAMETRI
                        WHERE s_tipo_param IN {DbUtility.BuildInWithConstants(tenantConfigurationParams)}
                        ",
                });
        TenantOwnConfigurationQr output = new();
        foreach (ParameterValueDb parameter in parameters)
        {
            switch (parameter.ParamKey)
            {
                case DisableRegistrationForUsers:
                    output.DisableRegistrationForUsers = parameter.Enabled;
                    break;

                case ParameterConfiguredLanguages:
                    output.DbCulturesIsoCodes = GetConfiguredLanguages(parameter);
                    break;

                default:
                    throw new PmDataException($"retrieved a unexpected key '{parameter.ParamKey}' ");
            }
        }

        return output;
    }

    /// <summary>
    /// allow multiple separator for languages code storage
    /// </summary>
    private static readonly char[] LanguagesSeparators = new char[] { ';', ',' };

    private IList<string> GetConfiguredLanguages(ParameterValueDb languageParameter)
    {
        if (languageParameter is null)
        {
            //no languages specified, return default language
            return
                new List<string>() { SupportedCulturesConstants.IsoCodeDefault };
        }

        Guard.Against.InvalidInput(
            languageParameter.ParamKey
            , nameof(languageParameter.ParamKey)
            , (key) => key.EqualsInvariant(ParameterConfiguredLanguages)
            , $"provided value '{languageParameter.ParamKey}'; expected '{ParameterConfiguredLanguages}'"
            );


        if (!languageParameter.Enabled
            || languageParameter.Text.Empty())
        {
            //if no languages specified, return default language
            return
                new List<string>() { SupportedCulturesConstants.IsoCodeDefault };
        }


        string[] dbLanguagesArr =
            languageParameter.Text.Split(
                LanguagesSeparators
                , StringSplitOptions.RemoveEmptyEntries
                );

        IList<string> culturesIsoCodes = new List<string>();

        string tmpCultureIsoCode;
        foreach (string lang in dbLanguagesArr.Distinct())
        {
            tmpCultureIsoCode =
                lang switch
                {
                    DbUtility.DbItalianIsoLanguage
                            => SupportedCulturesConstants.IsoCodeItalian,

                    DbUtility.DbEnglishIsoLanguage
                            => SupportedCulturesConstants.IsoCodeEnglish,

                    DbUtility.DbSpanishIsoLanguage
                            => SupportedCulturesConstants.IsoCodeSpanish,

                    _ => string.Empty,
                };


            if (tmpCultureIsoCode.StringHasValue())
            {
                culturesIsoCodes.Add(tmpCultureIsoCode);
            }
            else
            {
                //we simply ignore unsupported languages
                _logger.LogWarning(
                    "found unsupported language '{Lang}' "
                    , lang
                    );
            }
        }

        //if no language was found set default language
        if (culturesIsoCodes.Count == 0)
        {
            culturesIsoCodes.Add(SupportedCulturesConstants.IsoCodeDefault);
        }

        return culturesIsoCodes;
    }




    #region expiration

    private record ExpirationPeriodConfig(
        string EncodedPeriod
        , string Description
        );
    public IEnumerable<ExpirationConfigQr> GetExpirationConfigs()
    {
        IEnumerable<ExpirationPeriodConfig> expirationPeriodConfigFound =
            _dataCommandManagerTenant.Query<ExpirationPeriodConfig>(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT 
                            s_valore AS {nameof(ExpirationPeriodConfig.EncodedPeriod)}
                            , s_des_sintetica AS {nameof(ExpirationPeriodConfig.Description)}
                        FROM Z_SYS_SPARAMETRI{_contextApp.LanguageSuffix()}
                        WHERE s_tipo_param = 'SCW' 
                            AND b_valore = 'TRUE' 
                        ORDER BY n_cod_param
                        ",
                });

        if (expirationPeriodConfigFound.IsNullOrEmpty())
        {
            return null;
        }


        int groupIndex;
        List<ExpirationConfigQr> result = new();

        foreach (ExpirationPeriodConfig expirationPeriodConfig in expirationPeriodConfigFound)
        {
            ExpirationConfigQr expirationConfig =
                new()
                {
                    Description = expirationPeriodConfig.Description.Clean(),
                };

            string encodedExpirationPeriod = expirationPeriodConfig.EncodedPeriod.Clean();
            if (encodedExpirationPeriod.Empty())
            {
                throw new PmDataException("parameter 'SCW' empty");
            }

            groupIndex = 0;//reset


            //parse all groups string (example LAV-2,LAV-3,6-D,0-D)
            string[] cfgGroups =
                encodedExpirationPeriod
                    .Split(
                        ParametersQrUtility.CfgGroupSeparator
                        , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    .Select(p => p.Clean())
                    .ToArray();
            if (cfgGroups.IsNullOrEmpty() || cfgGroups.Length < 3)
            {
                throw new PmDataException($"parameter 'SCW' should have 3 elements '{string.Join(',', cfgGroups)}' ");
            }


            //group: step from
            string[] cfgGroupValues =
                cfgGroups[groupIndex]
                    .Split(
                        ParametersQrUtility.CfgValueSeparator
                        , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    .Select(p => p.Clean())
                    .ToArray();
            if (cfgGroupValues.IsNullOrEmpty() || cfgGroupValues.Length < 2)
            {
                throw new PmDataException(
                    $"parameter 'SCW' group '{groupIndex}' should have length 2  '{string.Join(',', cfgGroupValues)}' "
                    );
            }
            expirationConfig.StartState = cfgGroupValues[0];
            if (expirationConfig.StartState.Empty())
            {
                throw new PmDataException($"parameter 'SCW' group '{groupIndex}' has start state empty");
            }
            expirationConfig.StartPhase = cfgGroupValues[1];
            if (expirationConfig.StartPhase.Empty())
            {
                throw new PmDataException($"parameter 'SCW' group '{groupIndex}' has start phase empty");
            }

            //group: step to
            groupIndex++;
            cfgGroupValues =
                cfgGroups[groupIndex]
                    .Split(
                        ParametersQrUtility.CfgValueSeparator
                        , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    .Select(p => p.Clean())
                    .ToArray();
            if (cfgGroupValues.IsNullOrEmpty() || cfgGroupValues.Length < 2)
            {
                throw new PmDataException(
                    $"parameter 'SCW' group '{groupIndex}' should have length 2  '{string.Join(',', cfgGroupValues)}' "
                    );
            }
            expirationConfig.EndState = cfgGroupValues[0];
            if (expirationConfig.EndState.Empty())
            {
                throw new PmDataException($"parameter 'SCW' group '{groupIndex}' has end state empty");
            }
            expirationConfig.EndPhase = cfgGroupValues[1];
            if (expirationConfig.EndPhase.Empty())
            {
                throw new PmDataException($"parameter 'SCW' group '{groupIndex}' has end phase empty");
            }


            //group: expiration duration
            groupIndex++;
            cfgGroupValues =
                cfgGroups[groupIndex]
                    .Split(
                        ParametersQrUtility.CfgValueSeparator
                        , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    .Select(p => p.Clean())
                    .ToArray();
            if (cfgGroupValues.IsNullOrEmpty() || cfgGroupValues.Length < 2)
            {
                throw new PmDataException(
                    $"parameter 'SCW' group '{groupIndex}' should have length 2  '{string.Join(',', cfgGroupValues)}' "
                    );
            }

            if (!int.TryParse(cfgGroupValues[0], out int tmpExpirationNumber))
            {
                throw new PmDataException(
                    $"parameter 'SCW' group '{groupIndex}' item 0 should be numeric '{string.Join(',', cfgGroupValues)}' "
                    );
            }


            expirationConfig.GenericQuantity = tmpExpirationNumber;
            expirationConfig.Type = cfgGroupValues[1];
            if (expirationConfig.Type.Empty())
            {
                throw new PmDataException($"parameter 'SCW' group '{groupIndex}' expiration quantity type empty");
            }


            //forth element of scwParameters is never used, discarded
            result.Add(expirationConfig);
        }
        return result;
    }


    private const int DefaultMonthsToExpiration = 6;
    /// <summary>
    /// if parameter is not found or wrong configuration return 
    /// default password expiration
    /// </summary>
    /// <returns></returns>
    public DateTime CalculateNextExpirationPassword()
    {
        string expirationString =
            _dataCommandManagerTenant.ExecuteToGetCleanString(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT TOP 1
                            s_valore 
                        FROM Z_SYS_SPARAMETRI  
                        WHERE s_tipo_param = 'PWD' 
                            AND b_valore='TRUE' 
                        ",
                });

        DateTime defaultExpiration = DateTime.Now.Date.AddMonths(DefaultMonthsToExpiration);
        //example 0-D,6-M
        if (expirationString.Empty()
            || !expirationString.ContainsInvariant(ParametersQrUtility.CfgGroupSeparator.ToString())
            || !expirationString.ContainsInvariant(ParametersQrUtility.CfgValueSeparator.ToString()))
        {
            return defaultExpiration;
        }

        string[] expirations =
            expirationString.Split(
                ParametersQrUtility.CfgGroupSeparator
                , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                );
        if (expirations.IsNullOrEmpty())
        {
            return defaultExpiration;
        }

        #region extrapolate expiration data from parameter

        string[] expirationSingle;
        int months = int.MinValue;
        int days = int.MinValue;
        bool foundDays = false;
        bool foundMonths = false;
        bool testParse;
        foreach (string exp in expirations)
        {
            expirationSingle =
                exp.Split(
                    ParametersQrUtility.CfgValueSeparator
                    , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                    );
            if (expirationSingle.IsNullOrEmpty()
                || expirationSingle.Length < 2)
            {
                continue;
            }

            if (expirationSingle[0].StringHasValue()
                && expirationSingle[1].StringHasValue()
                && expirationSingle[1] == ParametersQrUtility.MonthCode)
            {
                testParse = int.TryParse(expirationSingle[0], out months);
                foundMonths = true;
            }
            if (expirationSingle[0].StringHasValue()
                && expirationSingle[1].StringHasValue()
                && expirationSingle[1] == ParametersQrUtility.DayCode)
            {
                testParse = int.TryParse(expirationSingle[0], out days);
                foundDays = true;
            }
        }

        DateTime date = DateTime.Now.Date;
        if (foundMonths || foundDays)
        {
            if (months.Valid())
            {
                date = date.AddMonths(months);
            }

            if (days.Valid())
            {
                date = date.AddDays(days);
            }

            return date;
        }
        #endregion

        return defaultExpiration;
    }
    #endregion





    /// <summary>
    /// test if one or more SSO method are allowed for users to log in
    /// </summary>
    /// <returns></returns>
    public SsoLoginMode GetSsoLoginMode()
    {
        string ssoType =
            _dataCommandManagerTenant.ExecuteToGetCleanString(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT 
                            s_valore
                        FROM Z_SYS_SPARAMETRI  
                        WHERE s_tipo_param = 'SSO'
                            AND b_valore='TRUE' 
                        ",
                });
        if (ssoType.Empty())
        {
            return SsoLoginMode.Local;
        }

        return ssoType.ToEnum<SsoLoginMode>();
    }



    private bool CheckSimpleParameter(string paramCode)
    {
        string codeFound =
            _dataCommandManagerTenant.ExecuteToGetCleanString(
                new CommandExecutionDb()
                {
                    CommandText = $@"
                        SELECT TOP 1
                            1
                        FROM Z_SYS_SPARAMETRI  
                        WHERE s_tipo_param = '{paramCode}'
                            AND b_valore='TRUE' 
                        ",
                });

        return codeFound.StringHasValue();
    }

    public bool GetTenantDisplayPreferences()
    {
        return CheckSimpleParameter("LOGO_PROD");
    }

    public bool HasReportAdvanced()
    {
        return CheckSimpleParameter("REP_ADV");
    }


    public bool HasOldOptionsSchemaType()
    {
        bool newSchemaFound = CheckSimpleParameter("TABNEW");

        //negate because method is checking if config uses old schema
        return !newSchemaFound;
    }


    public bool TenantCanInsertItemsFromFile()
    {
        return CheckSimpleParameter("INSMASFILE");
    }

    public bool AllowUserChatOnItems()
    {
        return CheckSimpleParameter("MSGENOVER");
    }

    public bool ForceNotificationEmailForItemChangeStep()
    {
        return CheckSimpleParameter("CHSTEP_OVR");
    }
}