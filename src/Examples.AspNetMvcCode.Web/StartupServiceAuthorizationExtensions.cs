namespace Examples.AspNetMvcCode.Web;

/// <summary>
/// Add here extension methods of <see cref="IServiceCollection"/> 
/// </summary>
public static class StartupServiceAuthorizationExtensions
{
    /// <summary>
    /// Include authorization policy rules
    /// </summary>
    /// <param name="services"></param>
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        //https://docs.microsoft.com/it-it/aspnet/core/security/authorization/policies?view=aspnetcore-3.0
        //https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
        //https://docs.microsoft.com/it-it/aspnet/core/security/authorization/policies?view=aspnetcore-2.2
        //https://developer.okta.com/blog/2018/05/11/policy-based-authorization-in-aspnet-core
        //https://stormpath.com/blog/tutorial-policy-based-authorization-asp-net-core
        //https://www.red-gate.com/simple-talk/dotnet/c-programming/policy-based-authorization-in-asp-net-core-a-deep-dive/

        services.AddAuthorization(
            options =>
                {
                    options.AddPolicy(
                        PoliciesKeys.UserShouldHaveTenantProfile
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantProfile)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.TenantHasAnonymousConfig
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasAnonymousConfig)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.TenantHasRegisteredConfig
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasRegisteredConfig)
                            )
                        );

                    options.AddPolicy(
                            PoliciesKeys.TenantHasTwoFactorAuthenticationEnabled
                            , policy => policy.RequireAssertion(
                                context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasTwoFactorAuthenticationEnabled)
                                )
                            );

                    options.AddPolicy(
                            PoliciesKeys.TenantHasSsoOnly
                            , policy => policy.RequireAssertion(
                                context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasSsoOnly)
                                )
                            );

                    options.AddPolicy(
                            PoliciesKeys.TenantHasSsoOptional
                            , policy => policy.RequireAssertion(
                                context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasSsoOptional)
                                )
                            );

                    //this only gives that tenant has available a sso authentication option
                    //but user has still to log in
                    options.AddPolicy(
                            PoliciesKeys.TenantHasSso
                            , policy => policy.RequireAssertion(
                                context =>
                                    context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasSsoOnly)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantHasSsoOptional
                                    )
                                )
                            );

                    /// <summary>
                    /// validate both claims with this policy
                    /// because we can't have userprofile without tenant profile
                    /// </summary>
                    options.AddPolicy(
                        PoliciesKeys.UserShouldHaveCompleteProfile
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.TenantProfile)
                                && context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserProfile)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserAccessedWithLoginAndPassword
                        , policy => policy.RequireAssertion(
                            context =>
                                (context.User.HasClaim(claim => claim.Type == ClaimsKeys.BasicRoleUserRegistered)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.SupervisorWithAnonymousConfig)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.SupervisorWithRegisteredConfig)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAlsoAdminTenant)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAdminApplication)
                                    )
                                //NOT
                                && !context.User.HasClaim(claim => claim.Type == ClaimsKeys.BasicRoleUserAnonymousWithLoginCode)
                                )
                            );

                    options.AddPolicy(
                        PoliciesKeys.UserIsOnlyAdminTenant
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsSupervisor
                        , policy => policy.RequireAssertion(
                            context =>
                                (context.User.HasClaim(claim => claim.Type == ClaimsKeys.SupervisorWithAnonymousConfig)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.SupervisorWithRegisteredConfig)
                                    )
                                //NOT
                                && !context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                && !context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAdminApplication)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsSupervisorWithRegisteredConfig
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.SupervisorWithRegisteredConfig)
                                //NOT
                                && !context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserHasNotAccessWithLoginCode
                        , policy => policy.RequireAssertion(
                            context => !context.User.HasClaim(claim => claim.Type == ClaimsKeys.BasicRoleUserAnonymousWithLoginCode)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserHasAccessWithLoginCode
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.BasicRoleUserAnonymousWithLoginCode)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsNotLoggedInForSimpleAnonymousInsert
                        , policy => policy.RequireAssertion(
                            context => !context.User.HasClaim(claim => claim.Type == ClaimsKeys.BasicRoleUserAnonymousForInsert)
                            )
                        );

                    options.AddPolicy(
                        PoliciesKeys.IfUserHasLoginCodeMustMatchCurrentItem
                        , policy => policy.Requirements.Add(
                            new IfUserHasLoginCodeMustMatchCurrentItemRequirement())
                        );


                    options.AddPolicy(
                        PoliciesKeys.UserIsAdminOnly
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAdminApplication)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsAdmin
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAlsoAdminTenant)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                    || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAdminApplication)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsAdminTenant
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAlsoAdminTenant)
                                || context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsOnlyAdminTenant)
                                )
                        );

                    options.AddPolicy(
                        PoliciesKeys.UserIsAdminApplication
                        , policy => policy.RequireAssertion(
                            context =>
                                context.User.HasClaim(claim => claim.Type == ClaimsKeys.UserIsAdminApplication)
                                )
                        );



                    options.AddPolicy(
                        PoliciesKeys.EnableRegistrationForUsers
                        , policy => policy.RequireAssertion(
                            context => !context.User.HasClaim(claim => claim.Type == ClaimsKeys.DisableRegistrationForUsers)
                            )
                        );

                    //this claim presence is enough to detect user is logged in through sso authentication
                    options.AddPolicy(
                        PoliciesKeys.UserLoggedInThroughSso
                        , policy => policy.RequireAssertion(
                            context => context.User.HasClaim(claim => claim.Type == ClaimsKeys.SsoClaimTypesList)
                            )
                        );
                });
    }
}