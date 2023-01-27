namespace Examples.AspNetMvcCode.Logic;
public class FormConfigSetLgc
{
    public HashSet<FormConfigLgc> FormConfigSet { get; set; } = new();
    public bool HasItemEndEditableField { get; set; }
}