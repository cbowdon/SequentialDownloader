using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageScraperLib
{
	/// <summary>
	/// Sequential count : for basic basic counting.
	/// </summary>
	public class SequentialGenerator : UrlGenerator
	{		
		#region Start
		private string _start = "1";

		public override string Start {
			get {
				return _start;
			} 
			set {
				int num;
				var ok = int.TryParse (value, out num);
				if (!ok || num < 0) {
					throw new ArgumentException ("Invalid Start!");
				}
				// when setting start value, make some guesses about padding
				if (value.Substring (0, 1) == "0" && value.Length > 1) {
					Padded = true;
				} else if (value.Length == 1) {
					Padded = false;
				}
				_start = value;
			}
		}
		#endregion
				
		#region Padded
		bool _paddingSet = false;
		bool padded;

		public bool Padded { 
			get {
				if (_paddingSet == false) {
					_paddingSet = true;
					padded = IsPadded ();					
				} 
				return padded;
			}
			
			set {
				_paddingSet = true;
				padded = value;
			}
		}

		bool IsPadded ()
		{
			// identify if it is a fixed-length number	
			if (WebUtils.UrlExists (string.Format (Comic.Base, "1"))) {
				// unpadded
				return false;		
			} else if (WebUtils.UrlExists (string.Format (Comic.Base, "1".PadLeft (Comic.Indices [0].Length, '0')))) {
				// padded
				return true;
			} else {
				// throw error
				throw new ArgumentException (String.Format ("SequentialCount.IsPadded: Cannot figure out if {0} is padded.", Comic.AbsoluteUri));
			} 			
		}
		
		#endregion
		
		#region Constructors
		public SequentialGenerator (ComicUri comic) : base (comic)
		{
			this.Comic = comic;
			if (comic.Indices.Length != 1) {
				throw new ArgumentException ("Sequential Count cannot accept a comic with > 1 index");
			}
						
			if (comic.Indices [0].Substring (0, 1) == "0" && comic.Indices [0].Length > 1) {
				Padded = true;
			}
			
			if (comic.Indices [0].Length == 1) {
				Padded = false;
			}
		}
		
		public SequentialGenerator (string url) : this (new ComicUri(url))
		{
		}		
		#endregion
		
		public List<string> Generate (IEnumerable<int> range)
		{				
			List<string> urls = new List<string> ();			
			IEnumerable<string> numbers;				
			
			if (Padded) {								
				var len = Comic.Indices [0].Length;							
				numbers = range.Select<int,string> (x => x.ToString ().PadLeft (len, '0'));				
			} else {				
				numbers = range.Select<int,string> (x => x.ToString ());
			}
			
			urls = numbers.Select<string,string> (x => String.Format (Comic.Base, x)).ToList ();

			return urls;

		}
		
		/// <summary>
		/// Generates 7 (predicted) previous comic URLs.
		/// </summary>
		/// <returns>
		/// The 7 comic URLs prior to the given one, or 7 starting from 1 or 0 if given < 7.
		/// </returns>
		public override List<string> GenerateSome ()
		{
			var index = int.Parse (Comic.Indices [0]);			
			IEnumerable<int> range;
			
			if (index < 7) {				
				range = Enumerable.Range (int.Parse (Start), 7);	
			} else {
				range = Enumerable.Range (index, 7);
			}
			
			return Generate (range);
		}
		
		/// <summary>
		/// Get the comics at the specified startIndex(+Start) and num.
		/// </summary>
		/// <param name='startIndex'>
		/// Start index.
		/// </param>
		/// <param name='num'>
		/// Number.
		/// </param>
		public override List<string> Get (int startIndex, int num)
		{
			var urls = Generate (Enumerable.Range (startIndex + int.Parse (Start), num));
			if (IsImageFile) {				
				// just count
				return urls;
			} else {
				// scan page for img tags, select the most likely
				var imgUrls = urls.Select<string, string> (x => WebUtils.GetImg (x, ImgIndex));
				return imgUrls.ToList ();
			}					
		}
	}
}

