namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// these constants are used to build querystrings 
/// or naming for html form submit key inputs
/// For each constant we will report in what action they are used
/// marking them for special attention when changing the string
/// because it can break action method
/// </summary>
public static class ParamsNames
{
    //available in all html forms, automatically validated by architecture for all post requests
    //remember to exclude it when getting form values by all keys
    public const string AntiforgeryToken = "__RequestVerificationToken";

    public const string SubmitType = "tipoRichiesta";

    //used to handle status code pages
    public const string Code = "codice";

    //use it to strip from every possible querystring
    public const string ReturnUrl = "ReturnUrl";

    //necessary for config identification for Sso login
    public const string RelayState = "RelayState";
    public const string SsoConfigId = "ssoConfigId";
    public const string AccessData = "accessData";

    //argument LoginToken GET action arguments        
    public const string HideLogo = "nascondiLogo";

    //argument in LoginToken GET action 
    //argument in ConfermaRegistrazioneConLink GET action
    public const string TenantToken = "token";

    //arguments for user login forms
    public const string LoginCode = "loginCode";
    public const string UserLogin = "userLogin";
    public const string Password = "password";

    //argument in ConfermaRegistrazioneConLink GET action
    public const string ValidationCode = "codiceconferma";

    //argument in Consulta GET action
    //argument in CancellaAllegatoScheda GET action 
    //argument in ScaricaAllegatoStepStoricizzato GET action 
    //argument in ScaricaAllegatoScheda GET action 
    public const string IdItem = "idElemento";

    //argument in ConsultaEvoluzioneDati
    public const string FormTableCode = "codicePassaggio";

    //CancellaAllegatoScheda
    //ScaricaAllegatoStepStoricizzato
    //ScaricaAllegatoScheda
    public const string Phase = "fase";

    //CancellaAllegatoScheda
    //ScaricaAllegatoStepStoricizzato
    //ScaricaAllegatoScheda
    public const string State = "stato";

    //not used directly in action parameters
    public const string ItemFormUpdateType = "azione";

    //ScaricaAllegatoMessaggioStoricizzato
    public const string MessageId = "idMessaggio";

    //InviaMessaggio
    public const string MessageSubject = "oggetto-messaggio";
    public const string MessageText = "testo-messaggio";

    //CancellaAllegatoScheda
    //ScaricaAllegatoStepStoricizzato
    //ScaricaAllegatoScheda
    public const string AttachmentId = "idAllegato";

    //NuovaRicerca
    //item search and reporting
    public const string ProcessId = "idProcesso";
    public const string StepStateGroup = "tipologiaStato";
    public const string ProcessStep = "stepProcesso";
    public const string DateSubmitFrom = "dataInvioDa";
    public const string DateSubmitTo = "dataInvioA";
    public const string DateExpirationFrom = "dataScadenzaDa";
    public const string DateExpirationTo = "dataScadenzaA";

    //Report
    public const string ProcessAll = "tuttiProcessi";
    public const string ProcessIdList = "idProcessi";

    public const string AddLinkedItemsToResults = "recuperaAncheCollegatiAdElementi";

    //DataGrid (dev express) save view params
    public const string DataGridSaveStateSerialized = "salva-stato-griglia-serializzato";
    public const string DataGridSaveStateDescription = "salva-stato-griglia-descrizione";
    public const string DataGridSaveStateChooseIfSaveIsForAllProfiles = "salva-stato-griglia-personale-profili";

    //DataGrid (dev express) load view params
    public const string DataGridLoadStateChoose = "carica-stato-griglia-select";

    //rivelazione identità
    public const string Motivation = "motivazione";

    //amministrazione utenti
    public const string PerformSearch = "fareRicerca";
    public const string Surname = "cognome";
    public const string Name = "nome";
    public const string Email = "email";
    public const string Roles = "ruoli";
    public const string UserId = "idutente";

    //elementi collegati
    public const string ProcessLinkedId = "idProcessoCollegato";
    public const string ProcessMasterId = "idProcessoMaster";
    public const string PhaseMaster = "faseMaster";
    public const string StateMaster = "statoMaster";
}