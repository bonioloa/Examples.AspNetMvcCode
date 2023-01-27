//dependencies
//jquery.js
//jquery.dataTables.js
//dataTables.responsive
//SharedConstDtLocalizedMessages -  layout
//SharedVarSearchHasResults - view
//SharedConstSearchDtColumnsConfig - view
//SharedConstSearchDtRowsData - view
//SharedConstUserSearchPage- view

$(function () {
    //add filter fields validations
    $('#nome').rules('add', { nameSurnameSearch: true });
    $('#cognome').rules('add', { nameSurnameSearch: true });
    $('#email').rules('add', { emailSearch: true });

    if (SharedVarSearchHasResults) {
        createTable();
    }
});


function createTable() {

    var table = $('#search-results-table').DataTable({
        deferRender: true,
        pageLength: 10,
        bLengthChange: false,
        responsive: true,
        search: {caseInsensitive: true},
        language: SharedConstDtLocalizedMessages,
        columns: SharedConstSearchDtColumnsConfig,
        data: SharedConstSearchDtRowsData,
        order: [SharedConstUserSearchPage.get("ColumnIndexSurname"), "asc"],
        columnDefs: [
            {
                targets: [
                    SharedConstUserSearchPage.get("ColumnIndexLogin"),
                    SharedConstUserSearchPage.get("ColumnIndexSurname"),
                    SharedConstUserSearchPage.get("ColumnIndexName"),
                    SharedConstUserSearchPage.get("ColumnIndexEmail")
                ],
                autowidth: true,
            }
        ],
        drawCallback: function (settings) {

            $('#search-results-table tbody tr').on('click', function () {
                var currentRow = $(this).closest("tr");
                var rowData = table.row(currentRow).data();
                if (typeof rowData !== "undefined"
                    && typeof rowData[SharedConstUserSearchPage.get("ColumnNameLinkSupervisorModify")] !== "undefined"
                    && rowData[SharedConstUserSearchPage.get("ColumnNameLinkSupervisorModify")].length > 0) {

                    jqNavigate(rowData[SharedConstUserSearchPage.get("ColumnNameLinkSupervisorModify")]);
                }
            });
        },
        createdRow: function (row, data, index) {
            if (data[SharedConstUserSearchPage.get("ColumnNameRoleType")]
                === SharedConstUserSearchPage.get("UserProfileSupervisor")) {
                $(row).addClass('user-with-roles-active');
                return;
            }
            if (data[SharedConstUserSearchPage.get("ColumnNameRoleType")]
                === SharedConstUserSearchPage.get("UserProfileBasicRoleActive")) {
                $(row).addClass('user-no-roles-active');
                return;
            }
            if (data[SharedConstUserSearchPage.get("ColumnNameRoleType")]
                === SharedConstUserSearchPage.get("UserProfileBasicRoleDeactivated")) {
                $(row).addClass('user-no-roles-disabled');
                return;
            }
        }
    });
}