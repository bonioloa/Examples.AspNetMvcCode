namespace Examples.AspNetMvcCode.Web.Models;

public class SsoLoginModel
{
    public string LanguageIso { get; set; }
    public string Token { get; set; }

    /// <summary>
    /// <see cref="long.MinValue"/> if not found in RelayState
    /// </summary>
    public long SsoConfigId { get; set; }
    public IEnumerable<ClaimModel> SsoIdentity { get; set; }
}