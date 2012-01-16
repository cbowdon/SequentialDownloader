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
		
		#region FindImgs
		/// <summary>
		/// Finds the src of all imgs in the page.
		/// </summary>
		/// <returns>
		/// Source of the imgs.
		/// </returns>
		public List<string> FindImgs (string source)
		{	
			var quoteChar = "(\"|')";
			var attrKey = "([A-Za-z0-9\\-]+)\\s*=\\s*";
			var attrValue = "([A-Za-z0-9\\-/:;&#!\\.\\?\\s]+)";
			// for some reason, Regex doesn't like OR used with lookbehind
			var srcBehindA = "(?<=src\\s*=\\s*\")";
			var srcBehindB = "(?<=src\\s*=\\s*')";
			var imgPattern = String.Format ("<img ({0}{2}{1}{2}\\s*)+\\s*/*>", attrKey, attrValue, quoteChar);
			var srcPattern = String.Format ("({0}{2}|{1}{2})", srcBehindA, srcBehindB, attrValue);
			
			var img = new Regex (imgPattern, RegexOptions.IgnoreCase);
			var src = new Regex (srcPattern, RegexOptions.IgnoreCase);
			
			var matches = img.Matches (source);	
			
			var ans = from Match m in matches
				let c = m.Captures [0].Value
				let s = src.Match (c)
				where s.Success
				select s.Groups [1].Value;
			
			return ans.ToList ();
		}
		
		public List<string> FindImgs ()
		{
			return FindImgs (HtmlSource);
		}
		#endregion
	
		#region IdentifyImg
		/// <summary>
		/// Identifies the image.
		/// </summary>
		/// <returns>
		/// The image index (from first page).
		/// </returns>
		/// <param name='pageUrls'>
		/// Page urls.
		/// </param>
		/// <param name='imgUrl'>
		/// Image URL (from first page).
		/// </param>
		/// <exception cref='NotImplementedException'>
		/// Is thrown when a requested operation is not implemented for a given type.
		/// </exception>
		public int IdentifyImg (IEnumerable<string> pageUrls, out string imgUrl)
		{			
			// fill array of source code
			var pageSources = pageUrls.Select<string,string> (x => WebUtils.GetSourceCode (x)).ToList ();			
			// get jagged list
			var pageImgs = pageSources.Select<string,List<string>> (x => FindImgs (x)).ToList ();
			
			// get jagged array
			var imgUrls = new string[pageUrls.Count ()][];
			for (int i = 0; i < pageUrls.Count(); i++) {				
				var source = pageSources [i];
				var fullImgUrls = FindImgs (source).Select<string, ComicUri> (x => new ComicUri (x));				
				var rightImgUrls = fullImgUrls.Select<ComicUri, string> (x => x.GetRightPart (UriPartial.Authority));
				imgUrls [i] = rightImgUrls.ToArray ();
			}
			
			// setup the list of possible indices
			var possibleIndices = Enumerable.Range (0, imgUrls [0].Length).ToList ();			
			
			// remove the index of any item that appears in more than one page
			for (int i = 0; i < imgUrls[0].GetLength(0); i++) {
				for (int j = 1; j < imgUrls.GetLength(0); j++) {
					if (imgUrls [j].Contains (imgUrls [0] [i])) {
						possibleIndices.Remove (i);
					}
				}
			}
			
			// if only one item left, remove it
			if (possibleIndices.Count == 1) {
				int index = possibleIndices.Single ();
				imgUrl = pageImgs [0] [index];
				return index;
			}
			
			// else choose the remaining item that shows MOST similarity to same item on next list			
			var scores = possibleIndices.Select<int, double> (p => WebUtils.CompareUrls (imgUrls [0] [p], imgUrls [1] [p])).ToList ();
			var topIndex = scores.IndexOf (scores.Max ());
			
			imgUrl = pageImgs [0] [topIndex];
			return topIndex;
		}
		
		public int IdentifyImg (IEnumerable<string> pageUrls)
		{
			string wasted;
			return IdentifyImg (pageUrls, out wasted);
		}
		#endregion
			
#warning Reimplement as Generator/IEnumerable<string> ?
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
						
			UrlGenerator gen;
			
			if (comic.Indices.Length == 1) {
				// option A or B
				
				var dateCount = new BlockDateCount (comic);
					
				// identify if it's a valid date
				if (dateCount.Format != DateType.NotRecognized) {					
					// B of some kind
					gen = dateCount;					
				} else {
					// A of some kind
					var seqCount = new SequentialCount (comic);
					gen = seqCount;
				}
			} else {
				// option C or D
				throw new NotImplementedException ();
			}
			
			// identify img tag index
			
			// generate whole list of pages
			var allPages = gen.GenerateLast100 ();
			
			// and get nth tag for each						
			// (map second (map FindImg, allPages))
			List<string> urls;			
			if (!comic.IsImageFile) {
				
				int imgIndex = IdentifyImg (gen.GenerateSome ());			
								
				urls = new List<string> ();
				
				foreach (var x in allPages) {					
					try {
						
						var imgs = FindImgs (WebUtils.GetSourceCode (x));	
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
		
#endregion
	}
}

