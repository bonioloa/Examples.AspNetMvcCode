

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(
    (hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    );

ConfigureServices(builder);

WebApplication app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();


void ConfigureServices(WebApplicationBuilder builder)
{
    //DON'T inject appsettings json, use IOption and build models for sections
    //services.AddSingleton(Configuration);

    //captcha configuration
    builder.Services.AddCaptchaServices();


    //other appsettings
    builder.Services.AddApplicationSettingsSections(builder.Configuration);


    //we can't properly retrieve value with injection in this class, so we use  this workaround
    WebArchitectureSettings webArchitectureSettings = GetWebArchitectureSection(builder.Configuration);


    #region architecture components

    builder.Services.AddAntiforgery(
        options =>
            {
                options.Cookie.Name = webArchitectureSettings.CookiesPrefix + CookieConstants.AntiforgeryName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

    builder.Services.AddCustomCookieAuthentication(
        webArchitectureSettings.CookiesPrefix
        , webArchitectureSettings.CookieAuthenticationTimeoutInMinutes
        );
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextServices(
        webArchitectureSettings.CookiesPrefix
        , webArchitectureSettings.SessionTimeoutInMinutes
        );

    builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    builder.Services.AddScoped(
        x =>
            {
                ActionContext actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                IUrlHelperFactory factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });//urlhelper injection IUrlHelper _urlHelper;

    builder.Services.AddLocalizationByRoute();

    builder.Services.Configure<FormOptions>(
        options =>
            {
                // Set the limit to for multipart forms (files/attachment included)
                //value to set up in web.config and program.cs
                options.MultipartBodyLengthLimit = webArchitectureSettings.FormMultipartBodyMaxInBytes;
            });
    #endregion



    builder.Services.AddContextServices();
    builder.Services.AddAppLayersServices();
    builder.Services.AddExternalLibrariesServices();


    builder.Services.AddAppFilters();


    //enable changes to views while debugging
    //https://stackoverflow.com/questions/53639969/net-core-mvc-page-not-refreshing-after-changes
    //https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.0#runtime-compilation
    builder.Services
        .AddControllersWithViews(
            options =>
                {
                    //see class comments
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());//applied for all post actions (see class comment)

                    //global actionfilter
                    options.Filters.AddService(typeof(GlobalFilter));
                })
        .AddViewLocalization() //mandatory for HtmlLocalizer use
        .AddSessionStateTempDataProvider()//enable temp data for session
        .AddRazorRuntimeCompilation();


    //important: don't set lowercase also for querystrings
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    //https://github.com/VahidN/DNTCaptcha.Core
    builder.Services.AddDNTCaptcha(
        options =>
        {
            options.UseSessionStorageProvider()
                    .AbsoluteExpiration(minutes: 7)
                    .ShowThousandsSeparators(false)
                    .WithNoise(pixelsDensity: 400, linesCount: 20)
                    .WithEncryptionKey("applicationantispamkey");//TODO assign new if you checkout this solution
        });


    if (IsDeployEnvironment(builder.Environment))
    {
        //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio#http-strict-transport-security-protocol-hsts
        //https://aka.ms/aspnetcore-hsts.
        builder.Services.AddHsts(
            options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(webArchitectureSettings.HstsMaxAgeInDays);//initial setting for productions
                                                                                                 //options.MaxAge = TimeSpan.FromDays(365); //if everything works set one year
                });


        //prevent error "Failed to determine the https port for redirect"
        //for correct working on IIS deploy. Additional code in Configure method
        //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1#forwarded-headers-middleware-options
        //https://medium.com/@laimis/couple-issues-with-https-redirect-asp-net-core-7021cf383e00
        //https://github.com/dotnet/aspnetcore/issues/18594
        //https://docs.microsoft.com/it-it/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio
        //https://pradeeploganathan.com/aspnetcore/https-in-asp-net-core-31/
        builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 443);


        //prevent host injection and setup for reverse proxy
        //https://andrewlock.net/adding-host-filtering-to-kestrel-in-aspnetcore/
        //https://www.iaspnetcore.com/blog/blogpost/5a068d74e42370369cf0b718
        //https://stackoverflow.com/questions/43749236/net-core-x-forwarded-proto-not-working

        IList<string> hosts = webArchitectureSettings.AllowedHosts;
        //if empty or wild-card is provided, any host will be allowed
        bool filterHostsProvided = hosts?.Count > 0 && !hosts.Contains("*");//host name wildcard
        if (filterHostsProvided)
        {
            builder.Services.Configure<HostFilteringOptions>(options => options.AllowedHosts = hosts);
        }

        //setup here header forwarding if in reverse proxy
    }
}


