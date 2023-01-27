using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class ValidateAndParseSubmittedRolesSelectionForNewUserTests
{
    public record ValidateAndParseSubmittedRolesSelectionForNewUserTestContract(
        //data returned from dependencies call
        bool ContextIsAlsoAdminTenant
        , ExclusiveRole ContextExclusiveRoleType
        , IEnumerable<OptionQr> RolesWithDescriptionsToReturn
        //method inputs
        , string InputSelectedExclusiveRole
        , IEnumerable<long> InputSelectedSupervisorRoles
        //expected output of tested method
        , RolesSelectionResultLgc ExpectedOutput
        );

    ValidateAndParseSubmittedRolesSelectionForNewUserTestContract DefaultSuccessTestContract;
    IEnumerable<OptionQr> DefaultRolesToReturn;
    RolesSelectionResultLgc DefaultExpectedOutput;

    public ValidateAndParseSubmittedRolesSelectionForNewUserTests()
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

        DefaultSuccessTestContract =
            new ValidateAndParseSubmittedRolesSelectionForNewUserTestContract(
                //data returned from dependencies call
                ContextIsAlsoAdminTenant: false
                , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                , DefaultRolesToReturn
                //method inputs
                , InputSelectedExclusiveRole: "None"//ExclusiveRole.None.ToString()
                , InputSelectedSupervisorRoles: new long[] { 30, 40 }
                //expected output of tested method
                , DefaultExpectedOutput
                );
    }

    private static RolesSelectionResultLgc CallForTest(ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData)
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
                MoqIRoleReadQueries.ValidateAndParseSubmittedRolesSelectionForNewUserSetup(
                    testData.RolesWithDescriptionsToReturn
                    )
                );

        return
            _logicRoleAdminManaged.ValidateAndParseSubmittedRolesSelectionForNewUser(
                testData.InputSelectedExclusiveRole
                , testData.InputSelectedSupervisorRoles
                );
    }



    [Fact]
    public void TestExceptionUserNotAdmin()
    {
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = false,
                ContextExclusiveRoleType = ExclusiveRole.None,
            };

        Assert.Throws<PmCommonException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestExceptionNoRolesConfiguredInDb()
    {
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesWithDescriptionsToReturn = new List<OptionQr>(),
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }




    [Fact]
    public void TestKoExclusiveRoleNull()
    {
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = "invalid exclusive role",
            };
        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }



    [Fact]
    public void TestOkContextAdminApplicationChangeUserToScheduler()
    {
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData =
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
        ValidateAndParseSubmittedRolesSelectionForNewUserTestContract testData = DefaultSuccessTestContract;
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }
}