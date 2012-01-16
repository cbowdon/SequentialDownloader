using System;
using System.IO;
using System.Linq;
using SequentialDownloader;

using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestComicConvert
	{
		[Test()]
		public void ImgsToCbz ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC");			
			var myCbz = "MyTestFile_2.cbz";
			
			ComicConvert.ImgsToCbz (dir, myCbz);
			
			Assert.IsTrue (File.Exists (myCbz));
			
			var combinedSizeOfFiles = Directory.GetFiles (dir).Select<string,long> (x => (new FileInfo (x)).Length).Sum ();
			var sizeOfMyCbz = (new FileInfo (myCbz)).Length;
			
			Assert.Greater (combinedSizeOfFiles, sizeOfMyCbz);
			Assert.Greater (sizeOfMyCbz, combinedSizeOfFiles / 2);
			
			File.Delete (myCbz);
		}
		
		[Test()]
		public void CbzToImgs ()
		{
			var dir = Path.GetFullPath ("TestFilesSMBC_2");			
			var myCbz = "MyTestFile.cbz";
			
			ComicConvert.CbzToImgs (myCbz, dir);
			
			Assert.IsTrue (Directory.Exists (dir));
			Assert.AreEqual (100, Directory.GetFiles (dir).Count ());
			
			var combinedSizeOfFiles = Directory.GetFiles (dir).Select<string,long> (x => (new FileInfo (x)).Length).Sum ();
			var sizeOfMyCbz = (new FileInfo (myCbz)).Length;
			
			Assert.Greater (combinedSizeOfFiles, sizeOfMyCbz);
			Assert.Greater (sizeOfMyCbz, combinedSizeOfFiles / 2);
			
			Directory.Delete (dir);
		}
		
		[Test()]
		public void ImgsToPdf ()
		{
			throw new NotImplementedException ();
		}
		
		[Test()]
		public void PdfToImgs ()
		{
			throw new NotImplementedException ();
		}
	}
}

