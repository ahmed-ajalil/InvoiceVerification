 

function ChangePassword() {
    var ChangePassword = {};

    CurrentPassword = $("#CurrentPassword").val();
    NewPassword = $("#NewPassword").val();
    ConfirmNewPassword = $("#ConfirmNewPassword").val();

    if (CurrentPassword == "" || CurrentPassword == undefined) {
        new swal({
            title: "Error",
            text: "Current Password can't be empty",
            icon: "error"
        });
    }

    else if (NewPassword == "" || NewPassword == undefined) {
        new swal({
            title: "Error",
            text: "New Password can't be empty",
            icon: "error"
        });
    }

    else if (ConfirmNewPassword == "" || ConfirmNewPassword == undefined) {
        new swal({
            title: "Error",
            text: "Confirm New Password can't be empty",
            icon: "error"
        });
    }

    else if (NewPassword != ConfirmNewPassword) {
        new swal({
            title: "Error",
            text: "New Password and Confirm New Password are not matching",
            icon: "error"
        });
    }
    else {

        debugger
        $.ajax({
            
            url:actionurl._ChangePassword,
            method: 'post',
            data: { CurrentPassword: CurrentPassword, NewPassword: NewPassword, ConfirmNewPassword: ConfirmNewPassword },
            success: function (response) {
                if (response == true) {
                    new swal({
                        title: "Success",
                        text: "Password changed successfully",
                        icon: "success"
                    }).then((result) => {
                        if (result) {
                            window.location.href = actionurl._Logout;
                        }
                    });
                }
                else if (response == "OldPasswordIncorrect") {
                    new swal({
                        title: "Error",
                        text: "Current Password you entered is incorrect",
                        icon: "error"
                    })
                }
                else if (response == false) {
                    new swal({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    });
                }
            },
            error: function (error) {
                new swal({
                    title: "Error",
                    text: "Something went wrong",
                    icon: "error"
                });
            }
        });
    }
}