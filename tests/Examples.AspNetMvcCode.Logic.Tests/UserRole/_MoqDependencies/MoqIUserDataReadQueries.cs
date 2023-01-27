namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal static class MoqIUserDataReadQueries
{
    internal static IUserDataReadQueries UserAdminManagedReadSetup(IEnumerable<UserDataQr> usersFound)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        //we don't need to restrict the input value here because the output is already fixed
        _queryUserDataRead
            .Setup(svc =>
                    svc.SearchUsers(
                        It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                        , It.IsAny<DateTime>()
                        , It.IsAny<DateTime>()
                        , It.IsAny<DateTime>()
                        , It.IsAny<bool>()
                        , It.IsAny<bool>()
                        , It.IsAny<string>()
                        , It.IsAny<string>()
                        , It.IsAny<string>()
                        , It.IsAny<IEnumerable<long>>()
                        ))
            .Returns(usersFound);

        return _queryUserDataRead.Object;
    }

    internal static IUserDataReadQueries SaveNewSupervisorSetup(UserDataQr outputGetByUserLogin, UserDataQr outputGetByEmail)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
            .Setup(svc =>
                svc.GetByUserLogin(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<string>()
                    )
            )
            .Returns(outputGetByUserLogin);

        _queryUserDataRead
            .Setup(svc =>
                svc.GetByEmail(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<string>()
                    )
            )
            .Returns(outputGetByEmail);

        return _queryUserDataRead.Object;
    }

    internal static IUserDataReadQueries IncludeUserInManagedSetup(
        UserDataQr outputByUserId
        )
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetByUserId(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   )
           )
           .Returns(outputByUserId);

        return _queryUserDataRead.Object;
    }




    internal static IUserDataReadQueries ResetPasswordFromAdminSetup(
       UserDataQr outputByUserId
       )
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetByUserId(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   )
           )
           .Returns(outputByUserId);

        return _queryUserDataRead.Object;
    }


    internal static IUserDataReadQueries ModifyUserDataFromAdminSetup(
        UserDataQr outputByUserId
        , IEnumerable<UserDataQr> returnedActiveUsersWithEmail
        , UserDataQr outputGetByUserIdForModify
        )
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetByUserId(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   )
           )
           .Returns(outputByUserId);

        _queryUserDataRead
           .Setup(svc =>
               svc.GetActiveUsersWithEmail(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<bool>()
                   , It.IsAny<bool>()
                   )
           )
           .Returns(returnedActiveUsersWithEmail);

        _queryUserDataRead
            .Setup(svc =>
                svc.GetActiveManagedByUserIdForModify(
                    It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsAny<DateTime>()
                    , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                    )
            )
            .Returns(outputGetByUserIdForModify);

        return _queryUserDataRead.Object;
    }


    internal static IUserDataReadQueries CheckIfEmailExistsSetup(UserDataQr returnedUserByEmail)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetByEmail(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<string>()
                   )
           )
           .Returns(returnedUserByEmail);

        return _queryUserDataRead.Object;
    }

    internal static IUserDataReadQueries CheckIfEmailExistsForOtherActiveUsersSetup(IEnumerable<UserDataQr> returnedOtherUsersFound)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetActiveUsersWithEmail(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<bool>()
                   , It.IsAny<bool>()
                   )
           )
           .Returns(returnedOtherUsersFound);

        return _queryUserDataRead.Object;
    }


    internal static IUserDataReadQueries CheckIfLoginExistsSetup(UserDataQr returnedUserByLogin)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetByUserLogin(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<string>()
                   )
           )
           .Returns(returnedUserByLogin);

        return _queryUserDataRead.Object;
    }


    internal static IUserDataReadQueries CheckIfLoginExistsForOtherActiveUsersSetup(IEnumerable<UserDataQr> returnedOtherUsersFound)
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
           .Setup(svc =>
               svc.GetActiveUsers(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<bool>()
                   , It.IsAny<bool>()
                   )
           )
           .Returns(returnedOtherUsersFound);

        return _queryUserDataRead.Object;
    }

    internal static IUserDataReadQueries ChecksEnableDisableSetup(
        UserDataQr returnedUserById
        , IEnumerable<UserDataQr> returnedOtherUsersFoundForEmail
        , IEnumerable<UserDataQr> returnedOtherUsersFoundForLogin
        )
    {
        Mock<IUserDataReadQueries> _queryUserDataRead = new();

        _queryUserDataRead
          .Setup(svc =>
              svc.GetByUserId(
                  It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                  , It.IsAny<DateTime>()
                  , It.IsAny<DateTime>()
                  , It.IsAny<DateTime>()
                  , It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                  )
          )
          .Returns(returnedUserById);


        _queryUserDataRead
           .Setup(svc =>
               svc.GetActiveUsersWithEmail(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<bool>()
                   , It.IsAny<bool>()
                   )
           )
           .Returns(returnedOtherUsersFoundForEmail);

        _queryUserDataRead
           .Setup(svc =>
               svc.GetActiveUsers(
                   It.IsInRange(1, long.MaxValue, Moq.Range.Inclusive)
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<DateTime>()
                   , It.IsAny<bool>()
                   , It.IsAny<bool>()
                   )
           )
           .Returns(returnedOtherUsersFoundForLogin);

        return _queryUserDataRead.Object;
    }
}