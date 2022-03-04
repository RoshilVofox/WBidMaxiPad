using System;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public class ConstraintTableSource: UITableViewSource
	{
		ConstraintModalView _parentVC;
		public ConstraintTableSource (ConstraintModalView parentVC)
		{
			_parentVC = parentVC;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return  Constants.listConstraints.Count;
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			ConstraintModalCell cell = ConstraintModalCell.Create ();
			bool isChecked = false;
			switch (indexPath.Row) {
			case 0:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is AMPMConstriants) {
						isChecked = true;
						break;
					}
				}
				break;
//			case 1:
//				foreach (var item in SharedObject.Instance.ListConstraint) {
//					if (item is BlankReserveConstraint) {
//						isChecked = true;
//						break;
//					}
//				}
//				break;
			case 1:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is FtCommutableLine) {
						isChecked = true;
						break;
					}
				}
				break;

			case 2:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is DaysOfMonthCx) {
						isChecked = true;
						break;
					}
				}
				break;

			case 3:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is DaysOfWeekAll) {
						isChecked = true;
						break;
					}
				}
				break;
			case 4:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is DaysOfWeekSome) {
						isChecked = true;
						break;
					}
				}
				break;
			case 5:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is DHFristLastConstraint) {
						isChecked = true;
						break;
					}
				}
				break;
			case 6:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is EquirementConstraint) {
						isChecked = true;
						break;
					}
				}
				break;
			case 7:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is LineTypeConstraint) {
						isChecked = true;
						break;
					}
				}
				break;
			case 8:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is OvernightCitiesCx) {
						isChecked = true;
						break;
					}
				}
				break;
			case 9:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is RestCx) {
						isChecked = true;
						break;
					}
				}
				break;
			case 10:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is StartDayOfWeek) {
						isChecked = true;
						break;
					}
				}
				break;
			case 11:
				foreach (var item in SharedObject.Instance.ListConstraint) {
					if (item is CxTripBlockLength) {
						isChecked = true;
						break;
					}
				}
				break;
			default:
				break;
			}
			cell.FillData (indexPath.Row, isChecked);
			return cell;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			_parentVC.AddConstraintAtIndex (indexPath.Row);
		}
	}
}
