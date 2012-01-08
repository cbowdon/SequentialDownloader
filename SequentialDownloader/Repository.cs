using System;
using System.IO;
using System.Collections.Generic;

namespace SequentialDownloader
{
	public class Repository
	{
		public string Location { get; private set; }
		
		public Repository ()
		{
			this.Location = Path.GetFullPath ("/");				
		}
		
		public Repository (string loc)
		{
			this.Location = Path.GetFullPath (loc);
		}
		
		public bool DownloadSingle (string url)
		{
			throw new NotImplementedException ();
		}
		
		public Dictionary<ComicUri, string> Files {
			get {
				throw new NotImplementedException ();
			}
		}
	}
}

