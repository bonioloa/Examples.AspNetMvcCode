namespace Comunica.ProcessManager.Web.Code;

public static partial class MvcComponents
{
    public const string ViewSharedFolderPath = "~/Views/Shared/";

    //shared views
    //public const string SharedView = ViewSharedFolderPath + ".cshtml";



    #region layouts and templates (templates are nested layouts)

    public const string SharedLayoutsAndTemplatesPath = ViewSharedFolderPath + "LayoutsAndTemplates/";

    public const string LayoutMainFile = SharedLayoutsAndTemplatesPath + "_Layout.cshtml";
    public const string LayoutEmpty = SharedLayoutsAndTemplatesPath + "_LayoutEmpty.cshtml";

    //pages half white -half with panel example: userlogin and tenantlogin
    public const string TemplateSplitPageFile = SharedLayoutsAndTemplatesPath + "_TemplateSplitPage.cshtml";
    //pages with panel (recover, registration, registration validation, etc)
    public const string TemplatePanelPageFile = SharedLayoutsAndTemplatesPath + "_TemplatePanelPage.cshtml";
    //wrapping div of secure area
    public const string TemplateSecureAreaOuterFile = SharedLayoutsAndTemplatesPath + "_TemplateSecureAreaOuter.cshtml";
    //inner div for all other pages (not used in "inserimento" pages)
    public const string TemplateSecureAreaInnerGenericFile = SharedLayoutsAndTemplatesPath + "_TemplateSecureAreaInnerGeneric.cshtml";

    #endregion


    #region section names

    public const string SectionStyles = "styles";
    public const string SectionScripts = "scripts";
    public const string SectionScriptsLast = "scriptsLast";
    public const string SectionHeaderSecureArea = "headerSecureArea";
    #endregion


    #region shared partials

    public const string SharedPartialPath = ViewSharedFolderPath + "Partials/";
    public const string SharedPartialButtonContinue = SharedPartialPath + "_ButtonContinue.cshtml";
    public const string SharedPartialButtonsBackSubmit = SharedPartialPath + "_ButtonsBackSubmit.cshtml";
    public const string SharedPartialButtonSubmit = SharedPartialPath + "_ButtonSubmit.cshtml";
    public const string SharedPartialIncludeMinifiableLocalFile = SharedPartialPath + "_IncludeLocalMinifiableFile.cshtml";
    public const string SharedPartialLogoWithInfo = SharedPartialPath + "_LogoWithInfo.cshtml";
    public const string SharedPartialTitlePageInSecureArea = SharedPartialPath + "_TitlePageInSecureArea.cshtml";
    public const string SharedPartialFileInputBasic = SharedPartialPath + "_FileInputBasic.cshtml";
    #endregion









    //we remove this suffix so we can leverage nameof() controllers and
    //parametrize action and controllers without hammering strings
    private const string ControllerSuffixToRemove = "Controller";

    public const string SharedViewCompAppWarning = "AppWarning";
    public const string SharedViewCompCentralPanelLogo = "CentralPanelLogo";
    public const string SharedViewCompLeftPanel = "LeftPanel";
    public const string SharedViewCompRightPanelLogo = "RightPanelLogo";
    public const string SharedViewCompHeaderSecureArea = "HeaderSecureArea";
    public const string SharedViewCompInputOptionalFileUpload = "InputOptionalFileUpload";
    public const string SharedViewCompInputRadio = "InputRadio";
    public const string SharedViewCompInputCheck = "InputCheck";
    public const string SharedViewCompInputRecap = "InputRecap";
    public const string SharedViewCompInputSelect = "InputSelect";
    public const string SharedViewCompInputTextSimple = "InputTextSimple";
    public const string SharedViewCompInputNumeric = "InputNumeric";
    public const string SharedViewCompInputFile = "InputFile";
    public const string SharedViewCompInputFileUploadOnly = "InputFileUploadOnly";
    public const string SharedViewCompInputDate = "InputDate";
    public const string SharedViewCompInputTextArea = "InputTextArea";
    public const string SharedViewCompInputTextAreaOther = "InputTextAreaOther";
    public const string SharedViewCompLanguageSelector = "LanguageSelector";
    public const string SharedViewCompRoleSingle = "SingleRole";
    public const string SharedViewCompRoleMultiple = "MultipleRoles";
    public const string SharedViewCompBackUrl = "BackUrl";
    public const string SharedViewCompPoliciesLinks = "PoliciesLinks";
    public const string SharedViewCompReCaptcha = "ReCaptcha";
    public const string SharedViewCompBannerPolicies = "BannerPolicies";
    public const string SharedViewCompModalTrigger = "ModalTrigger";
    public const string SharedViewCompAppLink = "AppLink";


