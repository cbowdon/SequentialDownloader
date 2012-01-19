using System;
using System.Collections.Generic;
using System.Linq;

namespace SequentialDownloader
{
	/// <summary>
	/// Abstract base class for SequentialCount and DateCount.
	/// Since SeqCount and DateCount are solutions to the same problem, a factory pattern
	/// seemed natural. Unfortunately they only share one signature so far: the constructor.
	/// </summary>
	public abstract class UrlGenerator
	{
		public abstract string Start { get; set; }
		
		public ComicUri Comic { get; protected set; }
		
		#region IsImageFile
		public bool IsImageFile { 
			get {
				return Comic.IsImageFile;
			}
		}
		#endregion
		
		#region ImgIndex
		private int _imgIndex = -1;

		public int ImgIndex {
			get {
				if (IsImageFile) {
					throw new ArgumentException ("{0} is an image file; it cannot have an index.", this.ToString ());
				} else {
					if (_imgIndex == -1) {
						var someUrls = GenerateSome ();
						_imgIndex = IdentifyImg (someUrls);
					}
					return _imgIndex;
				}				
			}			
		}
		#endregion
		
		#region Constructors
		protected UrlGenerator (ComicUri comic)
		{
			this.Comic = comic;
		}
		#endregion
		
		#region IdentifyImg
		public static int IdentifyImg (IEnumerable<string> pageUrls, out string imgUrl)
		{			
			// get jagged list
			var pages = pageUrls.ToList ();
			var pageImgs = pageUrls.Select<string,List<string>> (x => WebUtils.GetImgs (x)).ToList ();
			
			// get jagged array
			var imgUrls = new string[pageUrls.Count ()][];
			for (int i = 0; i < pageUrls.Count(); i++) {				
				var fullImgUrls = WebUtils.GetImgs (pages [i]).Select<string, ComicUri> (x => new ComicUri (x));				
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
		
		public static int IdentifyImg (IEnumerable<string> pageUrls)
		{
			string wasted;
			return IdentifyImg (pageUrls, out wasted);
		}
		#endregion
		
		public abstract List<string> GenerateSome ();
		
		public abstract List<string> Get (int startIndex, int num);
	}
}

