namespace Examples.AspNetMvcCode.Web.Models;

public interface ISubmitResultJsonModel
{
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}