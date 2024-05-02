using ChatZone.ApplicationCore.Helpers;
using ChatZone.WebUI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ChatZone.WebUI.Controllers
{
	public class BaseController : Controller
	{
		private IHubContext<ChatHub> _hubContext;

		#region SuccessAlert

		public BaseController(IHubContext<ChatHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public async void SuccessAlert(bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.Success());
			// HttpContext.Response.Cookies.Append("SystemAlert", model);

			 await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.Success(isReload));

		}
		public async void SuccessAlert(string message, bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.Success(message));
			// HttpContext.Response.Cookies.Append("SystemAlert", model);
			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.Success(message,isReload));
		}

		#endregion

		#region ErrorAlert

		public  async void ErrorAlert(bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.Error());
			// HttpContext.Response.Cookies.Append("SystemAlert", model);
			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.Error(isReload));

		}
		public async  void ErrorAlert(string message, bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.Error(message));
			// HttpContext.Response.Cookies.Append("SystemAlert", model);
			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.Error(message,isReload));

		}

		#endregion

		#region NotFoundAlert

		public async void NotFoundAlert(bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.NotFound());
			// HttpContext.Response.Cookies.Append("SystemAlert", model);
			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.NotFound(isReload));

		}
		public async  void NotFoundAlert(string message, bool isReload = false)
		{
			// var model = JsonConvert.SerializeObject(OperationResult.NotFound(message));
			// HttpContext.Response.Cookies.Append("SystemAlert", model);

			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveOperationResult", OperationResult.NotFound(message,isReload));

		}

		#endregion

	}
}
