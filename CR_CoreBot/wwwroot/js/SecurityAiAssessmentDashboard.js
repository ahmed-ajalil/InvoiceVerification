 

window.onload = function () {
    $.ajax({
        
       url:actionurl._GetAiDashboardData,
        data: {},
        contentType: "application/json; charset=utf-8",
        cache: false,
        type: "Get",
        beforeSend: function () {
            $("#loader").show();
        },
        success: function (data) {
            lst = data.performanceMatrixCheckers;
            BindToxicityPromptArea(lst);
        },
        complete: function () {
            $("#loader").hide();
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function ShowPopupRemlist(val) {
    //var link = '<br><p>Learn More:<a target="_blank" href=' + $("#lbl_" + val).get(0).innerHTML + '> Go To Resource</p>';
    $('#RemlistDiv').html($("#lblBestPractices_" + val).get(0).innerHTML);
    //$('#SpanDiv').html(link);
    $("#myModalRemStep").modal("show");
}
function BindToxicityPromptArea(Applst) {
    var dbTable = '';
    if (Applst != null && Applst.length !== 0) {
        $("#aadusertable").empty();
        $('#listaad').empty();
        for (var j = 0; j < Applst.length; j++) {
            if (j == 0) {
                dbTable += ("<table id='listaad' class='table table-hover table-bordered display' cellspacing='0' width='100%'>");
                dbTable += ("<thead class='ctm-table'>");
                dbTable += ("<tr>");
                dbTable += ("<th>S No.</th>");
                dbTable += ("<th>Prompt</th>");
                dbTable += ("<th style='display:none'>Response</th>");
                dbTable += ("<th>Response</th>");
                dbTable += ("<th>Hallucination</th>");
                dbTable += ("<th>Plagiarism</th>");
                dbTable += ("<th>Prompt Injection</th>");
                dbTable += ("<th>Hate Speech</th>");
                dbTable += ("<th>Self Harm</th>");
                dbTable += ("<th>Sexual</th>");
                dbTable += ("<th>Violence</th>");
                dbTable += ("</tr>");
                dbTable += ("</thead>");
                dbTable += ("<tbody>");
            }
            dbTable += ("<tr>");
            dbTable += ("<td class='comName'>" + (j + 1) + "</td>");
            dbTable += ("<td class='comName'>" + (Applst[j].prompt == null ? "N/A" : Applst[j].prompt) + "</td>");
            //dbTable += ("<td class='comName'>" + (Applst[j].completion == null ? "N/A" : Applst[j].completion) + "</td>");
            dbTable += ("<td class='comName'  style='display:none;'><div id='lblBestPractices_" + j + "'>" + (Applst[j].completion == 'N/A' ? 'No steps found.' : Applst[j].completion) + "</td>");
            dbTable += ("<td class='comName'><span onclick=ShowPopupRemlist(" + j + ") class='btn btn-info viw btn-sm border-bottom-0'>View</span></td>");
            dbTable += ("<td class='comName'>" + (Applst[j].hallucination == null ? "N/A" : (Applst[j].hallucination == true ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;Detected (" + Applst[j].hallucinationScore + "%)</i>" : "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Not Detected</i>")) + "</td>");
            dbTable += ("<td class='comName'>" + (Applst[j].plagiarism == null ? "N/A" : (Applst[j].plagiarism == true ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;Detected</i>" : "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Not Detected</i>")) + "</td>");
            dbTable += ("<td class='comName'>" + (Applst[j].promptInjection == null ? "N/A" : (Applst[j].promptInjection == true ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;Detected</i>" : "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Not Detected</i>")) + "</td>");
            dbTable += `<td class='comName'>${Applst[j].hateSeverity == null ? "N/A" :
                Applst[j].hateSeverity === "0" ? "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Safe</i>" :
                    Applst[j].hateSeverity === "6" ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;High</i>" :
                        Applst[j].hateSeverity === "4" ? "<i class='fa fa-exclamation-triangle' aria-hidden='true' style='color:#FDD835  '>&nbsp;&nbsp;Medium</i>" :
                            Applst[j].hateSeverity === "2" ? "<i class='fa fa-exclamation-circle' aria-hidden='true' style='color:#F57F17'>&nbsp;&nbsp;Low</i>" :
                                "Unknown"
                }</td>`;
            dbTable += `<td class='comName'>${Applst[j].selfHarmSeverity == null ? "N/A" :
                Applst[j].selfHarmSeverity === "0" ? "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Safe</i>" :
                    Applst[j].selfHarmSeverity === "6" ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;High</i>" :
                        Applst[j].selfHarmSeverity === "4" ? "<i class='fa fa-exclamation-triangle' aria-hidden='true' style='color:#FDD835  '>&nbsp;&nbsp;Medium</i>" :
                            Applst[j].selfHarmSeverity === "2" ? "<i class='fa fa-exclamation-circle' aria-hidden='true' style='color:#F57F17'>&nbsp;&nbsp;Low</i>" :
                                "Unknown"
                }</td>`;
            dbTable += `<td class='comName'>${Applst[j].sexualSeverity == null ? "N/A" :
                Applst[j].sexualSeverity === "0" ? "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Safe</i>" :
                    Applst[j].sexualSeverity === "6" ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;High</i>" :
                        Applst[j].sexualSeverity === "4" ? "<i class='fa fa-exclamation-triangle' aria-hidden='true' style='color:#FDD835  '>&nbsp;&nbsp;Medium</i>" :
                            Applst[j].sexualSeverity === "2" ? "<i class='fa fa-exclamation-circle' aria-hidden='true' style='color:#F57F17'>&nbsp;&nbsp;Low</i>" :
                                "Unknown"
                }</td>`;
            dbTable += `<td class='comName'>${Applst[j].violenceSeverity == null ? "N/A" :
                Applst[j].violenceSeverity === "0" ? "<i class='fa fa-check' aria-hidden='true' style='color:green'>&nbsp;&nbsp;Safe</i>" :
                    Applst[j].violenceSeverity === "6" ? "<i class='fa fa-ban' aria-hidden='true' style='color:red'>&nbsp;&nbsp;High</i>" :
                        Applst[j].violenceSeverity === "4" ? "<i class='fa fa-exclamation-triangle' aria-hidden='true' style='color:#FDD835  '>&nbsp;&nbsp;Medium</i>" :
                            Applst[j].violenceSeverity === "2" ? "<i class='fa fa-exclamation-circle' aria-hidden='true' style='color:#F57F17'>&nbsp;&nbsp;Low</i>" :
                                "Unknown"
                }</td>`;
            dbTable += ("</tr>");
        }
        dbTable += ("</tbody>");
        dbTable += ("</table>");
        $('#aadusertable').html(dbTable);
        $('#listaad').dataTable({
            "pageLength": 10,
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
                search: "" // Removed the trailing comma here
            }
        });
    }
    else {
        $('#aadusertable').html("<span id='no-data'>No Data Available</span>");
    }
}
function handleRadioChange(event) {
    //alert(`Selected option: ${event.target.value}`);
    var slectedBtn = event.target.value;
    $.ajax({
       
        url:  actionurl._GetAiDashboardDropdown,
        data: { val: slectedBtn },
        contentType: "application/json; charset=utf-8",
        cache: false,
        type: "Get",
        beforeSend: function () {
            $("#loader").show();
        },
        success: function (data) {
            lst = data.performanceMatrixCheckers;
            BindToxicityPromptArea(lst);
        },
        complete: function () {
            $("#loader").hide();
        },
        error: function (data) {
            console.log(data);
        }
    });
}