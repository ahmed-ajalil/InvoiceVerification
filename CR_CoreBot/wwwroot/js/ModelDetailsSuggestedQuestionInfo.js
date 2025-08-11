 

let boxCounter = 1; // Box counter starts at 1 because we already have the first box
const BoxLimit = 6; // Set Maximum box limit

function addBox() {
    if (boxCounter >= BoxLimit) {
        Swal.fire({
            title: "Limit Reached",
            text: "Only " + BoxLimit + " Question can be added.",
            icon: "warning"
        });
        return;
    }

    // Increment the box counter
    boxCounter++;

    // Create new box HTML
    let CreateBox = `<div id="box_${boxCounter}" class="row g-2 align-items-end mb-4">
                                         <div class="col-md-10">
                                             <div class="shadow bg-white p-3 rounded-3 border">
                                                 <div class="form-floating mb-3">
                                                     <input type="text" class="form-control" name="Questions[${boxCounter - 1}].Question" id="Question_${boxCounter}" placeholder="Enter Question" />
                                                     <label for="Question_${boxCounter}">Question *</label>
                                                 </div>
                                             </div>
                                         </div>
                                         <div class="col-auto">
                                             <div class="pm-controls">
                                                         <a href="javascript:void(0);" onclick="addBox()" style="background-color: #127d91;"><img src="${baseUrl}plus.png" /></a>
                                                         <a href="javascript:void(0);" onclick="removeBox(${boxCounter})" style="background-color: #127d91;"><img src="${baseUrl}minus.png"/></a>
                                             </div>
                                         </div>
                                     </div>`;

    // Append the new box
    $("#Boxes").append(CreateBox);



    // Hide "add" and "delete" buttons in all previous boxes
    $("#Boxes .row").each(function () {
        $(this).find(".pm-controls a:first-child").hide(); // Hide "add"
        $(this).find(".pm-controls a:last-child").hide();  // Hide "delete"
    });

    // Show both "add" and "delete" buttons only in the latest box
    $(`#box_${boxCounter} .pm-controls a:first-child`).show(); // Show "add"
    $(`#box_${boxCounter} .pm-controls a:last-child`).show();  // Show "delete"
    htmlscrollToBottom();
}
function htmlscrollToBottom() {
    var SuggestedQuestiondiv = document.getElementById('SuggestedQuestiondiv');
    // Check if user is near the bottom
    if (!SuggestedQuestiondiv.scrollHeight - SuggestedQuestiondiv.scrollTop <= SuggestedQuestiondiv.clientHeight + 100) {
        SuggestedQuestiondiv.scrollTop = SuggestedQuestiondiv.scrollHeight;
    }
}

function removeBox(getBoxID) {
    $("#box_" + getBoxID).remove();
    reindexBoxes();
}

function reindexBoxes() {
    let count = 0;
    $("#Boxes .row").each(function () {
        $(this).find("input").attr("name", `Questions[${count}].Question`);
        $(this).find("input").attr("id", `Question_${count}`);
        count++;
    });
    boxCounter = count;


    $("#Boxes .row").each(function () {
        $(this).find(".pm-controls a:first-child").hide();
        $(this).find(".pm-controls a:last-child").hide();
    });


    if (boxCounter > 0) {
        $(`#box_${boxCounter} .pm-controls a:first-child`).show();
        $(`#box_${boxCounter} .pm-controls a:last-child`).show();
    }
}


jQuery(document).ready(function () {
    $("#Updatebtn").hide();
    $("#Insertbtn").show();

    // Check if there's an Id in the query string
    const params = new URLSearchParams(window.location.search);
    const editedId = params.get("Id");

    if (editedId) {
        $("#Updatebtn").show();
        $("#Insertbtn").hide();

        $.ajax({
           
            url: actionurl._GetEditedSuggestedQuestionInformationData +'?Id=' + editedId,
            method: 'GET',
            success: function (response) {
            
                response.forEach((item, index) => {
                    if (index === 0) {
                        $("#Question_0").val(item.question);
                    } else {
                        addBox();
                        $(`#Question_${index}`).val(item.question);
                    }
                });
            },
            error: function () {
                Swal.fire({
                    title: "Error",
                    text: "Failed to load suggested question.",
                    icon: "error"
                });
            }
        });
    }
});

function SaveSuggestedQuestionInfo(type) {
    var formData = $('#modelForm').serialize();  
   
    var url = type === "add" ? actionurl._SaveSuggestedQuestionInfo : actionurl._UpdateSuggestedQuestionInfo;

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: "Suggested Question " + (type === "add" ? "saved" : "updated") + " successfully.",
                    icon: "success"
                }).then(function () {
                    
                     window.location.href = actionurl._SuggestedQuestionInfoList;
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