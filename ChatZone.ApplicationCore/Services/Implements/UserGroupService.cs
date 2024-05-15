using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.UserGroups;
using ChatZone.ApplicationCore.Dtos.Users;
using ChatZone.ApplicationCore.Helpers;
using ChatZone.ApplicationCore.Services.Base;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.Domain.Context;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.ApplicationCore.Services.Implements
{
	public class UserGroupService : BaseService, IUserGroupService
	{


		public UserGroupService(ChatDbContext context) : base(context)
		{
		}

		public async Task JoinToGroup(UserGroupDto dto)
		{

			try
			{


				var userGroup = new UserGroup
				{
					CreatedDate = DateTime.Now,
					IsDeleted = false,
					UserId = dto.UserId,
					GroupId = dto.GroupId,

				};

				Insert(userGroup);
				await Save();

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		public async Task<List<UserGroupDto>?> GetUserGroupList(long userId)
		{

			try
			{
				var result = Table<UserGroup>()

					.Include(ug => ug.ChatGroup).Include(ug => ug.ChatGroup!.OwnerUser)
					.Include(ug => ug.ChatGroup!.ReceiverUser).Include(ug => ug.ChatGroup!.Chats)
					.Where(u => u.UserId == userId)
					.OrderByDescending(ug => ug.CreatedDate)
					.Select(u => new UserGroupDto()
					{
						Id = u.Id,
						GroupId = u.GroupId,
						IsUserChat = u.ChatGroup != null && u.ChatGroup.ReceiverId != 0,
						UserId = userId,
						ChatGroup = new ChatGroupDto
						{
							Id = u.ChatGroup.Id,
							CreatedDate = u.ChatGroup.CreatedDate,
							IsDeleted = u.ChatGroup.IsDeleted,
							Title = u.ChatGroup.Title,
							Description = u.ChatGroup.Description,
							GroupImage = u.ChatGroup.GroupImage,
							Token = u.ChatGroup.Token,
							OwnerId = u.ChatGroup.OwnerId,
							IsPrivate = u.ChatGroup.IsPrivate,
							ReceiverId = u.ChatGroup.ReceiverId ?? 0,
							Chats = u.ChatGroup!.Chats.Select(ch => new ChatDto
							{
								Id = ch.Id,
								CreateDate = ch.CreatedDate,
								ChatBody = ch.ChatBody,
								GroupId = ch.GroupId,
								UserId = ch.UserId,
								FileName = ch.FileName
							}).ToList() ?? null,
							OwnerUser = new UserDto
							{
								Id = u.ChatGroup.OwnerUser!.Id,
								UserName = u.ChatGroup.OwnerUser!.UserName,
								Password = u.ChatGroup.OwnerUser!.Password,
								Avatar = u.ChatGroup.OwnerUser!.Avatar,
								Bio = u.ChatGroup.OwnerUser!.Bio,
								CreatedDate = u.ChatGroup.OwnerUser!.CreatedDate,
								IsDeleted = u.ChatGroup.OwnerUser!.IsDeleted,
							},
							ReceiverUser = u.ChatGroup.ReceiverUser != null
								? new UserDto
								{
									Id = u.ChatGroup.ReceiverUser.Id,
									UserName = u.ChatGroup.ReceiverUser!.UserName,
									Password = u.ChatGroup.ReceiverUser!.Password,
									Avatar = u.ChatGroup.ReceiverUser!.Avatar,
									Bio = u.ChatGroup.ReceiverUser!.Bio,
									CreatedDate = u.ChatGroup.ReceiverUser!.CreatedDate,
									IsDeleted = u.ChatGroup.ReceiverUser!.IsDeleted,
								}
								: null

						},
						CreateDate = u.CreatedDate,



					});



				return await result.ToListAsync();


			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}



		}

		public async Task<List<GroupSearchResultsDto>?> SearchBy(string title)
		{

			try
			{
				var result = new List<GroupSearchResultsDto>();

				var chatGroups = await Table<ChatGroup>().Where(ch => ch.Title!.Contains(title) && !ch.IsPrivate)
					.Include(ch => ch.Chats)
					.Select(ch => new GroupSearchResultsDto
					{

						IsUser = false,
						Title = ch.Title,
						GroupImage = ch.GroupImage,
						Token = ch.Token,
						LastChat = ch.Chats != null
							? ch.Chats.OrderByDescending(c => c.CreatedDate).First().ChatBody
							: null,
						LastChatDate = ch.Chats != null
							? ch.Chats.OrderBy(c => c.CreatedDate).First().CreatedDate
							: null,
					}).ToListAsync();


				var users = await Table<User>().Where(u => u.UserName!.Contains(title)).Select(u =>
					new GroupSearchResultsDto
					{
						IsUser = true,
						Title = u.UserName,
						GroupImage = u.Avatar,
						Token = u.Id.ToString(),
						LastChat = u.Chats != null
							? u.Chats.OrderByDescending(c => c.CreatedDate).First().ChatBody
							: null,
						LastChatDate = u.Chats != null ? u.Chats.OrderBy(c => c.CreatedDate).First().CreatedDate : null,
					}).ToListAsync();

				result.AddRange(chatGroups);
				result.AddRange(users);

				return result;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}




		}

		public async Task<List<GroupUserDto>?> GetGroupUsers(long groupId)
		{

			try
			{

				return await Table<UserGroup>().Include(ug => ug.User).Where(g => g.GroupId == groupId).Select(g =>
					new GroupUserDto
					{
						UserId = g.UserId,
						UserName = g.User!.UserName ?? null
					}).ToListAsync();


			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		public async Task<bool> IsJoinedGroup(long groupId, long userId)
		{
			try
			{
				return await Table<UserGroup>().AnyAsync(ug => ug.GroupId == groupId && ug.UserId == userId);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		public async Task<bool> IsUserAlreadyJoinedToPrivateChat(long userId, long currentUserId)
		{
			try
			{
				return await Table<ChatGroup>().AnyAsync(c =>
					c.OwnerId == userId && c.ReceiverId == currentUserId ||
					c.OwnerId == currentUserId && c.ReceiverId == userId);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}



		}

		public async Task<OperationResult> LeaveGroup(long userId, long groupId)
		{

			try
			{

				var userGroup =
					await Table<UserGroup>().SingleOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
				if (userGroup is null)
					return OperationResult.NotFound();

				userGroup.IsDeleted = true;

				Update(userGroup);
				await Save();

				return OperationResult.Success("شما گروه را ترک کردید!");

			}
			catch (Exception e)
			{
				return OperationResult.Error();
			}
		}

		public async Task<UserGroupDto?> GetUserGroupBy(long groupId)
		{
			try
			{
				var userGroup = await Table<UserGroup>().Include(ug => ug.ChatGroup)
					.Include(ug => ug.ChatGroup!.OwnerUser)
					.Include(ug => ug.ChatGroup!.ReceiverUser).Include(ug => ug.ChatGroup!.Chats)
					.FirstAsync(ug => ug.GroupId == groupId);

				return new UserGroupDto
				{
					Id = userGroup.Id,
					GroupId = userGroup.GroupId,
					IsUserChat = userGroup.ChatGroup != null && userGroup.ChatGroup.ReceiverId != 0,
					UserId = userGroup.ChatGroup!.OwnerId,
					ChatGroup = new ChatGroupDto
					{
						Id = userGroup.ChatGroup.Id,
						CreatedDate = userGroup.ChatGroup.CreatedDate,
						IsDeleted = userGroup.ChatGroup.IsDeleted,
						Title = userGroup.ChatGroup.Title,
						Description = userGroup.ChatGroup.Description,
						GroupImage = userGroup.ChatGroup.GroupImage,
						Token = userGroup.ChatGroup.Token,
						OwnerId = userGroup.ChatGroup.OwnerId,
						IsPrivate = userGroup.ChatGroup.IsPrivate,
						ReceiverId = userGroup.ChatGroup.ReceiverId ?? 0,
						Chats = userGroup.ChatGroup?.Chats?.Select(ch => new ChatDto
						{
							Id = ch.Id,
							CreateDate = ch.CreatedDate,
							ChatBody = ch.ChatBody,
							GroupId = ch.GroupId,
							UserId = ch.UserId,
							FileName = ch.FileName
						}).ToList() ?? null,
						OwnerUser = new UserDto
						{
							Id = userGroup.ChatGroup!.OwnerUser!.Id,
							UserName = userGroup.ChatGroup.OwnerUser!.UserName,
							Password = userGroup.ChatGroup.OwnerUser!.Password,
							Avatar = userGroup.ChatGroup.OwnerUser!.Avatar,
							Bio = userGroup.ChatGroup.OwnerUser!.Bio,
							CreatedDate = userGroup.ChatGroup.OwnerUser!.CreatedDate,
							IsDeleted = userGroup.ChatGroup.OwnerUser!.IsDeleted,
						},
						ReceiverUser = userGroup.ChatGroup.ReceiverUser != null
							? new UserDto
							{
								Id = userGroup.ChatGroup.ReceiverUser.Id,
								UserName = userGroup.ChatGroup.ReceiverUser!.UserName,
								Password = userGroup.ChatGroup.ReceiverUser!.Password,
								Avatar = userGroup.ChatGroup.ReceiverUser!.Avatar,
								Bio = userGroup.ChatGroup.ReceiverUser!.Bio,
								CreatedDate = userGroup.ChatGroup.ReceiverUser!.CreatedDate,
								IsDeleted = userGroup.ChatGroup.ReceiverUser!.IsDeleted,
							}
							: null

					},
					CreateDate = userGroup.CreatedDate,

				};
			}
			catch (Exception e)
			{
				return null;
			}
		}

	}
}
