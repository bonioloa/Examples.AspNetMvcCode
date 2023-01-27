using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class CheckAndSetRolesSelectionForModifyUserTests
{
    public record CheckAndSetRolesSelectionForModifyUserTestContract(
        //data returned from dependencies call
        bool ContextIsAlsoAdminTenant
        , ExclusiveRole ContextExclusiveRoleType
        , IEnumerable<OptionQr> RolesWithDescriptionsToReturn
        , RolesAssignedToUserQr RolesAssignedToUserToReturn
        //method inputs
        , long InputUserId
        //expected output of tested method
        , RolesSelectionLgc ExpectedOutput
        );

    CheckAndSetRolesSelectionForModifyUserTestContract DefaultSuccessTestContract;
    IEnumerable<OptionQr> DefaultRolesToReturn;
    RolesAssignedToUserQr DefaultRolesAssignedToUserToReturn;
    RolesSelectionLgc DefaultExpectedOutput;

    public CheckAndSetRolesSelectionForModifyUserTests()
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

        DefaultRolesAssignedToUserToReturn =
            new RolesAssignedToUserQr(
                ExclusiveRoleType: ExclusiveRole.None,
                IsAlsoAdminTenant: false,
                AssignedSupervisorRolesList: new long[] { 30, 40 }
                );


        DefaultExpectedOutput =
            new RolesSelectionLgc(
                ExclusiveRolesFound:
                    new[]
                    {
                        new OptionLgc()
                        {
                            Value = ExclusiveRole.AdminApplication.ToString(),
                            Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultAdminApplication
                        },
                        new OptionLgc()
                        {
                            Value = ExclusiveRole.IsBasicUserOnly.ToString(),
                            Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultBasicRole,
                        },
                        new OptionLgc()
                        {
                            Value = ExclusiveRole.None.ToString(),
                            Description = new HtmlString("scheduler role"),
                            Selected = true,
                        },
                    },
                SupervisorRolesFound:
                    new[]
                    {
                        new OptionLgc()
                        {
                            Value = "30", Description = new HtmlString("test role 30"),Selected = true
                        },
                        new OptionLgc()
                        {
                            Value = "40", Description = new HtmlString("test role 40"), Selected = true
                        },
                        new OptionLgc()
                        {
                           Value = "690", Description = new HtmlString("test role 690")
                        },
                        new OptionLgc()
                        {
                           Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role")
                        },
                    }
                );

        DefaultSuccessTestContract =
            new CheckAndSetRolesSelectionForModifyUserTestContract(
                //data returned from dependencies call
                ContextIsAlsoAdminTenant: false
                , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                , DefaultRolesToReturn
                , RolesAssignedToUserToReturn: DefaultRolesAssignedToUserToReturn
                //method inputs
                , InputUserId: GeneratorsUtility.GetStrictlyPositiveLong()
                //expected output of tested method
                , DefaultExpectedOutput
                );
    }

    private static RolesSelectionLgc CallForTest(CheckAndSetRolesSelectionForModifyUserTestContract testData)
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
                MoqIRoleReadQueries.CheckAndSetRolesSelectionForModifyUserSetup(
                    testData.RolesWithDescriptionsToReturn
                    , testData.RolesAssignedToUserToReturn
                    )
                );

        return
            _logicRoleAdminManaged.CheckAndSetRolesSelectionForModifyUser(
                testData.InputUserId
                );
    }




    [Fact]
    public void TestExceptionUserNotAdmin()
    {
        CheckAndSetRolesSelectionForModifyUserTestContract testData =
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
        CheckAndSetRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesAssignedToUserToReturn = null,
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }

    [Fact]
    public void TestExceptionContextUserIsAdminTenantAndModifiedUserIsAdminApplication()
    {
        CheckAndSetRolesSelectionForModifyUserTestContract testData =
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
        CheckAndSetRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesWithDescriptionsToReturn = new List<OptionQr>(),
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestOkAdminApplication()
    {
        CheckAndSetRolesSelectionForModifyUserTestContract testData = DefaultSuccessTestContract;
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }


    [Fact]
    public void TestOkAdminTenant()
    {
        CheckAndSetRolesSelectionForModifyUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
                ExpectedOutput =
                    new RolesSelectionLgc(
                        ExclusiveRolesFound:
                            new[]
                            {
                                new OptionLgc()
                                {
                                    Value = ExclusiveRole.IsBasicUserOnly.ToString(),
                                    Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultBasicRole,
                                },
                                new OptionLgc()
                                {
                                    Value = ExclusiveRole.None.ToString(),
                                    Description = new HtmlString("scheduler role"),
                                    Selected = true,
                                },
                            },
                        SupervisorRolesFound:
                            new[]
                            {
                                new OptionLgc()
                                {
                                    Value = "30", Description = new HtmlString("test role 30"),Selected = true
                                },
                                new OptionLgc()
                                {
                                    Value = "40", Description = new HtmlString("test role 40"), Selected = true
                                },
                                new OptionLgc()
                                {
                                   Value = "690", Description = new HtmlString("test role 690")
                                },
                                new OptionLgc()
                                {
                                   Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role")
                                },
                            }
                        )
            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }
}