//dependencies
//jquery
//jquery-validate
//immutable.js
//site.js
//form-dynamic.js
//SharedConstSite - view _layout
//SharedConstItemManagementPage - localization json in view

$(function () {

    //disable jump actions if permission inclusion is mandatory
    if (isObjNotEmpty($('#permission-inclusion-mandatory'))
        && $('#permission-inclusion-mandatory').val() === 'true') {

        var actionNextButton = $("#" + SharedConstItemManagementPage.get('IdNext'));
        if (isObjNotEmpty(actionNextButton)) {
            actionNextButton.attr("disabled", "disabled");
        }

        var actionJumpAlternativeButton = $("#" + SharedConstItemManagementPage.get('IdAlternative'));
        if (isObjNotEmpty(actionJumpAlternativeButton)) {
            actionJumpAlternativeButton.attr("disabled", "disabled");
        }
    }

    //#region event handling for optional form submit fields
    $("#" + SharedConstItemManagementPage.get('IdNext')).on("click", function (e) {
        e.preventDefault();

        modalConfirm(
            GetMessageForConfirmActionItem(
                SharedConstItemManagementPage.get('IdNext')
                , SharedConstItemManagementPage.get('ConfirmAdvanceStepMsgWithPlh')
                )
            , submitFormDynamic
            , SharedConstItemManagementPage.get('ActionUpdateAndPhaseAdvance')
            , SharedConstItemManagementPage.get('ReValidateBeforePost')
        );
    });

    $("#" + SharedConstItemManagementPage.get('IdAlternative')).on("click", function (e) { 
        e.preventDefault();

        modalConfirm(
            GetMessageForConfirmActionItem(
                SharedConstItemManagementPage.get('IdAlternative')
                , SharedConstItemManagementPage.get('ConfirmAdvanceStepMsgWithPlh')
                )
            , submitFormDynamic
            , SharedConstItemManagementPage.get('ActionUpdateAndAlternativeAdvance')
            , SharedConstItemManagementPage.get('ReValidateBeforePost')
        );
    });
    //#endregion



    //#region event handling for optional elements

    $('[name="' + SharedConstItemManagementPage.get('NameAssignPermission')+'"]').on("change", function (event) {
        if ($('[name="' + SharedConstItemManagementPage.get('NameAssignPermission') +'"]').not(':disabled').is(':checked') === true) {
            $('#' + SharedConstItemManagementPage.get('IdSubmitIncludeGroups')).prop("disabled", false);
        }
        else {
            $('#' + SharedConstItemManagementPage.get('IdSubmitIncludeGroups')).prop("disabled", true);
        }
    });


    //bind opening event to messages popups
    if ($('#countMessages').length > 0) {
        var countMessages = $('#countMessages').val();
        if ($.isNumeric(countMessages) && countMessages > 0) {
            var progressiveSelector = "";
            for (var index; index < countMessages; index++) {
                progressiveSelector = '#modal-msg' + index;

                $(progressiveSelector).on('click', function (event) {
                    showhide(progressiveSelector);
                })
            }
        }
    }

    //#endregion
});

function GetMessageForConfirmActionItem(btnId, modalBaseText) {
    return modalBaseText.trim().replace(SharedConstItemManagementPage.get('PlhActionText'), $("#" + btnId).text().trim());
}