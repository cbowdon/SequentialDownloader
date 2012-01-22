using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		private bool readyToGo;
		private Repository repo;
		
		#region Constructors
		public ModelController ()
		{
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
			this.InputUrl = inputUrl;
			this.OutputFileName = outputFileName;
			this.NumberToDownload = numberToDownload;
			this.readyToGo = true;
			return true;
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
			
			var urlGen = SetupTask ();
			
			var urls = urlGen.Get (0, NumberToDownload);
			
			repo = new Repository ();
			
			// pass on the events
			repo.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
				NumberDownloaded += 1;
				Console.WriteLine ("\tfile dl'd ({0})", NumberDownloaded);
				try {
					FileDownloaded.Invoke (sender, e);									
				} catch (NullReferenceException) {
					// no handler was added
				}
			};
			
			repo.DownloadProgressChanged += delegate(object sender, System.Net.DownloadProgressChangedEventArgs e) {
				Console.Write ("{0}, ", e.ProgressPercentage);
				try {
					FileProgress.Invoke (sender, e);					
				} catch (NullReferenceException) {
					// no handler was added	
				}

			};
			
			repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
				Console.WriteLine ("[Done]");
				try {
					TaskCompleted.Invoke (this, new EventArgs ());								
				} catch (NullReferenceException) {
					// no handler was added
				} finally {
					ComicConvert.ImgsToCbz (repo.Location, OutputFileName);			
					repo.Dispose ();
					Active = false;					
				}
			};
			
			// set number downloaded to zero
			NumberDownloaded = 0;
				
			// begin (blocks)
			// repo.Active is set and unset automatically			
			Console.WriteLine ("[Downloading and Adding]");
			
			
//			repo.DownloadAndAdd (urls, OutputFileName);
			repo.Download (urls);
			
		}
		
		public void CancelTask ()
		{
			if (repo.Active) {
				Console.WriteLine ("[Cancelling downloads]");
				repo.CancelDownloads ();
				Console.WriteLine ("[Cancelled]");
				repo.Dispose ();
				Console.WriteLine ("[Disposed]");
				Active = false;
				try {
					TaskCancelled.Invoke (this, new EventArgs ());					
				} catch (NullReferenceException) {
					// no handler was added
				}
			}
		}
	}
}

