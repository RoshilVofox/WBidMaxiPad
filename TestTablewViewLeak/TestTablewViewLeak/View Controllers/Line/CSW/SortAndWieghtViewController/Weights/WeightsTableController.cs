using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class WeightsTableController : UITableViewController
	{
		public WeightsTableController () : base (UITableViewStyle.Plain)
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
			TableView.Source = new WeightsTableSource ();
			this.TableView.BackgroundColor = UIColor.Clear;
			//this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.SeparatorColor = ColorClass.ListSeparatorColor;
			this.TableView.DelaysContentTouches = false;
			TableView.TableFooterView = new UIView (CGRect.Empty);
			TableView.ScrollIndicatorInsets = new UIEdgeInsets (0, 0, 0, -2);
		}
	}
}

