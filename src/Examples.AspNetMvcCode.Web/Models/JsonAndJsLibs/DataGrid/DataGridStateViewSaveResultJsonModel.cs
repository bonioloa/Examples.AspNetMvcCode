namespace Examples.AspNetMvcCode.Web.Models;

public class DataGridStateViewSaveResultJsonModel : ISubmitResultJsonModel
{
    public List<OptionViewModel> SavedDataGridViewStateList { get; set; } = new();


    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}