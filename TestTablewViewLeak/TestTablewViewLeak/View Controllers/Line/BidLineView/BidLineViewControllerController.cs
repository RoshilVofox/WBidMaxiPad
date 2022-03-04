//using System;
//using CoreGraphics;
//using Foundation;
//using UIKit;
//using System.Collections.Generic;

//namespace WBid.WBidiPad.iOS
//{
//	public class BidLineViewControllerController : UITableViewController
//	{
//		public string wblFileName;

//		public BidLineViewControllerController () : base (UITableViewStyle.Plain)
//		{
//			this.View.Frame = new CGRect (0, 0, 1024, 660);

//		}

//		public override void DidReceiveMemoryWarning ()
//		{
//			// Releases the view if it doesn't have a superview.
//			base.DidReceiveMemoryWarning ();
			
//			// Release any cached data, images, etc that aren't in use.
//		}
//		public BidLineViewControllerController(IntPtr handle) : base (handle)
//		{
//		}


//		public override void ViewDidLoad ()
//		{
//			base.ViewDidLoad ();

//			// Register the TableView's data source
//			TableView.Source = new BidLineViewControllerSource ();
//			TableView.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);
//			TableView.SeparatorColor = UIColor.FromRGB (220, 220, 220);
//			TableView.ContentInset = new UIEdgeInsets (0, 0, 5, 0);
//			TableView.ScrollIndicatorInsets = new UIEdgeInsets (0, 0, 5, 0);
//			this.Editing = true;
//			TableView.AllowsSelectionDuringEditing = true;

//		}
//	}
//}

