using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Dtos.Users
{
	public class UserDto
	{

		public long Id { get; set; }

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public string? Avatar { get; set; }

		public string? Bio { get; set; }

		public DateTime CreatedDate { get; set; }

		public bool IsDeleted { get; set; }

		

	}
}
