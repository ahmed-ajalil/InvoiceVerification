 

$(document).ready(function () {
    //      getUserAdmin();
});

function LogoutandFeedback(type) {
    debugger;
    if (type == "logoutaction") {
  
        window.location.href =actionurl._Feedback +'?type=' + encodeURIComponent('logaction');
    }
    else if (type == "logoutactionAd") {
    
        window.location.href = actionurl._Feedback + '?type=' + encodeURIComponent('logoutactionforad');
    }
}

function getUserAdmin() {
    $.ajax({
         
        url:actionurl._getUserAdmin,
        type: 'Get',
        data: {},
        async: false,
        success: function (response) {
            $('#adminuserid').text(response);
        },
        error: function (xhr, status, error) {
        }
    });
}
function PageLoader() {
    show_loader();
    var data;
    for (var i = 0; i < 100000000; i++) {
        data = i / 2 + 3 + 4 + 5;
        if (data > 1000) {
            data = 1223;
        }
        if (data < 0) {
            data = -1;
        }
    }
}



function show_loader() {
    document.getElementById('loader').style.visibility = "visible";
}
function hide_loader() {
    document.getElementById('loader').style.visibility = "hidden";
}

