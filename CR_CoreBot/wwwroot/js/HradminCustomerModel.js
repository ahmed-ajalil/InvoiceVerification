@if (@ViewBag.customerModelmsg != "" && @ViewBag.customerModelmsg == "succesfully") {
     
        $(document).ready(function () {
            swal("Successfully done !", " ", "success");
        });
     
}