using System;
using System.Collections.Generic;

namespace SequentialDownloader
{
	/// <summary>
	/// Abstract base class for SequentialCount and DateCount.
	/// Since SeqCount and DateCount are solutions to the same problem, a factory pattern
	/// seemed natural. Unfortunately they only share one signature so far: the constructor.
	/// </summary>
	public abstract class UrlGenerator
	{
		public abstract string Start { get; set; }
		
		protected UrlGenerator (ComicUri comic){}
		
		public abstract List<string> GenerateLast100 ();
		
		public abstract List<string> GenerateNext100 ();
		
		public abstract List<string> GenerateSome ();
		
		public abstract List<string> Get (int startIndex, int num);
	}
}