void ConfigureMiddleware(WebApplication app)
{
    //additional code in ConfigureServices
    if (IsDeployEnvironment(app.Environment))
    {
        //header forwarding.

        //app.UseForwardedHeaders();
        //app.Use(
        //    (context, next) =>
        //        {
        //            context.Request.Scheme = "https";
        //            return next();
        //        });
    }


    //test https://securityheaders.com/

    //https://docs.nwebsec.com/en/latest/nwebsec/libraries.html
    //alternatives https://github.com/juunas11/aspnetcore-security-headers
    //https://www.nuget.org/packages/NetEscapades.AspNetCore.SecurityHeaders/
    //https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders
    //https://www.nuget.org/packages/NetEscapades.AspNetCore.SecurityHeaders.TagHelpers/
    #region header security configuration (NWebsec.AspNetCore.Middleware)

    app.UseXContentTypeOptions();
    app.UseXDownloadOptions();
    app.UseReferrerPolicy(opts => opts.NoReferrer());
    app.UseXXssProtection(options => options.EnabledWithBlockMode());
    // X-Frame-Options header
    app.UseXfo(options => options.Deny());

    //WARNING!!! following section will not play nice if you use modernizer or other libraries that do dynamic css/script injection
    //
    //Content - Security - Policy header
    app.UseCsp(opts => opts
        .BlockAllMixedContent()

        .StyleSources(s => s.Self())
        //this is enabled only to allow modernizer to work
        //see https://github.com/Modernizr/Modernizr/issues/1262
        //if it's solved, this directive can be deleted

        .StyleSources(s => s.UnsafeInline())
        //allow download from cdn 
        .StyleSources(s => s.CustomSources(
            "https://fonts.googleapis.com"
            , "https://fonts.gstatic.com"
            , "https://cdnjs.cloudflare.com/ajax/libs/"
            ))

        .FontSources(s => s.Self())
        .FontSources(s => s.CustomSources(
            "https://fonts.googleapis.com"
            , "https://fonts.gstatic.com"
            ))

        .FormActions(s => s.Self())

        .FrameSources(s => s.Self())
   // this example is to allow for google recaptcha cross origin, tu be added in a remote future when they will start respecting GDPR
       // .FrameSources(s => s.CustomSources("https://www.google.com"))

        .FrameAncestors(s => s.Self())

        .ImageSources(s => s.Self())
        .ImageSources(s => s.CustomSources(
            "https://cdnjs.cloudflare.com/ajax/libs/"
            , "https://www.gstatic.com"
            ))

        .ScriptSources(s => s.Self())
        //allow download from cdn 
        .ScriptSources(s => s.CustomSources(
            "https://www.google.com"
            , "https://www.gstatic.com"
            , "https://cdnjs.cloudflare.com/ajax/libs/"
            , "https://cdn.datatables.net/plug-ins/"
            ))
        //this is necessary because we use <script> tag for localization and js plugin configuration
        //at this moment it's impossible to omit
        //an alternative could be json gets, but it could be a security and performance problem                
        .ScriptSources(s => s.UnsafeInline())
    ) ;
    #endregion

    //disable all Permissions-Policy (not included in NWebsec)
    /*https://www.w3.org/TR/permissions-policy-1/
     * https://www.hanselman.com/blog/easily-adding-security-headers-to-your-aspnet-core-web-app-and-getting-an-a-grade
     * https://scotthelme.co.uk/goodbye-feature-policy-and-hello-permissions-policy/
     */
    app.Use(
        (context, next) =>
            {
                context.Response.Headers.Add(
                    "Permissions-Policy"
                    , "geolocation=(),midi=(),notifications=(),push=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),speaker=(),vibrate=(),fullscreen=(self),payment=()"
                    );

                return next();
            });


    string errorHandlingPath = $"/{SupportedCulturesConstants.IsoCodeDefault}/{MvcComponents.CtrlErrors}/";

    if (IsDeployEnvironment(app.Environment))
    {
        //TODO necessary a batter handling of errors/exception 
        app.UseExceptionHandler(errorHandlingPath + MvcComponents.ActErrors);

        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
        //app.UseExceptionHandler(errorHandlingPath);
    }

    app.UseStatusCodePagesWithReExecute(errorHandlingPath + MvcComponents.ActProblem, "?" + ParamsNames.Code + "={0}");


    app.UseHttpsRedirection();//secure all site and prevent simple http protocol use

    //https://docs.nwebsec.com/en/latest/nwebsec/Configuring-cache-headers.html
    app.UseNoCacheHttpHeaders();


    //TODO raffinare con https://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/
    FileServerOptions fsOptions = new();

    fsOptions.StaticFileOptions.OnPrepareResponse =
        (context) =>
            {
                // Disable caching of all static files.
                context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Context.Response.Headers["Pragma"] = "no-cache";
                context.Context.Response.Headers["Expires"] = "-1";
                //TODO use js/css processor to add version suffix to files to ensure user cache busting
            };

    app.UseFileServer(fsOptions);


    app.UseStaticFiles();
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-6.0#serve-files-outside-of-web-root
    app.UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider =
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), AppPaths.AppPathConcreteStaticFiles)
                    ),
            RequestPath = PathsStaticFilesAdditional.AppPathBaseStaticFiles,//folder is aliased with shorter name
        });

    //place this directive before components that needs logging
    //and after the ones that don't need logging
    app.UseSerilogRequestLogging();
    app.AddRewriteRules();

    app.UseSession();

    WebArchitectureSettings webArchitectureSettings = GetWebArchitectureSection(app.Configuration);

    //add here redirect white list. Important for SSO federations with customer IDP
    //Unfortunately no way to load url at runtime after tenant identification,
    //so all tenants federation urls must be added to appsettings and loaded here
    app.UseRedirectValidation(
        opts =>
            {
                if (webArchitectureSettings.RedirectAllowedDestinations.HasValues())
                {
                    foreach (string redirlink in webArchitectureSettings.RedirectAllowedDestinations)
                    {
                        opts.AllowedDestinations(redirlink);
                    }
                }
            });

    app.AddApplicationRouteLocalization();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
}



