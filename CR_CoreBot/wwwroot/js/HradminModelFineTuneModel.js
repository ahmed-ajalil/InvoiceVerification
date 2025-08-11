 

$(document).ready(function () {
    $('#tblResponse').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        //"scrollX": true,
        "scrollX": '100%',
        order: [],
        "dom":
            "<''<''<'DtblTopCntrlBox'<'DtblTopCntrlInn'l><'DtblTopCntrlInn'f><'DtblTopCntrlInn'p>>>" +
            "<''<''tr>>" +
            "<' pt-1'<'col-sm-5'i><'col-sm-7 d-none'p>>",
        "language": {
            "lengthMenu": "Display _MENU_ ",
            searchPlaceholder: "Search records",
            search: "_INPUT_",
        },
    });
});



$('.long-text').each(function () {
    var content = $(this).text();
    var showChar = 100;
    var ellipsestext = "";
    var moretext = "Show More";
    var lesstext = "Show Less";

    if (content.length > showChar) {
        var c = content.substr(0, showChar);
        var h = content.substr(showChar, content.length - showChar);

        var html = c + '<span class="moreellipses">' + ellipsestext + '</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';

        $(this).html(html);
    }
});

$(document).on("click", ".morelink", function (event) {
    event.preventDefault();
    if ($(this).hasClass("less")) {
        $(this).removeClass("less");
        $(this).html("Show More");
    } else {
        $(this).addClass("less");
        $(this).html("Show Less");
    }
    $(this).prev().toggle();
    $(this).prev().prev().toggle();
});

jQuery(document).ready(function () {

    //$("#Updatebtn").hide();
    //$("#Insertbtn").show();

    // Bind grid data
    function bindData() {
        $.ajax({
         
            url:actionurl._GetModelfinetunedata,
            method: 'GET',
            success: function (response) {
                var data = response;
                $.each(data, function (index, item) {
                    $("#tblResponse").append(data);
                });
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    bindData();

    // bind model name
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
            },
            error: function (error) {
                console.error(error);
            }
        });
    }

    // bindModelName();
});

function editTag(id) {
    var completionField = document.getElementById(id);
    var completionText = completionField.textContent.trim();
    var uniqueId = "tagInput" + id;
    // completionField.innerHTML = '<input type="text" class="text-input"  id="' + uniqueId + '" value="' + completionText + '">';
    completionField.innerHTML = '<textarea class="text-input" id="' + uniqueId + '">' + completionText + '</textarea>';

    // Show the save button and hide the edit button
    document.getElementById("editButton" + id).style.display = "none";
    document.getElementById("saveButton" + id).style.display = "inline";
}
function SaveNewCompletion(itemId) {
    var newtext = document.getElementById("tagInput" + itemId).value;
    $.ajax({
        
        url:actionurl._AddNewCompletionResult,
        type: 'POST',
        data: { id: itemId, text: newtext },
        success: function (response) {
            if (response === "Success") {
                var completionId = document.getElementById(itemId); // Moved this line inside the success block
                completionId.innerHTML = newtext;
                document.getElementById("saveButton" + itemId).style.display = "none";
                document.getElementById("editButton" + itemId).style.display = "inline";
                Swal.fire({
                    title: "Success",
                    text: "Data Saved Successfully",
                    icon: "success"
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        }
    });
}

function editmodelfinetunedatanxt(Id) {
    debugger;
    var id = Id;
    $("#Updatebtn").show();
     window.location.href = '@Url.Action("EditModelFineTuneData", "HRadmin")' + '?Id=' + id;
     
}