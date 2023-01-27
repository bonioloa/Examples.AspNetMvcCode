namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal class MoqIRandomGeneratorLogic
{
    internal static IRandomGeneratorLogic GetNewStandardPasswordSetup(string newPasswordToReturn)
    {
        Mock<IRandomGeneratorLogic> _logicRandomGenerator = new();

        _logicRandomGenerator.Setup(svc => svc.GetNewStandardPassword())
                             .Returns(newPasswordToReturn);

        return _logicRandomGenerator.Object;
    }
}