 

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
    window.location.href = actionurl._Create+'?Id=' + id;
}

 
function Delete(Id) {
    debugger
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        debugger
        if (result.isConfirmed) {
            debugger
            $.ajax({
              
                url: actionurl._Delete + '?Id=' + Id,
              
                type: 'post',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()  
                },
                success: function (response) {
                    debugger
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: "Model Information Deleted Successfully",
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
                error: function (error) {
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

var previousCheckedIds = [];

function setActivatedModels(checkbox, originalStatus) {
    debugger
    var isChecked = checkbox.checked;
    var actionMessage = isChecked ? 'activate' : 'deactivate';
    var checkboxElement = $(checkbox);
    var totalActiveModels = $('input[name="ModelActivated"]:checked').length;

    if (!isChecked && (totalActiveModels + 1) <= 1) {
        Swal.fire({
            title: "Error",
            text: "At least one model must remain active.",
            icon: "error"
        });
        checkboxElement.prop('checked', true); // Revert the checkbox to active
        return;
    }

    var previousState = !isChecked;

    // Prepare selected models
    var selectedIds = [];
    $('input[name="ModelActivated"]:checked').each(function () {
        selectedIds.push($(this).val());
    });

    Swal.fire({
        title: `Are you sure you want to ${actionMessage} this model?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: `Yes, ${actionMessage.charAt(0).toUpperCase() + actionMessage.slice(1)}`,
        cancelButtonText: 'No, cancel!'
    }).then((result) => {
        if (result.isConfirmed) {
            debugger
            $.ajax({
                 url:actionurl._SetActivatedModels,
                type: 'POST',
                data: {
                    ids: selectedIds,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: `Model successfully ${actionMessage}d.`,
                            icon: "success"
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: response.message,
                            icon: "error"
                        });
                        checkboxElement.prop('checked', previousState); // Revert the checkbox state
                    }
                },
                error: function () {
                    Swal.fire({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    });
                    checkboxElement.prop('checked', previousState); // Revert on error
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            checkboxElement.prop('checked', previousState); // Revert on cancel
        }
    });
}
