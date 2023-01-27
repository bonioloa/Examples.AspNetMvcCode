namespace Examples.AspNetMvcCode.Web.ViewComponents;

/// <summary>
/// can be considered like an extension of text simple
/// </summary>
public class FieldInputDateComp : ViewComponent
{
    private readonly IMainLocalizer _localizer;

    public FieldInputDateComp(
        IMainLocalizer localizer
        )
    {
        _localizer = localizer;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.InputDate)
        {
            throw new PmWebException($"wrong field type: requires '{FieldType.InputDate}'; provided '{fieldModel.FieldType}' ");
        }

        //this properties must be overridden to transform a simple text input in a date picker
        fieldModel.AdditionalClasses = fieldModel.AdditionalClasses + CodeConstants.Space + "datepicker";
        fieldModel.HasPlaceholder = true;
        fieldModel.Placeholder = _localizer[nameof(LocalizedStr.SharedPlaceholderDate)];
        fieldModel.CharactersLimit = _localizer[nameof(LocalizedStr.SharedPlaceholderDate)].ToString().Length;


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputDate, fieldModel)
               ).ConfigureAwait(false);
    }
}