namespace Examples.AspNetMvcCode.Web.Models;

/// <summary>
/// dto for DataGrid view state save
/// </summary>
public class DataGridViewStateSaveViewModel
{
    /// <summary>
    /// page context where state belongs
    /// </summary>
    public DataGridViewUsage Type { get; set; }

    /// <summary>
    /// necessary because the states schema are completely different between processes
    /// </summary>
    public long ProcessId { get; set; }

    /// <summary>
    /// user input, handle with care, because it will not be encrypted
    /// </summary>
    public string UserProvidedDescription { get; set; }

    /// <summary>
    /// if true the state will be available to all users of all profiles assigned to current user
    /// </summary>
    public bool SaveAlsoForAllProfiles { get; set; }

    /// <summary>
    /// datagrid component serialized view state
    /// </summary>
    public string StateSerialized { get; set; }
}