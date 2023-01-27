namespace Examples.AspNetMvcCode.Web.Models;

public class FileInputBasicViewModel
{
    public bool IsMandatory { get; set; }

    /// <summary>
    /// special property. When attachments are present, <see cref="IsMandatory"/> must be forced to false<br/>
    /// But we need to have a flag that will keep track if it's necessary to check for mandatory 
    /// in case all attachments are deleted by user
    /// </summary>
    public bool IsMandatoryWhenEmpty { get; set; }
    public IHtmlContent Description { get; set; }
    public string FieldName { get; set; }

    /// <summary>
    /// display field as non editable but it will be submittable, if it's a input type
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// display field as non editable and not submittable
    /// </summary>
    public bool IsDisabled { get; set; }

    public IList<string> AdditionalClasses { get; set; } = new List<string>();
    public IDictionary<string, string> InputFileAdditionalAttributes { get; set; } = new Dictionary<string, string>();
    public IList<string> AcceptFileExtensions { get; set; } = new List<string>();
    public bool ForceSingleFile { get; set; }
}