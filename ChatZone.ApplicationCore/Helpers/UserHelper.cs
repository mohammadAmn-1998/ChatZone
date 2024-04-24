using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Exception = System.Exception;

namespace ChatZone.ApplicationCore.Helpers
{
	public static class UserHelper
	{

		public static long GetUserId(this ClaimsPrincipal? claims)
		{

			try
			{
				if (claims == null)
					return 0;

				var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				return Convert.ToInt64(userId);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			
		}

	}
}
