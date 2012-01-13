using System;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestDateCount
	{	
		[Test()]
		public void Constructor ()
		{
			var isoDate = "http://comic.com/19871024";
			var ukDate = "http://comic.com/24101987";
			var usDate = "http://comic.com/10241987";
			var isoDateShort = "http://comic.com/871024";
			var ukDateShort = "http://comic.com/241087";
			var usDateShort = "http://comic.com/102487";
			var fakeDate = "http://comic.com/99999999";
			
			var dateCount = new BlockDateCount (new ComicUri (isoDate));			
			Assert.AreEqual (DateType.Iso, dateCount.Format);
			
			dateCount = new BlockDateCount (new ComicUri (ukDate));			
			Assert.AreEqual (DateType.Uk, dateCount.Format);
			
			dateCount = new BlockDateCount (new ComicUri (usDate));			
			Assert.AreEqual (DateType.Us, dateCount.Format);
			
			dateCount = new BlockDateCount (new ComicUri (isoDateShort));			
			Assert.AreEqual (DateType.IsoShort, dateCount.Format);

			dateCount = new BlockDateCount (new ComicUri (ukDateShort));			
			Assert.AreEqual (DateType.UkShort, dateCount.Format);
			
			dateCount = new BlockDateCount (new ComicUri (usDateShort));			
			Assert.AreEqual (DateType.UsShort, dateCount.Format);
			
			dateCount = new BlockDateCount (new ComicUri (fakeDate));
			Assert.AreEqual (DateType.NotRecognized, dateCount.Format);
			
		}
	}
}

