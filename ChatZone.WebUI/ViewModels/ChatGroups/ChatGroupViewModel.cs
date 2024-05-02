using ChatZone.WebUI.ViewModels.Chats;

namespace ChatZone.WebUI.ViewModels.ChatGroups
{
	public class ChatGroupViewModel
	{
		public long GroupId { get; set; }

		public bool IsUser { get; set; }

		public long UserId { get; set; }

		public string? GroupTitle { get; set; }

		public string? ImageName { get; set; }

		public string? CreateDate { get; set; }

		public bool IsJoined { get; set; }

		public long OwnerId { get; set; }

		public long ReceiverId { get; set; }

		public List<ChatViewModel>? Chats { get; set; }
	}
}
