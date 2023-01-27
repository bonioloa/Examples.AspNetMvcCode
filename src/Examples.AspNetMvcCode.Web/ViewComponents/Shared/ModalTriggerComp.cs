namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class ModalTriggerComp : ViewComponent
{
    public ModalTriggerComp(
        )
    {
    }




    public async Task<IViewComponentResult> InvokeAsync(ModalTriggerViewModel model)
    {
        Guard.Against.Null(model, nameof(ModalTriggerViewModel));

        Guard.Against.NullOrWhiteSpace(model.ModalId, nameof(ModalTriggerViewModel.ModalId));

        //do not validate Description because it can be empty for icon trigger


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompModalTrigger, model)
               ).ConfigureAwait(false);
    }
}