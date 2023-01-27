namespace Examples.AspNetMvcCode.Common.Extensions;

public static class ContextUserExtensions
{
    public static void GuardAgainstNotAdmin(this ContextUser contextUser)
    {
        if (!contextUser.IsAlsoAdminTenant && contextUser.ExclusiveRoleType != ExclusiveRole.AdminApplication)
        {
            throw new PmCommonException($"user is not an admin '{JsonSerializer.Serialize(contextUser)}' ");
        }
    }

    public static bool IsAdminApplication(this ContextUser contextUser)
    {
        return contextUser.ExclusiveRoleType == ExclusiveRole.AdminApplication;
    }

    public static bool IsBasicUserOnly(this ContextUser contextUser)
    {
        return contextUser.ExclusiveRoleType == ExclusiveRole.IsBasicUserOnly;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextUser"></param>
    /// <returns></returns>
    /// <exception cref="PmCommonException">thrown if user has no exclusive roles and supervisor role list is empty</exception>
    public static bool IsSupervisor(this ContextUser contextUser)
    {
        if (contextUser.ExclusiveRoleType == ExclusiveRole.None
            && contextUser.AssignedSupervisorRolesFound.IsNullOrEmpty()
            && !contextUser.IsAlsoAdminTenant)
        {
            throw new PmCommonException("user has no exclusive role type but supervisor roles are empty. This is a configuration error");
        }

        return
            contextUser.ExclusiveRoleType == ExclusiveRole.None
            && (contextUser.AssignedSupervisorRolesFound.HasValues()
                || contextUser.IsAlsoAdminTenant);
    }
}