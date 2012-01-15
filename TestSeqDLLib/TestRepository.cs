using System;
using System.Collections.Generic;
using System.Linq;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestRepository
	{
		[Test()]
		public void DownloadSingle ()
		{
			var imgUrl = "http://imgs.xkcd.com/comics/woodpecker";
			var repo = new Repository ();
			
			repo.Download (imgUrl);
			Assert.AreEqual (1, repo.Files.Count);
			
			
		}
		
		[Test()]
		public void DownloadMany ()
		{			
			var smbcUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			var smbcParser = new ComicParser (smbcUrl);
			var smbcUrls = smbcParser.FindUrls ();
			
			var repo = new Repository ();
			repo.Download (smbcUrls);
			Assert.AreEqual (smbcUrls.Where<string> (x => x != String.Empty), repo.Files.Count);
		}
	}
}

