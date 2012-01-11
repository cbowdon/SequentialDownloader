using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	/// <summary>
	/// ISO date count: for counting operations involving dates like 19871024.
	/// </summary>
	/// <exception cref='NotImplementedException'>
	/// Is thrown when a requested operation is not implemented for a given type.
	/// </exception>
	public class ISODateCount : ICountingRule
	{
		ComicUri comic;
		string startDate;
		int num;
		DayOfWeek[] days;
		
		public ISODateCount (ComicUri comic, string startDate, int number)
		{
			this.comic = comic;	
			this.startDate = startDate;
			this.num = number;
			this.days = new DayOfWeek[7];
			this.days [0] = DayOfWeek.Monday;
			this.days [1] = DayOfWeek.Tuesday;
			this.days [2] = DayOfWeek.Wednesday;
			this.days [3] = DayOfWeek.Thursday;
			this.days [4] = DayOfWeek.Friday;
			this.days [5] = DayOfWeek.Saturday;			
			this.days [6] = DayOfWeek.Sunday;
		}
		
		public ISODateCount (ComicUri comic, string startDate, int number, DayOfWeek[] days)
		{
			this.comic = comic;	
			this.startDate = startDate;
			this.num = number;
			this.days = days;
		}
		
		public string[] Generate ()
		{
			var date = GetDateTime (startDate);			
			var urls = new List<string> ();
			
			int i = 0;
			while (urls.Count < num) {
				var d = date.AddDays (i);
				if (days.Contains(d.DayOfWeek))	{			
					urls.Add (String.Format (comic.Base, d.ToString ("yyyyMMdd")));
				}
				i++;
			}
			return urls.ToArray ();			
		}
		
		public static DateTime GetDateTime (string date)
		{
			if (date.Length != 8) {
				throw new ArgumentException ("ISODateCount.GetDateTime: Not a valid date string: {0} -> format is yyyyMMdd", date);
			}						
			return DateTime.ParseExact (date, "yyyyMMdd", null);			
		}
		
		public static bool IsDateTime (string date)
		{
			try {
				GetDateTime (date);
				return true;
			} catch {
				return false;
			}
		}
	}	
}

