using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public class ConstraintsTableSource : UITableViewSource
	{
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		FtCommutableLine currentElement;
		public ConstraintsTableSource ()
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return ConstraintsApplied.MainList.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			if (section == ConstraintsApplied.MainList.IndexOf ("Aircraft Changes")) {
				if (wBIdStateContent.CxWtState.ACChg.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Blocks of Days Off")) {
				if (wBIdStateContent.CxWtState.BDO.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Cmut DHs")) {
				if (wBIdStateContent.CxWtState.DHD.Cx)
					return ConstraintsApplied.CmutDHsConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Commutable Lines - Manual")) {
				if (wBIdStateContent.CxWtState.CL.Cx)
					return 1;
				else
					return 0;
			}
            else if (section == ConstraintsApplied.MainList.IndexOf("Commutable Lines"))
            {
                if (wBIdStateContent.CxWtState.CL.Cx)
                    return 1;
                else
                    return 0;
            }
            else if (section == ConstraintsApplied.MainList.IndexOf ("Days of the Week")) {
				if (wBIdStateContent.CxWtState.DOW.Cx)
					return ConstraintsApplied.daysOfWeekConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Days of the Month")) {
				if (wBIdStateContent.CxWtState.SDO.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("DH - first - last")) {
				if (wBIdStateContent.CxWtState.DHDFoL.Cx)
					return ConstraintsApplied.DhFirstLastConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Duty period")) {
				if (wBIdStateContent.CxWtState.DP.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Equipment Type")) {
				if (wBIdStateContent.CxWtState.EQUIP.Cx)
					return ConstraintsApplied.EQTypeConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Flight Time")) {
				if (wBIdStateContent.CxWtState.FLTMIN.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Ground Time")) {
				if (wBIdStateContent.CxWtState.GRD.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Intl – NonConus")) {
				if (wBIdStateContent.CxWtState.InterConus.Cx)
					return ConstraintsApplied.IntlNonConusConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Legs Per Duty Period")) {
				if (wBIdStateContent.CxWtState.LEGS.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Legs Per Pairing")) {
				if (wBIdStateContent.CxWtState.LegsPerPairing.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Number of Days Off")) {
				if (wBIdStateContent.CxWtState.NODO.Cx)
					return 1;
				else
					return 0;
			} 
			else if (section == ConstraintsApplied.MainList.IndexOf ("Overnight Cities"))
			{
				if (wBIdStateContent.CxWtState.RON.Cx)
					return ConstraintsApplied.OvernightCitiesConstraints.Count;
				else
					return 0;
			} 
			else if (section == ConstraintsApplied.MainList.IndexOf ("Cities-Legs"))
			{
				if (wBIdStateContent.CxWtState.CitiesLegs.Cx)
					return ConstraintsApplied.CitiesLegsConstraints.Count;
				else
					return 0;
			} 
			else if (section == ConstraintsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
				if (wBIdStateContent.CxWtState.BulkOC.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("PDO")) {
				if (wBIdStateContent.CxWtState.WtPDOFS.Cx)
					return ConstraintsApplied.PDOConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Start Day of Week")) {
				if (wBIdStateContent.CxWtState.SDOW.Cx)
					return ConstraintsApplied.StartDayofWeekConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Rest")) {
				if (wBIdStateContent.CxWtState.Rest.Cx)
					return ConstraintsApplied.RestConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Time-Away-From-Base")) {
				if (wBIdStateContent.CxWtState.PerDiem.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Trip Length")) {
				if (wBIdStateContent.CxWtState.TL.Cx)
					return ConstraintsApplied.TripLengthConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Work Blk Length")) {
				if (wBIdStateContent.CxWtState.WB.Cx)
					return ConstraintsApplied.WorkBlockLengthConstraints.Count;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Work Days")) {
				if (wBIdStateContent.CxWtState.WorkDay.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Min Pay")) {
				if (wBIdStateContent.CxWtState.MP.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("3-on-3-off")) {
				if (wBIdStateContent.CxWtState.No3on3off.Cx)
					return 1;
				else
					return 0;
			} else if (section == ConstraintsApplied.MainList.IndexOf ("Overlap Days")) {
				if (wBIdStateContent.CxWtState.NOL.Cx)
					return 1;
				else
					return 0;

			}
			else if (section == ConstraintsApplied.MainList.IndexOf ("Commutable Line - Auto")) 
			{
				if (wBIdStateContent.CxWtState.CLAuto.Cx)
					return 1;
				else
					return 0;

			}
			else if (section == ConstraintsApplied.MainList.IndexOf ("Commutability")) 
			{
				if (wBIdStateContent.CxWtState.Commute.Cx)
					return 1;
				else
					return 0;

			}
            else if (section == ConstraintsApplied.MainList.IndexOf("Start Day"))
            {
                if (wBIdStateContent.CxWtState.StartDay.Cx)
                    return ConstraintsApplied.StartDayConstraints.Count;
                else
                    return 0;
            }
            else if (section == ConstraintsApplied.MainList.IndexOf("Report-Release"))
            {
                if (wBIdStateContent.CxWtState.ReportRelease.Cx)
                    return ConstraintsApplied.ReportReleaseConstraints.Count;
                else
                    return 0;
            }
			else if (section == ConstraintsApplied.MainList.IndexOf("Mixed Hard/Reserve"))
			{
				if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
					return 1;
				else
					return 0;
			}
			//			else if (section == ConstraintsApplied.MainList.IndexOf ("Cities-Legs")) 
			//			{
			//				if (wBIdStateContent.CxWtState.CitiesLegs.Cx)
			//					return 1;
			//				else
			//					return 0;
			//
			//			}
			else
				return 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
            if (indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Month") || indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Lines - Manual") || indexPath.Section == ConstraintsApplied.MainList.IndexOf("Commutable Lines")) {
				tableView.RegisterNibForCellReuse (UINib.FromName ("DaysOfMonthCell", NSBundle.MainBundle), "DaysOfMonthCell");
				DaysOfMonthCell cell = (DaysOfMonthCell)tableView.DequeueReusableCell (new NSString ("DaysOfMonthCell"), indexPath);
				cell.DisplayMode = "Constraints";
				cell.bindData (indexPath);
				return cell;
			} else if (indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
				tableView.RegisterNibForCellReuse (UINib.FromName ("OvernightBulkCell", NSBundle.MainBundle), "OvernightBulkCell");
				OvernightBulkCell cell = (OvernightBulkCell)tableView.DequeueReusableCell (new NSString ("OvernightBulkCell"), indexPath);
				cell.DisplayMode = "Constraints";
				cell.bindData (indexPath);
				return cell;
			} else if (indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Line - Auto")) 
			{
				CommutableLinesCell cell = CommutableLinesCell.Create ();
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.CSWConstraintFilldata ( wBIdStateContent.Constraints.CLAuto);
				return cell;
			}
            else if (indexPath.Section == ConstraintsApplied.MainList.IndexOf("Report-Release"))
            {
               
                 ReportReleaseCell cell = ReportReleaseCell.Create();
                 cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.bindData (indexPath,wBIdStateContent.Constraints.ReportRelease.lstParameters);
                //cell.ConstrainFilleData(wBIdStateContent.Constraints.ReportRelease.lstParameters);
                return cell;
            }
			else {
				tableView.RegisterNibForCellReuse (UINib.FromName ("ConstraintsCell", NSBundle.MainBundle), "ConstraintsCell");
				ConstraintsCell cell = (ConstraintsCell)tableView.DequeueReusableCell (new NSString ("ConstraintsCell"), indexPath);
				cell.bindData (indexPath);
				return cell;
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
            if (indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Month") || indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Lines - Manual") || indexPath.Section == ConstraintsApplied.MainList.IndexOf("Commutable Lines")) {
				return 300;
			} 
			else if(indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Line - Auto") )
			{
				return 50;
			}
            else if (indexPath.Section == ConstraintsApplied.MainList.IndexOf("Report-Release"))
            {
                return 150;
            }
            else if (indexPath.Section == ConstraintsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
				//int count1 = GlobalSettings.OverNightCitiesInBid.Count / 5;
				//int count2 = GlobalSettings.OverNightCitiesInBid.Count % 5;

				int count1;
				int count2;

				if (GlobalSettings.OverNightCitiesInBid.Count < 80)
				{
					count1 = 18;
					count2 = 0;
				}
				else
				{
					count1 = GlobalSettings.OverNightCitiesInBid.Count / 5;
					count2 = GlobalSettings.OverNightCitiesInBid.Count % 5;
				}
				if (count2 == 0)
					return 30 * count1;
				else
					return 30 * (count1 + 1);
			} else {
				return 50;
			}
		}
	}
}

