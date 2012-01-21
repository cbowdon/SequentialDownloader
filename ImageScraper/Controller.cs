using System;
using System.IO;
using System.Threading;
using ImageScraperLib;

namespace ImageScraper
{
	public class Controller
	{
		public event EventHandler FileDownloaded;
		public event EventHandler AllFilesDownloaded;
		
		#region Active
		private bool active = false;

		public bool Active {
			get {
				return active;
			}
			set {
				active = value;
			}
		}
		#endregion
		
		#region InputUrl
		private string inputUrl = "http://";

		public string InputUrl {
			get {
				return inputUrl;
			}
			set {
				inputUrl = value;
				try {
					// can hang horribly here, due to the 'seek img src' function in UrlGenerator.ChooseGenerator
					Parser = new ComicParser (inputUrl);		
				} catch {
									
				}				
			}
		}
		#endregion
		
		#region OutputFileName
		private string outputFileName;

		public string OutputFileName {
			get {
				return outputFileName;
			}
			set {
				outputFileName = Path.GetFullPath (value);
			}
		}
		#endregion
		
		#region Start
		private string start;

		public string Start {
			get {
				return start;
			}
			set {
				int notUsed;
				if (int.TryParse (value, out notUsed)) {
					start = value;
					// will crash!
					//UrlGen.Start = start;	
				}				
			}
		}
		#endregion
		
		#region NumberToDownload
		private int numberToDownload = 100;

		public int NumberToDownload {
			get {
				return numberToDownload;
			}
			set {
				numberToDownload = value;
			}
		}
		#endregion
		
		#region NumberDownloaded
		private int numberDownloaded = 0;

		public int NumberDownloaded {
			get {
				return numberDownloaded;
			}
			set {
				FileDownloaded.Invoke (this, new EventArgs ());
				numberDownloaded = value;
			}
		}
		#endregion
		
		#region Parser
		private ComicParser parser;

		public ComicParser Parser {
			get {
				return parser;
			}
			set {
				parser = value;
				UrlGen = parser.GetUrlGenerator ();
				Start = UrlGen.Start;
			}
		}
		#endregion
		
		public UrlGenerator UrlGen { get; private set; }
		
		public Repository Repo { get; private set; }
		
		#region Constructor
		public Controller ()
		{
			
		}
		#endregion
		
		public void BeginDownloading ()
		{
			Console.WriteLine ("BEGIN DOWNLOADING");
			Repo = new Repository ();
			var imgUrls = UrlGen.Get (0, NumberToDownload);
								
			Active = true;
				
			Repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
				AllFilesDownloaded.Invoke (this, new EventArgs ());
				AfterDownloading ();
			};
				
			Repo.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
				NumberDownloaded += 1;
			};
				
			Repo.Download (imgUrls);			
		}
		
		public void Cancel ()
		{			
			Console.WriteLine ("CANCEL");
			Repo.CancelDownloads ();
			Repo.Dispose ();
			Active = false;
		}
		
		public void AfterDownloading ()
		{			
			Console.WriteLine ("AFTER DOWNLOADING");
			ComicConvert.ImgsToCbz (Repo.Location, OutputFileName);
			Repo.Dispose ();
			Active = false;
		}
	}
}

