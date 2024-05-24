using System.Security.Claims;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.UserGroups;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.WebUI.Hubs;
using ChatZone.WebUI.ViewModels.ChatGroups;
using ChatZone.WebUI.ViewModels.Chats;
using ChatZone.WebUI.ViewModels.CustomModels;
using ChatZone.WebUI.ViewModels.UserGroups;
using ChatZone.WebUI.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.JSInterop.Infrastructure;
using Newtonsoft.Json.Linq;

namespace ChatZone.WebUI.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{

		#region injected services ctor

		private IHubContext<ChatHub> _hubContext;
		IUserGroupService _UserGroupService;
		IChatGroupService _ChatGroupService;
		private IFileManagerService _fileManagerService;
		private IChatService _chatService;
		IUserService _userService;

		public HomeController(IUserGroupService userGroupService, IChatGroupService chatGroupService,
			IFileManagerService fileManagerService, IChatService chatService, IUserService userService,
			IHubContext<ChatHub> hubContext) : base(hubContext)
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

			var imageName = "Default.jpg";

			if(model.ImageFile is not null)
					 imageName =
					await _fileManagerService.UploadFileAndReturnFileName(model.ImageFile!,
					Directories.GetUserGroupImageDirectory());


			var chatGroupDto = new ChatGroupDto
			{
				Title = model.Title,
				GroupImage = imageName,
				IsPrivate = model.IsPrivate,
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

			await _UserGroupService.JoinToGroup(userGroupDto);
			SuccessAlert("گروه ساخته شد!",true);
			return new ObjectResult(new { message = "Success" });

		}

