using System;

namespace SequentialDownloader
{
	public interface ICountingRule
	{
		string[] Generate();
	}	
}

