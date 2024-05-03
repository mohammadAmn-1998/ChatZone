using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Users;

namespace ChatZone.ApplicationCore.Dtos.UserGroups
{
	public class UserGroupDto
	{

		public long Id { get; set; }

		public DateTime? CreateDate { get; set; }

		public long UserId { get; set; }

		public long GroupId { get; set; }

		public UserDto? User { get; set; }

		public ChatGroupDto? ChatGroup { get; set; }

		public bool IsUserChat { get; set; }

		

	}
}
