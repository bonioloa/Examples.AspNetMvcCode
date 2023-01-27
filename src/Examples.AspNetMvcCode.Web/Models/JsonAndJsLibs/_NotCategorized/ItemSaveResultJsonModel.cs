namespace Examples.AspNetMvcCode.Web.Models;

public class ItemSaveResultJsonModel : ISubmitResultJsonModel
{
    public string LoginCodeForAnonymousInsert { get; set; }

    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}