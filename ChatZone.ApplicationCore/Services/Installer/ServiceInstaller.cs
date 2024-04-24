using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.ApplicationCore.Services.Implements;
using ChatZone.ApplicationCore.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChatZone.ApplicationCore.Services.Installer
{
	public static class ServiceInstaller
	{

		public static void AddCustomServices(this IServiceCollection services)
		{

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IChatGroupService, ChatGroupService>();
			services.AddScoped<IUserGroupService, UserGroupService>();
			services.AddScoped<IFileManagerService, FileManagerService>();

		}

	}
}
