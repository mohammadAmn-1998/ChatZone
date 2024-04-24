using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Services.Interfaces
{
	public interface IFileManagerService
	{
		Task<string?> UploadFileAndReturnFileName(IFormFile file, string directoryPath);

	}
}
