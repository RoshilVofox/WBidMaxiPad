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
using CoreFoundation;


namespace WBid.WBidiPad.iOS
{
	public class SummaryListSource : UITableViewSource ,IUITableViewDataSource
	{
		public SummaryListSource ()
		{
		}
		UITableView TableView;
		public bool clearData;
		[Export ("numberOfSectionsInTableView:")]
		public override nint  NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			TableView = tableview;
			// TODO: return the actual number of items in the section
			Console.WriteLine ("Lines Count :-" + GlobalSettings.Lines.Count.ToString());

			//if (clearData)
			//	return 0;
			//else
			return GlobalSettings.Lines.Count;


		}



		[Export ("tableView:cellForRowAtIndexPath:")]
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			tableView.RegisterNibForCellReuse(UINib.FromName("summaryListCell", NSBundle.MainBundle), "summaryListCell");



			summaryListCell cell = (summaryListCell)tableView.DequeueReusableCell(new NSString("summaryListCell"), indexPath);


			if (clearData)
			{
				cell.RemoveAllViews(cell);
			}


			cell.SetNeedsLayout();
			//cell.LayoutIfNeeded();

			//cell.SetNeedsDisplay();


			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//			if (wBIdStateContent.TagDetails != null)
//			{
//				wBIdStateContent.TagDetails.ForEach(x => GlobalSettings.Lines.FirstOrDefault(y => y.LineNum == x.Line).Tag = x.Content);
//			}
			if (GlobalSettings.TagDetails != null)
			{
				GlobalSettings.TagDetails.ForEach(x => GlobalSettings.Lines.FirstOrDefault(y => y.LineNum == x.Line).Tag = x.Content);
			}

            DispatchQueue.MainQueue.DispatchAsync (() => {
                cell.bindData (GlobalSettings.Lines [indexPath.Row], indexPath);
            });


			//cell.bindData (GlobalSettings.Lines [indexPath.Row], indexPath);
			Line line = GlobalSettings.Lines[indexPath.Row];
			if (GlobalSettings.Lines[indexPath.Row].BlankLine || GlobalSettings.Lines[indexPath.Row].LineDisplay.Contains("RR"))
				cell.BackgroundColor = ColorClass.BlankLineColor;
			else if (GlobalSettings.Lines[indexPath.Row].ReserveLine)
				cell.BackgroundColor = ColorClass.ReserveLineColor;
			else if (GlobalSettings.WBidINIContent.User.IsSummaryViewShade)
			{
				if (line.AMPM == "AM")
				{
					//cell.BackgroundColor = UIColor.FromRGB(175,203,247);
					cell.BackgroundColor = ColorClass.SummaryViewAMlineColor;
				}
				else if (line.AMPM == " PM")
				{
					//cell.BackgroundColor = UIColor.FromRGB(255, 222, 164);
					cell.BackgroundColor = ColorClass.SummaryViewPMLineColor;
				}
				else
				{
					if (indexPath.Row % 2 == 0)
						cell.BackgroundColor = UIColor.FromRGB(255, 255, 255);
					else
						cell.BackgroundColor = UIColor.FromRGB(249, 249, 249);
				}
			}
			else
			{
				if (indexPath.Row % 2 == 0)
					cell.BackgroundColor = UIColor.FromRGB(255, 255, 255);
				else
					cell.BackgroundColor = UIColor.FromRGB(249, 249, 249);
			
			}
			//
			return cell;
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			CommonClass.lineVC.scrlPath = this.TableView.IndexPathForRowAtPoint(this.TableView.Bounds.Location);
		}
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);


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
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                WindowAlert.Dispose();

            }
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			//			if(GlobalSettings.Lines [currentIndex].TopLock == false && GlobalSettings.Lines [currentIndex].BotLock == false)
			//				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);
			//			else
			//				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummary"), null, 0.2);

			//Earlier table view reloading by a small delay ,Now made it as normal rload because in summary view the whole screen is getting blank for a second while moving a line from one position to other.
			this.PerformSelector (new ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);

			GlobalSettings.isModified = true;
			CommonClass.lineVC.UpdateSaveButton ();
		}
		public override void DraggingStarted(UIScrollView scrollView)
		{

			PerformSelector(new ObjCRuntime.Selector("reloadActualData"), null, 0.2);

		}
		[Export("reloadActualData")]
		public void reloadActualData()
		{
			clearData = false;
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


