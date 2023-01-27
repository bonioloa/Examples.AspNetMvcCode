namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputTextSimpleComp : ViewComponent
{
    public FieldInputTextSimpleComp(
        )
    {
    }



    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.FieldType != FieldType.InputTextSimple
            && fieldModel.FieldType != FieldType.InputDate
            && fieldModel.FieldType != FieldType.InputNumeric)
        {
            throw new ArgumentException(
                @$"wrong field type: requires '{FieldType.InputTextSimple}' or '{FieldType.InputDate}' or '{FieldType.InputNumeric}'; 
                provided '{fieldModel.FieldType}' "
                );
        }


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputTextSimple, fieldModel)
               ).ConfigureAwait(false);
    }
}