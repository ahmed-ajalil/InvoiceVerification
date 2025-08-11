using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace CR_CoreBot.CustomFilters
{
	public class UserPrevent : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var normaluserlogin = context.HttpContext.Session.GetString("LoginInfo");
			if (normaluserlogin != null)
			{
				var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
				Controller controller = context.Controller as Controller;
				if (logindata.RoleId == 3 )
				{
					context.Result = controller.RedirectToAction("Index", "Home", new { area = "" });
				}
			}
			base.OnActionExecuting(context);
		}
	}
}