		public async Task<IActionResult> LoadUserGroupsList()
		{

			var userGroupsList = await _UserGroupService.GetUserGroupList(HttpContext.User.GetUserId());

			var model = userGroupsList?.Select(ug => new UserGroupViewModel
			{
				IsUser = ug.IsUserChat,
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
				IsPrivate = chatsDto!.IsPrivate,
				OwnerId = chatsDto?.OwnerId ??0,
				ReceiverId = chatsDto?.ReceiverId ??0 ,
				CreateDate = chatsDto?.CreateDate?.ConvertToPersianDate(),
				IsJoined = isJoined,
				IsUser = false,
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
				,OwnerUser = chatsDto?.OwnerUser != null ? new UserProfileViewModel()
				{
					Id = chatsDto.OwnerUser.Id,
					UserName = chatsDto.OwnerUser.UserName,
					Avatar = chatsDto.OwnerUser.Avatar,
					Bio = chatsDto.OwnerUser.Bio
				} : null
			};

			ViewBag.isJoined = isJoined;


			var fixedModel = FixUserPrivateChatGroup(model);

			return PartialView("_ChatsPartial", fixedModel);


		}

		public async Task<IActionResult> JoinGroup(long groupId)
		{


			if (groupId <= 0)
				return NotFound();

			await _UserGroupService.JoinToGroup(new UserGroupDto
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
			SuccessAlert("شما وارد این گروه شدید!");
			await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString())
				.SendAsync("receiveCurrentGroupId", model.GroupId);
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
			await _UserGroupService.JoinToGroup(new UserGroupDto
			{
				UserId = result.OwnerId,
				GroupId = result.Id,
			});
			await _UserGroupService.JoinToGroup(new UserGroupDto
			{
				UserId = result.ReceiverId ?? 0,
				GroupId = result.Id,
			});

			var model = FixPrivateChatGroup(result, userId);

			SuccessAlert("شروع چت!");

			return PartialView("_ChatsPartial", model);

		}

		public async Task<IActionResult> SearchUsers(string userName,long currentGroupId)
		{

			var usersList = await _userService.SearchBy(userName,currentGroupId);

			var fixedModel = usersList?.Where(u => u.UserName != HttpContext.User.GetUserName()).Select(u =>
				new SearchUserViewModel
				{
					Id = u.Id,
					UserName = u.UserName,
					IsMember = u.IsMember

				}).ToList();
			return new ObjectResult(fixedModel);
			
		}

		public async Task<IActionResult> AddMemberToGroup(long groupId, long userId)
		{

		   await _UserGroupService.JoinToGroup(new UserGroupDto
			{
				UserId = userId,
				GroupId = groupId,
				
			});
			SuccessAlert("کاربر به گروه اضافه شد!");
			var user = await _userService.GetUserById(userId);

			var userGroupDto = await _UserGroupService.GetUserGroupBy(groupId);

			if (userGroupDto == null)
			{
				ErrorAlert("مشکلی به وجود آمده دوباره تلاش کنید");
				return View("Index");
			}


			var model = new UserGroupViewModel
			{
				Title = userGroupDto.ChatGroup!.Title,
				ImageName = userGroupDto.ChatGroup.GroupImage,
				Token = userGroupDto.ChatGroup.Token,
				IsUser = false,
				LastChat = userGroupDto.ChatGroup.Chats?.OrderBy(ch=> ch.CreateDate).LastOrDefault()?.ChatBody,
				LastChatDate = DateTime.Now.ConvertToPersianDate()
			};

			await _hubContext.Clients.User(userId.ToString()).SendAsync("receiveNewChatGroup", model);

			var notificationModel = new NotificationViewModel
			{
				Title = $"شما به گروه {model.Title} !اضافه شدید",
				Body = DateTime.Now.ConvertToPersianDate()
			};


			await _hubContext.Clients.User(userId.ToString()).SendAsync(
				"receiveChatGroupNotification", notificationModel);

			return new ObjectResult(new {Status="success",UserId = userId,GroupId = groupId,UserName= user!.UserName});
		}

		public async Task<IActionResult> GetUserProfile(long userId)
		{


			var user = await _userService.GetUserById(userId);
			if (user == null)
			{
				NotFoundAlert("کاربر پیدا نشد");
				return Empty;
			}

			var model = new UserProfileViewModel
			{
				Id = user.Id,
				UserName = user.UserName,
				Avatar = user.Avatar,
				Bio = user.Bio
			};
			return new ObjectResult(model);
		}

		public async Task<IActionResult> GetChatGroupProfile(long groupId)
		{


			var users =await _UserGroupService.GetGroupUsers(groupId);
			var chatGroup = await _ChatGroupService.GetChatGroupBy(groupId);

			var model = new ChatGroupProfileViewModel
			{
				Users = users?.Select(u => new SearchUserViewModel
					{
						Id = u.UserId,
						UserName = u.UserName,

					}).ToList(),
					
				Image = chatGroup?.GroupImage,
				Title = chatGroup?.Title,

			};
			return new ObjectResult(model);

		}

		public async Task<IActionResult> DeleteChatGroup(long groupId)
		{

			var result = await _ChatGroupService.DeleteChatGroup(groupId);

			
			 ShowAlert(result,true);

			 Thread.Sleep(5000);

			 return Redirect("/");


		}

		public async Task<IActionResult> LeaveChatGroup(long groupId)
		{


			var result = await _UserGroupService.LeaveGroup(HttpContext.User.GetUserId(), groupId);
			ShowAlert(result, true);

			Thread.Sleep(5000);

			return Redirect("/");


		}

		[HttpPost]
		public async Task<IActionResult> SendFileToGroup([FromForm] SendFileViewModel? fileViewModel)
		{

			if (fileViewModel == null)
			{
				ErrorAlert("مشکلی به وجود آمده است");
				return new ObjectResult("Error");
			}

			if (fileViewModel.File == null )
			{
				ErrorAlert("فایلی انتخاب نشده است");
				return new ObjectResult("Error");
			}



			var fileExtension = Path.GetExtension(fileViewModel.File!.FileName);

			if (fileExtension is ".jpg" or ".png" or ".bmp" or "jpeg" or ".mkv" or ".mp4")
			{

				if (fileExtension is ".bmp" or "mkv" or "mp4")
				{

					if (fileViewModel.File.Length > 1250000) //equal to 100 megabytes
					{

						ErrorAlert("اندازه فایل باید کمتر از10 مگابایت باشد");
						return new ObjectResult("Error");

					}

				}

				var fileName = await 
					_fileManagerService.UploadFileAndReturnFileName(fileViewModel.File, Directories.GetChatFilesDirectory());

				if (fileName is null)
				{
					ErrorAlert("مشکلی در سرور به وجود آمده دوبار تلاش کنید.",false);
					return new ObjectResult("Error");
				}


				var chatDto = new ChatDto
				{
					
					CreateDate = DateTime.Now,
					ChatBody = fileViewModel.Caption,
					GroupId = fileViewModel.GroupId,
					UserId = HttpContext.User.GetUserId(),
					FileName = fileName,
					
				};

				var result = await _chatService.InsertChat(chatDto);

				if (result == null)
				{
					ErrorAlert("مشکلی در سرور به وجود آمده دوبار تلاش کنید.", false);
					return new ObjectResult("Error");
				}

				var model = new ChatViewModel
				{
					ChatBody = result.ChatBody,
					GroupId = result.GroupId,
					CreateDate = result.CreateDate.ConvertToPersianDate(),
					UserId = result.UserId,
					FileName = result.FileName,
					UserName = HttpContext.User.GetUserName(),
					
				};

				var users = await _UserGroupService.GetGroupUsers(fileViewModel.GroupId);

				var userIds = new List<string>();
				
				foreach (var groupUserDto in users!)
				{
					if(groupUserDto.UserId == HttpContext.User.GetUserId())
						continue;

					userIds.Add(groupUserDto.UserId.ToString());

				}

				model.IsCallerChat = true;
				await _hubContext.Clients.User(HttpContext.User.GetUserId().ToString()).SendAsync("receiveFile",
					model);

				model.IsCallerChat = false;

				await _hubContext.Clients.Users(userIds).SendAsync("receiveFile",
					model);

				SuccessAlert("فایل به گروه فرستاده شد",false);
				
				return new ObjectResult("success");

			}
			else
			{
				ErrorAlert("فایل باید یا تصویر باشد یا ویدیو", false);
				return new ObjectResult("Error");
			}
			
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
								IsUser = groupDto != null && groupDto.IsUserChat,
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
								IsUser = groupDto != null && groupDto.IsUserChat,
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
							IsUser = groupDto != null && groupDto.IsUserChat,
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

		private ChatGroupViewModel FixUserPrivateChatGroup(ChatGroupViewModel model)
		{

			if (model.ReceiverId == HttpContext.User.GetUserId())
			{
				model.GroupTitle = model.OwnerUser!.UserName;
				model.ImageName = model.OwnerUser.Avatar;
			}

			return model;

		}

		#endregion

	}
}
