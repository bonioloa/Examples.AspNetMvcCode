//dependencies
//jquery
//jquery validate
//site.js
//SharedConstRecoverPage - localization json in view

$(function () {
    $('#inputChangeAnchor').on('click', function (event) {
        displayOtherRecoverType();
    }); 
    showRecoverPassword();
});

function displayOtherRecoverType() {
    if ($('#Login').is(":visible") === true) {showRecoverUser();}
    else {showRecoverPassword();}
}
function showRecoverPassword() {
    $('.simple-panel-title').text(SharedConstRecoverPage.get('UserRecoverCredentialsFormPasswordTitleType'));
    $('#Login').rules('add', { required: true });
    $('#divlogin').show();

    $('#Email').rules('remove');
    $('#Email').val('');
    $('#divemail').hide();

    $('#anchorText').text(SharedConstRecoverPage.get('UserRecoverCredentialsFormUserAnchorType'));   
    return true;
}
function showRecoverUser() {
    $('.simple-panel-title').text(SharedConstRecoverPage.get('UserRecoverCredentialsFormUserTitleType'));
    $('#Login').rules('remove');
    $('#Login').val('');
    $('#divlogin').hide();

    $('#Email').rules('add', { required: true, email:true });
    $('#divemail').show();

    $('#anchorText').text(SharedConstRecoverPage.get('UserRecoverCredentialsFormPasswordAnchorType'));    
    return true;
}