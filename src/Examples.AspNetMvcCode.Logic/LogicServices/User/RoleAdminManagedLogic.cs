namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public class RoleAdminManagedLogic : IRoleAdminManagedLogic
{
    private readonly ILogger<RoleAdminManagedLogic> _logger;

    private readonly ContextUser _contextUser;

    private readonly IRoleReadQueries _queryRoleRead;

    public RoleAdminManagedLogic(
        ILogger<RoleAdminManagedLogic> logger
        , ContextUser contextUser
        , IRoleReadQueries queryRoleRead
        )
    {
        _logger = logger;
        _contextUser = contextUser;
        _queryRoleRead = queryRoleRead;
    }



    //useless to test, it's just a pass-through method
    [ExcludeFromCodeCoverage]
    public IEnumerable<HtmlString> GetRolesOrphansOfUsers()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetRolesOrphansOfUsers) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        IEnumerable<HtmlString> rolesOphanedDescriptions =
            _queryRoleRead.GetRolesOrphansOfUsers(_contextUser.IsAdminApplication());

        return rolesOphanedDescriptions;
    }


    //search role filter is built with all available roles as a list, no matter if some of them are the exclusive type
    public IEnumerable<OptionLgc> GetRolesFilterForSearch(IEnumerable<long> selectedFilterRoles)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetRolesFilterForSearch) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.Null(selectedFilterRoles, nameof(selectedFilterRoles));

        _contextUser.GuardAgainstNotAdmin();



        IEnumerable<OptionQr> allRolesWithDescriptionFound = _queryRoleRead.GetAllRolesWithDescriptions();

        IEnumerable<long> selectedRolesNotAvailableFound =
            selectedFilterRoles.Except(allRolesWithDescriptionFound.Select(sr => long.Parse(sr.Value)));
        if (selectedRolesNotAvailableFound.HasValues())
        {
            throw new PmLogicException(
                @$"one or more of submitted roles are not included in configured supervisor list 
                '{JsonSerializer.Serialize(selectedFilterRoles.Except(allRolesWithDescriptionFound.Select(sr => long.Parse(sr.Value))))}' "
                );
        }

        List<OptionLgc> rolesWithDescriptionList = new();
        foreach (OptionQr roleWithDescription in allRolesWithDescriptionFound)
        {
            long role = long.Parse(roleWithDescription.Value);

            if (!_contextUser.IsAdminApplication()
                && RoleQrUtility.RolesNotAvailableForAdminTenant.Contains(role))
            {
                continue; //exclude this role if context user is not admin application
            }

            OptionLgc optionLgc = roleWithDescription.MapFromDataToLogic();

            if (selectedFilterRoles.HasValues())
            {
                if (selectedFilterRoles.Contains(role))
                {
                    optionLgc.Selected = true;
                }
            }

            rolesWithDescriptionList.Add(
                optionLgc
                );
        }

        return rolesWithDescriptionList;
    }


    public RolesSelectionLgc CheckAndSetRolesSelectionForNewUser(
        string selectedExclusiveRole
        , IEnumerable<long> selectedSupervisorRoles
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckAndSetRolesSelectionForNewUser) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        //new user creation will restore selected roles in previous attempt
        IEnumerable<OptionQr> allRolesWithDescriptionFound = _queryRoleRead.GetAllRolesWithDescriptions();

        IEnumerable<OptionLgc> exclusiveRolesFound =
            BuildExclusiveRoleSelection(selectedExclusiveRole, allRolesWithDescriptionFound);

        IEnumerable<OptionLgc> supervisorRolesFound =
            BuildSupervisorRoles(
                allRolesWithDescriptionFound
                , selectedSupervisorRoles
                );

        return
            new RolesSelectionLgc(
                ExclusiveRolesFound: exclusiveRolesFound
                , SupervisorRolesFound: supervisorRolesFound
                );
    }



    public RolesSelectionLgc CheckAndSetRolesSelectionForModifyUser(long userId)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CheckAndSetRolesSelectionForModifyUser) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        //for this method we will not restore the changed roles in case of fail
        //we will simply reload the actual saved roles for user saved in database
        IEnumerable<OptionQr> allRolesWithDescriptionFound = _queryRoleRead.GetAllRolesWithDescriptions();

        RolesAssignedToUserQr rolesAssignedToUser = CheckPermissionAndGetData(userId);

        IEnumerable<OptionLgc> exclusiveRolesFound =
            BuildExclusiveRoleSelection(
                rolesAssignedToUser.ExclusiveRoleType
                , allRolesWithDescriptionFound
                );

        List<long> roles = rolesAssignedToUser.AssignedSupervisorRolesList.ToList();
        if (rolesAssignedToUser.IsAlsoAdminTenant)
        {
            roles.Add(RoleQrUtility.AdminTenantRole);
        }

        IEnumerable<OptionLgc> supervisorRolesFound =
            BuildSupervisorRoles(
                allRolesWithDescriptionFound
                , roles
                );

        return
            new RolesSelectionLgc(
                ExclusiveRolesFound: exclusiveRolesFound
                , SupervisorRolesFound: supervisorRolesFound
                );
    }



    public RolesSelectionResultLgc ValidateAndParseSubmittedRolesSelectionForNewUser(
        string selectedExclusiveRole
        , IEnumerable<long> selectedSupervisorRoles
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidateAndParseSubmittedRolesSelectionForNewUser) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        return
            ValidateAndParseSubmittedRolesSelection(
                selectedExclusiveRole
                , selectedSupervisorRoles
                );
    }


    public RolesSelectionResultLgc ValidateAndParseSubmittedRolesSelectionForModifyUser(
        long userId
        , string selectedExclusiveRole
        , IEnumerable<long> selectedSupervisorRoles
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidateAndParseSubmittedRolesSelectionForModifyUser) }
                });

        _logger.LogDebug("CALL");



        _contextUser.GuardAgainstNotAdmin();

        RolesAssignedToUserQr rolesAssignedToUser = CheckPermissionAndGetData(userId);

        return
            ValidateAndParseSubmittedRolesSelection(
                selectedExclusiveRole
                , selectedSupervisorRoles
                );
    }



    private RolesSelectionResultLgc ValidateAndParseSubmittedRolesSelection(
        string selectedExclusiveRole
        , IEnumerable<long> selectedSupervisorRoles
        )
    {
        if (selectedExclusiveRole.Empty())
        {
            return
                new RolesSelectionResultLgc(
                    ErrorMessage: RoleAdminManagedLgcUtility.ErrorNewUserSubmitExclusiveRoleEmpty,
                    SelectedRolesToSave: Enumerable.Empty<long>()
                    );
        }

        IEnumerable<OptionQr> allRolesWithDescriptionFound = _queryRoleRead.GetAllRolesWithDescriptions();



        IEnumerable<OptionLgc> exclusiveRolesFound =
            BuildExclusiveRoleSelection(selectedExclusiveRole, allRolesWithDescriptionFound);

        IEnumerable<OptionLgc> exclusiveRolesSelectedFound = exclusiveRolesFound.Where(role => role.Selected);

        if (exclusiveRolesSelectedFound.IsNullOrEmpty())
        {
            throw new PmLogicException($"{nameof(selectedExclusiveRole)} is invalid; value '{selectedExclusiveRole}'; should be one on {nameof(ExclusiveRole)} enum");
        }

        if (exclusiveRolesSelectedFound.Count() > 1)
        {
            throw new PmLogicException($"{nameof(selectedExclusiveRole)} is duplicated. review code asap. value {selectedExclusiveRole}");
        }

        ExclusiveRole selectedExclusiveRoleEnum = exclusiveRolesSelectedFound.Single().Value.ToEnum<ExclusiveRole>();

        if (selectedExclusiveRoleEnum != ExclusiveRole.None)
        {
            //in this case ignore submitted roles, they are irrelevant when user has chosen an exclusive role
            long exclusiveRoleCodeSelected =
                selectedExclusiveRoleEnum switch
                {
                    ExclusiveRole.IsBasicUserOnly => RoleQrUtility.BasicRoleId,
                    ExclusiveRole.AdminApplication => RoleQrUtility.AdminApplicationRoleCode,
                    ExclusiveRole.Scheduler => RoleQrUtility.SchedulerRole,
                    _ => throw new PmLogicException($"unhandled type of {nameof(ExclusiveRole)}, value {selectedExclusiveRoleEnum}"),
                };

            return
                new RolesSelectionResultLgc(
                    ErrorMessage: string.Empty
                    , SelectedRolesToSave: new List<long> { exclusiveRoleCodeSelected }
                    );
        }


        //no exclusive role, validate supervisor roles

        if (selectedSupervisorRoles.IsNullOrEmpty())
        {
            return
                new RolesSelectionResultLgc(
                    ErrorMessage: SupervisorSaveLgcUtility.ErrorRolesEmpty
                    , SelectedRolesToSave: Enumerable.Empty<long>()
                    );
        }

        IEnumerable<OptionLgc> supervisorRolesFound =
            BuildSupervisorRoles(
                allRolesWithDescriptionFound
                , selectedSupervisorRoles
                );

        IEnumerable<long> submittedRolesNotConfiguredInDb =
            selectedSupervisorRoles.Except(supervisorRolesFound.Select(role => long.Parse(role.Value)));
        if (submittedRolesNotConfiguredInDb.HasValues())
        {
            _logger.LogError("supervisor roles '{InvalidRoles}' are not configured in database ", submittedRolesNotConfiguredInDb);

            //we can't return a precise message, because only user codes were submitted
            //and we just found out they don't exist in db. It's just invalid.
            return
               new RolesSelectionResultLgc(
                   ErrorMessage: SupervisorSaveLgcUtility.ErrorRolesInvalid
                   , SelectedRolesToSave: Enumerable.Empty<long>()
                   );
        }


        IEnumerable<OptionLgc> selectedSupervisorRolesFound = supervisorRolesFound.Where(role => role.Selected);

        if (selectedSupervisorRolesFound.IsNullOrEmpty())
        {
            return
               new RolesSelectionResultLgc(
                   ErrorMessage: SupervisorSaveLgcUtility.ErrorRolesInvalid
                   , SelectedRolesToSave: Enumerable.Empty<long>()
                   );
        }

        return
            new RolesSelectionResultLgc(
                ErrorMessage: string.Empty
                , SelectedRolesToSave: selectedSupervisorRolesFound.Select(role => long.Parse(role.Value))
                );
    }

    private RolesAssignedToUserQr CheckPermissionAndGetData(long userId)
    {
        RolesAssignedToUserQr rolesAssignedToUser = _queryRoleRead.GetInfo(userId);

        if (rolesAssignedToUser == null)
        {
            throw new PmLogicException($"{nameof(RolesAssignedToUserQr)} is null");
        }

        //an admin tenant can't modify a admin application/scheduler user
        if ((rolesAssignedToUser.ExclusiveRoleType is ExclusiveRole.AdminApplication
            || rolesAssignedToUser.ExclusiveRoleType is ExclusiveRole.Scheduler)
            && !_contextUser.IsAdminApplication())
        {
            throw new PmLogicException($"context user {_contextUser.UserIdLoggedIn} has no permissions to modify a user with {nameof(rolesAssignedToUser.ExclusiveRoleType)} '{rolesAssignedToUser.ExclusiveRoleType}' ");
        }

        return rolesAssignedToUser;
    }

    /// <summary>
    /// use when we need to build the filter list without previous selection
    /// </summary>
    /// <returns></returns>
    private IEnumerable<OptionLgc> BuildExclusiveRoleSelection(
        string selectedExclusiveRole
        , IEnumerable<OptionQr> allRolesWithDescriptionFound
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(BuildExclusiveRoleSelection) }
                });

        ExclusiveRole? restoredSelectExclusiveRole = ConvertSelectedExclusiveRole(selectedExclusiveRole);
        //use this method to prevent exception. In case the value was not valid use the default enum value


        return
            BuildExclusiveRoleSelection(
                exclusiveRoleAssignedToUserToModify: restoredSelectExclusiveRole
                , allRolesWithDescriptionFound
                );
    }

    private ExclusiveRole? ConvertSelectedExclusiveRole(string selectedExclusiveRole)
    {
        ExclusiveRole? restoredSelectExclusiveRole = null;
        if (selectedExclusiveRole.StringHasValue())
        {
            try
            {
                restoredSelectExclusiveRole = selectedExclusiveRole.ToEnum<ExclusiveRole>();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex
                    , "Failed Enum parse to {ExclusiveRole}. Provided value '{SelectedExclusiveRole}' "
                    , nameof(ExclusiveRole)
                    , selectedExclusiveRole
                    );
            }
        }

        return restoredSelectExclusiveRole;
    }

    private IEnumerable<OptionLgc> BuildExclusiveRoleSelection(
        ExclusiveRole? exclusiveRoleAssignedToUserToModify
        , IEnumerable<OptionQr> allRolesWithDescriptionFound
        )
    {
        bool exclusiveRoleUserToModifyProvided = exclusiveRoleAssignedToUserToModify is not null;

        List<OptionLgc> exclusiveRoleList = new();

        if (_contextUser.IsAdminApplication())
        {
            exclusiveRoleList.Add(
                new OptionLgc()
                {
                    Value = ExclusiveRole.AdminApplication.ToString(),
                    Description =
                        GetRoleDescription(
                            allRolesWithDescriptionFound
                            , RoleQrUtility.AdminApplicationRoleCode
                            , RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultAdminApplication
                            ),
                    Selected =
                        exclusiveRoleUserToModifyProvided && exclusiveRoleAssignedToUserToModify == ExclusiveRole.AdminApplication
                });

            exclusiveRoleList.Add(
                new OptionLgc()
                {
                    Value = ExclusiveRole.Scheduler.ToString(),
                    Description =
                        GetRoleDescription(
                            allRolesWithDescriptionFound
                            , RoleQrUtility.SchedulerRole
                            , RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultScheduler
                            ),
                    Selected =
                        exclusiveRoleUserToModifyProvided && exclusiveRoleAssignedToUserToModify == ExclusiveRole.Scheduler
                });
        }

        exclusiveRoleList.Add(
                new OptionLgc()
                {
                    Value = ExclusiveRole.IsBasicUserOnly.ToString(),
                    Description =
                        GetRoleDescription(
                            allRolesWithDescriptionFound
                            , RoleQrUtility.BasicRoleId
                            , RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultBasicRole
                            ),
                    Selected =
                        exclusiveRoleUserToModifyProvided && exclusiveRoleAssignedToUserToModify == ExclusiveRole.IsBasicUserOnly
                });


        //this must be last so roles selection menu is just after this choice
        exclusiveRoleList.Add(
            new OptionLgc()
            {
                Value = ExclusiveRole.None.ToString(),
                Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultNone,
                Selected = exclusiveRoleUserToModifyProvided && exclusiveRoleAssignedToUserToModify == ExclusiveRole.None
            });



        return exclusiveRoleList;
    }


    private static IHtmlContent GetRoleDescription(
        IEnumerable<OptionQr> allRolesWithDescriptionFound
        , long roleCode
        , IHtmlContent defaultDescription
        )
    {
        IEnumerable<OptionQr> roleFound =
            allRolesWithDescriptionFound.HasValues()
            ? allRolesWithDescriptionFound.Where(role => long.Parse(role.Value) == roleCode)
            : null;

        IHtmlContent roleDescription =
            roleFound.HasValues()
            ? roleFound.Single().Description
            : defaultDescription;

        return roleDescription;
    }


    private static IEnumerable<OptionLgc> BuildSupervisorRoles(
        IEnumerable<OptionQr> allRolesWithDescriptionFound
        , IEnumerable<long> supervisorRolesToSelect
        )
    {
        List<OptionLgc> outputSupervisorRolesList = new();

        foreach (OptionQr roleData in allRolesWithDescriptionFound)
        {
            long role = long.Parse(roleData.Value);

            if (RoleQrUtility.RolesExclusiveComplete.Contains(role))
            {
                continue;
            }

            OptionLgc roleLgc = roleData.MapFromDataToLogic();
            if (supervisorRolesToSelect.HasValues() && supervisorRolesToSelect.Contains(role))
            {
                roleLgc.Selected = true;
            }


            outputSupervisorRolesList.Add(roleLgc);
        }

        if (outputSupervisorRolesList.IsNullOrEmpty())
        {
            throw new PmLogicException("no specific supervisor roles configured, only exclusive roles");
        }

        return outputSupervisorRolesList;
    }
}