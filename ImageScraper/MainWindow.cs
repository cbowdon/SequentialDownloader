using System;
using System.IO;
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
		
	}
	
	protected void OnNumberValueChanged (object sender, System.EventArgs e)
	{
		
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
		
			if (chooser.Run () == (int)ResponseType.Ok) {			
				
				outputFileName = chooser.Filename;
				ScrapeButton.Label = "Cancel";
				chooser.Destroy ();
				
				if (model.ValidateInputs (inputUrl, outputFileName, number)) {
					
					model.TaskCompleted += delegate(object snd, EventArgs evt) {
						ScrapeButton.Label = "Scrape Images";
					};
					
					thread.Start ();					
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
}
