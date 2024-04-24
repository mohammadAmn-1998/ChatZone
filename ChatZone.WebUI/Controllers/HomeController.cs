using System.Security.Claims;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.UserGroups;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.WebUI.ViewModels.ChatGroups;
using ChatZone.WebUI.ViewModels.UserGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatZone.WebUI.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{

		IUserGroupService _UserGroupService;
		IChatGroupService _ChatGroupService;
		private IFileManagerService _fileManagerService;

		public HomeController(IUserGroupService userGroupService, IChatGroupService chatGroupService, IFileManagerService fileManagerService)
		{
			_UserGroupService = userGroupService;
			_ChatGroupService = chatGroupService;
			_fileManagerService = fileManagerService;
		}
		public IActionResult Index()
		{
			return View();
		}



		public async Task<IActionResult> AddNewGroup([FromForm]CreateChatGroupViewModel model)
		{

			if (!ModelState.IsValid)
				return new ObjectResult(new{message="Error"});


			var imageName =
				await _fileManagerService.UploadFileAndReturnFileName(model.ImageFile!,
					Directories.GetUserGroupImageDirectory());
			var chatGroupDto = new ChatGroupDto
			{
				Title = model.Title,
				Description = null,
				GroupImage = imageName,
				OwnerId = HttpContext.User.GetUserId(),
			};

			var result = await _ChatGroupService.InsertNewChatGroup(chatGroupDto);

			if (result == null)
				return new ObjectResult(new { message = "Error" });

			var userGroupDto = new UserGroupDto
			{
				UserId = result.OwnerId,
				GroupId = result.Id,
			};

			await _UserGroupService.AddNewUserGroup(userGroupDto);
			return new ObjectResult(new { message = "Success" });

		}


		public async Task<IActionResult> LoadUserGroupsList()
		{

			var userGroupsList = await _UserGroupService.GetUserGroupList(HttpContext.User.GetUserId());


			var model = userGroupsList?.Select(ug => new UserGroupViewModel
			{
				Title = ug.ChatGroup?.Title,
				ImageName = ug.ChatGroup?.GroupImage,
				Token = ug.ChatGroup?.Token,
				LastChat = ug.ChatGroup?.Chats.OrderBy(c => c.CreateDate).LastOrDefault()?.ChatBody,
				LastChatDate = ug.ChatGroup?.Chats.OrderBy(c => c.CreateDate).LastOrDefault()?.CreateDate
					.ConvertToPersianDate()
			}).ToList();

			return PartialView("_UserGroupsPartial", model);
		}


		public async Task<IActionResult> SearchUserGroups(string title)
		{

			var searchResults = await _UserGroupService.SearchBy( title);


			var model = searchResults?.Select(sr => new UserGroupViewModel
			{
				Title = sr.Title,
				ImageName = sr.GroupImage,
				LastChat = sr.LastChat ?? "",
				Token = sr.Token,
				LastChatDate = sr.LastChatDate?.ConvertToPersianDate() ?? ""
			}).ToList();

			return new ObjectResult(model);
		}
	}
}
