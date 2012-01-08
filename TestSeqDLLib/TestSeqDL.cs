using System;
using System.IO;
using SequentialDownloader;
using NUnit.Framework;

namespace TestSeqDLLib
{
	[TestFixture()]
	public class TestSeqDL
	{
		[Test()]
		public void GetComics ()
		{
			string inputUrl = "http://xkcd.com";
			string outputUrl = "xkcd.cbz";
			Assert.IsTrue (SeqDL.GetComics (inputUrl, outputUrl));
			Assert.IsTrue (File.Exists (outputUrl));
			Assert.Greater ((new FileInfo (outputUrl)).Length, 0);			
		}
		
		[Test()]
		public void ExtractComics ()
		{
			string inputUrl = "xkcd.cbz";			
			Assert.IsTrue (SeqDL.ExtractComics (inputUrl));
			Assert.Greater (Directory.GetFiles ("xkcd").Length, 0);
		}
		
		[Test()]
		public void ConvertComics ()
		{
			string inputUrl = "xkcd.cbz";
			string outputUrl = "xkcd.pdf";
			Assert.IsTrue (SeqDL.ConvertComics (inputUrl, outputUrl));
			Assert.IsTrue (File.Exists (outputUrl));
			Assert.Greater ((new FileInfo (outputUrl)).Length, 0);			
		}
	}
}

