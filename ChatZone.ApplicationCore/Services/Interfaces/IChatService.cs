using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public interface IChatService
	{

		Task<GroupChatsDto?> GetChats(string groupToken);

		Task<GroupChatsDto?> GetChats(long groupId);

		Task<ChatDto?> InsertChat(ChatDto dto);

		
	}
}
