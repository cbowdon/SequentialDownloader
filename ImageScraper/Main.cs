using System;
using Gtk;

namespace ImageScraper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			Controller controller = new Controller ();
			MainWindow win = new MainWindow (controller);
			win.Show ();
			Application.Run ();
		}
	}
}
