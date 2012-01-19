using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;

namespace SequentialDownloader
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
		
		#region Methods
			
		/// <summary>
		/// Finds the urls of other comics.
		/// </summary>
		/// <returns>
		/// The urls.
		/// </returns>
		public List<string> FindUrls ()
		{			
			// take ComicUri(url)
			var comic = new ComicUri (inputUrl);
						
			/// Procedure
			/// 
			/// figure out the naming rules --> create an appropriate ICountingRule
			/// 
			/// identify which number img tag src is the comic --> use the ICountingRule to generate a few test pages
			/// 
			/// for each page, try get the src of the nth img tag -- use the ICountingRule->FindImgs to get urls 
			/// 
			
			// figure out the naming rules
			
			/// possible formats:
			/// A straight number sequence : 1 number, length unknown, possibly padded, increment is 1
			/// B iso/uk/us dates: 1 number, length 6, padded, increment unknown
			/// C separated iso/uk/us dates : 3 numbers, length 1-2 and 2-4, possibly padded
			/// D numbered volumes: 2 or 3 numbers, length unknown, possibly padded				
						
			UrlGenerator gen = GetUrlGenerator ();			
			
			// generate whole list of pages
			var allPages = gen.Get (0, 100);
			
			List<string> urls;			
			if (!comic.IsImageFile) {
				// identify img tag index
				int imgIndex = UrlGenerator.IdentifyImg (gen.GenerateSome ());			
				urls = new List<string> ();
				foreach (var x in allPages) {					
					try {
						var imgs = WebUtils.GetImgs (WebUtils.GetSourceCode (x));	
						urls.Add (imgs [imgIndex]);
					} catch {	
						urls.Add (String.Empty);
					}
				}
				
			} else {	
				urls = allPages;				
			}			
			return urls.ToList ();
		}
				
		public UrlGenerator GetUrlGenerator ()
		{
			// start with the input URL
			var comic = new ComicUri (inputUrl);			
			// choose appropriate generator type
			var urlGen = ChooseGenerator (inputUrl);			
			
			if (!comic.IsImageFile) {
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

