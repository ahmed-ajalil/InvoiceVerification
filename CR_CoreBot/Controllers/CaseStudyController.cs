using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using static CR_CoreBot.Controllers.HRadminController;

namespace CR_CoreBot.Controllers
{
    public class CaseStudyController : Controller
    {
        public IActionResult UnderConst()
        {
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    if (loginInfo != null)
                    {
                        ViewBag.Role = loginInfo.RoleId;
                        ViewBag.EmailId = loginInfo.EmailId;
                        ViewBag.username = loginInfo.EmailId;
                        ViewBag.LoginType = "Login";
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO adLoginInfoDTO = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    if (adLoginInfoDTO != null)
                    {
                        ViewBag.EmailId = adLoginInfoDTO.EmailId.Trim();
                        ViewBag.username = adLoginInfoDTO.EmailId.Trim();
                        ViewBag.LoginType = "AdLogin";
                        ViewBag.Role = adLoginInfoDTO.RoleId;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }
        public ActionResult DashBoard()
        {
            return Redirect("https://portal.azure.com/#@unifycloud.com/dashboard/arm/subscriptions/062179ae-4f94-40cb-b67e-bbc95517c8a5/resourcegroups/dashboards/providers/microsoft.portal/dashboards/c0cac825-9461-41b6-bf9e-1bbc2325b734");
        }
    }
}
