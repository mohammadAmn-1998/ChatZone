using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public  interface IChatGroupService
	{
		Task<ChatGroupDto?> InsertNewChatGroup(ChatGroupDto dto);

	}
}
