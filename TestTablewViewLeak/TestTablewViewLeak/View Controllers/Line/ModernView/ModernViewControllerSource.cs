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
    public class ModernViewControllerSource : UITableViewSource
    {
        public UITableView tablView;
        public bool dragging;
        public bool FastDragging;
        public int LoadIndex = 0;
        public ModernViewControllerSource()
        {
        }
        UITableView TableView;
        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            TableView = tableview;
            // TODO: return the actual number of items in the section
            return GlobalSettings.Lines.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("ModernViewCell", NSBundle.MainBundle), "ModernViewCell");
            ModernViewCell cell = (ModernViewCell)tableView.DequeueReusableCell(new NSString("ModernViewCell"), indexPath);
            //cell.bindData (GlobalSettings.Lines [indexPath.Row], indexPath);
            //			if (indexPath.Row % 2==0)
            //				cell.BackgroundColor = UIColor.FromRGB (255, 255, 255);
            //			elsenew CGRect(0, 0, 50, 50);
            //				cell.BackgroundColor = UIColor.FromRGB (249, 249, 249);
            cell.Tag = indexPath.Row;
            cell.BackgroundColor = ColorClass.normDayColor;


            if (LoadIndex < 12) {
                cell.bindData(GlobalSettings.Lines[(int)cell.Tag], indexPath);

                cell.BindOverLayData(true);

            }
            cell.Accessory = UITableViewCellAccessory.None;
            UIView viewss = new UIView();
            viewss.Frame = new CGRect(0, 0, 50, 50);
            viewss.BackgroundColor = UIColor.Red;
            cell.AccessoryView = viewss;
            LoadIndex++;
            cell.BindOverLayDataLine(GlobalSettings.Lines[(int)cell.Tag]);
            cell.createCellBorder(GlobalSettings.Lines[indexPath.Row], cell);
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            ModernViewCell Cell = (ModernViewCell)cell;
            if (dragging) Cell.BindOverLayData(false);
            else Cell.BindOverLayData(true);


            if (FastDragging) {

                Cell.bindData(GlobalSettings.Lines[(int)cell.Tag], indexPath);
            }

            Cell.createCellBorder(GlobalSettings.Lines[indexPath.Row], cell);

        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            dragging = true;
            FastDragging = false;
            foreach (ModernViewCell cell in tablView.VisibleCells) {
                cell.BindOverLayData(false);
            }


        }


        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {



            if (!willDecelerate) {

                foreach (ModernViewCell cell in tablView.VisibleCells) {
                    NSIndexPath indexPath = NSIndexPath.FromRowSection(cell.Tag, (nint)1);
                    cell.bindData(GlobalSettings.Lines[(int)cell.Tag], indexPath);
                    cell.BindOverLayData(true);
                    cell.createCellBorder(GlobalSettings.Lines[indexPath.Row], cell);
                }
                //dragging = false;
            }
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            foreach (ModernViewCell cell in tablView.VisibleCells)
            {
                NSIndexPath indexPath = NSIndexPath.FromRowSection(cell.Tag, (nint)1);
                cell.bindData(GlobalSettings.Lines[(int)cell.Tag], indexPath);
                cell.BindOverLayData(true);
                cell.createCellBorder(GlobalSettings.Lines[indexPath.Row], cell);
            }
            CommonClass.lineVC.scrlPath = this.TableView.IndexPathForRowAtPoint(this.TableView.Bounds.Location);
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            //commented due to the issue in the trip view display uncertianity when double tapping from the modern bid line view
            //NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopHide", null);
            if (CommonClass.selectedTrip != null) {
                CommonClass.selectedTrip = null;
                tableView.ReloadData();
            }
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (GlobalSettings.WBidINIContent.User.IsModernViewShade == false)
            {
                return 80;
            }

            else
            {
                if (GlobalSettings.Lines[indexPath.Row].ManualScroll == 1)
                {
                    return 100;
                }
                else
                {
                    return 80;
                }
            }


        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }
        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }
        public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {

            StateManagement stateManagement = new StateManagement();
            stateManagement.UpdateWBidStateContent();
            WBidHelper.PushToUndoStack();

            if (sourceIndexPath != destinationIndexPath)
            {

                foreach (var line in GlobalSettings.Lines)
                {
                    line.ManualScroll = 0;
                }

                int prevIndex, currentIndex;
                Line prevLine = GlobalSettings.Lines[sourceIndexPath.Row];
                Line currentLine = GlobalSettings.Lines[destinationIndexPath.Row];

                prevIndex = GlobalSettings.Lines.IndexOf(prevLine);
                currentIndex = GlobalSettings.Lines.IndexOf(currentLine);

                bool isNeedtoShowBlueLineonFirstLine = false;
                if (prevIndex > 0)

                    GlobalSettings.Lines[prevIndex - 1].ManualScroll = 1;
                else
                    isNeedtoShowBlueLineonFirstLine = true;

                GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == prevLine.LineNum).ManualScroll = 2;

                GlobalSettings.Lines.RemoveAt(prevIndex);
                GlobalSettings.Lines.Insert(currentIndex, prevLine);

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
                if (isNeedtoShowBlueLineonFirstLine)
                    GlobalSettings.Lines[0].ManualScroll = 3;

                WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBIdStateContent.ForceLine.IsBlankLinetoBottom == true && prevLine.BlankLine)
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
                NSNotificationCenter.DefaultCenter.PostNotificationName("CalPopHide", null);
                //			if(GlobalSettings.Lines [currentIndex].TopLock == false && GlobalSettings.Lines [currentIndex].BotLock == false)
                //				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);
                //			else
                //				this.PerformSelector (new MonoTouch.ObjCRuntime.Selector("ReloadSummary"), null, 0.2);
                dragging = false;
                this.PerformSelector(new ObjCRuntime.Selector("ReloadSummaryWhole"), null, 0.2);
                GlobalSettings.isModified = true;
                CommonClass.lineVC.UpdateSaveButton();
                //			foreach(ModernViewCell cell in	tablView.VisibleCells)
                //			{
                //				cell.BindOverLayData (true);
                //			}

            }
        }


        [Export("ReloadSummary")]
        private void reload()
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }

        [Export("ReloadSummaryWhole")]
        private void reloadWhole()
        {
            var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent.SortDetails == null) {
                wBIdStateContent.SortDetails = new SortDetails();
            }
            wBIdStateContent.SortDetails.SortColumn = "Manual";
            CommonClass.columnID = 0;
            //NSNotificationCenter.DefaultCenter.PostNotificationName("DataCulumnsUpdated", null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }

    }

}