void ConfigureEndpoints(IEndpointRouteBuilder endpointBuilder)
{
    endpointBuilder.MapControllerRoute(
        name: "default",
        pattern:
            "{" + RouteParams.Language
            + "}/{" + RouteParams.Controller
            + "}/{" + RouteParams.Action
            + "}",
        defaults: WebAppUtility.GetRouteDefaults()
      );
    //UNCOMMENT WHEN TRANSLATED ROUTE WILL BE A REQUIREMENT
    //endpointBuilder.MapDynamicControllerRoute<TranslationTransformer>(
    //    pattern:
    //       "{" + RouteParams.Language
    //       + "}/{" + RouteParams.Controller
    //       + "}/{" + RouteParams.Action
    //       + "}"
    //       );
}



/// <summary>
/// verifica se l'ambiente di hosting è deploy IIS su server
/// </summary>
/// <returns></returns>
bool IsDeployEnvironment(IWebHostEnvironment webHostEnvironment)
{
    return
        webHostEnvironment.IsProduction()
        || webHostEnvironment.EnvironmentName == AppEnvironmentsKeys.TestEnvironment;
}


WebArchitectureSettings GetWebArchitectureSection(IConfiguration configuration)
{
    return configuration.GetSection(nameof(WebArchitectureSettings))
                        .Get<WebArchitectureSettings>();
}