namespace Examples.AspNetMvcCode.Web.Models;

public class DataGridStateViewLoadResultJsonModel : ISubmitResultJsonModel
{
    public DataGridViewStateJsonModel DataGridViewState { get; set; }


    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}