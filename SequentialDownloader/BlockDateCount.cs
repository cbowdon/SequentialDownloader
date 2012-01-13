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
	public class BlockDateCount : UrlGenerator
	{
		#region Properties
		ComicUri comic;
		string _format;
		DateTime _date = new DateTime (0);
		
		public DateTime Date {
			get {
				if (_date == new DateTime (0)) {
					_date = FindFormat (comic.Indices, out _format);
				}
				return _date;
			}
		}

		public string Format { 
			get {
				if (_format == null) {
					_date = FindFormat (comic.Indices, out _format);
				}
				return _format;
			}
		}
		#endregion
		
		#region Constructors		
		public BlockDateCount (ComicUri comic) : base (comic)
		{
			this.comic = comic;			
		}
		#endregion
		
		#region Methods
		public string[] Generate (int number)
		{
			string[] days = new string[7];
			days [0] = DayOfWeek.Monday.ToString ();
			days [1] = DayOfWeek.Tuesday.ToString ();
			days [2] = DayOfWeek.Wednesday.ToString ();
			days [3] = DayOfWeek.Thursday.ToString ();
			days [4] = DayOfWeek.Friday.ToString ();
			days [5] = DayOfWeek.Saturday.ToString ();
			days [6] = DayOfWeek.Sunday.ToString ();
			return Generate (number, days);
		}
		
		public string[] Generate (int number, string[] days)
		{
			var urls = new List<string> ();
			
			int i = 0;
			while (urls.Count < number) {
				var d = Date.AddDays (i);
				if (days.Contains (d.DayOfWeek.ToString ())) {			
					urls.Add (String.Format (comic.Base, d.ToString (Format)));
				}
				i++;
			}
			return urls.ToArray ();			
		}
			
		public static DateTime FindFormat (string[] indices, out string format)
		{
			if (indices.Length != 1) {
				throw new ArgumentException ("Only working with block dates right now!");
			}
			string date = indices [0];
			
			// outputs to be assigned
			DateTime res;
			string form = DateType.NotRecognized;
			
			// shorthand			
			Func <string, bool> TryParseFormat = x => DateTime.TryParseExact (date, x, null, System.Globalization.DateTimeStyles.None, out res);	
			
			// iterate through DateType collection, testing TryParseExact		
			var dType = new DateType ();						
			for (int i = 0; i < dType.Length; i++) {				
				if (TryParseFormat (dType [i])) {
					form = dType [i];
					break;
				}	
			}			
			
			// assign and return
			format = form;
			return res;
		}
		
		public override List<string> GenerateAll ()
		{
			throw new NotImplementedException ();
		}
		
		public override List<string> GenerateSome ()
		{
			throw new NotImplementedException ();
		}
		
		#endregion
	}	
}

