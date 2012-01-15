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
		protected UrlGenerator (ComicUri comic)
		{
		}
		
		public abstract List<string> GenerateLast1000 ();
		
		public abstract List<string> GenerateNext1000 ();
		
		public abstract List<string> GenerateSome ();
	}
}

