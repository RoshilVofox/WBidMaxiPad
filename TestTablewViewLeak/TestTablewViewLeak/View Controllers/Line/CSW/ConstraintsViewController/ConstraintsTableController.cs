using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public class ConstraintsTableController : UITableViewController
	{
		public ConstraintsTableController () : base (UITableViewStyle.Plain)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);



//			foreach (UIView view in this.View.Subviews) {
//
//				DisposeClass.DisposeEx(view);
//			}
//			this.View.Dispose ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source
			TableView.Source = new ConstraintsTableSource ();
			this.TableView.BackgroundColor = UIColor.Clear;
			//this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.SeparatorColor = ColorClass.ListSeparatorColor;
			this.TableView.DelaysContentTouches = false;
			this.TableView.ScrollEnabled = false;
			TableView.TableFooterView = new UIView (CGRect.Empty);
		}
	}
}

