using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.Domain.Entities.Users;
using ChatZone.Domain.SeedWork.Base;

namespace ChatZone.Domain.Entities.Chats
{
	public class ChatGroup : BaseEntity
	{

		public string? Title { get; set; }

		public string? Description { get; set; }

		public string? GroupImage { get; set; }

		public string? Token { get; set; }

		public long OwnerId { get; set; }

		public bool IsPrivate { get; set; }

		public long ReceiverId { get; set; }

		#region Relations

		public List<Chat>? Chats { get; set; }

		[ForeignKey(nameof(OwnerId))]
		public User? OwnerUser { get; set; }

		public List<UserGroup>? UserGroups { get; set; }

		#endregion

	}
}
