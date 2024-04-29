using ChatZone.ApplicationCore.Dtos.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Dtos.ChatGroups
{
	public class GroupChatsDto
	{
		public long GroupId { get; set; }

		public string? GroupTitle { get; set; }

		public string? ImageName { get; set; }

		public DateTime? CreateDate { get; set; }

		public List<ChatDto>? Chats { get; set; }


	}
}
