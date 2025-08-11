 


let boxCounter = 1; // Box counter starts at 1 because we already have the first box
const BoxLimit = 6; // Set Maximum box limit

function addBox() {
    if (boxCounter >= BoxLimit) {
        Swal.fire({
            title: "Limit Reached",
            text: "Only " + BoxLimit + " Question and Query can be added.",
            icon: "warning"
        });
        return;
    }

    // Increment the box counter
    boxCounter++;

    // Create new box HTML
    let CreateBox = `<div id="box_${boxCounter}" class="row g-2 align-items-end mb-4">
                                              <div class="col">
                                                  <div class="shadow bg-white p-3 rounded-3 border">
                                                      <div class="form-floating mb-3">
                                                                  <input type="text" class="form-control Question" name="Questions[${boxCounter - 1}].Question" id="Question_${boxCounter}" placeholder="Enter Question" />
                                                          <label for="Question_${boxCounter}">Question</label>
                                                      </div>
                                                      <div class="form-floating">
                                                                  <input type="text" class="form-control SQLQuery" name="SQLQuery[${boxCounter - 1}].SQLQuery" id="SQLQuery_${boxCounter}" placeholder="Enter SQL Query" />
                                                          <label for="Question_${boxCounter}">SQL Query</label>
                                                     </div>
                                                  </div>
                                              </div>
                                              <div class="col-2">
                                                  <div class="pm-controls">
                                                          <a href="javascript:void(0);" onclick="addBox()" style="background-color: #127d91;"><img src="${baseUrl}plus.png" /></a>
                                                          <a href="javascript:void(0);" onclick="removeBox(${boxCounter})" style="background-color: #127d91;"><img src="${baseUrl}minus.png"/></a>
                                                  </div>
                                              </div>
                                          </div>`;


    // Append the new box
    $("#Boxes").append(CreateBox);
    htmlscrollToBottom();

    // Hide "add" and "delete" buttons in all previous boxes
    $("#Boxes .row").each(function () {
        $(this).find(".pm-controls a:first-child").hide(); // Hide "add"
        $(this).find(".pm-controls a:last-child").hide();  // Hide "delete"
    });

    // Show both "add" and "delete" buttons only in the latest box
    $(`#box_${boxCounter} .pm-controls a:first-child`).show(); // Show "add"
    $(`#box_${boxCounter} .pm-controls a:last-child`).show();  // Show "delete"
}

function htmlscrollToBottom() {
    var TrainModeldiv = document.getElementById('TrainModeldiv');
    // Check if user is near the bottom
    if (!TrainModeldiv.scrollHeight - TrainModeldiv.scrollTop <= TrainModeldiv.clientHeight + 100) {
        TrainModeldiv.scrollTop = TrainModeldiv.scrollHeight;
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
            url: actionurl._GetEditedTrainModelInformationData +'?Id=' + editedId,
            method: 'GET',
            success: function (response) {
                // Assuming response is an array of questions
                response.forEach((item, index) => {
                    if (index === 0) {
                        $("#Question_0").val(item.question);
                        $("#SQLQuery_0").val(item.sqlquery);
                    } else {
                        addBox();
                        $(`#Question_${index}`).val(item.question);
                        $(`#SQLQuery_${index}`).val(item.sqlquery);
                    }
                });
            },
            error: function () {
                Swal.fire({
                    title: "Error",
                    text: "Failed to load Question and Query.",
                    icon: "error"
                });
            }
        });
    }
});

function SaveTrainModelInfo(type) {
    // Create an array of TrainModelDTO objects
    var trainModels = [];

    // Loop through all boxes with the class '.row' inside the '#Boxes' container
    $('#Boxes .row').each(function (index, element) {
        var question = $(element).find('.Question').val();
        var sqlQuery = $(element).find('.SQLQuery').val();

        // Check if both question and sqlQuery have values
        if (question && sqlQuery) {
            trainModels.push({
                Id: 0, // Set Id to 0 for new entries; or use the actual Id if you're editing an existing one
                Question: question,
                SQLQuery: sqlQuery
            });
        }
    });

    // If no models to save, display an error
    if (trainModels.length === 0) {
        Swal.fire({
            title: "Error",
            text: "Please add at least one question and query.",
            icon: "error"
        });
        return;
    }

  
    var url = type === "add" ? actionurl._SaveTrainModelInfo : actionurl._UpdateTrainModelInfo;

    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json', // Set content type to JSON
        data: JSON.stringify(trainModels), // Send data as JSON array
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: "Train Model " + (type === "add" ? "saved" : "updated") + " successfully.",
                    icon: "success"
                }).then(function () {
                    window.location.href = actionurl._TrainModelInfoList;
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
