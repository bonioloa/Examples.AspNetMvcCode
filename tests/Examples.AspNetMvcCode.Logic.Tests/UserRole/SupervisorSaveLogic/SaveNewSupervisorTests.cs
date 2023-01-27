using Examples.AspNetMvcCode.Logic.LogicServices.User;
using Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole;

public class SaveNewSupervisorTests
{
    public record SaveNewSupervisorTestDataContract(
        //data returned from dependencies call
        Product ReturnedProduct
        , long ContextCompanyGroupId
        , long ContextUserIdLoggedIn
        , bool ContextIsAlsoAdminTenant
        , ExclusiveRole ContextExclusiveRoleType
        , UserDataQr ReturnedUserDataGetByUserLogin
        , UserDataQr ReturnedUserDataGetByEmail
        , IEnumerable<OptionQr> ReturnedRolesSelection
        , long ReturnedNewUserId
        , string ReturnedNewRandomPassword
        , RolesSelectionResultLgc RolesSelectionResultToReturn
        , string ReturnedLoginErrorMessage
        , string ReturnedEmailErrorMessage
        //method inputs
        , UserNewSupervisorLgc Input
        //expected output of tested method
        , UserNewSupervisorResultLgc ExpectedOutput
        );

    UserNewSupervisorLgc DefaultValidNewSupervisor;
    IEnumerable<OptionQr> RolesAvailableForSelection;
    UserNewSupervisorResultLgc DefaultExpectedOutput;
    RolesSelectionResultLgc DefaultRolesSelectionResultToReturn;
    SaveNewSupervisorTestDataContract DefaultSuccessTestContract;
    string DefaultNewRandomPassword;
    long DefaultNewUserId;

    public SaveNewSupervisorTests()
    {
        SeedTestData();
    }

    private void SeedTestData()
    {
        Faker faker = new();
        Fixture fixture = new();

        RolesAvailableForSelection =
            new List<OptionQr>()
            {
                new OptionQr(){ Value = "15", Description = new HtmlString("Responsabile " + faker.Random.Word())},
                new OptionQr(){ Value = "2", Description = new HtmlString("Responsabile " + faker.Random.Word())},
                new OptionQr(){ Value = "9865", Description = new HtmlString("Responsabile " + faker.Random.Word())}
            };

        List<long> selectedRoleList = new() { long.Parse(RolesAvailableForSelection.Skip(1).Take(1).First().Value) };

        DefaultValidNewSupervisor =
            new UserNewSupervisorLgc(
                Login: "monaco.serioso"
                , Name: "franco"
                , Surname: "d' annunzio"
                , Email: "fraserioso@gmail.com"
                , ExclusiveRole: "None"
                , SupervisorRoles: selectedRoleList
                );

        DefaultNewRandomPassword = RandomGenerator.GetNewStandardPassword();

        DefaultNewUserId = GeneratorsUtility.GetStrictlyPositiveLong();

        DefaultRolesSelectionResultToReturn =
            new RolesSelectionResultLgc(
                ErrorMessage: string.Empty
                , SelectedRolesToSave: selectedRoleList
                );


        DefaultExpectedOutput =
            new UserNewSupervisorResultLgc(
                ErrorMessage: string.Empty
                , NewUserIdSupervisor: DefaultNewUserId
                , Password: DefaultNewRandomPassword
                );

        DefaultSuccessTestContract =
            new SaveNewSupervisorTestDataContract(
                    ReturnedProduct: fixture.Create<Product>()
                    , ContextCompanyGroupId: GeneratorsUtility.GetStrictlyPositiveLong()
                    , ContextUserIdLoggedIn: GeneratorsUtility.GetStrictlyPositiveLong()
                    , ContextIsAlsoAdminTenant: false
                    , ContextExclusiveRoleType: ExclusiveRole.AdminApplication
                    , ReturnedUserDataGetByUserLogin: null
                    , ReturnedUserDataGetByEmail: null
                    , ReturnedRolesSelection: RolesAvailableForSelection
                    , ReturnedNewUserId: DefaultNewUserId
                    , ReturnedNewRandomPassword: DefaultNewRandomPassword
                    , RolesSelectionResultToReturn: DefaultRolesSelectionResultToReturn
                    , ReturnedLoginErrorMessage: string.Empty
                    , ReturnedEmailErrorMessage: string.Empty
                    //method inputs
                    , Input: DefaultValidNewSupervisor
                    //expected output of tested method
                    , ExpectedOutput: DefaultExpectedOutput
                );
    }


