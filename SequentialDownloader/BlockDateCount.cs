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
		DateTime _date = new DateTime (0);
		
		public DateTime Date {
			get {
				if (_date == new DateTime (0)) {
					_date = FindFormat (comic.Indices, out _format);
				}
				return _date;
			}
		}

		string _format;
		
		public string Format { 
			get {
				if (_format == null) {
					_date = FindFormat (comic.Indices, out _format);
				}
				return _format;
			}
		}
		
		public string[] Days { get; set; }
		#endregion
		
		#region Constructors		
		public BlockDateCount (ComicUri comic) : base (comic)
		{
			this.comic = comic;			
		}
		#endregion
		
		#region Methods
		public List<string> Generate (int number)
		{
			List<string> days = new List<string> ();
			days.Add (DayOfWeek.Monday.ToString ());
			days.Add (DayOfWeek.Tuesday.ToString ());
			days.Add (DayOfWeek.Wednesday.ToString ());
			days.Add (DayOfWeek.Thursday.ToString ());
			days.Add (DayOfWeek.Friday.ToString ());
			days.Add (DayOfWeek.Saturday.ToString ());
			days.Add (DayOfWeek.Sunday.ToString ());
			
			return Generate (number, days);
		}
		
		public List<string> Generate (int number, IEnumerable<string> days)
		{
			var urls = new List<string> ();
			
			int i = 0;
			while (urls.Count < number) {
				var d = Date.AddDays (i);
				if (days.Contains (d.DayOfWeek.ToString ())) {			
					urls.Add (String.Format (comic.Base, d.ToString (Format)));
				}
				i--;
			}
			urls.Reverse ();
			return urls;			
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
		
		/// <summary>
		/// Generates URLs for previous 7 dates.
		/// </summary>
		/// <returns>
		/// URLs of previous 7 dates.
		/// </returns>
		public override List<string> GenerateSome ()
		{
			if (Days == null || Days.Length == 7) {
				return Generate (7);
			} else {
				return Generate (7, Days);
			}
		}
		
		public override List<string> GenerateAll ()
		{
			throw new NotImplementedException ();
		}		
		#endregion
	}	
}

