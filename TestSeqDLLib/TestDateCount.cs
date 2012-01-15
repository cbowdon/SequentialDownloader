using System;
using System.Collections.Generic;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestDateCount
	{	
		[Test()]
		public void Days ()
		{
			var comic = new ComicUri ("http://www.smbc-comics.com/comics/20061011.gif");
			var dateCount = new BlockDateCount (comic);
			// by chance, two consecutive Thursdays were missed here
			Assert.AreEqual (6, dateCount.Days.Count);
			Assert.IsFalse (dateCount.Days.Contains ("Thursday"));
		}
		
		[Test()]		
		public void FindFormat ()
		{
			var isoDate = "http://comic.com/19871024";
			var ukDate = "http://comic.com/24101987";
			var usDate = "http://comic.com/10241987";
			var isoDateShort = "http://comic.com/871024";
			var ukDateShort = "http://comic.com/241087";
			var usDateShort = "http://comic.com/102487";
			var fakeDate = "http://comic.com/99999999";
			
			// implicitly tests FindFormat by calling the property BlockDateCount.Format
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
		
		[Test()]
		public void GenerateSome ()
		{
			var smbc = "http://www.smbc-comics.com/comics/20061017.gif";
			
			var smbcUrls = new string[7];
			smbcUrls [0] = "http://www.smbc-comics.com/comics/20061011.gif";
			smbcUrls [1] = "http://www.smbc-comics.com/comics/20061012.gif";
			smbcUrls [2] = "http://www.smbc-comics.com/comics/20061013.gif";
			smbcUrls [3] = "http://www.smbc-comics.com/comics/20061014.gif";
			smbcUrls [4] = "http://www.smbc-comics.com/comics/20061015.gif";
			smbcUrls [5] = "http://www.smbc-comics.com/comics/20061016.gif";
			smbcUrls [6] = "http://www.smbc-comics.com/comics/20061017.gif";
			
			var dateCount = new BlockDateCount (new ComicUri (smbc));
			Assert.AreEqual (smbcUrls, dateCount.GenerateSome ().ToArray ());
			
			smbcUrls [0] = "http://www.smbc-comics.com/comics/20061011.gif";
			smbcUrls [1] = "http://www.smbc-comics.com/comics/20061012.gif";
			smbcUrls [2] = "http://www.smbc-comics.com/comics/20061013.gif";
			smbcUrls [3] = "http://www.smbc-comics.com/comics/20061018.gif";
			smbcUrls [4] = "http://www.smbc-comics.com/comics/20061019.gif";
			smbcUrls [5] = "http://www.smbc-comics.com/comics/20061020.gif";
			smbcUrls [6] = "http://www.smbc-comics.com/comics/20061025.gif";			
			var days = new string[]{DayOfWeek.Wednesday.ToString (), DayOfWeek.Thursday.ToString (), DayOfWeek.Friday.ToString ()};
			dateCount = new BlockDateCount (new ComicUri (smbcUrls [6]));
			dateCount.Days = new List<string> (days);
			Assert.AreEqual (smbcUrls, dateCount.GenerateSome ());			
		}
		
		[Test()]
		public void GenerateLast100 ()
		{
			var url = "http://www.smbc-comics.com/comics/20061011.gif";
			var dateCount = new BlockDateCount (new ComicUri (url));
			var urls = dateCount.GenerateLast100 ();
			Assert.AreEqual (100, urls.Count);
			Assert.AreEqual(url, urls[99]);
			
			dateCount.Days = new List<string> ();
			// 11th Oct 2006 was a Wednesday
			dateCount.Days.Add ("Wednesday");
			urls = dateCount.GenerateLast100 ();
			Assert.AreEqual (100, urls.Count);
			Assert.AreEqual(url, urls[99]);
		}
		
		[Test()]
		public void GenerateNext100 ()
		{
			var url = "http://www.smbc-comics.com/comics/20061011.gif";
			var dateCount = new BlockDateCount (new ComicUri (url));
			var urls = dateCount.GenerateNext100 ();
			Assert.AreEqual (100, urls.Count);
			Assert.IsFalse (urls.Contains(url));
			
			dateCount.Days = new List<string> ();
			dateCount.Days.Add ("Monday");
			urls = dateCount.GenerateNext100 ();
			Assert.AreEqual (100, urls.Count);
			Assert.IsFalse (urls.Contains(url));
		}
	}
}

