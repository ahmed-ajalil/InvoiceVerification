 

jQuery(document).ready(function () {
    debugger
    var table = document.getElementById("tblResponse");

    // Get the count of all rows in the table
    var TotalDocument = table.rows.length - 1;
    $("#totaldoc").html(TotalDocument);

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
    debugger;

    function bindData() {
        debugger
        $.ajax({
            
            URL:actionurl._ProcessedDocumentTotalLimit,
            method: 'get',
            async: false,
            success: function (response) {
                debugger;
                var FilesAllowed = response;
                $("#totalLimit").html(FilesAllowed);
                var remainingdoc = FilesAllowed - TotalDocument;
                $("#remainingDocument").html(remainingdoc);
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    bindData();
});


function DeleteBlobDocument(Filename, LoginCase) {
    var Filetodelete = Filename;
    debugger;
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        if (result.isConfirmed) {
            debugger;
            $.ajax({
               
                url: actionurl._DeleteBlobDocument +'? Filename = ' + Filetodelete + ' & LoginCase=' + LoginCase,
                type: 'post',
                success: function (response) {
                    if (response == true) {
                        Swal.fire({
                            title: "Success",
                            text: "File Deleted Successfully",
                            icon: "success",
                            showCancelButton: false,
                            confirmButtonColor: "#3085d6",
                            confirmButtonText: "OK",
                        }).then((result) => {
                            debugger;
                            if (result.isConfirmed) {

                                 window.location.href =  actionurl._ProcessedDocument;
                                 
                            }
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