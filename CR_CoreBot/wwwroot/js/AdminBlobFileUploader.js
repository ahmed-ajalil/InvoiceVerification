$(document).ready(function () {
    // var baseURl = getBaseURL(0);

    $('#uploadBtn').click(function () {
        var formData = new FormData();
        var files = $('#fileInput')[0].files;

        if (files.length === 0) {
            alert("Please select at least one file.");
            return;
        }

        for (let i = 0; i < files.length; i++) {
            formData.append("files", files[i]);
        }

        formData.append("file", fileInput);
        $.ajax({
            // url: baseURl + '/Admin/UploadFilesToBlob',
            /*url: '/Admin/UploadFilesToBlob',*/
            url: actionurl._UploadFilesToBlob,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    BindTable(response);
                    $('#fileInput').val('');

                    GetRootCauses();
                } else {
                    alert(response.message);
                    BindTable(response);
                    $('#fileInput').val('');

                    GetRootCauses();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("An error occurred while uploading the file.");
            }
        });
    });
    FillDatatable();

    function FillDatatable() {
        $.ajax({
            // url: baseURl + '/Admin/GetBlobData',
            /*  url: '/Admin/GetBlobData',*/
            url:  actionurl._GetBlobData,
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

        var files = response.file_Names;
        var tableBody = $('<tbody>');
        var accountName = response.accountName;
        var containerName = response.containerName;

        $.each(files, function (index, file) {
            var row = $('<tr>');

            row.append($('<td>').text(index + 1));
            row.append($('<td>').text(file));
            var fileUrl = `https://${accountName}.blob.core.windows.net/${containerName}/${encodeURIComponent(file)}`;
            row.append($('<td>').html(`<a href="${fileUrl}" target="_blank" class="button">View</a>`));

            tableBody.append(row);
        });

        $('#tblResponse2').append(tableBody);
    }

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
                "<'row'<'col-sm-8'l><'col-sm-2 text-center'B><'col-sm-2'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row pt-1'<'col-sm-5'i><'col-sm-7'p>>",
            "buttons": [
                {
                    extend: 'excelHtml5',
                    text: 'Download Excel',
                    title: '',
                    exportOptions: {
                        columns: ':visible:not(:first-child):not(:last-child)'
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

    $(document).on('click', '.file-link', function (e) {
        e.preventDefault();
        const fileName = $(this).data('filename');
        const anchor = $(this);

        $.ajax({
            // url: baseURl + `/Banking/BankBot/CheckFileExists`,
            /* url: `/Banking/BankBot/CheckFileExists`,*/
            url:  actionurl._CheckFileExists,
            data: { fileName: fileName },
            method: 'GET',
            success: function (response) {
                if (response.exists) {
                    const blobUrl = `https://${response.accountName}.blob.core.windows.net/${response.containerName}/${encodeURIComponent(fileName)}`;
                    window.open(blobUrl, '_blank');

                } else {
                    alert('This file is not available on blob.');
                }
            },
            error: function () {
                alert('An error occurred while checking the file.');
            }
        });
    });

});