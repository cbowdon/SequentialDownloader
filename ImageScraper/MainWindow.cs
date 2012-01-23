using System;
using System.IO;
using System.ComponentModel;
using System.Threading;
using Gtk;
using ImageScraper;

public partial class MainWindow: Gtk.Window
{	
	ModelController model;
	string overallProgress = "Overall Progress";
	
	public MainWindow (ModelController model): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		this.model = model;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (model.Active) {
			model.CancelTask ();
		}
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
			
			if (SaveDialog (out outputFileName)) {
				
				ScrapeButton.Label = "Cancel";
				
				// validate and run
				if (model.ValidateInputs (inputUrl, outputFileName, number)) {
										
					model.TaskCompleted += OnTaskCompleted;
					model.FileProgress += OnIndividualProgressUpdate;
					model.FileDownloaded += OnOverallProgressUpdate;
					model.TaskCancelled += OnTaskCancelled;
					
					OverallProgressBar.Fraction = 0;

					thread.Start ();					
					IndividualProgressLabel.Text = String.Format ("Individual Progress: {0}", model.Status.ToLower ());
				} 
				
			} else {
				
				return;
			}
			
		} else {
						
			model.CancelTask ();
			
			if (thread.IsAlive) {
				thread.Join ();
			}
						
			ScrapeButton.Label = "Scrape Images";			
		}
	}

	protected void OnTaskCompleted (object sender, EventArgs e)
	{
		IndividualProgressBar.Fraction = 1.0;
		IndividualProgressLabel.Text = String.Format ("Individual Progress: {0}", model.Status.ToLower ());
		OverallProgressBar.Fraction = 1.0;
		OverallProgressLabel.Text = String.Format ("{0}: 100%", overallProgress);
		ScrapeButton.Label = "Scrape Images";		
	}
	
	protected void OnTaskCancelled (object sender, EventArgs e)
	{	
		IndividualProgressLabel.Text = String.Format ("Individual Progress: {0}", model.Status.ToLower ());
		ScrapeButton.Label = "Scrape Images";		
	}
	
	protected void OnOverallProgressUpdate (object sender, EventArgs e)
	{
		if (model.Active) {
			// 100% individual progress
			IndividualProgressBar.Fraction = 1.0;		
			IndividualProgressLabel.Text = String.Format ("Individual Progress: {0}", model.Status.ToLower ());
		
			// update overall progress
			double frac = (double)model.NumberDownloaded / (double)model.NumberToDownload;
			OverallProgressBar.Fraction = frac;
			OverallProgressLabel.Text = String.Format ("{0}: {1}%", overallProgress, Math.Round (100 * frac));
		
			// 0% individual progress
			IndividualProgressBar.Fraction = 0;	
		}
	}
	
	protected void OnIndividualProgressUpdate (object sender, ProgressChangedEventArgs e)
	{
		if (model.Active) {
			IndividualProgressBar.Fraction = e.ProgressPercentage / (double)100;	
		}
		
	}
	
	protected void Reset ()
	{
		IndividualProgressBar.Fraction = 0;
		IndividualProgressLabel.Text = "Individual Progress";
		OverallProgressBar.Fraction = 0;
		OverallProgressLabel.Text = "Overall Progress";
		ScrapeButton.Label = "Scrape Images";
	}
	
	protected bool SaveDialog (out string outputFileName)
	{
		var chooser = new FileChooserDialog ("Saves as...", 
		                                     this, 
		                                     FileChooserAction.Save, 
		                                     Stock.Save, 
		                                     ResponseType.Ok, 
		                                     Stock.Cancel, 
		                                     ResponseType.Cancel);
		
		// present dialog
		var ans = chooser.Run () == (int)ResponseType.Ok;				
		var temp = chooser.Filename;
		chooser.Destroy ();
		
		if (!CanWriteFile (temp)) {
			return SaveDialog (out outputFileName);
		}
		
		outputFileName = temp;
		return ans;
	}
	
	protected bool CanWriteFile (string fileName)
	{
		if (File.Exists (fileName)) {
			var md = new MessageDialog (this, 
			                            DialogFlags.DestroyWithParent, 
			                            MessageType.Question, 
			                            ButtonsType.YesNo, 
			                            "File already exists! Do you want to over-write it?");
			var result = (ResponseType)md.Run ();
			var ans = result == ResponseType.Yes;
			md.Destroy ();
			return ans;
		} else {
			return true;
		}
	}
	
	


}
