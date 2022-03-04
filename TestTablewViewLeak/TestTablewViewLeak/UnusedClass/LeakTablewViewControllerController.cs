
using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak
{
	[Register ("LeakTablewViewControllerController")]
	public partial class LeakTablewViewControllerController : UITableViewController
	{
		public LeakTablewViewControllerSource objsource = new LeakTablewViewControllerSource();
		public LeakTablewViewControllerController () : base (UITableViewStyle.Grouped)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source

			TableView.Source = objsource;
			objsource.tablView = TableView;

		}


		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			//TableView.IDisposable();


//			TableView.DataSource = null;
//			objsource.Dispose ();
			//TableView.Dispose ();
		}



	}
}

