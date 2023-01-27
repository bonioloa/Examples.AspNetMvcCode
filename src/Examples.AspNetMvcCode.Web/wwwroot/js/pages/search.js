//dependencies
//jquery.js
//jquery.dataTables.js
//moment.js
//datetime-moment.js
//dataTables.responsive
//SharedConstDtLocalizedMessages -layout
//SharedConstItemSearchPage -view
//SharedVarItemSearchHasResults -view
//SharedConstItemSearchDtColumnsConfig -view
//SharedConstItemSearchDtRowsData -view

$(function () {
    
    if (SharedVarItemSearchHasResults) {
        createTable();
    }
});

function createTable() {
    $.fn.dataTable.moment(SharedConstSite.get("MomentJsDateTimeFormatStandard"));

    var indexGroupColumn = SharedConstItemSearchPage.get("ColumnProcessDescriptionIndex");

    var table = $('#search-results-table').DataTable({
        deferRender: true,
        pageLength: 10,
        bLengthChange: false,
        responsive: true,
        search: { caseInsensitive: true },
        language: SharedConstDtLocalizedMessages,        
        columns: SharedConstItemSearchDtColumnsConfig,
        data: SharedConstItemSearchDtRowsData,
        order: [[indexGroupColumn, "asc"], [SharedConstItemSearchPage.get("ColumnSubmitDateTimeIndex"), "desc"]],
        columnDefs: columnDefsConfig,
        drawCallback: function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;

            api.column(indexGroupColumn, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="' + SharedConstItemSearchPage.get("SearchResultsColumns") + '">' + group + '</td></tr>'
                    );
                    last = group;
                }
            });

            $('#search-results-table tbody tr').on('click', function () {
                var currentRow = $(this).closest("tr");
                var rowData = table.row(currentRow).data();
                if (typeof rowData !== "undefined"
                    && typeof rowData[SharedConstItemSearchPage.get("ColumnItemLinkName")] !== "undefined"
                    && rowData[SharedConstItemSearchPage.get("ColumnItemLinkName")].length > 0) {

                    jqNavigate(rowData[SharedConstItemSearchPage.get("ColumnItemLinkName")]);
                }
            });
        },
        createdRow: function (row, data, index) {
            if (data[SharedConstItemSearchPage.get("ColumnRequiresAttentionName")] === "true") {
                $(row).addClass('requires-action');
            }
            if (data[SharedConstItemSearchPage.get("ColumnRequiresAttentionName")] === "false") {
                $(row).addClass('no-action');
            }
        }
    });
    //SharedConstItemSearchDtColumnsConfig
    // Order by the grouping
    $('#search-results-table tbody').on('click', 'tr.group', function () {
        var currentOrder = table.order()[0];
        if (currentOrder[0] === indexGroupColumn && currentOrder[1] === 'asc') {
            table.order([indexGroupColumn, 'desc']).draw();
        }
        else {
            table.order([indexGroupColumn, 'asc']).draw();
        }
    });
}