 

var FilesAllowed;
var TotalDocument;
var Remaininglimit;
jQuery(document).ready(function () {
    $('#tblblobResponse').DataTable();

    function FilesAllowedLimit() {
        $.ajax({
         
           URL:actionurl._ProcessedDocumentTotalLimit,
            method: 'get',
            async: false,
            success: function (response) {
                debugger;
                FilesAllowed = response;

            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    FilesAllowedLimit();

    function bindData() {
        $.ajax({
          
            URL:actionurl._GetBlobfilesconfig,
            method: 'get',
            async: false,
            success: function (response) {
                console.log(response);
                debugger;
                TotalDocument = response.count;
                Remaininglimit = FilesAllowed - TotalDocument;
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    bindData();

});

var stringList = [];
function fnselectFile(DownloadUrl, Filename) {
    //stringList.push(DownloadUrl);
    if (stringList.indexOf(Filename) === -1) {
        stringList.push(Filename);
    }
    else {
        var index = stringList.indexOf(Filename);
        if (index !== -1) {
            stringList.splice(index, 1);
        }
    }
    // stringList.push(Filename);
}

function fnSelectSaveToBlob(TotalDocument, FilesAllowed, Remaininglimit) {
    debugger;
    // Create the credentialsModels object and save the credentials
    var credentialsModels = {
        blobmsg: '@((ViewBag.UserData).blobmsg)',
        filestatus: '@((ViewBag.UserData).Filestatus)',
        sourceAccountKey: '@((ViewBag.UserData).sourceAccountKey)',
        sourceAccountName: '@((ViewBag.UserData).sourceAccountName)',
        sourceBlobContainerName: '@((ViewBag.UserData).sourceBlobContainerName)'
    };
    show_loader();

    if (stringList.length < Remaininglimit) {
        $.ajax({
           
            url:actionurl._BlobConnectedFileList,
            type: 'POST',
            data: { stringList: stringList, credentialsModel: credentialsModels },
            async: true,
            success: function (response) {
                hide_loader();
                if (response == "AzureBlob") {
                    new swal({
                        title: "Success",
                        text: "File Upload successfully",
                        icon: "success"
                    }).then((result) => {
                        if (result) {
                            window.location.reload();
                        }
                    });
                }
                else {
                    new swal({
                        title: "Error",
                        text: "Failed!!",
                        icon: "error"
                    });
                }

            },
            error: function (xhr, status, error) {
                hide_loader();
                new swal({
                    title: "Error",
                    text: "Failed!!",
                    icon: "error"
                });
            }
        });
    }
    else {
        hide_loader();
        new swal({
            title: 'Error',
            text: `Files Allowed: ${FilesAllowed}, Total Document Processed: ${TotalDocument}, Remaining Document Limit: ${Remaininglimit}`,
            icon: 'Error'
        });
    }
}
// Function to show the selected files
function showSelectedFiles() {
    var fileInput = document.getElementById('customFile');
    var selectedFilesDiv = document.getElementById('selectedFilesDiv');
    selectedFilesDiv.innerHTML = '';
    for (var i = 0; i < fileInput.files.length; i++) {
        var fileName = fileInput.files[i].name;
        var fileListItem = document.createElement('p');
        fileListItem.textContent = fileName;
        selectedFilesDiv.appendChild(fileListItem);
    }
}
//Uplod Data INTO BLOB
function fnUploadMultiplefiles() {
    debugger;
    var files = $('#customFile')[0].files;
    var Checkfile = $('#customFile').val();

    if (Checkfile == "") {
        new swal({
            title: "Error",
            text: "Please select a file",
            icon: "error"
        });
        return;
    }

    if (files.length > 0) {
        document.getElementById('loader').style.visibility = "visible";
        var urlParams = new URLSearchParams(window.location.search);
        var credentialsModelsEncoded = urlParams.get('credentialsModels');

        // Decode the URL-encoded JSON string
        var credentialsModelsDecoded = decodeURIComponent(credentialsModelsEncoded);

        // Parse the JSON string to get an object
        //var credentials = JSON.parse(credentialsModelsDecoded);
        // Create a FormData object
        var formData = new FormData();

        // Add credentials to the FormData object
        formData.append('model.Credentials', credentialsModelsDecoded);

        // Add files to the FormData object
        for (var i = 0; i < files.length; i++) {
            formData.append('model.Images', files[i]);
        }

        $.ajax({
           
           url:actionurl._UploadFilesToBlob,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                document.getElementById('loader').style.visibility = "hidden";
                if (response.blobmsg === "successfully") {
                    new swal({
                        title: "Success",
                        text: "All File Upload successfully",
                        icon: "success"
                    }).then((result) => {
                        if (result) {
                            
                            window.location.href = actionurl._BlobConnectedFileList+'?credentialsModels=' + response.encryptedCredentials;
                        }
                    });
                } else {
                    new swal({
                        title: "Error",
                        text: response,
                        icon: "error"
                    });
                }
            },
            error: function (xhr, status, error) {
                document.getElementById('loader').style.visibility = "hidden";
                new swal({
                    title: "Error",
                    text: "Failed!!",
                    icon: "error"
                });
            }
        });
    }
}

function getFileExtension(fileName) {
    // Split the file name by the dot (.)
    const parts = fileName.split('.');

    // Get the last part of the split
    const extension = parts[parts.length - 1];
    debugger
    // Convert the extension to lowercase (optional)
    return extension.toLowerCase();
}