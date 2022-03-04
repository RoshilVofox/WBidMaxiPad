using System;
using System.Drawing;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class horizontalTableViewSource : UITableViewSource
	{
		public horizontalTableViewSource ()
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return (nint)1;
		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return (nfloat)46.545f;
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return (nint)22;
		}

//		public override string TitleForHeader (UITableView tableView, int section)
//		{
//			return "Header";
//		}
//
//		public override string TitleForFooter (UITableView tableView, int section)
//		{
//			return "Footer";
//		}
//
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			tableView.RegisterNibForCellReuse(UINib.FromName("summaryHeaderCell", NSBundle.MainBundle), "summaryHeaderCell");
			summaryHeaderCell cell = (summaryHeaderCell)tableView.DequeueReusableCell(new NSString("summaryHeaderCell"), indexPath);
			return cell;
		}
	}
}

