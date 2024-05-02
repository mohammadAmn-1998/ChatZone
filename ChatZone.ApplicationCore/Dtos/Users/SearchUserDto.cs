using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Dtos.Users
{
	public class SearchUserDto
	{

		public long Id { get; set; }

		public string? UserName { get; set; }

		public bool IsMember { get; set; }


	}
}
