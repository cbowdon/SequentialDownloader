using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using ScraperLib;
using NUnit.Framework;

namespace TestScraperLibLib
{
	[TestFixture()]
	public class TestRepository
	{	
		static AutoResetEvent auto = new AutoResetEvent (false);
		
		[Test()]
		public void DownloadSingle ()
		{
			var imgUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			
			var repo = new Repository ();			
			TestOneDownload (repo, imgUrl);			
		}
		
		string TestOneDownload (Repository repo, string imgUrl)
		{
			// do download
			repo.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs e) {
				auto.Set ();
			};
			
			bool downloadStartedEvent = false;
			repo.DownloadStarted += delegate(object sender, EventArgs e) {
				downloadStartedEvent = true;
			};
			
			repo.Download (imgUrl);
			
			Assert.IsTrue(downloadStartedEvent);
			
			// check on Files dict
			Assert.AreEqual (1, repo.Files.Count, "Only one entry in Files");												
			var fileUrl = Path.Combine (repo.Location, "00001.gif");
			Assert.IsTrue (repo.Files.Contains (new KeyValuePair<string,string> (imgUrl, fileUrl)), "Files contains this entry");
			Assert.AreEqual (imgUrl, repo.CurrentlyDownloadingUrl);
			
			// wait for auto
			auto.WaitOne ();
			Assert.IsTrue (File.Exists (fileUrl), "File exists");
			Assert.AreEqual (26823, (new FileInfo (fileUrl)).Length, "File is correct size");
			return fileUrl;
		}
		
		[Test()]
		public void Disposable ()
		{
			var imgUrl = "http://www.smbc-comics.com/comics/20061011.gif";
			string fileUrl = "";
			using (var repo = new Repository ()) {		
				fileUrl = TestOneDownload (repo, imgUrl);
			}			
			Assert.IsFalse (File.Exists (fileUrl));
			Assert.IsFalse (Directory.Exists (Path.GetDirectoryName (fileUrl)));
		}
		
		[Test()]
		public void DownloadMany ()
		{			
			
			// choose target files to download
			var url = "http://www.smbc-comics.com/comics/20061001.gif";
//			var url = "http://www.irregularwebcomic.net/55.html";
			var parser = new ComicParser (url);
			var urlGen = parser.GetUrlGenerator () as DateGenerator;
			urlGen.Days = DateGenerator.EveryDay;
			var urls = urlGen.Get (0, 20);
			
			foreach (var x in urls) {
				Console.WriteLine (x);
			}

			var numComics = urls.Where<string> (x => x != String.Empty).Count ();
			
			using (var repo = new Repository ()) {
			
				// set the gate
				AutoResetEvent auto = new AutoResetEvent (false);
			
				// event handler allows program to progress
				repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
					auto.Set ();
				};
			
				// begin the download and block
				repo.Download (urls);
//				auto.WaitOne ();
				
				// check that it worked
				Assert.AreEqual (numComics, repo.Files.Count);				
			}
		}
		
		[Test()]
		public void CancelDownloads ()
		{
			// choose target files to download
			var url = "http://www.smbc-comics.com/comics/20061001.gif";
//			var url = "http://www.irregularwebcomic.net/55.html";
			var parser = new ComicParser (url);
			var urlGen = parser.GetUrlGenerator () as DateGenerator;
			urlGen.Days = DateGenerator.EveryDay;
			var urls = urlGen.Get (0, 20);
			
			foreach (var x in urls) {
				Console.WriteLine (x);
			}
			
			using (var repo = new Repository ()) {
			
				// set the gate
				AutoResetEvent auto = new AutoResetEvent (false);
				
				int num = 0;

				// event handler to count number of async downloads
				repo.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs e) {
					num += 1;
					// here we set the canceller
					if (num == 5) {
						repo.CancelDownloads ();
						auto.Set ();
					}
				};
				
				// event handler allows program to progress (in case of cancel failure)
				repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
					auto.Set ();
				};
			
				// begin the download and block
				repo.Download (urls);
				auto.WaitOne ();
				
				// check that it worked
				Assert.AreEqual (5, repo.Files.Count);				
			}
		}
		
		[Test()]
		public void DownloadAndAdd ()
		{
			// Add to existing:
			
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2.cbz";
			var urls = new List<string> ();
			urls.Add ("http://www.smbc-comics.com/comics/20061111.gif");
			
			// check original compression worked
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz));
			var sizeBefore = (new FileInfo (myCbz)).Length;			
									
			// download and add
			using (var repo = new Repository()) {
				
				// add a turnstile
				AutoResetEvent auto = new AutoResetEvent (false);				
				
				// event handler allows program to progress (in case of cancel failure)
				repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
					auto.Set ();
				};				
				
				repo.DownloadAndAdd (urls, myCbz);
	
				// hold on until test finished
				auto.WaitOne ();
			}
			
			// check sizes indicate added file
			var sizeAfter = (new FileInfo (myCbz)).Length;
			Assert.Greater (sizeAfter, sizeBefore);
						
			// clean up
			File.Delete (myCbz);
			
			// Create new:
			
			// download and add
			using (var repo = new Repository()) {
				
				// add a turnstile
				AutoResetEvent auto = new AutoResetEvent (false);				
				
				// event handler allows program to progress (in case of cancel failure)
				repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
					auto.Set ();
				};				
				
				repo.DownloadAndAdd (urls, myCbz);
	
				// hold on until test finished
				auto.WaitOne ();
			}
			
			// check file was made
			Assert.IsTrue (File.Exists (myCbz));
			
			// check file size > 0 but less than before
			var sizeOne = (new FileInfo (myCbz)).Length;			
			Assert.Greater (sizeOne, 0, "Fresh-made zip file has non-zero size");
			Assert.Less (sizeOne, sizeBefore, "Zip file with only one entry smaller than with 100 entries");
			
			// delete file again
			File.Delete (myCbz);

		}		
	}
}

