namespace Comunica.ProcessManager.Web.Code;

public static class ActionDescriptorExtensions
{
    public static bool ActionHasCustomAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
    {
        if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            object[] tmpActionAttributes =
                controllerActionDescriptor.MethodInfo
                .GetCustomAttributes(
                    typeof(T)
                    , inherit: false
                    );
            return tmpActionAttributes.HasValues();
        }
        return false;
    }


    public static bool ControllerHasCustomAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
    {
        if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            object[] tmpActionAttributes =
                controllerActionDescriptor.ControllerTypeInfo
                .GetCustomAttributes(
                    typeof(T)
                    , inherit: false
                    );
            return tmpActionAttributes.HasValues();
        }
        return false;
    }
}
