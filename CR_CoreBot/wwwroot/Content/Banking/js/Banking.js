$(document).ready(function () {
   loadInteractionLogChart();
});


function loadInteractionLogChart() {
    debugger
    var customerId = parseInt(document.getElementById('customerId').dataset.customerId);
    $.ajax({
        url: '/Banking/Customer/GetInteractionLogChartData',
        method: 'Post',
        data: { CustomerId: customerId },
        success: function (data) {           
            var chartData = data.map(function (item) {
                return {
                    name: item.websiteVisited,
                    y: item.appUsageDuration
                };
            });
            console.log(chartData);
            
                Highcharts.chart('demochart', {
                chart: {
                   plotBackgroundColor: null,
                   height:200,
                   padding:[0,0,0,0],
                   plotBorderWidth: null,
                   plotShadow: false,
                   type: 'pie'
                },
                title: {
                   text: '',
                   align: 'left'
                },
                tooltip: {
                   pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                },
                navigation: {
                   buttonOptions: {
                      enabled: false
                   },
                },
                accessibility: {
                   point: {
                      valueSuffix: '%'
                   }
                },
                plotOptions: {
                   pie: {
                      allowPointSelect: true,
                      cursor: 'pointer',
                      dataLabels: {
                         enabled: false
                      },
                      showInLegend: true
                   }
                },
                credits: {
                enabled: false
               },
                    series: [{
                            name: 'Websites',
                            colorByPoint: true,
                            data: chartData
                        }]
             });
        },
        error: function (error) {
            console.error('Error fetching data:', error);
        }
    });
}
function makeAjaxRequest(url, type, data) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: type,
            data: data,
            success: function (response) {
                resolve(response);
            },
            error: function (xhr, status, error) {
                reject({ xhr: xhr, status: status, error: error });
            }
        });
    });
}

function previewAudiofile() {

    recognition.stop();
    $('#exampleFormControlTextarea').val('');
    $('#exampleFormControlTextarea').attr('placeholder', ' ');
    $('#tblsummary').val('');
    $('#errorTextarea1').hide();
    $("#div_Entity").html("");
    $("#_idReferenceDoc").html("");
    $("#div_keyPhrase").html("");
    $("#_sentementValue").text(0.5);
    $(".indicator").css("--left-value", "235px");
    $(".indicator").addClass("indicator-moved");
    $(".indicator span").css("left", "235px");
    $("#exampleFormControlTextarea").prop('disabled', true);

    var value = $('#audioInput').val();
    value.substring(value.lastIndexOf("\\") + 1, value.length);
    var files = $('#audioInput').prop("files");
    formData = new FormData();
    formData.append("fileName", files[0]);
    $.ajax({
        url: "/Banking/Banking/PreviewAudioFile/",
        type: "POST",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (responseData) {
            var source = document.getElementById('audioSource').setAttribute('src', responseData);
            var audio = document.getElementById('audio');
            audio.load();
        },
        failure: function (responseData) {
            console.log(responseData);
        },
        error: function (responseData) {
            console.log(responseData);
        }
    })
}

var ScopeStatus = false;
var _lableIndustry = "";
var recognition = new (webkitSpeechRecognition || SpeechRecognition)();
function uploadAudioFile() {
    
    var audioFile = document.getElementById('audioInput').files[0];
    var formData = new FormData();
    formData.append('audio_file', audioFile);
    formData.append('ScopeStatus', ScopeStatus);
    formData.append('_lableIndustry', _lableIndustry);
    $.ajax({
        url: "/Banking/Banking/genrateText/",
        type: "POST",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            bind_Text(response);
            $("#exampleFormControlTextarea").val(response.transcript);
            $("#exampleFormControlTextarea").prop('disabled', false);
            hide_loader();
        },
        error: function (responseData) {
            hide_loader();
        }
    });
}

function funbtnsubmit() {
    clear_All();
    show_loader();
    setTimeout(function () {
        var textarea = $('#exampleFormControlTextarea').val();
        if ($("#exampleFormControlTextarea").prop('disabled')) {
            uploadAudioFile();
        }
        else if ((textarea == "" || textarea == undefined || textarea == null) && !$("#exampleFormControlTextarea").prop('disabled')) {

            $('#errorTextarea1').show();
            hide_loader();
        }
        else {
            debugger
            textarea = $('#exampleFormControlTextarea').val();
            $.ajax({
                url: '/Banking/Banking/getAnalyticsHelper',
                type: 'POST',
                dataType: 'json',
                data: { text: textarea, ScopeStatus: ScopeStatus, _lableIndustry: _lableIndustry },
                async: false,
                success: function (response) {
                    bind_Text(response)
                    hide_loader();
                },
                error: function (error) {
                    console.error(error);
                    hide_loader();
                }
            });
        }
    }, 500);
}

