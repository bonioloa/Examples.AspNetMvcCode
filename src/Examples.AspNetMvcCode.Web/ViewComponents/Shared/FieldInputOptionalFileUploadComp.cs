namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputOptionalFileUploadComp : ViewComponent
{
    private readonly IHtmlMainLocalizer _htmlLocalizer;

    public FieldInputOptionalFileUploadComp(
        IHtmlMainLocalizer htmlLocalizer
        )
    {
        _htmlLocalizer = htmlLocalizer;
    }


    public async Task<IViewComponentResult> InvokeAsync(FieldViewModel fieldModel)
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        /*add field for simple optional file upload
          everything stubbed because this field must be available on all editable form pages and steps,
          but it must not be saved as field on dynamic part of form (as happens in initial step form) 
          , just its attachments if provided
         */
        FieldViewModel model =
            new()
            {
                //show a radio requesting optional attachment with "NO" preselected
                FieldType = FieldType.OptionsRadio,
                FieldName = fieldModel.FieldName,
                Description = _htmlLocalizer[nameof(HtmlLocalization.SharedDefaultConditionalUploadAttachQuestion)],

                Options =
                    new List<OptionViewModel>()
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
                            Value = AppConstants.OptionFileAttachmentPrefix,
                            Selected =  false
                        },
                    }
            };


        return
            await Task.FromResult<IViewComponentResult>(
               View(MvcComponents.SharedViewCompFieldInputOptionalFileUpload, model)
               ).ConfigureAwait(false);
    }
}