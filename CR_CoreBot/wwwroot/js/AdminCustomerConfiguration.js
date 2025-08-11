var editedid;
$(document).ready(function () {
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
            $.ajax({
                url: actionurl._GetEditedCustomerConfigurationData+'?Id=' + editedid,
                method: 'GET',
                async: false,
                success: function (response) {
                    var data = response;
                    debugger
                    console.log(data);
                    $("#username").prop('disabled', true);
                    $("#username").val(data[0].username);
                    $("#filesallowed").val(data[0].filesAllowed);
                    $("#usersallowed").val(data[0].usersAllowed);

                    var fileformat = data[0].fileFormat;
                    var fileformatArray = fileformat.split(",");

                    // Setting false on each checkbox first
                    $("#Pdf").prop('checked', false);
                    $("#Word").prop('checked', false);
                    $("#Csv").prop('checked', false);
                    $("#Image").prop('checked', false);


                    // Loop through each element
                    for (var i = 0; i < fileformatArray.length; i++) {
                        var element = fileformatArray[i].toLowerCase(); // Convert to lowercase for case-insensitive comparison

                        // Check if the element contains word,pdf,csv or image
                        if (element.indexOf("pdf") !== -1) {
                            $("#Pdf").prop('checked', true);
                        }

                        if (element.indexOf("word") !== -1) {
                            $("#Word").prop('checked', true);
                        }

                        if (element.indexOf("csv") !== -1) {
                            $("#Csv").prop('checked', true);
                        }

                        if (element.indexOf("image") !== -1) {
                            $("#Image").prop('checked', true);
                        }
                        //if (data[0].Image == false) {
                        //    $('#Image').prop('checked', false);
                        //}
                        //else{
                        //    $('#Image').prop('checked', true);
                        //}

                    }
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

function SaveCustomerConfiguration() {
    debugger

    var username = $("#username").val();
    var usersallowed = $("#usersallowed").val();
    var filesallowed = $("#filesallowed").val();

    //var isPdfChecked = $('#Pdf').is(':checked');

    var isPdfChecked = $('#Pdf').is(':checked');
    var isWordChecked = $('#Word').is(':checked');
    var isCsvChecked = $('#Csv').is(':checked');
    var isImageChecked = $('#Image').is(':checked');


    var CustomerConfiguration = {};
    CustomerConfiguration.Username = username;
    CustomerConfiguration.UsersAllowed = usersallowed;
    CustomerConfiguration.FilesAllowed = filesallowed;
    var fileformat = "";
    debugger;
    var result = $('input[type="checkbox"]:checked');
    if (result.length > 0) {
        var resultString = "";
        var checkedvalues = "";

        for (var i = 0; i < result.length; i++) {
            checkedvalues += result[i].value + ",";
            //  if ($(this).val() == true) {
            // checkedvalues += $(this).attr("id") + ",";
            //  }
        }
    }

    if (username == "" || username == undefined) {
        new swal({
            title: "Error",
            text: "Username can't be empty",
            icon: "error"
        });
    }

    else if (usersallowed == "" || usersallowed == undefined) {
        new swal({
            title: "Error",
            text: "No. of Users allowed can't be empty",
            icon: "error"
        });
    }

    else if (filesallowed == "" || filesallowed == undefined) {
        new swal({
            title: "Error",
            text: "No. of Files allowed can't be empty",
            icon: "error"
        });
    }

    else {
        show_loader();
        var formData = new FormData();
        formData.append('Username', username);
        formData.append('UsersAllowed', usersallowed);
        formData.append('FilesAllowed', filesallowed);
        formData.append('FileFormat', checkedvalues);

        //   show_loader();
        $.ajax({
            
            url: actionurl._SaveCustomerConfiguration,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                hide_loader();
                debugger
                if (response.completionResult == "true") {
                    new swal({
                        title: " Success",
                        text: " Customer Configuration Created Successfully !!",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                      
                        window.location.href =  actionurl._CustomerConfigurationList;
                    });
                } else if (response == "fileformatempty") {
                    new swal({
                        title: "Error",
                        text: "File Formats can't be empty",
                        icon: "Error",
                        button: "Ok",
                    })
                }
                else if (response.completionResult == "false") {
                    new swal({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "Error",
                        button: "error",
                    })
                }
            }
        });
    }
}

var id;
 
function UpdateCustomerConfiguration() {

    debugger;
    var Id = parseInt(editedid);

    debugger
    var username = $("#username").val();
    var usersallowed = $("#usersallowed").val();
    var filesallowed = $("#filesallowed").val();

    var fileformat = "";
    debugger;

    var result2 = $('input[type="checkbox"]:checked');
    var checkboxes = document.getElementsByName("checkbox");

    var checkedvalues = "";
    for (var i = 0; i < result2.length; i++) {
        checkedvalues += result2[i].value + ",";
    }

    //var result = $('input[type="checkbox"]:checked');
    //if (result.length > 0) {
    //    var resultString = "";
    //    var checkedvalues = "";

    //    for (var i = 0; i < result.length; i++) {
    //        checkedvalues += result[i].value + ",";
    //        //  if ($(this).val() == true) {
    //        // checkedvalues += $(this).attr("id") + ",";
    //        //  }
    //    }
    //}
    //else {
    //    alert("Error. You must select a file format");
    //    //   new swal("Error", "You must select a file format", "Error");
    //}

    if (username == "" || username == undefined) {
        new swal({
            title: "Error",
            text: "Username can't be empty",
            icon: "error"
        });
    }

    else if (usersallowed == "" || usersallowed == undefined) {
        new swal({
            title: "Error",
            text: "No. of Users allowed can't be empty",
            icon: "error"
        });
    }

    else if (filesallowed == "" || filesallowed == undefined) {
        new swal({
            title: "Error",
            text: "No. of Files allowed can't be empty",
            icon: "error"
        });
    }
    else {
        show_loader();
        var formData = new FormData();
        formData.append('Username', username);
        formData.append('UsersAllowed', usersallowed);
        formData.append('FilesAllowed', filesallowed);
        formData.append('ConfigurationId', Id);
        formData.append('FileFormat', checkedvalues);
        $.ajax({
           
            url: actionurl._UpdateCustomerConfigurationData,
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
                        text: "Customer Configuration Updated Successfully !!",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                     
                        window.location.href =  actionurl._CustomerConfigurationList;
                    });
                }
                else if (data == "fileformatempty") {
                    new swal({
                        title: "Error",
                        text: "File Formats can't be empty",
                        icon: "Error",
                        button: "Ok",
                    })
                }
                else {
                    alert("Something went wrong");
                }
            },
            error: function (error) {
                hide_loader();
                swal({
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

