@if (@ViewBag.customerINFOmsg != "" && @ViewBag.customerINFOmsg == "succesfully")
{

        $(document).ready(function () {
            swal("Successfully done !", " ", "success");
        });
     
}