using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.SeedWork.Base;

namespace ChatZone.Domain.Entities.Users
{
	public class User : BaseEntity
	{

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public string? Avatar { get; set; }

		public string? Bio { get; set; }

		#region Relations

		public List<Chat>? Chats { get; set; }

		[InverseProperty("OwnerUser")]
		public ICollection<ChatGroup>? OwnerChatGroups { get; set; }
		
		[InverseProperty("ReceiverUser")]
		public ICollection<ChatGroup>? ReceiverChatGroups { get; set; }
		
		public List<UserGroup>? UserGroups { get; set; }

		#endregion


	}
}
