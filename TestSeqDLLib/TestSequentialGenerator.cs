using System;
using System.Collections.Generic;
using System.Linq;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestSequentialGenerator
	{
		
		[Test()]
		public void Start ()
		{
			var url = "http://xkcd.com/614";
			var seqCo = new SequentialGenerator (url);
			Assert.AreEqual ("1", seqCo.Start);
			Assert.IsFalse (seqCo.Padded);
			
			var newStart = "600";
			seqCo = new SequentialGenerator(url);
			seqCo.Start = newStart;
			Assert.AreEqual (newStart, seqCo.Start);
			Assert.IsFalse (seqCo.Padded);
			
			newStart = "006";
			seqCo.Start = newStart;
			Assert.AreEqual (newStart, seqCo.Start);
			Assert.IsTrue (seqCo.Padded);			
		}
		
		[Test()]
		public void Padded ()
		{
			// figure out the real answer
			var url = "http://xkcd.com/614";
			var seqCo = new SequentialGenerator (new ComicUri (url));
			Assert.IsFalse (seqCo.Padded);
			
			// we set it
			seqCo.Padded = true;
			Assert.IsTrue (seqCo.Padded);
			seqCo.Padded = false;
			Assert.IsFalse (seqCo.Padded);
			
			// assume answer from original URL
			url = "http://comic.com/01";
			seqCo = new SequentialGenerator (url);
			Assert.IsTrue (seqCo.Padded);
			
			// assume answer from original URL
			url = "http://comic.com/1";
			seqCo = new SequentialGenerator (url);
			Assert.IsFalse (seqCo.Padded);
		}
		
		[Test()]
		public void GenerateRange ()
		{
			var xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/610";
			xkcdPages [1] = "http://xkcd.com/611";
			xkcdPages [2] = "http://xkcd.com/612";
			xkcdPages [3] = "http://xkcd.com/613";
			xkcdPages [4] = "http://xkcd.com/614";
			var comic = new ComicUri ("http://xkcd.com/614");
			var xkcdRules = new SequentialGenerator (comic);
			Assert.AreEqual (xkcdPages, xkcdRules.Generate (Enumerable.Range (610, 5)));			
			
			xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/1";
			xkcdPages [1] = "http://xkcd.com/2";
			xkcdPages [2] = "http://xkcd.com/3";
			xkcdPages [3] = "http://xkcd.com/4";
			xkcdPages [4] = "http://xkcd.com/5";
			comic = new ComicUri ("http://xkcd.com/614");
			xkcdRules = new SequentialGenerator (comic);
			Assert.AreEqual (xkcdPages, xkcdRules.Generate (Enumerable.Range (1, 5)));			
			
			xkcdPages = new string[5];
			xkcdPages [0] = "http://xkcd.com/001";
			xkcdPages [1] = "http://xkcd.com/002";
			xkcdPages [2] = "http://xkcd.com/003";
			xkcdPages [3] = "http://xkcd.com/004";
			xkcdPages [4] = "http://xkcd.com/005";
			comic = new ComicUri ("http://xkcd.com/614");
			xkcdRules = new SequentialGenerator (comic);
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
			var seqCount = new SequentialGenerator (comic);
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
			seqCount = new SequentialGenerator (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());
		}
		
		[Test()]
		public void GenerateLast100 ()
		{
			var comic = new ComicUri ("http://xkcd.com/60");
			var seqCount = new SequentialGenerator (comic);
			seqCount.Padded = false;
			var all = seqCount.GenerateLast100 ();
			Assert.AreEqual (60, all.Count);
			Assert.AreEqual ("http://xkcd.com/1", all [0]);
			Assert.AreEqual ("http://xkcd.com/60", all [59]);
			
			comic = new ComicUri ("http://xkcd.com/1002");
			seqCount = new SequentialGenerator (comic);
			seqCount.Padded = false;
			var all3 = seqCount.GenerateLast100 ();
			Assert.AreEqual (100, all3.Count);
			Assert.AreEqual ("http://xkcd.com/1002", all3 [99]);			
		}
		
		[Test()]
		public void GenerateNext100 ()
		{
			var comic = new ComicUri ("http://xkcd.com/614");
			var seqCount = new SequentialGenerator (comic);
			seqCount.Padded = false;
			var all = seqCount.GenerateNext100 ();
			Assert.AreEqual (100, all.Count);
			Assert.AreEqual ("http://xkcd.com/615", all [0]);
			Assert.AreEqual ("http://xkcd.com/714", all [99]);
		}
	}
}

