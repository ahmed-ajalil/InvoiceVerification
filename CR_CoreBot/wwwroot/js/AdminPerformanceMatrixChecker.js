//var actionurl = {
//    _CustomerInfo='@Url.Action("CustomerInfo", "Admin")',
//    _DeleteCustomerInfo='@Url.Action("DeleteCustomerInfo", "Admin")'
//    /* _DeleteCustomerInfo='@Url.Action("SaveCustomerConfiguration", "Admin")'*/
//}

jQuery(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

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
    /*window.location.href = '/Admin/CustomerInfo?CustomerId=' + CustomerId;*/
    window.location.href = actionurl._CustomerInfo + '?CustomerId=' + CustomerId;

}

var CustomerIdtodelete;
function DeleteCustomerInfo(CustomerId) {
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
            // The user clicked the "Yes, delete it!" button
            // Perform the deletion or any other action here

            $.ajax({
                /*url: '/Admin/DeleteCustomerInfo?CustomerId=' + CustomerIdtodelete,*/
                url: actionurl._DeleteCustomerInfo + '?CustomerId=' + CustomerIdtodelete,
                type: 'post',
                success: function (response) {
                    if (response == true) {
                        //Swal.fire(
                        //    'Deleted!',
                        //    'Your file has been deleted.',
                        //    'success'
                        //);

                        new swal({
                            title: " Success",
                            text: " Customer Information Deleted Successfully",
                            icon: "Success",
                            button: "Ok",
                        }).then(function () {
                            window.location.href = '@Url.Action("CustomerInfoList", "Admin")';
                        });

                        //new swal({
                        //    title: "Success",
                        //    text: "Customer Information deleted successfully",
                        //    icon: "success"
                        //}).then((result) => {
                        //    if (result) {
                        //        window.location.href = '/Admin/CustomerInfoList/';
                        //    }
                        //});
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

$('.long-text').each(function () {
    var content = $(this).text();
    var showChar = 100;
    var ellipsestext = "";
    var moretext = "Show More";
    var lesstext = "Show Less";

    if (content.length > showChar) {
        var c = content.substr(0, showChar);
        var h = content.substr(showChar, content.length - showChar);

        var html = c + '<span class="moreellipses">' + ellipsestext + '</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';

        $(this).html(html);
    }
});

$(document).on("click", ".morelink", function (event) {
    event.preventDefault();
    if ($(this).hasClass("less")) {
        $(this).removeClass("less");
        $(this).html("Show More");
    } else {
        $(this).addClass("less");
        $(this).html("Show Less");
    }
    $(this).prev().toggle();
    $(this).prev().prev().toggle();
});
