using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
    
	public class blockSortTableControllerController : UITableViewController
	{
        UIPopoverController popoverController;

		public blockSortTableControllerController () : base (UITableViewStyle.Plain)
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
			TableView.Source = new blockSortTableControllerSource ();
			this.SetEditing (true, false);
			TableView.AllowsSelectionDuringEditing = true;
			TableView.BackgroundColor = UIColor.Clear;
			TableView.TableFooterView = new UIView (CGRect.Empty);
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.SeparatorColor = ColorClass.ListSeparatorColor;

		}
       
	}
}

