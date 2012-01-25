using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.IO;
using ImageScraperLib;

namespace ImageScraper
{
	public class ModelController
	{			
		public event EventHandler TaskCompleted;
		public event EventHandler TaskCancelled;
		public event AsyncCompletedEventHandler FileDownloaded;
		public event ProgressChangedEventHandler FileProgress;
		
		public bool Active { get; private set; }
		
		public string OutputFileName { get; private set; }
		
		public string InputUrl { get; private set; }
		
		#region Start
		private bool startSet = false;
		private string start;

		public string Start {
			get {
				return start;
			}
			set {
				startSet = true;
				start = value;
			}
		}
		#endregion
		
		#region Days
		private bool daysSet = false;
		private List<string> days;

		public List<string> Days {
			get {
				return days;
			}
			set {
				daysSet = true;
				days = value;
			}
		}
		#endregion
				
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
		public bool ValidateInputs (string inputUrl, string outputFileName, int numberToDownload, string start, List<string> days)
		{
			this.Start = start;
			this.Days = days;
			return ValidateInputs (inputUrl, outputFileName, numberToDownload);
		}
		
		public bool ValidateInputs (string inputUrl, string outputFileName, int numberToDownload, List<string> days)
		{
			this.Days = days;
			return ValidateInputs (inputUrl, outputFileName, numberToDownload);
		}
		
		public bool ValidateInputs (string inputUrl, string outputFileName, int numberToDownload, string start)
		{
			this.Start = start;
			return ValidateInputs (inputUrl, outputFileName, numberToDownload);
		}

		public bool ValidateInputs (string inputUrl, string outputFileName, int numberToDownload)
		{
			this.InputUrl = ParseInputUrl (inputUrl);
			if ((new ComicUri (InputUrl)).Indices.Length != 1) {
				throw new ArgumentException ("Cannot parse this input url!");
			}
			this.OutputFileName = ParseOutputFileName (outputFileName);
			this.NumberToDownload = numberToDownload;
			this.readyToGo = true;
			return true;				
		}
		
		private string ParseInputUrl (string inputUrl)
		{
			var scheme = "http://";
			if (inputUrl.Substring (0, 7) != scheme &&
			    inputUrl.Substring (0, 7) != "ftp://" &&
			    inputUrl.Substring (0, 7) != "https://") {
				return String.Format ("{0}{1}", scheme, inputUrl);
			} else {
				return inputUrl;	
			}				
		}
		
		private string ParseOutputFileName (string outputFileName)
		{
			string outputFilePath;
			if (Path.GetExtension (outputFileName) == string.Empty) {
				outputFilePath = String.Format ("{0}.cbz", outputFileName);
			} else {
				outputFilePath = outputFileName;
			}
			if (File.Exists (outputFilePath)) {
				var name = Path.GetFileNameWithoutExtension (outputFilePath);
				var ext = Path.GetExtension (outputFilePath);
				int i = 1;
				while (i < 10000) {
					var newName = String.Format ("{0} ({1}){2}", name, i, ext);
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
			ComicParser parser = new ComicParser (InputUrl);
			
			var urlGen = parser.GetUrlGenerator ();
			
			// set start
			if (startSet) {
				urlGen.Start = Start;
			}
			
			// if date, set days and return
			if (urlGen.ToString () == "ImageScraperLib.DateGenerator") {
				var dateGen = (DateGenerator)urlGen;
				if (daysSet) {
					dateGen.Days = Days;
				}				
				return dateGen;
			} else {
				// else just return
				return urlGen;
			}			
		}
		
		public void RunTask ()
		{
			if (!readyToGo) {
				throw new ArgumentException ("Not ready to go!");
			}
			
			Active = true;
			
			Status = "Determining image source";
			
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
				repo.Download (urls);// DownloadAndAdd is not used because of Zip64 issues
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
					try {
						TaskCancelled.Invoke (this, new EventArgs ());					
					} catch (NullReferenceException) {
						// no handler was added
					} 
				}	
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
			
			Active = false;				
			Status = "Task cancelled";
			try {
				TaskCancelled.Invoke (this, new EventArgs ());					
			} catch (NullReferenceException) {
				// no handler was added
			} 
		}
		
		#region EventHandlers
		public void OnMultipleDownloadsCompleted (object sender, EventArgs e)
		{					
			ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
			Active = false;
			Status = "Finished";
			try {
				TaskCompleted.Invoke (this, new EventArgs ());								
			} catch (NullReferenceException) {
				// no handler was added
			} 
		}
		
		public void OnSingleDownloadCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			NumberDownloaded += 1;
			// update status, don't contradict MultipleDownloadsCompleted
			if (NumberDownloaded != NumberToDownload) {
				Status = String.Format ("Downloading {0}", repo.CurrentlyDownloadingUrl);
			}
			// caching behaviour
			if (NumberDownloaded % 10 == 0) {
				ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
			}
			try {
				FileDownloaded.Invoke (sender, e);									
			} catch (NullReferenceException) {
				// no handler was added
			}
		}
		
		public void OnDownloadProgressChanged (object sender, System.Net.DownloadProgressChangedEventArgs e)
		{
			try {
				FileProgress.Invoke (sender, e);					
			} catch (NullReferenceException) {
				// no handler was added	
			}
		}
		#endregion
	}
	
}

