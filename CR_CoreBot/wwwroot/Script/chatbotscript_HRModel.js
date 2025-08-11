$(document).ready(function () {
    var _lableIndustry = "HRModel";
    if (_lableIndustry != "BankingModel") {
        $('#id_PersonalBanking').html("");
        $('#id_PersonalBanking').css({
            'width': '0',
            'height': '0',
            'overflow': 'hidden'
        });
    }
    if (_lableIndustry == "BankingModel") {
        $('#id_PersonalBanking').append("<label style='padding: 0; margin: 0; margin-top: 4px!important;'><b>Personal Banking</b></label>" +
            "<input type = 'checkbox' id = 'toggle' onchange = 'getToggleButtonValue()' class='toggle-input'>" +
            "<label for='toggle' class='toggle-label' style='width:75px;margin-left:140px;'>" +
            "<span class='toggle-text'>ON</span>" +
            "<span class='toggle-text''>OFF</span>" +
            "</label>");
    }
    $("#myselect").append("<option value='" + _lableIndustry + "'>" + _lableIndustry + "</option>");
    $("#hyperInputDiv").val("Assistant is an intelligent chatbot designed to help users answer their HR related questions." +
        "Instructions:" + "-Only answer questions related to HR." + "-If you're unsure of an answer, you can say I don't know or I'm not sure and recommend users go to the HR documentation for more information.");
});
function getToggleButtonValue() {
    var toggle = document.getElementById("toggle");
    toggleValue = toggle.checked;

    if (toggleValue) {
        console.log("Toggle is ON");
    } else {
        console.log("Toggle is OFF");
    }
    return toggleValue;
}
function fnchatExport() {
    var UserChat = $('.Usermsg-text').text();
    var BotReply = $('.ChatBotmsg-text').text();
    var OrganizationName = $("#OrganizationName").val();
    $.ajax({
        url: "/Home/fnALLChatbot/",
        type: "POST",
        data: { UserChat: UserChat, BotReply: BotReply, OrganizationName: OrganizationName },
        async: false,
        success: function (responseData) {
            var link = document.createElement('a');
            link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(responseData);
            link.download = 'ChatBot.txt';
            link.style.display = 'none';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        failure: function (responseData) {
        },
        error: function (responseData) {
        }
    })
}
$(window).on('unload', function () {
    var UserChat = $('.Usermsg-text').text();
    var BotReply = $('.ChatBotmsg-text').text();
    var OrganizationName = $("#OrganizationName").val();
    $.ajax({
        url: "/Home/fnSessionEnd/",
        type: "POST",
        data: { UserChat: UserChat, BotReply: BotReply, OrganizationName: OrganizationName },
        async: true,
        success: function (responseData) {
        },
        failure: function (responseData) {
        },
        error: function (responseData) {
        }
    })

});

function fnsuggestion1() {
    var txtSuggestion = $('#messageSuggestion1').text();
    appendMessage(PERSON_NAME, PERSON_IMG, "right", txtSuggestion, "", "");
    botResponse(txtSuggestion);
}
function fnsuggestion2() {
    var txtSuggestion = $('#messageSuggestion2').text();
    appendMessage(PERSON_NAME, PERSON_IMG, "right", txtSuggestion, "", "");
    botResponse(txtSuggestion);
}
function fnClearChat() {
    $('.ctext-wrap').empty();
    $('#chatBotDIV').empty();
    $('#clearmessageSuggestionDIV').append("<div class='ctext-wrap'><div class='d-flex chat-box-suggestion' id='messageSuggestionDIV'></div> </div>");
    $.ajax({
        url: "/Home/ClearChat/",
        type: "POST",
        async: true,
        success: function (responseData) {
        },
        failure: function (responseData) {
        },
        error: function (responseData) {
        }
    })

}
function copy_data(containerid, flag) {
    var idcheck = "check" + flag;
    var idclipboard = "clipboard" + flag;
    $('#' + idclipboard + '').css("display", "none");
    $('#' + idcheck + '').css("display", "block");
    var range = document.createRange();
    range.selectNode(containerid); //changed here
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(range);
    document.execCommand("copy");
    window.getSelection().removeAllRanges();
    setTimeout(
        function () {
            $('#' + idclipboard + '').css("display", "block");
            $('#' + idcheck + '').css("display", "none");
        }, 1000);

    //alert("data copied");
}

