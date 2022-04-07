using System;
using Foundation;
using UIKit;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public class VacDiffTableViewControllerSource: UITableViewSource
    {
        public VacDiffTableViewControllerSource()
        {
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            throw new NotImplementedException();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            throw new NotImplementedException();
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return base.NumberOfSections(tableView);
        }
        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return base.TitleForHeader(tableView, section);
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return base.GetViewForHeader(tableView, section);
        }
    }
}
