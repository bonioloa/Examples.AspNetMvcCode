namespace Examples.AspNetMvcCode.Data;

public static class AccessTypeEnumExtensions
{
    public static bool HasAnonymousConfig(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.BasicRoleUserAnonymousForInsert
                || accessTypeFlag == AccessType.BasicRoleUserAnonymousWithLoginCode
                || accessTypeFlag == AccessType.SupervisorWithAnonymousConfig;
    }

    public static bool HasRegisteredConfig(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.BasicRoleUserRegistered
                || accessTypeFlag == AccessType.SupervisorWithRegisteredConfig;
    }


    public static bool IsBasicRoleUser(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.BasicRoleUserAnonymousWithLoginCode
                || accessTypeFlag == AccessType.BasicRoleUserAnonymousForInsert
                || accessTypeFlag == AccessType.BasicRoleUserRegistered;
    }
    public static bool IsSupervisorUser(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.SupervisorWithAnonymousConfig
                || accessTypeFlag == AccessType.SupervisorWithRegisteredConfig;
    }
    public static bool IsAnonymousBasicRoleUser(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.BasicRoleUserAnonymousWithLoginCode
                || accessTypeFlag == AccessType.BasicRoleUserAnonymousForInsert;
    }
    public static bool UserIsLoggedWithLoginAndPassword(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.SupervisorWithRegisteredConfig
                || accessTypeFlag == AccessType.SupervisorWithAnonymousConfig
                || accessTypeFlag == AccessType.BasicRoleUserRegistered;
    }
    public static bool UserMustExist(this AccessType accessTypeFlag)
    {
        return accessTypeFlag == AccessType.SupervisorWithRegisteredConfig
                || accessTypeFlag == AccessType.SupervisorWithAnonymousConfig
                || accessTypeFlag == AccessType.BasicRoleUserRegistered
                || accessTypeFlag == AccessType.BasicRoleUserAnonymousWithLoginCode;
    }
}