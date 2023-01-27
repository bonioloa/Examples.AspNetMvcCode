//dependencies
//jquery
//SharedConstReportPage -  page view

$(function () {
    HandleProcessesFilters();

    $('input[name="' + SharedConstReportPage.get('InputProcessAll') + '"]').on('change', HandleProcessesFilters);

    $('#btn-get-data-to-excel').on('click', function () {
        if ($('#report-no-results-warning').length > 0) {
            $('#report-no-results-warning').hide();
        }
    });
});

function HandleProcessesFilters() {
    if ($('input[name="' + SharedConstReportPage.get('InputProcessAll') + '"]').is(':checked')) {
        $('#processes-filter-slave').hide();
        $('#' + SharedConstReportPage.get('InputProcesses')).val([]);
        $('#' + SharedConstReportPage.get('InputProcesses')).attr('disabled', true);
        $('#' + SharedConstReportPage.get('InputProcesses')).removeAttr('required');
        //.removeClass('valid invalid form-error');
    }
    else {
        $('#processes-filter-slave').show();
        $('#' + SharedConstReportPage.get('InputProcesses')).attr('disabled', false);
        $('#' + SharedConstReportPage.get('InputProcesses')).attr('required', 'required');
    }
    //reinitialize
    $('#' + SharedConstReportPage.get('InputProcesses')).formSelect();
}