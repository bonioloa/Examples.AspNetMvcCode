namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class FieldInputTextAreaOtherComp : ViewComponent
{

    public FieldInputTextAreaOtherComp(
        )
    {
    }




    public async Task<IViewComponentResult> InvokeAsync(
        FieldViewModel fieldModel
        )
    {
        Guard.Against.Null(fieldModel, nameof(fieldModel));

        if (fieldModel.RelatedFieldHtmlLabel.Invalid())
        {
            return await Task.FromResult<IViewComponentResult>(
                     Content(string.Empty)
                     ).ConfigureAwait(false);
        }


        FieldViewModel model =
            new()
            {
                FieldType = FieldType.InputTextArea,
                InputSimpleType = InputSimpleType.Text,
                FieldName = fieldModel.FieldName + AppConstants.HtmlNameOtherFieldSuffix,
                HideMandatorySymbol = true,
                IsReadOnly = fieldModel.IsReadOnly,//inherited from parent
                IsDisabled = fieldModel.IsDisabled,
                Description = fieldModel.RelatedFieldHtmlLabel,
                CharactersLimit = WebAppConstants.OtherTextAreaMaxLenth,
                UseFixedLabel = true, //always, because it only appears for customer configurable fields
                                      //this must be handled more gracefully
            };

        return
            await Task.FromResult<IViewComponentResult>(
                   View(MvcComponents.SharedViewCompFieldInputTextAreaOther, model)
                   ).ConfigureAwait(false);
    }
}