namespace Examples.AspNetMvcCode.Common;

public class EmailDefaultSettings
{
    /// <summary>
    /// Mail address that will appear in "From" field. Can be overridden by email tenant logics
    /// </summary>
    public string FromAddress { get; set; }

    /// <summary>
    /// Display name for Mail address that will appear in "From" field. Can be overridden by email tenant logics
    /// </summary>
    public string FromDisplayName { get; set; }

    /// <summary>
    /// If true the emails in <see cref="BccAddresses"/> will be added in BCC field
    /// </summary>
    public bool IncludeBccMail { get; set; }

    /// <summary>
    /// Email to add in BCC field. Only if <see cref="IncludeBccMail"/> true
    /// </summary>
    public List<string> BccAddresses { get; set; }
}