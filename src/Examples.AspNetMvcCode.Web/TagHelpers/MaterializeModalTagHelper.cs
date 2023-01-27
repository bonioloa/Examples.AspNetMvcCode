namespace Examples.AspNetMvcCode.Web.TagHelpers;

/// <summary>
/// this helper creates html required to render a materialize modal, around child content
/// </summary>
[HtmlTargetElement("materialize-modal", Attributes = "id")]
public class MaterializeModalTagHelper : TagHelper
{
    /// <summary>
    /// The Id of the modal. Required
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The title of the modal. Optional
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// class to use for initialization and behaviour. Default is a modal that should be opened with a link
    /// </summary>
    public string ClassForBehaviour { get; set; } = "modal-link-triggered";

    public string FooterButtonOkText { get; set; }
    public string FooterButtonCancelText { get; set; }

    /// <summary>
    /// necessary for not dismissible modals.
    /// </summary>
    //public bool UseClosingLink { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        TagHelperContent childContent =
            await output.GetChildContentAsync().ConfigureAwait(false);

        string template =
            $@"<div id='{Id}' class='modal {ClassForBehaviour}'>
                        <div class='modal-content left-align'>";


        string titleTemplate = string.Empty;

        if (Title.StringHasValue())
        {
            //string tmpTitle = Title.Invalid() ? "" : Title;
            //string tmpClosingLink = 
            //    UseClosingLink ? 
            //    $@"<span class='right valign-middle modal-close modal-close-symbol strong'
            //        >&times;</span>" 
            //    : "";

            //this will strip any html tag from title text to prevent any possible XSS vulnerability.
            //unfortunately it will also strip particular mathematical expression (ex " <3 and >0" )
            string sanitizedTitle = Title.HtmlInputSanitize();

            titleTemplate = $@"
                <div class='row row-no-line-after'>
                    <div class='col xl12 l12 m12 s12'>
                         <h3 class='modal-title left'>{sanitizedTitle}</h3>
                    </div>
                </div>";
        }


        string buttonsSectionTemplate = string.Empty;

        if (ClassForBehaviour == WebAppConstants.HtmlClassModalWarning
            || ClassForBehaviour == WebAppConstants.HtmlClassModalAlert
            || ClassForBehaviour == WebAppConstants.HtmlClassModalConfirm)
        {
            string dismissButton = string.Empty;
            //we need to set a different id for ok button to allow alert and confirm modal to coexist
            string okButtonId = "alert-ok";
            if (ClassForBehaviour == WebAppConstants.HtmlClassModalConfirm)
            {
                dismissButton = $@"
                        <a href='#'
                           id='confirm-dismiss'
                           class='left modal-close {WebAppConstants.HtmlClassButton} {WebAppConstants.HtmlClassButtonDefault}'
                           >{FooterButtonCancelText}</a>";
                //button text here is safe, because it's configured by us un database
                okButtonId = "confirm-ok";
            }


            //we don't use modal-footer class, seems only to create only problems for layout 
            buttonsSectionTemplate = @$"
                    <div class='row row-no-line-after'>
                        <div class='col xl12 l12 m12 s12'>
                            {dismissButton}
                            <a id='{okButtonId}'
                               href='#'
                               class='right modal-close {WebAppConstants.HtmlClassButton} {WebAppConstants.HtmlClassButtonPrimary}'
                               >{FooterButtonOkText}</a>
                        </div>
                    </div>";//button text here is safe, because it's configured by us un database
        }

        output.Content.AppendHtml(template);//also opens modal div and modal-content div
        output.Content.AppendHtml(titleTemplate);
        output.Content.AppendHtml(childContent);
        output.Content.AppendHtml(buttonsSectionTemplate);//we are keeping it inside modal content for alignment
        output.Content.AppendHtml("</div>");//close modal-content div            
        output.Content.AppendHtml("</div>");//close modal div
    }
}