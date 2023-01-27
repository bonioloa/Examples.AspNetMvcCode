namespace Examples.AspNetMvcCode.Web.Models;

public class FieldViewModel
{
    public long Progressive { get; set; }
    public int SectionIndex { get; set; }
    public string FieldName { get; set; }
    public FieldType FieldType { get; set; }
    public bool IsMandatory { get; set; }

    /// <summary>
    /// display field as non editable but it will be submittable, if it's a input type
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// display field as non editable and not submittable
    /// </summary>
    public bool IsDisabled { get; set; }

    public IHtmlContent Description { get; set; }
    public long CharactersLimit { get; set; }

    public string CssClass { get; set; }
    public IHtmlContent PrefixHtml { get; set; }
    public IHtmlContent SuffixHtml { get; set; }
    public DependencyRule Rule { get; set; }
    public string DependingField { get; set; }
    public IHtmlContent RelatedFieldHtmlLabel { get; set; }

    public string Value { get; set; }
    public IList<FileAttachmentViewModel> Attachments { get; set; } = new List<FileAttachmentViewModel>();

    /// <summary>
    /// in case a field has a label too long to use the default materialize animation, </br>
    /// set this property at true and the label will be placed in a div above the field taking all the space needed
    /// without going above the input field
    /// </summary>
    public bool UseFixedLabel { get; set; }
    public bool HideMandatorySymbol { get; set; }

    public string OptionsGroupCode { get; set; }
    public IList<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

    public bool ChoicesUseOnlyImagesAsDescription { get; set; }
    public long ChoicesOnSameLine { get; set; }
    public long GroupId { get; set; }
    public bool IsGroupsMaster { get; set; }
    public bool IsGroupSlave { get; set; }
    public string DisplayValue { get; set; }


    //following fields not available in Logic Dto
    public bool HasPlaceholder { get; set; }
    public string Placeholder { get; set; }
    public InputSimpleType InputSimpleType { get; set; }
    public string AdditionalClasses { get; set; }
    public IList<string> AcceptFileExtensions { get; set; }
}