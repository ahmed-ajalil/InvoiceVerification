 

var editedId;

jQuery(document).ready(function () {
    debugger
    $("#Updatebtn").hide();
    $("#Insertbtn").show();

    // Check if there's an Id in the query string
    if (window.location.search) {
        const params = new URLSearchParams(window.location.search);
        editedId = params.get("Id");
        editedId = parseInt(editedId);

        if (editedId) {
            $("#Updatebtn").show();
            $("#Insertbtn").hide();
            debugger
            $.ajax({
                
                url: actionurl._GetEditedModelInformationData + '?Id=' + editedId, 
                method: 'GET',
                success: function (response) {
                    var data = response;
                    $("#EndPoint").val(data[0].endPoint);
                    $("#ApiKey").val(data[0].apiKey);
                    $("#DeploymentName").val(data[0].deploymentName);
                    $("#ApiVersion").val(data[0].apiVersion);
                    $("#ModelName").val(data[0].modelName);

                    // Trigger the change event to load sizes for the selected ModelName
                    $("#ModelName").trigger('change');

                    // Use a callback to set the size after the ModelName dropdown is populated
                    loadAndSetSize(data[0].modelName, data[0].size);
                },
                error: function (error) {
                    console.error(error);
                    Swal.fire({
                        title: "Error",
                        text: "Failed to load model information.",
                        icon: "error"
                    });
                }
            });
        }
    }

    function loadAndSetSize(modelName, size) {
        const sizeSelect = $('#Size');
        sizeSelect.prop('disabled', true);


        sizeSelect.empty();
        sizeSelect.append(new Option("--Select--", ""));

        if (modelName) {
            debugger
            $.ajax({
              
               url:actionurl._GetSizesByModelName,
                type: 'GET',
                data: { modelName: modelName },
                success: function (data) {
                    debugger
                    if (data.length > 0) {

                        data.forEach(item => {

                            if (sizeSelect.find(`option[value='${item.size}']`).length === 0) {
                                sizeSelect.append(new Option(item.size, item.size));
                            }
                        });


                        const sizeOption = sizeSelect.find(`option[value='${size}']`);
                        if (sizeOption.length > 0) {
                            sizeSelect.val(size);
                        } else {
                            sizeSelect.val("");
                            console.warn("Size not found in options:", size);
                        }
                    }
                    sizeSelect.prop('disabled', false);
                },
                error: function () {
                    Swal.fire({
                        title: "Error",
                        text: "Failed to load sizes for the selected model.",
                        icon: "error"
                    });
                    sizeSelect.prop('disabled', false);
                }
            });
        } else {
            sizeSelect.prop('disabled', false);
        }
    }

    $('#ModelName').change(function () {
        const selectedModel = $(this).val();
        loadAndSetSize(selectedModel, ""); // Load sizes but do not set a value on manual change
    });
});


function SaveModelInfo(type) {
    var formData = $('#modelForm').serialize(); // Use jQuery to serialize the form
    // Append Id if it exists for update
    if (editedId) {
        formData += '&Id=' + editedId;
    }

   
    var url = type === "add" ? actionurl._SaveModelInfo :  actionurl._UpdateModelInfo;

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: `Model Information ${(type === "add") ? "saved" : "updated"} successfully.`,
                    icon: "success"
                }).then(function () {
                   
                    window.location.href = actionurl._Index;
                });
            } else {
                showError(response.message || "An error occurred.");
            }
        },
        error: function () {
            showError("Something went wrong.");
        }
    });
}

function showError(message) {
    Swal.fire({
        title: "Error",
        text: message,
        icon: "error"
    });
}