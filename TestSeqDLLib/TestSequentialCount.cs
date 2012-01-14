using System;
using System.Collections.Generic;
using System.Linq;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestSequentialCount
	{
		[Test()]
		public void Padded ()
		{
			var url = "http://xkcd.com/614";
			var seqCo = new SequentialCount (new ComicUri (url));
			Assert.IsFalse (seqCo.Padded);
			seqCo.Padded = true;
			Assert.IsTrue (seqCo.Padded);
			seqCo.Padded = false;
			Assert.IsFalse (seqCo.Padded);
		}
		
		[Test()]
		public void ZeroBased ()
		{
			var url = "http://xkcd.com/614";
			var seqCo = new SequentialCount (new ComicUri (url));
			Assert.IsFalse (seqCo.ZeroBased);
			seqCo.ZeroBased = true;
			Assert.IsTrue (seqCo.ZeroBased);
			seqCo.ZeroBased = false;
			Assert.IsFalse (seqCo.ZeroBased);
		}
		
		[Test()]
		public void GenerateUrlsXkcd ()
		{
			var xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/610";
			xkcdPages [1] = "http://xkcd.com/611";
			xkcdPages [2] = "http://xkcd.com/612";
			xkcdPages [3] = "http://xkcd.com/613";
			xkcdPages [4] = "http://xkcd.com/614";
			var comic = new ComicUri ("http://xkcd.com/614");
			var xkcdRules = new SequentialCount (comic);
			Assert.AreEqual (xkcdPages, xkcdRules.Generate (Enumerable.Range (610,5)));			
			
			xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/1";
			xkcdPages [1] = "http://xkcd.com/2";
			xkcdPages [2] = "http://xkcd.com/3";
			xkcdPages [3] = "http://xkcd.com/4";
			xkcdPages [4] = "http://xkcd.com/5";
			comic = new ComicUri ("http://xkcd.com/614");
			xkcdRules = new SequentialCount (comic);
			Assert.AreEqual (xkcdPages, xkcdRules.Generate (Enumerable.Range (1, 5)));			
			
			xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/001";
			xkcdPages [1] = "http://xkcd.com/002";
			xkcdPages [2] = "http://xkcd.com/003";
			xkcdPages [3] = "http://xkcd.com/004";
			xkcdPages [4] = "http://xkcd.com/005";
			comic = new ComicUri ("http://xkcd.com/614");
			xkcdRules = new SequentialCount (comic);
			xkcdRules.Padded = true;
			Assert.AreEqual (xkcdPages, xkcdRules.Generate (Enumerable.Range (1, 5)));			
		}
		
		[Test()]
		public void GenerateSome ()
		{			
			var xkcdPages = new string[7];
			xkcdPages [0] = "http://xkcd.com/608";
			xkcdPages [1] = "http://xkcd.com/609";
			xkcdPages [2] = "http://xkcd.com/610";
			xkcdPages [3] = "http://xkcd.com/611";
			xkcdPages [4] = "http://xkcd.com/612";
			xkcdPages [5] = "http://xkcd.com/613";
			xkcdPages [6] = "http://xkcd.com/614";
			var comic = new ComicUri ("http://xkcd.com/614");
			var seqCount = new SequentialCount (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());				
			
			xkcdPages = new string[7];
			xkcdPages [0] = "http://xkcd.com/1";
			xkcdPages [1] = "http://xkcd.com/2";
			xkcdPages [2] = "http://xkcd.com/3";
			xkcdPages [3] = "http://xkcd.com/4";
			xkcdPages [4] = "http://xkcd.com/5";
			xkcdPages [5] = "http://xkcd.com/6";
			xkcdPages [6] = "http://xkcd.com/7";
			comic = new ComicUri ("http://xkcd.com/3");
			seqCount = new SequentialCount (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());
		}
		
		[Test()]
		public void GenerateAll ()
		{
			var comic = new ComicUri ("http://xkcd.com/614");
			var seqCount = new SequentialCount (comic);
			seqCount.ZeroBased = false;
			seqCount.Padded = false;
			var all = seqCount.GenerateAll ();
			Assert.AreEqual (614, all.Count);
			Assert.AreEqual ("http://xkcd.com/235", all [234]);
			
			seqCount.ZeroBased = true;
			var all2 = seqCount.GenerateAll ();			
			Assert.AreEqual (615, all2.Count);
			Assert.AreEqual ("http://xkcd.com/234", all2 [234]);
		}
	}
}

