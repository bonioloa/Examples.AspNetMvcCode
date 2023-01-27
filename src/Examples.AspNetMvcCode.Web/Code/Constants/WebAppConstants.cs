namespace Examples.AspNetMvcCode.Web.Code;

public static class WebAppConstants
{
    public const string HtmlClassFullColumn = "col s12 m12 l12 xl12";

    //standard attributes
    private const string Disabled = "disabled";
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
    public const string HtmlAttrSelected = "selected";

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
    public static readonly IHtmlContent HtmlCodePrint = new HtmlString("print");//&#xe8ad;
    public static readonly IHtmlContent HtmlCodeSave = new HtmlString("save");//&#xe161;
    public static readonly IHtmlContent HtmlCodeSearch = new HtmlString("search");//&#xe8b6;
    public static readonly IHtmlContent HtmlCodeSend = new HtmlString("send");//&#xe163;
    public static readonly IHtmlContent HtmlCodeStop = new HtmlString("stop");//&#xe047;
    public static readonly IHtmlContent HtmlCodeSupervisorAccount = new HtmlString("supervisor_account");//&#xe8d3;
    public static readonly IHtmlContent HtmlCodeAdminPanel = new HtmlString("admin_panel_settings");//&#xef3d;
    public static readonly IHtmlContent HtmlCodeGroupAdd = new HtmlString("group_add");//&#xe7f0;
    public static readonly IHtmlContent HtmlCodeWarning = new HtmlString("warning");//&#xe002;
    public static readonly IHtmlContent HtmlCodePrivacy = new HtmlString("privacy_tip");//&#xf0dc;
    public static readonly IHtmlContent HtmlCodeRollback = new HtmlString("undo");//&#xe166;
    public static readonly IHtmlContent HtmlCodeHistory = new HtmlString("history");//&#xw889;
    public static readonly IHtmlContent HtmlCodePreview = new HtmlString("preview");//&#xf1c5;
    public static readonly IHtmlContent HtmlCodePieChart = new HtmlString("pie_chart");//&#xe6c4;
    public static readonly IHtmlContent HtmlCodeEdit = new HtmlString("edit");//&#xe3c9;
    public static readonly IHtmlContent HtmlPdf = new HtmlString("picture_as_pdf");//&#xe415;

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

    public const string HtmlIdUploadCompleteItemsFromTemplate = "complete-items-forms";
    public const string HtmlIdUploadCompleteItemsTemplateDownloadLink = "template-link";

    public const string HtmlItemIdsInsertFromTemplate = "items-insert-from-template";
    public const string HtmlNameCheckSaveAsDraft = "check-save-items-as-drafts";
    public const string HtmlValueCheckSaveAsDraft = "bozza";


    public const string ItemCommandIdNext = "prossima-fase";
    public const string ItemCommandIdAlternative = "salto-alternativo";

    public const int SimpleInputMaxLength = 50;
    public const int TextAreaMaxLength = 2000;//TODO non deve mai succedere. Se input testuale ha lunghezza 0 è un errore
    public const int OtherTextAreaMaxLenth = 500;
    public const int MessageTextAreaMaxLength = 2500;//su tabella è 4000 ma criptato diventa più lungo
    private const int MaxFilesUploadInMegabytes = 200;
    public const long MaxFilesUploadInBytes = MaxFilesUploadInMegabytes * 1024 * 1024;
    //non 4000 perché la criptazione di 4000 risulterebbe in una stringa più lunga e sicuro error di troncamento durante insert
}