using System;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestComicParser
	{
		#region FindUrls
		[Test()]
		public void FindUrls ()
		{
			var xkcdUrl = "http://xkcd.com/614";
			var xkcdParser = new ComicParser (xkcdUrl);
			var xkcdUrls = xkcdParser.FindUrls ().ToArray ();
			var xkcdImg = "http://imgs.xkcd.com/comics/woodpecker.png";
			Assert.AreEqual (xkcdUrls [613], xkcdImg);						
		}
		#endregion
		
		#region ParameterizeUrls
		[Test()]
		public void ParameterizeUrlXkcd ()
		{			
			var xkcdUrl = "http://xkcd.com/614";
			var xkcdSplit = new ComicUri (xkcdUrl);
			var xkcdUrlBase = xkcdSplit.Base;
			string[] xkcdUrlIndices = xkcdSplit.Indices;			
			Assert.AreEqual ("http://xkcd.com/{0}", xkcdUrlBase);			
			Assert.AreEqual (1, xkcdUrlIndices.Length);
			Assert.AreEqual ("614", xkcdUrlIndices [0]);
		}
		
		[Test()]
		public void ParameterizeUrlSmbc ()
		{
			var smbcUrl = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			var smbcSplit = new ComicUri (smbcUrl);
			var smbcUrlBase = smbcSplit.Base;
			var smbcUrlIndices = smbcSplit.Indices;
			Assert.AreEqual ("http://www.smbc-comics.com/index.php?db=comics&id={0}", smbcUrlBase);			
			Assert.AreEqual (1, smbcUrlIndices.Length);
			Assert.AreEqual ("614", smbcUrlIndices [0]);				
			
			smbcUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			smbcSplit = (new ComicParser (smbcUrl)).ParameterizeUrl ();
			smbcUrlBase = smbcSplit.Base;
			smbcUrlIndices = smbcSplit.Indices;
			Assert.AreEqual ("http://www.smbc-comics.com/comics/{0}.gif", smbcUrlBase);			
			Assert.AreEqual (1, smbcUrlIndices.Length);
			Assert.AreEqual ("20061011", smbcUrlIndices [0]);				
		}
		
		[Test()]
		public void ParameterizeUrlPennyArcade ()
		{
			var paUrl = "http://penny-arcade.com/comic/2012/01/04";			
			var paSplit = new ComicUri (paUrl);
			var paUrlBase = paSplit.Base;
			var paUrlIndices = paSplit.Indices;
			Assert.AreEqual ("http://penny-arcade.com/comic/{0}/{1}/{2}", paUrlBase);
			Assert.AreEqual (3, paUrlIndices.Length);
			Assert.AreEqual ("2012", paUrlIndices [0]);
			Assert.AreEqual ("01", paUrlIndices [1]);
			Assert.AreEqual ("04", paUrlIndices [2]);
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
		#endregion
			
	}
}

