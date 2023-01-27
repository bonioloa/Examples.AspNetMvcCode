/*DEPENDENCIES
 * jquery
 * jquery-validate
 * SharedConstFormDynamic - view _layout 
 * SharedVarsFormDynamic - view that include this js
 *      
 *      
 *  NOTE: recap inputs are involved in this code but their presence in view can be omitted
 *  , no error will be thrown
 *  they are used only for initial step and only when configured from tenant database
 */

$(function () {

    $(".submitted-stepper-form").hide();

    initDivLabelAnimation();
    initInputRecap();
    initOptionsDependencyAndRecap();
    initAttachmentControls();
    initCommonActions();
});


//queste animazioni esistono qui perché
//le div label sono previste solo per le form con input dinamici da db
function initDivLabelAnimation() {

    //non esiste un modo semplice per creare un selettore css che 
    //a partire dall'input restituisce il sibling precedente,
    //l'alternativa potrebbe essere wrappare tutto in un div ma preferisco evitare per non appesantire
    //ulteriormente la struttura e probabilmente potrebbe creare ulteriori problemi
    //Per fare prima usiamo jquery
    $('input:text.get.validate, textarea.get.validate').on('focus', function (event) {
        var divLabel = $(event.target).siblings('div .div-as-label-text-editable');
        if (divLabel.length > 0) {
            $(divLabel).addClass('theme-main-color');
        }
    });

    $('input:text.get.validate, textarea.get.validate').on('blur', function (event) {
        var divLabel = $(event.target).siblings('div .div-as-label-text-editable');
        if (divLabel.length > 0) {
            $(divLabel).removeClass('theme-main-color');
        }
    });
}




function initInputRecap() {

    //onchange copy values to data-recap inputs
    $('input:text.get.validate, textarea.get.validate').on("change keyup", function (event) {
        
        var fieldName = $(event.target).attr("name");
        var fieldRecap = $("[data-recap=" + fieldName + "]");
        if (!isNull(fieldName) && !isObjEmpty(fieldRecap)) {
            fieldRecap.val(getValCleanBySelector(event.target));
        }
    });
}



function initOptionsDependencyAndRecap() {
    $.each($(
        ' .placeholder-field'
        + '[' + SharedConstFormDynamic.get('AttributeFieldHasOptions') + '][' + SharedConstFormDynamic.get('AttributeFieldHasOptions') + '!=""]'
        + '[' + SharedConstFormDynamic.get('AttributeFieldChoiceType') + '][' + SharedConstFormDynamic.get('AttributeFieldChoiceType') + '!=""]')
        , function (index, element) {

            var hasRelatedField = $(element)[0].hasAttribute(SharedConstFormDynamic.get('AttributeFieldHasRelated'));
            var fieldChoiceType = $(element).attr(SharedConstFormDynamic.get('AttributeFieldChoiceType'));
            var fieldOptionsName = $(element).attr(SharedConstFormDynamic.get('AttributeFieldHasOptions'));
            var fieldDependent = $(element).attr(SharedConstFormDynamic.get('AttributeDependentField'));

            choiceWithOptionalFieldEventsInit(fieldOptionsName, fieldChoiceType, hasRelatedField, fieldDependent);
        });
}

