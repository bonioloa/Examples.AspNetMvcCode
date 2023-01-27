//dependencies
//jquery
//jquery-validate
//materialize

$(function () {
    if ($('.form-anon-existing').length > 0) {

        $('.tabs').tabs();

        $('#loginCode').rules('add', { loginCodeWb: true });
    }
    else {
        $('#userLogin').focus();
    }
});