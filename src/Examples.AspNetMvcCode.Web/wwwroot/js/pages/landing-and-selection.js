//dependencies
//jquery
//jquery-validate
//SharedConstStartNew - view ProcessHub

//this is an approximate height between 'vetrino' border and bottom of official wb images
var backImgAdditionalHeight = 100;

$(function () {
    resizeBackground();
    initProcessSelection();    
});

$(window).on('resize', function () {
    $('.official-background').height($('section.main-area').height() + backImgAdditionalHeight);
});



function resizeBackground() {
    var adjustedHeight = $('section.main-area').height() + backImgAdditionalHeight;
    $('.official-background').height(adjustedHeight);
}

function initProcessSelection() {

    var hasMasterGroup = false;
    var groupInputSelector = '[name="' + SharedConstStartNew.get('groupInputName') + '"]';

    if ($(groupInputSelector).length > 0) {

        hasMasterGroup = true;
        $(groupInputSelector).rules('add', { required: true });
        $('[' + SharedConstStartNew.get('groupSlaveAttribute') + ']').hide();

        $(groupInputSelector).on("change", function (event) {
            var current = $(event.target);

            var groupSlaveAttribute = SharedConstStartNew.get('groupSlaveAttribute');
            var selectedGroupSelector = '[' + groupSlaveAttribute + '="' + current.attr('value') + '"]';
            $(selectedGroupSelector).show();

            var requiredInputs = $(selectedGroupSelector + ' label input');
            requiredInputs.prop('checked', false);
            requiredInputs.each(function () { $(this).rules('add', { required: true }); });

            var groupsNotSelectedSelector =
                '[' + groupSlaveAttribute + ']'
                + '[' + groupSlaveAttribute + '!="' + current.attr('value') + '"]';
            var inputsToReset = $(groupsNotSelectedSelector + ' label input');
            inputsToReset.prop('checked', false);
            inputsToReset.each(function () { $(this).rules('remove', 'required') });

            $(groupsNotSelectedSelector).hide();

            initializeValidate();
        });
    }
    if (!hasMasterGroup) {
        $('input[type="radio"].process-selection-input').each(function () { $(this).rules('add', { required: true }) })
    }
}