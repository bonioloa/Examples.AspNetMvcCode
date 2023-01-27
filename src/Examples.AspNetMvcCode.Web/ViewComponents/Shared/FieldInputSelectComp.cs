namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputSelectComp : ViewComponent
{
    public FieldInputSelectComp(
        )
    {
    }



    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.OptionsSelect
            && fieldModel.FieldType != FieldType.OptionsSelectMultiple)
        {
            throw new ArgumentException(
                $"wrong field type: requires '{FieldType.OptionsSelect}' or '{FieldType.OptionsSelectMultiple}'; provided '{fieldModel.FieldType}' "
                );
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputSelect, fieldModel)
               ).ConfigureAwait(false);
    }
}