using ChatZone.ApplicationCore.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ChatZone.ApplicationCore.Services.Implements;

public class FileManagerService : IFileManagerService
{
	public async Task<string?> UploadFileAndReturnFileName(IFormFile file, string directoryPath)
	{

		if (!Path.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

		var filePath = Path.Combine(directoryPath, fileName);

		if (!await CopyFileToFilePath(file, filePath))
		{

			return null;

		}

		return fileName;
	}

	private async Task<bool> CopyFileToFilePath(IFormFile file, string filePath)
	{

		try
		{
			var str = new FileStream(filePath, FileMode.Create);

			await file.CopyToAsync(str);
			return true;

		}
		catch
		{
			return false;
		}

	}
}