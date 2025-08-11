 

var editedId;
 
jQuery(document).ready(function () {
  
    // Check if there's an Id in the query string
    if (window.location.search) {
        const params = new URLSearchParams(window.location.search);
        editedId = params.get("Id");
        editedId = parseInt(editedId);

        if (editedId) {
            $("#Updatebtn").show();
            $("#Insertbtn").hide();

            $.ajax({
            
                url: actionurl._GetEditedSystemInstructionInformationData+'?Id=' + editedId,
                method: 'GET',
                success: function (response) {
                    if (response && !response.error) {
                        $("#Rules").val(response.rules); // Ensure `response.rules` and `response.industry` exist
                        $("#Industry").val(response.industry);
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: response.error || "Failed to load System Instruction.",
                            icon: "error"
                        });
                    }
                },
                error: function (error) {
                    console.error(error);
                    Swal.fire({
                        title: "Error",
                        text: "Failed to load System Instruction.",
                        icon: "error"
                    });
                }
            });
        }
    } else {
        $("#Updatebtn").hide();
        $("#Insertbtn").show();
    }
});

function SaveSystemInstructionInfo(type) {
    debugger
    var formData = $('#modelForm').serialize(); // Use jQuery to serialize the form
    // Append Id if it exists for update
    if (editedId) {
        formData += '&Id=' + editedId;
    }

    var url = type === "add" ? actionurl._SaveSystemInstructionInfo : actionurl._UpdateSystemInstructionInfo;
    debugger
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                debugger
                Swal.fire({
                    title: "Success",
                    text: "System Instruction " + (type === "add" ? "saved" : "updated") + " successfully.",
                    icon: "success"
                }).then(function () {
                    window.location.href = actionurl._SystemInstructionInfoList;
                });
            } else {
                Swal.fire({
                    title: "Error",
                    text: response.message || "An error occurred.",
                    icon: "error"
                });
            }
        },
        error: function () {
            Swal.fire({
                title: "Error",
                text: "Something went wrong.",
                icon: "error"
            });
        }
    });
}