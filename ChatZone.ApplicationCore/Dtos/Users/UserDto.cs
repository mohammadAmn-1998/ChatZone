using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.UserGroups;

namespace ChatZone.ApplicationCore.Dtos.Users
{
	public class UserDto
	{

		public long Id { get; set; }

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public string? Avatar { get; set; }

		public string? Bio { get; set; }

		public DateTime CreatedDate { get; set; }

		public bool IsDeleted { get; set; }

		public List<ChatDto>? Chats { get; set; }

		public List<ChatGroupDto>? ChatGroups { get; set; }

		public List<UserGroupDto>? UserGroups { get; set; }

	}
}
