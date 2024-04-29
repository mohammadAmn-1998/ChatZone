using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Services.Base;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.Domain.Context;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.SeedWork.Base;
using Microsoft.EntityFrameworkCore;

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
					ReceiverId = result.ReceiverId ?? 0,

				};

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
				
			}

		}

		public async Task<List<ChatGroupDto>?> GetChatGroupsBy(string title)
		{
			try
			{
				var result = Table<ChatGroup>().Where(c => c.Title == title);
				return await result.Select(r=> new ChatGroupDto
				{
					Id = r.Id,
					CreatedDate = r.CreatedDate,
					IsDeleted = r.IsDeleted,
					Title = r.Title,
					Description = r.Description,
					GroupImage = r.GroupImage,
					Token = r.Token,
					OwnerId = r.OwnerId,
					IsPrivate = r.IsPrivate,
					ReceiverId = r.ReceiverId??0,

				}).ToListAsync();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		public async Task<ChatGroupDto?> GetChatGroupBy(long groupId)
		{

			try
			{
				var result = await Table<ChatGroup>().FirstOrDefaultAsync(c => c.Id == groupId);

				if (result is null)
					return null;

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
					ReceiverId = result.ReceiverId??0,

				};
			}
			catch (Exception e)
			{

				throw new Exception(e.Message);
			}

			

		}

		public async Task<ChatGroupDto?> GetUserPrivateChatGroup(long userId, long currentUserId)
		{
			try
			{
				var result = await Table<ChatGroup>().Include(c=> c.Chats).Include(c=> c.OwnerUser).SingleAsync(c =>
					(c.OwnerId == userId && c.ReceiverId == currentUserId) ||
					(c.OwnerId == currentUserId && c.ReceiverId == userId));

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
					ReceiverId = result.ReceiverId ??0,
					Chats = result.Chats?.Select(chat => new ChatDto
					{
						Id = chat!.Id,
						CreateDate = chat.CreatedDate,
						ChatBody = chat.ChatBody,
						UserName = chat.User?.UserName,
						GroupId = chat.GroupId,
						UserId = chat.UserId,
						FileName = chat.FileName,
						GroupName = chat.ChatGroup?.Title
					}).ToList(),
					OwnerUser = result.OwnerUser
				};
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
