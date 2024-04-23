using ChatZone.ApplicationCore.Dtos.Users;

namespace ChatZone.ApplicationCore.Services.Interfaces;

public interface IUserService
{

	Task AddNewUser(UserDto dto);

	Task<UserDto?> GetUserByUserNameAndPassword(string userName, string password);

	Task<bool> IsUserNameExists(string userName);

}