using System;
using System.Linq;
using System.Collections.Generic;
using ScraperLib;
using NUnit.Framework;

namespace TestScraperLibLib
{
	[TestFixture()]
	public class TestComicParser
	{
		[Test()]
		public void TryRedirectToImgSmbc ()
		{
			var url = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var imgUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var parser = new ComicParser (url);
			var urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (imgUrl.Substring (10), urlGen.Comic.AbsoluteUri.Substring (10));
						
			parser = new ComicParser (imgUrl);
			urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (imgUrl.Substring (10), urlGen.Comic.AbsoluteUri.Substring (10));
		}
		
		[Test()]
		public void TryRedirectToImgIrregularWebcomic ()
		{
			var url = "http://www.irregularwebcomic.net/32.html";
			var imgUrl = "http://www.irregularwebcomic.net/comics/irreg0032.jpg";
			var parser = new ComicParser (url);
			var urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (imgUrl, urlGen.Comic.AbsoluteUri);
						
			parser = new ComicParser (imgUrl);
			urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (imgUrl, urlGen.Comic.AbsoluteUri);
		}

		[Test()]
		public void GetUrlGeneratorXkcd ()
		{
			var xkcdUrl = "http://xkcd.com/614";
			var xkcdParser = new ComicParser (xkcdUrl);
			
			// get the generator object - should be the right type
			UrlGenerator urlGen = xkcdParser.GetUrlGenerator ();
			Assert.IsTrue (urlGen.ToString ().Contains ("SequentialGenerator"));		
		}
		
		[Test()]
		public void GetUrlGeneratorSmbc ()
		{
			var smbcUrl = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcUrl0 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcUrl1 = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061012.gif";
			var smbcParser = new ComicParser (smbcUrl);
			
			// get the generator object
			DateGenerator urlGen = (DateGenerator)smbcParser.GetUrlGenerator ();
			urlGen.Days = DateGenerator.EveryDay;
			
			// generate 10 urls, going backwards, including the original (and starting from the original)
			List<string> backUrls = urlGen.Get (-1, 10);
			// generate 10 urls, going forwards, excluding the original (and starting from the original)
			List<string> forwardUrls = urlGen.Get (0, 10);
			
			// each url should be the comic, directly
			// urls are sorted
			Assert.AreEqual (smbcUrl0, backUrls [0]);
			Assert.AreEqual (smbcUrl1, backUrls [1]);
			Assert.AreEqual (10, backUrls.Count ());
			Assert.AreEqual (smbcUrl1, forwardUrls [0]);
			Assert.AreEqual (smbcUrl2, forwardUrls [1]);
			Assert.AreEqual (10, forwardUrls.Count ());
		}
		
		[Test()]
		public void GetUrlGeneratorIrregularWebcomic ()
		{
			var pageUrl = "http://www.irregularwebcomic.net/32.html";
			var comicUrl = "http://www.irregularwebcomic.net/comics/irreg0032.jpg";
			var parser = new ComicParser (pageUrl);
			var urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (comicUrl, urlGen.Comic.AbsoluteUri);
		}
	}
}

