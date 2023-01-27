//dependencies
//jquery
//jquery-validate
//materialize
//recaptcha/api.js
//SharedConstSite - view _layout

$(function () {

    if ($('#' + SharedConstSite.get('TenantToken')).length <= 0) {
        $('#page-wrapper').removeClass('hidden');
    }
    initModals();
    initMatTooltips();
    initMatDropdowns();

    initializeSelects();
    initInputControls();

    initBannerPolicies();

    initPreloaderForSimpleLinks();

    setupJqueryValidate();

    hidePreloader();
});

$(window).on('resize', function () {
    fixLabels();
    fixTextArea();
});


function fixLabels() {
    //prevent label overlapping with value
    setTimeout(function () { M.updateTextFields() }, 0);
}
function fixTextArea() {
    //correct textareas resizing after changing values with js
    setTimeout(function () {
        $('textarea.get').each(function () {
            M.textareaAutoResize(this);
        });
    }, 0);
}


function initModals() {
    M.Modal.init($('.modal.modal-link-triggered'), {
        onOpenEnd: fixBodyOnModalOpening,
        onCloseEnd: fixBodyOnModalClosing,
        preventScrolling: false,
    });

    M.Modal.init($('.modal.modal-alert, .modal.modal-confirm'), {
        onOpenEnd: fixBodyOnModalOpening,
        onCloseEnd: fixBodyOnModalClosing,
        preventScrolling: false,
        dismissible: false,
    });

    M.Modal.init($('.modal.modal-warning'), {
        onOpenEnd: fixBodyOnModalOpening,
        onCloseEnd: reloadOnModalClosing,
        preventScrolling: false,
        dismissible: false,
    });
   

    //When the user clicks on (x), close the modal.
    //This modal should not be closed on generic click, 
    //because we need user to be able to copy message
    if ($('#warningModal').length > 0) {
        var instance = M.Modal.getInstance($('#warningModal'));
        instance.open();
    }
}

function fixBodyOnModalOpening() {
    $('body').removeAttr('style');
    var actualInnerWidth = document.body.clientWidth; 
    $('body').css('width', actualInnerWidth );
    $('body').addClass('lock-scrolling');
    $('body').addClass('left');
}

function fixBodyOnModalClosing() {
    $('body').removeAttr('style');
    $('body').removeClass('lock-scrolling');
    $('body').removeClass('left');
}

function reloadOnModalClosing() {
    fixBodyOnModalClosing();

    if ($('#reload-on-close').length > 0) {
        location.reload();
    }
}

function modalAlert(message) {
    $('.modal-alert-message').textAsHtmlMultiline(message);

    var instance = M.Modal.getInstance($('#alertModal'));
    instance.open();
}
function modalAlertBeforeReload(message) {
    $('.modal-alert-message').textAsHtmlMultiline(message);

    $('#alert-ok').on('click', function () {
        functionToExecuteOnOk();
    });

    var instance = M.Modal.getInstance($('#alertModal'));
    instance.open();
}

function modalAlertBeforeRedirectToLogin(message) {
    $('.modal-alert-message').textAsHtmlMultiline(message);

    $('#alert-ok').on('click', function () {
        jqNavigate(SharedConstSite.get('BaseRedirectUrl'))
    });

    var instance = M.Modal.getInstance($('#alertModal'));
    instance.open();
}

function modalConfirm(message, functionToExecuteOnOk, functionArgument1, functionArgument2) {
    //set display message and bind return function to ok linkbutton
    safeHtmlDisplay('.modal-confirm-message', message);

    $('#confirm-ok').on('click', function () {
        functionToExecuteOnOk(functionArgument1, functionArgument2);
    });

    var instance = M.Modal.getInstance($('#confirmModal'));
    instance.open();
}



function initMatTooltips() {
    $('.tooltipped').tooltip();
}

function initMatDropdowns() {
    $(".dropdown-trigger").dropdown();
}





// #region functions input fields general init and configuration

function initializeSelects() {
    $('select').formSelect();
}

function initInputControls() {   
    $('input:text.get, textarea.get').on("change keyup", function (event) {
        $(event.target).valid();
    });

    //init datepickers
    M.Datepicker.init(document.querySelectorAll('.datepicker'), ConfigDatePicker);

    fixTextArea();
    fixLabels();   
}
// #endregion


function initBannerPolicies() {
    var token = '#' + SharedConstSite.get('TenantToken');

    $("#btn-dismiss").on('click', function () {
        var domainSubPath = '/';

        //try setting the correct site domain subpath
        var urlCompletePath = window.location.pathname.split("/");        
        if (urlCompletePath.length > 1 && !isObjEmpty(urlCompletePath[1])) {
            domainSubPath = domainSubPath + urlCompletePath[1];
        }
        Cookies.set(
            SharedConstSite.get('CookieBannerPoliciesDismissed')
            , SharedConstSite.get('CookieValueTrue')
            , { expires: 365, path: domainSubPath, secure: true, sameSite: 'lax' }
        );

        $('#banner-policies').fadeOut(200);

        if ($(token).length > 0) {
            $(token).focus();
        }
    });
}

/* general rules: don't show preloader for links
 * download  
 * opening in new tab 
 * modal triggers
 * stepping links (no page reload or navigation)
 * */
function initPreloaderForSimpleLinks() {
    $('a.show-preloader').on('click', function () {
        showPreloader();
    })
}
function showPreloader() {
    $(".preloader-background").fadeIn(50);
}
function hidePreloader() {
    $(".preloader-background").fadeOut(50);
}

function jqNavigate(url) {
    showPreloader();
    document.location.href = url;
}
function jqLoginNavigate() {
    showPreloader();
    document.location.href = SharedConstSite.get('BaseRedirectUrl');
}
function ajaxJsonResponseSafeParse(data) {
    //console.log(data);

    if (!isNull(data)
        && !isNull(data.target)
        && !isNull(data.target.response)
        && typeof data.target.response === "string"
        && !isWhitespace(data.target.response)) {

        var parsedResponse = null;
        try {
            parsedResponse = JSON.parse(data.target.response);
        }
        catch (e) {
            hidePreloader();
            //server error, not possible right now to handle properly, simply redirect
            modalAlertBeforeRedirectToLogin(SharedConstSite.get('AjaxPostGenericErrorMessage'));
            //jqNavigate(SharedConstSite.get('BaseRedirectUrl'))
            //Materialize.toast("Error: server response invalid or empty", 7000);
            //console.log("json parse fail: " + data.target.response);
        }
        return parsedResponse;
    }
    hidePreloader();
    console.log("Error: server response invalid or empty");
    modalAlertBeforeRedirectToLogin(SharedConstSite.get('AjaxPostGenericErrorMessage'));
}