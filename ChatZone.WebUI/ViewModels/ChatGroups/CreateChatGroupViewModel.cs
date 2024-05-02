namespace ChatZone.WebUI.ViewModels.ChatGroups
{
	public class CreateChatGroupViewModel
	{

		public string? Title { get; set; }

		public IFormFile? ImageFile { get; set; }

		public bool IsPrivate { get; set; }

	}
}
