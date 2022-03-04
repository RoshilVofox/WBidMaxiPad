using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
    public class blockSortTableControllerSource : UITableViewSource
    {
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

        public blockSortTableControllerSource()
        {
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            // TODO: return the actual number of items in the section
            if (wBIdStateContent.SortDetails.BlokSort == null)
                wBIdStateContent.SortDetails.BlokSort = new List<string>();
            return wBIdStateContent.SortDetails.BlokSort.Count;
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
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // string value = wBIdStateContent.SortDetails.BlokSort [indexpath.Row];
            if(wBIdStateContent.SortDetails.BlokSort [(int)indexPath.Row] == "30" || wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "31"|| wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "32")
            {
            //if ((WBidCollection.GetBlockSortListData () [indexPath.Row].Name) == "Commutability") {
                tableView.RegisterNibForCellReuse (UINib.FromName ("CommutabilityCell", NSBundle.MainBundle), "CommutabilityCell");
                CommutabilityCell cell = (CommutabilityCell)tableView.DequeueReusableCell (new NSString ("CommutabilityCell"), indexPath);
                cell.bindData (indexPath);
                return cell;

            }
            if (wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "33" || wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "34" || wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "35" ||
            wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "36" || wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "37" || wBIdStateContent.SortDetails.BlokSort[(int)indexPath.Row] == "38")
            {
               
                tableView.RegisterNibForCellReuse(UINib.FromName("CommutabilityCell", NSBundle.MainBundle), "CommutabilityCell");
                CommutabilityCell cell = (CommutabilityCell)tableView.DequeueReusableCell(new NSString("CommutabilityCell"), indexPath);
                cell.bindData(indexPath);
                return cell;

            }
            else {
                tableView.RegisterNibForCellReuse (UINib.FromName ("blockSortCollectionCell", NSBundle.MainBundle), "blockSortCollectionCell");
                blockSortCollectionCell cell = (blockSortCollectionCell)tableView.DequeueReusableCell (new NSString ("blockSortCollectionCell"), indexPath);

                cell.bindData (indexPath);
                return cell;
            }
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 50;
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {


            if ((sourceIndexPath.Row != destinationIndexPath.Row) && (sourceIndexPath.Row > -1 && sourceIndexPath.Row < wBIdStateContent.SortDetails.BlokSort.Count()) && (destinationIndexPath.Row > -1 && destinationIndexPath.Row < wBIdStateContent.SortDetails.BlokSort.Count()))
            {
                WBidHelper.PushToUndoStack();
                int prevIndex, currentIndex;
                string prevSort = wBIdStateContent.SortDetails.BlokSort[sourceIndexPath.Row];
                string currentSort = wBIdStateContent.SortDetails.BlokSort[destinationIndexPath.Row];
                prevIndex = wBIdStateContent.SortDetails.BlokSort.IndexOf(prevSort);
                currentIndex = wBIdStateContent.SortDetails.BlokSort.IndexOf(currentSort);
                wBIdStateContent.SortDetails.BlokSort.RemoveAt(prevIndex);
                wBIdStateContent.SortDetails.BlokSort.Insert(currentIndex, prevSort);
                NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
            }

        }
    }
}

