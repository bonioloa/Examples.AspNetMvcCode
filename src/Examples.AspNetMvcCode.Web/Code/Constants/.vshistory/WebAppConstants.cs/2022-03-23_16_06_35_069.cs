namespace Comunica.ProcessManager.Web.Code;

public static class WebAppConstants
{
    public const string CookieAntiforgery = "antiforgery";
    public const string CookieBannerDismissed = "dismissed";
    public const string CookieSessionName = "session";
    public const string CookieAuthName = "presence";/*i know, name is weird; IMHO it is better to not give a easily recognizable name for authentication cookie*/

    public const string CookieValueTrue = "yes";

    public const string MailDefaultCredentials = "WB";


    public const string GenericContentType = "application/octet-stream";
    public const string ExcelNormalMimeType = "application/vnd.ms-excel";
    public const string ExcelOpenXmlMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string WordNormalMimeType = "application/msword";
    public const string WordOpenXmlMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    #region html attributes, classes and name

    public const string HtmlClassFullColumn = "col s12 m12 l12 xl12";

    //standard attributes
    public const string Disabled = "disabled";
    public const string HtmlMandatorySymbol = "*";
    public const string HtmlClassHidden = "hidden";
    public const string HtmlClassActive = "active";
    public const string HtmlClassDisabled = Disabled;
    public const string HtmlAttrType = "type";
    public const string HtmlAttrChecked = "checked";
    public const string HtmlAttrRequired = "required";
    public const string HtmlAttrDisabled = Disabled;
    public const string HtmlAttrReadOnly = "readonly";
    public const string HtmlAttrMultiple = "multiple";
    public const string HtmlAttrAccept = "accept";
    public const string HtmlAttrPlaceholder = "placeholder";
    public const string HtmlAttrValue = "value";
    public const string HtmlAttrStyle = "style";

    //material icons codes and classes
    public const string HtmlStandardIconClasses = "material-icons valign-middle";

    public static readonly IHtmlContent HtmlCodeAddBox = new HtmlString("add_box");//&#xe146; 
    public static readonly IHtmlContent HtmlCodeArrowDropDown = new HtmlString("arrow_drop_down");//&#xe5c5;
    public static readonly IHtmlContent HtmlCodeAttachFile = new HtmlString("attach_file");//&#xe226;
    public static readonly IHtmlContent HtmlCodeChevronRight = new HtmlString("chevron_right");//&#xe5cc;
    public static readonly IHtmlContent HtmlCodeDelete = new HtmlString("delete");//&#xe872;
    public static readonly IHtmlContent HtmlCodeDeleteForever = new HtmlString("delete_forever");//&#xe92b;
    public static readonly IHtmlContent HtmlCodeExitToApp = new HtmlString("exit_to_app");//&#xe879;
                                                                                          //public static readonly IHtmlContent HtmlCodeGetApp = new HtmlString("get_app");/*&#xe884; */
    public static readonly IHtmlContent HtmlCodeFileDownload = new HtmlString("file_download");/*&#xe2c4; */
    public static readonly IHtmlContent HtmlCodeLock = new HtmlString("lock");//&#xe897;
    public static readonly IHtmlContent HtmlCodeMailOutline = new HtmlString("mail_outline");/*&#xe0e1;*/
    public static readonly IHtmlContent HtmlCodePermIdentity = new HtmlString("perm_identity");/*&#xe8a6;*/
    //public static readonly IHtmlContent HtmlCodePrint = new HtmlString("print");//&#xe8ad;
    public static readonly IHtmlContent HtmlCodeSave = new HtmlString("save");//&#xe161;
    public static readonly IHtmlContent HtmlCodeSearch = new HtmlString("search");//&#xe8b6;
    public static readonly IHtmlContent HtmlCodeSend = new HtmlString("send");//&#xe163;
    public static readonly IHtmlContent HtmlCodeStop = new HtmlString("stop");//&#xe047;
    public static readonly IHtmlContent HtmlCodeSupervisorAccount = new HtmlString("supervisor_account");//&#xe8d3;
    public static readonly IHtmlContent HtmlCodeGroupAdd = new HtmlString("group_add");//&#xe7f0;
    public static readonly IHtmlContent HtmlCodeWarning = new HtmlString("warning");//&#xe002;
    public static readonly IHtmlContent HtmlCodePrivacy = new HtmlString("privacy_tip");//&#xf0dc;
    public static readonly IHtmlContent HtmlCodeRollback = new HtmlString("undo");//&#xe166;
    public static readonly IHtmlContent HtmlCodeHistory = new HtmlString("history");//&#xw889;
    public static readonly IHtmlContent HtmlCodePreview = new HtmlString("preview");//&#xf1c5;
    public static readonly IHtmlContent HtmlCodePieChart = new HtmlString("pie_chart");//&#xe6c4;
    public static readonly IHtmlContent HtmlCodeEdit = new HtmlString("edit");//&#xe3c9;

    //public static readonly IHtmlContent HtmlCode = new HtmlString("");

