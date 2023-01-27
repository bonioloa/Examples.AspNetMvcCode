namespace Comunica.ProcessManager.Web.ViewComponents;

public class InputOptionalFileUploadViewComponent : ViewComponent
{
    private readonly HtmlMainLocalizer _htmlLocalizer;

    public InputOptionalFileUploadViewComponent(
        HtmlMainLocalizer htmlLocalizer
        )
    {
        _htmlLocalizer = htmlLocalizer;
    }

    public async Task<IViewComponentResult> InvokeAsync(InputControlViewModel inputModel)
    {
        /*add control for simple optional file upload
          everything stubbed because this field must be available on all editable form pages and steps,
          but it must not be saved as field on dynamic part of form (as happens in initial step form) 
          , just its attachments if provided
         */
        InputControlViewModel model = new()
        {
            //show a radio requesting optional attachment with "NO" preselected
            ControlType = FormControlType.OptionsRadio,
            FieldName = inputModel.FieldName,
            Description = _htmlLocalizer[nameof(HtmlLocalization.SharedDefaultConditionalUploadAttachQuestion)],
            ChoiceOptions = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                    {
                        Description = _htmlLocalizer[nameof(HtmlLocalization.SharedLabelNegative)],
                        Value = "NO",
                        Selected = true
                    },
                    new OptionViewModel()
                    {
                        Description =_htmlLocalizer[nameof(HtmlLocalization.SharedLabelPositive)],
                        Value = AppConstants.FileAttachmentIdChoiceRoot,
                        Selected =  false
                    },
                }
        };

        return await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompInputOptionalFileUpload, model)
               ).ConfigureAwait(false);
    }
}
