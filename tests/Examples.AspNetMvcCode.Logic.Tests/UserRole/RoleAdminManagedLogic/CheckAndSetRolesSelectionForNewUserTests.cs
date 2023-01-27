using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class CheckAndSetRolesSelectionForNewUserTests
{
    public record CheckAndSetRolesSelectionForNewUserTestContract(
       //data returned from dependencies call
       bool ContextIsAlsoAdminTenant
       , ExclusiveRole ContextExclusiveRoleType
       , IEnumerable<OptionQr> RolesWithDescriptionsToReturn
       //method inputs
       , string InputSelectedExclusiveRole
       , IEnumerable<long> InputSelectedSupervisorRoles
       //expected output of tested method
       , RolesSelectionLgc ExpectedOutput
       );

    CheckAndSetRolesSelectionForNewUserTestContract DefaultSuccessTestContract;
    IEnumerable<OptionQr> DefaultRolesToReturn;
    //IEnumerable<long> DefaultInputSelectedFilterRoles;
    RolesSelectionLgc DefaultExpectedOutput;

    public CheckAndSetRolesSelectionForNewUserTests()
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
            new CheckAndSetRolesSelectionForNewUserTestContract(
                //data returned from dependencies call
                ContextIsAlsoAdminTenant: false
                , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                , DefaultRolesToReturn
                //method inputs
                , InputSelectedExclusiveRole: string.Empty
                , InputSelectedSupervisorRoles: null
                //expected output of tested method
                , DefaultExpectedOutput
                );
    }

    private static RolesSelectionLgc CallForTest(CheckAndSetRolesSelectionForNewUserTestContract testData)
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
                MoqIRoleReadQueries.CheckAndSetRolesSelectionForNewUserSetup(testData.RolesWithDescriptionsToReturn)
                );

        return
            _logicRoleAdminManaged.CheckAndSetRolesSelectionForNewUser(
                testData.InputSelectedExclusiveRole
                , testData.InputSelectedSupervisorRoles
                );
    }


    [Fact]
    public void TestExceptionUserNotAdmin()
    {
        CheckAndSetRolesSelectionForNewUserTestContract testData =
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
        CheckAndSetRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesWithDescriptionsToReturn = new List<OptionQr>(),
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestOkAdminApplicationNoInput()
    {
        CheckAndSetRolesSelectionForNewUserTestContract testData = DefaultSuccessTestContract;
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestOkAdminTenantNoInput()
    {
        CheckAndSetRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
                ExpectedOutput =
                    new RolesSelectionLgc(
                        ExclusiveRolesFound:
                            new[]
                            {
                                //admin and scheduler are not available for admin tenant
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


    //InputSelectedExclusiveRole = "invalid" -> none
    [Fact]
    public void TestOkAdminApplicationWithExclusiveRole()
    {
        CheckAndSetRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = ExclusiveRole.AdminApplication.ToString(),
                InputSelectedSupervisorRoles = null,
                ExpectedOutput =
                    new RolesSelectionLgc(
                        ExclusiveRolesFound:
                            new[]
                            {
                                new OptionLgc()
                                {
                                    Value = ExclusiveRole.AdminApplication.ToString(),
                                    Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultAdminApplication,
                                    Selected = true
                                },
                                new OptionLgc()
                                {
                                    Value = ExclusiveRole.Scheduler.ToString(),
                                    Description = new HtmlString("scheduler role"),
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
                                    Value = "30", Description = new HtmlString("test role 30")
                                },
                                new OptionLgc()
                                {
                                    Value = "40", Description = new HtmlString("test role 40")
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


    [Fact]
    public void TestOkAdminApplicationWithSupervisorRoles()
    {
        CheckAndSetRolesSelectionForNewUserTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedExclusiveRole = "wrong string will be treated as None",
                InputSelectedSupervisorRoles = new long[] { 30, 40 },
                ExpectedOutput =
                    new RolesSelectionLgc(
                        ExclusiveRolesFound:
                            new[]
                            {
                                new OptionLgc()
                                {
                                    Value = ExclusiveRole.AdminApplication.ToString(),
                                    Description = RoleAdminManagedLgcUtility.DescriptionExclusiveRoleDefaultAdminApplication,
                                    Selected = true
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
                                    Value = "30", Description = new HtmlString("test role 30"), Selected = true
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