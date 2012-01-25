using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;

namespace ScraperLib
{
	public class ComicParser
	{
		#region Properties
		string inputUrl;
		string htmlSource;

		public string HtmlSource {
			get { 
				if (htmlSource == null) {
					htmlSource = WebUtils.GetSourceCode (inputUrl);
				}
				return htmlSource;
			}
		}
		#endregion
		
		#region Constructors
		public ComicParser (string inputUrl)
		{
			this.inputUrl = inputUrl;
		}
		#endregion
		
		#region GetUrlGenerator
		public UrlGenerator GetUrlGenerator ()
		{
			// start with the input URL
			var comic = new ComicUri (inputUrl);			
			// choose appropriate generator type
			var urlGen = ChooseGenerator (inputUrl);			
			
			if (!comic.IsRemoteFile) {
				// find the img src URL
				var someUrls = urlGen.GenerateSome ();
				string srcUrl = "";
				UrlGenerator.IdentifyImg (someUrls, out srcUrl);
				try {
					// try to create a generator for img src URLs
					return ChooseGenerator (srcUrl);	
				} catch (NotImplementedException) {
					// if not possible, just return page URL generator
					return urlGen;
				}				
			} else {
				// return page URL generator
				return urlGen;
			}
			
						
		}
		
		private UrlGenerator ChooseGenerator (string comicUrl)
		{									
			var aComic = new ComicUri (comicUrl);
			
			UrlGenerator gen;
			
			if (aComic.Indices.Length == 1) {
				// option A or B
				
				var dateCount = new DateGenerator (aComic);
					
				// identify if it's a valid date
				if (dateCount.Format != DateType.NotRecognized) {					
					// B of some kind
					gen = dateCount;					
				} else {
					// A of some kind
					var seqCount = new SequentialGenerator (aComic);
					gen = seqCount;
				}
			} else {
				// option C or D
				throw new NotImplementedException ();
			}
			return gen;	
		}
		#endregion
	}
}

