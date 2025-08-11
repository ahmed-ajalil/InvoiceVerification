

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
    debugger
    $.ajax({
 
        url: actionurl._GetEditedSuggestedQuestionInformationData + '/' + id,
        method: 'GET',
        success: function (response) {
            $('#editId').val(response.id);
            $('#editQuestion').val(response.question);
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

function Delete(Id) {
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
                
                url: actionurl._DeleteSuggestedQuestion + '/' + Id,
                type: 'post', 
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Include anti-forgery token
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: "Suggested Question Deleted Successfully",
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

function SaveSuggestedQuestionInfo(action) {
    var formData = $('#editForm').serialize();
    var url = action === 'update' ? actionurl._UpdateSuggestedQuestionInfo : actionurl._AddSuggestedQuestion;
    $.ajax({
        url: url,
        method: 'POST',
        data: formData,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: "Suggested Question Saved Successfully",
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