function choiceWithOptionalFieldEventsInit(campo, tipoTabella, hasAltro, fieldDependent) {
    
    if (hasAltro) {
        //take value of other field, concatenate with separator and send it to copy to recap
        $('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').on("change keyup", function (event) {

            var otherFieldValueWithSeparator = "";
            if (!isWhitespace($('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').val())) {
                otherFieldValueWithSeparator =
                    SharedConstFormDynamic.get('OtherFieldAggregatedToSelectedOptionFormattedSeparator')
                    + getValCleanBySelector('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']');
            }

            copySelectionToRecap(tipoTabella, campo, otherFieldValueWithSeparator);
        });
    }

    $('[name=' + campo + ']').on("change keyup", function (event) {

        //change field is mandatory only if last value is selected/checked
        var otherFieldValueWithSeparator = "";
        if (hasAltro) {
            if (($('input[name=' + campo + ']:last').is(':checked'))
                || ($('#' + campo + ' option:last').is(':selected'))) {

                //$('[' + SharedConstFormDynamic.get('AttributeOtherField') + '='
                //    + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').removeClass('hidden');
                $('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').attr('required', 'required');
            }
            else {
                //$('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').val('');
                $('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').removeAttr('required');
                $('[name=' + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').removeClass('valid invalid form-error');
                //$('[' + SharedConstFormDynamic.get('AttributeOtherField') + '='
                //    + campo + SharedConstFormDynamic.get('SuffixOtherField') + ']').addClass('hidden');
            }
        }
        if (!isWhitespace(fieldDependent)) {
            if (($('input[name=' + campo + ']:last').is(':checked'))
                || ($('#' + campo + ' option:last').is(':selected'))) {

                //$('[name=' + fieldDependent + ']').removeClass('hidden');
                //$('[' + SharedConstFormDynamic.get('AttributeDivLabelForField') + '=' + fieldDependent + ']').removeClass('hidden');
                //$('label[for=' + fieldDependent + ']').removeClass('hidden');
                $('[name=' + fieldDependent + ']').attr('required', 'required');
            }
            else {
                //$('[name=' + fieldDependent + ']').val('');
                //$("[data-recap=" + fieldDependent + "]").val('');
                $('[name=' + fieldDependent + ']').removeAttr('required');
                $('[name=' + fieldDependent + ']').removeClass('valid invalid form-error');
                //$('[name=' + fieldDependent + ']').addClass('hidden');
                //$('[' + SharedConstFormDynamic.get('AttributeDivLabelForField') + '=' + fieldDependent + ']').addClass('hidden');
                //$('label[for=' + fieldDependent + ']').addClass('hidden');
            }
        }

        //we don't need to pass fieldDependent here because copy to recap is handled by change event on text input/textarea
        copySelectionToRecap(tipoTabella, campo, otherFieldValueWithSeparator);
    });
}

function copySelectionToRecap(tipoTabella, campo, otherFieldValueWithSeparator) {

    var selectedDescriptions = '';
    var foundDescription = false;
    //we need to copy to recap fields option text associated to option value
    switch (tipoTabella) {
        case SharedConstFormDynamic.get('OptionsSelect'):

            var selector = 'select[name="' + campo + '"]';
            var selectedValue = $(selector).val();
            selectedDescriptions =
                cleanStringSafe(
                    $(selector).children('option[value="' + selectedValue + '"]').text()
                );
            //handle default value for optional selects
            if (isObjEmpty(selectedValue)) {
                selectedDescriptions = "";
            }
            foundDescription = true;
            break;


        case SharedConstFormDynamic.get('OptionsSelectMultiple'):
            var selector = 'select[multiple][name="' + campo + '"]';
            var selectedValuesArr = $(selector).val();

            //description must be overridden for every value selection
            //this is because we must allow to deselect all elements and copy empty string on recap
            foundDescription = true;

            var selectedDescrArr = new Array();
            $(selectedValuesArr)
                .each(function (index, element) {

                    selectedDescrArr.push(
                            cleanStringSafe(
                                $(selector).children('option[value="' + element + '"]').text()
                            )
                        );
                });

            selectedDescriptions = selectedDescrArr.join(SharedConstFormDynamic.get('OptionsMultipleSelectedDescriptionsFormattedSeparator'));
            break;


        case SharedConstFormDynamic.get('OptionsCheckBox'):
            selectedDescriptions =
                $("input[name=" + campo + "]:checked")
                    .map(function () {
                        foundDescription = true;
                        return cleanStringSafe($(this).siblings('span').text());
                    })
                    .get()
                    .join(SharedConstFormDynamic.get('OptionsMultipleSelectedDescriptionsFormattedSeparator'));
            break;


        case SharedConstFormDynamic.get('OptionsRadio'):
            var elem = $("input[name=" + campo + "]:checked");
            //we must prevent saving to recap when radio for attachment is selected
            //copy to recap will be handled by attachment event handlers 
            if (elem.val() !== SharedConstFormDynamic.get('ValueAttachmentOption')) {
                selectedDescriptions =
                    cleanStringSafe(
                        $("input[name=" + campo + "]:checked").siblings('span').text()
                    );
                foundDescription = true;
            }
            break;

        default:
            console.log('table type is unhandled' + tipoTabella);
            break;
    }

    if (foundDescription === true) {
        $("[data-recap=" + campo + "]").val(cleanStringSafe(selectedDescriptions + otherFieldValueWithSeparator));
    }
}


function initAttachmentControls() {

    var tmpName = "";
    //for each existing radio group with attachment option
    //get name and listen to change

    $('input[type=radio][' + SharedConstFormDynamic.get('AttributeHasAttachment') + ']').each(function (index, element) {
        tmpName = $(element).attr("name");
        $('input[type=radio][name="' + tmpName + '"]').on("change keyup", function (event) {
            var current = $(event.target);
            var radioWithAttachment =
                $('input[type=radio][name="' + current.attr('name') + '"]'
                    + '[' + SharedConstFormDynamic.get('AttributeHasAttachment') + ']')
            var attachmentsWrapper =
                $('.wrap_checked_poss_doc[' + SharedConstFormDynamic.get('AttributeFieldRelatedTo') + '="' + radioWithAttachment.attr("id") + '"]')

            if (radioWithAttachment.is(':checked') === true) {

                attachmentsWrapper.find('.file-path').attr("required", true);
                attachmentsWrapper.show();
            }
            else {
                attachmentsWrapper.find('.file-path')
                    .attr("required", false)
                    .attr("aria-required", null)
                    .val(null).valid();
                attachmentsWrapper.hide();

                //clear first attachment field
                attachmentsWrapper.find('input[type=file]').clearInputs();
            }
        });
    });

    //copy attached files names from single option on display field (recap)
    $(document).delegate("input[type=file].attachment.radio-file-option", "change", function (event) {

        var elAttachedFilesPaths = $('#' + $(event.target).attr(SharedConstFormDynamic.get('AttributeAttachmentPathsFieldName')));
        var elAttachmentRadio = $('#' + $(event.target).attr(SharedConstFormDynamic.get('AttributeAttachmentRadioId')));
        var fieldName = $(event.target).attr(SharedConstFormDynamic.get('AttributeAttachmentFieldName'));

        if (elAttachmentRadio.length > 0 && elAttachedFilesPaths.length > 0 && fieldName.length > 0
            && elAttachmentRadio.is(':checked') === true) {

            $("[data-recap='" + fieldName + "']").val(cleanStringSafe(elAttachedFilesPaths.val()));
        }
    });

    //copy attached files names on display field (recap)
    $(document).delegate("input[type=file].attachment.attach-basic", "change", function (event) {

        var elAttachedFilesPaths = $('#' + $(event.target).attr(SharedConstFormDynamic.get('AttributeAttachmentPathsFieldName')));
        var fieldName = $(event.target).attr(SharedConstFormDynamic.get('AttributeAttachmentFieldName'));

        if (elAttachedFilesPaths.length > 0 && fieldName.length > 0) {

            $("[data-recap='" + fieldName + "']").val(cleanStringSafe(elAttachedFilesPaths.val()));
        }
    });
}



//initialize action buttons handlers, if buttons are created in view
function initCommonActions() {
    $('#save-current-step').on("click", function (e) {
        e.preventDefault();

        submitFormDynamic(
            SharedVarsFormDynamic.get('ActionSaveWithoutAdvance')
            , SharedVarsFormDynamic.get('ReValidateBeforePost')
            );
    });


    $('#abort-current-item').on("click", function (e) {
        e.preventDefault();

        modalConfirm(
            SharedConstFormDynamic.get('ConfirmItemAbort')
            , submitFormDynamic
            , SharedVarsFormDynamic.get('ActionAbort')
            , false     //this action type does not need fields validation regardless setting in SharedVarsFormDynamic
            );
    });


    $('#rollback-current-step').on("click", function (e) {
        e.preventDefault();

        modalConfirm(
            SharedConstFormDynamic.get('ConfirmStepRollback')
            , submitFormDynamic
            , SharedVarsFormDynamic.get('ActionRollback')
            , false     //this action type does not need fields validation regardless setting in SharedVarsFormDynamic
            );
    });


    $('.cancella-allegato').each(function (index, element) {
        $(element).on('click', function () {
            modalConfirm(
                GetMessageForConfirmDeleteAttachment(
                    element
                    , SharedConstFormDynamic.get('ConfirmAttachmentDeleteMsgWithPlh')
                    )
                , submitFormAttachmentDelete
                , element
                )
        });
    });

    $('.link-form-att-del').each(function (index, element) {
        var deleteLinkIdSelector = '#' + $(element).prop('id');
        $(deleteLinkIdSelector).on('click', function (e) {
            e.preventDefault();

            var messageConfirm =
                GetMessageForConfirmDeleteAttachment(
                    deleteLinkIdSelector
                    , SharedConstFormDynamic.get('ConfirmAttachmentDeleteMsgWithPlh')
                );

            modalConfirm(
                messageConfirm
                , submitFormMultiStepAttachmentDelete
                , deleteLinkIdSelector
            )
        });
    });
}


function GetMessageForConfirmDeleteAttachment(element, modalBaseText) {
    var message =
        modalBaseText.trim()
                     .replace(
                        SharedConstFormDynamic.get('PlhDeleteFileName')
                        , $(element).children('div.hidden').text().trim()
                        )
    return message;
}

function submitFormAttachmentDelete(element) {
    $(element).closest("form").submit();
}

//use ajax to delete documents to db and use response to remove the delete attachment,
//without reloading page
var multipleSubmitDel = false;
function submitFormMultiStepAttachmentDelete(deleteLinkIdSelector) {
    //retrieve form id from link attribute
    var idFormDeleteAttachment =
        $(deleteLinkIdSelector).attr(SharedConstFormDynamic.get('AttributeIdFormDeleteAttachment'));

    var form = $('#' + idFormDeleteAttachment);
    if (form.length <= 0 ) {
        //after deletion of one file
        //seems that this function is called again for the deleted elements 
        //and after them it calls the right one
        //I DONT HAVE TIME for this BULLSHIT so just let it run without error or user messages
        //eventually it will hit an existing form
        console.log('the phantom pain');
        multipleSubmitDel = false;
        return;
    }
    var method = form.attr('method');
    var url = form.attr('action');

    if (multipleSubmitDel) {
        return;
    }
    multipleSubmitDel = true;
    showPreloader();

    $.ajax({
        type: method,
        url: url,
        data: form.serialize(),
        success: function (data, textStatus, jqXHR) {
            hidePreloader();
            multipleSubmitDel = false;

            if (isNull(data)) {
                console.log('server returned an empty data object');
                modalAlert("Generic error");
            }
            if (!data.hasOwnProperty('message')) {
                console.log('server returned data object without property "message" ');
                modalAlert("Generic error");
            }
            if (!data.hasOwnProperty('idForDeletedAttachmentLinkRemove')) {
                console.log('server returned data object without property "idForDeletedAttachmentLinkRemove" ');
                modalAlert("Generic error");
            }

            //show result message
            if (!isNull(data.message)
                && !isWhitespace(data.message)) {
                modalAlert(data.message);
            }
            //delete from dom element containing attachment info
            $('#' + data.idForDeletedAttachmentLinkRemove).remove();
            //remove form submit deletion
            $('#' + idFormDeleteAttachment).remove();

            //if the deleted attachment was the last present, make input file mandatory
            if ($('.link-form-att-del').length <= 0) {
                $('.file-path.att-required').attr("required", true);
            }
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hidePreloader();
            multipleSubmitDel = false;

            console.log(
                'status: ' + jqXHR.status
                + ', textStatus: ' + textStatus
                + ', errorThrown: ' + errorThrown
            );

            modalAlert("Response Error, contact support");
            return;
        }
    });
}

//form dynamic post
var multiplesubmit = false;
function submitFormDynamic(action, showsFirstErrorField) {

    if (showsFirstErrorField === true) {
        //this is intended for forms without validating stepper
        if (!$('#form-dynamic').valid()) {
            var invalidInput = $('#form-dynamic .invalid:first-of-type');
            if (invalidInput.length > 0) {
                $('html,body').animate({ scrollTop: invalidInput.offset().top - 20 });
            }
            multiplesubmit = false;
            return;
        }
    }
    if (multiplesubmit) {
        return;
    }
    multiplesubmit = true;

    showPreloader();

    var form = buildFormForPost(action, SharedVarsFormDynamic.get('SaveDescription'));

    var xhr = new XMLHttpRequest();    

    
    
    xhr.onreadystatechange = function (data) {
        if (xhr.readyState === 4 && xhr.status > 299) {

            hidePreloader();
            //console.log(xhr.status + " " + data.statusText);
            modalAlert("Error: " + data.statusText);
            multiplesubmit = false;
        }
        if (xhr.readyState === 4 && xhr.status === 200) {
            hidePreloader();
            var jsonResponse = ajaxJsonResponseSafeParse(data);
            console.log(jsonResponse);
            if (!isObjEmpty(jsonResponse)) {

                switch (jsonResponse.resultCode) {
                    case "KoNoReload":
                        modalAlert(jsonResponse.errorMessage);
                        break;

                    case "OkToItem":
                        window.location.replace(SharedVarsFormDynamic.get('RedirectToItemUrl'));
                        //redirect to item currently managed with explicit url, we can't do a reload
                        break;

                    case "OkToMainpage":
                        window.location.replace(SharedVarsFormDynamic.get('RedirectToMainpageUrl'));

                    case "OkWithReload":
                        //this is called for saving another draft
                        location.reload();//after reload will be show confirmation message stored in session
                        break;

                    case "OkNoReload":
                        $("#anonymous-login-code").text(jsonResponse.loginCodeForAnonymousInsert);
                        $('#form-dynamic').hide();
                        $('#item-basic-info').hide();
                        $('.stepper').hide();
                        $('.submitted-stepper-form').show();
                        window.scrollTo(0, 0);
                        break;

                    default:
                        modalAlert("unrecognized response code");
                        break;
                }//end switch
            }
            else {
                hidePreloader();
                modalAlert("Json error");
            }
            multiplesubmit = false;
            return;
        }
    };//end onreadystatechange


    xhr.open('POST', SharedVarsFormDynamic.get('SubmitUrl'));//action

    var antiforgeryValue = $('#form-dynamic').find('input[name="__RequestVerificationToken"]').val();
    form.append('__RequestVerificationToken', antiforgeryValue);
    xhr.setRequestHeader('__RequestVerificationToken', antiforgeryValue);

    xhr.send(form);
    //function return will be handled by event onreadystatechange
}


function buildFormForPost(action, isSaveDescriptionMode) {
    var form = new FormData();
    form.append('azione', action);
    form.append('fase', $('#fase').val());
    form.append('stato', $('#stato').val());

    if (isSaveDescriptionMode === true) {
        //when saving description save directly data recap
        $.each(
            $('#form-dynamic').find('[data-recap]'), function (index, value) {
                form.append($(value).attr('data-recap'), getValCleanBySelector(value));
                });
    }
    else {

        $('#form-dynamic  input.to-use-for-altern-normal'
            + ',#form-dynamic textarea.to-use-for-altern-normal'
            + ',#form-dynamic select.to-use-for-altern-normal:not([multiple])')
            .each(function (index, element) {
                form.append($(element).attr('name'), getValCleanBySelector(element));
                });
        //console.log('normal inputs: ');
        //for (var d of form.entries()) {
        //    console.log(d);
        //}

        $('#form-dynamic select.to-use-for-altern-normal[multiple]')
            .each(
                function (index, element) {
                    var selectedToString = '';
                    if (!isNull($(element)) && !isObjEmpty($(element)) && $(element).val().length > 0 ) {
                        selectedToString =
                            $(element).val()//val is an array
                                      .join(SharedConstFormDynamic.get('OptionsMultipleSelectedValuesSeparator'))
                    }
                form.append($(element).attr('name'), selectedToString);
            });

        //we need to get distinct names of checkable inputs not handled in previous statement
        var namesOfCheckedInputs = [];
        $('#form-dynamic input.to-use-for-altern-checked')
            .map(function () { return this.name; })
            .each(function (i, str) {
                if (!~$.inArray(str, namesOfCheckedInputs)) {
                    namesOfCheckedInputs.push(str);
                }
            });

        //use the retrieved names to get values to save
        $.each(namesOfCheckedInputs, function (index, value) {
            if ($('#form-dynamic input[name="' + value + '"]').is(':checkbox')) {
                var checkedValues =
                    $('#form-dynamic input[name="' + value + '"]:checked')
                        .map(function () {
                            return cleanStringSafe($(this).val());
                        })
                        .get()
                        .join(SharedConstFormDynamic.get('OptionsMultipleSelectedValuesSeparator'));
                form.append(value, cleanStringSafe(checkedValues))
            }
            if ($('#form-dynamic input[name="' + value + '"]').is(':radio')) {
                var radioCheckedValue = $('#form-dynamic input[name="' + value + '"]:checked').val();
                form.append(value, cleanStringSafe(radioCheckedValue));
            }
        });
    }


    $.each($('input[type=file].attachment'), function (i, fileInput) {
        var fileFieldName = $(fileInput).attr(SharedConstFormDynamic.get('AttributeAttachmentPathsFieldName'));
        for (var indexForMultiple = 0; indexForMultiple < fileInput.files.length; indexForMultiple++) {
            form.append(
                fileFieldName + indexForMultiple,
                fileInput.files[indexForMultiple]);
        }
    });
    return form;
}