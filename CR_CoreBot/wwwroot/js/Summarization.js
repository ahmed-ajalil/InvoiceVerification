 
function fnSummarizechanged() {
    document.getElementById('displayfileUpload').style.display = 'block';
    document.getElementById('multipleupload').style.display = 'none';
    document.getElementById('divRepository').style.display = 'none';
    $("#divSummery").css("display", "none");
    $('#ConnectionOption').css('display', 'block');
    $('#BulkListItem').css('display', 'none');
}
function fnClassifychanged() {
    document.getElementById('multipleupload').style.display = 'block';
    document.getElementById('displayfileUpload').style.display = 'none';
    document.getElementById('divRepository').style.display = 'none';
    $("#divSummery").css("display", "none");
    $('#ConnectionOption').css('display', 'block');
    $('#BulkListItem').css('display', 'none');
}
function fnRepository() {
    document.getElementById('divRepository').style.display = 'block';
    document.getElementById('displayfileUpload').style.display = 'none';
    document.getElementById('multipleupload').style.display = 'none';
    $("#divSummery").css("display", "none");
    $('#ConnectionOption').css('display', 'block');
    $('#BulkListItem').css('display', 'none');
}

$('[name=storageType]').each(function (i, d) {
    var p = $(this).prop('checked');
    //   console.log(p);
    if (p) {
        $('.rd-grp').eq(i)
            .addClass('on');
    }
});

$('[name=storageType]').on('change', function () {
    var p = $(this).prop('checked');
    var i = $('[name=storageType]').index(this);

    $('.rd-grp').removeClass('on');
    $('.rd-grp').eq(i).addClass('on');
});

$(document).ready(function () {
    $('.file-input-file').on('change', function (event) {
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            $("<div class='file-value'><div class='file-value-text'>" + file.name + "</div><div class='file-value-remove' data-id='" + file.name + "' ></div></div>").insertAfter('#file-input');
        }
    });

});

var temp = 0;
var objModel = {};
var _Classify = $("#formRadiosExtract").val();
$(document).ready(function () {
    $('#tablist1').hide();

    $("p").last().addClass("selected highlight");
});

$(function () {
    var obj = $("#PropmpInputExtract").val('Provide a summary of the text below that captures its main idea');
    //obj.html(obj.html().replace(/\n/g, '<br/>'));
    $("#PropmpInput").focus();
    $("#btnclear").click(function () {
        $("#MainChatDiv").empty();
    });
    function fnloaderData() {
        temp++;
        //$("#ExtractData").append('<li> <div class="conversation-list">' +
        //    '<div id="blockerMigloader' + temp + '">' +
        //    '<div id="loading">' +
        //    '<div class="loaderclass" style="background-color: none; padding: 5px;">' +
        //    '<div><span>Typing</span><div class="spinner-grow text-dark m-1" role="status">' +
        //    '<span class="sr-only">Loading...</span></div><div class="spinner-grow text-dark m-1" role="status">' +
        //    '<span class="sr-only">Loading...</span></div><div class="spinner-grow text-dark m-1" role="status">' +
        //    '<span class="sr-only">Loading...</span></div></div>' +
        //    '</div>' +
        //    '</div>' +
        //    '</div>' +
        //    '</div>' +
        //    '</li>');
        $("#summarizData").append('<li> <div class="conversation-list">' +
            '<div id="blockerMigloader' + temp + '">' +
            '<div id="loading">' +
            '<div class="loaderclass" style="background-color: none; padding: 5px;">' +
            '<div><span>Typing</span><div class="spinner-grow text-dark m-1" role="status">' +
            '<span class="sr-only">Loading...</span></div><div class="spinner-grow text-dark m-1" role="status">' +
            '<span class="sr-only">Loading...</span></div><div class="spinner-grow text-dark m-1" role="status">' +
            '<span class="sr-only">Loading...</span></div></div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</li>');
    }

    $("#btnSubmitData").click(function () {
        var selectValue = $('#DocumentBot :selected').text();
        $("#summarizData").empty();
        objModel.PropmpInputExtract = $("#PropmpInputExtract").val();
        objModel.ExtractData = $("#ExtractDataPreviewData").text();
        objModel.DocumentBot = selectValue;
        objModel.DataSource = "manual";
        objModel.ActionType = "Summarize";
        fnloaderData();
        $.ajax({
            type: "POST",
            /*url: "/Summerization/Summarization",*/
            URL:actionurl._Summarization,
            data: objModel,
            success: function (responseData) {
                $("#summarizData").empty();
                if (objModel.ActionType == "Summarize") {
                    fnOutputdataSummery(responseData.summeryResult)
                    fnOutputdataPreview()
                }
            },
            failure: function (response) {
                $("#blockerMigloader" + temp + "").css("display", "none");
                alert(response.responseText);
            },
            error: function (response) {
                $("#blockerMigloader" + temp + "").css("display", "none");
                alert(response.responseText);
            }
        });
    });
    function fnOutputdataExtract(data) {
        $("#blockerMigloader" + temp + "").css("display", "none");
        //$("#ExtractData").append('<li> <div class="conversation-list">' +
        //    '<p style="margin-left:10px;">' + data + '</p>' +
        //    '</div>' +

        //    '</li>');
        $('#PropmpInput').val('');
    }
    function fnOutputdataSummery(data) {
        $("#summarizData").append('<p style="margin-left:10px;">' + data + '</p>');
    }
    function fnOutputdataPreview() {
        $('#divPrompt').text('Prompt: You must extract entities based on below.');
        $("#divSummery").css("display", "block");
        $("#displayfileUpload").css("display", "none");
    }
    function fnOutputdataClassify(data) {
        $("#blockerMigloader" + temp + "").css("display", "none");
        //$("#ExtractData").append('<li> <div class="conversation-list">' +
        //    '<p style="margin-left:10px;">' + data + '</p>' +
        //    '</div>' +
        //    '</li>');
        $("#summarizData").append('<li> <div class="conversation-list">' +
            '<p style="margin-left:10px;">' + data + '</p>' +
            '</div>' +
            '</li>');
        $('#PropmpInput').val('');
    }
});
function showVal(newVal) {
    document.getElementById("valBox").innerHTML = newVal;
}
function showValTopp(newVal) {
    document.getElementById("topp").innerHTML = newVal;
}
function showValdoc(newVal) {
    document.getElementById("valBoxdoc").innerHTML = newVal;
}
function showValToppdoctop(newVal) {
    document.getElementById("valBoxdocTopp").innerHTML = newVal;
}