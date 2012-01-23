using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;

namespace ImageScraperLib
{
	public static class WebUtils
	{
		/// <summary>
		/// Gets the source code.
		/// </summary>
		/// <returns>
		/// The source code.
		/// </returns>
		/// <param name='url'>
		/// URL.
		/// </param>
		/// <exception cref='TimeoutException'>
		/// Is thrown if site can't be reached.
		/// </exception>
		public static string GetSourceCode (string url)
		{
			string userAgent = "Mozilla/5.0 (Windows NT 6.2; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
//			string userAgentHonest = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
			Func <string, string> getSource = x => {
//				using (var client = new WebClient()) {
//					client.Headers.Add ("user-agent", userAgent);
//					return client.DownloadString (x);	
//				}
				// or
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);					
				request.UserAgent = userAgent;
				request.Method = "GET";
				using (WebResponse response = request.GetResponse()) {
					using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
						return reader.ReadToEnd ();
					}
				}
			};
			
			var errorString = String.Format ("GetSourceCode({0}): time-out exception", url);
			
			for (int i = 0; i < 10; i++) {
				try {
					return getSource (url);
				} catch (TimeoutException) {
					continue;						
				}
			}
			
			throw new TimeoutException (errorString);
		}
		
		/// <summary>
		/// Extract one img src from a page
		/// </summary>
		/// <returns>
		/// The image.
		/// </returns>
		/// <param name='url'>
		/// URL.
		/// </param>
		/// <param name='index'>
		/// Index.
		/// </param>
		public static string GetImg (string url, int index)
		{
			var imgUrls = GetImgs (url);
			return imgUrls [index];
		}
		
		/// <summary>
		/// Gets all img src tags from the page.
		/// </summary>
		/// <returns>
		/// The imgs.
		/// </returns>
		/// <param name='url'>
		/// URL.
		/// </param>
		public static List<string> GetImgs (string url)
		{			
			var originalUrl = new Uri (url);
			var source = GetSourceCode (url);
			var quoteChar = "(\"|')";
			var attrKey = "([A-Za-z0-9\\-]+)\\s*=\\s*";
			var attrValue = "([A-Za-z0-9\\-/:;&#!_\\.\\?\\s]+)";
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
			
			Func<string,bool> isAbsoluteUri = x => {
				var uri = new Uri (x);
				return uri.GetLeftPart (UriPartial.Scheme) == "http://";	
			};
			
			var fullAns = new List<string> ();
			foreach (var x in ans) {
				if (isAbsoluteUri (x)) {
					fullAns.Add (x);
				} else {
					fullAns.Add (String.Format ("{0}{1}", originalUrl.GetLeftPart (UriPartial.Authority), x));					
				}
			}
			
			return fullAns.ToList ();
		}
		
		public static bool UrlExists (string url)
		{
			int i = 0;
			while (i < 3) {
				try {
					HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
					request.Method = "HEAD";
					HttpWebResponse response = request.GetResponse () as HttpWebResponse;
					bool ans = response.StatusCode == HttpStatusCode.OK;
					response.Close ();
					return ans;
				} catch {
					i++;
				}
			}
			return false;
		}
	
		/// <summary>
		/// Compares two URLs
		/// </summary>
		/// <returns>
		/// Score representing similarity (higher is closer)
		/// </returns>
		/// <param name='u1'>
		/// U1.
		/// </param>
		/// <param name='u2'>
		/// U2.
		/// </param>
		public static double CompareUrls (string u1, string u2)
		{
			var u1ext = u1.Substring (0, u1.Length - 4);
			var u2ext = u2.Substring (0, u2.Length - 4);
						
			var u1c = u1ext.ToCharArray ();
			var u2c = u2ext.ToCharArray ();
					
			double score = 0;
			for (int i = 0; i < Math.Min (u1c.Length, u2c.Length); i++) {
				if (u1c [i] == u2c [i]) {
					score += 100;
				} else {
					score += 100 - Math.Abs ((int)u1c [i] - (int)u2c [i]);
				}
			}
			
			score /= (100 * Math.Min (u1c.Length, u2c.Length));
			
			return score;
		}
		
		/// <summary>
		/// Gets the size of remote file - just for reference.
		/// </summary>
		/// <returns>
		/// The size.
		/// </returns>
		/// <param name='fileUrl'>
		/// File URL.
		/// </param>
		public static long GetSize (string fileUrl)
		{
			Uri url = new Uri (fileUrl);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
			request.Method = "GET";
			using (WebResponse response = request.GetResponse()) {
				// gets the size of the file in bytes
				return response.ContentLength; 
			}
		}
		
		/// <summary>
		/// Downloads the file - just for reference
		/// </summary>
		/// <returns>
		/// The file.
		/// </returns>
		/// <param name='url'>
		/// URL.
		/// </param>
		public static byte[] DownloadFile (string url)
		{
			// http://odetocode.com/blogs/scott/archive/2004/10/04/webrequest-and-binary-data.aspx
			byte[] result;
			byte[] buffer = new byte[4096];
 
			WebRequest wr = WebRequest.Create (url);
 
			using (WebResponse response = wr.GetResponse()) {
				using (Stream responseStream = response.GetResponseStream()) {
					using (MemoryStream memoryStream = new MemoryStream()) {
						int count = 0;
						do {
							count = responseStream.Read (buffer, 0, buffer.Length);
							memoryStream.Write (buffer, 0, count);
 
						} while(count != 0);
 
						result = memoryStream.ToArray ();
						return result;
					}
				}
			}
		}
	}
}

