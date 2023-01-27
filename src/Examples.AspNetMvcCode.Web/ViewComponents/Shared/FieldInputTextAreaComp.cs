namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputTextAreaComp : ViewComponent
{
    public FieldInputTextAreaComp(
        )
    {
    }




    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.InputTextArea)
        {
            throw new ArgumentException($"wrong field type: requires '{FieldType.InputTextArea}'; provided '{fieldModel.FieldType}' ");
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputTextArea, fieldModel)
               ).ConfigureAwait(false);
    }
}