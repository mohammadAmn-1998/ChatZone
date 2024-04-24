using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.Domain.Entities.Chats;

namespace ChatZone.ApplicationCore.Dtos.UserGroups
{
	public class GroupSearchResultsDto
	{

		public bool IsUser { get; set; }

		public string? Title { get; set; }

		public string? GroupImage { get; set; }

		public string? Token { get; set; }

		public string? LastChat { get; set; }

		public DateTime? LastChatDate { get; set; }


	}
}
