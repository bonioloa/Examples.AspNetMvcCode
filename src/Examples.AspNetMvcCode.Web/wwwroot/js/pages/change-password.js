//dependencies
//jquery
//jquery validate
//site.js
$(function () {
    $('#OldPassword').rules('add', { required: true }); 
    $('#NewPassword').rules('add', { required: true, passwordWb: true });
    $('#ConfirmPassword').rules('add',
        { required: true, passwordConfirmWb: true, equalTo: "#NewPassword" });
});