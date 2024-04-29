using System.Diagnostics.Eventing.Reader;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Users;
using ChatZone.Domain.Entities.Chats;

namespace ChatZone.WebUI.ViewModels.UserGroups
{
	public class UserGroupViewModel
	{

		public string? Title { get; set; }

		public string? ImageName { get; set; }

		public string? Token { get; set; }

		public bool IsUser { get; set; }

		public string? LastChat { get; set; }

		public string? LastChatDate { get; set; }

	}
}
