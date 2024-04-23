using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Helper
{
	public static class Encoder
	{

		private const string Salt = "ThIsIsSaLt";

		#region MD5

		public static string EncodeToMd5(this string text) //Encrypt using MD5   
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			var originalBytes = Encoding.Default.GetBytes(text + Salt);
			var encodedBytes = md5.ComputeHash(originalBytes);
			return BitConverter.ToString(encodedBytes);
		}
		/// <summary>
		/// If the first parameter was equal to the second parameter Return True (in md5 type)
		/// </summary>
		/// <param name="firstParam">This Is a Md5 Text</param>
		/// <param name="secondParam">This is a String Text</param>
		/// <returns></returns>
		public static bool IsCompareMd5Text(this string md5Text, string secondParam) //Encrypt using MD5   
		{
			secondParam = secondParam.EncodeToMd5();
			return md5Text == secondParam;
		}
		public static bool IsMD5(string input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return false;
			}
			return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
		}

		#endregion

		#region SHA256

		public static string EncodeToSha256(this string? input)
		{

			var bytes = Encoding.UTF8.GetBytes(input + Salt);

			var sha256HashString = new SHA256Managed();

			var hash = sha256HashString.ComputeHash(bytes);

			return Encoding.UTF8.GetString(hash);
		}

		public static bool IsSha256(this string input)
		{

			if (string.IsNullOrEmpty(input))
			{
				return false;
			}

			return Regex.IsMatch(input + Salt, "^[0-9a-fA-F]{64}$", RegexOptions.Compiled);

		}

		public static bool CompareWithSha256Text(this string sha256Text, string secondParam)
		{

			var hashedSecondParam = secondParam.EncodeToSha256();
			return sha256Text == secondParam;

		}

		#endregion


	}
}
