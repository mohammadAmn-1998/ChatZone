using System.Security.Claims;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.UserGroups;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.WebUI.Hubs;
using ChatZone.WebUI.ViewModels.ChatGroups;
using ChatZone.WebUI.ViewModels.Chats;
using ChatZone.WebUI.ViewModels.UserGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace ChatZone.WebUI.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{

		#region injected services ctor

		IHubContext<ChatHub> _hubContext;
		IUserGroupService _UserGroupService;
		IChatGroupService _ChatGroupService;
		private IFileManagerService _fileManagerService;
		private IChatService _chatService;
		IUserService _userService;

		public HomeController(IUserGroupService userGroupService, IChatGroupService chatGroupService,
			IFileManagerService fileManagerService, IChatService chatService, IUserService userService,
			IHubContext<ChatHub> hubContext)
		{
			_UserGroupService = userGroupService;
			_ChatGroupService = chatGroupService;
			_fileManagerService = fileManagerService;
			_chatService = chatService;
			_userService = userService;
			_hubContext = hubContext;
		}


		#endregion



		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddNewGroup([FromForm] CreateChatGroupViewModel model)
		{

			if (!ModelState.IsValid)
				return new ObjectResult(new { message = "Error" });


			var imageName =
				await _fileManagerService.UploadFileAndReturnFileName(model.ImageFile!,
					Directories.GetUserGroupImageDirectory());
			var chatGroupDto = new ChatGroupDto
			{
				Title = model.Title,
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

			var fixedModel = FixPrivateUserGroupList(userGroupsList);



			return PartialView("_UserGroupsPartial", fixedModel);
		}

		public async Task<IActionResult> SearchUserGroups(string title)
		{

			var searchResults = await _UserGroupService.SearchBy(title);


			var model = searchResults?.Select(sr => new UserGroupViewModel
			{
				IsUser = sr.IsUser,
				Title = sr.Title,
				ImageName = sr.GroupImage,
				LastChat = sr.LastChat ?? "",
				Token = sr.Token,
				LastChatDate = sr.LastChatDate?.ConvertToPersianDate() ?? ""
			}).Where(u => u.Title != HttpContext.User.GetUserName()).ToList();

			return new ObjectResult(model);

		}

		public async Task<IActionResult> LoadGroupChats(string? token)
		{

			if (token == null)
				return Empty;

			var chatsDto = await _chatService.GetChats(token);

			if (chatsDto == null)
				return Empty;

			var isJoined = await _UserGroupService.IsJoinedGroup(chatsDto.GroupId, HttpContext.User.GetUserId());

			var model = new ChatGroupViewModel
			{

				GroupId = chatsDto.GroupId,
				GroupTitle = chatsDto?.GroupTitle,
				ImageName = chatsDto?.ImageName,
				CreateDate = chatsDto?.CreateDate?.ConvertToPersianDate(),
				IsJoined = isJoined,
				Chats = chatsDto?.Chats?.Select(c => new ChatViewModel
				{

					ChatBody = c.ChatBody,
					GroupId = c.GroupId,
					UserName = c.UserName,
					CreateDate = c.CreateDate.ConvertToPersianDate(),
					UserId = c.UserId,
					FileName = c.FileName,
					IsCallerChat = c.UserId == HttpContext.User.GetUserId()
				}).ToList()
			};

			ViewBag.isJoined = isJoined;

			return PartialView("_ChatsPartial", model);


		}

		public async Task<IActionResult> JoinGroup(long groupId)
		{


			if (groupId <= 0)
				return NotFound();

			await _UserGroupService.AddNewUserGroup(new UserGroupDto
			{
				UserId = HttpContext.User.GetUserId(),
				GroupId = groupId,
			});


			var chatsDto = await _chatService.GetChats(groupId);

			if (chatsDto == null)
				return Empty;

			var isJoined = await _UserGroupService.IsJoinedGroup(chatsDto.GroupId, HttpContext.User.GetUserId());

			var model = new ChatGroupViewModel
			{
				GroupId = chatsDto.GroupId,
				GroupTitle = chatsDto?.GroupTitle,
				ImageName = chatsDto?.ImageName,
				CreateDate = chatsDto?.CreateDate?.ConvertToPersianDate(),
				IsJoined = isJoined,
				Chats = chatsDto?.Chats?.Select(c => new ChatViewModel
				{

					ChatBody = c.ChatBody,
					GroupId = c.GroupId,
					UserName = c.UserName,
					CreateDate = c.CreateDate.ConvertToPersianDate(),
					UserId = c.UserId,
					FileName = c.FileName,
					IsCallerChat = c.UserId == HttpContext.User.GetUserId()
				}).ToList()
			};

			ViewBag.IsJoined = isJoined;

			return PartialView("_ChatsPartial", model);


		}

		public async Task<IActionResult> GetUserChats(long userId)
		{

			var user = await _userService.GetUserById(userId);
			if (user == null) return NotFound();

			var result = await _ChatGroupService.GetUserPrivateChatGroup(userId, HttpContext.User.GetUserId());


			//if result is null it means this user had never chat with you and need to create a new private chatGroup for current user and this user
			ChatGroupViewModel model;
			if (result is not null)
			{
				 model = FixPrivateChatGroup(result, user.Id);
			}
			else
			{
				 model = new ChatGroupViewModel
				{
					GroupId = 0,
					IsUser = true,
					UserId = userId,
					GroupTitle = user.UserName,
					ImageName = user.Avatar,
					CreateDate = DateTime.Now.ConvertToPersianDate(),
					IsJoined = false,

				};
			}

			return PartialView("_ChatsPartial", model);
		}

		public async Task<IActionResult> AddNewUserPrivateChatGroup(long userId)
		{

			if (userId == 0) return BadRequest();

			var user = await _userService.GetUserById(userId);
			if (user is null) return NotFound();

			var chatGroupDto = new ChatGroupDto
			{
				Title = user.UserName,
				GroupImage = user.Avatar,
				OwnerId = HttpContext.User.GetUserId(),
				IsPrivate = true,
				ReceiverId = user.Id,
			};

			//add new PrivateChatGroup:
			var result = await _ChatGroupService.InsertNewChatGroup(chatGroupDto);

			//join each of users to this private chat group:
			await _UserGroupService.AddNewUserGroup(new UserGroupDto
			{
				UserId = result.OwnerId,
				GroupId = result.Id,
			});
			await _UserGroupService.AddNewUserGroup(new UserGroupDto
			{
				UserId = result.ReceiverId ?? 0,
				GroupId = result.Id,
			});

			var model = FixPrivateChatGroup(result, userId);

			return PartialView("_ChatsPartial", model);

		}


		#region private helper Methods

		private ChatGroupViewModel FixPrivateChatGroup(ChatGroupDto dto,long userId)
		{


			try
			{
				if (dto.OwnerId == HttpContext.User.GetUserId() && dto.ReceiverId == userId)
				{

					return new ChatGroupViewModel
					{

						GroupId = dto.Id,
						GroupTitle = dto?.Title,
						ImageName = dto?.GroupImage,
						CreateDate = dto?.CreatedDate.ConvertToPersianDate(),
						IsJoined =  _UserGroupService.IsJoinedGroup(dto!.Id,HttpContext.User.GetUserId()).Result,
						Chats = dto?.Chats?.Select(c => new ChatViewModel
						{

							ChatBody = c.ChatBody,
							GroupId = c.GroupId,
							UserName = c.UserName,
							CreateDate = c.CreateDate.ConvertToPersianDate(),
							UserId = c.UserId,
							FileName = c.FileName,
							IsCallerChat = c.UserId == HttpContext.User.GetUserId()

						}).ToList()
					};

				}
				else if (dto.ReceiverId == HttpContext.User.GetUserId() && dto.OwnerId == userId)

				{

					return new ChatGroupViewModel
					{

						GroupId = dto.Id,
						GroupTitle = dto?.OwnerUser!.UserName,
						ImageName = dto?.OwnerUser!.Avatar,
						CreateDate = dto?.CreatedDate.ConvertToPersianDate(),
						IsJoined = _UserGroupService.IsJoinedGroup(dto!.Id, HttpContext.User.GetUserId()).Result,
						Chats = dto?.Chats?.Select(c => new ChatViewModel
						{

							ChatBody = c.ChatBody,
							GroupId = c.GroupId,
							UserName = c.UserName,
							CreateDate = c.CreateDate.ConvertToPersianDate(),
							UserId = c.UserId,
							FileName = c.FileName,
							IsCallerChat = c.UserId == HttpContext.User.GetUserId()

						}).ToList()
					};
				}
				else
				{
					return null;
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		private List<UserGroupViewModel>? FixPrivateUserGroupList(List<UserGroupDto>? groupLists)
		{
			List<UserGroupViewModel> model = new List<UserGroupViewModel>();
			if (groupLists == null)
				return null;
			foreach (var groupDto in groupLists)
			{

				

				var receiverId = groupDto?.ChatGroup!.ReceiverId;
				if (receiverId is not null or 0)
				{
					if (receiverId == HttpContext.User.GetUserId())
					{

						//it means owner is the other user so group image and title is receiver avatar and userName so we need to change group image and title to owner because we are in receiver account.

						model.Add(
							new UserGroupViewModel
							{
								Title = groupDto!.ChatGroup!.OwnerUser!.UserName,
								ImageName = groupDto.ChatGroup?.OwnerUser!.Avatar,
								Token = groupDto.ChatGroup?.Token,
								LastChat = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
									.LastOrDefault()?.ChatBody,
								LastChatDate = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
									.LastOrDefault()?.CreateDate
									.ConvertToPersianDate()
							});


					}
					else
					{
						model.Add(
							new UserGroupViewModel
							{
								Title = groupDto!.ChatGroup!.Title,
								ImageName = groupDto.ChatGroup?.GroupImage,
								Token = groupDto.ChatGroup?.Token,
								LastChat = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
									.LastOrDefault()?.ChatBody,
								LastChatDate = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
									.LastOrDefault()?.CreateDate
									.ConvertToPersianDate()
							});
					}

				}
				else
				{
					model.Add(
						new UserGroupViewModel
						{
							Title = groupDto!.ChatGroup!.Title,
							ImageName = groupDto.ChatGroup?.GroupImage,
							Token = groupDto.ChatGroup?.Token,
							LastChat = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
								.LastOrDefault()?.ChatBody,
							LastChatDate = groupDto.ChatGroup?.Chats?.OrderBy(c => c.CreateDate)
								.LastOrDefault()?.CreateDate
								.ConvertToPersianDate()
						});
				}

			}

			return model ;

		}

		#endregion



	}
}
