using System;
using System.IO;
using System.ComponentModel;
using System.Threading;
using Gtk;
using ImageScraper;

public partial class MainWindow: Gtk.Window
{	
	ModelController model;
	
	public MainWindow (ModelController model): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		this.model = model;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected void OnInputUrlChanged (object sender, System.EventArgs e)
	{
		if (!model.Active) {
			Reset ();	
		}		
	}
	
	protected void OnNumberValueChanged (object sender, System.EventArgs e)
	{
		if (!model.Active) {
			Reset ();	
		}		
	}
	
	protected void OnScrapeButtonClicked (object sender, System.EventArgs e)
	{		
		var number = int.Parse (NumberButton.Text);
		var inputUrl = UrlEntry.Text;
		string outputFileName;
		
		Thread thread = new Thread (model.RunTask);
		
		if (!model.Active) {
			
			var chooser = new FileChooserDialog ("Saves as...", 
		                                     this, 
		                                     FileChooserAction.Save, 
		                                     Stock.Save, 
		                                     ResponseType.Ok, 
		                                     Stock.Cancel, 
		                                     ResponseType.Cancel);
		
			// present dialog
			if (chooser.Run () == (int)ResponseType.Ok) {			
				
				outputFileName = chooser.Filename;
				ScrapeButton.Label = "Cancel";
				chooser.Destroy ();
				
				// validate and run
				if (model.ValidateInputs (inputUrl, outputFileName, number)) {
										
					model.TaskCompleted += OnTaskCompleted;
					model.FileProgress += OnIndividualProgressUpdate;
					model.FileDownloaded += OnOverallProgressUpdate;
					model.TaskCancelled += OnTaskCancelled;
					
					OverallProgressBar.Fraction = 0;
					
					thread.Start ();					
					
				} else {
					// handle this properly in future
					chooser.Destroy ();
				}
				
			} else {
				
				ScrapeButton.Label = "Scrape Images";
				chooser.Destroy ();
				return;
			}
			
		} else {
						
			AutoResetEvent auto = new AutoResetEvent (false);
			
			model.TaskCancelled += delegate(object snd, EventArgs evt) {
				auto.Set ();
			};
			
			model.CancelTask ();
			auto.WaitOne ();				
			
			if (thread.IsAlive) {
				thread.Join ();
			}
						
			
			ScrapeButton.Label = "Scrape Images";			
		}
	}

	protected void OnTaskCompleted (object sender, EventArgs e)
	{
		IndividualProgressBar.Fraction = 1.0;
		OverallProgressBar.Fraction = 1.0;
		ScrapeButton.Label = "Scrape Images";		
	}
	
	protected void OnTaskCancelled (object sender, EventArgs e)
	{		
		ScrapeButton.Label = "Scrape Images";		
	}
	
	protected void OnOverallProgressUpdate (object sender, EventArgs e)
	{
		IndividualProgressBar.Fraction = 1.0;
		double frac = (double)model.NumberDownloaded / (double)model.NumberToDownload;
		OverallProgressBar.Fraction = frac;
		IndividualProgressBar.Fraction = 0;
	}
	
	protected void OnIndividualProgressUpdate (object sender, ProgressChangedEventArgs e)
	{
		IndividualProgressBar.Fraction = e.ProgressPercentage / (double)100;
	}
	
	protected void Reset ()
	{
		IndividualProgressBar.Fraction = 0;
		OverallProgressBar.Fraction = 0;
		ScrapeButton.Label = "Scrape Images";
	}
	
	


}
