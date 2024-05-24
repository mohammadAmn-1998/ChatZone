using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Helpers
{
	public static class Directories
	{

		public const string UserGroupImageDirectory = "wwwroot/assets/home_page/img/group_img";

		public const string ChatFilesDirectory = "wwwroot/assets/home_page/img/chat_files";

		public static string GetGroupImagePath(string? imageName)
		{

			if (imageName == null)
				return "/assets/home_page/img/Default.jpg";

			return Path.Combine(UserGroupImageDirectory, imageName).Replace("wwwroot", "");

		}

		public static string GetUserGroupImageDirectory()
		{

			return Path.Combine(Directory.GetCurrentDirectory(), UserGroupImageDirectory.Replace("/", "\\"));

		}


		public static string GetChatFilesDirectory()
		{

			return Path.Combine(Directory.GetCurrentDirectory(), ChatFilesDirectory.Replace("/", "\\"));

		}

		public static string GetChatFilePath(string? fileName)
		{

			if (fileName == null)
				return "/assets/home_page/img/Default.jpg";

			return Path.Combine(ChatFilesDirectory, fileName).Replace("wwwroot", "");

		}
	}
}
