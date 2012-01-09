using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	public class ComicUri : Uri
	{
		
		#region Indices
		string[] indices;

		public string[] Indices {
			get {
				if (indices == null) {
					string leftPart = GetLeftPart (UriPartial.Authority);
					string rightPart = AbsoluteUri.Substring (leftPart.Length);
			
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
						Console.WriteLine (bBase);
					}
					uriBase = bBase.ToString ();
				}
				return uriBase;
			}
		}
		#endregion
		
		public ComicUri (string url) : base (url)
		{
		}
	}
}

