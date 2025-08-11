 

jQuery(document).ready(function () {

    $('.tile_img').click(function () {
        var $img = $(this);
        $('#divLargerImage').html($img.clone()).add($('#divOverlay')).fadeIn();
    });

    $('#divLargerImage').add($('#divOverlay')).click(function () {
        $('#divLargerImage').add($('#divOverlay')).fadeOut(function () {
            $('#divLargerImage').empty();
        });
    });

    if (performance.navigation.type == 2) {
        refreshRadiobuttonvalue();
        getradiobuttonvalue();
    }
    $("#chatbotloader").click(function () {
        document.getElementById("loader").style.visibility = "visible";
    });
});
const messages = document.getElementById('messages');
document.addEventListener('DOMContentLoaded', function () {
    refreshRadiobuttonvalue();
    getradiobuttonvalue();
});
function getMessages() {
    shouldScroll = messages.scrollTop + messages.clientHeight === messages.scrollHeight;
    if (!shouldScroll) {
        scrollToBottom();
    }
}

function scrollToBottom() {
    messages.scrollTop = messages.scrollHeight;
}
scrollToBottom();

setInterval(getMessages, 1000);

function getcheckboxvalue() {
    var checkbox = document.getElementById("formCheck3");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    return checkboxValue;
}

function checkboxcount() {
    var count = 0;
    var checkbox = document.getElementById("knowledgemining");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        count = count + 1;
    }
    var checkbox = document.getElementById("sentimentanalysis");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        count = count + 1;
    }
    var checkbox = document.getElementById("entityextraction");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        count = count + 1;
    }
    var checkbox = document.getElementById("docprocessing");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        count = count + 1;
    }
    var checkbox = document.getElementById("agentchatbot");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        count = count + 1;
    }
    return count;
}
function getcapabilityvalue(capability) {
    var checkbox = document.getElementById("knowledgemining");
    checkboxValue = checkbox.checked ? checkbox.value : "False";
    if (checkboxValue == "True") {
        document.getElementById("knwldmine").style.display = "block";
    } else {
        document.getElementById("knwldmine").style.display = "none";
    }

    var checkbox1 = document.getElementById("sentimentanalysis");
    checkboxValue1 = checkbox1.checked ? checkbox1.value : "False";
    if (checkboxValue1 == "True") {
        document.getElementById("sentanaly").style.display = "block";
    } else {
        document.getElementById("sentanaly").style.display = "none";
    }

    var checkbox2 = document.getElementById("entityextraction");
    checkboxValue2 = checkbox2.checked ? checkbox2.value : "False";
    if (checkboxValue2 == "True") {
        document.getElementById("entextr").style.display = "block";
    } else {
        document.getElementById("entextr").style.display = "none";
    }

    var checkbox3 = document.getElementById("docprocessing");
    checkboxValue3 = checkbox3.checked ? checkbox3.value : "False";
    if (checkboxValue3 == "True") {
        document.getElementById("docproc").style.display = "block";
    } else {
        document.getElementById("docproc").style.display = "none";
    }

    var checkbox4 = document.getElementById("agentchatbot");
    checkboxValue4 = checkbox4.checked ? checkbox4.value : "False";
    if (checkboxValue4 == "True") {
        document.getElementById("agentchat").style.display = "block";
    } else {
        document.getElementById("agentchat").style.display = "none";
    }
    var checkcount = checkboxcount();
    if (checkcount == 0) {
        document.getElementById("knwldmine").style.display = "block";
        document.getElementById("sentanaly").style.display = "block";
        document.getElementById("entextr").style.display = "block";
        document.getElementById("docproc").style.display = "block";
        document.getElementById("agentchat").style.display = "block";
        document.getElementById("industrydiv").style.display = "block";
        $('.card-ctm:after').css('display', 'none');

    }
    else {
        document.getElementById("industrydiv").style.display = "block";
    }
}
function refreshRadiobuttonvalue() {
    debugger
    const genderRadioButtons = document.getElementsByName('ind_radio');
    // var categoryValue = '@categoryValue';
    var categoryValue = 'AiAssistantModel';
    // let selectedLabel = categoryValue;
    let selectedLabel = "AiAssistantModel";
    var roleId = '@roleId'
    for (const radioButton of genderRadioButtons) {
        if (roleId == 2) {
            radioButton.disabled = false;
        } else {
            if (radioButton.value === categoryValue) {
                radioButton.checked = true;
            } else {
                radioButton.checked = false;
            }
            radioButton.disabled = true;
        }

    }
    var radioElement = document.querySelector(`input[name='ind_radio'][value='${categoryValue}']`);
    if (radioElement) {
        radioElement.disabled = false;
    }
    document.getElementById("Banking").style.display = (selectedLabel === "BankingModel") ? "block" : "none";
    document.getElementById("Ai").style.display = (selectedLabel === "AiAssistantModel") ? "block" : "none";
}
function getradiobuttonvalue() {
    debugger;
    const genderRadioButtons = document.getElementsByName('ind_radio');

    let selectedValue = null;
    let selectedLabel = null;

    for (const radioButton of genderRadioButtons) {
        if (radioButton.checked) {
            selectedValue = radioButton.value;
            selectedLabel = document.querySelector(`label[for=${radioButton.id}]`).innerText;
            break;
        }
    }
    selectedValue = "AiAssistant";
    selectedLabel = "AiAssistant";

    if (selectedLabel == "Banking") {
        document.getElementById("Banking").style.display = "block";
        document.getElementById("Ai").style.display = "none";

        var modelType = getTofindModelType();
        var pageShow = "ROI";
        //var bankingcalltranscript = '/Home/CapabilityRedirect?capability=Call Transcript' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        //$('#bankingcalltranscript').attr('href', bankingcalltranscript);
        //var _bankingcalltranscriptROI = '/Home/CapabilityRedirect?capability=Call Transcript' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        //$('#bankingcalltranscriptroi').attr('href', _bankingcalltranscriptROI);
        //var _chatbotloaderPageurl = '/Home/CapabilityRedirect?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        //$('#chatbotloader').attr('href', _chatbotloaderPageurl);
        //var chatbotLoaderroi = '/Home/CapabilityRedirect?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        //$('#chatbotloaderroi').attr('href', chatbotLoaderroi);
        //var _CustomerInteractionUrl = '/Home/CapabilityRedirect?capability=PCI' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        //$('#_customerInteraction').attr('href', _CustomerInteractionUrl);
        //var _CustomerInteractionroi = '/Home/CapabilityRedirect?capability=PCI' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        //$('#_customerInteractionroi').attr('href', _CustomerInteractionUrl);
        var bankingcalltranscript = actionurl._CapabilityRedirect +'?capability=Call Transcript' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        $('#bankingcalltranscript').attr('href', bankingcalltranscript);
        var _bankingcalltranscriptROI = actionurl._CapabilityRedirect+'?capability=Call Transcript' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        $('#bankingcalltranscriptroi').attr('href', _bankingcalltranscriptROI);
        var _chatbotloaderPageurl =actionurl._CapabilityRedirect +'?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        $('#chatbotloader').attr('href', _chatbotloaderPageurl);
        var chatbotLoaderroi = actionurl._CapabilityRedirect+'?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        $('#chatbotloaderroi').attr('href', chatbotLoaderroi);
        var _CustomerInteractionUrl = actionurl._CapabilityRedirect+'?capability=PCI' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        $('#_customerInteraction').attr('href', _CustomerInteractionUrl);
        var _CustomerInteractionroi = actionurl._CapabilityRedirect +'?capability=PCI' + '&industry=' + selectedLabel + '&ModelType=' + modelType + '&PageShow=' + pageShow;
        $('#_customerInteractionroi').attr('href', _CustomerInteractionUrl);
    }
    else if (selectedLabel == "AiAssistant") {
        document.getElementById("Banking").style.display = "none";
        document.getElementById("Ai").style.display = "block";

        var modelType = getTofindModelType();
        //var _chatbotloaderPageurl = '/Home/CapabilityRedirect?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        //$('#AiAssistantChatbotLoader').attr('href', _chatbotloaderPageurl);
        var _chatbotloaderPageurl = actionurl._CapabilityRedirect+'?capability=Agent Chatbot' + '&industry=' + selectedLabel + '&ModelType=' + modelType;
        $('#AiAssistantChatbotLoader').attr('href', _chatbotloaderPageurl);
    }
    return selectedLabel;
}
function getTofindModelType() {
    debugger
    const genderRadioButtons = document.getElementsByName('ind_radio');
    let selectedValue = null;
    for (const radioButton of genderRadioButtons) {
        if (radioButton.checked) {
            selectedValue = radioButton.value;
            localStorage.setItem("_lableIndustry", selectedValue);
            break;
        }
    }
    return selectedValue;
}
