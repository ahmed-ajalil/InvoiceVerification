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

function CustomerInfo(CustomerId) {
   
    window.location.href = actionurl._CustomerInfo+'?CustomerId=' + CustomerId;
}

var CustomerIdtodelete;
function DeleteCustomerInfo(CustomerId) {
    debugger
    CustomerIdtodelete = CustomerId;
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        if (result.isConfirmed) {
            debugger
            $.ajax({
               
                url:actionurl._DeleteCustomerInfo +'?CustomerId=' + CustomerIdtodelete,
                type: 'post',
                success: function (response) {
                    if (response == true) {
                        new swal({
                            title: " Success",
                            text: " Customer Information Deleted Successfully",
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
            Swal.fire(
                'Cancelled',
                'Your file is safe :)',
                'error'
            );
        }
    });

}





 