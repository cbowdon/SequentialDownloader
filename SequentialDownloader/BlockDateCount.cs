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
	public class BlockDateCount : ICountingRule
	{
		ComicUri comic;
		string _format;
		BlockDateType _type;
		DateTime _date;
				
		public string Format { 
			get {
				if (_format == null) {
					_date = FindFormat (comic.Indices, out _format, out _type);
				}
				return _format;
			}			
		}

		public BlockDateType Type { 
			get {
				if (_type == BlockDateType.NotAssigned) {
					_date = FindFormat (comic.Indices, out _format, out _type);
				}
				return _type;
			}
		}
		
		string startDate;
		int num;
		DayOfWeek[] days;
		
		public BlockDateCount (ComicUri comic)
		{
			this.comic = comic;			
		}
		
		public BlockDateCount (ComicUri comic, string startDate, int number)
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
		
		public BlockDateCount (ComicUri comic, string startDate, int number, DayOfWeek[] days)
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
				if (days.Contains (d.DayOfWeek)) {			
					urls.Add (String.Format (comic.Base, d.ToString ("yyyyMMdd")));
				}
				i++;
			}
			return urls.ToArray ();			
		}
		
		public static DateTime GetDateTime (string date)
		{		
			DateTime result;
			string form;
			var x = FindFormat (date, out result, out form);
			if (x == BlockDateType.NotRecognised) {
				throw new ArgumentException ("ISODateCount.GetDateTime: Not a valid date string: {0} -> format is yyyyMMdd", date);
			}
			return result;			
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
		
		public static BlockDateType FindFormat (string date, out DateTime result, out string format)
		{
			// TryParseExact each date format string
			// if true, return
			DateTime res;
			Func <string, bool> TryParseFormat = x => DateTime.TryParseExact (date, x, null, System.Globalization.DateTimeStyles.None, out res);	
						
			string form = "";
			BlockDateType bdFormat = BlockDateType.NotRecognised;
			
			if (TryParseFormat ("yyyyMMdd")) {
				form = "yyyyMMdd";
				bdFormat = BlockDateType.ISO;
			} else if (TryParseFormat ("ddMMyyyy")) {
				form = "ddMMyyyy";
				bdFormat = BlockDateType.UK;
			} else if (TryParseFormat ("MMddyyyy")) {
				form = "MMddyyyy";
				bdFormat = BlockDateType.US;
			} else if (TryParseFormat ("yyMMdd")) {
				form = "yyMMdd";
				bdFormat = BlockDateType.ShortISO;
			} else if (TryParseFormat ("ddMMyy")) {
				form = "ddMMyy";
				bdFormat = BlockDateType.ShortUK;
			} else if (TryParseFormat ("MMddyy")) {
				form = "MMddyy";
				bdFormat = BlockDateType.ShortUS;
			} 
			format = form;
			result = res;
			return bdFormat;
		}
		
		DateTime FindFormat (string[] indices, out string format, out BlockDateType type)
		{
			string date = indices [0];
			DateTime res;
			Func <string, bool> TryParseFormat = x => DateTime.TryParseExact (date, x, null, System.Globalization.DateTimeStyles.None, out res);	
						
			string form = "";
			BlockDateType bdFormat = BlockDateType.NotRecognised;
			
			string[] formats = new string[6];
			formats [0] = "yyyyMMdd";
			formats [1] = "ddMMyyyy";
			formats [2] = "MMddyyyy";
			formats [3] = "yyMMdd";
			formats [4] = "ddMMyy";
			formats [5] = "MMddyy";
			
			foreach (int i in BlockDateType) {
				if (TryParseFormat (formats[(int)i])) {
					form = formats[(int)i];
					bdFormat = i;
				}	
			}
			
			format = form;
			return res;
		}
	}	
}