    public static readonly string CtrlFallback =
        nameof(FallbackController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActRedirectToDefaultLanguage = nameof(FallbackController.ToDefaultLanguage);
    public const string ActRedirectToLoginTenant = nameof(FallbackController.ToTenantLogin);
    public const string ActRedirectToLoginTenantNoLogo = nameof(FallbackController.ToTenantLoginNoLeftPanel);
    public const string ActRedirectToValidateRegistration = nameof(FallbackController.ToValidateRegistration);
    public const string ActRedirectToItem = nameof(FallbackController.ToItem);
    public const string ActRedirectToSendErrorReport = nameof(FallbackController.ToSendErrorReport);


    public static readonly string CtrlErrors =
        nameof(ErroriController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActErrors = nameof(ErroriController.Avviso);
    public const string ActProblem = nameof(ErroriController.Problema);
    public const string ViewErrors = "Error";
    public const string ActNotSupportedBrowser = nameof(ErroriController.BrowserNonSupportato);
    public const string ViewNotSupportedBrowser = "NotSupportedBrowser";
    public const string ActMaintenance = nameof(ErroriController.Manutenzione);
    public const string ViewMaintenance = "Maintenance";


    public static readonly string CtrlAccessMain =
        nameof(AccessoPrincipaleController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginTenant = nameof(AccessoPrincipaleController.IdentificazioneGruppo);
    public const string ActLoginTenantNoLogo = nameof(AccessoPrincipaleController.IdentificazioneGruppoRidotto);
    public const string ViewTenantTokenLogin = "TenantTokenLogin";
    public const string ActLoginTenantPost = nameof(AccessoPrincipaleController.IdentificazioneGruppoPost);


    public static readonly string CtrlAccessUser =
        nameof(AccessoUtenteController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginUser = nameof(AccessoUtenteController.LoginUtente);
    public const string ViewUserLoginAnonymous = "UserLoginAnonymous";
    public const string ViewUserLoginRegistered = "UserLoginRegistered";
    public const string PartialLoginWithCredentialsForm = "_LoginWithCredentialsForm";


    public static readonly string CtrlAccessUserPost =
        nameof(AccessoUtentePostController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginAnonymous = nameof(AccessoUtentePostController.LoginUtenteAnonimo);
    public const string ActLoginWithLoginCode = nameof(AccessoUtentePostController.LoginUtenteCodice);
    public const string ActLoginWithCredentials = nameof(AccessoUtentePostController.LoginUtenteCredenziali);


    public static readonly string CtrlAccessSso =
        nameof(AccessoSsoController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActConsumeAssertion = nameof(AccessoSsoController.ConsumeAssertion);
    public const string ActLoginUserSso = nameof(AccessoSsoController.LoginUtenteSso);
    public const string ActSingleLogout = nameof(AccessoSsoController.SingleLogout);
    public const string ActPerformSsoLogin = nameof(AccessoSsoController.PerformSsoLogin);
    public const string ActLoggedOut = nameof(AccessoSsoController.LoggedOut);
    public const string ViewConsumeAssertion = "ConsumeAssertion";
    public const string ViewLoggedOut = "LoggedOut";


    public static readonly string CtrlAccess2fa =
        nameof(Accesso2faController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActLogin2faGet = nameof(Accesso2faController.ValidazioneCodice);
    public const string ActLogin2faPost = nameof(Accesso2faController.ValidazioneCodicePost);
    public const string ViewUserLogin2fa = "UserLogin2fa";


    public static readonly string CtrlAccessRecover =
        nameof(AccessoRecuperoController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActRecoverUserData = nameof(AccessoRecuperoController.RecuperoDati);
    public const string ViewUserRecoverCredentials = "UserRecoverCredentials";
    public const string ActResultRecoverUserData = nameof(AccessoRecuperoController.EsitoRecuperoDati);
    public const string ViewUserRecoverCredentialsResult = "UserRecoverCredentialsResult";


    public static readonly string CtrlAccessRecoverSave =
        nameof(AccessoRecuperoSaveController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActRecoverUserDataSave = nameof(AccessoRecuperoSaveController.Recover);


    public static readonly string CtrlAccessRegistration =
        nameof(AccessoRegistrazioneController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActRegistrationGet = nameof(AccessoRegistrazioneController.Registrazione);
    public const string ViewUserRegistration = "UserRegistration";
    public const string ActRegistrationPost = nameof(AccessoRegistrazioneController.RegistrazionePost);


    public static readonly string CtrlAccessValidationRegistration =
        nameof(AccessoValidazioneRegistrazioneController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActValidateRegistrationGet = nameof(AccessoValidazioneRegistrazioneController.ValidaRegistrazione);
    public const string ViewUserValidateRegistration = "UserValidateRegistration";
    public const string ActValidateRegistrationPost = nameof(AccessoValidazioneRegistrazioneController.ValidaRegistrazionePost);


    public static readonly string CtrlAccountBase =
        nameof(AccountBaseController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActLogout = nameof(AccountBaseController.Logout);


    public static readonly string CtrlAccountCredentials =
        nameof(AccountCredenzialiController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActChangePassword = nameof(AccountCredenzialiController.CambioPassword);
    public const string ViewUserChangePassword = "UserChangePassword";
    public const string ActResultChangePassword = nameof(AccountCredenzialiController.EsitoCambioPassword);
    public const string ViewUserChangePasswordResult = "UserChangePasswordResult";

    public static readonly string CtrlAccountAdministration =
        nameof(AccountAmministrazioneController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActSupervisorManagement = nameof(AccountAmministrazioneController.GestioneResponsabili);
    public const string ViewSupervisorManagement = "SupervisorManagement";
    public const string ActSupervisorEdit = nameof(AccountAmministrazioneController.ModificaResponsabili);
    public const string ViewSupervisorEdit = "SupervisorEdit";
    public const string ActNewSupervisor = nameof(AccountAmministrazioneController.NuovoResponsabile);
    public const string ViewNewSupervisor = "NewSupervisor";

    public const string ActConfirmSendCredentials = nameof(AccountAmministrazioneController.ConfermaCredenzialiResponsabili);
    public const string ViewSendCredentials = "SendCredentials";
    public const string ActSendCredentialsPost = nameof(AccountAmministrazioneController.InvioEmailUtenzeNuoviResponsabili);


    public static readonly string CtrlFeedback = nameof(FeedbackController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActSendErrorReport = nameof(FeedbackController.SegnalaProblema);
    public const string ActSendErrorReportPost = nameof(FeedbackController.SegnalaProblemaPost);
    public const string ViewProblemReport = "ProblemReport";
    public const string ActProblemReportSendOk = nameof(FeedbackController.Inviata);
    public const string ViewProblemReportConfirm = "ProblemReportConfirm";


    public static readonly string CtrlProcesses =
        nameof(ProcessiController).Replace(ControllerSuffixToRemove, string.Empty);

    public const string ActWelcome = nameof(ProcessiController.Benvenuto);
    public const string ActProcessSelection = nameof(ProcessiController.SelezionaProcesso);
    public const string ActProcessLinkedSelection = nameof(ProcessiController.SelezionaProcessoCollegato);
    public const string ViewLandingAndSelection = "LandingAndSelection";
    public const string ViewCompModalProcessSelection = "ProcessSelection";
    public const string ViewCompSingleProcessSelection = "SingleProcess";


    public static readonly string CtrlItemSave =
        nameof(ElementiSalvaController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActSaveNewItemForm = nameof(ElementiSalvaController.SalvaNuovo);
    public const string ActSaveItemStepForm = nameof(ElementiSalvaController.SalvaPassaggio);
    public const string ActSaveItemsFromFile = nameof(ElementiSalvaController.CaricamentoMassivoPost);



    public static readonly string CtrlItemManagement =
        nameof(ElementiController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActStartNewItem = nameof(ElementiController.Compila);
    public const string ActStartNewLinkedItem = nameof(ElementiController.CompilaCollegato);
    public const string ActViewAndManage = nameof(ElementiController.Consulta);
    public const string ActViewAndManageCurrent = nameof(ElementiController.ConsultaCorrente);
    public const string ViewItemManagementStepper = "ItemManagementStepper";
    public const string ViewItemManagementSimple = "ItemManagementSimple";
    public const string PartialItemBasicInfo = "_ItemBasicInfo";
    public const string ViewCompItemManagementExpiration = "ItemManagementExpiration";
    public const string ViewCompItemFormStepperSectionInputs = "ItemFormStepperSectionInputs";
    public const string ViewCompItemFormStepperSubmitResult = "ItemFormStepperSubmitResult";
    public const string ViewCompItemFormSimple = "ItemFormSimple";
    public const string ViewCompItemFormSimpleInputs = "ItemFormSimpleInputs";
    public const string ViewCompItemStepAttachments = "ItemStepAttachments";
    public const string ViewCompItemStepAttachmentsDelCommands = "ItemStepAttachmentsDelCommands";
    public const string ViewCompItemCommands = "ItemCommands";
    public const string ViewCompItemHistory = "ItemHistory";
    public const string ViewCompItemLinkedInfo = "ItemLinkedInfo";
    public const string ViewCompIdentityDisclosureView = "IdentityDisclosureView";
    public const string ViewCompIdentityDisclosureAnonymous = "IdentityDisclosureAnonymous";
    public const string ViewCompIdentityDisclosureSubmitRequest = "IdentityDisclosureSubmitRequest";
    public const string ViewCompItemUsersChat = "ItemUsersChat";
    public const string ViewCompItemUserNewMessage = "ItemUserNewMessage";
    public const string ViewCompItemBackUrl = "ItemBackUrl";
    public const string ViewCompItemInsertFromFile = "ItemInsertFromFile";

    public const string ActStartNewItemsFromFile = nameof(ElementiController.CaricamentoMassivo);
    public const string ViewItemManagementFromTemplate = "ItemManagementFromTemplate";

    public const string ActStartNewItemsFromFileGetTemplate = nameof(ElementiController.ScaricaTemplateCaricamentoMassivo);



    public const string ActViewItemStepCompleteDataHistory = nameof(ElementiController.ConsultaEvoluzioneDati);
    public const string ViewCompItemStepCompleteDataHistory = "ItemStepCompleteDataHistory";

    public const string ActViewItemExportStepToExcel = nameof(ElementiController.EsportaDatiPassaggioSuExcel);
    public const string ActEditOldStepData = nameof(ElementiController.ModificaVecchioPassaggio);



    public static readonly string CtrlItemIdentity =
        nameof(GestioneIdentitaController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActRequestSubmitUserIdentity = nameof(GestioneIdentitaController.RichiediIdentitaSegnalante);


    public static readonly string CtrlItemUsersChat =
        nameof(ElementiChatUtentiController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActSendItemUserMessage = nameof(ElementiChatUtentiController.InviaMessaggio);


    public static readonly string CtrlItemFileAttachment =
        nameof(GestioneAllegatiController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActDeleteFormAttachment = nameof(GestioneAllegatiController.CancellaAllegatoScheda);
    public const string ActDeleteFormAttachmentNoReload = nameof(GestioneAllegatiController.CancellaAllegatoSchedaNoReload);
    public const string ActDownloadFormAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoScheda);
    public const string ActDownloadHistoryStepAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoStepStoricizzato);
    public const string ActDownloadMessageAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoMessaggioStoricizzato);




    public static readonly string CtrlPermissions =
        nameof(GestionePermessiController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActAssignPermissions = nameof(GestionePermessiController.AssegnaPermessi);


    public static readonly string CtrlReporting =
        nameof(ReportisticaController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ViewEmptyReport = "EmptyReport";//same for all pages of reporting group
    public const string ActReportingDataExport = nameof(ReportisticaController.ScaricaReport);
    public const string ViewReportDataExport = "Report";

    public const string ActReportingAdvanced = nameof(ReportisticaController.ReportAvanzato);
    public const string ViewReportAdvanced = "ReportAdvanced";

    public const string ActReportingStatistics = nameof(ReportisticaController.Statistiche);
    public const string ViewReportStatistics = "Statistics";

    


    public static readonly string CtrlSearch =
        nameof(RicercaController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActSearchNew = nameof(RicercaController.NuovaRicerca);
    public const string ViewSearch = "Search";
    public const string ViewEmptySearch = "EmptySearch";

    public static readonly string CtrlScheduling =
        nameof(SchedulingController).Replace(ControllerSuffixToRemove, string.Empty);
    public const string ActDoDaily = nameof(SchedulingController.DoDaily);
    public const string ViewSchedulingDone = "SchedulingDone";
}