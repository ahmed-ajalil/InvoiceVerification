$(document).ready(function () {
   
    $('#tblResponse').DataTable({
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: false,
        responsive: false,
        scrollX: '100%',
        order: [],
        dom:
            "<''<''<'DtblTopCntrlBox'<'DtblTopCntrlInn'l><'DtblTopCntrlInn'f><'DtblTopCntrlInn'p>>>" +
            "<''<''tr>>" +
            "<' pt-1'<'col-sm-5'i><'col-sm-7 d-none'p>>",
        language: {
            lengthMenu: "Display _MENU_",
            searchPlaceholder: "Search records",
            search: "_INPUT_",
        },
    });
 
});
 
function bindData() {
    debugger
    $.ajax({
        
        url:  actionurl._GetUserInformationData,
        method: 'GET',
        success: function (response) {
            var data = response;
            $.each(data, function (index, item) {
                $("#tblResponse").append(data);
            });
        },
        error: function (error) {
            console.error(error);
        }
    });
}
bindData();
function edituserdata(id) {
    debugger
    
    window.location.href = actionurl._UserInfo +'?Id=' + id;;
}

function addnewuser() {
    //    sessionStorage.clear();
  
    window.location.href = actionurl._UserInfo +'?Id=0';
}


var UserIdtodelete;
function DeleteUserInfo(UserId) {
    UserIdtodelete = UserId;
    
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
            debugger
            $.ajax({
               
                url: actionurl._DeleteUserInfo + '?UserId=' + UserIdtodelete,
                type: 'post',
                success: function (response) {
                    if (response == true) {
                        

                        new swal({
                            title: "Success",
                            text: "User Information Deleted Successfully",
                            icon: "Success",
                            button: "Ok",
                        }).then(function () {
                            
                            window.location.href = actionurl._UserInfoList;
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
