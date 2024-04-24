using System.ComponentModel.DataAnnotations;

namespace ChatZone.ApplicationCore.Dtos.Chats;

public class ChatDto
{

	public long Id { get; set; }

	public DateTime CreateDate { get; set; }

	public string? ChatBody { get; set; }

	public long GroupId { get; set; }

	public long UserId { get; set; }

	public string? FileName { get; set; }
}