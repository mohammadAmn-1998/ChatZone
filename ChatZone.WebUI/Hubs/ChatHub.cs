using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.WebUI.ViewModels.Chats;
using Microsoft.AspNetCore.SignalR;

namespace ChatZone.WebUI.Hubs;

public class ChatHub : Hub, IChatHub
{
	private readonly IChatService _chatService;
	private readonly IUserGroupService _userGroupService;

	public ChatHub(IChatService chatService, IUserGroupService userGroupService)
	{
		_chatService = chatService;
		_userGroupService = userGroupService;
	}

	public async Task AddNewMessage(string message, string id)
	{
		var groupId = Convert.ToInt64(id);

		var dto = new ChatDto
		{
			ChatBody = message,
			UserId = Context.User.GetUserId(),
			GroupId = groupId
		};

		var result = await _chatService.InsertChat(dto);

		if (result is null)
		{
			await Clients.Caller.SendAsync("receiveChat", "Error");
			return;
		}

		var groupUsersList = await _userGroupService.GetGroupUsers(groupId);

		if (groupUsersList == null)
			throw new NullReferenceException();

		var usersIds = new List<string>();

		foreach (var user in groupUsersList)
		{
			if (user.UserId == Context.User.GetUserId())
				continue;
			usersIds.Add(user.UserId.ToString());
		}

		var model = new ChatViewModel
		{
			ChatBody = result.ChatBody,
			GroupId = result.GroupId,
			CreateDate = result.CreateDate.ConvertToPersianDate(),
			UserId = result.UserId,
			FileName = result.FileName,
			UserName = result.UserName,
			GroupName = result.GroupName
		};


		await Clients.Users(usersIds).SendAsync("receiveChat", model);

		await Clients.Users(usersIds).SendAsync("receiveChatNotification", model);

		model.IsCallerChat = true;


		await Clients.Caller.SendAsync("receiveChat", model);
	}
}

public interface IChatHub
{
	Task AddNewMessage(string message, string id);
}