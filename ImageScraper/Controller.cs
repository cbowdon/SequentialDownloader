using System;
using System.IO;
using ImageScraperLib;

namespace ImageScraper
{
	public class Controller
	{
		#region State
		public enum AppState
		{
			Scraping,
			NotScraping
		}
		
		public AppState State = AppState.NotScraping;
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
					UrlGen.Start = start;	
				}				
			}
		}
		#endregion
		private int numberToDownload = 100;

		public int NumberToDownload {
			get {
				return numberToDownload;
			}
			set {
				numberToDownload = value;
			}
		}
		
		public int NumberDownloaded { get; private set; }
		
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
			using (Repo = new Repository ()) {
				var imgUrls = UrlGen.Get (0, NumberToDownload);
				
				foreach (var i in imgUrls) {
					Console.WriteLine ("imgUrl:\t{0}", i);
				}
				
				State = AppState.Scraping;
				
				Repo.MultipleDownloadsCompleted += delegate(object sender, EventArgs e) {
					AfterDownloading ();
				};
				
				Repo.Download (imgUrls);
			}
		}
		
		public void AfterDownloading ()
		{
			ComicConvert.ImgsToCbz (Repo.Location, OutputFileName);
			State = AppState.NotScraping;			
		}		
	}
}

