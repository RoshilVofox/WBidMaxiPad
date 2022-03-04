
using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak
{
	public class LeakTablewViewControllerSource : UITableViewSource
	{
		public UIView baseviewCell;
		public sampleCell cellTemp;
		public UITableView tablView;
		bool dragging;
		public LeakTablewViewControllerSource ()
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return 600;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Header";
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return "Footer";
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return (nfloat)80;
		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			 cellTemp = tableView.DequeueReusableCell (sampleCell.Key) as sampleCell;
			if (cellTemp == null)
				cellTemp = new sampleCell ();
			cellTemp.Tag = indexPath.Row;

			return cellTemp;
		}

		public override void DraggingStarted (UIScrollView scrollView)
		{
			dragging = true;
		}

		public override void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
		{

			dragging = false;
			if (!willDecelerate) {
				foreach (sampleCell cell in	tablView.VisibleCells) {
					cell.SetNeedsDisplay ();
				}
			}
		}

		public override void DecelerationEnded (UIScrollView scrollView)
		{
			foreach(sampleCell cell in	tablView.VisibleCells)
			{
				cell.SetNeedsDisplay ();
			}
		}





	}

}

