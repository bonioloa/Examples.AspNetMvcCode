namespace Examples.AspNetMvcCode.Data;


/// <summary>
/// main codes used for personalization. Normally each one represent a page or a part of a page
/// </summary>
public enum PersonalizationMain
{
    CompanyGroupInfo,
    CompanyGroupPolicies,
    PageLandingAndSelection,
    PageItemFormStepperSubmitResult,
    SearchMessages,
    SearchResultColumnsItem,
    SearchResultsColumnsForm,
    ReportMessages,
    ReportResults,
}


/// <summary>
/// sub codes used for personalization.
/// </summary>
public enum PersonalizationElement
{
    None,

    //CompanyGroupPolicies
    PoliciesLinks,

    //CompanyGroupInfo
    InfoWebsite,
    InfoEmail,

    //LandingAndSelection
    LandingAndSelectionTitle,
    LandingAndSelectionSubTitle,
    LandingAndSelectionInsertButton,

    //FormStepperSubmitResult
    FormStepperSubmitResultTitle,
    FormStepperSubmitResultMessageFirst,
    FormStepperSubmitResultItemCodeStyle,
    FormStepperSubmitResultMessageLast,
    FormStepperSubmitResultGreetings,


    //SearchMessages
    SearchNoResultsFound,

    //SearchResultColumnsItem (item fields, same for all tenants)
    SearchResultsColumnItemCode,
    SearchResultsColumnItemDateTimeSubmit,
    SearchResultsColumnItemDateEnd,
    SearchResultsColumnItemStepDescription,

    //SearchResultsColumnsForm (form fields only for some tenants)
    //do not map these fields in this enum because
    //a code change will be needed when a new column will be added
    //mapping them by string, you allow the system co enable them simpy by
    ///by db configuration

    //ReportMessages
    ReportNoResultsFound,
}


internal enum SqlDateTimeCoalescing
{
    None,
    MinDateTime,
    MaxDateTime,
}


public enum ItemDescriptiveCodeProgressiveType
{
    Missing,
    ByProcessOnly,
    //ByProcessAndYear,
}