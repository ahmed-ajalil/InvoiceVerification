jQuery(document).ready(function () {
    var selectedCaseNumbers = [];
    var caseNumber = [];

    FillDatatable();

    function FillDatatable() {
        $.ajax({

            url: actionurl._GetComplaintData,
            type: 'Get',
            contentType: false,
            success: function (response) {
                if (response.success) {
                    BindTable(response);

                    GetRootCauses();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function BindTable(response) {
        $('#tblResponse2 tbody').remove();

        var caseDetails = response.data;
        var tableBody = $('<tbody>');

        $.each(caseDetails, function (index, caseDetail) {
            var row = $('<tr>');

            row.append($('<td>').html('<input type="checkbox" class="select-checkbox">'));
            row.append($('<td style="display:none;">').text(caseDetail.id));
            row.append($('<td>').text(caseDetail.name));

            if (caseDetail.isProcessed == null || caseDetail.isProcessed == '' || caseDetail.isProcessed == 0) {
                row.append($('<td>').html('<button class="btn btn-primary btn-Generate">Generate</button>'));
            }
            else {
                row.append($('<td>').html('<button class="btn btn-primary btn-View">View</button>'));
            }

            tableBody.append(row);
        });

        $('#tblResponse2').append(tableBody);
    }

    function toggleActionButton() {
        var isAnyChecked = $('.select-checkbox:checked').length > 0;
        $('#actionBtn').prop('disabled', !isAnyChecked);
    }

    $(document).on('change', '#selectAll', function () {
        var isChecked = $(this).is(':checked');
        var table = $('#tblResponse2').DataTable();
        var rows = table.rows({ 'page': 'current' }).nodes();

        $('input[type="checkbox"].select-checkbox', rows).prop('checked', isChecked);
        toggleActionButton();
        updateSelectedCaseNumbers();
    });

    $(document).on('change', '.select-checkbox', function () {
        var table = $('#tblResponse2').DataTable();
        var rows = table.rows({ 'page': 'current' }).nodes();
        var totalCheckboxes = $('input[type="checkbox"].select-checkbox', rows).length;
        var checkedCheckboxes = $('input[type="checkbox"].select-checkbox:checked', rows).length;

        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
        toggleActionButton();
        updateSelectedCaseNumbers();
    });

    function GetRootCauses() {
        if ($.fn.DataTable.isDataTable('#tblResponse2')) {
            $('#tblResponse2').DataTable().destroy();
        }

        $('#tblResponse2').DataTable({
            "paging": true,
            "pageLength": 10,
            "lengthMenu": [10, 25, 100],
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": false,
            "responsive": false,
            "scrollX": '100%',
            "order": [],
            "dom":
                "<'row g-2'<'col'l><'col-auto text-center'B><'col-auto'f>>" +  // Adds length dropdown, buttons, and search in the same row
                "<'row'<'col-sm-12'tr>>" +
                "<'row pt-1'<'col-sm-5'i><'col-sm-7'p>>",
            "buttons": [
                {
                    extend: 'excelHtml5',
                    text: 'Download Excel',
                    title: 'Dashboard Data',
                    className: 'btn-sm',
                    exportOptions: {
                        columns: ':visible:not(:first-child):not(:last-child)'  // Excludes "Select All" and "Action" columns
                    }
                }
            ],
            "language": {
                "lengthMenu": "Display _MENU_",
                "searchPlaceholder": "Search records",
                "search": "_INPUT_",
            }
        });

    }

    $('#actionBtn').click(function () {
        ProcessSelectedComplaints();
    });

    jQuery(document).on('click', '#tblResponse2 .btn-Generate', function () {
        caseNumber = $(this).closest('tr').find('td:eq(1)').text();
        ProcessSingleComplaint();
    });

    jQuery(document).on('click', '#tblResponse2 .btn-View', function () {
        caseNumber = $(this).closest('tr').find('td:eq(1)').text();
        GetFlightValidationDetails(caseNumber);
    });

    function GetFlightValidationDetails(caseNumber) {
        show_loader();
        $.ajax({
            url: actionurl._FlightValidationDetails,
            type: 'POST',
            data: { caseNumber: caseNumber },
            success: function (response) {
                $('#modalDataTable tbody').remove();
                var caseDetails = response.data;
                var tableBody = $('<tbody>');
                debugger
                $.each(caseDetails, function (index, caseDetail) {
                    var flightDate = new Date(caseDetail.flightDate);

                    if (isNaN(flightDate.getTime())) {
                        flightDate = 'Invalid Date';
                    } else {
                        flightDate = flightDate.toLocaleDateString('en-GB');
                    }
                    var row = $('<tr>');

                    row.append($('<td>').text(flightDate));
                    row.append($('<td>').text(caseDetail.flightNumber));
                    row.append($('<td>').text(caseDetail.dep));
                    row.append($('<td>').text(caseDetail.arr));
                    row.append($('<td>').text(caseDetail.bcMeals));
                    row.append($('<td>').text(caseDetail.internalTotalBCMealsCost));
                    row.append($('<td>').text(caseDetail.validationStatus));
                    row.append($('<td>').text(caseDetail.fileName));

                    tableBody.append(row);
                });

                $('#modalDataTable').append(tableBody);


                GetDatatableFormatting();
                $('#FlightDetails').modal('show');
                hide_loader();

            },
            error: function (xhr, status, error) {
                alert("An error occurred while taking action.");
                hide_loader();
            }
        });
    }

    function GetDatatableFormatting() {
        if ($.fn.DataTable.isDataTable('#modalDataTable')) {
            $('#modalDataTable').DataTable().destroy();
        }
        $('#modalDataTable').DataTable({
            "paging": true,
            "pageLength": 10,
            "lengthMenu": [10, 25, 100],
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": false,
            "responsive": false,
            "scrollX": '100%',
            "order": [],
            "dom":
                "<'row g-2'<'col'l><'col-auto text-center'B><'col-auto'f><'col-auto'p>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row pt-1'<'col-sm-5'i>>",
            "buttons": [
                {
                    extend: 'excelHtml5',
                    text: 'Download Excel',
                    title: 'Meal Invoice Cost Validation Data',
                    className: 'btn-sm',
                    exportOptions: {
                        columns: ':visible'
                    }
                }
            ],
            "language": {
                "lengthMenu": "Display _MENU_",
                "searchPlaceholder": "Search records",
                "search": "_INPUT_",
            }
        });
    }

    function updateSelectedCaseNumbers() {
        selectedCaseNumbers = [];

        var table = $('#tblResponse2').DataTable();
        var rows = table.rows({ 'page': 'current' }).nodes();

        $('input[type="checkbox"].select-checkbox:checked', rows).each(function () {
            var row = $(this).closest('tr');
            var caseNumber = row.find('td:eq(1)').text();
            selectedCaseNumbers.push(caseNumber);
        });

        if (selectedCaseNumbers.length > 0) {
            $('#takeActionBtn').prop('disabled', false);
        } else {
            $('#takeActionBtn').prop('disabled', true);
        }
    }

    function ProcessSelectedComplaints() {
        if (selectedCaseNumbers.length > 0) {
            show_loader();
            $.ajax({

                url: actionurl._ProcessComplaints,
                type: 'POST',
                data: { caseNumbers: selectedCaseNumbers },
                success: function (response) {
                    alert(response.message);
                    FillDatatable();
                    $('#actionBtn').prop('disabled', true);
                    $('#selectAll').prop('checked', false);
                    selectedCaseNumbers = [];
                    hide_loader();
                },
                error: function (xhr, status, error) {
                    alert("An error occurred while taking action.");
                    hide_loader();
                }
            });
        }
    }
    function ProcessSingleComplaint() {
        show_loader();
        $.ajax({

            url: actionurl._ProcessComplaints,
            type: 'POST',
            data: { caseNumbers: caseNumber },
            success: function (response) {
                alert(response.message);
                FillDatatable();
                $('#actionBtn').prop('disabled', true);
                $('#selectAll').prop('checked', false);
                hide_loader();
            },
            error: function (xhr, status, error) {
                alert("An error occurred while taking action.");
                hide_loader();
            }
        });
    }

});      