namespace Comunica.ProcessManager.Web.Models;

public class InputControlViewModel
{
    public string CssClass { get; set; }
    public IHtmlContent PrefixHtml { get; set; }
    public IHtmlContent SuffixHtml { get; set; }
    public int SectionIndex { get; set; }
    public FormControlType ControlType { get; set; }
    public string FieldName { get; set; }
    public bool IsMandatory { get; set; }
    /// <summary>
    /// only used in select control (for now)
    /// </summary>
    public bool IsMultiple { get; set; }
    public IHtmlContent Description { get; set; }
    public long CharactersLimit { get; set; }
    public long Progressive { get; set; }
    public string Value { get; set; }
    public IList<FileAttachmentViewModel> Attachments { get; set; }
    public DependencyRule Rule { get; set; }
    public string DependingField { get; set; }
    public IHtmlContent RelatedFieldHtmlLabel { get; set; }

    public bool UseFixedLabel { get; set; }
    public bool HideMandatorySymbol { get; set; }
    public IList<OptionViewModel> ChoiceOptions { get; set; }
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