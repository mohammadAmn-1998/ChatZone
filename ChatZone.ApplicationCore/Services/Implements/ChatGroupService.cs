using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Services.Base;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.Domain.Context;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.SeedWork.Base;

namespace ChatZone.ApplicationCore.Services.Implements
{
	public class ChatGroupService  : BaseService,IChatGroupService
	{
		public ChatGroupService(ChatDbContext context) : base(context)
		{
		}

		public async Task<ChatGroupDto?> InsertNewChatGroup(ChatGroupDto dto)
		{

			try
			{

				var chatGroup = new ChatGroup
				{
					
					CreatedDate = DateTime.Now,
					Title = dto.Title,
					GroupImage = dto.GroupImage,
					Token = Guid.NewGuid().ToString(),
					OwnerId = dto.OwnerId,
					IsPrivate = dto.IsPrivate,
					ReceiverId = dto.ReceiverId,

				};

				Insert(chatGroup);
				await Save();

				var result = Table<ChatGroup>().First(c => c.Token == chatGroup.Token);

				return new ChatGroupDto
				{
					Id = result.Id,
					CreatedDate = result.CreatedDate,
					IsDeleted = result.IsDeleted,
					Title = result.Title,
					Description = result.Description,
					GroupImage = result.GroupImage,
					Token = result.Token,
					OwnerId = result.OwnerId,
					IsPrivate = result.IsPrivate,
					ReceiverId = result.ReceiverId,

				};

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
				
			}

		}
	}
}