    //materialize attributes and classes
    public const string HtmlClassToolTip = "tooltipped";
    public const string HtmlAttrTooltipDescription = "data-tooltip";
    public const string HtmlAttrTooltipPosition = "data-position";
    public const string HtmlValTop = "top";//default tooltip position
    public const string HtmlClassButton = "btn waves-effect waves-light";

    //custom css values
    public const string HtmlClassButtonPrimary = "btn-primary";
    public const string HtmlClassButtonDefault = "btn-default";
    public const string HtmlClassButtonWarning = "btn-warning";
    public const string HtmlClassForSubmitBySimpleVal = "to-use-for-altern-normal";
    public const string HtmlClassForSubmitByValChecked = "to-use-for-altern-checked";

    public const string HtmlClassModalWarning = "modal-warning";
    public const string HtmlClassModalAlert = "modal-alert";
    public const string HtmlClassModalConfirm = "modal-confirm";

    //following attributes are writed in view 
    //and used by JavaScript to make events and decisions using html only
    //this saves us from building JavaScript in view 
    //and we also obtain a separation from JavaScript and view
    public const string HtmlAttrHasAttachment = "data-app-has-attachment";
    public const string HtmlAttrFieldHasOptions = "data-app-field-has-options";
    public const string HtmlAttrFieldHasRelated = "data-app-has-related-field";
    public const string HtmlAttrFieldChoiceType = "data-app-choice-type";
    public const string HtmlAttrAttachmentFieldName = "data-app-attachment-field-name";
    public const string HtmlAttrAttachmentRadioId = "data-app-attachment-radio-id";
    public const string HtmlAttrAttachmentPathsFieldName = "data-app-attachment-paths-field-name";
    public const string HtmlAttrRelatedTo = "data-app-related-to";
    public const string HtmlAttrHasDependent = "data-app-depending-field";
    public const string HtmlAttrMasterOfGroup = "data-app-master-of-group";
    public const string HtmlAttrSlaveOfGroup = "data-app-slave-of-group";
    public const string HtmlAttrGroupId = "data-app-groupid";
    public const string HtmlAttrOtherField = "data-app-other-field";
    public const string HtmlAttrDivForField = "data-app-div-for-field";
    public const string HtmlAttrIdFormDeleteAttachment = "data-app-id-form-att-del";

    public const string HtmlValueIncludeLinked = "includeLinked";

    public const string HtmlNameAssignPermission = "assign-permissions";
    public const string HtmlNameRoleGroupIdForPermissionAssignment = "role-group-id-for-permission-assignment";
    public const string HtmlIdSubmitIncludeGroups = "submit-includi-gruppi";
    public const string HtmlIdFormAttDelPrefix = "form-att-del-";
    public const string HtmlIdElemLinkAttachDelete = "el-link-attach-del-";

    public const string BannerPolicies = "banner-policies";

    public const string HtmlIdModalPolicies = "modal-policies";

    public const string HtmlIdTokenForm = "tenant-login";

    public const string HtmlIdItemsInsertFromTemplate = "items-insert-from-template";

    public const string ItemCommandIdNext = "prossima-fase";
    public const string ItemCommandIdAlternative = "salto-alternativo";
    #endregion

    public const int SimpleInputMaxLength = 50;
    public const int TextAreaMaxLength = 2000;//TODO non deve mai succedere. Se input testuale ha lunghezza 0 è un errore
    public const int OtherTextAreaMaxLenth = 500;
    public const int MessageTextAreaMaxLength = 2500;//su tabella è 4000 ma criptato diventa più lungo
    private const int MaxFilesUploadInMegabytes = 200;
    public const long MaxFilesUploadInBytes = MaxFilesUploadInMegabytes * 1024 * 1024;
    //non 4000 perché la criptazione di 4000 risulterebbe in una stringa più lunga e sicuro error di troncamento durante insert

    //this option will serialize properties with first letter to lowercase
    //so we can define json object with normal C# convention 
    //and serialization will change the case accordingly
    public static readonly JsonSerializerOptions JsonObjectModelOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


    #region static content paths (paths valid only in mvc controller/view) don't use in libraries

    public const string AppLibOpenFonts = @"~/lib/@openfonts/";

    public const string AppPathContentSite = @"~/css/site/";
    public const string AppPathContentPages = @"~/css/pages/";
    public const string AppPathContentThemes = @"~/css/themes/";

    public const string AppPathStaticDocuments = @"~/documents/";
    public const string AppPathPrivacyPolicyNoLang = AppPathStaticDocuments + "_site/privacy_policy_";


    public const string AppPathImages = @"~/images/";
    public const string AppPathTenantsLogo = AppPathImages + @"tenants_logo/";

    public const string AppPathProductsLogoPath = AppPathImages + @"products_logo/";

    public const string AppPathProductsFavIcons = AppPathImages + @"products_favicons/";

    public const string AppPathScriptsComponents = @"~/js/components/";//these are script for generic application parts, not strictly viewcomponents
    public const string AppPathScriptsPages = @"~/js/pages/";
    public const string AppPathScriptsSite = @"~/js/site/";
    public const string AppPathLibsExternalComponents = @"~/js/libUtils/";


    #endregion

    //paths to use in library context
    public const string AppPathFileTenantsLogo = @"wwwroot\images\tenants_logo\";
}