namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputRecapComp : ViewComponent
{
    public FieldInputRecapComp()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync(FieldViewModel fieldModel)
    {
        //we must not check field type, because this field must be generated for any input type            

        Guard.Against.Null(fieldModel, nameof(fieldModel));

        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputRecap, fieldModel)
               ).ConfigureAwait(false);
    }
}