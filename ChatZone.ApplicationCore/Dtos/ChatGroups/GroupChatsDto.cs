using ChatZone.ApplicationCore.Dtos.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.Users;

namespace ChatZone.ApplicationCore.Dtos.ChatGroups
{
	public class GroupChatsDto
	{
		public long GroupId { get; set; }

		public string? GroupTitle { get; set; }

		public string? ImageName { get; set; }

		public long OwnerId { get; set; }

		public long ReceiverId { get; set; }

		public DateTime? CreateDate { get; set; }

		public bool IsPrivate { get; set;  }

		public List<ChatDto>? Chats { get; set; }

		public UserDto? OwnerUser { get; set; }


	}
}
