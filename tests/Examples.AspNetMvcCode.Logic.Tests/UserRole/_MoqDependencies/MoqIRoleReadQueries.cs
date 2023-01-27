namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal static class MoqIRoleReadQueries
{
    internal static IRoleReadQueries GetRolesFilterForSearchSetup(IEnumerable<OptionQr> rolesWithDescriptionsToReturn)
    {
        return StandardGetAllRolesWithDescriptionsSetup(rolesWithDescriptionsToReturn).Object;
    }

    internal static IRoleReadQueries CheckAndSetRolesSelectionForNewUserSetup(IEnumerable<OptionQr> rolesWithDescriptionsToReturn)
    {
        return StandardGetAllRolesWithDescriptionsSetup(rolesWithDescriptionsToReturn).Object;
    }

    internal static IRoleReadQueries CheckAndSetRolesSelectionForModifyUserSetup(
        IEnumerable<OptionQr> rolesWithDescriptionsToReturn
        , RolesAssignedToUserQr rolesAssignedToUserToReturn
        )
    {
        Mock<IRoleReadQueries> _queryRoleRead = StandardGetAllRolesWithDescriptionsSetup(rolesWithDescriptionsToReturn);

        _queryRoleRead.Setup(svc => svc.GetInfo(It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)))
                      .Returns(rolesAssignedToUserToReturn);

        return _queryRoleRead.Object;
    }

    internal static IRoleReadQueries ValidateAndParseSubmittedRolesSelectionForNewUserSetup(IEnumerable<OptionQr> rolesWithDescriptionsToReturn)
    {
        return StandardGetAllRolesWithDescriptionsSetup(rolesWithDescriptionsToReturn).Object;
    }

    internal static IRoleReadQueries ValidateAndParseSubmittedRolesSelectionForModifyUserSetup(
         IEnumerable<OptionQr> rolesWithDescriptionsToReturn
        , RolesAssignedToUserQr rolesAssignedToUserToReturn
        )
    {
        Mock<IRoleReadQueries> _queryRoleRead = StandardGetAllRolesWithDescriptionsSetup(rolesWithDescriptionsToReturn);

        _queryRoleRead.Setup(svc => svc.GetInfo(It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)))
                      .Returns(rolesAssignedToUserToReturn);

        return _queryRoleRead.Object;
    }




    private static Mock<IRoleReadQueries> StandardGetAllRolesWithDescriptionsSetup(IEnumerable<OptionQr> rolesWithDescriptionsToReturn)
    {
        Mock<IRoleReadQueries> _queryRoleRead = new();

        _queryRoleRead.Setup(svc => svc.GetAllRolesWithDescriptions())
                      .Returns(rolesWithDescriptionsToReturn);

        return _queryRoleRead;
    }
}
