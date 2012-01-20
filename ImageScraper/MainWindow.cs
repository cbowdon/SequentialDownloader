using System;
using System.Threading;
using Gtk;
using ImageScraper;

public partial class MainWindow: Gtk.Window
{	
	Controller controller;
	
	public MainWindow (Controller controller): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		this.controller = controller;
		this.controller.NumberToDownload = int.Parse (NumberButton.Text);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnChanged (object sender, System.EventArgs e)
	{
		controller.InputUrl = UrlEntry.Text;
		StartEntry.Text = controller.Start;
	}
	
	protected void OnScrapeButtonClicked (object sender, System.EventArgs e)
	{		
		Thread thread = new Thread (controller.BeginDownloading);
		if (controller.State == Controller.AppState.NotScraping) {
			ScrapeButton.Label = "Cancel";
			string outputFileName;
			FileChooserDialog saveDialog = 
			new FileChooserDialog ("Save as...", 
			                       this, 
			                       FileChooserAction.Save, 
			                       "Save", 
			                       ResponseType.Apply, 
			                       "Cancel", 
			                       ResponseType.Cancel);
		
			if (saveDialog.Run () == (int)ResponseType.Apply) {
				outputFileName = saveDialog.Filename;
				controller.OutputFileName = outputFileName;
			} else {
				return;
			}
		
			saveDialog.Destroy ();
			
			controller.BeginDownloading ();
			ScrapeButton.Label = "Scrape Images!";

		
			Console.WriteLine (controller.State.ToString ());
		} else {
			ScrapeButton.Label = "Scrape Images!";
		}	
	}

	protected void OnValueChanged (object sender, System.EventArgs e)
	{
		controller.NumberToDownload = (int)NumberButton.Value;
	}
}
