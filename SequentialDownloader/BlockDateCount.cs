using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	/// <summary>
	/// Counter for dates expressed as single block number, e.g. "241087" or "19871024".
	/// </summary>
	/// <exception cref='ArgumentException'>
	/// Is thrown when an argument passed to a method is invalid.
	/// </exception>
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
		
		List<string> _days = null;

		public List<string> Days { 
			get {
				if (_days == null) {
					_days = new List<string> ();
					var urls = Generate (14, 0, -1);
					var hits = urls.Select<string,bool> (x => WebUtils.UrlExists (x)).ToList ();
					for (int i = 0; i < urls.Count(); i++) {
						if (hits [i]) {
							var d = Date.AddDays (1 - i).DayOfWeek.ToString ();							
							if (!_days.Contains (d)) {
								_days.Add (d);
							}								
						} 
					}
				}
				return _days;
			} 
			set {
				_days = value;
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
		public List<string> Generate (int number, int offset, int increment)
		{
			List<string> days = new List<string> ();
			days.Add (DayOfWeek.Monday.ToString ());
			days.Add (DayOfWeek.Tuesday.ToString ());
			days.Add (DayOfWeek.Wednesday.ToString ());
			days.Add (DayOfWeek.Thursday.ToString ());
			days.Add (DayOfWeek.Friday.ToString ());
			days.Add (DayOfWeek.Saturday.ToString ());
			days.Add (DayOfWeek.Sunday.ToString ());
			
			return Generate (number, days, offset, increment);
		}
		
		public List<string> Generate (int number, IEnumerable<string> days, int offset, int increment)
		{
			var urls = new List<string> ();
			
			int i = offset;
			while (urls.Count < number) {
				var d = Date.AddDays (i);
				if (days.Contains (d.DayOfWeek.ToString ())) {			
					urls.Add (String.Format (comic.Base, d.ToString (Format)));
				}
				i += increment;
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
			if (Days == null || Days.Count () == 7) {
				return Generate (7, 0, -1);
			} else {
				return Generate (7, Days, 0, -1);
			}
		}
		
		public override List<string> GenerateLast100 ()
		{
			return Generate (100, Days, 0, -1);
		}
		
		public override List<string> GenerateNext100 ()
		{
			return Generate (100, Days, 1, 1);
		}
		#endregion
	}	
}

