 


jQuery(document).ready(function () {
    $('#tblResponse').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        "scrollX": '100%',
        order: [],
        "dom":
            "<''<''<'DtblTopCntrlBox'<'DtblTopCntrlInn'l><'DtblTopCntrlInn'f><'DtblTopCntrlInn'p>>>" +
            "<''<''tr>>" +
            "<' pt-1'<'col-sm-5'i><'col-sm-7 d-none'p>>",
        "language": {
            "lengthMenu": "Display _MENU_ ",
            searchPlaceholder: "Search records",
            search: "_INPUT_",
        },
    });
});

function Edit(id) {
    $.ajax({
       
        url: actionurl._GetEditedTrainModelInformationData + '/' + id,
        method: 'GET',
        success: function (response) {
            $('#editId').val(response.id);
            $('#editQuestion').val(response.question);
            $('#editSQLQuery').val(response.sqlQuery);
            $('#editModal').modal('show');
        },
        error: function () {
            Swal.fire({
                title: "Error",
                text: "Failed to load question.",
                icon: "error"
            });
        }
    });
}

function Delete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: actionurl._DeleteTrainModel + '/' + id,
                type: 'post', // Ensure DELETE method
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Include anti-forgery token
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: "Question and Query Deleted Successfully",
                            icon: "success"
                        }).then(function () {
                            location.reload(); // Reload the page to reflect changes
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: response.message,
                            icon: "error"
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    });
                }
            });
        }
    });
}

function SaveTrainModelInfo(action) {
    var formData = new FormData($('#editForm')[0]); // Grabs all form data

    // Clear previous error messages
    $('#error-question').text('');
    $('#error-sqlquery').text('');

    $.ajax({
        url: actionurl._UpdateTrainModelInfo,
        type: 'POST',
        data: formData,
        processData: false, // Important for sending FormData
        contentType: false, // Important for sending FormData
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: response.message,
                    icon: "success"
                }).then(function () {
                    location.reload(); // Reload the page to reflect changes
                });
                $('#editModal').modal('hide'); // Close the modal
            } else {
                Swal.fire({
                    title: "Error",
                    text: response.message,
                    icon: "error"
                });
            }
        },
        error: function (jqXHR) {
            if (jqXHR.responseJSON && jqXHR.responseJSON.errors) {
                // Display server-side validation errors
                if (jqXHR.responseJSON.errors.Question) {
                    $('#error-question').text(jqXHR.responseJSON.errors.Question[0]);
                }
                if (jqXHR.responseJSON.errors.SQLQuery) {
                    $('#error-sqlquery').text(jqXHR.responseJSON.errors.SQLQuery[0]);
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: "Failed to update model information.",
                    icon: "error"
                });
            }
        }
    });
}

