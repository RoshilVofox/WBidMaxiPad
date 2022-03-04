using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;

namespace WBid.WBidiPad.iOS
{
	public class TripPopListViewSource : UITableViewSource
	{
		public TripPopListViewSource ()
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
			return CommonClass.tripData.Count;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (TripPopListViewCell.Key) as TripPopListViewCell;
			if (cell == null)
				cell = new TripPopListViewCell ();

			// TODO: populate the cell with the appropriate data based on the indexPath
			//cell.TextLabel.Font = UIFont.SystemFontOfSize (12);
            
			cell.TextLabel.Text = CommonClass.tripData [indexPath.Row].Content;
			cell.TextLabel.Font = UIFont.FromName ("Courier", 11);
			cell.TextLabel.Lines = 0;

//			if (CommonClass.tripData [indexPath.Row].IsStrike) {
//				NSAttributedString attString = new NSAttributedString (
//					CommonClass.tripData [indexPath.Row].Content,
//					new UIStringAttributes {StrikethroughStyle = NSUnderlineStyle.Single}
//				);
//				cell.TextLabel.AttributedText = attString;
//			}

			return cell;
		}
        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			if (CommonClass.tripData [indexPath.Row].BackColor == "VA") {
				cell.TextLabel.BackgroundColor = ColorClass.VacationTripColor;
				cell.TextLabel.TextColor = UIColor.Black;
			} else if (CommonClass.tripData [indexPath.Row].BackColor == "VD") {
				cell.TextLabel.BackgroundColor = ColorClass.VacationDropTripColor;
				cell.TextLabel.TextColor = UIColor.White;
			} else if (CommonClass.tripData [indexPath.Row].BackColor == "VO") {
				cell.TextLabel.BackgroundColor = ColorClass.VacationOverlapTripColor;
				cell.TextLabel.TextColor = UIColor.Black;
			} else if (CommonClass.tripData [indexPath.Row].BackColor == "Overlap") {
				cell.TextLabel.BackgroundColor = ColorClass.OverlapColor;
				cell.TextLabel.TextColor = UIColor.Black;
			}  else if (CommonClass.tripData [indexPath.Row].BackColor == "MD") {
				cell.TextLabel.BackgroundColor = UIColor.Orange;
				cell.TextLabel.TextColor = UIColor.Black;
			}
            else if (CommonClass.tripData[indexPath.Row].BackColor == "FV")
            {
                cell.TextLabel.BackgroundColor = ColorClass.FVVacationColor;
                cell.TextLabel.TextColor = UIColor.Black;
            }
            else {
				cell.TextLabel.BackgroundColor = UIColor.Clear;
				cell.TextLabel.TextColor = UIColor.Black;
			}

			if (CommonClass.tripData [indexPath.Row].IsStrike) {
				NSAttributedString attString = new NSAttributedString (
                   CommonClass.tripData [indexPath.Row].Content,
                   new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.Single }
               );
				cell.TextLabel.AttributedText = attString;
			} else {
				NSAttributedString attString = new NSAttributedString (
					CommonClass.tripData [indexPath.Row].Content,
					new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.None }
				);
				cell.TextLabel.AttributedText = attString;
			}

		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0)
				return 30;
			else
				return 20;
		}
	}
}

