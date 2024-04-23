using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.Domain.Entities.Users;
using ChatZone.Domain.SeedWork.Base;

namespace ChatZone.Domain.Entities.Chats
{
	public class Chat : BaseEntity
	{
		[MaxLength(2000)]
		public string? ChatBody { get; set; }

		public long GroupId { get; set; }

		public long UserId { get; set; }

		public string? FileName { get; set; }


		#region Relations

		[ForeignKey(nameof(GroupId))]
		public ChatGroup? ChatGroup { get; set; }

		[ForeignKey(nameof(UserId))]
		public User? User { get; set; }

		#endregion


	}
}
