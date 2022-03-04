using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class SmallPopoverTableController : UITableViewController
	{
		public string PopType;
		public string SubPopType;
		public int index;

		public SmallPopoverTableController () : base (UITableViewStyle.Plain)
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

			SmallPopoverTableSource tableSource = new SmallPopoverTableSource ();
			tableSource.PopType = PopType;
			tableSource.SubPopType = SubPopType;
			tableSource.index = index;
			TableView.Source = tableSource;
			TableView.SeparatorInset = new UIEdgeInsets (0, 10, 0, 10);
			TableView.TableFooterView = new UIView (CGRect.Empty);
		}
	}
}

