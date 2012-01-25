using System;
using System.Collections.Generic;
using System.Linq;
using ScraperLib;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestWebUtils
	{
		[Test()]
		public void GetImgsXkcd ()
		{
			string xkcd614 = "http://xkcd.com/614";			
			var xkcd614Imgs = WebUtils.GetImgs (xkcd614);
			Assert.AreEqual (4, xkcd614Imgs.Count);
			
			string secondTag = "http://imgs.xkcd.com/comics/woodpecker.png";
			Assert.AreEqual (secondTag, xkcd614Imgs [1]);
			string fourthTag = "http://imgs.xkcd.com/static/somerights20.png";
			Assert.AreEqual (fourthTag, xkcd614Imgs [3]);
			
			string xkcd615 = "http://xkcd.com/615";						
			var xkcd615Imgs = WebUtils.GetImgs (xkcd615);			
			Assert.AreEqual (xkcd614Imgs.Count, xkcd615Imgs.Count);
		}
		
		[Test()]
		public void GetImgsSmbc ()
		{			
			string smbc614 = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbc614Imgs = WebUtils.GetImgs (smbc614);
			Assert.AreEqual (6, smbc614Imgs.Count);
			
			var smbc614ComicA = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbc614ComicB = "http://zs1.smbc-comics.com/comics/20061011.gif";
			Assert.IsTrue (smbc614Imgs.Contains (smbc614ComicA) || smbc614Imgs.Contains (smbc614ComicB));
			
			string smbc615 = "http://www.smbc-comics.com/index.php?db=comics&id=615";
			var smbc615Imgs = WebUtils.GetImgs (smbc615);
			Assert.AreEqual (smbc614Imgs.Count, smbc614Imgs.Count);
			
			var smbc615ComicA = "http://www.smbc-comics.com/comics/20061012.gif";
			var smbc615ComicB = "http://zs1.smbc-comics.com/comics/20061012.gif";
			Assert.IsTrue (smbc615Imgs.Contains (smbc615ComicA) || smbc615Imgs.Contains (smbc615ComicB));
		}
		
		[Test()]
		public void GetImgsIrregularWebComic ()
		{
			var pageUrl = "http://www.irregularwebcomic.net/32.html";
			var fullComicUrl = "http://www.irregularwebcomic.net/comics/irreg0032.jpg";
			var imgs = WebUtils.GetImgs (pageUrl);
			Assert.AreEqual (3, imgs.Count ());
			Assert.IsTrue (imgs.Contains (fullComicUrl));
			Assert.AreEqual (fullComicUrl, imgs [1]);
		}
		
		[Test()]
		public void UrlExists ()
		{
			Assert.IsTrue (WebUtils.UrlExists ("http://xkcd.com"));
			Assert.IsFalse (WebUtils.UrlExists ("http://xkcd.com/91235252624363"));
			Assert.IsTrue (WebUtils.UrlExists ("http://imgs.xkcd.com/comics/woodpecker.png"));
			Assert.IsTrue (WebUtils.UrlExists ("http://www.smbc-comics.com/index.php?db=comics&id=614"));
		}
	}
}

