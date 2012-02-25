using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.IO;
using ScraperLib;

namespace Scraper
{
	public class ModelController
	{			
		public event EventHandler TaskStarted;
		public event EventHandler TaskCompleted;
		public event EventHandler TaskCancelled;
		public event AsyncCompletedEventHandler FileDownloaded;
		public event ProgressChangedEventHandler FileProgress;
		
		public bool Active { get; private set; }
		
		public string OutputFileName { get; private set; }
		
		public string InputUrl { get; private set; }
				
		public int NumberToDownload { get; private set; }
		
		public int NumberDownloaded { get; private set; }
		
		public string Status { get; private set; }
		
		private bool readyToGo;
		private Repository repo;
		
		#region Constructors
		public ModelController ()
		{
			Status = "Determining image source";
		}
		#endregion
		
		#region ValidInputs
		public bool ValidateInputs (string inputUrl, string outputFileName, int numberToDownload)
		{
			this.InputUrl = ParseInputUrl (inputUrl);
			if ((new ComicUri (InputUrl)).Indices.Length != 1) {
				throw new ArgumentException ("Invalid input URL: could not find the index.");
			}
			this.OutputFileName = ParseOutputFileName (outputFileName);
			this.NumberToDownload = numberToDownload;
			if (NumberToDownload < 0) {
				throw new ArgumentException ("Invalid number to download: cannot download < 1 files.");
			}
			this.readyToGo = true;
			return true;				
		}
		
		private string ParseInputUrl (string inputUrl)
		{
			var scheme = "http://";
			if (inputUrl.Substring (0, 7) != scheme &&
			    inputUrl.Substring (0, 6) != "ftp://" &&
			    inputUrl.Substring (0, 8) != "https://" &&
			    inputUrl.Substring (0, 7) != "file://") {
				return String.Format ("{0}{1}", scheme, inputUrl);
			} else {
				return inputUrl;	
			}				
		}
		
		private string ParseOutputFileName (string outputFileName)
		{
			string outputFilePath;
			if (Path.GetExtension (outputFileName) == string.Empty) {
				outputFilePath = String.Format ("{0}.zip", Path.GetFullPath (outputFileName));
			} else {
				outputFilePath = Path.GetFullPath (outputFileName);
			}
			if (File.Exists (outputFilePath)) {
				var dir = Path.GetDirectoryName (outputFilePath);
				var name = Path.GetFileNameWithoutExtension (outputFilePath);
				var ext = Path.GetExtension (outputFilePath);
				int i = 1;
				while (i < 10000) {
					var newName = Path.GetFullPath (String.Format ("{0} ({1}){2}", Path.Combine (dir, name), i, ext));
					if (!File.Exists (newName)) {
						return newName;
					} else {
						i++;
					}
				}
				throw new IOException (String.Format ("Can't find a filename like {0} that isn't taken!", outputFilePath));
				
			} else {
				return outputFilePath;
			}
		}
		#endregion
		
		private UrlGenerator SetupTask ()
		{
			var comic = new ComicUri (InputUrl);

			var parser = new ComicParser (InputUrl);
			
			var urlGen = parser.GetUrlGenerator ();
			
			// in the interests of simplicity, just start from given comicå			
			urlGen.Start = comic.Indices [0];
			
			return urlGen;
		}
		
		public void RunTask ()
		{
			if (!readyToGo) {
				throw new ArgumentException ("Not ready to go!");
			} else {
				if (TaskStarted != null) {
					TaskStarted.Invoke (this, new EventArgs ());
				}
			}
			
			Active = true;
			
			Status = "Analyzing source";
			
			var urlGen = SetupTask ();
			
			var urls = urlGen.Get (0, NumberToDownload);
			
			using (repo = new Repository ()) {
				
				// pass on the events
				repo.DownloadFileCompleted += OnSingleDownloadCompleted;
			
				repo.DownloadProgressChanged += OnDownloadProgressChanged;
			
				repo.MultipleDownloadsCompleted += OnMultipleDownloadsCompleted;
			
				// set number downloaded to zero
				NumberDownloaded = 0;
								
				// begin (blocks)			
				repo.Download (urls);
				// repo.Active is set and unset automatically			
			}
		}
		
		public void CancelTask ()
		{
			try {
				if (repo.Active) {
					repo.CancelDownloads ();
					Active = false;				
					Status = "Task cancelled";
					ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
					if (TaskCancelled != null) {
						TaskCancelled.Invoke (this, new EventArgs ());						
					}
				}	
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
			
			Active = false;				
			Status = "Task cancelled";
			if (TaskCancelled != null) {
				TaskCancelled.Invoke (this, new EventArgs ());						
			}
		}
		
		#region EventHandlers
		public void OnMultipleDownloadsCompleted (object sender, EventArgs e)
		{					
			ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
			Active = false;
			Status = "Finished";
			if (TaskCompleted != null) {
				TaskCompleted.Invoke (this, new EventArgs ());								
			} 
		}
		
		public void OnSingleDownloadCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			NumberDownloaded += 1;
			// update status, don't contradict MultipleDownloadsCompleted
			if (NumberDownloaded != NumberToDownload) {
				Status = String.Format ("Downloading {0}", repo.CurrentlyDownloadingUrl);
			}
			// efficient dumping behaviour
			if (NumberDownloaded % 10 == 0) {
				ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
			}
			if (FileDownloaded != null) {
				FileDownloaded.Invoke (sender, e);										
			}
		}
		
		public void OnDownloadProgressChanged (object sender, System.Net.DownloadProgressChangedEventArgs e)
		{
			if (FileProgress != null) {
				FileProgress.Invoke (sender, e);							
			}
		}
		#endregion
	}
	
}

