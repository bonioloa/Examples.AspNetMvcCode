namespace Examples.AspNetMvcCode.Logic;

/// <summary>
/// represents a complete field with value, attachment and recursive calculation flow
/// </summary>
/// <param name="Progressive"></param>
/// <param name="SectionIndex"></param>
/// <param name="FieldName"></param>
/// <param name="FieldType"></param>
/// <param name="IsMandatory"></param>
/// <param name="IsReadOnly">display field as non editable but it will be submittable, if it's a input type</param>
/// <param name="IsDisabled">display field as non editable and not submittable</param>
/// <param name="Description"></param>
/// <param name="CharactersLimit"></param>
/// <param name="DecimalPositionsForFormatting"></param>
/// <param name="CssClass"></param>
/// <param name="PrefixHtml"></param>
/// <param name="SuffixHtml"></param>
/// <param name="Rule"></param>
/// <param name="DependingField"></param>
/// <param name="RelatedFieldHtmlLabel"></param>
/// <param name="ShowOnReports"></param>
/// <param name="OptionsGroupCode"></param>
/// <param name="CalculationType"></param>
/// <param name="Calculation"></param>
/// <param name="CalculationExecutionOrder"></param>
/// <param name="Options"></param>
/// <param name="Value"></param>
/// <param name="Attachments"></param>
/// <param name="DisplayValue"></param>
/// <param name="UseFixedLabel">for practicality this value is stored in every field, instead of wrapping father Logic DTO</param>
/// <param name="HideMandatorySymbol"></param>
/// <param name="ChoicesUseOnlyImagesAsDescription"></param>
/// <param name="ChoicesOnSameLine"></param>
/// <param name="GroupId"></param>
/// <param name="IsGroupsMaster"></param>
/// <param name="IsGroupSlave"></param>
public record FieldLgc(
    long Progressive
    , int SectionIndex
    , string FieldName
    , FieldType FieldType
    , bool IsMandatory
    , bool IsReadOnly
    , bool IsDisabled
    , IHtmlContent Description
    , long CharactersLimit
    , long DecimalPositionsForFormatting
    , string CssClass
    , IHtmlContent PrefixHtml
    , IHtmlContent SuffixHtml
    , DependencyRule Rule
    , string DependingField
    , IHtmlContent RelatedFieldHtmlLabel
    , bool ShowOnReports
    , string OptionsGroupCode
    , CalculationType CalculationType
    , CalculationLgc Calculation
    , int CalculationExecutionOrder
    , List<OptionLgc> Options
    , string Value
    , List<FileAttachmentLgc> Attachments

    //following fields not available in DATA but present in VIEWMODEL
    , HashSet<OptionLocalizedLgc> OptionLocalizedSet
    , string DisplayValue
    , bool HideMandatorySymbol
    , bool ChoicesUseOnlyImagesAsDescription
    , long ChoicesOnSameLine
    , long GroupId
    , bool IsGroupsMaster
    , bool IsGroupSlave
    );