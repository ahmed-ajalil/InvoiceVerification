 

var editedid;
jQuery(document).ready(function () {


    $("#Updatebtn").hide();
    $("#Insertbtn").show();

    // IF there is something in the query string when edit page opens
    if (window.location.search) {
        debugger
        const params = new URLSearchParams(window.location.search);
        editedid = params.get("Id");
        editedid = parseInt(editedid);

        // When page is opened for update operation
        if (editedid != 0 && editedid != undefined || editedid != "") {
            $("#Updatebtn").show();
            $("#Insertbtn").hide();
            debugger
            $.ajax({
                
                url: actionurl._GetEditUserInformationData + '?Id=' + editedid,
                method: 'GET',
                async: false,
                success: function (response) {
                    var data = response;

                    $("#username").val(data[0].userName);
                    $("#username").prop('disabled', true);
                    $("#password").val(data[0].password);
                    $("#firstname").val(data[0].firstName);
                    $("#lastname").val(data[0].lastName);

                    // var dropdown = document.getElementById("Role");

                    // var roleId = data[0].roleId;
                    // if (roleId == 1 || roleId == 2 || roleId == 3) {
                    //     dropdown.value = roleId.toString();
                    // }
                    debugger
                    if (data[0].streaming === 1) {
                        $('#streamingOn').prop('checked', true);
                    } else {
                        $('#streamingOff').prop('checked', true);
                    }
                },
                error: function (error) {
                    console.error(error);
                }
            });
        }

        // When page is opened for insert
        else {

            $("#Updatebtn").hide();
            $("#Insertbtn").show();
            $("#username").val(null);
            $("#password").val(null);


        }
    }

    // When no query string.
    else {
        $("#Updatebtn").hide();
        $("#Insertbtn").show();
        $("#username").val(null);
        $("#password").val(null);
    }
});

function SaveUserInfo() {
    debugger
    var firstname = $("#firstname").val();

    var lastname = $("#lastname").val();
    var username = $("#username").val();
    var password = $("#password").val();
    var streaming = $('input[name="streaming"]:checked').val();
    //var RoleId= $("#Role").val();
    var RoleId = 3;


    if (firstname == "" || firstname == undefined) {
        new swal({
            title: "Error",
            text: "First name can't be empty",
            icon: "error"
        });
    }

    else if (lastname == "" || lastname == undefined) {
        new swal({
            title: "Error",
            text: "Last name can't be empty",
            icon: "error"
        });
    }

    else if (username == "" || username == undefined) {
        new swal({
            title: "Error",
            text: "Username can't be empty",
            icon: "error"
        });
    }

    else if (password == "" || password == undefined) {
        new swal({
            title: "Error",
            text: "Password can't be empty",
            icon: "error"
        });
    }

    else if (RoleId == "" || RoleId == undefined) {
        new swal({
            title: "Error",
            text: "Role can't be empty",
            icon: "error"
        });
    }

    else {
        show_loader();
        var formData = new FormData();
        formData.append('firstname', firstname);
        formData.append('lastname', lastname);
        formData.append('username', username);
        formData.append('password', password);
        formData.append('RoleId', RoleId);
        formData.append('streaming', streaming);
        //   show_loader();
        debugger
        $.ajax({
         
            url: actionurl._SaveUserInfo,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                debugger
                hide_loader();
                
                if (response.completionResult == "true") {
                    new swal({
                        title: " Success",
                        text: " User Created Successfully !!",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                    
                        window.location.href = actionurl._UserInfoList;
                    });


                }
                else if (response.completionResult == "limit") {
                    new swal({
                        title: "Error",
                        text: "Users creation limit exhausted.",
                        icon: "Error",
                        button: "error",
                    })
                }
                else if (response.completionResult == "UserExists") {
                    new swal({
                        title: "Error",
                        text: "User already exists",
                        icon: "Error",
                        button: "error",
                    })
                }
                else {
                    hide_loader();
                    new swal({
                        title: " Error",
                        text: "Something went wrong",
                        icon: "Error",
                        button: "Ok",
                    });
                }
            }
             
        });
    }
}

function redirecttolist() {
    new swal({
        title: "Success",
        text: "User Information saved successfully",
        icon: "success"
    }).then((result) => {
        if (result) {
            
            window.location.href = actionurl._UserInfoList;
        }
    });
}

var id;
// Update Code Js function
function UpdateUserInfo() {
    var Id = parseInt(editedid);

    var firstname = $("#firstname").val();

    var lastname = $("#lastname").val();
    var username = $("#username").val();
    var password = $("#password").val();
    //var RoleId = $("#Role").val();
    var RoleId = 3;
    var streaming = $('input[name="streaming"]:checked').val();


    if (firstname == "" || firstname == undefined) {
        new swal({
            title: "Error",
            text: "First name can't be empty",
            icon: "error"
        });
    }

    else if (lastname == "" || lastname == undefined) {
        new swal({
            title: "Error",
            text: "Last name can't be empty",
            icon: "error"
        });
    }

    else if (username == "" || username == undefined) {
        new swal({
            title: "Error",
            text: "Username can't be empty",
            icon: "error"
        });
    }

    else if (password == "" || password == undefined) {
        new swal({
            title: "Error",
            text: "Password can't be empty",
            icon: "error"
        });
    }

    else if (RoleId == "" || RoleId == undefined) {
        new swal({
            title: "Error",
            text: "Role can't be empty",
            icon: "error"
        });
    }
    else {

        var formData = new FormData();
        formData.append('firstname', firstname);
        formData.append('lastname', lastname);
        formData.append('username', username);
        formData.append('password', password);
        formData.append('RoleId', RoleId);
        formData.append('UserId', Id);
        formData.append('streaming', streaming);

        $.ajax({
           
            url: actionurl._UpdateUserData,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                hide_loader();
                debugger
                var data = response;
                if (data == true) {
                    new swal({
                        title: " Success",
                        text: " User Updated Successfully !!",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                        window.location.href = actionurl._UserInfoList;
                    });

                }
                else {

                    new swal({
                        title: " Error",
                        text: " Something went wrong",
                        icon: "Error",
                        button: "Ok",
                    });

                }
            },
            error: function (error) {
                hide_loader();

                new swal({
                    title: "Error",
                    text: "Something went wrong",
                    icon: "error"
                });
                console.log(error);
                console.log(error.responseText);
            }

        });
    }
}

