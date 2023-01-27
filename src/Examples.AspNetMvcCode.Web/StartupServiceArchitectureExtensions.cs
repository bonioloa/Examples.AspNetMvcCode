namespace Examples.AspNetMvcCode.Web;

/// <summary>
/// add here extension methods of <see cref="IServiceCollection"/> for app architecture configuration
/// </summary>
public static class StartupServiceArchitectureExtensions
{
    /// <summary>
    /// setup cookie authentication
    /// </summary>
    /// <param name="services"></param>
    public static void AddCustomCookieAuthentication(
        this IServiceCollection services
        , string cookiePrefix
        , int expireInMinutes
        )
    {
        //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
        //https://andrewlock.net/exploring-the-cookieauthenticationmiddleware-in-asp-net-core/
        //no asp .net identity used, too much work to enable it for application and db logics
        services
            .AddAuthentication(
                options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })

            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme
                , options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.Name = cookiePrefix + CookieConstants.AuthenticationName;
                    options.Cookie.IsEssential = true;

                    options.ExpireTimeSpan = TimeSpan.FromMinutes(expireInMinutes);
                    options.LoginPath = $"/{SupportedCulturesConstants.IsoCodeDefault}/{MvcComponents.CtrlAccessMain}/{MvcComponents.ActLoginTenant}";
                    options.AccessDeniedPath = $"/{SupportedCulturesConstants.IsoCodeDefault}/{MvcComponents.CtrlAccessMain}/{MvcComponents.ActLoginTenant}";
                    options.SlidingExpiration = true;
                    options.EventsType = typeof(CookieAuthenticationEventsCustom);
                    //options.Events.OnRedirectToLogin = (context) =>
                    //{
                    //    context.Response.StatusCode = 401;
                    //    return Task.CompletedTask;
                    //};
                });

        //transient because we only need once in request
        services.AddTransient<CookieAuthenticationEventsCustom>();
    }



    /// <summary>
    /// session configuration setup
    /// </summary>
    /// <param name="services"></param>
    public static void AddHttpContextServices(
        this IServiceCollection services
        , string cookiePrefix
        , int idleTimeoutInMinutes
        )
    {
        //we use this type for now because we don't really need DistributedMemoryCache 
        //(session storing in database or cookie because of multiple IIS instances)
        //but anyway we save objects in session as serialized so if we need the change
        //it will need only a directive change and not other reworks in object storing
        services.AddMemoryCache();


        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-3.1#configure-session-state
        services.AddSession(
            options =>
            {
                //longer duration because timeout reset happens only when session is read
                options.IdleTimeout = TimeSpan.FromMinutes(idleTimeoutInMinutes);
                options.Cookie.HttpOnly = true;
                //options.Cookie.Path = "";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = cookiePrefix + CookieConstants.SessionName;
                options.Cookie.IsEssential = true;
            });

        //handles routing and session 
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessorWeb, HttpContextAccessorWeb>();
    }



    public static void AddCaptchaServices(
        this IServiceCollection services
        )
    {
        services.AddTransient<ICaptchaWeb, CaptchaWeb>(); //application helper service
    }



    //filters
    //all transient because we only implement OnActionExecuting.
    //in case we need also OnActionExecuted must be upgraded to scoped
    public static void AddAppFilters(this IServiceCollection services)
    {
        services.AddTransient<GlobalFilter>(); //additional code in Startup class method AddControllersWithViews

        services.AddTransient<CheckPasswordFilter>();
        services.AddTransient<RedirectIfHasCompleteLoginFilter>();
        services.AddTransient<RedirectIfAccessWithLoginCodeFilter>();
        services.AddTransient<RedirectIfAccessSimpleAnonymousFilter>();
    }


    /// <summary>
    /// map appsettings files sections to a corresponding object
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddApplicationSettingsSections(
        this IServiceCollection services
        , IConfiguration config
        )
    {
        //more information
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
        //https://stackoverflow.com/questions/31453495/how-to-read-appsettings-values-from-json-file-in-asp-net-core
        //how to access appsettings
        //https://docs.microsoft.com/en-us/dotnet/core/extensions/options


        services.Configure<EmailClientSettings>(config.GetSection(nameof(EmailClientSettings)));
        services.Configure<EmailDefaultSettings>(config.GetSection(nameof(EmailDefaultSettings)));
        services.Configure<EmailUfSupportSettings>(config.GetSection(nameof(EmailUfSupportSettings)));
        services.Configure<WebArchitectureSettings>(config.GetSection(nameof(WebArchitectureSettings)));
        services.Configure<WebsiteSettings>(config.GetSection(nameof(WebsiteSettings)));
        services.Configure<ProductSettings>(config.GetSection(nameof(ProductSettings)));
        services.Configure<DataAccessRootSettings>(config.GetSection(nameof(DataAccessRootSettings)));
        services.Configure<DataAccessTenantSettings>(config.GetSection(nameof(DataAccessTenantSettings)));

        //services.Configure<>(config.GetSection(nameof()));
    }



    public static void AddContextServices(this IServiceCollection services)
    {
        services.AddScoped<ContextApp>();
        services.AddScoped<ContextTenant>();
        services.AddScoped<ContextUser>();
    }

    public static void AddExternalLibrariesServices(this IServiceCollection services)
    {
        //init external libraries here
    }
}