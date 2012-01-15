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
		
		public bool Download (string url)
		{
			throw new NotImplementedException ();
		}
		
		public bool Download (IEnumerable<string> url)
		{
			throw new NotImplementedException ();
		}
		
		public Dictionary<string, string> Files {
			get {
				throw new NotImplementedException ();
			}
		}
		
		public void Clear ()
		{
			throw new NotImplementedException ();
		}
	}
}

