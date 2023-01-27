namespace Examples.AspNetMvcCode.Web.Models;

public class OptionViewModel
{
    public bool Selected { get; set; }
    public string Value { get; set; }

    [JsonConverter(typeof(IHtmlContentConverter))]
    public IHtmlContent Description { get; set; }
    public string ImagePath { get; set; }
    public bool Disabled { get; set; }
    public string ColorValue { get; set; }
}