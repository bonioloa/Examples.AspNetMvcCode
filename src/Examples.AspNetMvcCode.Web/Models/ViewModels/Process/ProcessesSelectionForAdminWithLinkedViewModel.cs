namespace Examples.AspNetMvcCode.Web.Models;


/// <summary>
/// data for selection, master process info included
/// </summary>
/// <param name="ProcessId"></param>
/// <param name="Description"></param>
/// <param name="ProcessIdMaster"></param>
/// <param name="DescriptionMaster"></param>
public record ProcessesSelectionForAdminWithLinkedViewModel(
    long ProcessId
    , IHtmlContent Description
    , long? ProcessIdMaster
    , IHtmlContent DescriptionMaster
    );