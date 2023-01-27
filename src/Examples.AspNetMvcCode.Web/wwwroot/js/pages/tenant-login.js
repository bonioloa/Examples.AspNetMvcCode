//dependencies
//jquery
//SharedConstSite - view _layout
//SharedConstTenantLoginPage - page view

$(function () {
    //if token was provided in link, submit form automatically without showing page
    var token = '#' + SharedConstSite.get('TenantToken');
    var formToken = '#' + SharedConstTenantLoginPage.get('formTenantId');
    if ($(token).val().length > 0) {
        $(formToken).submit();
    }
    else {
        $('#page-wrapper').removeClass('hidden');
        var banner = '#' + SharedConstSite.get('BannerPolicies');
        if ($(banner).length <= 0 ) {
            $(token).focus();
            }
    }
});