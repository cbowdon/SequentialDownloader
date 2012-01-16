using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace SequentialDownloader
{
	public class Repository : WebClient, IDisposable
	{
		public event EventHandler MultipleDownloadsCompleted;
		
		private static AutoResetEvent auto = new AutoResetEvent (false);
			
		#region Properties
		public string Location { get; private set; }
		
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
		}
		#endregion
		
		
		#region Methods
		public void Download (string url)
		{
			// create filename			
			string fileName = (Files.Count + 1).ToString ().PadLeft (5, '0') + Path.GetExtension (url);
			Files.TryAdd (url, Path.Combine (Location, fileName));
			DownloadFileAsync (new Uri (url), Path.Combine (Location, fileName));			
		}
		
		public void Download (IEnumerable<string> urls)
		{		
			DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
				auto.Set ();
			};
			foreach (var url in urls) {
				Download (url);
				auto.WaitOne ();
			}
			MultipleDownloadsCompleted.Invoke (this, new EventArgs());
		}
		
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
	}
}

