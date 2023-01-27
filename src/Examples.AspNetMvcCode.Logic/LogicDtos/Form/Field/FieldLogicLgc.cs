namespace Examples.AspNetMvcCode.Logic;

public class FieldLogicLgc
{
    public DynamicFormFieldForLogic DynamicFormFieldForLogic { get; set; }
    public string DynamicFormFieldKey { get; set; }
    public string SavedValue { get; set; }
    public bool ContainsValues { get; set; }
    public IHtmlContent FieldDescription { get; set; }
}