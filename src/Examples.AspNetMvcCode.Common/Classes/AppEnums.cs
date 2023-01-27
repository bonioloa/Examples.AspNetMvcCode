namespace Examples.AspNetMvcCode.Common;

//ENUM CONVENTION: default values must go first
//in flags declarations, so when variables are initialized,
//they start with first/default value
//https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces#naming-enumerations
//"Type" suffix is used in some enums but not mandatory, it's used mainly when meaning can be misinterpreted

/// <summary>
/// types of possible login in application.
/// </summary>
public enum ConfigurationType
{
    Missing,
    Anonymous,//with this configuration, basicRole user needs to provide only token for access
    Registered,
}

/// <summary>
/// can be used also to verify if user has logged in.
/// Is also intermingles with role concept
/// </summary>
public enum AccessType
{
    Missing, //this is placed as first so it becomes the default initialization value 
    BasicRoleUserAnonymousForInsert,
    BasicRoleUserAnonymousWithLoginCode,
    BasicRoleUserRegistered,
    SupervisorWithAnonymousConfig,
    SupervisorWithRegisteredConfig,
}

/// <summary>
/// Type of fields actually handled by application
/// </summary>
/// <remarks>When a new field type is added, 
/// YOU MUST also update extension <see cref="FieldTypeExtensions"/>
/// </remarks>
public enum FieldType
{
    Missing,

    /// <summary>
    /// title text
    /// </summary>
    LabelTitle,

    /// <summary>
    /// title text smaller than <see cref="LabelTitle"/>
    /// </summary>
    LabelTitleSmaller,

    /// <summary>
    /// simple html text
    /// </summary>
    LabelSimple,

    OptionsCheckBox,
    OptionsRadio,
    OptionsSelect,
    OptionsSelectMultiple,

    /// <summary>
    /// short text field, limited normally limited to 40 characters in forms but can be freely configurable elsewhere 
    /// </summary>
    InputTextSimple,

    /// <summary>
    /// text field for length above 40 characters
    /// </summary>
    InputTextArea,

    /// <summary>
    /// text field for date storage and manipulation. Use in combination with a date picker
    /// </summary>
    InputDate,

    /// <summary>
    /// text field for date time storage and manipulation. Use in combination with a date time picker
    /// </summary>
    /// <remarks>not completely supported for now</remarks>
    InputDateTime,

    /// <summary>
    /// numeric field
    /// </summary>
    InputNumeric,

    /// <summary>
    /// field for multiple file upload. For now no need for a field type to handle a single file upload
    /// </summary>
    InputMultipleFile,

    /// <summary>
    /// field for single field upload that does not need to display uploaded
    /// </summary>
    InputSingleFileOnly,

    /// <summary>
    /// this is the type used for the no/yes option field with a file field on yes option
    /// It's a default field that should be automatically added to form in old databases
    /// for all steps after the first
    /// </summary>
    InputOptionalMultipleFileUpload,
}


public enum InputSimpleType
{
    Text, //default
    Email,
    Password,
}


public enum StepStateGroupType
{
    Missing,
    All,
    Open,
    Closed,
    Aborted,
}


/// <summary>
/// pay attention, this object is not the same of token config type
/// It is also needed to show the correct form after failed post
/// </summary>
public enum LoginType
{
    Anonymous, //default
    LoginCode,
    Credentials
}


public enum PermissionType
{
    View,
    Modify
}
public enum SsoLoginMode
{
    /// <summary>
    /// only local authentication (user and password)
    /// </summary>
    Local,
    /// <summary>
    /// allow authentication only through SSO for all users (authentication with item code will still be allowed)
    /// </summary>
    OnlyThroughSso,
    /// <summary>
    /// allow authentication both with application user and password or sso
    /// </summary>
    SsoOptional,

    //there is room also for other modes (ex: user uses traditional and supervisors sso or reverse)
}

public enum SsoType
{
    //only type handled by now
    Saml2Basic,

    //future types
    //OauthMicrosoft, OauthGoogle
}

public enum SsoMethodSpToIdp
{
    /// <summary>
    /// Value necessary for validations
    /// </summary>
    None,
    /// <summary>
    /// Call IDP authentication with a redirect (http get)
    /// </summary>
    Get,
    /// <summary>
    /// Call IDP authentication with http post
    /// </summary>
    Post,
}
public enum SsoIdpMetadataType
{
    /// <summary>
    /// I metadata Idp sono stati forniti in formato xml una tantum, 
    /// monitorare la situazione perché le modifiche lato Idp 
    /// non vengono recepite automaticamente dall'applicazione
    /// </summary>
    Xml,

    /// <summary>
    /// I metadata vengono scaricati da un url messo a disposizione da Idp.
    /// Verrà inviata email di avviso perché bisognerà installare il certificato incluso nei metadati
    /// </summary>
    Url,

    //config can also be loaded by file but our architecture preference
    //dictates that tenant configurations must be saved in database
}


/// <summary>
/// keys necessary to bind tenant sso claims to our app identity claims 
/// </summary>
public enum SsoAppInternalClaim
{
    UserLogin,
    Name,
    Surname,
    Email,
    /// <summary>
    /// intended when idp sends one attribute instance and all roles are found in the same value 
    /// separator must be specified in idp configuration
    /// </summary>
    RolesBySeparator,
    /// <summary>
    /// intended when idp sends multiple instance of attribute/value and one role for each value
    /// </summary>
    RolesList,
}



public enum Product
{
    Undefined,
}


/// <summary>
/// represent the possible special roles assignable to users.
/// A user can have only one of this roles and no other normal roles
/// </summary>
public enum ExclusiveRole
{
    None = 0,
    IsBasicUserOnly,
    AdminApplication,
}