using System.ComponentModel.DataAnnotations;

namespace ChatZone.WebUI.ViewModels.Users
{
	public class RegisterViewModel
	{

		[Required(ErrorMessage = "وارد کردن نام کاربری ضروری است.")]
		[MaxLength(200,ErrorMessage = "نام کاربری نباید بیشتر از 200 کاراکتر داشته باشد")]
		[MinLength(2,ErrorMessage = "نام کاربری باید بیشتر از 2 کاراکتر داشته باشد")]
		public string? UserName { get; set; }

		[Required(ErrorMessage = "وارد کردن پسورد اجباری است")]
		[MinLength(6,ErrorMessage = "پسورد باید بیشتر از 5 کاراکتر باشد")]
		public string? Password { get; set; }

		[Compare(nameof(Password),ErrorMessage = "تکرار رمز عبور اشتباه است.")]
		public string? RePassword { get; set; }
	}
}
