﻿using System;
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
	public class UserGroupService : BaseService,IUserGroupService
	{
		public UserGroupService(ChatDbContext context) : base(context)
		{
		}

		public async Task AddNewUserGroup(UserGroupDto dto)
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
				return await Table<UserGroup>().Where(u => u.UserId == userId).Include(u => u.ChatGroup).ThenInclude(u => u!.Chats)
					.Select(u => new UserGroupDto()
					{
						Id = u.Id,
						GroupId = u.GroupId,
						User = new UserDto
						{
							Id = u.User.Id,
							UserName = u.User.UserName,
							Password = u.User.Password,
							Avatar = u.User.Avatar,
							Bio = u.User.Bio,
							CreatedDate = u.User.CreatedDate,
							IsDeleted = u.User.IsDeleted,
						},
						UserId = userId,
						ChatGroup = new ChatGroupDto
						{
							Id = u.ChatGroup.Id ,
							CreatedDate = u.ChatGroup.CreatedDate,
							IsDeleted = u.ChatGroup.IsDeleted,
							Title = u.ChatGroup.Title,
							Description = u.ChatGroup.Description,
							GroupImage = u.ChatGroup.GroupImage,
							Token = u.ChatGroup.Token,
							OwnerId = u.ChatGroup.OwnerId,
							IsPrivate = u.ChatGroup.IsPrivate,
							ReceiverId = u.ChatGroup.ReceiverId,
							Chats = u.ChatGroup.Chats.Select(ch=> new ChatDto
							{
								Id = ch.Id,
								CreateDate = ch.CreatedDate,
								ChatBody = ch.ChatBody,
								GroupId = ch.GroupId,
								UserId = ch.UserId,
								FileName = ch.FileName
							}).ToList()?? null
							
						},
						CreateDate = u.CreatedDate
					}).ToListAsync();
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

				var chatGroups = await Table<ChatGroup>().Where(ch => ch.Title!.Contains(title) && !ch.IsPrivate).Include(ch => ch.Chats)
					.Select(ch => new GroupSearchResultsDto
					{

						IsUser = false,
						Title = ch.Title,
						GroupImage = ch.GroupImage,
						Token = ch.Token,
						LastChat = ch.Chats != null ? ch.Chats.OrderByDescending(c => c.CreatedDate).First().ChatBody : null,
						LastChatDate = ch.Chats != null ? ch.Chats.OrderBy(c => c.CreatedDate).First().CreatedDate : null,
					}).ToListAsync();


				var users = await Table<User>().Where(u => u.UserName!.Contains(title)).Select(u => new GroupSearchResultsDto
				{
					IsUser = true,
					Title = u.UserName,
					GroupImage = u.Avatar,
					Token = null,
					LastChat = null,
					LastChatDate = null
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
	}
}