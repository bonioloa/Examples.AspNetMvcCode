namespace Examples.AspNetMvcCode.Web.Models;

public class FieldLogicModel
{
    public DynamicFormFieldForLogic DynamicFormField { get; set; }
    public string DynamicFormFieldKey { get; set; }
    public string SavedValue { get; set; }
    public bool ContainsValues { get; set; }
    public IHtmlContent FieldDescription { get; set; }
}