    private static UserNewSupervisorResultLgc CallForTest(SaveNewSupervisorTestDataContract testData)
    {
        //requirements for tests to work
        Guard.Against.Null(testData);
        Guard.Against.Null(testData.Input);

        SupervisorSaveLogic _logicSupervisorSave =
            new(
                Mock.Of<ILogger<SupervisorSaveLogic>>(),
                MoqIOptionsProduct.SupervisorSaveProductSetup(testData.ReturnedProduct),
                new ContextTenant() { CompanyGroupId = testData.ContextCompanyGroupId },
                new ContextUser()
                {
                    UserIdLoggedIn = testData.ContextUserIdLoggedIn,
                    IsAlsoAdminTenant = testData.ContextIsAlsoAdminTenant,
                    ExclusiveRoleType = testData.ContextExclusiveRoleType,
                },
                MoqIUserDataReadQueries.SaveNewSupervisorSetup(testData.ReturnedUserDataGetByUserLogin, testData.ReturnedUserDataGetByEmail),
                MoqIUserRoleAdminManagedUow.CreateNewSupervisorSetup(testData.ReturnedNewUserId),
                MoqIRandomGeneratorLogic.GetNewStandardPasswordSetup(testData.ReturnedNewRandomPassword),
                MoqIRoleAdminManagedLogic.SaveNewSupervisorSetup(testData.RolesSelectionResultToReturn),
                MoqISupervisorSaveChecksLogic.SaveNewSupervisorSetup(testData.ReturnedLoginErrorMessage, testData.ReturnedEmailErrorMessage)
                );

        return
            _logicSupervisorSave.SaveNewSupervisor(testData.Input);
    }

    [Fact]
    public void TestOkCreateFromAdminApp()
    {
        SaveNewSupervisorTestDataContract testData = DefaultSuccessTestContract;
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestOkCreateFromAdminTenant()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = true,
                ContextExclusiveRoleType = ExclusiveRole.None,
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoContextUserNotAdmin()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                ContextIsAlsoAdminTenant = false,
                ContextExclusiveRoleType = ExclusiveRole.None,
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorSaveNewPermissions),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoLoginNull()
    {
        SaveNewSupervisorTestDataContract testData =
             DefaultSuccessTestContract with
             {
                 Input =
                        DefaultValidNewSupervisor with
                        {
                            Login = string.Empty
                        },
                 ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorLoginEmpty),
             };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoLoginEmpty()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Login = string.Empty
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorLoginEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoLoginTooLong()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input = DefaultValidNewSupervisor with
                {
                    Login = GeneratorsUtility.GetRandomStringOfLength(AppConstants.LoginMaxCharacters + 1)
                },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorLoginTooLong),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoLoginTooShort()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input = DefaultValidNewSupervisor with
                {
                    Login = GeneratorsUtility.GetRandomStringOfLength(AppConstants.LoginMinCharacters - 1)
                },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorLoginTooShort),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoLoginInvalidChar()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                        DefaultValidNewSupervisor with
                        {
                            //create a valid login with one char less from valid length and add a invalid char to fail test
                            Login = GeneratorsUtility.GetRandomStringOfLength(AppConstants.LoginMaxCharacters - 1) + ";"
                        },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorLoginInvalidFormat),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoNameNull()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Name = null
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorNameEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoNameEmpty()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Name = string.Empty
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorNameEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoSurnameNull()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Surname = null
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorSurnameEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoSurnameEmpty()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Surname = string.Empty
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorSurnameEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoEmailNull()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Email = null
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorEmailEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoEmailEmpty()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                    DefaultValidNewSupervisor with
                    {
                        Email = string.Empty
                    },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorEmailEmpty),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }

    [Fact]
    public void TestKoEmailInvalidChar()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                Input =
                        DefaultValidNewSupervisor with
                        {
                            //invalid email without @ symbol
                            Email = "cwoiwefewfew.vdcds"
                        },
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(SupervisorSaveLgcUtility.ErrorEmailInvalidFormat),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }


    [Fact]
    public void TestExceptionRolesNull()
    {
        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                RolesSelectionResultToReturn = null,
            };
        Assert.Throws<PmLogicException>(() => CallForTest(testData));
    }

    [Fact]
    public void TestKoRolesError()
    {
        string rolesSelectionResultToReturnMessage = "error";

        SaveNewSupervisorTestDataContract testData =
            DefaultSuccessTestContract with
            {
                RolesSelectionResultToReturn =
                    new RolesSelectionResultLgc(
                        ErrorMessage: rolesSelectionResultToReturnMessage
                        , SelectedRolesToSave: Enumerable.Empty<long>()
                        ),
                ExpectedOutput = SupervisorSaveLgcUtility.GetSaveNewAsKoData(rolesSelectionResultToReturnMessage),
            };
        UserNewSupervisorResultLgc userNewSupervisorResult = CallForTest(testData);

        userNewSupervisorResult.Should().BeEquivalentTo(testData.ExpectedOutput);
    }
}