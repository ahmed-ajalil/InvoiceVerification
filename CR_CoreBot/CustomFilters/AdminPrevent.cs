using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CR_CoreBot.CustomFilters
{
	public class AdminPrevent: ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var normaluserlogin = context.HttpContext.Session.GetString("LoginInfo");
			if (normaluserlogin != null)
			{
				var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
				Controller controller = context.Controller as Controller;
				if (logindata.RoleId == 1)
				{
					context.Result = controller.RedirectToAction("Index", "Home", new { area = "" });
				}
			}
			base.OnActionExecuting(context);
		}
	}
}
