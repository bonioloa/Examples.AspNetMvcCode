namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputNumericComp : ViewComponent
{
    public FieldInputNumericComp(
        )
    {
    }



    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));


        if (fieldModel.FieldType != FieldType.InputNumeric)
        {
            throw new ArgumentException(
                $"wrong field type: requires '{FieldType.InputNumeric}' or '{FieldType.InputDate}'; provided '{fieldModel.FieldType}' "
                );
        }

        //this properties must be overridden to transform a simple text input in a numeric field
        fieldModel.AdditionalClasses = fieldModel.AdditionalClasses + CodeConstants.Space + "input-numeric";
        fieldModel.HasPlaceholder = true;
        fieldModel.Placeholder = "0";
        fieldModel.CharactersLimit = NumericsConstants.NumericMaxLength;


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputNumeric, fieldModel)
               ).ConfigureAwait(false);
    }
}