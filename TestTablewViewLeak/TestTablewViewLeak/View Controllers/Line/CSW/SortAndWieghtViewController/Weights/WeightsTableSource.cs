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
	public class WeightsTableSource : UITableViewSource
	{
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

		public WeightsTableSource ()
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return WeightsApplied.MainList.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			if (section == WeightsApplied.MainList.IndexOf ("Aircraft Changes")) {
				if (wBIdStateContent.CxWtState.ACChg.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("AM/PM")) {
				if (wBIdStateContent.CxWtState.AMPM.Wt) {
					return WeightsApplied.AMPMWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Blocks of Days Off")) {
				if (wBIdStateContent.CxWtState.BDO.Wt) {
					return WeightsApplied.BlocksOfDaysOffWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Cmut DHs")) {
				if (wBIdStateContent.CxWtState.DHD.Wt) {
					return WeightsApplied.CmutDHsWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Commutable Lines - Manual")) {
				if (wBIdStateContent.CxWtState.CL.Wt) {
					return 1;
				} else {
					return 0;
				}
			}
			else if (section == WeightsApplied.MainList.IndexOf ("Commutable Line - Auto")) {
				if (wBIdStateContent.CxWtState.CLAuto.Wt) {
					return 1;
				} else {
					return 0;
				}
			}else if (section == WeightsApplied.MainList.IndexOf ("Days of the Month")) {
				if (wBIdStateContent.CxWtState.SDO.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Days of the Week")) {
				if (wBIdStateContent.CxWtState.DOW.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("DH - first - last")) {
				if (wBIdStateContent.CxWtState.DHDFoL.Wt) {
					return WeightsApplied.dhFirstLastWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Duty period")) {
				if (wBIdStateContent.CxWtState.DP.Wt) {
					return WeightsApplied.DutyPeriodWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Equipment Type")) {
				if (wBIdStateContent.CxWtState.EQUIP.Wt) {
					return WeightsApplied.EQTypeWeights.Count;
				} else {
					return 0;
				}
			}
			else if (section == WeightsApplied.MainList.IndexOf("ETOPS"))
			{
				if (wBIdStateContent.CxWtState.ETOPS.Wt)
				{
					return WeightsApplied.ETOPSWeights.Count;
				}
				else
				{
					return 0;
				}
			}

			else if (section == WeightsApplied.MainList.IndexOf("ETOPS-Res"))
			{
				if (wBIdStateContent.CxWtState.ETOPSRes.Wt)
				{
					return WeightsApplied.ETOPSResWeights.Count;
				}
				else
				{
					return 0;
				}
			}
			else if (section == WeightsApplied.MainList.IndexOf ("Flight Time")) {
				if (wBIdStateContent.CxWtState.FLTMIN.Wt) {
					return WeightsApplied.FlightTimeWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Ground Time")) {
				if (wBIdStateContent.CxWtState.GRD.Wt) {
					return WeightsApplied.GroundTimeWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Intl – NonConus")) {
				if (wBIdStateContent.CxWtState.InterConus.Wt) {
					return WeightsApplied.IntlNonConusWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Largest Block of Days Off")) {
				if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Legs Per Duty Period")) {
				if (wBIdStateContent.CxWtState.LEGS.Wt) {
					return WeightsApplied.LegsPerDutyPeriodWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Legs Per Pairing")) {
				if (wBIdStateContent.CxWtState.LegsPerPairing.Wt) {
					return WeightsApplied.LegsPerPairingWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Normalize Days Off")) {
				if (wBIdStateContent.CxWtState.NormalizeDays.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Number of Days Off")) {
				if (wBIdStateContent.CxWtState.NODO.Wt) {
					return WeightsApplied.NumOfDaysOffWeights.Count;
				} else {
					return 0;
				}
			}
			else if (section == WeightsApplied.MainList.IndexOf ("Overnight Cities")) {
				if (wBIdStateContent.CxWtState.RON.Wt) {
					return WeightsApplied.OvernightCitiesWeights.Count;
				} else {
					return 0;
				}
			} 
			else if (section == WeightsApplied.MainList.IndexOf ("Cities-Legs")) {
				if (wBIdStateContent.CxWtState.CitiesLegs.Wt) {
					return WeightsApplied.CitiesLegsWeights.Count;
				} else {
					return 0;
				}
			} 
			else if (section == WeightsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
				if (wBIdStateContent.CxWtState.BulkOC.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("PDO-after")) {
				if (wBIdStateContent.CxWtState.PDAfter.Wt) {
					return WeightsApplied.PDOAfterWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("PDO-before")) {
				if (wBIdStateContent.CxWtState.PDBefore.Wt) {
					return WeightsApplied.PDOBeforeWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Position")) {
				if (wBIdStateContent.CxWtState.Position.Wt) {
					return WeightsApplied.PositionWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Start Day of Week")) {
				if (wBIdStateContent.CxWtState.SDOW.Wt) {
					return WeightsApplied.StartDOWWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Rest")) {
				if (wBIdStateContent.CxWtState.Rest.Wt) {
					return WeightsApplied.RestWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Time-Away-From-Base")) {
				if (wBIdStateContent.CxWtState.PerDiem.Wt) {
					return 1;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Trip Length")) {
				if (wBIdStateContent.CxWtState.TL.Wt) {
					return WeightsApplied.TripLengthWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Work Blk Length")) {
				if (wBIdStateContent.CxWtState.WB.Wt) {
					return WeightsApplied.WorkBlockLengthWeights.Count;
				} else {
					return 0;
				}
			} else if (section == WeightsApplied.MainList.IndexOf ("Work Days")) {
				if (wBIdStateContent.CxWtState.WorkDay.Wt) {
					return WeightsApplied.WorkDaysWeights.Count;
				} else {
					return 0;
				}
			} 
			else if (section == WeightsApplied.MainList.IndexOf ("Commutability")) {
				if (wBIdStateContent.CxWtState.Commute.Wt) {
					return 1;
				} else {
					return 0;
				}
			} 
			else
				return 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Days of the Month") || indexPath.Section == WeightsApplied.MainList.IndexOf ("Commutable Lines - Manual")) {
				tableView.RegisterNibForCellReuse (UINib.FromName ("DaysOfMonthCell", NSBundle.MainBundle), "DaysOfMonthCell");
				DaysOfMonthCell cell = (DaysOfMonthCell)tableView.DequeueReusableCell (new NSString ("DaysOfMonthCell"), indexPath);
				cell.DisplayMode = "Weights";
				cell.bindData (indexPath);
				return cell;
			} else if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
				tableView.RegisterNibForCellReuse (UINib.FromName ("OvernightBulkCell", NSBundle.MainBundle), "OvernightBulkCell");
				OvernightBulkCell cell = (OvernightBulkCell)tableView.DequeueReusableCell (new NSString ("OvernightBulkCell"), indexPath);
				cell.DisplayMode = "Weights";
				cell.bindData (indexPath);
				return cell;
			} 
			else if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Commutable Line - Auto")) 
			{
				
					CommutableLinesCell cell = CommutableLinesCell.Create ();
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.CSWWeightFilldata ( wBIdStateContent.Weights.CLAuto);
					return cell;
				}
			else {
				tableView.RegisterNibForCellReuse (UINib.FromName ("WeightsCell", NSBundle.MainBundle), "WeightsCell");
				WeightsCell cell = (WeightsCell)tableView.DequeueReusableCell (new NSString ("WeightsCell"), indexPath);
				cell.bindData (indexPath);
				return cell;
			}//Commutability
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Days of the Month") || indexPath.Section == WeightsApplied.MainList.IndexOf ("Commutable Lines - Manual")) {
				return 300;
			} else if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Days of the Week")) {
				return 100;
			} else if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Commutable Line - Auto")) {
				return 50;
			}
			else if (indexPath.Section == WeightsApplied.MainList.IndexOf ("Overnight Cities - Bulk")) {
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

