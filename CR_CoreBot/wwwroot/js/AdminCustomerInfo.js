
var editedid;
var ImgVal;
$(document).ready(function () {
    $("#Updatebtn").hide();
    $("#Insertbtn").show();
    if (window.location.search) {
        const params = new URLSearchParams(window.location.search);
        editedid = params.get("CustomerId");
        editedid = parseInt(editedid);
        if (editedid != 0 && editedid != undefined || editedid != "") {
            $("#Updatebtn").show();
            $("#Insertbtn").hide();
            $.ajax({
                url: actionurl._EditCustomerInfo + '?Id=' + editedid,
                method: 'GET',
                async: false,
                success: function (response) {
                    debugger
                    var data = response;
                    $("#category").val(data[0].category);
                    $("#customername").val(data[0].name);
                    $("#organizationname").val(data[0].organizationName);
                    $("#password").val(data[0].password);
                    $("#username").prop('disabled', true);
                    $("#username").val(data[0].userName);

                    $("#BlobContainerName").val(data[0].blobContainerName);

                    $("#AccountName").val(data[0].accountName);
                    $("#AccountKey").val(data[0].accountKey);
                    var selectElement = document.getElementById("RoleType");
                    if (data[0].roleName == "Admin") {
                        $('#RoleType').val('1');
                        $('#RoleType option[value="1"]').prop('selected', true);
                    }
                    else if (data[0].roleName == "SuperAdmin") {
                        $('#RoleType').val('2');
                        $('#RoleType option[value="2"]').prop('selected', true);

                    }
                    else {
                        $('#RoleType').val('3');
                        $('#RoleType option[value="3"]').prop('selected', true);
                    }

                    var image = document.getElementById("imagePreview");
                    image.style.display = 'block';
                    // image.src = data[0].organizationLogo;
                    image.src = '../Logo/' + data[0].organizationLogo;
                    ImgVal = data[0].organizationLogo;
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

function checkContainerNameExistence() {

    var containerName = $("#BlobContainerName").val();

    // Perform AJAX call to check container name existence
    $.ajax({
        
        url: actionurl._CheckContainerNameExist + '?BlobContainerName=' + containerName,
        method: 'GET',
        success: function (response) {

            if (response.exists) {
                $("#containerExist").text("Container name already exists.").css("color", "red");
            } else {
                $("#containerExist").text("").css("color", "");
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function previewImg() {

    var preview = document.getElementById("imagePreview");
    var file = document.getElementById("imgfile").files[0];

    if (file.type == "image/png" || file.type == "image/jpg" || file.type == "image/jpeg" || file.type == "image/img") {

        $("#imagePreview").css("display", "block");

        var reader = new FileReader();
        reader.onloadend = function () {
            preview.src = reader.result;
        }

        if (file) {
            reader.readAsDataURL(file);
        }
    }
    else {
        new swal({
            title: "Error",
            text: "Please select valid Image file",
            icon: "Warning"
        });
    }
}

function SaveCustomerInfo(type) {
    debugger

    var ImgCheck;
    var formData = new FormData();
    var fileInput = document.getElementById('imgfile');
    var file = fileInput.files[0];
    if (file == undefined || file == "") {
        var fileInput1 = document.getElementById('imagePreview');
        var file1 = fileInput1.currentSrc;
        formData.append('img_file', file1);
    }
    else {
        formData.append('img_file', file);
    }
    if (ImgVal != null) {
        ImgCheck = ImgVal;
    }
    else {
        ImgCheck = file;
    }
    var username = $("#username").val();
    var password = $("#password").val();
    var customername = $("#customername").val();
    var organizationname = $("#organizationname").val();
    var category = $("#category").val();
    // var BlobContainerName = $("#BlobContainerName").val();
    // var AccountKey = $("#AccountKey").val();
    // var AccountName = $("#AccountName").val();
    var streaming = $('input[name="streaming"]:checked').val();
    var roleId = $("#RoleType").val();
    var a = typeof (roleId);
    var dataArray = [username, password, customername, organizationname, category, roleId, ImgCheck];
    var validationStatus = Validate(dataArray);

    formData.append('username', username);
    formData.append('password', password);
    formData.append('customername', customername);
    formData.append('organizationname', organizationname);
    formData.append('category', category);
    // formData.append('BlobContainerName', BlobContainerName);
    // formData.append('AccountKey', AccountKey);
    // formData.append('AccountName', AccountName);
    formData.append('RoleId', roleId);
    formData.append('CustomerId', editedid);
    formData.append('streaming', streaming);
    debugger
    if (type == "add") {
        if (validationStatus) {
            // All data is valid, proceed with further actions
            show_loader();
            debugger
            $.ajax({
                /*url: '/Admin/SaveCustomerInfo',*/
                url: actionurl._SaveCustomerInfo,
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    debugger
                    hide_loader();
                    var generatedtext = response.completionResult;
                    validateResponse(generatedtext, type);
                },
                error: function (error) {
                    hide_loader();
                    swal({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    });
                }
            });
        }
        else {
            new swal({
                title: "Error",
                text: "Please fill in all required fields.",
                icon: "error"
            });
        }
    }
    else {
        if (validationStatus) {
            show_loader();
            $.ajax({
                
                url: actionurl._UpdateCustomerInfo,
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    debugger
                    hide_loader();
                    var generatedtext = response.completionResult;
                    validateResponse(generatedtext, type);
                },
                error: function (error) {
                    hide_loader();
                    swal({
                        title: "Error",
                        text: "Something went wrong",
                        icon: "error"
                    });
                }
            });
        }
        else {
            new swal({
                title: "Error",
                text: "Please fill in all required fields.",
                icon: "error"
            });
        }
    }
}

function Validate(dataArray) {
    for (var i = 0; i < dataArray.length; i++) {
        var data = dataArray[i];
        if (data === undefined || data === null || data === "") {
            return false;
        }
    }
    return true;
}

function validateResponse(generatedtext, type) {
    var txt1 = "";
    var txt2 = "";
    if (type == "add") {
        txt1 = "saved";
        txt2 = "Saving";
    } else {
        txt1 = "Updated";
        txt2 = "Updating"
    }
    if (generatedtext == "true") {
        new swal({
            title: "Success",
            text: "Customer Information " + txt1 + " successfully",
            icon: "success"
        }).then((result) => {
            if (result) {
                
                window.location.href = actionUrl._redirect;
            }
        });
    }
    else if (generatedtext == "UserExists") {
        new swal({
            title: "Error",
            text: "User already exists",
            icon: "Error"
        })
        $("#containerExist").text("");
    }
    else if (generatedtext == "ContainerExists") {
        new swal({
            title: "Error",
            text: "Container name already exist",
            icon: "Error"
        })
        $("#containerExist").text("Container name already exists.");
    }
    else {
        new swal({
            title: "Error",
            text: "Issues while " + txt2 + " Customer Information",
            icon: "Error"
        });
        $("#containerExist").text("");
    }
}