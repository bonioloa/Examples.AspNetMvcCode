//dependencies
//jquery
//jquery validate
//site.js
$(function () {
    if (!isObjEmpty($("#form-supervisor-data-edit"))) {
        $('#form-supervisor-data-edit input[name="Nome"]').rules('add', { required: true, nameSurname: true });
        $('#form-supervisor-data-edit input[name="Cognome"]').rules('add', { required: true, nameSurname: true });
        $('#form-supervisor-data-edit input[name="Email"]').rules('add', { required: true, email: true });
        $('#form-supervisor-data-edit input[name="ProfiloEsclusivo"]').rules('add', { required: true });

        setRolesOptional();

        $('#form-supervisor-data-edit input[name="ProfiloEsclusivo"]').on("change keyup", function (event) {
            setRolesOptional();
        });
    }
});


function setRolesOptional() {
    var elem = $("#form-supervisor-data-edit input[name='ProfiloEsclusivo']:checked");
    if (!isObjEmpty(elem) && elem.val() === 'None') {
        $('#form-supervisor-data-edit input[name="Profili"]').rules('add', { required: true });
        $('.optional-profiles').show();
    }
    else {
        $('#form-supervisor-data-edit input[name="Profili"]').rules('remove');
        $('#form-supervisor-data-edit input[name="Profili"]').prop('checked', false);//reset selection
        $('.optional-profiles').hide();
    }
}