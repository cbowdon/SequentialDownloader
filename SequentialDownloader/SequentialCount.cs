using System;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	/// <summary>
	/// Sequential count : for basic basic counting.
	/// </summary>
	public class SequentialCount
	{
		ComicUri comic;
		
		public bool Padded { get; set; }
		
		public SequentialCount (ComicUri comic)
		{
			this.comic = comic;
			this.Padded = false;
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
		
		
	}
}

