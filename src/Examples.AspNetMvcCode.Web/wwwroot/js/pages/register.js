//dependencies
//jquery
//jquery validate
//site.js
$(function () {
    $('#Login').rules('add', { required: true, user: true});
    $('#Password').rules('add', { required: true, passwordWb: true });
    $('#ConfirmPassword').rules('add',
        { required: true, passwordConfirmWb: true, equalTo: "#Password" });
    $('#Nome').rules('add', { required: true, nameSurname: true });
    $('#Cognome').rules('add', { required: true, nameSurname: true });
    $('#Email').rules('add', { required: true, email: true });
});