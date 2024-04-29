using ChatZone.ApplicationCore.Dtos.Chats;
using System.ComponentModel.DataAnnotations;

namespace ChatZone.WebUI.ViewModels.Chats
{
	public class ChatViewModel
	{
		public string? GroupName { get; set; }

		public string? ChatBody { get; set; }

		public long GroupId { get; set; }

		public string? CreateDate { get; set; }

		public long UserId { get; set; }

		public string? FileName { get; set; }

		public string? UserName { get; set; }

		

		public bool IsCallerChat { get; set; }



	}
}
