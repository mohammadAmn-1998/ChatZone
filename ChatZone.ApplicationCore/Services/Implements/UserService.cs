using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.Chats;
using ChatZone.ApplicationCore.Dtos.Users;
using ChatZone.ApplicationCore.Services.Base;
using ChatZone.ApplicationCore.Services.Interfaces;
using ChatZone.Domain.Context;
using ChatZone.Domain.Entities.Users;
using ChatZone.Domain.SeedWork.Base;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.ApplicationCore.Services.Implements
{
	public class UserService : BaseService , IUserService
	{
		public UserService(ChatDbContext context) : base(context)
		{
		}

		public async Task AddNewUser(UserDto dto)
		{
			try
			{
				var user = new User
				{
					CreatedDate = DateTime.Now,
					UserName = dto.UserName,
					Password = dto.Password,
					Avatar = "Default.jpg"
				};

				Insert(user);
				await Save();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			
		}

		public async Task<UserDto?> GetUserByUserNameAndPassword(string userName, string password)
		{
			try
			{

				var user = await Table<User>().FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

				if (user == null)
					return null;

				return new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					Password = user.Password,
					Avatar = user.Avatar,
					Bio = user.Bio,
					CreatedDate = user.CreatedDate,
				};
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		public async Task<bool> IsUserNameExists(string userName)
		{
			try
			{
				return await Table<User>().AnyAsync(u => u.UserName == userName);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			
		}

		public async Task<UserDto?> GetUserById(long id)
		{

			try
			{
				var user = await Table<User>().Include(u => u.Chats).ThenInclude(c => c.ChatGroup).FirstOrDefaultAsync(u => u.Id == id);

				if (user is null)
					return null;
				return new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					Password = null,
					Avatar = user.Avatar,
					Bio = user.Bio,
					CreatedDate = user.CreatedDate,
					IsDeleted = user.IsDeleted,
					Chats = user.Chats?.OrderBy(c => c.CreatedDate).Select(c => new ChatDto
					{
						Id = c.Id,
						CreateDate = c.CreatedDate,
						ChatBody = c.ChatBody,
						UserName = user.UserName,
						GroupId = c.GroupId,
						UserId = user.Id,
						FileName = c.FileName,
						GroupName = c.ChatGroup?.Title,
					}).ToList()
				};
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

			
		}

		public async Task<List<SearchUserDto>?> SearchBy(string userName, long currentGroupId)
		{

			try
			{
				return await Table<User>().Include(u=> u.UserGroups).Where(u => u.UserName!.Contains(userName)).OrderBy(u=> u.UserName).Select(u => new SearchUserDto
				{
					Id = u.Id,
					UserName = u.UserName,
					IsMember =  Table<UserGroup>().Any(ug=> ug.GroupId == currentGroupId && ug.UserId == u.Id)
				}).ToListAsync();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			
		}
	}
}
