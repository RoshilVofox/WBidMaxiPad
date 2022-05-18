using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.CommuteDifferenceView
{
    public class CommuteDifferenceTableViewSource:UITableViewSource
    {
        List<CommuteFltChangeValues> lstCommuteDifference { get; set; }
        public CommuteDifferenceTableViewSource(List<CommuteFltChangeValues> _lstCommuteDifference)
        {
            lstCommuteDifference = _lstCommuteDifference;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("CommutDiffTableViewCell", NSBundle.MainBundle), "CommutDiffTableViewCell");
            CommutDiffTableViewCell cell = (CommutDiffTableViewCell)tableView.DequeueReusableCell(new NSString("CommutDiffTableViewCell"), indexPath);

            cell.LabelValues(lstCommuteDifference[indexPath.Row]);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return lstCommuteDifference.Count;
        }
                

       
       
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
            //return base.NumberOfSections(tableView);
        }
    }
}
