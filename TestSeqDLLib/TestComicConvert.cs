using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
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
		public void CbzToImgs ()
		{			
			var myCbz = "MyTestFile.cbz";
			var exeDir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			var outputDir = Path.GetFullPath ("MyTestFile");			
			
			// check it worked
			Assert.IsTrue (ComicConvert.CbzToImgs (myCbz, exeDir));			
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

