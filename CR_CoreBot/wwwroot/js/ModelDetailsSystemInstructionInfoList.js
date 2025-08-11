$(document).ready(function () {
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

    if (showWarning === 'True') {
        Swal.fire({
            title: "Warning",
            text: warningMessage,
            icon: "warning"
        }).then(function () {
            window.location.href = actionurl._SystemInstructionInfoList; // Navigate to the system instruction info list page
        });
    }
});

function Edit(id) {
    window.location.href = actionurl._SystemInstructionInfo + '?Id=' + id; // Edit action URL with ID parameter
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
                url: actionurl._DeleteSystemInstruction + '/' + Id,  
                type: 'post', 
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Anti-forgery token
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: "System Instruction Deleted Successfully",
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

var previousCheckedValue;

function setActivatedInstruction(radio) {
    event.preventDefault(); // Prevent default behavior of radio button
    var Id = $(radio).val(); // Get the value from the clicked radio button

    // Store the currently checked radio button value
    previousCheckedValue = '@ViewBag.LatestModelId';

    // Uncheck the clicked radio button until confirmed
    $(radio).prop('checked', false);

    Swal.fire({
        title: 'Are you sure?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, Activate System Instruction',
        cancelButtonText: 'No, cancel!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: actionurl._SetActivatedInstruction,  
                type: 'POST',
                data: {
                    id: Id,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()  
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success",
                            text: "System Instruction activated successfully",
                            icon: "success"
                        }).then(function () {
                            // Check the clicked radio button after activation
                            $('input[name="SystemInstructionActivated"]').prop('checked', false); // Uncheck all first
                            $(radio).prop('checked', true); // Check the activated model
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
        else {
            // Restore the previous state of the radio button if user cancels
            if (previousCheckedValue) {
                $('input[name="SystemInstructionActivated"][value="' + previousCheckedValue + '"]').prop('checked', true);
            }
        }
    });
}
