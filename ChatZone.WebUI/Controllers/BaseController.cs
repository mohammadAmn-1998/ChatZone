using ChatZone.ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatZone.WebUI.Controllers
{
	public class BaseController : Controller
	{
		#region SuccessAlert

		public void SuccessAlert()
		{
			var model = JsonConvert.SerializeObject(OperationResult.Success());
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}
		public void SuccessAlert(string message)
		{
			var model = JsonConvert.SerializeObject(OperationResult.Success(message));
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}

		#endregion

		#region ErrorAlert

		public void ErrorAlert()
		{
			var model = JsonConvert.SerializeObject(OperationResult.Error());
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}
		public void ErrorAlert(string message)
		{
			var model = JsonConvert.SerializeObject(OperationResult.Error(message));
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}

		#endregion

		#region NotFoundAlert

		public void NotFoundAlert()
		{
			var model = JsonConvert.SerializeObject(OperationResult.NotFound());
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}
		public void NotFoundAlert(string message)
		{
			var model = JsonConvert.SerializeObject(OperationResult.NotFound(message));
			HttpContext.Response.Cookies.Append("SystemAlert", model);

		}

		#endregion

	}
}
