using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class GetRolesFilterForSearchTests
{
    public record GetRolesFilterForSearchTestContract(
        //data returned from dependencies call
        bool ContextIsAlsoAdminTenant
        , ExclusiveRole ContextExclusiveRoleType
        , IEnumerable<OptionQr> RolesWithDescriptionsToReturn
        //method inputs
        , IEnumerable<long> InputSelectedFilterRoles
        //expected output of tested method
        , IEnumerable<OptionLgc> ExpectedOutput
        );

    GetRolesFilterForSearchTestContract DefaultSuccessTestContract;
    IEnumerable<OptionQr> DefaultRolesToReturn;
    IEnumerable<long> DefaultInputSelectedFilterRoles;
    IEnumerable<OptionLgc> DefaultExpectedOutput;

    public GetRolesFilterForSearchTests()
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
                new OptionQr(){Value = RoleQrUtility.AdminApplicationRoleCode.ToString(), Description = new HtmlString("admin application role") },
                new OptionQr(){Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role") },
            };

        DefaultInputSelectedFilterRoles =
            new long[]
            {
                30,
                40,
                1
            };

        DefaultExpectedOutput =
            new[]
            {
                new OptionLgc(){Value = "30", Description = new HtmlString("test role 30") , Selected = true},
                new OptionLgc(){Value = "40", Description = new HtmlString("test role 40") , Selected = true},
                new OptionLgc(){Value = "690", Description = new HtmlString("test role 690") },
                new OptionLgc()
                {
                    Value = RoleQrUtility.AdminApplicationRoleCode.ToString()
                    , Description = new HtmlString("admin application role")
                    , Selected = true
                },
                new OptionLgc(){Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role") },
            };

        DefaultSuccessTestContract =
            new GetRolesFilterForSearchTestContract(
                //data returned from dependencies call
                ContextIsAlsoAdminTenant: false
                , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                , DefaultRolesToReturn
                //method inputs
                , DefaultInputSelectedFilterRoles
                //expected output of tested method
                , DefaultExpectedOutput
                );
    }

    private static IEnumerable<OptionLgc> CallForTest(GetRolesFilterForSearchTestContract testData)
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
                MoqIRoleReadQueries.GetRolesFilterForSearchSetup(testData.RolesWithDescriptionsToReturn)
                );

        return
            _logicRoleAdminManaged.GetRolesFilterForSearch(testData.InputSelectedFilterRoles);
    }



    [Fact]
    public void TestExceptionNullInput()
    {
        GetRolesFilterForSearchTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedFilterRoles = null
            };

        Assert.Throws<ArgumentNullException>(() => CallForTest(testData));
    }

    [Fact]
    public void TestExceptionUserNotAdmin()
    {
        GetRolesFilterForSearchTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = false,
                ContextExclusiveRoleType = ExclusiveRole.None,
            };

        Assert.Throws<PmCommonException>(() => CallForTest(testData));
    }

    [Fact]
    public void TestExceptionSubmitRolesNotExist()
    {
        GetRolesFilterForSearchTestContract testData =
            DefaultSuccessTestContract with
            {
                InputSelectedFilterRoles =
                    new long[]
                    {
                        99,
                        98
                    }
            };

        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestExceptionInvalidRole()
    {
        GetRolesFilterForSearchTestContract testData =
            DefaultSuccessTestContract with
            {
                RolesWithDescriptionsToReturn =
                    new[]
                    {
                         new OptionQr(){Value = "errore"},
                    }
            };

        Assert.Throws<FormatException>(() => CallForTest(testData));
    }


    [Fact]
    public void TestOkForAdminTenant()
    {
        GetRolesFilterForSearchTestContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
                InputSelectedFilterRoles =
                    new long[]
                    {
                        30,
                        40
                    },
                ExpectedOutput =
                    new[]
                    {
                        new OptionLgc(){Value = "30", Description = new HtmlString("test role 30") , Selected = true},
                        new OptionLgc(){Value = "40", Description = new HtmlString("test role 40") , Selected = true},
                        new OptionLgc(){Value = "690", Description = new HtmlString("test role 690") },
                        new OptionLgc(){Value = RoleQrUtility.AdminTenantRole.ToString(), Description = new HtmlString("admin tenant role") },
                    }

            };
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }



    [Fact]
    public void TestOkAdminApplication()
    {
        GetRolesFilterForSearchTestContract testData = DefaultSuccessTestContract;
        var output = CallForTest(testData);

        output.Should().BeEquivalentTo(testData.ExpectedOutput);
    }
}