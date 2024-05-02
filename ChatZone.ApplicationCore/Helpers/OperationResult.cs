using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.ApplicationCore.Helpers
{
	public  class OperationResult
	{

		public string Message { get; set; }
		public OperationResultStatus Status { get; set; }
		public bool IsReload { get; set; }

		#region Success

		public static OperationResult Success(bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Success,
				Message = "عملیات موفق!",
				IsReload = isReload
			};
		}

		public static OperationResult Success(string message, bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Success,
				Message = message,
				IsReload = isReload
			};
		}

		#endregion

		#region Error

		public static OperationResult Error(bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Error,
				Message = "عملیات ناموفق!",
				IsReload = isReload
			};
		}

		public static OperationResult Error(string message, bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Error,
				Message = message,
				IsReload = isReload
			};
		}

		#endregion

		#region NotFound

		public static OperationResult NotFound(bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.NotFound,
				Message = "صفحه یا چیز مورد نظر پیدا نشد!",
				IsReload = isReload
			};
		}

		public static OperationResult NotFound(string message, bool isReload = false)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.NotFound,
				Message = message,
				IsReload = isReload
			};
		}

		#endregion
	}

	public enum OperationResultStatus
	{

		Error=10,
		Success = 200,
		NotFound = 404

	}
}
