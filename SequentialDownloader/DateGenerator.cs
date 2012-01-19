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
	public class DateGenerator : UrlGenerator
	{
		#region Start
		private string _start;
			
		public override string Start {
			get {
				if (_start == null) {
					var date = FindFormat (Comic.Indices, out _format);
					_start = date.ToString (Format);
				}
				return _start;
			}
			set {				
				string throwaway = "";				
				var tempDate = FindFormat (value, out throwaway);
				_start = tempDate.ToString (Format);
			}
		}
		#endregion
				
		#region Date		
		public DateTime Date {
			get {
				Console.WriteLine ("Called");
				return DateTime.ParseExact (Start, Format, null);
			}
		}
		#endregion
		
		#region Format
		private string _format;

		public string Format { 
			get {
				if (_format == null) {
					FindFormat (Comic.Indices, out _format);
				}
				return _format;
			}
		}
		#endregion
		
		#region Days
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
		
		static List<string> _everyDay = new List<string> ();

		public static List<string> EveryDay {
			get {
				if (_everyDay.Count == 0) {
					foreach (var x in Enum.GetValues(typeof(DayOfWeek))) {
						_everyDay.Add (x.ToString ());
					}
				}
				return _everyDay;
			}
		}
		#endregion
				
		#region Constructors		
		public DateGenerator (ComicUri comic) : base (comic)
		{
			this.Comic = comic;			
		}
		
		public DateGenerator (string url) : this (new ComicUri(url))
		{
		}
		#endregion
		
		#region Methods
		public static DateTime FindFormat (string date, out string format)
		{
			return FindFormat (new string[]{date}, out format);
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

		public List<string> Generate (int offset, int number, IEnumerable<string> days)
		{
			var urls = new List<string> ();
			int i = offset;
			while (urls.Count < number) {								
				var d = Date.AddDays (i);
				if (days.Contains (d.DayOfWeek.ToString ())) {			
					urls.Add (String.Format (Comic.Base, d.ToString (Format)));
				}
				i++;
			}
			return urls;			
		}
		
		public List<string> Generate (int startIndex, int number, int increment)
		{
			return Generate (startIndex, number, increment, EveryDay);
		}

		public List<string> Generate (int startIndex, int number, int increment, IEnumerable<string> days)
		{
			var urls = new List<string> ();
			
			int i = startIndex;
			while (urls.Count < number) {
				var d = Date.AddDays (i);
				if (days.Contains (d.DayOfWeek.ToString ())) {			
					urls.Add (String.Format (Comic.Base, d.ToString (Format)));
				}
				i += increment;
			}
			urls.Reverse ();
			return urls;			
		}
			
	
		/// <summary>
		/// Generates URLs for previous 7 dates.
		/// </summary>
		/// <returns>
		/// URLs of previous 7 dates.
		/// </returns>
		public override List<string> GenerateSome ()
		{
			return Generate (0, 7, Days);
		}
		
		public override List<string> Get (int startIndex, int num)
		{
			var urls = Generate (startIndex, num, Days);
			if (IsImageFile) {				
				// just count
				return urls;
			} else {
				// scan page for img tags, select the most likely
				var imgUrls = urls.Select<string, string> (x => WebUtils.GetImg (x, ImgIndex));
				return imgUrls.ToList ();
			}					

		}
		#endregion
	}	
}

