using System;

namespace SequentialDownloader
{
	public class Range
	{
		public int Start { get; set; }

		public int End { get; set; }

		public int Increment { get; set; }
		
		public Range (int start, int end, int increment)
		{
			this.Start = start;
			this.End = end;
			this.Increment = increment;
		}
	}
}

