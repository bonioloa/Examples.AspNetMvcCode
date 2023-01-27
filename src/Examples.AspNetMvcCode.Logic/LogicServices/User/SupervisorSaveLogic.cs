namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public class SupervisorSaveLogic : ISupervisorSaveLogic
{
    private readonly ILogger<SupervisorSaveLogic> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly IUserDataReadQueries _queryUserDataRead;

    private readonly IUserRoleAdminManagedUow _uowUserRoleAdminManaged;

    private readonly IRandomGeneratorLogic _logicRandomGenerator;
    private readonly IRoleAdminManagedLogic _logicRoleAdminManaged;
    private readonly ISupervisorSaveChecksLogic _logicSupervisorSaveChecks;

    public SupervisorSaveLogic(
        ILogger<SupervisorSaveLogic> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IUserDataReadQueries queryUserDataRead
        , IUserRoleAdminManagedUow uowUserRoleAdminManaged
        , IRandomGeneratorLogic logicRandomGenerator
        , IRoleAdminManagedLogic logicRoleAdminManaged
        , ISupervisorSaveChecksLogic logicSupervisorSaveChecks
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _queryUserDataRead = queryUserDataRead;
        _uowUserRoleAdminManaged = uowUserRoleAdminManaged;
        _logicRandomGenerator = logicRandomGenerator;
        _logicRoleAdminManaged = logicRoleAdminManaged;
        _logicSupervisorSaveChecks = logicSupervisorSaveChecks;
    }



    public UserNewSupervisorResultLgc SaveNewSupervisor(UserNewSupervisorLgc input)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SaveNewSupervisor) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.Null(input, nameof(UserNewSupervisorLgc));

        string errorMessage = ValidateNewSupervisorSubmit(input);

        if (errorMessage.StringHasValue())
        {
            return SupervisorSaveLgcUtility.GetSaveNewAsKoData(errorMessage);
        }

        RolesSelectionResultLgc rolesSelectionResult =
            _logicRoleAdminManaged.ValidateAndParseSubmittedRolesSelectionForNewUser(
                selectedExclusiveRole: input.ExclusiveRole
                , selectedSupervisorRoles: input.SupervisorRoles
                );
        if (rolesSelectionResult is null)
        {
            throw new PmLogicException($"{nameof(RolesSelectionResultLgc)} is null");
        }
        if (rolesSelectionResult.ErrorMessage.StringHasValue())
        {
            return SupervisorSaveLgcUtility.GetSaveNewAsKoData(rolesSelectionResult.ErrorMessage);
        }


        string initialPassword = _logicRandomGenerator.GetNewStandardPassword();

        long newUserIdSupervisor =
            _uowUserRoleAdminManaged.CreateNewSupervisor(
                userNewSupervisor:
                 new UserNewSupervisorQr(
                     Login: input.Login
                     , Password: initialPassword
                     , Name: input.Name
                     , Surname: input.Surname
                     , Email: input.Email
                     , Roles: rolesSelectionResult.SelectedRolesToSave
                     )
                , userIdOperation: _contextUser.UserIdLoggedIn
                , operationDateTime: DateTime.Now
                );

        return
            new UserNewSupervisorResultLgc(
                ErrorMessage: string.Empty
                , NewUserIdSupervisor: newUserIdSupervisor
                , Password: initialPassword
                );
    }

    private string ValidateNewSupervisorSubmit(UserNewSupervisorLgc input)
    {
        if (!_contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
        {
            return SupervisorSaveLgcUtility.ErrorSaveNewPermissions;
        }

        if (input.Login.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorLoginEmpty;
        }

        if (input.Login.Length > AppConstants.LoginMaxCharacters)
        {
            return SupervisorSaveLgcUtility.ErrorLoginTooLong;
        }

        if (input.Login.Length < AppConstants.LoginMinCharacters)
        {
            return SupervisorSaveLgcUtility.ErrorLoginTooShort;
        }

        Match match = new Regex(AppRegexPatterns.User).Match(input.Login);
        if (match == null || !match.Success)
        {
            _logger.LogWarning("userLogin {Login} doesn't respect validation pattern", input.Login);

            return SupervisorSaveLgcUtility.ErrorLoginInvalidFormat;
        }

        match = new Regex(AppRegexPatterns.LoginCode).Match(input.Login);
        if (match != null && match.Success)
        {
            _logger.LogWarning("userLogin {Login} has a login code pattern, not allowed for named users", input.Login);
            //same error user message, logs will discriminate the type of error
            return SupervisorSaveLgcUtility.ErrorLoginInvalidFormat;
        }


        string errorMessage = _logicSupervisorSaveChecks.CheckIfLoginExists(input.Login);
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }


        if (input.Name.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorNameEmpty;
        }


        if (input.Surname.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorSurnameEmpty;
        }


        if (input.Email.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorEmailEmpty;
        }

        match = new Regex(RegexPatterns.Email).Match(input.Email);
        if (match == null || !match.Success)
        {
            return SupervisorSaveLgcUtility.ErrorEmailInvalidFormat;
        }


        errorMessage = _logicSupervisorSaveChecks.CheckIfEmailExists(input.Email);
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }

        return string.Empty;
    }






    public string IncludeUserInManaged(long userId)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(IncludeUserInManaged) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.NegativeOrZero(userId, nameof(userId));

        if (!_contextUser.IsAdminApplication())
        {
            return SupervisorSaveLgcUtility.ErrorIncludeManagedPermissions;
        }


        UserDataQr credentialUserToInclude =
            _queryUserDataRead.GetByUserId(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userId: userId
                );
        if (credentialUserToInclude is null)
        {
            return SupervisorSaveLgcUtility.ErrorIncludeUserNotExisting;
        }

        if (credentialUserToInclude.IsTenantManaged)
        {
            return SupervisorSaveLgcUtility.ErrorIncludeUserAlreadyManaged;
        }


        string errorMessage =
            _logicSupervisorSaveChecks.CheckIfLoginExistsForOtherActiveUsers(
                credentialUserToInclude.Login
                , userId
                );
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }

        errorMessage =
            _logicSupervisorSaveChecks.CheckIfEmailExistsForOtherActiveUsers(
               credentialUserToInclude.Email
               , userId
               );
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }



        _uowUserRoleAdminManaged.ChangeUserToTenantManaged(
            _contextUser.UserIdLoggedIn
            , userIdModified: userId
            , operationDateTime: DateTime.Now
            );

        return string.Empty;
    }


    //useless to test, it's practically a pass-through method
    [ExcludeFromCodeCoverage]
    public UserResetResultLgc EnableUser(long userId)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(EnableUser) }
               });

        _logger.LogDebug("CALL");



        string errorMessage = _logicSupervisorSaveChecks.ChecksEnableDisable(userId, isEnableCommand: true);

        if (errorMessage.StringHasValue())
        {
            return
                new UserResetResultLgc(
                    ErrorResult: errorMessage
                    , NewPassword: string.Empty
                    );
        }


        string newPassword = _logicRandomGenerator.GetNewStandardPassword();

        _uowUserRoleAdminManaged.EnableSupervisor(
            companyGroupId: _contextTenant.CompanyGroupId
            , userIdOperation: _contextUser.UserIdLoggedIn
            , userIdModified: userId
            , newPassword
            , operationDateTime: DateTime.Now
            );

        return
            new UserResetResultLgc(
                ErrorResult: string.Empty
                , NewPassword: newPassword
                );
    }


    //useless to test, it's practically a pass-through method
    [ExcludeFromCodeCoverage]
    //removes also roles from user
    public string DisableUser(long userId)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(DisableUser) }
               });

        _logger.LogDebug("CALL");



        string errorMessage = _logicSupervisorSaveChecks.ChecksEnableDisable(userId, isEnableCommand: false);

        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }

        _uowUserRoleAdminManaged.DisableSupervisor(
            _contextTenant.CompanyGroupId
            , userIdOperation: _contextUser.UserIdLoggedIn
            , userIdModified: userId
            , operationDateTime: DateTime.Now
            );

        return string.Empty;
    }




    public UserResetResultLgc ResetPasswordFromAdmin(long userId)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(ResetPasswordFromAdmin) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.NegativeOrZero(userId, nameof(userId));

        if (!_contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
        {
            return
                new UserResetResultLgc(
                    ErrorResult: SupervisorSaveLgcUtility.ErrorResetPasswordFromAdminPermission,
                    NewPassword: string.Empty
                    );
        }

        UserDataQr credentialUser =
            _queryUserDataRead.GetByUserId(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userId: userId
                );
        if (credentialUser is null)
        {
            return
                new UserResetResultLgc(
                    ErrorResult: SupervisorSaveLgcUtility.ErrorResetPasswordFromAdminUserNotFound,
                    NewPassword: string.Empty
                    );
        }

        string newPassword = _logicRandomGenerator.GetNewStandardPassword();

        _uowUserRoleAdminManaged.ChangePasswordFromAdminAndCommit(
            userIdOperation: _contextUser.UserIdLoggedIn
            , userIdModified: userId
            , newPassword: newPassword
            , newPasswordExpiration: DateTime.Now.AddDays(-1)//set new password with expired date, must be changed on first access
            , operationDateTime: DateTime.Now
            );

        return
            new UserResetResultLgc(
                ErrorResult: string.Empty
                , NewPassword: newPassword
                );
    }



    public UserEditFromAdminResultLgc ModifyUserDataFromAdmin(UserEditFromAdminLgc input)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(ModifyUserDataFromAdmin) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.Null(input, nameof(UserEditFromAdminLgc));

        Guard.Against.NegativeOrZero(input.UserId, nameof(input.UserId));

        //check first userId so we can return it
        UserDataQr credentialUser =
            _queryUserDataRead.GetByUserId(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userId: input.UserId
                );
        if (credentialUser is null)
        {
            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: SupervisorSaveLgcUtility.ErrorModifyUserDataFromAdminUserNotExists
                    , UserId: long.MinValue
                    , EmailChanged: false
                    , UserDataBefore: null
                    , UserDataUpdated: null
                    );
        }

        string errorResult = ChecksModifyUserData(input);

        if (errorResult.StringHasValue())
        {
            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: errorResult
                    , UserId: input.UserId
                    , EmailChanged: default
                    , UserDataBefore: null
                    , UserDataUpdated: null
                    );
        }

        UserDataQr userDataBeforeUpdate =
            _queryUserDataRead.GetActiveManagedByUserIdForModify(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userId: input.UserId
                );

        if (userDataBeforeUpdate is null || !userDataBeforeUpdate.IsActive)
        {
            _logger.LogError("user found for id {UserId} is not active", input.UserId);

            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: SupervisorSaveLgcUtility.ErrorModifyUserDataFromAdminUserNotActive
                    , UserId: input.UserId
                    , EmailChanged: default
                    , UserDataBefore: null
                    , UserDataUpdated: null
                    );
        }

        RolesSelectionResultLgc rolesSelectionResult =
            _logicRoleAdminManaged.ValidateAndParseSubmittedRolesSelectionForModifyUser(
                input.UserId
                , input.ExclusiveRole
                , input.SupervisorRoles
                );

        if (rolesSelectionResult is null)
        {
            throw new PmLogicException($"{nameof(RolesSelectionResultLgc)} is null");
        }
        if (rolesSelectionResult.ErrorMessage.StringHasValue())
        {
            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: rolesSelectionResult.ErrorMessage
                    , UserId: input.UserId
                    , EmailChanged: default
                    , UserDataBefore: null
                    , UserDataUpdated: null
                    );
        }




        _uowUserRoleAdminManaged.ModifyPersonalDataAndCommit(
            companyGroupId: _contextTenant.CompanyGroupId
            , userIdOperation: _contextUser.UserIdLoggedIn
            , userIdModified: input.UserId
            , name: input.Name
            , surname: input.Surname
            , email: input.Email
            , roles: rolesSelectionResult.SelectedRolesToSave
            , operationDateTime: DateTime.Now
            );


        if (input.Email.EqualsInvariant(userDataBeforeUpdate.Email))
        {
            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: string.Empty
                    , UserId: input.UserId
                    , EmailChanged: false
                    , UserDataBefore: null //data not needed in this case
                    , UserDataUpdated: null
                    );
        }
        else
        {
            UserDataQr userDataCurrent = input.MapFromLogicToDataCustomWithNullCheck();

            return
                new UserEditFromAdminResultLgc(
                    ErrorMessage: string.Empty
                    , UserId: input.UserId
                    , EmailChanged: true
                    , UserDataBefore: userDataBeforeUpdate.MapFromDataToLogicCustomWithNullCheck()
                    , UserDataUpdated: userDataCurrent.MapFromDataToLogicCustomWithNullCheck()
                    );
        }
    }

    private string ChecksModifyUserData(UserEditFromAdminLgc input)
    {
        if (!_contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
        {
            return SupervisorSaveLgcUtility.ErrorModifyUserDataFromAdminUnauthorized;
        }

        if (input.Name.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorNameEmpty;
        }

        if (input.Surname.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorSurnameEmpty;
        }

        if (input.Email.Empty())
        {
            return SupervisorSaveLgcUtility.ErrorEmailEmpty;
        }

        Match match = new Regex(RegexPatterns.Email).Match(input.Email);
        if (match == null || !match.Success)
        {
            return SupervisorSaveLgcUtility.ErrorEmailInvalidFormat;
        }

        _logicSupervisorSaveChecks.CheckIfEmailExistsForOtherActiveUsers(
            input.Email
            , input.UserId
            );

        return string.Empty;
    }
}