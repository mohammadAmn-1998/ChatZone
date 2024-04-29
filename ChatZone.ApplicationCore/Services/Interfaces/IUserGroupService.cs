using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.UserGroups;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public interface IUserGroupService
	{


		Task AddNewUserGroup(UserGroupDto dto);

		Task<List<UserGroupDto>?> GetUserGroupList(long userId);

		Task<List<GroupSearchResultsDto>?> SearchBy(string title);

		Task<List<GroupUserDto>?> GetGroupUsers(long groupId);

		Task<bool> IsJoinedGroup(long groupId, long userId);

		Task<bool> IsUserAlreadyJoinedToPrivateChat(long userId, long currentUserId);

	}
}
