using System;
using Gtk;

namespace ImageScraper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			ModelController model = new ModelController ();
			MainWindow win = new MainWindow (model);
			win.Show ();
			Application.Run ();
		}
	}
}
