
using System;

using Foundation;
using UIKit;

namespace TestTablewViewLeak
{
	public class sampleSource : UITableViewSource
	{
		public sampleSource ()
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
			return 1;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Header";
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return "Footer";
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (sampleCell.Key) as sampleCell;
			if (cell == null)
				cell = new sampleCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.DetailTextLabel.Text = "DetailsTextLabel";
			
			return cell;
		}
		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
//			sampleCell Cell = (sampleCell)cell;
//			Cell.SetNeedsDisplay ();
		}
	}
}

