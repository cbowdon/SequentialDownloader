using System;
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
			var repo = new Repository();
			Assert.IsTrue (repo.DownloadSingle (imgUrl));
			Assert.AreEqual (1, repo.Files.Count);
		}
	}
}

