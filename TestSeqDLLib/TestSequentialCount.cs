using System;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestSequentialCount
	{
		[Test()]
		public void IsPadded ()
		{
			var url = "http://xkcd.com/614";
			var seqCo = new SequentialCount (new ComicUri (url));
			Assert.IsFalse (seqCo.Padded);
			seqCo.Padded = true;
			Assert.IsTrue (seqCo.Padded);
		}
		
		[Test()]
		public void GenerateSome ()
		{
			var xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/610";
			xkcdPages [1] = "http://xkcd.com/611";
			xkcdPages [2] = "http://xkcd.com/612";
			xkcdPages [3] = "http://xkcd.com/613";
			xkcdPages [4] = "http://xkcd.com/614";
			var comic = new ComicUri ("http://xkcd.com/614");
			var seqCount = new SequentialCount (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());				
		}
		
		[Test()]
		public void GenerateAll ()
		{
			
		}
	}
}

