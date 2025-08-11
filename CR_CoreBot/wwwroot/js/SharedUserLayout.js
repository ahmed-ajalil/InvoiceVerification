
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

function loaderPage() {
    if (Page_ClientValidate()) {

        //function that sometimes takes a long time
        show_loaderNew();
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
}
function show_loaderNew() {
    document.getElementById('loader1').style.visibility = "visible";
}
function hide_loaderNew() {
    document.getElementById('loader1').style.visibility = "hidden";
}