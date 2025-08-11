using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace CR_CoreBot.CustomFilters
{
	public class HolUser: ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var normaluserlogin = context.HttpContext.Session.GetString("LoginInfo");
			if (normaluserlogin != null)
			{
				var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
				Controller controller = context.Controller as Controller;
				if (logindata.LoginWith == "demo")
				{
					context.HttpContext.Session.Clear();
					context.HttpContext.Session.Remove("LogoutInfo");
					context.HttpContext.Session.Remove("UserLoginTime");
					context.HttpContext.Session.Remove("LoginUserInfo");
					context.HttpContext.Session.Remove("LoginInfo");
					context.Result = controller.RedirectToAction("Index", "Login");
				}
			}
			base.OnActionExecuting(context);
		}
	}
}
