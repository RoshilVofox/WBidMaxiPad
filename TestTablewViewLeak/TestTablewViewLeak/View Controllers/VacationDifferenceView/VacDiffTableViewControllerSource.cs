using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public class VacDiffTableViewControllerSource: UITableViewSource
    {
        private List<FlightDataChangeVacValues> _lstFlightDataChangevalues { get; set; }
        public VacDiffTableViewControllerSource(List<FlightDataChangeVacValues> lstFlightDataChangevalues)
        {
            _lstFlightDataChangevalues = lstFlightDataChangevalues;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("VacDiffDetailsCell", NSBundle.MainBundle), "VacDiffDetailsCell");
            // var tes =tableView.DequeueReusableCell(new NSString("VacDiffCell"), indexPath);


            VacDiffDetailsCell cell = (VacDiffDetailsCell)tableView.DequeueReusableCell(new NSString("VacDiffDetailsCell"), indexPath);
            string domicile = string.Empty;
            string position = string.Empty;
            string previousmonthcap = string.Empty;
            string currentmonthcap = string.Empty;

            //previousmonthcap = (lstCAPOutputParameter[indexPath.Row].PreviousMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].PreviousMonthCap.ToString();
            //currentmonthcap = (lstCAPOutputParameter[indexPath.Row].CurrentMonthCap == null) ? string.Empty : lstCAPOutputParameter[indexPath.Row].CurrentMonthCap.ToString();

            //cell.LabelValues(_lstFlightDataChangevalues[indexPath.Row]);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _lstFlightDataChangevalues.Count;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
            //return base.NumberOfSections(tableView);
        }
        public override string TitleForHeader(UITableView tableView, nint section)
        {
            //return base.TitleForHeader(tableView, section);
            return "c";
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return base.GetViewForHeader(tableView, section);
        }
    }
}
