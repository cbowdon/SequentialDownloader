using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	public class ComicUri : Uri
	{
		public string[] Indices { get; private set; }
		
		public string Base { get; private set; }
		
		public ComicUri (string url) : base (url)
		{
			Parameterize ();
		}
		
		bool Parameterize ()
		{
			// remove the authority ; work on the right part
			string leftPart = GetLeftPart (UriPartial.Authority);
			string rightPart = AbsoluteUri.Substring (leftPart.Length);
			
			// match all numbers (or months or days) -> string[]
			var numRx = new Regex ("[0-9]+");
			var nums = numRx.Matches (rightPart);
			
			var inds = from Match n in nums
					where n.Success
					let c = n.Captures [0].Value
					select c;
			
			Indices = inds.ToArray ();
			if (Indices.Length == 0) {
				return false;
			}
			
			// replace each item in array with {i}, assign to base string
			var bBase = new StringBuilder (AbsoluteUri);
			for (int i = 0; i < Indices.Length; i++) {
				bBase.Replace (Indices [i], "{" + i + "}");
				Console.WriteLine (bBase);
			}
			Base = bBase.ToString ();
			
			return true;
		}
		
	}
}

