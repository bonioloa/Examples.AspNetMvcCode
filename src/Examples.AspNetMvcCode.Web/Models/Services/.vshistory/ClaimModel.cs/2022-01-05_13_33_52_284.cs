namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// POCO class to allow claims serialization because Claim class has some not serializable properties
/// </summary>
public class ClaimModel
{
    public string Type { get; set; }
    public IDictionary<string, string> Properties { get; set; }
    public string OriginalIssuer { get; set; }
    public string Issuer { get; set; }
    public string ValueType { get; set; }
    public string Value { get; set; }
}
