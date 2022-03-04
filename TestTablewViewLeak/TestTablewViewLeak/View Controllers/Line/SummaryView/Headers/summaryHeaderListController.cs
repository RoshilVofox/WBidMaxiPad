using System;
using CoreGraphics;
using Foundation;
using UIKit;


namespace WBid.WBidiPad.iOS
{
	public class summaryHeaderListController : UITableViewController
	{
		NSObject summaryheadNotif;
		public summaryHeaderListController () : base (UITableViewStyle.Plain)
		{
			if (this != null) {
				const double M_PI = 3.14159265358979323846;
				const float k90DegreesCounterClockwiseAngle = (float) -(90 * M_PI / 180.0);
				CGRect frame = new CGRect(0,0,900,45);
				CGAffineTransform transform = CGAffineTransform.MakeRotation (k90DegreesCounterClockwiseAngle);
				this.View.Transform = transform;
				this.View.Frame = frame;
				this.View.Layer.BorderWidth = 1;
				this.View.Layer.BorderColor = UIColor.FromRGB (158, 179, 131).CGColor;

//				UIView vwFooter = new UIView (new RectangleF(0, 0, 45, 100));
//				vwFooter.BackgroundColor = UIColor.White;
//				vwFooter.Layer.BorderWidth = 1;
//				string[] title = { "SEL", "MOV" };
//				for (int i = 0; i <= 1; i++) {
//					UIImageView imgBack = new UIImageView (new RectangleF (0, 0+i*50, 45, 50));
//					imgBack.Layer.BorderWidth = 1;
//					vwFooter.AddSubview (imgBack);
//					UILabel lblTitle = new UILabel (new RectangleF(0, 0+i*50, 50, 50));
//					lblTitle.Font = UIFont.BoldSystemFontOfSize (12);
//					lblTitle.TextAlignment = UITextAlignment.Center;
//					lblTitle.TextColor = UIColor.FromRGB (222, 54, 37);
//					lblTitle.Text = title[i];
//					vwFooter.AddSubview (lblTitle);
//					lblTitle.Transform = CGAffineTransform.MakeRotation((float) M_PI / 2);
//					//lblTitle.Center = imgBack.Center;
//				}
//				TableView.TableFooterView = vwFooter;
			}
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
			//TableView.RegisterNibForCellReuse(UINib.FromName("summaryHeaderCell", NSBundle.MainBundle), "summaryHeaderCell");
			TableView.Source = new summaryHeaderListSource ();
			TableView.ScrollEnabled = false;
			this.addGestures ();
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			//CommonClass.lineVC.hTable = (summaryHeaderListController)this;
			summaryheadNotif =NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("disablesummaryheaderediting"), handledisable);
		}
		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver(summaryheadNotif);

		}

//		public override void ViewDidAppear (bool animated)
//		{
//			base.ViewDidAppear (animated);
//			TableView.Editing = true;
//		}

		#region Getures
		public void addGestures ()
		{
			// Long press gesture to enable and disable sorting of columns.

 
			this.addLongPressGesture ();
		}

		public void addLongPressGesture()
		{
			
			UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(handleLongPress);
			this.View.AddGestureRecognizer (longPress);
		}
		public void handleLongPress(UILongPressGestureRecognizer gest) 
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("Dismisstextfield", null);
			if (gest.State == UIGestureRecognizerState.Began) {
				this.SetEditing (true, false);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);

			}
		}
		public void handledisable(NSNotification n)
		{
			this.SetEditing(false, false);

		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 50;
		}

		#endregion
	}
}

