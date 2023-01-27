namespace Comunica.ProcessManager.Web.Models;

public class SupervisorSearchViewModel : IOperationResultViewModel
{
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IList<OptionViewModel> Roles { get; set; }
    public IList<HtmlString> OrphanedRolesDescriptions { get; set; }
    public bool ShowResults { get; set; }
    public DataTablesNetViewModel SearchResultsModel { get; set; }



    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
