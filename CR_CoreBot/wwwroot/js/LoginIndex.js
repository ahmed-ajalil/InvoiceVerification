 

function closeCookiePopUp() {
    $('#gdprbx').css("display", "none");
}
$(document).ready(function () {
    $(".owl-carousel").owlCarousel({
        items: 1,
        loop: true,
        margin: 10,
        autoplay: true,
        autoplayTimeout: 2100,
        autoplayHoverPause: true
    });
});
function ForgotPassword() {
    //  alert("Please contact Admin if you lost or forgot your password");
    new swal({
        title: "Info",
        text: " Please contact Admin if you lost or forgot your password",
        icon: "info",
        button: "Ok",
    })
}


function show_loader() {
    document.getElementById('loader').style.visibility = "visible";
}
function hide_loader() {
    document.getElementById('loader').style.visibility = "hidden";
}
function Logins() {
    var username = $("#Email").val();
    var password = $("#Password").val();
    if (username == "") {
        $("#email_valid").removeClass("hidden");
        //hide_loader();
    }
    if (password == "") {
        $("#pass_valid").removeClass("hidden");
        //hide_loader();
    }
    if (username != "" && password != "") {
        var rememberme = document.getElementById('RememberMe').checked;
        if (username == "demo@unifycloud.com" && password == "Unify@123") {
           /* window.location.href = "Home/Index";*/
            window.location.href = actionurl._Index;
        } else {
            alert('User not found for this Email');
        }
    }
}
function displayLogin() {
    if (document.getElementById('TermsnConditions').checked == true) {
        document.getElementById('btnLogin').classList.remove("disabled");
    } else {
        document.getElementById('btnLogin').classList.add("disabled");
    }
}
function Login() {
    show_loader();
    var username = $("#Email").val();
    var password = $("#Password").val();
    $("#email_valid").addClass("hidden");
    $("#pass_valid").addClass("hidden");
    $("#email_wrong").addClass("hidden");
    $("#pass_wrong").addClass("hidden");
    if (username == "") {
        $("#email_valid").removeClass("hidden");
        hide_loader();
    }
    if (password == "") {
        $("#pass_valid").removeClass("hidden");
        hide_loader();
    }
    if (username != "" && password != "") {
        var rememberme = document.getElementById('RememberMe').checked;
        $.ajax({
        
            url:actionurl._getCustomAuthentication,
            type: 'POST',
            data: {
                username: username,
                password: password,
                rememberme: rememberme
            },
            success: function (response) {
                hide_loader();
                if (response.msg == "Success") {
                    var Category = response.additionalProperty;
                    if (Category == null) {
                     
                        window.location.href = actionurl._Index;
                    }
                    else {
                        var roleId = response.roleId;

                        if (roleId == 1) {
                            window.location.href = actionurl._admin_Dashboard;
                        }
                        else if (roleId == 3) {
                            window.location.href = actionurl._user_dashboard;
                        }
                    
                        //window.location.href = actionurl._aiassistantbot;

                       
                    }
                }
                else if (response.msg == "demo Success") {
                  
                    window.location.href = actionurl._Hol_Index;
                }
                else if (response.msg == "Failed") {
                    $(document).ready(function () {
                        $("#email_wrong").removeClass("hidden");
                        hide_loader();
                    });
                }
                else if (response.msg == "Not Exist") {
                    $(document).ready(function () {
                        $("#pass_wrong").removeClass("hidden");
                        document.getElementById('passwordcounter').innerHTML = (5 - response.count) + ' out of 5 attempts remaining';
                        hide_loader();
                    });
                }
                else if (response.msg == "TryWith") {
                    hide_loader();
                }
            },
            error: function (response) {
            }
        });
    }
}

 