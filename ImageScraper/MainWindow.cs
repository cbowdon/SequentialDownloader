using System;
using System.IO;
using System.Threading;
using Gtk;
using ImageScraper;

public partial class MainWindow: Gtk.Window
{	
	ModelController model = new ModelController ();
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
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
				
				// re-initialize model and thread
				model = new ModelController (inputUrl, outputFileName, number);					
				thread = new Thread (model.RunTask);
				
				// start
				thread.Start ();				
			} else {
				
				ScrapeButton.Label = "Scrape Images";
				chooser.Destroy ();
				return;
			}
			
		} else {
			
			if (thread.IsAlive) {
				thread.Join ();
			}
						
			AutoResetEvent auto = new AutoResetEvent (false);
			
			model.TaskCancelled += delegate(object snd, EventArgs evt) {
				auto.Set ();
			};
			
			model.CancelTask ();
			auto.WaitOne ();
			
			ScrapeButton.Label = "Scrape Images";
			
		}
		
		
		
	}
	
	protected void RegisterEvents ()
	{
		
	}

}
