@if (@ViewBag.msg != "" && @ViewBag.msg == "succesfully") {
    
        $(document).ready(function () {
            swal("Successfully done !", " ", "success");
        });
    

}