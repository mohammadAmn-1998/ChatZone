using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Dtos.UserGroups;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public interface IUserGroupService
	{


		Task AddNewUserGroup(UserGroupDto dto);

		Task<List<UserGroupDto>?> GetUserGroupList(long userId);

		Task<List<GroupSearchResultsDto>?> SearchBy(string title);

	}
}
