using System.Security.Claims;
using ChatZone.ApplicationCore.Dtos.Users;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.WebUI.ViewModels.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.WebUI.Controllers
{
	public class AuthController : Controller
	{
		IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		public IActionResult Index()
		{
			return View();
		}


		public async Task<IActionResult> Register(RegisterViewModel model)
		{

			if (!ModelState.IsValid)
				return View("Index", model);

			if (await _userService.IsUserNameExists(model.UserName!))
			{
				ModelState.AddModelError("UserName","این نام کاربری قبلا ثبت شده است.");
				return View("Index",model);
			}

			var dto = new UserDto
			{
				UserName = model.UserName,
				Password = model.Password!.EncodeToMd5(),
			};

			await _userService.AddNewUser(dto);

			return Redirect($"/auth#login");

		}

		public async Task<IActionResult> Login(LoginViewModel model)
		{

			if (!ModelState.IsValid)
				return View("Index", model);

			

			var result = await _userService.GetUserByUserNameAndPassword(model.UserName!,model.Password!.EncodeToMd5());

			if (result == null)
			{
				ModelState.AddModelError("","نام کاربری یا پسورد اشتباه است");
				return View("Index",model);
			}

			var isSignedIn =  await SignInUser(result);
			if ( isSignedIn)
				return Redirect("/");

			ModelState.AddModelError("","مشکلی پیش امده دوباره امتحان کنید");
			return View("Index");



		}


		#region PrivateMethods

		private async Task<bool> SignInUser(UserDto dto)
		{

			try
			{
				var claims = new List<Claim>()
				{
					new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString()),
					new Claim(ClaimTypes.Name, dto.UserName!)
				};

				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var principal = new ClaimsPrincipal(identity);

				var properties = new AuthenticationProperties
				{
					IsPersistent = true,
					ExpiresUtc = DateTime.UtcNow.AddDays(3),
				};

				await HttpContext.SignInAsync( principal, properties);
				return true;
			}
			catch 
			{
				return false;
			}

		}

		#endregion

	}
}
