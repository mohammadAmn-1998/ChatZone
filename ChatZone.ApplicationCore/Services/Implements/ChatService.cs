using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.Users;
using ChatZone.ApplicationCore.Services.Base;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.Domain.Context;
using ChatZone.Domain.Entities.Chats;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.ApplicationCore.Services.Implements
{
	public class ChatService: BaseService,IChatService
	{
		public ChatService(ChatDbContext context) : base(context)
		{
		}

		public async Task<GroupChatsDto?> GetChats(string groupToken)
		{

			try
			{
				var result = Table<ChatGroup>().Include(c => c.Chats)?.ThenInclude(c=> c.User).Include(ch=> ch.OwnerUser)
					.FirstOrDefault(ch => ch.Token == groupToken );

				if(result == null)
					return null;

				var dto = new GroupChatsDto
				{
					
					GroupId = result.Id,
					OwnerId = result.OwnerId,
					GroupTitle = result.Title,
					ImageName = result.GroupImage,
					IsPrivate = result.IsPrivate,
					ReceiverId = result.ReceiverId ??0,
					CreateDate = result.CreatedDate,
					Chats = result.Chats?.OrderBy(c => c.CreatedDate).Select(c => new ChatDto
					{
						Id = 0,
						CreateDate = c.CreatedDate,
						ChatBody = c.ChatBody,
						UserName = c.User.UserName,
						GroupId = c.GroupId,
						UserId = c.UserId,
						FileName = c.FileName
					}).ToList()
					,OwnerUser = result.OwnerUser != null ?  new UserDto
					{
						Id = result.OwnerUser.Id,
						UserName = result.OwnerUser.UserName,
						Avatar = result.OwnerUser.Avatar,
						Bio = result.OwnerUser.Bio,
						CreatedDate = result.OwnerUser.CreatedDate,
						IsDeleted = result.OwnerUser.IsDeleted,
						
					} :null
				};

				return dto;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			
		}

		public async Task<GroupChatsDto?> GetChats(long groupId)
		{

			try
			{
				var result = Table<ChatGroup>().Include(c => c.Chats).ThenInclude(c => c.User)
					.FirstOrDefault(ch => ch.Id == groupId);

				if (result == null)
					return null;

				var dto = new GroupChatsDto
				{
					GroupId = result.Id,
					GroupTitle = result.Title,
					ImageName = result.GroupImage,
					CreateDate = result.CreatedDate,
					Chats = result.Chats?.OrderBy(c => c.CreatedDate).Select(c => new ChatDto
					{
						Id = 0,
						CreateDate = c.CreatedDate,
						ChatBody = c.ChatBody,
						UserName = c.User.UserName,
						GroupId = c.GroupId,
						UserId = c.UserId,
						FileName = c.FileName
					}).ToList()
				};

				return dto;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		public async Task<ChatDto?> InsertChat(ChatDto dto)
		{

			try
			{
				var chat = new Chat
				{

					CreatedDate = DateTime.Now,
					ChatBody = dto.ChatBody,
					GroupId = dto.GroupId,
					UserId = dto.UserId,
					FileName = dto.FileName,
				};

				Insert(chat);
				await Save();


				var result = Table<Chat>().Include(c=> c.ChatGroup).Include(c=> c.User).FirstOrDefault(c => c.CreatedDate == chat.CreatedDate);

				if (result is null)
					return null;
				return new ChatDto
				{
					Id = result.Id,
					CreateDate = result.CreatedDate,
					ChatBody = result.ChatBody,
					UserName = result.User!.UserName,
					GroupId = result.GroupId,
					UserId = result.UserId,
					FileName = result.FileName,
					GroupName = result.ChatGroup?.Title,
				};
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}
	}
}
