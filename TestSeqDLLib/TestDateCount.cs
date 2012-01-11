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
			Assert.AreEqual (BlockDateFormat.ISO, BlockDateCount.FindFormat (isoDate, out result));
			Assert.AreEqual (BlockDateFormat.UK, BlockDateCount.FindFormat (ukDate, out result));
			Assert.AreEqual (BlockDateFormat.US, BlockDateCount.FindFormat (usDate, out result));			
			Assert.AreEqual (BlockDateFormat.ShortISO, BlockDateCount.FindFormat (isoDateShort, out result));
			Assert.AreEqual (BlockDateFormat.ShortUK, BlockDateCount.FindFormat (ukDateShort, out result));
			Assert.AreEqual (BlockDateFormat.ShortUS, BlockDateCount.FindFormat (usDateShort, out result));
			Assert.AreEqual (BlockDateFormat.NotRecognised, BlockDateCount.FindFormat (fakeDate, out result));
			
			Assert.IsTrue (BlockDateCount.IsDateTime (isoDate));						
			Assert.IsTrue (BlockDateCount.IsDateTime (ukDate));			
			Assert.IsTrue (BlockDateCount.IsDateTime (usDate));
			Assert.IsTrue (BlockDateCount.IsDateTime (isoDateShort));			
			Assert.IsTrue (BlockDateCount.IsDateTime (ukDateShort));
			Assert.IsTrue (BlockDateCount.IsDateTime (usDateShort));

		}
	}
}

