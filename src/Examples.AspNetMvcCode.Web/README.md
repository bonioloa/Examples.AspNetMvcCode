# Examples.AspNetMvcCode.Web

progetto frontend ASP .NET MVC

- wwwroot: contiene tutti i file statici del sito
  - css,js, immagini e librerie (per semplicità abbiamo omesso sia le immagini che le librerie css/js)


### Code

  - CookieAuthenticationEventsCustom: 
    - eseguito per ogni azione decorata dall'Attributo [Authorize]
    - verifica che il profilo tenant e utente salvati nel cookie di autenticazione siano coerenti con l'attuale situazione presente nel db. Se dei dati sono stati cambiati l'utente sarà rimandato alla login

  - Constants
    - MvcComponents: contiene tutte le possibili stringhe Mvc che possono essere utilizzate per generare la route o richiamare layouts, view, partials o viewComponents
    - ParamsNames:  contiene le stringhe che possono essere usate nelle querystring e come parametri delle azioni e delle chiavi per azioni con metodo POST
    - PathsStaticFilesAdditional: composizione path files statici
    - PoliciesKeys: chiavi authorization policies
    - WebAppConstants: stringhe che possono essere utilizzate nel codice html della pagina (attributi, classi, simboli)

  - Extensions: estensioni per componenti MVC (sessione, tempdata, viewdata, url)

  - Filters: utilizzati per implementare comportamenti non gestibili tramite authorization policies non bastano. E' incluso il filtro globale per tutte le azioni

  - QuerystringValidation: attributi utili da applicare ai parametri delle querystring per rifiutare chiamate GET non conformi

  - Services: contiene i servizi esclusivi per quest layer in particolare
    - AuthorizationCustomWeb: estende il servizio base di autorizzazione per applicare authorization policies come servizio
    - DataTablesNetBuilderWeb: costruisce il modello json per costruire tabelle js sulle pagine che lo necessitano
    - HttpContextAccessorWeb: estende la classe HttpContextAccessor per rendere disponibile valori di sessione usando proprietà con un tipo stabilito invece che stringhe come chiavi e valori arbitrari
    - ResultMessageMapperWeb: costruisce dinamicamente messaggi di errore localizzati in base all'output dei servizi Logic


### Controllers e azioni

E' seguito lo standard MVC per la generazione della route, per questo motivo i nomi sono in italiano, non si è voluto complicare ulteriormente creando alias per la generazione della relativa root

Ogni azione per costruzione può avere un solo attributo per l'indicazione di quale metodo HTTP è supportato

Le azioni con metodo POST per costruzione eseguono sempre una redirect ad una azione di tipo GET alla fine dell'esecuzione

NOTA: non è presente l'attributo [ ValidateAntiForgeryToken] perché è applicato automaticamente in tutte le azioni con metodo HTTP unsafe con il filtro 
AutoValidateAntiforgeryTokenAttribute (vedi Program.cs)

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


### Models

Sono stati usati solo classi e record POCO 

### TagHelpers

Un unico caso di implementazione custom di tag helper: è per creare in modo centralizzato un wrapper di codice html intorno al contenuto dinamico che si vuole fare comparire in una modal materializer

### ViewComponents

Componenti a metà strada fra un controller e una partial, utili per richiamare servizi in modo centralizzato, riutilizzabile e testabile

### Views

tutte le form create nelle views hanno l'attributo asp-antiforgery per prevenire cross script injection

E' istanziato in pagina l'anticaptcha dove esiste una form con id "form-captcha-protected"

    <form asp-controller="@MvcComponents.CtrlAccess2fa" 
      asp-action="@MvcComponents.ActLogin2faPost"
      asp-antiforgery="true" method="post" 
      id="form-captcha-protected"
      class = ""
      autocomplete="off">

Gli script esterni sono validati con hash

     <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js" 
                integrity="sha512-894YE6QWD5I59HgZOGReFYm4dnWc1Qt5NtvYSaNcOP+u1T9qYdvdihz0PPSiiqn/+/3e7Jo4EaG7TubfWGUrMQ==" 
                crossorigin="anonymous"
                asp-fallback-src="~/lib/jquery/jquery.min.js"
                asp-fallback-test="window.jQuery"
                asp-suppress-fallback-integrity="true"
                >
        </script>

Le variabili localizzate o altri parametri che sono presenti nella solution che devono essere messi a disposizione di javascript sono messi in un json freezato (non si può usare const perchè bisogna supportare IE 10)

    <script>
                var SharedConstDtLocalizedMessages = Object.freeze({

### Program.cs e estensioni configurazione startup

tutti i cookies seguono la policy
CookieSecurePolicy.Always

e dove possibile
SameSiteMode.Strict
HttpOnly

il tempo di durata è configurabile ma non più di 20-30 minuti

si usa la libreria 

> NWebsec.AspNetCore.Middleware

per configurare in modo più accurato i permessi per le referenze esterne al dominio



è stato incluso un header per bloccare i permessi extra del browser

    app.Use(
        (context, next) =>
            {
                context.Response.Headers.Add(
                    "Permissions-Policy"
                    , "geolocation=(),midi=(),notifications=(),push=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),speaker=(),vibrate=(),fullscreen=(self),payment=()"
                    );

                return next();
            });

Sono previsti url rewrite per gestire gli url dei vecchi applicativi e rimandare all' url correspondiente nel nuovo applicativo


IL sistema anticaptcha è stato aggiunto su tutte le pagine pubbliche con form e possibili dati sensibili.

E' stato omesso su pagina di immissione token e login utente per requisito