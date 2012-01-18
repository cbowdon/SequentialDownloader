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
		#region FindUrls
		[Test()]
		public void FindUrlsXkcd ()
		{
			var xkcdUrl = "http://xkcd.com/614";
			var xkcdParser = new ComicParser (xkcdUrl);
			var xkcdUrls = xkcdParser.FindUrls ().ToArray ();
			var xkcdImg = "http://imgs.xkcd.com/comics/woodpecker.png";
			Assert.AreEqual (xkcdImg, xkcdUrls [98]);			
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
			Assert.IsInstanceOfType(typeof(SequentialGenerator), urlGen.GetType(), "Correct type");
			
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
		public void FindUrlsSmbc ()
		{
			var smbcUrl = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcUrl1 = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcParser = new ComicParser (smbcUrl);
			var smbcUrls = smbcParser.FindUrls ();
			Assert.AreEqual (100, smbcUrls.Count ());
			Assert.AreEqual (smbcUrl1.Substring (10), smbcUrls [99].Substring (10));
			Assert.AreEqual (smbcUrl2.Substring (10), smbcUrls [98].Substring (10));
		}
		
		[Test()]
		public void FindUrlsSmbcImg ()
		{
			var smbcUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcParser = new ComicParser (smbcUrl);
			var smbcUrls = smbcParser.FindUrls ();
			
			Assert.AreEqual (smbcUrl.Substring (10), smbcUrls [99].Substring (10));
			Assert.AreEqual (smbcUrl2.Substring (10), smbcUrls [98].Substring (10));
		}
		
		[Test()]
		public void UrlExists ()
		{
			Assert.IsTrue (WebUtils.UrlExists ("http://xkcd.com"));
			Assert.IsFalse (WebUtils.UrlExists ("http://xkcd.com/91235252624363"));
			Assert.IsTrue (WebUtils.UrlExists ("http://imgs.xkcd.com/comics/woodpecker.png"));
			Assert.IsTrue (WebUtils.UrlExists ("http://www.smbc-comics.com/index.php?db=comics&id=614"));
		}
		#endregion
			
		#region FindImgs
		[Test()]
		public void FindImgsXkcd ()
		{
			string xkcd614 = "http://xkcd.com/614";			
			var xkcd614Imgs = (new ComicParser (xkcd614)).FindImgs ();
			Assert.AreEqual (4, xkcd614Imgs.Count);
			
			string secondTag = "http://imgs.xkcd.com/comics/woodpecker.png";
			Assert.AreEqual (secondTag, xkcd614Imgs [1]);
			string fourthTag = "http://imgs.xkcd.com/static/somerights20.png";
			Assert.AreEqual (fourthTag, xkcd614Imgs [3]);
			
			string xkcd615 = "http://xkcd.com/615";						
			var xkcd615Imgs = (new ComicParser (xkcd615)).FindImgs ();			
			Assert.AreEqual (xkcd614Imgs.Count, xkcd615Imgs.Count);
		}
		
		[Test()]
		public void FindImgsSmbc ()
		{			
			string smbc614 = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbc614Imgs = (new ComicParser (smbc614)).FindImgs ();			
			Assert.AreEqual (5, smbc614Imgs.Count);
			
			var smbc614ComicA = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbc614ComicB = "http://zs1.smbc-comics.com/comics/20061011.gif";
			Assert.IsTrue (smbc614Imgs.Contains (smbc614ComicA) || smbc614Imgs.Contains (smbc614ComicB));
			
			string smbc615 = "http://www.smbc-comics.com/index.php?db=comics&id=615";
			var smbc615Imgs = (new ComicParser (smbc615)).FindImgs ();
			Assert.AreEqual (smbc614Imgs.Count, smbc614Imgs.Count);
			
			var smbc615ComicA = "http://www.smbc-comics.com/comics/20061012.gif";
			var smbc615ComicB = "http://zs1.smbc-comics.com/comics/20061012.gif";
			Assert.IsTrue (smbc615Imgs.Contains (smbc615ComicA) || smbc615Imgs.Contains (smbc615ComicB));
		}
		
		[Test()]
		public void IdentifyImgXkcd ()
		{			
			var url = "http://xkcd.com/614";
			var xkcdRules = new SequentialGenerator (new ComicUri (url));
			var comic = new ComicParser (url);
			var actualUrl = "http://imgs.xkcd.com/comics/woodpecker.png";
			string result = null;
			Assert.AreEqual (1, comic.IdentifyImg (xkcdRules.Generate (Enumerable.Range (614, 3)), out result));
			Assert.AreEqual (actualUrl, result);
		}
		
		[Test()]
		public void IdentifyImgSmbc ()
		{			
			string url = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcRules = new SequentialGenerator (new ComicUri (url));
			var comic = new ComicParser (url);
			var actualUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var actualUrl2 = "http://zs1.smbc-comics.com/comics/20061011.gif";
			string result = null;
			Assert.AreEqual (2, comic.IdentifyImg (smbcRules.Generate (Enumerable.Range (614, 2)), out result));
			Assert.IsTrue (result.Equals (actualUrl) || result.Equals (actualUrl2));
		}
		#endregion
			
	}
}

