$(document).ready(function () {
    $('#tblResponse').DataTable({

        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        //"scrollX": true,
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
function EditCustomerConfiguration(ConfigurationId) {
    
    window.location.href = actionurl._CustomerConfiguration+'?Id=' + ConfigurationId;
}


var ConfigurationIdtodelete;
function DeleteCustomerConfiguration(ConfigurationId) {
    ConfigurationIdtodelete = ConfigurationId;
     
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
               
                url: actionurl._DeleteCustomerConfiguration+'?ConfigurationId=' + ConfigurationIdtodelete,
                type: 'post',
                success: function (response) {
                    if (response == true) {
                         

                        new swal({
                            title: "Success",
                            text: "Customer Configuration Deleted Successfully",
                            icon: "Success",
                            button: "Ok",
                        }).then(function () {
                             
                            window.location.href =  actionurl._redirect;
                        });
                    }
                    else if (response == false) {
                        new swal({
                            title: "Error",
                            text: "Something went wrong",
                            icon: "error"
                        })
                    }
                },
                error: function (error) {
                    new swal({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    })
                }

            })


        } else if (result.dismiss === Swal.DismissReason.cancel) {
            // The user clicked the "No, cancel!" button, or outside the modal
            Swal.fire(
                'Cancelled',
                'Your file is safe :)',
                'error'
            );
        }
    });
}