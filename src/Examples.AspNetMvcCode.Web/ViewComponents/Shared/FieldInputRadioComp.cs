namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputRadioComp : ViewComponent
{
    public FieldInputRadioComp(
        )
    {
    }


    public async Task<IViewComponentResult> InvokeAsync(FieldViewModel fieldModel)
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.OptionsRadio)
        {
            throw new ArgumentException($"wrong field type: requires '{FieldType.OptionsRadio}'; provided '{fieldModel.FieldType}' ");
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputRadio, fieldModel)
               ).ConfigureAwait(false);
    }
}