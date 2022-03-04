using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public class BAblockSortTableControllerController : UITableViewController
	{
		public SortTempValues BASort ;
		public BAblockSortTableControllerController (SortTempValues Sort) : base (UITableViewStyle.Plain)
		{
			BASort = Sort;
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
			TableView.Source = new BAblockSortTableControllerSource (BASort);
			this.SetEditing (true, false);
			TableView.AllowsSelectionDuringEditing = true;
			TableView.BackgroundColor = UIColor.Clear;
			TableView.TableFooterView = new UIView (CGRect.Empty);
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.SeparatorColor = ColorClass.ListSeparatorColor;

		}
	}
}

