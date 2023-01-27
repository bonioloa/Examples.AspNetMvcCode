namespace Comunica.ProcessManager.Web.Code;

public interface IHtmlFormToModelMapperWeb
{
    ItemSubmitViewModel MapItemSubmitFromPostedForm(IFormCollection htmlForm);
    ItemUserMessageSubmitViewModel MapItemUserMessageSubmit(IFormCollection htmlForm);
    SsoLoginModel MapPostSsoStateData(IFormCollection postSsoStateForm);
    ProcessSelectionResultModel MapProcessSelectionFromPostedForm(IFormCollection processSelectionForm);
    RoleInclusionViewModel MapRoleInclusion(IFormCollection inclusionForm);
    FileAttachmentViewModel MapItemInsertFromFile(IFormCollection formFileUpload);
    DataGridViewSaveViewModel MapGridViewSave(IFormCollection formGridViewSave, DataGridViewUsage gridViewUsageType);
}