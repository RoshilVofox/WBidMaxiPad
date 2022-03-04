using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.SharedLibrary;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
//using WBid.WBidiPad.PortableLibrary.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
	public class BidLineViewControllerSource : UITableViewSource
	{
		public BidLineViewControllerSource ()
		{
		}
		UITableView TableView;
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			TableView = tableview;
			// TODO: return the actual number of items in the section
			return GlobalSettings.Lines.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.RegisterNibForCellReuse(UINib.FromName("BidLineViewCell", NSBundle.MainBundle), "BidLineViewCell");
			BidLineViewCell cell = (BidLineViewCell)tableView.DequeueReusableCell(new NSString("BidLineViewCell"), indexPath);
			cell.bindData (GlobalSettings.Lines [indexPath.Row], indexPath);
			if (indexPath.Row % 2==0)
				cell.BackgroundColor = UIColor.FromRGB (255, 255, 255);
			else
				cell.BackgroundColor = UIColor.FromRGB (249, 249, 249);
			return cell;
		}
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopHide", null);
			if (CommonClass.selectedTrip != null) {
				CommonClass.selectedTrip = null;
				tableView.ReloadData ();
			}
		}
		public override void DecelerationEnded(UIScrollView scrollView)
		{
			CommonClass.lineVC.scrlPath = this.TableView.IndexPathForRowAtPoint(this.TableView.Bounds.Location);
		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 80;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}
		public override bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			StateManagement stateManagement = new StateManagement();
			stateManagement.UpdateWBidStateContent();
			WBidHelper.PushToUndoStack ();

			int prevIndex, currentIndex;
			Line prevLine = GlobalSettings.Lines [sourceIndexPath.Row];
			Line currentLine = GlobalSettings.Lines [destinationIndexPath.Row];
			prevIndex = GlobalSettings.Lines.IndexOf (prevLine);
			currentIndex = GlobalSettings.Lines.IndexOf (currentLine);
			GlobalSettings.Lines.RemoveAt (prevIndex);
			GlobalSettings.Lines.Insert (currentIndex, prevLine);

            if (currentLine.TopLock == true)
            {
                GlobalSettings.Lines[currentIndex].TopLock = true;
                GlobalSettings.Lines[currentIndex].BotLock = false;
            }
            else if (currentLine.BotLock == true)
            {
                GlobalSettings.Lines[currentIndex].BotLock = true;
                GlobalSettings.Lines[currentIndex].TopLock = false;
            }
            else
            {
                GlobalSettings.Lines[currentIndex].TopLock = false;
                GlobalSettings.Lines[currentIndex].BotLock = false;
            }
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent.ForceLine.IsBlankLinetoBottom==true && prevLine.BlankLine)
            
            {
				wBIdStateContent.ForceLine.IsBlankLinetoBottom = false;
			    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                WindowAlert.RootViewController = new UIViewController();
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
				WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                

            }
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
//			if(GlobalSettings.Lines [currentIndex].TopLock == false && GlobalSettings.Lines [currentIndex].BotLock == false)
//				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);
//			else
//				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummary"), null, 0.2);

			this.PerformSelector (new ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);
			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}

		[Export("ReloadSummary")]
		private void reload ()
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		}

		[Export("ReloadSummaryWhole")]
		private void reloadWhole ()
		{
			var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBIdStateContent.SortDetails == null) {
				wBIdStateContent.SortDetails = new SortDetails ();
			}
			wBIdStateContent.SortDetails.SortColumn = "Manual";
			CommonClass.columnID = 0;
			//NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		}

	}
}

