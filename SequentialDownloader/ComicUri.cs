using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace ScraperLib
{
	public class ComicUri : Uri
	{
		
		#region Indices
		string[] indices;

		public string[] Indices {
			get {
				if (indices == null) {
					string rightPart = GetRightPart (UriPartial.Authority);
			
					// match all numbers (or months or days) -> string[]
					var numRx = new Regex ("[0-9]+");
					var nums = numRx.Matches (rightPart);
			
					var inds = from Match n in nums
					where n.Success
					let c = n.Captures [0].Value
					select c;
			
					indices = inds.ToArray ();
				}
				return indices;
			}
		}
		#endregion
		
		#region Base
		string uriBase;

		public string Base {
			get {
				if (uriBase == null) {
					var bBase = new StringBuilder (AbsoluteUri);
					for (int i = 0; i < Indices.Length; i++) {
						bBase.Replace (Indices [i], "{" + i + "}");
					}
					uriBase = bBase.ToString ();
				}
				return uriBase;
			}
		}
		#endregion
		
		#region IsFile
		public bool IsRemoteFile {
			get {
				var htmlExts = @"(.html)|(.shtml)|(.cshtml)|(.aspx)|(.php)|(.dhtml)|(.ars)|(\?[a-zA-Z0-9_]+=[a-zA-Z0-9_]+)";
				var docExts = @"(.png)|(.jpg)|(.gif)|(.jpeg)|(.bmp)|(.tif)|(.tiff)|(.doc)|(.docx)|(.xls)|(.xlsx)|(.ppt)|(.pptx)|(.txt)|(.dat)|(.zip)|(.cbz)|(.mp3)|(.mov)|(.avi)|(.mkv)|(.swf)|(.m4a)|(.m4v)|(.aac)|(.ogg)|(.wma)|(.wav)|(.mp4)|(.pdf)|(.rar)";
				var htmlRegex = new Regex (htmlExts, RegexOptions.IgnoreCase);
				var definiteHtml = htmlRegex.IsMatch (GetRightPart (UriPartial.Authority));
				var docRegex = new Regex (docExts, RegexOptions.IgnoreCase);
				var definiteDoc = docRegex.IsMatch (GetRightPart (UriPartial.Authority));
				if (definiteHtml) {
					return false;
				} else if (definiteDoc) {
					return true;
				} else {
					try {
						var firstText = WebUtils.DownloadPartial (AbsoluteUri);
						if (Regex.IsMatch (firstText.ToLower (), @"(<html)|(doctype html)")) {
							return false;
						} else {
							return true;
						}						
					} catch {
						return true;
					}
				}
				
			}
		}
		#endregion
		
		public ComicUri (string url) : base (url)
		{
		}
		
		public string GetRightPart (UriPartial uriPartial)
		{
			string leftPart = GetLeftPart (uriPartial);
			string rightPart = AbsoluteUri.Substring (leftPart.Length);
			return rightPart;
		}
		
	}
}

