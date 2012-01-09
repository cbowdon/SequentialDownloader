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
					htmlSource = GetSourceCode (inputUrl);
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
		public List<string> FindImgs ()
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
			
			var matches = img.Matches (HtmlSource);	
			
			var ans = from Match m in matches
				let c = m.Captures [0].Value
				let s = src.Match (c)
				where s.Success
				select s.Groups [1].Value;
			
			return ans.ToList ();
		}
		
		public static string GetSourceCode (string url)
		{
			Func <string, string> getSource = x => {
				using (var client = new WebClient()) {
					client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
					return client.DownloadString (x);	
				}
				// or
//				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);					
//				request.Method = "GET";
//				using (WebResponse response = request.GetResponse()) {
//					using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
//						string result = reader.ReadToEnd ();
//					}
//				}
			};
			
			var errorString = String.Format ("GetSourceCode({0}): time-out exception", url);
			
			for (int i = 0; i < 5; i++) {
				try {
					return getSource (url);
				} catch {
					continue;
				}
			}
			
			throw new TimeoutException (errorString);
		}
		
		public List<string> FindUrls ()
		{
			// take ComicUri(url)
			
			// categorize: date / normal
			
			// decrement highest index accordingly
			
			// test for real url
			throw new NotImplementedException ();
		}
		
		public static bool UrlExists (string url)
		{
			try {
				HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
				request.Method = "HEAD";
				HttpWebResponse response = request.GetResponse () as HttpWebResponse;
				bool ans = response.StatusCode == HttpStatusCode.OK;
				response.Close ();
				return ans;
			} catch {
				return false;
			}
		}
#endregion
	}
}

