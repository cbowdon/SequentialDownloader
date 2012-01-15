using System;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	/// <summary>
	/// Sequential count : for basic basic counting.
	/// </summary>
	public class SequentialCount : UrlGenerator
	{
		ComicUri comic;
		
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
			if (ComicParser.UrlExists (string.Format (comic.Base, "1"))) {
				// unpadded
				return false;		
			} else if (ComicParser.UrlExists (string.Format (comic.Base, "1".PadLeft (comic.Indices [0].Length, '0')))) {
				// padded
				return true;
			} else {
				// throw error
				throw new ArgumentException ("SequentialCount.IsPadded: Cannot figure out if {0} is padded.", comic.AbsoluteUri);
			} 			
		}
		
		#endregion
		
		#region ZeroBased
		bool _zeroSet = false;
		bool zeroBased;

		public bool ZeroBased {
			get {
				if (_zeroSet == false) {
					_zeroSet = true;
					zeroBased = IsZeroBased ();					
				}
				return zeroBased;
			}
			
			set {
				_zeroSet = true;
				zeroBased = value;				
			}
		}
		
		bool IsZeroBased ()
		{
			string zeroNum = "0";
			if (Padded) {
				var len = comic.Indices [0].Length;
				zeroNum = "0".PadLeft (len, '0');
			} 
			return ComicParser.UrlExists (String.Format (comic.Base, zeroNum));
		}		
		#endregion
		
		public SequentialCount (ComicUri comic) : base (comic)
		{
			this.comic = comic;
			if (comic.Indices.Length != 1) {
				throw new ArgumentException ("Sequential Count cannot accept a comic with > 1 index");
			}
		}
		
		public List<string> Generate (IEnumerable<int> range)
		{				
			List<string> urls = new List<string> ();			
			IEnumerable<string> numbers;				
			
			if (Padded) {								
				var len = comic.Indices [0].Length;							
				numbers = range.Select<int,string> (x => x.ToString ().PadLeft (len, '0'));				
			} else {				
				numbers = range.Select<int,string> (x => x.ToString ());
			}
			
			urls = numbers.Select<string,string> (x => String.Format (comic.Base, x)).ToList ();

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
			var index = int.Parse (comic.Indices [0]);			
			IEnumerable<int> range;
			
			if (index < 7) {				
				if (ZeroBased) {
					range = Enumerable.Range (0, 7);	
				} else {
					range = Enumerable.Range (1, 7);	
				}				
			} else {
				range = Enumerable.Range (index - 6, 7);
			}
			
			return Generate (range);
		}
		
		public override List<string> GenerateLast100 ()
		{			
			IEnumerable<int> range;
			var hiNum = int.Parse (comic.Indices [0]);
			var loNum = hiNum - 99;
			if (loNum < 1) {
				if (ZeroBased) {				
					range = Enumerable.Range (0, hiNum + 1);	
				} else {
					range = Enumerable.Range (1, hiNum);	
				}		
			} else {
				range = Enumerable.Range (loNum, 100);
			}
			
			return Generate (range);
		}
		
		public override List<string> GenerateNext100 ()
		{
			var index = int.Parse (comic.Indices [0]);
			return Generate (Enumerable.Range (index + 1, 100));
		}
	}
}

