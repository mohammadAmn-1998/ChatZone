using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
