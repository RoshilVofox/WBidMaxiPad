using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Linq;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public class PopoverTableViewControllerController : UITableViewController
	{
		public string PopType;
		public string SubPopType;
		public bool selectionEnabled;
        UIPopoverController popoverController;

		public PopoverTableViewControllerController () : base (UITableViewStyle.Plain)
		{
			ConstraintsApplied.MainList = ConstraintsApplied.MainList.OrderBy (x => x).ToList ();
			var item = ConstraintsApplied.MainList.FirstOrDefault ();
			if (item == "3-on-3-off") {
				ConstraintsApplied.MainList.Remove (item);
				ConstraintsApplied.MainList.Add (item);
			}
			WeightsApplied.MainList = WeightsApplied.MainList.OrderBy (x => x).ToList ();
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
			PopoverTableViewControllerSource tableSource = new PopoverTableViewControllerSource ();
			tableSource.PopType = PopType;
			tableSource.SubPopType = SubPopType;
			TableView.Source = tableSource;
			TableView.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);

         NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ReloadPopover"), reload);
		}

		public void reload(NSNotification n)
		{
			this.TableView.ReloadData();
		}

	}
}

