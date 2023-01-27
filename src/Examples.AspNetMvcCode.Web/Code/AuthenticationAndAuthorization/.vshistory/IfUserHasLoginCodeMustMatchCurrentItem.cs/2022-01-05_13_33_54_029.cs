namespace Comunica.ProcessManager.Web.Code;

public class IfUserHasLoginCodeMustMatchCurrentItemRequirement : IAuthorizationRequirement
{
}
public class IfUserHasLoginCodeMustMatchCurrentItemHandler
    : AuthorizationHandler<IfUserHasLoginCodeMustMatchCurrentItemRequirement>
{
    private readonly ILogger<IfUserHasLoginCodeMustMatchCurrentItemHandler> _logger;
    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    public IfUserHasLoginCodeMustMatchCurrentItemHandler(
        ILogger<IfUserHasLoginCodeMustMatchCurrentItemHandler> logger
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        )
    {
        _logger = logger;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
    }
    protected override Task HandleRequirementAsync(
       AuthorizationHandlerContext context
       , IfUserHasLoginCodeMustMatchCurrentItemRequirement requirement
       )
    {
        _logger.LogAppDebug("CALL");

        IEnumerable<string> existsQuery =
            from c in context?.User?.Claims
            where c.Type == ClaimsKeys.BasicRoleUserAnonymousWithLoginCode
            select c.Value;

        if (existsQuery.HasValues() && existsQuery.First().StringHasValue())
        {
            long idItemFromLoginCode = long.MinValue;

            IEnumerable<string> idItemQuery =
                from c in context.User.Claims
                where c.Type == ClaimsKeys.IdItemFromLoginCode
                select c.Value;
            //validation against session because context is a scoped
            //and can't be used in a singleton

            if (idItemQuery.HasValues()
                && long.TryParse(
                    idItemQuery.First()
                    , out idItemFromLoginCode)
                && idItemFromLoginCode > 0
                && _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser > 0
                && idItemFromLoginCode ==
                    _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else
            {
                _logger.LogAppError(@$"Requirement Fail. 
                        user has logged with logincode but identity is missing IdItem or mismatch with session IdItem 
                        -identity IdItem '{idItemFromLoginCode}' 
                        -SessionIdItemCurrentlyManagedByUser '{_httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser}' ");

                context.Fail();
                return Task.CompletedTask;
            }
        }

        //if other user type is automatically a success
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
