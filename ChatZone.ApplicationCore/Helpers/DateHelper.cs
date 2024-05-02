using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatZone.ApplicationCore.Helpers
{
	public static class DateHelper
	{

		private static readonly PersianCalendar PCalendar = new PersianCalendar();

		public static string? ConvertToPersianDate(this DateTime time)
		{

			try
			{
				return string.Format(
					$"({HowManyDaysPast(time)}) {GetPersianDayOfTheWeekName(time)} {PCalendar.GetDayOfMonth(time)} {GetPersianMonthOfYearName(time)} {PCalendar.GetYear(time)}");
			}
			catch (Exception e)
			{
				return null;
			}
				
			
			
		}

		#region Private Methods

		private static string HowManyDaysPast(DateTime time)
		{
			

			var yearsLeft = DateTime.Now.Year - time.Year;
			var monthLeft = Math.Abs(DateTime.Now.Month - time.Month);
			var daysPast = Math.Abs(DateTime.Now.DayOfYear - time.DayOfYear);
			var hoursLeft = Math.Abs(DateTime.Now.Hour - time.Hour);
			var minutesLeft = (DateTime.Now.Minute < time.Minute) ? Math.Abs((60 + DateTime.Now.Minute) - time.Minute) : Math.Abs(DateTime.Now.Minute - time.Minute);
			
			var secondsLeft = Math.Abs(DateTime.Now.Second - time.Second);

			if (yearsLeft != 0)
			{
				return string.Format($"{yearsLeft} سال پیش");
			}

			if (monthLeft != 0 && daysPast > 29)
			{
				return string.Format($"{monthLeft} ماه پیش");
			}

			if (monthLeft == 1 && daysPast <= 29)
			{
				return string.Format($"{daysPast} روز پیش");
			}
			if (daysPast == 0 && hoursLeft != 0)
			{
				if (minutesLeft < 60)
				{
					return string.Format("{0} ساعت{1} و  دقیقه پیش 	", hoursLeft,minutesLeft);
				}

				return string.Format($"{hoursLeft} ساعت پیش");
			}

			if (daysPast == 0 && hoursLeft == 0 && minutesLeft != 0)
			{
				return string.Format($"{minutesLeft} دقیقه پیش");
			}

			if (daysPast == 0 && hoursLeft == 0 && minutesLeft == 0 && secondsLeft != 0)
			{
				return string.Format($"{secondsLeft} ثانیه پیش");
			}

			if (daysPast == 0 && hoursLeft == 0 && minutesLeft == 0 && secondsLeft == 0)
			{
				return string.Format("هم اکنون");
			}


			return string.Format($"{daysPast} روز پیش");

		}

		private static string GetPersianDayOfTheWeekName(DateTime time)
		{

			var dayOfWeek = (int)(PCalendar.GetDayOfWeek(time));

			return dayOfWeek switch
			{
				0 => "یک شنبه",
				1 => "دوشنبه",
				2 => "سه شنبه",
				3 => "چهارشنبه",
				4 => "پنج شنبه",
				5 => "جمعه",
				6 => "شنبه",
				_ => ""
			};
		}

		private static string GetPersianMonthOfYearName(DateTime time)
		{

			var monthOfYear = PCalendar.GetMonth(time);

			return monthOfYear switch
			{
				1 => "فرودرین",
				2 => "اردیبهشت",
				3 => "خرداد",
				4 => "تیر",
				5 => "مرداد",
				6 => "شهریور",
				7 => "مهر",
				8 => "آبان",
				9 => "آذر",
				10 => "دی",
				11 => "بهمن",
				12 => "اسفند",
				_ => "",
			};


		}

		#endregion

	}
}
