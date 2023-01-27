//dependencies
//jquery
//jquery-validate
//immutable.js
//conditionize-steps.js
//site.js
//form-dynamic.js
//SharedConstFormStepperPage - page view

$(function () {
    $('.conditional').conditionize();
    $('[data-cond-option="step1"]').show();
    $("input:text:visible, textarea:visible").first().focus();


    $("#finalize-form-step").on("click", function (e) {
        e.preventDefault();

        //show confirmation modal only if allowed by configuration
        if (SharedConstFormStepperPage.get('IsDraftMode')) {

            modalConfirm(
                SharedConstFormStepperPage.get('ConfirmStepMessage')
                , submitFormDynamic
                , SharedConstFormStepperPage.get('ActionUpdateAndPhaseAdvance')
                , SharedConstFormStepperPage.get('ReValidateBeforePost')
                );
        }
        else {
            submitFormDynamic(SharedConstFormStepperPage.get('ActionUpdateAndPhaseAdvance'));
        }
    });
});