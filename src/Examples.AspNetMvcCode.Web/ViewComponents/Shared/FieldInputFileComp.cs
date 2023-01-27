namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputFileComp : ViewComponent
{
    public FieldInputFileComp(
        )
    {
    }


    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.InputMultipleFile)
        {
            throw new PmWebException(
                $"wrong field type: requires '{FieldType.InputMultipleFile}'; provided '{fieldModel.FieldType}' "
                );
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(
                   MvcComponents.SharedViewCompFieldInputFile
                   , fieldModel
                   )
               ).ConfigureAwait(false);
    }
}