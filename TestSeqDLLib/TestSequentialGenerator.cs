using System;
using System.Collections.Generic;
using System.Linq;
using ScraperLib;
using NUnit.Framework;

namespace TestScraperLibLib
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
			seqCo = new SequentialGenerator (url);
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
		public void GetDirectly ()
		{
			var url = "http://comic.com/009.png";
			var parser = new ComicParser (url);
			var img0 = "http://comic.com/613.png";
			var img1 = "http://comic.com/614.png";
			var img2 = "http://comic.com/615.png";
			
			// get the generator object
			UrlGenerator urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual ("1", urlGen.Start);
			
			// generate 10 urls, starting from 605
			List<string> backUrls = urlGen.Get (612, 10);
			// generate 10 urls, starting from 615
			List<string> forwardUrls = urlGen.Get (614, 10);
		
			// each url should be the comic, directly
			// urls are sorted
			Assert.AreEqual (10, backUrls.Count ());
			Assert.AreEqual (img0, backUrls [0]);
			Assert.AreEqual (img1, backUrls [1]);
			Assert.AreEqual (img2, backUrls [2]);
			Assert.AreEqual (10, forwardUrls.Count ());
			Assert.AreEqual (img2, forwardUrls [0]);
		}
		
		[Test()]
		public void GetFromPageIrregularWebcomic ()
		{
			var url = "http://www.irregularwebcomic.net/55.html";
			var parser = new ComicParser (url);
			var urlGen = parser.GetUrlGenerator ();
			var urls = urlGen.Get (0, 20);
			Assert.AreEqual("http://www.irregularwebcomic.net/comics/irreg0001.jpg", urls[0]);				
			Assert.AreEqual("http://www.irregularwebcomic.net/comics/irreg0002.jpg", urls[1]);				
			Assert.AreEqual("http://www.irregularwebcomic.net/comics/irreg0003.jpg", urls[2]);				
			Assert.AreEqual("http://www.irregularwebcomic.net/comics/irreg0004.jpg", urls[3]);				
			Assert.AreEqual("http://www.irregularwebcomic.net/comics/irreg0005.jpg", urls[4]);				
		}
		
		[Test()]
		public void GetFromPageXkcd ()
		{
			var xkcdUrl = "http://xkcd.com/614";
			var xkcdParser = new ComicParser (xkcdUrl);
			var xkcdImg = "http://imgs.xkcd.com/comics/woodpecker.png";
			var xkcdImg2 = "http://imgs.xkcd.com/comics/avoidance.png";
			
			// get the generator object
			UrlGenerator urlGen = xkcdParser.GetUrlGenerator ();
			
			Assert.AreEqual ("1", urlGen.Start);
			
			// generate 10 urls, starting from 605
			List<string> backUrls = urlGen.Get (605, 10);
			// generate 10 urls, starting from 615
			List<string> forwardUrls = urlGen.Get (615, 10);
			// generate 10 urls, starting from 614
			List<string> incUrls = urlGen.Get (614, 10);
			
			// each url should be the comic, directly
			// urls are sorted
			
//			foreach (var x in backUrls) {
//				Console.WriteLine (x);
//			}
			
			// Seems like this is a legitimate fail... 
			// backUrls[0] is actually comic 606, expected 605
			// further investigation required
			Assert.AreEqual (10, backUrls.Count ());
			Assert.AreEqual (xkcdImg, backUrls [9]);
			Assert.AreEqual (10, forwardUrls.Count ());
			Assert.AreEqual (xkcdImg2, forwardUrls [0]);
			Assert.AreEqual (10, incUrls.Count ());
			Assert.AreEqual (xkcdImg, incUrls [0]);
			Assert.AreEqual (xkcdImg2, incUrls [1]);		
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
			var comic = new ComicUri ("http://xkcd.com/608");
			var seqCount = new SequentialGenerator (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());				
			
			xkcdPages = new string[7];
			xkcdPages [0] = "http://xkcd.com/3";
			xkcdPages [1] = "http://xkcd.com/4";
			xkcdPages [2] = "http://xkcd.com/5";
			xkcdPages [3] = "http://xkcd.com/6";
			xkcdPages [4] = "http://xkcd.com/7";
			xkcdPages [5] = "http://xkcd.com/8";
			xkcdPages [6] = "http://xkcd.com/9";
			comic = new ComicUri ("http://xkcd.com/5");
			seqCount = new SequentialGenerator (comic);
			Assert.AreEqual (xkcdPages, seqCount.GenerateSome ());
		}
	}
}

