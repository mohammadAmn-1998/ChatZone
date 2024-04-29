using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.Users;

namespace ChatZone.ApplicationCore.Dtos.ChatGroups
{
	public class ChatGroupDto
	{
		public long Id { get; set; }

		public DateTime CreatedDate { get; set; }

		public bool IsDeleted { get; set; }

		public string? Title { get; set; }

		public string? Description { get; set; }

		public string? GroupImage { get; set; }

		public string? Token { get; set; }

		public long OwnerId { get; set; }

		public bool IsPrivate { get; set; }

		public long? ReceiverId { get; set; }

		public List<ChatDto>? Chats { get; set; }

		public List<UserGroup>? UserGroups { get; set; }

		public UserDto? OwnerUser { get; set; }
		
		public UserDto? ReceiverUser { get; set; }

	}
}
