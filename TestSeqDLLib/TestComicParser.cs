using System;
using System.Linq;
using System.Collections.Generic;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
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
			Assert.AreEqual (imgUrl.Substring(10), urlGen.Comic.AbsoluteUri.Substring(10));
						
			parser = new ComicParser (imgUrl);
			urlGen = parser.GetUrlGenerator ();
			Assert.AreEqual (imgUrl.Substring(10), urlGen.Comic.AbsoluteUri.Substring(10));
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
			var xkcdImg = "http://imgs.xkcd.com/comics/woodpecker.png";
			var xkcdImg2 = "http://imgs.xkcd.com/comics/avoidance.png";
			
			// get the generator object
			UrlGenerator urlGen = xkcdParser.GetUrlGenerator ();
			Assert.IsTrue (urlGen.ToString ().Contains ("SequentialGenerator"));
			
			Assert.AreEqual ("1", urlGen.Start);
			
			// generate 10 urls, starting from 605
			List<string> backUrls = urlGen.Get (605, 10);
			// generate 10 urls, starting from 615
			List<string> forwardUrls = urlGen.Get (615, 10);
			// generate 10 urls, starting from 614
			List<string> incUrls = urlGen.Get (614, 10);
			
			// each url should be the comic, directly
			// urls are sorted
			Assert.AreEqual (10, backUrls.Count ());
			Assert.AreEqual (xkcdImg, backUrls [9]);
			Assert.AreEqual (10, forwardUrls.Count ());
			Assert.AreEqual (xkcdImg2, forwardUrls [1]);
			Assert.AreEqual (10, incUrls.Count ());
			Assert.AreEqual (xkcdImg, incUrls [0]);
			Assert.AreEqual (xkcdImg2, incUrls [1]);		
		}
		
		[Test()]
		public void GetUrlGeneratorSmbc ()
		{
			var smbcUrl = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			//var smbcUrl0 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcUrl1 = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061012.gif";
			var smbcParser = new ComicParser (smbcUrl);
			
			// get the generator object
			UrlGenerator urlGen = smbcParser.GetUrlGenerator ();
			// generate 10 urls, going backwards, including the original (and starting from the original)
			List<string> backUrls = urlGen.Get (605, 10);
			// generate 10 urls, going forwards, excluding the original (and starting from the original)
			List<string> forwardUrls = urlGen.Get (615, 10);
			// generate 10 urls, going forwards, including the original (and starting from the original)
			List<string> incUrls = urlGen.Get (614, 10);
			
			// each url should be the comic, directly
			// urls are sorted
			Assert.AreEqual (smbcUrl1, backUrls [9]);
			Assert.AreEqual (10, backUrls.Count ());
			Assert.AreEqual (smbcUrl2, forwardUrls [1]);
			Assert.AreEqual (10, forwardUrls.Count ());
			Assert.AreEqual (smbcUrl1, incUrls [0]);
			Assert.AreEqual (10, incUrls.Count ());
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

