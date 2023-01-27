namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputCheckComp : ViewComponent
{
    public FieldInputCheckComp(
        )
    {
    }


    public async Task<IViewComponentResult> InvokeAsync(FieldViewModel fieldModel)
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.OptionsCheckBox)
        {
            throw new PmWebException($"wrong field type: requires '{FieldType.OptionsCheckBox}'; provided '{fieldModel.FieldType}' ");
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputCheck, fieldModel)
               ).ConfigureAwait(false);
    }
}