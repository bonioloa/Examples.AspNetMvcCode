using Examples.AspNetMvcCode.Logic.LogicServices.User;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal static class MoqIRoleAdminManagedLogic
{
    internal static IRoleAdminManagedLogic ConditionalSearchSetup(
        IEnumerable<OptionLgc> rolesForFilterWithSelected
        , IEnumerable<HtmlString> orphanedRolesDescriptions
        )
    {
        Mock<IRoleAdminManagedLogic> _logicRoleAdminManaged = new();

        //we don't need to restrict the input value here because the output is already fixed
        _logicRoleAdminManaged
            .Setup(svc =>
                    svc.GetRolesFilterForSearch(
                        It.IsAny<IEnumerable<long>>()
                        ))
            .Returns(rolesForFilterWithSelected);

        _logicRoleAdminManaged
            .Setup(svc => svc.GetRolesOrphansOfUsers())
            .Returns(orphanedRolesDescriptions);

        return _logicRoleAdminManaged.Object;
    }


    internal static IRoleAdminManagedLogic ModifyUserDataFromAdminSetup(RolesSelectionResultLgc rolesSelectionResultToReturn)
    {
        Mock<IRoleAdminManagedLogic> _logicRoleAdminManaged = new();

        _logicRoleAdminManaged
           .Setup(svc =>
                   svc.ValidateAndParseSubmittedRolesSelectionForModifyUser(
                       It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                       , It.IsAny<string>()
                       , It.IsAny<IEnumerable<long>>()
                       ))
           .Returns(rolesSelectionResultToReturn);

        return _logicRoleAdminManaged.Object;
    }

    internal static IRoleAdminManagedLogic SaveNewSupervisorSetup(
        RolesSelectionResultLgc rolesSelectionResultToReturn
        )
    {
        Mock<IRoleAdminManagedLogic> _logicRoleAdminManaged = new();

        _logicRoleAdminManaged
           .Setup(svc =>
                   svc.ValidateAndParseSubmittedRolesSelectionForNewUser(
                       It.IsAny<string>()
                       , It.IsAny<IEnumerable<long>>()
                       ))
           .Returns(rolesSelectionResultToReturn);

        return _logicRoleAdminManaged.Object;
    }
}