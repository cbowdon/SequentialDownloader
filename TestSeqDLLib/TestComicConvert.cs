using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using ScraperLib;

using NUnit.Framework;

namespace TestScraperLibLib
{
	[TestFixture()]
	public class TestComicConvert
	{
		[Test()]
		public void ImgsToCbz ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2.cbz";
			
			// check it worked
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz));
			
			// check sizes indicate compression
			var combinedSizeOfFiles = Directory.GetFiles (dir).Select<string,long> (x => (new FileInfo (x)).Length).Sum ();
			var sizeOfMyCbz = (new FileInfo (myCbz)).Length;			
			Assert.Greater (combinedSizeOfFiles, sizeOfMyCbz);
			Assert.Greater (sizeOfMyCbz, combinedSizeOfFiles / 2);
			
			// clean up
			File.Delete (myCbz);
		}
		
		[Test()]
		public void AddToCbz ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2.cbz";
			var fileName = "extra.gif";
			
			// check original compression worked
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz));
			var sizeBefore = (new FileInfo (myCbz)).Length;			
			
			// check adding
			Assert.IsTrue (ComicConvert.AddToCbz (fileName, myCbz));
			
			// check sizes indicate added file
			var sizeAfter = (new FileInfo (myCbz)).Length;
			Assert.Greater (sizeAfter, sizeBefore);
						
			// clean up
			File.Delete (myCbz);
		}
		
		[Test()]
		public void CbzToImgs ()
		{			
			var myCbz = "MyTestFile.cbz";
			var outputDir = Path.GetFullPath ("MyTestFile");			
			
			// check it worked
			Assert.IsTrue (ComicConvert.CbzToImgs (myCbz));			
			Assert.IsTrue (Directory.Exists (outputDir));
			Assert.AreEqual (100, Directory.GetFiles (outputDir).Where<string> (x => x.Contains (".gif")).Count ());
			
			// check sizes indicate compression
			var combinedSizeOfFiles = Directory.GetFiles (outputDir).Select<string,long> (x => (new FileInfo (x)).Length).Sum ();
			var sizeOfMyCbz = (new FileInfo (myCbz)).Length;			
			Assert.Greater (combinedSizeOfFiles, sizeOfMyCbz);
			Assert.Greater (sizeOfMyCbz, combinedSizeOfFiles / 2);
			
			// clean up
			foreach (var x in Directory.GetFiles(outputDir)) {
				File.Delete (x);
			}
			Directory.Delete (outputDir);
		}
		
		[Test()]
		public void Naming ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2";
			
			// check it worked
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz + ".cbz"));			
			File.Delete (myCbz + ".cbz");	
			
			myCbz = "MyTestFile_2.zip";
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));
			Assert.IsTrue (File.Exists (myCbz));
			File.Delete (myCbz);
		}
		
		[Test()]
		public void EmptyFilesNotIncluded ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2.cbz";
				
			// create once with no empty files
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz));
			var sizeBefore = (new FileInfo (myCbz)).Length;			
			File.Delete (myCbz);

			// create some empty files
			var origNum = Directory.GetFiles (dir).Where<string> (x => x.Contains (".gif")).Count ();
			
			for (int i = 0; i < 10; i++) {
				File.Create (Path.Combine (dir, String.Format ("blank_{0}.txt", i)));	
			}			
			Assert.AreEqual (10 + origNum, Directory.GetFiles (dir).Where<string> (x => x.Contains (".gif") || x.Contains(".txt")).Count ());
						
			// create again with empty files
			Assert.IsTrue (ComicConvert.ImgsToCbz (dir, myCbz));			
			Assert.IsTrue (File.Exists (myCbz));
			var sizeAfter = (new FileInfo (myCbz)).Length;			
			
			Assert.AreEqual (sizeBefore, sizeAfter);
		
			// unzip it and count
			var tempDir = "MyTestFile_2";
			ComicConvert.CbzToImgs (myCbz);
			Assert.AreEqual (origNum, Directory.GetFiles (tempDir).Where<string> (x => x.Contains (".gif") || x.Contains(".txt")).Count ());
			
			// clean up
			foreach (var x in Directory.GetFiles(tempDir)) {
				File.Delete (x);
			}
			Directory.Delete (tempDir);			
			File.Delete (myCbz);
			
		
		}
	}
}

