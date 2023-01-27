namespace Comunica.ProcessManager.Web.Models;

public class PersonalizationGenericModel
{
    public long Progressive { get; set; }

    /// <summary>
    /// some logics allow to handle personalization keys without statically defining them with enums or constants in code,
    /// but only with database convention matching 
    /// (in example: form columns for search results can be safely and completely defined in database)
    /// </summary>
    public string PersonalizationKey { get; set; }
    public string Content { get; set; }
}
