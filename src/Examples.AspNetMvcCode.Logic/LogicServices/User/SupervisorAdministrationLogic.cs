namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public class SupervisorAdministrationLogic : ISupervisorAdministrationLogic
{
    private readonly ILogger<SupervisorAdministrationLogic> _logger;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly IUserDataReadQueries _queryUserDataRead;

    private readonly IRoleAdminManagedLogic _logicRoleAdminManaged;

    public SupervisorAdministrationLogic(
        ILogger<SupervisorAdministrationLogic> logger
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IUserDataReadQueries queryUserDataRead
        , IRoleAdminManagedLogic logicRoleAdminManaged
        )
    {
        _logger = logger;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _queryUserDataRead = queryUserDataRead;
        _logicRoleAdminManaged = logicRoleAdminManaged;
    }






    public SupervisorSearchResultLgc ConditionalSearch(
        bool performSearch
        , string filterSurname
        , string filterName
        , string filterEmail
        , IEnumerable<long> filterRoles
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ConditionalSearch) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        IEnumerable<OptionLgc> rolesForFilterWithSelected =
            _logicRoleAdminManaged.GetRolesFilterForSearch(
                selectedFilterRoles: performSearch ? filterRoles : Enumerable.Empty<long>()
                );

        bool showResults = false;
        SupervisorSimpleSearchResult supervisorSimpleSearchResult = null;
        if (performSearch)
        {
            supervisorSimpleSearchResult =
                DoSearch(
                    filterSurname: filterSurname
                    , filterName: filterName
                    , filterEmail: filterEmail
                    , filterRolesFound: filterRoles
                    );

            showResults = supervisorSimpleSearchResult.FoundResults.HasValues();
        }


        return
            new SupervisorSearchResultLgc(
                OrphanedRolesDescriptions: _logicRoleAdminManaged.GetRolesOrphansOfUsers()
                , AvailableRolesWithSelected: rolesForFilterWithSelected
                , showResults

                , FoundResults:
                    supervisorSimpleSearchResult is null
                    ? Enumerable.Empty<UserFoundLgc>()
                    : supervisorSimpleSearchResult.FoundResults

                , supervisorSimpleSearchResult is null
                    ? new List<MessageField>()
                    : supervisorSimpleSearchResult.FieldToWarnList.ToList()

                , Success:
                    supervisorSimpleSearchResult is null || supervisorSimpleSearchResult.Success

                , ValuesAllowed: string.Empty

                , WarningType:
                     supervisorSimpleSearchResult is null
                    ? WarningType.None //if not performing search, no waning needed
                    : supervisorSimpleSearchResult.WarningType
                );
    }


    private record SupervisorSimpleSearchResult(
        IEnumerable<MessageField> FieldToWarnList
        , WarningType WarningType
        , IEnumerable<UserFoundLgc> FoundResults
        , bool Success
        );
    private SupervisorSimpleSearchResult DoSearch(
        string filterSurname
        , string filterName
        , string filterEmail
        , IEnumerable<long> filterRolesFound
        )
    {
        SupervisorSimpleSearchResult filtersValidationsResult =
            ValidateUserSearchFilters(filterSurname, filterName, filterEmail);

        if (filtersValidationsResult is not null)
        {
            return filtersValidationsResult;
        }

        return
            new SupervisorSimpleSearchResult(
                FieldToWarnList: Enumerable.Empty<MessageField>()
                , WarningType: WarningType.None
                , FoundResults:
                        _queryUserDataRead.SearchUsers(
                            companyGroupId: _contextTenant.CompanyGroupId
                            , referenceDateForActive: DateTime.Now
                            , referenceDatePasswordExpiration: DateTime.Now
                            , referenceDateForStrongAuthCode: DateTime.Now
                            , isAdminApplication: _contextUser.IsAdminApplication()
                            , isAdminTenant: _contextUser.IsAlsoAdminTenant
                            , surnameFilter: filterSurname
                            , nameFilter: filterName
                            , emailFilter: filterEmail
                            , rolesFilter: filterRolesFound
                            ).MapIEnumerableFromDataToLogic()
                , Success: true
            );
    }

    private static SupervisorSimpleSearchResult ValidateUserSearchFilters(
        string filterSurname
        , string filterName
        , string filterEmail
        )
    {
        if (filterSurname.StringHasValue()
            && (filterSurname.Length < AppConstants.PersonalNameSurnameMinimumCharacters
                || filterSurname.Length > AppConstants.PersonalNameSurnameMaximumCharactersSearch))
        {
            return
                new SupervisorSimpleSearchResult(
                    FieldToWarnList:
                        new List<MessageField>
                        {
                            MessageField.UserSurname,
                        }
                    , WarningType: WarningType.InvalidOrEmpty
                    , FoundResults: Enumerable.Empty<UserFoundLgc>()
                    , Success: false
                );
        }


        if (filterName.StringHasValue()
            && (filterName.Length < AppConstants.PersonalNameSurnameMinimumCharacters
                || filterName.Length > AppConstants.PersonalNameSurnameMaximumCharactersSearch))
        {
            return
                new SupervisorSimpleSearchResult(
                    FieldToWarnList:
                        new List<MessageField>
                        {
                            MessageField.UserName,
                        }
                    , WarningType: WarningType.InvalidOrEmpty
                    , FoundResults: Enumerable.Empty<UserFoundLgc>()
                    , Success: false
                );
        }


        if (filterEmail.StringHasValue())
        {
            Regex regex = new(AppRegexPatterns.EmailSearch);
            Match match = regex.Match(filterEmail);
            if (match == null || !match.Success)
            {
                return
                   new SupervisorSimpleSearchResult(
                       FieldToWarnList:
                           new List<MessageField>
                           {
                                MessageField.Email,
                           }
                       , WarningType: WarningType.InvalidOrEmpty
                       , FoundResults: Enumerable.Empty<UserFoundLgc>()
                       , Success: false
                   );
            }
        }

        return null;
    }




    //useless to test, it's practically a pass-through method
    [ExcludeFromCodeCoverage]
    public RolesSelectionLgc GetAvailableRolesForNewUser(
        string selectedExclusiveRole
        , IEnumerable<long> selectedSupervisorRoles
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetAvailableRolesForNewUser) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        return
            _logicRoleAdminManaged.CheckAndSetRolesSelectionForNewUser(
                selectedExclusiveRole
                , selectedSupervisorRoles
                );
    }


    //useless to test, it's practically a pass-through method
    [ExcludeFromCodeCoverage]
    public UserSupervisorDataLgc GetUserSupervisorData(long userId)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetUserSupervisorData) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        //if user does not exist exception will be thrown, so we will not handle fail with a user message
        UserDataQr userData =
            _queryUserDataRead.GetByUserIdForModify(
                companyGroupId: _contextTenant.CompanyGroupId
                , isAdminApplication: _contextUser.IsAdminApplication()
                , referenceDateForActive: DateTime.Now
                , referenceDatePasswordExpiration: DateTime.Now
                , referenceDateForStrongAuthCode: DateTime.Now
                , userId: userId
                );

        RolesSelectionLgc rolesSelection = _logicRoleAdminManaged.CheckAndSetRolesSelectionForModifyUser(userId);

        return
             new UserSupervisorDataLgc(
                 UserId: userId
                 , UserMustBeIncludedInManagedBeforeEdits: !userData.IsTenantManaged
                 , IsActive: userData.IsActive
                 , Login: userData.Login
                 , Name: userData.Name
                 , Surname: userData.Surname
                 , Email: userData.Email
                 , RolesSelection: rolesSelection
                 );
    }
}