function clear() {
    recognition.stop();
    $('#audioInput').val('');
    $('#audioSource').attr('src', null);
    $('#audio')[0].load();
    $('#exampleFormControlTextarea').val('');
    $('#tblsummary').val('');
    $('#errorTextarea1').hide();
    $("#div_Entity").html("");
    $("#_idReferenceDoc").html("");
    $("#div_keyPhrase").html("");
    $("#_sentementValue").text(0.5);
    $(".indicator").css("--left-value", "235px");
    $(".indicator").addClass("indicator-moved");
    $(".indicator span").css("left", "235px");
    $("#exampleFormControlTextarea").prop('disabled', false);
    $('#lblFirstExtractin').val('');
    $('#lblSecondExtractin').val('');
    $('#txtFirstExtractin').val('');
}
function clear_All() {
    recognition.stop();
    $('#tblsummary').val('');
    $('#errorTextarea1').hide();
    $("#div_Entity").html("");
    $("#_idReferenceDoc").html("");
    $("#div_keyPhrase").html("");
    $("#_sentementValue").text(0.5);
    $(".indicator").css("--left-value", "235px");
    $(".indicator").addClass("indicator-moved");
    $(".indicator span").css("left", "235px");
    $('#lblFirstExtractin').val('');
    $('#lblSecondExtractin').val('');
    $('#txtFirstExtractin').val('');
}
function bind_Text(response) {
    $.each(response.entityRole, function (key, value) {
        $('#div_Entity').append("<div class='my-2 p-1 rounded'> <p class='m-0'> " + value + " <span class='float-end'> </span></p> </div>");

    });
    var labelitem = "";
    $.each(response._documentName, function (index, value) {
        var link = $("<a></a>")
            .attr("href", response.documentSourceURL[index])
            .attr("target", "_blank")
            .css("border", "1px solid #c8f5fa")
            .text(value);

        labelitem += link.prop('outerHTML') + "<br />";
    });
    $("#_idReferenceDoc").append(labelitem);
    $('#lbltext').val(response.language);
    var floatValue = parseFloat(response.sentiment).toFixed(2);
    $("#_sentementValue").text(floatValue);
    if (floatValue >= 0.00 && floatValue <= 0.20) {
        $(".indicator").css("--left-value", "65px");
        $(".indicator").addClass("indicator-moved");
        $(".indicator span").css("left", "65px");
    }
    else if (floatValue >= 0.20 && floatValue <= 0.40) {
        $(".indicator").css("--left-value", "125px");
        $(".indicator").addClass("indicator-moved");
        $(".indicator span").css("left", "125px");
    }
    else if (floatValue >= 0.40 && floatValue <= 0.60) {
        $(".indicator").css("--left-value", "235px");
        $(".indicator").addClass("indicator-moved");
        $(".indicator span").css("left", "235px");
    }
    else if (floatValue >= 0.40 && floatValue <= 0.60) {
        $(".indicator").css("--left-value", "330px");
        $(".indicator").addClass("indicator-moved");
        $(".indicator span").css("left", "330px");
    }
    else {
        $(".indicator").css("--left-value", "410px");
        $(".indicator").addClass("indicator-moved");
        $(".indicator span").css("left", "410px");
    }

    $.each(response.phrasesRole, function (key, value) {
        $('#div_keyPhrase').append("<div class='my-2 p-1 rounded'> <p class='m-0'> " + value + " <span class='float-end'> </span></p> </div>");
    });
    $('#tblsummary').val(response.summary);
    $('#suggestivePrompt').val(response.suggestion);
}

function show_loader() {
    document.getElementById('loader').style.visibility = "visible";
}
function hide_loader() {
    document.getElementById('loader').style.visibility = "hidden";
}

$(document).ready(function () {
    _lableIndustry = localStorage.getItem("_lableIndustry");
    var configurationText = '@Html.Raw(ViewBag.ConfigurationText)';
    var Industry = '@Html.Raw(ViewBag.Industry)';

    $('#txtCustomPrompt').text(configurationText);
    $("#_sentementValue").text(0.5);
    var speech = true;
    recognition.interimResults = true;
    recognition.continuous = true;
    recognition.addEventListener('result', function (e) {
        var transcript = Array.from(e.results)
            .map(function (result) {
                return result[0].transcript;
            })
            .join('');
        $('#exampleFormControlTextarea').val(transcript);
    });
    $('#startButton').on('click', function () {
        clear();

        setTimeout(function () {
            $('#errorTextarea1').hide();
            $('#exampleFormControlTextarea').empty();
            $('#exampleFormControlTextarea').attr('placeholder', 'licensing...');
            recognition.start();
        }, 500);
    });
});
