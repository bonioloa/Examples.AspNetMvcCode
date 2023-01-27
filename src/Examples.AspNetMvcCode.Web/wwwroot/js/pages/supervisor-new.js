//dependencies
//jquery
//jquery validate
//site.js
$(function () {
    $('#Login').rules('add', { required: true, user: true });
    $('#Nome').rules('add', { required: true, nameSurname: true });
    $('#Cognome').rules('add', { required: true, nameSurname: true });
    $('#Email').rules('add', { required: true, email: true });
    $('#ProfiloEsclusivo').rules('add', { required: true });

    setRolesOptional();

    $('[name="ProfiloEsclusivo"]').on("change keyup", function (event) {
        setRolesOptional();
    });
});

function setRolesOptional() {
    var elem = $("input[name='ProfiloEsclusivo']:checked");
    if (!isObjEmpty(elem) && elem.val() === 'None') {
        $('input[name="Ruoli"]').rules('add', { required: true });
        $('.optional-profiles').show();
    }
    else {
        $('input[name="Ruoli"]').rules('remove');
        $('input[name="Ruoli"]').prop('checked', false);//reset selection;
        $('.optional-profiles').hide();
    }
}