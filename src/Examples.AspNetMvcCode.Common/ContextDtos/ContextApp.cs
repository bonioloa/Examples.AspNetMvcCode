namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// WARNING: this class properties are available after globalfilter, not available in cookie validation
/// </summary>
public class ContextApp
{
    public IList<string> AppSupportedCulturesIsoCodes { get; set; }
    public string CurrentCultureIsoCode { get; set; }
}