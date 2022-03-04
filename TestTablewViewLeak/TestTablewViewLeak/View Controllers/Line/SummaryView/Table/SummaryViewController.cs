using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public partial class SummaryViewController : UITableViewController
	{
		public string wblFileName;
		SummaryListSource TableSource;
		NSObject Notif;
		public SummaryViewController() : base (UITableViewStyle.Plain)
		{
			//this.View.Frame = new CGRect(0, 0, 1024, 615);
		}
		public SummaryViewController(IntPtr handle) : base (handle)
		{
		}
		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}
		//public void ReloadTable  (NSNotification n)
		//{
		//	this.TableView.SetNeedsLayout();
		//	this.TableView.ReloadData();
		//	this.TableView.ReloadInputViews();

		//}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			TableSource = new SummaryListSource();
			// Register the TableView's data source
			//TableView.RegisterNibForCellReuse(UINib.FromName("summaryListCell", NSBundle.MainBundle), "summaryListCell");

			//Notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ReloadTableview"), ReloadTable);
			TableView.Source = TableSource;
			//TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			TableView.SeparatorInset = new UIEdgeInsets(0, 3, 0, 3);
			TableView.SeparatorColor = UIColor.FromRGB(220, 220, 220);
			TableView.ContentInset = new UIEdgeInsets(0, 0, 5, 0);
			TableView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 5, 0);
			this.SetEditing(true, false);
			TableView.AllowsSelectionDuringEditing = true;

			CommonClass.lineVC.sumList =(SummaryViewController) this;


		}
		public void reloadData()
		{
			TableSource.clearData = true;
			TableView.ReloadData();

			PerformSelector(new ObjCRuntime.Selector("reloadActualData"), null, 0.05);


		}
		[Export("reloadActualData")]
		public void reloadActualData()
		{
			
			TableView.ReloadData();
            TableView.LayoutSubviews ();

            //cell.SetNeedsDisplay();


		}


	}
}



