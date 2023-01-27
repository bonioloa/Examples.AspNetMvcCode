namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public class SupervisorSaveChecksLogic : ISupervisorSaveChecksLogic
{
    private readonly ILogger<SupervisorSaveChecksLogic> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly IUserDataReadQueries _queryUserDataRead;

    public SupervisorSaveChecksLogic(
        ILogger<SupervisorSaveChecksLogic> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IUserDataReadQueries queryUserDataRead
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _queryUserDataRead = queryUserDataRead;
    }


    public string CheckIfEmailExists(string email)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckIfEmailExists) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.NullOrWhiteSpace(email, nameof(email));

        UserDataQr credential =
            _queryUserDataRead.GetByEmail(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userEmail: email
                );
        if (credential is not null)
        {
            _logger.LogError("supervisor email '{Email}' already exists", email);

            if (_optProduct.Value.Product == Product.
                && _contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
            {
                //per gli admin tenant non possiamo dire che l'email esiste già
                //perché per i db a registrazione potrebbe trattarsi di un'utente che ha segnalato
                //e potrebbe essere una violazione della riservatezza
                return SupervisorSaveLgcUtility.ErrorEmailExistsGeneric;
            }
            else
            {
                return SupervisorSaveLgcUtility.GetErrorEmailExistsExplicitReason(email);
            }
        }
        return string.Empty;
    }



    //verify in all other users that email do not exist already
    public string CheckIfEmailExistsForOtherActiveUsers(
        string email
        , long userToExclude
        )
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckIfEmailExistsForOtherActiveUsers) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.NullOrWhiteSpace(email, nameof(email));
        Guard.Against.NegativeOrZero(userToExclude, nameof(userToExclude));

        IEnumerable<UserDataQr> activeUsersWithEmailFound =
            _queryUserDataRead.GetActiveUsersWithEmail(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , isAdminApplication: _contextUser.IsAdminApplication()
                , isAdminTenant: _contextUser.IsAlsoAdminTenant
                );

        if (activeUsersWithEmailFound.IsNullOrEmpty())
        {
            throw new PmLogicException($"all users and current '{userToExclude}' are without emails, this is a configuration error");
        }

        IEnumerable<UserDataQr> otherActiveUsersWithEmailFound =
            activeUsersWithEmailFound.Where(user => user.UserId != userToExclude);

        if (otherActiveUsersWithEmailFound.IsNullOrEmpty())
        {
            //no other user with email found. Acceptable
            return string.Empty;
        }

        IEnumerable<UserDataQr> otherActiveUsersWithSameEmailFound =
            otherActiveUsersWithEmailFound.Where(user => user.Email.EqualsInvariant(email));

        if (otherActiveUsersWithSameEmailFound.HasValues())
        {
            _logger.LogError(
                "supervisor email '{Email}' already exists for other user/s '{UserIdFound}'"
                , email
                , JsonSerializer.Serialize(otherActiveUsersWithSameEmailFound.Select(user => user.UserId))
                );

            if (_optProduct.Value.Product == Product.
                && _contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
            {
                //per gli admin tenant non possiamo dire che l'email esiste già
                //perché per i db a registrazione potrebbe trattarsi di un'utente che ha segnalato
                //e potrebbe essere una violazione della riservatezza
                return SupervisorSaveLgcUtility.ErrorEmailExistsGeneric;
            }
            else
            {
                return SupervisorSaveLgcUtility.GetErrorEmailExistsExplicitReason(email);
            }
        }

        return string.Empty;
    }



    public string CheckIfLoginExists(string login)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckIfLoginExists) }
               });
        _logger.LogDebug("CALL");



        Guard.Against.NullOrWhiteSpace(login, nameof(login));

        UserDataQr userData =
            _queryUserDataRead.GetByUserLogin(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userLogin: login
                );
        if (userData is not null)
        {
            _logger.LogError("supervisor login '{Login}' already exists", login);

            if (_optProduct.Value.Product == Product.
                && _contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
            {
                //per gli admin tenant non possiamo dire che l'utenza esiste già
                //perché per i db a registrazione potrebbe trattarsi di un'utente che ha segnalato
                //e potrebbe essere una violazione della riservatezza
                return SupervisorSaveLgcUtility.ErrorSaveUserInfoGeneric;
            }
            else
            {
                return SupervisorSaveLgcUtility.GetErrorLoginExistsExplicitReason(login);
            }
        }
        return string.Empty;
    }


    //verify in all other users that email do not exist already
    public string CheckIfLoginExistsForOtherActiveUsers(
        string login
        , long userToExclude
        )
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckIfLoginExistsForOtherActiveUsers) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.NullOrWhiteSpace(login, nameof(login));
        Guard.Against.NegativeOrZero(userToExclude, nameof(userToExclude));

        IEnumerable<UserDataQr> activeUsers =
            _queryUserDataRead.GetActiveUsers(
                companyGroupId: _contextTenant.CompanyGroupId
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , isAdminApplication: _contextUser.IsAdminApplication()
                , isAdminTenant: _contextUser.IsAlsoAdminTenant
                );

        if (activeUsers.IsNullOrEmpty())
        {
            throw new PmLogicException($"all users and current '{userToExclude}' are without emails, this is a configuration error");
        }

        IEnumerable<UserDataQr> otherActiveUsersFound =
            activeUsers.Where(user => user.UserId != userToExclude);

        if (otherActiveUsersFound.IsNullOrEmpty())
        {
            //no other users found except the current in editing. Acceptable
            return string.Empty;
        }

        IEnumerable<UserDataQr> otherActiveUsersWithSameLoginFound =
            otherActiveUsersFound.Where(user => user.Login.EqualsInvariant(login));

        if (otherActiveUsersWithSameLoginFound.HasValues())
        {
            _logger.LogError("supervisor login '{Login}' already exists", login);

            if (_optProduct.Value.Product == Product.
                && _contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
            {
                //per gli admin tenant del non possiamo dire che l'utenza esiste già
                //perché per i db a registrazione potrebbe trattarsi di un'utente che ha segnalato
                //e potrebbe essere una violazione della riservatezza
                return SupervisorSaveLgcUtility.ErrorSaveUserInfoGeneric;
            }
            else
            {
                return SupervisorSaveLgcUtility.GetErrorLoginExistsExplicitReason(login);
            }
        }

        return string.Empty;
    }



    public string ChecksEnableDisable(long userId, bool isEnableCommand)
    {
        using IDisposable logScopeCurrentMethod =
           _logger.BeginScope(
               new Dictionary<string, object>
               {
                    { AppLogPropertiesKeys.MethodName, nameof(ChecksEnableDisable) }
               });

        _logger.LogDebug("CALL");



        Guard.Against.NegativeOrZero(userId, nameof(userId));

        if (!_contextUser.IsAlsoAdminTenant && !_contextUser.IsAdminApplication())
        {
            return SupervisorSaveLgcUtility.GetErrorEnableDisableUserPermission(isEnableCommand);
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
            return SupervisorSaveLgcUtility.GetErrorEnableDisableUserUserNotExist(isEnableCommand);
        }

        if (isEnableCommand && credentialUser.IsActive)
        {
            return SupervisorSaveLgcUtility.ErrorEnableDisableUserUserIsActive;
        }

        if (!isEnableCommand && !credentialUser.IsActive)
        {
            return SupervisorSaveLgcUtility.ErrorEnableDisableUserUserIsDisabled;
        }



        string errorMessage = CheckIfLoginExistsForOtherActiveUsers(credentialUser.Login, userId);
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }

        errorMessage = CheckIfEmailExistsForOtherActiveUsers(credentialUser.Email, userId);
        if (errorMessage.StringHasValue())
        {
            return errorMessage;
        }

        return string.Empty;
    }
}