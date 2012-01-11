using System;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestDateCount
	{
		[Test()]		
		public void TryParseDates ()
		{
			var isoDate = "19871024";
			var ukDate = "24101987";
			var usDate = "10241987";
			var isoDateShort = "871024";
			var ukDateShort = "241087";
			var usDateShort = "102487";
			var fakeDate = "99999999";
									
			DateTime result;
			string format;
			Assert.AreEqual (BlockDateType.ISO, BlockDateCount.FindFormat (isoDate, out result, out format));
			Assert.AreEqual (BlockDateType.UK, BlockDateCount.FindFormat (ukDate, out result, out format));
			Assert.AreEqual (BlockDateType.US, BlockDateCount.FindFormat (usDate, out result, out format));			
			Assert.AreEqual (BlockDateType.ShortISO, BlockDateCount.FindFormat (isoDateShort, out result, out format));
			Assert.AreEqual (BlockDateType.ShortUK, BlockDateCount.FindFormat (ukDateShort, out result, out format));
			Assert.AreEqual (BlockDateType.ShortUS, BlockDateCount.FindFormat (usDateShort, out result, out format));
			Assert.AreEqual (BlockDateType.NotRecognised, BlockDateCount.FindFormat (fakeDate, out result, out format));
			
			Assert.IsTrue (BlockDateCount.IsDateTime (isoDate));						
			Assert.IsTrue (BlockDateCount.IsDateTime (ukDate));			
			Assert.IsTrue (BlockDateCount.IsDateTime (usDate));
			Assert.IsTrue (BlockDateCount.IsDateTime (isoDateShort));			
			Assert.IsTrue (BlockDateCount.IsDateTime (ukDateShort));
			Assert.IsTrue (BlockDateCount.IsDateTime (usDateShort));
		}
		
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
			Assert.AreEqual (BlockDateType.ISO, dateCount.Type);
			
			dateCount = new BlockDateCount (new ComicUri (ukDate));			
			Assert.AreEqual (BlockDateType.UK, dateCount.Type);
			
			dateCount = new BlockDateCount (new ComicUri (usDate));			
			Assert.AreEqual (BlockDateType.US, dateCount.Type);
			
			dateCount = new BlockDateCount (new ComicUri (isoDateShort));			
			Assert.AreEqual (BlockDateType.ShortISO, dateCount.Type);

			dateCount = new BlockDateCount (new ComicUri (ukDateShort));			
			Assert.AreEqual (BlockDateType.ShortUK, dateCount.Type);
			
			dateCount = new BlockDateCount (new ComicUri (usDateShort));			
			Assert.AreEqual (BlockDateType.ShortUS, dateCount.Type);
			
			dateCount = new BlockDateCount (new ComicUri (fakeDate));
			Assert.AreEqual (BlockDateType.NotRecognised, dateCount.Type);
			
		}
	}
}

