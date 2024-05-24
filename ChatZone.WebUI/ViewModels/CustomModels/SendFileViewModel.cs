namespace ChatZone.WebUI.ViewModels.CustomModels
{
	public class SendFileViewModel
	{

		public long GroupId { get; set; }

		public IFormFile? File { get; set; }

		public string? Caption { get; set; }
	}
}
