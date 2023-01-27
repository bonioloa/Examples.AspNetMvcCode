namespace Examples.AspNetMvcCode.Web.Code;

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
    public const string SharedViewCompFieldInputOptionalFileUpload = "FieldInputOptionalFileUpload";
    public const string SharedViewCompFieldInputRadio = "FieldInputRadio";
    public const string SharedViewCompFieldInputCheck = "FieldInputCheck";
    public const string SharedViewCompFieldInputRecap = "FieldInputRecap";
    public const string SharedViewCompFieldInputSelect = "FieldInputSelect";
    public const string SharedViewCompFieldInputTextSimple = "FieldInputTextSimple";
    public const string SharedViewCompFieldInputNumeric = "FieldInputNumeric";
    public const string SharedViewCompFieldInputFile = "FieldInputFile";
    public const string SharedViewCompFieldInputDate = "FieldInputDate";
    public const string SharedViewCompFieldInputTextArea = "FieldInputTextArea";
    public const string SharedViewCompFieldInputTextAreaOther = "FieldInputTextAreaOther";
    public const string SharedViewCompLanguageSelector = "LanguageSelector";
    public const string SharedViewCompRoleSingle = "SingleRole";
    public const string SharedViewCompRoleMultiple = "MultipleRoles";
    public const string SharedViewCompBackUrl = "BackUrl";
    public const string SharedViewCompPoliciesLinks = "PoliciesLinks";
    public const string SharedViewCompCaptcha = "Captcha";
    public const string SharedViewCompBannerPolicies = "BannerPolicies";
    public const string SharedViewCompModalTrigger = "ModalTrigger";
    public const string SharedViewCompAppLink = "AppLink";


    public static readonly string CtrlFallback =
        nameof(FallbackController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActRedirectToDefaultLanguage = nameof(FallbackController.ToDefaultLanguage);
    public const string ActRedirectToLoginTenant = nameof(FallbackController.ToTenantLogin);
    public const string ActRedirectToLoginTenantNoLogo = nameof(FallbackController.ToTenantLoginNoLeftPanel);
    public const string ActRedirectToValidateRegistration = nameof(FallbackController.ToValidateRegistration);
    public const string ActRedirectToItem = nameof(FallbackController.ToItem);
    public const string ActRedirectToSendErrorReport = nameof(FallbackController.ToSendErrorReport);


    public static readonly string CtrlErrors =
        nameof(ErroriController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActErrors = nameof(ErroriController.Avviso);
    public const string ActProblem = nameof(ErroriController.Problema);
    public const string ViewErrors = "Error";
    public const string ActNotSupportedBrowser = nameof(ErroriController.BrowserNonSupportato);
    public const string ViewNotSupportedBrowser = "NotSupportedBrowser";
    public const string ActMaintenance = nameof(ErroriController.Manutenzione);
    public const string ViewMaintenance = "Maintenance";


    public static readonly string CtrlAccessMain =
        nameof(AccessoPrincipaleController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginTenant = nameof(AccessoPrincipaleController.IdentificazioneGruppo);
    public const string ActLoginTenantNoLogo = nameof(AccessoPrincipaleController.IdentificazioneGruppoRidotto);
    public const string ViewTenantTokenLogin = "TenantTokenLogin";
    public const string ActLoginTenantPost = nameof(AccessoPrincipaleController.IdentificazioneGruppoPost);


    public static readonly string CtrlAccessUser =
        nameof(AccessoUtenteController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginUser = nameof(AccessoUtenteController.LoginUtente);
    public const string ViewUserLoginAnonymous = "UserLoginAnonymous";
    public const string ViewUserLoginRegistered = "UserLoginRegistered";
    public const string PartialLoginWithCredentialsForm = "_LoginWithCredentialsForm";


    public static readonly string CtrlAccessUserPost =
        nameof(AccessoUtentePostController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActLoginAnonymous = nameof(AccessoUtentePostController.LoginUtenteAnonimo);
    public const string ActLoginWithLoginCode = nameof(AccessoUtentePostController.LoginUtenteCodice);
    public const string ActLoginWithCredentials = nameof(AccessoUtentePostController.LoginUtenteCredenziali);


    public static readonly string CtrlAccessSso =
        nameof(AccessoSsoController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActConsumeAssertion = nameof(AccessoSsoController.ConsumeAssertion);
    public const string ActLoginUserSso = nameof(AccessoSsoController.LoginUtenteSso);
    public const string ActSingleLogout = nameof(AccessoSsoController.SingleLogout);
    public const string ActPerformSsoLogin = nameof(AccessoSsoController.PerformSsoLogin);
    public const string ActLoggedOut = nameof(AccessoSsoController.LoggedOut);
    public const string ViewConsumeAssertion = "ConsumeAssertion";
    public const string ViewLoggedOut = "LoggedOut";


    public static readonly string CtrlAccess2fa =
        nameof(Accesso2faController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActLogin2faGet = nameof(Accesso2faController.ValidazioneCodice);
    public const string ActLogin2faPost = nameof(Accesso2faController.ValidazioneCodicePost);
    public const string ViewUserLogin2fa = "UserLogin2fa";


    public static readonly string CtrlAccessRecover =
        nameof(AccessoRecuperoController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActRecoverUserData = nameof(AccessoRecuperoController.RecuperoDati);
    public const string ViewUserRecoverCredentials = "UserRecoverCredentials";
    public const string ActResultRecoverUserData = nameof(AccessoRecuperoController.EsitoRecuperoDati);
    public const string ViewUserRecoverCredentialsResult = "UserRecoverCredentialsResult";


    public static readonly string CtrlAccessRecoverSave =
        nameof(AccessoRecuperoSaveController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActRecoverUserDataSave = nameof(AccessoRecuperoSaveController.Recover);


    public static readonly string CtrlAccessRegistration =
        nameof(AccessoRegistrazioneController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActRegistrationGet = nameof(AccessoRegistrazioneController.Registrazione);
    public const string ViewUserRegistration = "UserRegistration";
    public const string ActRegistrationPost = nameof(AccessoRegistrazioneController.RegistrazionePost);


    public static readonly string CtrlAccountBase =
        nameof(AccountBaseController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActLogout = nameof(AccountBaseController.Logout);


    public static readonly string CtrlAccountCredentials =
        nameof(AccountCredenzialiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActChangePassword = nameof(AccountCredenzialiController.CambioPassword);
    public const string ViewUserChangePassword = "UserChangePassword";
    public const string ActResultChangePassword = nameof(AccountCredenzialiController.EsitoCambioPassword);
    public const string ViewUserChangePasswordResult = "UserChangePasswordResult";


    public static readonly string CtrlAdministration =
       nameof(AmministrazioneController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);

    public const string ActAvailableActions = nameof(AmministrazioneController.AzioniPossibili);
    public const string ViewAvailableActions = "AvailableActions";


    public static readonly string CtrlAdministrationUsers =
        nameof(AmministrazioneUtentiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActSupervisorManagement = nameof(AmministrazioneUtentiController.GestioneResponsabili);
    public const string ViewSupervisorManagement = "SupervisorManagement";
    public const string ActNewSupervisor = nameof(AmministrazioneUtentiController.NuovoResponsabile);
    public const string ViewUserNewSupervisor = "UserNewSupervisor";
    public const string ActSupervisorEdit = nameof(AmministrazioneUtentiController.ModificaResponsabili);
    public const string ViewSupervisorEdit = "SupervisorEdit";
    public const string ViewCompUserTenantManagedInclude = "UserTenantManagedInclude";
    public const string ViewCompUserDisable = "UserDisable";
    public const string ViewCompUserEnable = "UserEnable";
    public const string ViewCompUserResetPassword = "UserResetPassword";
    public const string ViewCompUserDisplayData = "UserDisplayData";
    public const string ViewCompUserEditData = "UserEditData";



    public static readonly string CtrlAdministrationUsersPost =
        nameof(AmministrazioneUtentiPostController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActUserNewSupervisorPost = nameof(AmministrazioneUtentiPostController.NuovoResponsabilePost);
    public const string ActUserEditSupervisorIncludeInManagementPost = nameof(AmministrazioneUtentiPostController.IncludiUtenteInGestionePost);
    public const string ActUserEditSupervisorDisablePost = nameof(AmministrazioneUtentiPostController.DisattivaUtentePost);
    public const string ActUserEditSupervisorEnablePost = nameof(AmministrazioneUtentiPostController.AttivaUtentePost);
    public const string ActUserEditSupervisorResetPasswordPost = nameof(AmministrazioneUtentiPostController.ResetPasswordPost);
    public const string ActUserEditSupervisorUserDataPost = nameof(AmministrazioneUtentiPostController.ModificaDatiUtente);


    public static readonly string CtrlAdminUploadItemsComplete =
       nameof(CaricamentoElementiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActAdminUploadCompleteItemsFromExcel = nameof(CaricamentoElementiController.PreparaCaricamento);
    public const string ViewAdminUploadCompleteItems = "AdminUploadCompleteItems";

    public const string ActAdminUploadCompleteItemsDownloadTemplate = nameof(CaricamentoElementiController.ScaricaTemplate);


    public static readonly string CtrlAdminUploadItemsCompletePost =
        nameof(CaricamentoElementiPostController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActAdminUploadItemsCompleteFormUpload = nameof(CaricamentoElementiPostController.CaricaDaExcel);


    public static readonly string CtrlProcesses =
        nameof(ProcessiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);

    public const string ActWelcome = nameof(ProcessiController.Benvenuto);
    public const string ActProcessSelection = nameof(ProcessiController.SelezionaProcesso);
    public const string ActProcessLinkedSelection = nameof(ProcessiController.SelezionaProcessoCollegato);
    public const string ViewLandingAndSelection = "LandingAndSelection";
    public const string ViewCompModalProcessSelection = "ProcessSelection";
    public const string ViewCompSingleProcessSelection = "SingleProcess";


    public static readonly string CtrlItemSave =
        nameof(ElementiSalvaController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActSaveNewItemForm = nameof(ElementiSalvaController.SalvaNuovo);
    public const string ActSaveItemStepForm = nameof(ElementiSalvaController.SalvaPassaggio);
    public const string ActSaveItemsFromFile = nameof(ElementiSalvaController.CaricamentoMassivoPost);



    public static readonly string CtrlItemUsersChat =
        nameof(ElementiChatUtentiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActSendItemUserMessage = nameof(ElementiChatUtentiController.InviaMessaggio);


    public static readonly string CtrlItemFileAttachment =
        nameof(GestioneAllegatiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActDeleteFormAttachment = nameof(GestioneAllegatiController.CancellaAllegatoScheda);
    public const string ActDeleteFormAttachmentNoReload = nameof(GestioneAllegatiController.CancellaAllegatoSchedaNoReload);
    public const string ActDownloadFormAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoScheda);
    public const string ActDownloadHistoryStepAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoStepStoricizzato);
    public const string ActDownloadMessageAttachment = nameof(GestioneAllegatiController.ScaricaAllegatoMessaggioStoricizzato);




    public static readonly string CtrlPermissions =
        nameof(GestionePermessiController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActAssignPermissions = nameof(GestionePermessiController.AssegnaPermessi);


    public static readonly string CtrlReporting =
        nameof(ReportisticaController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ViewEmptyReport = "EmptyReport";//same for all pages of reporting group
    public const string ActReportingDataExport = nameof(ReportisticaController.ScaricaReport);
    public const string ViewReportDataExport = "Report";

    public const string ActReportingAdvanced = nameof(ReportisticaController.ReportAvanzato);
    public const string ViewReportAdvanced = "ReportAdvanced";
    public const string PartialDataGridReportAdvanced = "_DataGridReportAdvanced";

    public const string ActReportingAdvancedSaveLayout = nameof(ReportisticaController.ReportAvanzatoSalvaLayout);
    public const string ActReportingAdvancedLoadLayout = nameof(ReportisticaController.ReportAvanzatoCaricaLayout);

    public const string ActReportingStatistics = nameof(ReportisticaController.Statistiche);
    public const string ViewReportStatistics = "Statistics";




    public static readonly string CtrlSearch =
        nameof(RicercaController).ReplaceInvariant(ControllerSuffixToRemove, string.Empty);
    public const string ActSearchNew = nameof(RicercaController.NuovaRicerca);
    public const string ViewSearch = "Search";
    public const string ViewEmptySearch = "EmptySearch";
}