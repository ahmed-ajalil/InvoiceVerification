using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CR_CoreBot.CustomFilters
{
	public class CategoryAuthentication: ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var normaluserlogin = context.HttpContext.Session.GetString("LoginInfo");
			
			if (normaluserlogin != null)
			{
				var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
				Controller controller = context.Controller as Controller;
				if(logindata.RoleId != 2)
				{
					string areaName = (string)context.RouteData.Values["area"];
					var controllerModelDictionary = new Dictionary<string, string>
					{
						{ "Banking", "BankingModel" },
						{ "HR", "HRModel" },
						{ "HealthCare", "HealthCareModel" },
						{ "Manufacturing", "ManufacturingModel" },
						{ "Retail", "RetailModel" }
					};
					if (logindata.Category != controllerModelDictionary[areaName])
					{
						context.Result = new RedirectToActionResult("Index", "Home", new { area = "" });
					}
				}
				
			}
			base.OnActionExecuting(context);
		}
	}
}
