using System;
using ScraperLib;
using NUnit.Framework;

namespace TestScraperLibLib
{
	[TestFixture()]
	public class TestComicUri
	{
		[Test()]
		public void IsRemoteFile ()
		{
			var smbc614 = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			Assert.IsFalse ((new ComicUri (smbc614)).IsRemoteFile, "smbc comic page");
			
			var smbc614Comic = "http://www.smbc-comics.com/comics/20061011.gif";
			Assert.IsTrue ((new ComicUri (smbc614Comic)).IsRemoteFile, "smbc comic");
			
			Assert.IsFalse ((new ComicUri ("http://www.smbc-comics.com/")).IsRemoteFile, "smbc index");
			
			Assert.IsFalse ((new ComicUri ("http://www.xkcd.com/")).IsRemoteFile, "xkcd index");
		}
		
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
			smbcSplit = new ComicUri (smbcUrl);
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
	}
}

