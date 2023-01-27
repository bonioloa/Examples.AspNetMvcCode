namespace Examples.AspNetMvcCode.Logic.Test.LogicServices.UserRole._MoqDependencies;

internal class MoqIOptionsProduct
{
    //this should be valid for all calls of class SupervisorSaveLogic
    internal static IOptionsSnapshot<ProductSettings> SupervisorSaveProductSetup(
        Product productType
        )
    {
        Mock<IOptionsSnapshot<ProductSettings>> _optProduct = new();

        _optProduct.Setup(m => m.Value)
                   .Returns(
                        new ProductSettings()
                        {
                            Product = productType
                        });

        return _optProduct.Object;
    }
}
