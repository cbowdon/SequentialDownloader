using System;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestComicUri
	{
		[Test()]
		public void IsImageFile ()
		{
			var smbc614 = "http://www.smbc-comics.com/index.php?db=comics&id=614";
			Assert.IsFalse((new ComicUri(smbc614)).IsImageFile);
			var smbc614Comic = "http://www.smbc-comics.com/comics/20061011.gif";
			Assert.IsTrue((new ComicUri(smbc614Comic)).IsImageFile);
		}
	}
}

