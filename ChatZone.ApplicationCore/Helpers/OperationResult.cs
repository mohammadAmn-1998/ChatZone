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

		#region Success

		public static OperationResult Success()
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Success,
				Message = "عملیات موفق!",
			};
		}

		public static OperationResult Success(string message)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Success,
				Message = message,
			};
		}

		#endregion

		#region Error

		public static OperationResult Error()
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Error,
				Message = "عملیات ناموفق!",
			};
		}

		public static OperationResult Error(string message)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.Error,
				Message = message,
			};
		}

		#endregion

		#region NotFound

		public static OperationResult NotFound()
		{
			return new OperationResult
			{
				Status = OperationResultStatus.NotFound,
				Message = "صفحه یا چیز مورد نظر پیدا نشد!",
			};
		}

		public static OperationResult NotFound(string message)
		{
			return new OperationResult
			{
				Status = OperationResultStatus.NotFound,
				Message = message,
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
