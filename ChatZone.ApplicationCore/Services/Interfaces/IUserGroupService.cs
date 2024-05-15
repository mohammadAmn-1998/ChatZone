using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.ChatGroups;
using ChatZone.ApplicationCore.Dtos.UserGroups;
using ChatZone.ApplicationCore.Helpers;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public interface IUserGroupService
	{


		Task JoinToGroup(UserGroupDto dto);

		Task<List<UserGroupDto>?> GetUserGroupList(long userId);

		Task<List<GroupSearchResultsDto>?> SearchBy(string title);

		Task<List<GroupUserDto>?> GetGroupUsers(long groupId);

		Task<bool> IsJoinedGroup(long groupId, long userId);

		Task<bool> IsUserAlreadyJoinedToPrivateChat(long userId, long currentUserId);

		Task<OperationResult> LeaveGroup(long userId,long groupId);
		
		Task<UserGroupDto?> GetUserGroupBy(long groupId);

	}
}
