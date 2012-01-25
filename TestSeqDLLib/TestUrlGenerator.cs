using System;
using System.Collections.Generic;
using System.Linq;
using ScraperLib;
using NUnit.Framework;

namespace TestScraperLibLib
{
	[TestFixture()]
	public class TestUrlGenerator
	{
		[Test()]
		public void ImgIndexXkcd ()
		{
			var url = "http://xkcd.com/614";
			var urlGen = new SequentialGenerator (url);
			Assert.AreEqual (1, urlGen.ImgIndex);
		}
		
		[Test()]
		public void ImgIndexSmbc ()
		{
			string url = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var urlGen = new SequentialGenerator (url);
			Assert.AreEqual (2, urlGen.ImgIndex);			
		}
		
		[Test()]
		public void IdentifyImgXkcd ()
		{			
			var url = "http://xkcd.com/614";
			var xkcdRules = new SequentialGenerator (url);
			var actualUrl = "http://imgs.xkcd.com/comics/woodpecker.png";
			string result = null;
			Assert.AreEqual (1, UrlGenerator.IdentifyImg (xkcdRules.Generate (Enumerable.Range (614, 3)), out result));
			Assert.AreEqual (actualUrl, result);
		}
		
		[Test()]
		public void IdentifyImgSmbc ()
		{			
			string url = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcRules = new SequentialGenerator (url);
			var actualUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var actualUrl2 = "http://zs1.smbc-comics.com/comics/20061011.gif";
			string result = null;
			Assert.AreEqual (2, UrlGenerator.IdentifyImg (smbcRules.Generate (Enumerable.Range (614, 2)), out result));
			Assert.IsTrue (result.Equals (actualUrl) || result.Equals (actualUrl2));
		}
		
		[Test()]
		public void IdentifyImgIrregularWebcomic ()
		{
			var pageUrl = "http://www.irregularwebcomic.net/32.html";
			var fullComicUrl = "http://www.irregularwebcomic.net/comics/irreg0032.jpg";
			var seqGen = new SequentialGenerator (pageUrl);
			var somePageUrls = seqGen.GenerateSome ();
			foreach (var x in somePageUrls) {
				Console.WriteLine (x);
			}
			string imgUrl;
			var index = UrlGenerator.IdentifyImg (somePageUrls, out imgUrl);
			Assert.AreEqual (1, index);
			// depends on what GenerateSome provides
			Assert.AreEqual (fullComicUrl, imgUrl);
		}
	}
}

