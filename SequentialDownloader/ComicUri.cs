using System;

namespace SequentialDownloader
{
	public class ComicUri : Uri
	{
		public string[] Indices { get; private set; }
		
		public string Base { get; private set; }
		
		public ComicUri (string url) : base (url)
		{			
		}
	}
}

