using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace ImageScraperLib
{
	public class Repository : WebClient, IDisposable
	{
		#region Events
		public event EventHandler MultipleDownloadsCompleted;
		public event EventHandler DownloadStarted;
		public event EventHandler DownloadsCancelled;

		private AutoResetEvent auto = new AutoResetEvent (false);
		#endregion
			
		#region Properties
		public bool Active { get; private set; }
		
		public string Location { get; private set; }
		
		public string CurrentlyDownloadingUrl { get; private set; }
		
		public ConcurrentDictionary<string, string> Files { get; private set; }
		#endregion
		
		#region Constructors
		public Repository () : base ()
		{
			var thisDir = Assembly.GetExecutingAssembly ().Location;					
			Constructor (Path.GetDirectoryName (thisDir));			
		}
		
		public Repository (string loc) : base ()
		{
			Constructor (loc);
		}
		
		protected void Constructor (string loc)
		{	
			var tempFolderName = DateTime.Now.ToString ();
			var fullLoc = Path.Combine (Path.GetFullPath (loc), tempFolderName);
			Directory.CreateDirectory (fullLoc);
			this.Location = Path.GetFullPath (fullLoc);
			this.Files = new ConcurrentDictionary<string, string> ();
			DownloadStarted += HandleDownloadStarted;
		}

		void HandleDownloadStarted (object sender, EventArgs e)
		{
			var url = sender as string;
			CurrentlyDownloadingUrl = url;
		}
		#endregion
		
		#region Download
		public void Download (string url)
		{
			// declare current download
			DownloadStarted.Invoke ((object)url, new EventArgs ());
			// create filename			
			string fileName = (Files.Count + 1).ToString ().PadLeft (5, '0') + Path.GetExtension (url);
			// add to dictionary
			Files.TryAdd (url, Path.Combine (Location, fileName));
			// download (async so we can tap into the progress meter)
			DownloadFileAsync (new Uri (url), Path.Combine (Location, fileName));					
		}
		
		public void Download (IEnumerable<string> urls)
		{		
			Active = true;
			
			// after each single download, turn the stile
			DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
				auto.Set ();				
			};
			
			// loop through and download each
			foreach (var url in urls) {
				// check we are still a going concern
				if (Active) {
					Download (url);
					// block until download finished (turning async into sync)
					auto.WaitOne ();
				} else {
					// fire cancelled event
					try {
						DownloadsCancelled.Invoke (this, new EventArgs ());	
					} catch (NullReferenceException) {
						// no handlers were added
					}
					// return without invoking MultipleDownloadsCompleted
					return;
				}
			}			
			
			// all done, invoke event
			MultipleDownloadsCompleted.Invoke (this, new EventArgs ());
			
			// turn off the sign
			Active = false;
		}
		#endregion
		
		#region Cancel Downloads
		public void CancelDownloads ()
		{
			Active = false;
		}
		#endregion
		
		#region Dispose
		public new void Dispose ()
		{
			base.Dispose ();
			foreach (var x in Files) {
				try {
					File.Delete (x.Value);
				} catch (Exception e) {
					if (e is System.IO.IOException ||
					    e is System.IO.FileNotFoundException) {
						Console.WriteLine ("Could not delete {0}", x.Value);
					} else {
						throw;
					}
				}
			}
			try {
				Directory.Delete (Location);	
			} catch (Exception e) {
				if (e is System.IO.IOException ||
					    e is System.IO.FileNotFoundException) {
					Console.WriteLine ("Could not delete {0}", Location);
				} else {
					throw;
				}
			}
		}
		#endregion
		
		#region Download and Add
		public void DownloadAndAdd (IEnumerable<string> urls, string outputFileName)
		{
			Active = true;
			
			// loop through and download each
			foreach (var url in urls) {
				// check we are still a going concern
				Console.WriteLine (url);
				
				if (Active) {
					DownloadAndAdd (url, outputFileName);					
				} else {
					// return without invoking MultipleDownloadsCompleted
					return;
				}
			}			
			
			// all done, invoke event
			MultipleDownloadsCompleted.Invoke (this, new EventArgs ());
			
			// turn off the sign
			Active = false;
		}
		
		public void DownloadAndAdd (string url, string outputFileName)
		{
			AutoResetEvent singleAuto = new AutoResetEvent (false);
			
			// after each single download, turn the stile
			DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
				Console.WriteLine ("Download completed:\t{0}\t{1}", url, outputFileName);
				singleAuto.Set ();				
			};
			
			// create filename			
			string fileName = (Files.Count + 1).ToString ().PadLeft (5, '0') + Path.GetExtension (url);
			// add to dictionary
			Files.TryAdd (url, Path.Combine (Location, fileName));
			// download (async so we can tap into the progress meter)
			DownloadFileAsync (new Uri (url), Path.Combine (Location, fileName));			
			// block this thread until DL completed
			singleAuto.WaitOne ();
			// add to zip
			ComicConvert.AddToCbz (Path.Combine (Location, fileName), outputFileName);
		}
		#endregion
	}
}

