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
			Assert.AreEqual (xkcdImg, xkcdUrls [613]);						
		}

		[Test()]
		public void FindUrlsSmbc ()
		{
			var smbcUrl = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcUrl1 = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcParser = new ComicParser (smbcUrl);
			var smbcUrls = smbcParser.FindUrls ().ToArray ();
			Assert.AreEqual (100, smbcUrls.Count ());
			Assert.AreEqual (smbcUrl1.Substring(10), smbcUrls [99].Substring (10));
			Assert.AreEqual (smbcUrl2.Substring(10), smbcUrls [98].Substring (10));
		}
		
		[Test()]
		public void FindUrlsSmbcImg ()
		{
			var smbcUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcUrl2 = "http://www.smbc-comics.com/comics/20061010.gif";
			var smbcParser = new ComicParser (smbcUrl);
			var smbcUrls = smbcParser.FindUrls ().ToArray ();
			
			Assert.AreEqual (smbcUrl.Substring (10), smbcUrls [99].Substring (10));
			Assert.AreEqual (smbcUrl2.Substring (10), smbcUrls [98].Substring (10));
		}
		
		[Test()]
		public void UrlExists ()
		{
			Assert.IsTrue (ComicParser.UrlExists ("http://xkcd.com"));
			Assert.IsFalse (ComicParser.UrlExists ("http://xkcd.com/91235252624363"));
			Assert.IsTrue (ComicParser.UrlExists ("http://imgs.xkcd.com/comics/woodpecker.png"));
			Assert.IsTrue (ComicParser.UrlExists ("http://www.smbc-comics.com/index.php?db=comics&id=614"));
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
			var xkcdRules = new SequentialCount (new ComicUri (url));
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
			var smbcRules = new SequentialCount (new ComicUri (url));
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

