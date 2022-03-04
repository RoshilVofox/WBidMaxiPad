using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class HelpViewController : UIViewController
	{
		string[] arrDocuments = {
			"Constraints",
			"Weights",
			"Sorts",
            "Select-Move-Lock-Promote-Trash",
			"Column Headings",
			"EOM - End of Month",
			"OVR – MTM Overlap",
			"VAC - Vacation Pilots",
			"VAC – Vacation Flt Att",
			"Smart Sync",
			"Quicksets",
			"MIL Button",
			"Toolbar Controls"
		};
		string strSelDoc;

		string[] arrVideos = {
			"Download Bid Data",
			"Four Views of WBid",
			"Simple Pilot Bid with Constraints",
			"Submit Flt Att Bid 1st Round",
			"Bid Automator"
		};
		string[] arrVideoIDs = {
			"uRvOJvefjfQ",
			"Ux6ryrXpBOk",
			"xJCJNqSsrgY",
			"U37T4SSCSN8",
			"U2OqcVkqtBM"
		};
		//string strSelVid;
		string strSelVidID;

		public string pdfFileName;
		public CGPoint pdfOffset = CGPoint.Empty;
		public int selectRow = 0;

		public HelpViewController () : base ("HelpViewController", null)
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
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			try
			{
			navBar.Items [0].Title = "Help ( " + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + " )";
			tblSideView.Layer.BorderWidth = 1;
			tblSideView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			tblSideView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			tblSideView.Source = new SideMenuSource (this);

			wbHelpView.Layer.BorderWidth = 1;
			wbHelpView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;

			wbHelpView.Frame = new CGRect (340, 84, 664, 664);
			tblSideView.SelectRow (NSIndexPath.FromRowSection (selectRow, 0), false, UITableViewScrollPosition.None);
			string fileName = "Help Files/" + pdfFileName + ".pdf";
			string localDocUrl = Path.Combine (NSBundle.MainBundle.ResourcePath, fileName);
			wbHelpView.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));

			btnFullScrn.SetBackgroundImage (UIImage.FromBundle ("full-enter.png"), UIControlState.Normal);
			btnFullScrn.SetBackgroundImage (UIImage.FromBundle ("full-exit.png"), UIControlState.Selected);
		

			this.PerformSelector (new ObjCRuntime.Selector ("ScrollToPage"), null, 1);
			}
				catch(Exception e)
            {
                throw e;
            }
            //			wbHelpView.ScrollView.DraggingEnded += (object sender, DraggingEventArgs e) => {
            //				Console.WriteLine(wbHelpView.ScrollView.ContentOffset.Y.ToString());
            //			};
        }

        [Export("ScrollToPage")]
		void ScrollToPage ()
		{
			wbHelpView.ScrollView.SetContentOffset (pdfOffset, false);
			pdfOffset = CGPoint.Empty;
		}

		partial void btnDoneTapped (UIKit.UIBarButtonItem sender)
		{
			wbHelpView.LoadHtmlString (string.Empty, null);
			this.NavigationController.DismissViewController (true, null);
		}

		partial void btnFullScrnTapped (UIKit.UIButton sender)
		{
			try
			{
			sender.Selected = !sender.Selected;

			if (btnFullScrn.Selected) {
					DetailViewLeftConstraint.Constant = 20;
					//wbHelpView.Frame = new CGRect (0, 20, this.View.Frame.Width, this.View.Frame.Height);
				btnFullScrn.Frame = new CGRect (964, 94, 30, 30);
			} else {
					DetailViewLeftConstraint.Constant = 340;
					//wbHelpView.Frame = new CGRect (340, 84, 664, 664);
					btnFullScrn.Frame = new CGRect (964, 94, 30, 30);
			}
		//	wbHelpView.Reload ();
			}
			catch(Exception e)
			{
				
			}
		}

		partial void sgDocVidChanged (UIKit.UISegmentedControl sender)
		{
			tblSideView.Tag = sender.SelectedSegment;
			tblSideView.ReloadData ();
			tblSideView.SelectRow (NSIndexPath.FromRowSection (0, 0), false, UITableViewScrollPosition.None);
			wbHelpView.LoadHtmlString (string.Empty, null);

			if (tblSideView.Tag == 0) {
				btnFullScrn.Hidden = false;
				wbHelpView.Frame = new CGRect (340, 84, 664, 664);
				string fileName = "Help Files/" + arrDocuments [0] + ".pdf";
				string localDocUrl = Path.Combine (NSBundle.MainBundle.ResourcePath, fileName);
				wbHelpView.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
			} else {
				btnFullScrn.Hidden = true;
				if (Reachability.IsHostReachable ("www.youtube.com")) {
					wbHelpView.Frame = new CGRect (340, 166, 664, 500);
					string loadStr = "<iframe width=\"648\" height=\"486\" src=\"http://www.youtube.com/embed/" + arrVideoIDs [0] + "?rel=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
					wbHelpView.LoadHtmlString (loadStr, null);
					wbHelpView.ScrollView.ScrollEnabled = false;
					wbHelpView.ScrollView.SetZoomScale (0, false);
				} else {
					wbHelpView.LoadHtmlString (string.Empty, null);
				
                    UIAlertController okAlertController = UIAlertController.Create("Error", "No Internet connection found. Please try again later.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
			}
		}

		public class SideMenuSource : UITableViewSource
		{

			HelpViewController parentVC;

			public SideMenuSource (HelpViewController parent)
			{
				parentVC = parent;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (tableview.Tag == 0) {
					return parentVC.arrDocuments.Length;
				} else {
					return parentVC.arrVideos.Length;
				}
			}

			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 47;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cellIdentifier");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();

				cell.SelectedBackgroundView = new UIImageView (UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)));
				cell.TextLabel.TextColor = UIColor.DarkGray;
				cell.TextLabel.HighlightedTextColor = UIColor.White;
				cell.TextLabel.Font = UIFont.SystemFontOfSize (15);

				if (tableView.Tag == 0) {
					cell.TextLabel.Text = parentVC.arrDocuments [indexPath.Row];
				} else {
					cell.TextLabel.Text = parentVC.arrVideos [indexPath.Row];
				}

				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (tableView.Tag == 0) {
					parentVC.btnFullScrn.Hidden = false;
					parentVC.wbHelpView.Frame = new CGRect (340, 84, 664, 664);
					parentVC.strSelDoc = parentVC.arrDocuments [indexPath.Row];
					string fileName = "Help Files/" + parentVC.strSelDoc + ".pdf";
					string localDocUrl = Path.Combine (NSBundle.MainBundle.ResourcePath, fileName);
					parentVC.wbHelpView.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
				} else {
					parentVC.btnFullScrn.Hidden = true;
					if (Reachability.IsHostReachable ("www.youtube.com")) {
						parentVC.wbHelpView.Frame = new CGRect (340, 166, 664, 500);
						//parentVC.strSelVid = parentVC.arrVideos [indexPath.Row];
						parentVC.strSelVidID = parentVC.arrVideoIDs [indexPath.Row];
						string loadStr = "<iframe width=\"648\" height=\"486\" src=\"http://www.youtube.com/embed/" + parentVC.strSelVidID + "?rel=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
						parentVC.wbHelpView.LoadHtmlString (loadStr, null);
						parentVC.wbHelpView.ScrollView.ScrollEnabled = false;
						parentVC.wbHelpView.ScrollView.SetZoomScale (0, false);
					} else {
						parentVC.wbHelpView.LoadHtmlString (string.Empty, null);
                        UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                        WindowAlert.RootViewController = new UIViewController();
                        UIAlertController okAlertController = UIAlertController.Create("Error", "No Internet connection found. Please try again later.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        WindowAlert.Dispose();  


                    }
                }
			}

		}
	}
}

