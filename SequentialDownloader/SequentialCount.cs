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
		Range range;
		
		public SequentialCount (ComicUri comic, Range range)
		{
			this.comic = comic;
			this.range = range;
		}
		
		public string[] Generate ()
		{
			var urls = new List<string> ();
			for (int i = range.Start; i < range.End; i+= range.Increment) {					
				urls.Add (string.Format (comic.Base, i));
			}
			return urls.ToArray ();
		}
		
		
	}
}

