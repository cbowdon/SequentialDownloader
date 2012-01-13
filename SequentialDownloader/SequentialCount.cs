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
		
		public SequentialCount (ComicUri comic) : base (comic)
		{
			this.comic = comic;
			if (comic.Indices.Length != 1) {
				throw new ArgumentException ("Sequential Count cannot accept a comic with > 1 index");
			}
		}
		
		public string[] Generate (Range range)
		{
			var urls = new List<string> ();
			for (int i = range.Start; i < range.End; i+= range.Increment) {
				string num = i.ToString ();
				if (Padded) {
					var index = comic.Indices [0];
					num = num.PadLeft (index.Length, '0');
				}
				urls.Add (string.Format (comic.Base, num));
			}
			return urls.ToArray ();
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
		
		public override List<string> GenerateAll ()
		{
			throw new NotImplementedException ();
		}
		
		public override List<string> GenerateSome ()
		{
			throw new NotImplementedException ();
		}
		
		
	}
}

