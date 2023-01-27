using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class ValidateAndParseSubmittedRolesSelectionForModifyUserTests
{
    public record ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract(
        //data returned from dependencies call
        bool ContextIsAlsoAdminTenant
        , ExclusiveRole ContextExclusiveRoleType
        , IEnumerable<OptionQr> RolesWithDescriptionsToReturn
        , RolesAssignedToUserQr RolesAssignedToUserToReturn
        //method inputs
        , long InputUserId
        , string InputSelectedExclusiveRole
        , IEnumerable<long> InputSelectedSupervisorRoles
        //expected output of tested method
        , RolesSelectionResultLgc ExpectedOutput
        );

    ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract DefaultSuccessTestContract;
    IEnumerable<OptionQr> DefaultRolesToReturn;
    RolesAssignedToUserQr DefaultRolesAssignedToUserToReturn;
    RolesSelectionResultLgc DefaultExpectedOutput;

    public ValidateAndParseSubmittedRolesSelectionForModifyUserTests()
    {
        SeedTestData();
    }

    private void SeedTestData()
    {
        DefaultRolesToReturn =
            new[]
            {
                new OptionQr(){Value = "30", Description = new HtmlString("test role 30") },
                new OptionQr(){Value = "40", Description = new HtmlString("test role 40") },
                new OptionQr(){Value = "690", Description = new HtmlString("test role 690") },
                new OptionQr(){Value = RoleQrUtility.AdminApplicationRoleCode.ToString() },//no description to test default replacement
                new OptionQr(){Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role") },
                new OptionQr(){Value = RoleQrUtility.BasicRoleId.ToString() },//no description to test default replacement
            };

        DefaultExpectedOutput =
            new RolesSelectionResultLgc(
                ErrorMessage: string.Empty
                , SelectedRolesToSave: new long[] { 30, 40 }
                );

        DefaultRolesAssignedToUserToReturn =
            new RolesAssignedToUserQr(
                ExclusiveRoleType: ExclusiveRole.None,
                IsAlsoAdminTenant: false,
                AssignedSupervisorRolesList: new long[] { 30, 40 }
                );

        DefaultSuccessTestContract =
            new ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract(
                //data returned from dependencies call
                ContextIsAlsoAdminTenant: false
                , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                , DefaultRolesToReturn
                , RolesAssignedToUserToReturn: DefaultRolesAssignedToUserToReturn
                //method inputs
                , InputUserId: GeneratorsUtility.GetStrictlyPositiveLong()
                , InputSelectedExclusiveRole: "None"//ExclusiveRole.None.ToString()
                , InputSelectedSupervisorRoles: new long[] { 30, 40 }
                //expected output of tested method
                , DefaultExpectedOutput
                );
    }

    private static RolesSelectionResultLgc CallForTest(ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData)
    {
        //requirements for tests to work
        Guard.Against.Null(testData);

        RoleAdminManagedLogic _logicRoleAdminManaged =
            new(
                Mock.Of<ILogger<RoleAdminManagedLogic>>(),
                new ContextUser()
                {
                    IsAlsoAdminTenant = testData.ContextIsAlsoAdminTenant,
                    ExclusiveRoleType = testData.ContextExclusiveRoleType,
                },
                MoqIRoleReadQueries.ValidateAndParseSubmittedRolesSelectionForModifyUserSetup(
                    testData.RolesWithDescriptionsToReturn
                    , testData.RolesAssignedToUserToReturn
                    )
                );

        return
            _logicRoleAdminManaged.ValidateAndParseSubmittedRolesSelectionForModifyUser(
                testData.InputUserId
                , testData.InputSelectedExclusiveRole
                , testData.InputSelectedSupervisorRoles
                );
    }



    [Fact]
    public void TestExceptionUserNotAdmin()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = false,
                ContextExclusiveRoleType = ExclusiveRole.None,
            };

        Assert.Throws<PmCommonException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestExceptionNoRoleInfoForModifiedUser()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesAssignedToUserToReturn = null,
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }

    [Fact]
    public void TestExceptionContextUserIsAdminTenantAndModifiedUserIsAdminApplication()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
                RolesAssignedToUserToReturn =
                    new RolesAssignedToUserQr(
                        AssignedSupervisorRolesList: Array.Empty<long>()
                        , ExclusiveRoleType: ExclusiveRole.AdminApplication
                        , IsAlsoAdminTenant: false
                        ),
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }



    [Fact]
    public void TestExceptionNoRolesConfiguredInDb()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesWithDescriptionsToReturn = new List<OptionQr>(),
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }



    [Fact]
    public void TestKoExclusiveRoleNull()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = null,
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                        ErrorMessage: RoleAdminManagedLgcUtility.ErrorNewUserSubmitExclusiveRoleEmpty,
                        SelectedRolesToSave: Enumerable.Empty<long>()
                        )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoExclusiveRoleEmpty()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = string.Empty,
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                        ErrorMessage: RoleAdminManagedLgcUtility.ErrorNewUserSubmitExclusiveRoleEmpty,
                        SelectedRolesToSave: Enumerable.Empty<long>()
                        )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestExceptionExclusiveRoleInvalid()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = "invalid exclusive role",
            };
        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }




    [Fact]
    public void TestOkContextAdminApplicationChangeUserToScheduler()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = "Scheduler",//ExclusiveRole.Scheduler.ToString()
                InputSelectedSupervisorRoles = Enumerable.Empty<long>(),
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                    ErrorMessage: string.Empty,
                    SelectedRolesToSave: new long[] { RoleQrUtility.SchedulerRole }
                    )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestOkContextAdminApplicationChangeUserToAdminApplication()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = "AdminApplication", //ExclusiveRole.AdminApplication.ToString()
                InputSelectedSupervisorRoles = Enumerable.Empty<long>(),
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                    ErrorMessage: string.Empty
                    , SelectedRolesToSave: new long[] { RoleQrUtility.AdminApplicationRoleCode }
                    )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestOkContextAdminTenantChangeUserToBasicUser()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
                InputSelectedExclusiveRole = "IsBasicUserOnly", //ExclusiveRole.IsBasicUserOnly.ToString()
                InputSelectedSupervisorRoles = Enumerable.Empty<long>(),
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                    ErrorMessage: string.Empty
                    , SelectedRolesToSave: new long[] { RoleQrUtility.BasicRoleId }
                    )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }




    [Fact]
    public void TestKoContextAdminApplicationSupervisorRoleSelectedNull()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = ExclusiveRole.None.ToString(),
                InputSelectedSupervisorRoles = null,
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                        ErrorMessage: RoleAdminManagedLgcUtility.ErrorRolesEmpty,
                        SelectedRolesToSave: Enumerable.Empty<long>()
                        )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoContextAdminApplicationSupervisorRoleSelectedEmpty()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = ExclusiveRole.None.ToString(),
                InputSelectedSupervisorRoles = Enumerable.Empty<long>(),
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                        ErrorMessage: RoleAdminManagedLgcUtility.ErrorRolesEmpty,
                        SelectedRolesToSave: Enumerable.Empty<long>()
                        )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }





    [Fact]
    public void TestKoSubmitRolesNotExistInDb()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedSupervisorRoles =
                    new long[]
                    {
                        99,
                        98
                    },
                ExpectedOutput =
                    new RolesSelectionResultLgc(
                       ErrorMessage: SupervisorSaveLgcUtility.ErrorRolesInvalid
                       , SelectedRolesToSave: Enumerable.Empty<long>()
                       )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }









    [Fact]
    public void TestOkAdminApplicationSelectSupervisorRoles()
    {
        ValidateAndParseSubmittedRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract;
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }
}