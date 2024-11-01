﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Helpers;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public  interface IChatGroupService
	{
		Task<ChatGroupDto> InsertNewChatGroup(ChatGroupDto dto);

		Task<List<ChatGroupDto>?> GetChatGroupsBy(string title);

		Task<ChatGroupDto?> GetChatGroupBy(long groupId);

		Task<ChatGroupDto?> GetUserPrivateChatGroup(long userId, long currentUserId);

		Task<OperationResult> DeleteChatGroup(long groupId);
	}
}
