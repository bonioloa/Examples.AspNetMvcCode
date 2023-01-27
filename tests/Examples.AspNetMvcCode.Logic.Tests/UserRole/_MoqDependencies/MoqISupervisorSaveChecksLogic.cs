using Examples.AspNetMvcCode.Logic.LogicServices.User;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal static class MoqISupervisorSaveChecksLogic
{
    internal static ISupervisorSaveChecksLogic IncludeUserInManagedSetup(
        string returnedLoginErrorMessage
        , string returnedEmailErrorMessage
        )
    {
        Mock<ISupervisorSaveChecksLogic> _logicSupervisorSaveChecks = new();

        //we don't need to restrict the input value here because the output is already fixed
        _logicSupervisorSaveChecks
            .Setup(svc =>
                    svc.CheckIfLoginExistsForOtherActiveUsers(
                        It.IsAny<string>()
                        , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                        ))
            .Returns(returnedLoginErrorMessage);

        _logicSupervisorSaveChecks
            .Setup(svc =>
                    svc.CheckIfEmailExistsForOtherActiveUsers(
                        It.IsAny<string>()
                        , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                        ))
            .Returns(returnedEmailErrorMessage);

        return _logicSupervisorSaveChecks.Object;
    }


    internal static ISupervisorSaveChecksLogic ModifyUserDataFromAdminSetup(
        string returnedEmailErrorMessage
        )
    {
        Mock<ISupervisorSaveChecksLogic> _logicSupervisorSaveChecks = new();

        _logicSupervisorSaveChecks
            .Setup(svc =>
                    svc.CheckIfEmailExistsForOtherActiveUsers(
                        It.IsAny<string>()
                        , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                        ))
            .Returns(returnedEmailErrorMessage);

        return _logicSupervisorSaveChecks.Object;
    }


    internal static ISupervisorSaveChecksLogic SaveNewSupervisorSetup(
        string returnedLoginErrorMessage
        , string returnedEmailErrorMessage
        )
    {
        Mock<ISupervisorSaveChecksLogic> _logicSupervisorSaveChecks = new();

        //we don't need to restrict the input value here because the output is already fixed
        _logicSupervisorSaveChecks
            .Setup(svc =>
                    svc.CheckIfLoginExists(
                        It.IsAny<string>()
                        ))
            .Returns(returnedLoginErrorMessage);

        _logicSupervisorSaveChecks
            .Setup(svc =>
                    svc.CheckIfEmailExists(
                        It.IsAny<string>()
                        ))
            .Returns(returnedEmailErrorMessage);

        return _logicSupervisorSaveChecks.Object;
    }
}