namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal static class MoqIUserRoleAdminManagedUow
{
    internal static IUserRoleAdminManagedUow CreateNewSupervisorSetup(
        long newUserId
        )
    {
        Mock<IUserRoleAdminManagedUow> _uowUserRoleAdminManaged = new();


        _uowUserRoleAdminManaged
            .Setup(
                svc => svc.CreateNewSupervisor(
                            It.IsAny<UserNewSupervisorQr>()
                            , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                            , It.IsAny<DateTime>()
                            )
            )
            .Returns(newUserId);

        return _uowUserRoleAdminManaged.Object;
    }

    internal static IUserRoleAdminManagedUow ChangeUserToTenantManagedSetup()
    {
        Mock<IUserRoleAdminManagedUow> _uowUserRoleAdminManaged = new();

        _uowUserRoleAdminManaged
            .Setup(svc =>
                svc.ChangeUserToTenantManaged(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<DateTime>()
                    ));

        return _uowUserRoleAdminManaged.Object;
    }

    internal static IUserRoleAdminManagedUow ResetPasswordFromAdminSetup()
    {
        Mock<IUserRoleAdminManagedUow> _uowUserRoleAdminManaged = new();

        _uowUserRoleAdminManaged
            .Setup(svc =>
                svc.ChangePasswordFromAdminAndCommit(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<string>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    ));

        return _uowUserRoleAdminManaged.Object;
    }

    internal static IUserRoleAdminManagedUow ModifyUserDataFromAdminSetup()
    {
        Mock<IUserRoleAdminManagedUow> _uowUserRoleAdminManaged = new();

        _uowUserRoleAdminManaged
            .Setup(svc =>
                svc.ModifyPersonalDataAndCommit(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<string>()
                    , It.IsAny<string>()
                    , It.IsAny<string>()
                    , It.IsAny<IEnumerable<long>>()
                    , It.IsAny<DateTime>()
                    ));

        return _uowUserRoleAdminManaged.Object;
    }
}
