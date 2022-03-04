//using System;
//using CoreGraphics;
//using Foundation;
//using UIKit;
//using System.Collections.Generic;
//using WBid.WBidiPad.Core;
////using WBid.WBidiPad.iOS.Common;
//using WBid.WBidiPad.Model;

//namespace WBid.WBidiPad.iOS
//{
//	public class SummaryListController : UITableViewController
//	{
//		public string wblFileName;

//		public SummaryListController () : base (UITableViewStyle.Plain)
//		{
//			this.View.Frame = new CGRect (0, 0, 1024, 615);
//		}
//		public SummaryListController(IntPtr handle) : base (handle)
//		{
//		}
//		public override void DidReceiveMemoryWarning ()
//		{
//			// Releases the view if it doesn't have a superview.
//			base.DidReceiveMemoryWarning ();
			
//			// Release any cached data, images, etc that aren't in use.
//		}

//		public override void ViewDidLoad ()
//		{
//			base.ViewDidLoad ();
			
//			// Register the TableView's data source
//			//TableView.RegisterNibForCellReuse(UINib.FromName("summaryListCell", NSBundle.MainBundle), "summaryListCell");
//			TableView.Source = new SummaryListSource ();
//			//TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
//			TableView.SeparatorInset = new UIEdgeInsets (0, 3, 0, 3);
//			TableView.SeparatorColor = UIColor.FromRGB (220, 220, 220);
//			TableView.ContentInset = new UIEdgeInsets (0, 0, 5, 0);
//			TableView.ScrollIndicatorInsets = new UIEdgeInsets (0, 0, 5, 0);
//			this.SetEditing (true, false);
//			TableView.AllowsSelectionDuringEditing = true;

//		}
//	}
//}

