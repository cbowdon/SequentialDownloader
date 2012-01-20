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
		this.controller.FileDownloaded += delegate(object sender, EventArgs e) {
			var num = (double)this.controller.NumberDownloaded;
			var den = (double)this.controller.NumberToDownload;
			ScrapeProgressBar.Fraction = num / den;
		};
		this.controller.AllFilesDownloaded += delegate(object sender, EventArgs e) {
			ScrapeProgressBar.Fraction = 1.0;
			ScrapeButton.Label = "Scrape Images!";
		};
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
				                   Stock.Cancel, 
				                   ResponseType.Cancel, 
				                   Stock.Save, 
				                   ResponseType.Ok);
			if (saveDialog.Run () == (int)ResponseType.Ok) {				
				outputFileName = saveDialog.Filename;
				controller.OutputFileName = outputFileName;
				saveDialog.Destroy ();
			} else {			
				saveDialog.Destroy ();
				ScrapeButton.Label = "Scrape Images!";
				return;
			}   
			
			thread.Start ();
			
		} else {
			if (thread.IsAlive) {				
				controller.AfterDownloading ();
				thread.Join ();				
			}
			ScrapeProgressBar.Fraction = 0;
			ScrapeButton.Label = "Scrape Images!";
		}	
	}

	protected void OnValueChanged (object sender, System.EventArgs e)
	{
		controller.NumberToDownload = (int)NumberButton.Value;
	}
}
