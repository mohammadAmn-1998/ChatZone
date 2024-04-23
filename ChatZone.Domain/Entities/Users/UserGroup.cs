using System.ComponentModel.DataAnnotations.Schema;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.SeedWork.Base;

namespace ChatZone.Domain.Entities.Users;

public class UserGroup : BaseEntity
{

	public long UserId { get; set; }

	public long GroupId { get; set; }

	#region Relations

	[ForeignKey(nameof(UserId))] 
	public User? User { get; set; }

	[ForeignKey(nameof(GroupId))]
	public ChatGroup? ChatGroup { get; set; }

	#endregion


}