 

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

        // On Edit Model Name dropdown disabled
        if (editedid != "" || editedid != undefined && editedid != 0) {
            $("#ModelName").prop("disabled", true);
        }

        // When page is opened for update operation
        if (editedid != 0 && editedid != undefined || editedid != "") {
            $("#Updatebtn").show();
            $("#Insertbtn").hide();
            $.ajax({
               
                url: actionurl._GetEditModelFineTuneData+'?Id=' + editedid,
                method: 'GET',
                async: false,
                success: function (response) {
                    var data = response;
                    debugger
                    console.log(data);
                    $("#prompt").val(data[0].prompt);
                    $("#completion").val(data[0].completion);
                    $("#label").val(data[0].labelName);
                    $("#ModelName").val(data[0].modelName);



                    $("#url").val(data[0].url);
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
        }
    }

    // When no query string.
    else {
        $("#Updatebtn").hide();
        $("#Insertbtn").show();
    }

    // function to get whole data on page load
    function bindData() {
        $.ajax({
           
            url:actionurl._GetModelfinetunedatat,
            method: 'GET',
            success: function (response) {
                var data = response;
                $.each(data, function (index, item) {
                    $("#tblResponse").append(data);
                    //  var newRow = $("<tr>");
                    //   newRow.append($("<td>").text(item.Prompt));
                    //   newRow.append($("<td>").text(item.Completion));
                    // Append more cells if needed
                    //    $("#tblResponse tbody").append(newRow);
                });
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    bindData();

    // Bind model name dropdown from database in both insert or update case
    function bindModelName() {
        $.ajax({
            
            url:actionurl._bindModelName,
            method: 'GET',
            success: function (response) {
                var modelname = response;
                var newOption = $("<option>");
                newOption.val(modelname);
                newOption.text(modelname);
                $("#ModelName").append(newOption);
                $('#ModelName option:eq(1)').prop('selected', true);
            },
            error: function (error) {
                console.error(error);
            }
        });
    }

    bindModelName();
});

var id;

// Update Code Js function
function UpdateModelFineTuneData() {
    debugger;
    var Id = parseInt(editedid);
    var prompt = $("#prompt").val();
    var completion = $("#completion").val();
    var label = $("#label").val();
    var ModelName = $("#ModelName").val();
    var url = $("#url").val();

    if (prompt == "" || prompt == undefined) {
        new swal({
            title: "Error",
            text: "Prompt can't be empty",
            icon: "error"
        });
    }

    else if (completion == "" || completion == undefined) {
        new swal({
            title: "Error",
            text: "Completion can't be empty",
            icon: "error"
        });
    }

    else if (label == "" || label == undefined) {
        new swal({
            title: "Error",
            text: "Label name can't be empty",
            icon: "error"
        });
    }
    else if (url == "" || url == undefined) {
        new swal({
            title: "Error",
            text: "Document Reference can't be empty",
            icon: "error"
        });
    }



    else {
        $.ajax({
             
           url:actionurl._UpdateModelFineTuneData,
            type: 'POST',
            async: false,
            data: { Id: Id, Prompt: prompt, Completion: completion, LabelName: label, ModelName: ModelName, Url: url },
            success: function (response) {
                debugger
                var data = response;

                if (data == true) {
                    alert("Model fine tune data updated successfully");
                    
                    window.location.href = '@Url.Action("ModelFineTuneModel", "HRadmin")';
                    //new swal({
                    //    title: "Success",
                    //    text: "Model fine tune data updated successfully",
                    //    icon: "success"
                    //});


                }
                else {
                    // alert("Issues while saving Customer Information");
                    new swal({
                        title: "Error",
                        text: "Issues while updating Model fine tune data",
                        icon: "Error"
                    });
                }
            },
            error: function (error) {
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

// Insert Code Js function
function SaveModelFineTuneData() {
    debugger;
    var prompt = $("#prompt").val();
    var completion = $("#completion").val();
    var label = $("#label").val();
    var ModelName = $("#ModelName").val();
    var url = $("#url").val();

    if (prompt == "" || prompt == undefined) {
        new swal({
            title: "Error",
            text: "Prompt can't be empty",
            icon: "error"
        });
    }

    else if (completion == "" || completion == undefined) {
        new swal({
            title: "Error",
            text: "Completion can't be empty",
            icon: "error"
        });
    }

    else if (label == "" || label == undefined) {
        new swal({
            title: "Error",
            text: "Label name can't be empty",
            icon: "error"
        });
    }

    else if (ModelName == "" || ModelName == undefined) {
        new swal({
            title: "Error",
            text: "Model Name  can't be empty",
            icon: "error"
        });
    }

    else if (url == "" || url == undefined) {
        new swal({
            title: "Error",
            text: "Document Reference can't be empty",
            icon: "error"
        });
    }
    else {
        //   show_loader();
        $.ajax({
          
            url:actionurl._SaveModelFineTuneData,
            type: 'POST',
            data: { Prompt: prompt, Completion: completion, LabelName: label, ModelName: ModelName, Url: url },
            //contentType: false,
            //processData: false,
            success: function (response) {
                debugger
                var data = response;
                if (data == true) {
                    alert("Model fine tune data saved successfully");
                   
                    window.location.href =  actionurl._ModelFineTuneModel;

                    
                }
                else {
                    // alert("Issues while saving Customer Information");
                    new swal({
                        title: "Error",
                        text: "Issues while saving Model fine tune data",
                        icon: "Error"
                    });
                }


                //         hide_loader();

            },
            //error: function (jqXHR, textStatus, errorThrown) {
            //    console.log(textStatus, errorThrown);
            //}
            error: function (error) {
                //  hide_loader();
                //  alert("Something went wrong");
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