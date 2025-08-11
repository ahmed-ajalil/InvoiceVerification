var CustomerId;
$(document).ready(function () {
    debugger;
    var queryString = window.location.search;

    if (queryString.includes(type = "logaction")) {
        var searchParams = new URLSearchParams(queryString);
        CustomerId = searchParams.get('Act');
        //        $("#btnReviewChatbot").hide();
        //        $("#btncancelReviewChatbot").hide();


        $("#btnReviewLogout").show();
        $("#btncancelReviewLogout").show();

    }
    else if (queryString.includes(type = "logoutactionforad")) {
        //$("#btnReviewLogout").hide();
        //$("#btncancelReviewLogout").hide();
        $("#btnReviewLogoutAd").show();
        $("#btncancelReviewLogoutAd").show();
    }
    else if (queryString.includes(type = "AdFeedbackFromChatbot")) {
        $("#btnReviewChatbotAd").show();
        $("#btncancelReviewChatbotAd").show();
    }
    else {
        //      $("#btnReviewLogout").hide();
        //      $("#btncancelReviewLogout").hide();
        $("#btnReviewChatbot").show();
        $("#btncancelReviewChatbot").show();
    }

});

function SubmitReview(typeofaction) {

    debugger;
    var type = typeofaction.trim();
    // chatforad added by atul sh.
    if (type == "chatbot" || type == "logout" || type == "chatbotforad" || type == "chatbotAd") {
        var overallexperience = $("input[name='rating']:checked").val();
        if (overallexperience == undefined) {
            overallexperience = 0;
        }
        var satisfactionwithcurrentbot = $("input[id='satisfactionwithbot']:checked").val();
        var suggestionFeedback = $("#suggestionFeedback").val();
        // var emailmefeedback = $("input[id='emailmefeedback']:checked").val();
        var emailmefeedback = $('#emailmefeedback').is(':checked');
        var feedbackform = {};
        feedbackform.OveralExperience = overallexperience;
        feedbackform.Satisfactionwithbot = satisfactionwithcurrentbot;
        feedbackform.SuggestionFeedback = suggestionFeedback;
        feedbackform.EmailFeedback = emailmefeedback;


        $.ajax({
            /*url: '/Login/SaveFeedback',*/
            url:actionurl._SaveFeedback,
            type: 'POST',
            data: feedbackform,
            success: function (response) {
                debugger
                var data = response;
                if (data == true) {
                    new swal({
                        title: " Success",
                        text: "Feedback saved successfully",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                        /*window.close();*/
                        window.location.href = actionurl._Logout;
                    });

                    //       alert("Feedback Saved Successfully");
                    //  alert("Feedack Saved Successfully");
                    //   window.close();
                    //window.location.href = "/Login/Logout";

                }
                else {
                    alert("Something went wrong");
                    //new swal({
                    //    title: "Error",
                    //    text: "Issues while saving Model fine tune data",
                    //    icon: "Error"
                    //});
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



function CancelReview(type) {
    debugger
    if (type == "chatbot") {
        /*window.close();*/
        window.location.href = actionurl._Logout;
      
    }
    else if (type == "chatbotAd") {
        /*window.close();*/
        window.location.href = actionurl._Logout;
    }
    else if (type == "logout") {
       
        window.location.href = actionurl._Logout;
      
    }
    else if (type == "logoutforad") {
      
       window.location.href = actionurl._Logout;
    }
}

function RedirectToLoginPage() {
    var link = document.createElement('a');
    link.innerText = 'Logout';
  
    link.href = actionurl._Logout;
    //   document.body.appendChild(link);
    link.click();
}

function SubmitReviewOnLogout(type) {
    debugger;
    if (type == "logout") {
        var overallexperience = $("input[name='rating']:checked").val();
        if (overallexperience == undefined) {
            overallexperience = 0;
        }
        var satisfactionwithcurrentbot = $("input[id='satisfactionwithbot']:checked").val();
        var suggestionFeedback = $("#suggestionFeedback").val();
        // var emailmefeedback = $("input[id='emailmefeedback']:checked").val();
        var emailmefeedback = $('#emailmefeedback').is(':checked');
        var feedbackform = {};
        feedbackform.OveralExperience = overallexperience;
        feedbackform.Satisfactionwithbot = satisfactionwithcurrentbot;
        feedbackform.SuggestionFeedback = suggestionFeedback;
        feedbackform.emailFeedback = emailmefeedback;
        feedbackform.CustomerId = CustomerId;
        debugger
        $.ajax({
         
           url:actionurl._SaveFeedback,
            type: 'POST',
            data: feedbackform,
            success: function (response) {
                debugger
                var data = response;
                if (data == true) {
                    new swal({
                        title: "Success",
                        text: "Feedback saved successfully",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                        window.location.href = actionurl._Logout;
                    });

                    //  RedirectToLoginPage();

                }
                else {
                    alert("Something went wrong");
                }
            },

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
    else if (type == "logoutforad") {
        var overallexperience = $("input[name='rating']:checked").val();

        if (overallexperience == undefined) {
            overallexperience = 0;
        }

        var satisfactionwithcurrentbot = $("input[id='satisfactionwithbot']:checked").val();
        var suggestionFeedback = $("#suggestionFeedback").val();
        // var emailmefeedback = $("input[id='emailmefeedback']:checked").val();
        var emailmefeedback = $('#emailmefeedback').is(':checked');
        var feedbackform = {};
        feedbackform.OveralExperience = overallexperience;
        feedbackform.Satisfactionwithbot = satisfactionwithcurrentbot;
        feedbackform.SuggestionFeedback = suggestionFeedback;
        feedbackform.emailFeedback = emailmefeedback;
        feedbackform.CustomerId = CustomerId;
        $.ajax({
            /*url: '/Login/SaveFeedback',*/
            url:  actionurl._SaveFeedback,
            type: 'POST',
            data: feedbackform,
            success: function (response) {
                debugger
                var data = response;
                if (data == true) {
                    new swal({
                        title: "Success",
                        text: "Feedback saved successfully",
                        icon: "Success",
                        button: "Ok",
                    }).then(function () {
                        /* window.location.href = '@Url.Action("AdLogout", "Login")';*/
                        window.location.href =  actionurl._AdLogout;
                    });

                    //  RedirectToLoginPage();

                }
                else {
                    alert("Something went wrong");
                }
            },

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
