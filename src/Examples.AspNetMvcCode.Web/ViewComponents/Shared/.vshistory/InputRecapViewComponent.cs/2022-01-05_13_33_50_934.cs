namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputRecapViewComponent : ViewComponent
{
    public InputRecapViewComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync(InputControlViewModel inputModel)
    {
        //we must not check control type, because this control must be generated for any input type            

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputRecap, inputModel)
               ).ConfigureAwait(false);
    }
}
