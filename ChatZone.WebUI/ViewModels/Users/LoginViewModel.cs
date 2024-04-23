using System.ComponentModel.DataAnnotations;

namespace ChatZone.WebUI.ViewModels.Users
{
	public class LoginViewModel
	{

		[Required(ErrorMessage = "وارد کردن نام کاربری ضروری است.")]
		public string? UserName { get; set; }

		[Required(ErrorMessage = "وارد کردن پسورد اجباری است")]
		public string? Password { get; set; }

	}
}
