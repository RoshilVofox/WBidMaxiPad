using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public class PopoverTableViewControllerSource : UITableViewSource
	{
		public string PopType;
		public string SubPopType;

		string[] arrPopoverTitle = {
			"Top Lock this line and above",
			"Bottom Lock this line and below",
			"Move selected lines - Above",
			"Move selected lines - Below",
			"Deselect all lines"
		};

		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


        ConstraintCalculations constCalc = new ConstraintCalculations ();
		WeightCalculation weightCalc = new WeightCalculation ();
		ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListDataCSW ();
        List<string> CommutabilitySortvalue = new List<string> () { "Overall", "Front", "Back" };
		public PopoverTableViewControllerSource ()
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			if (PopType == "sumColumn" || PopType == "BidlineColumns" || PopType == "ModernColumns")
				return 2;
			else
				return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			if (PopType == "sumOpt") {
				return arrPopoverTitle.Length;
			} else if (PopType == "addConst") {
				return ConstraintsApplied.MainList.Count;
			} else if (PopType == "sumColumn") {
				if (section == 0)
					return 1;
				else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
						return GlobalSettings.AdditionalvacationColumns.Count;
					else
						return GlobalSettings.AdditionalColumns.Count;
				} 
			} else if (PopType == "BidlineColumns") {
				if (section == 0)
					return 1;
				else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
						return GlobalSettings.BidlineAdditionalvacationColumns.Count;
					else
						return GlobalSettings.BidlineAdditionalColumns.Count;
				} 
			} else if (PopType == "ModernColumns") {
				if (section == 0)
					return 1;
				else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
						return GlobalSettings.ModernAdditionalvacationColumns.Count;
					else
						return GlobalSettings.ModernAdditionalColumns.Count;
				} 
			} else if (PopType == "blockSort") {
				return lstblockData.Count;
			} else if (PopType == "addWeights") {
				return WeightsApplied.MainList.Count;
            }else if(PopType == "changeValueCommutabilitySort")
                return CommutabilitySortvalue.Count;
            else {
				return 0;
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.RegisterNibForCellReuse (UINib.FromName ("PopoverListCell", NSBundle.MainBundle), "PopoverListCell");
			PopoverListCell cell = (PopoverListCell)tableView.DequeueReusableCell (new NSString ("PopoverListCell"), indexPath);

			// TODO: populate the cell with the appropriate data based on the indexPath
			if (PopType == "sumOpt") {
				cell.bindData (arrPopoverTitle [indexPath.Row], indexPath);
				if (indexPath.Row < 2) {
					cell.TextLabel.TextColor = UIColor.Black;
					cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
					cell.UserInteractionEnabled = true;
				} else {
					if (CommonClass.selectedRows.Count == 0) {
						cell.TextLabel.TextColor = UIColor.LightGray;
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
						cell.UserInteractionEnabled = false;
					}
				}


			} else if (PopType == "addConst") {
				string cellText = ConstraintsApplied.MainList [indexPath.Row];
				if (cellText == "Aircraft Changes") {
					if (wBIdStateContent.CxWtState.ACChg.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Blocks of Days Off") {
					if (wBIdStateContent.CxWtState.BDO.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Cmut DHs") {
					if (wBIdStateContent.CxWtState.DHD.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Commutable Lines - Manual") {
					if (wBIdStateContent.CxWtState.CL.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Days of the Week") {
					if (wBIdStateContent.CxWtState.DOW.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Days of the Month") {
					if (wBIdStateContent.CxWtState.SDO.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "DH - first - last") {
					if (wBIdStateContent.CxWtState.DHDFoL.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Duty period") {
					if (wBIdStateContent.CxWtState.DP.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Equipment Type") {
					if (wBIdStateContent.CxWtState.EQUIP.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
                
                else if (cellText == "Flight Time") {
					if (wBIdStateContent.CxWtState.FLTMIN.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Ground Time") {
					if (wBIdStateContent.CxWtState.GRD.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Intl – NonConus") {
					if (wBIdStateContent.CxWtState.InterConus.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Legs Per Duty Period") {
					if (wBIdStateContent.CxWtState.LEGS.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Legs Per Pairing") {
					if (wBIdStateContent.CxWtState.LegsPerPairing.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Number of Days Off") {
					if (wBIdStateContent.CxWtState.NODO.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} 
				else if (cellText == "Overnight Cities") 
				{
					if (wBIdStateContent.CxWtState.RON.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Cities-Legs") 
				{
					if (wBIdStateContent.CxWtState.CitiesLegs.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} 
				else if (cellText == "Overnight Cities - Bulk") {
					if (wBIdStateContent.CxWtState.BulkOC.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "PDO") {
					if (wBIdStateContent.CxWtState.WtPDOFS.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Start Day of Week") {
					if (wBIdStateContent.CxWtState.SDOW.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Rest") {
					if (wBIdStateContent.CxWtState.Rest.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Time-Away-From-Base") {
					if (wBIdStateContent.CxWtState.PerDiem.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Trip Length") {
					if (wBIdStateContent.CxWtState.TL.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Work Blk Length") {
					if (wBIdStateContent.CxWtState.WB.Cx) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Work Days") {
					if (wBIdStateContent.CxWtState.WorkDay.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Min Pay") {
					if (wBIdStateContent.CxWtState.MP.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "3-on-3-off") {
					if (wBIdStateContent.CxWtState.No3on3off.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Overlap Days") {
					if (wBIdStateContent.CxWtState.NOL.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Commutable Line - Auto") {
					if (wBIdStateContent.CxWtState.CLAuto.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Commutability") {
					if (wBIdStateContent.CxWtState.Commute.Cx) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
                else if (cellText == "Start Day")
                {
                    if (wBIdStateContent.CxWtState.StartDay.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                else if (cellText == "Report-Release")
                {
                    if (wBIdStateContent.CxWtState.ReportRelease.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
               else if (cellText == "Mixed Hard/Reserve")
                {
                    if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                cell.TextLabel.Text = cellText;


			} else if (PopType == "addWeights") {
				string cellText = WeightsApplied.MainList [indexPath.Row];
				if (cellText == "Aircraft Changes") {
					if (wBIdStateContent.CxWtState.ACChg.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "AM/PM") {
					if (wBIdStateContent.CxWtState.AMPM.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Blocks of Days Off") {
					if (wBIdStateContent.CxWtState.BDO.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Cmut DHs") {
					if (wBIdStateContent.CxWtState.DHD.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Commutable Lines - Manual") {
					if (wBIdStateContent.CxWtState.CL.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Days of the Month") {
					if (wBIdStateContent.CxWtState.SDO.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Days of the Week") {
					if (wBIdStateContent.CxWtState.DOW.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "DH - first - last") {
					if (wBIdStateContent.CxWtState.DHDFoL.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Duty period") {
					if (wBIdStateContent.CxWtState.DP.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Equipment Type") {
					if (wBIdStateContent.CxWtState.EQUIP.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
                else if (cellText == "ETOPS")
                {
                    if (wBIdStateContent.CxWtState.ETOPS.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }

                else if (cellText == "ETOPS-Res")
                {
                    if (wBIdStateContent.CxWtState.ETOPSRes.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                else if (cellText == "Flight Time") {
					if (wBIdStateContent.CxWtState.FLTMIN.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Ground Time") {
					if (wBIdStateContent.CxWtState.GRD.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Intl – NonConus") {
					if (wBIdStateContent.CxWtState.InterConus.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Largest Block of Days Off") {
					if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Legs Per Duty Period") {
					if (wBIdStateContent.CxWtState.LEGS.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Legs Per Pairing") {
					if (wBIdStateContent.CxWtState.LegsPerPairing.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Normalize Days Off") {
					if (wBIdStateContent.CxWtState.NormalizeDays.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Number of Days Off") {
					if (wBIdStateContent.CxWtState.NODO.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Overnight Cities") {
					if (wBIdStateContent.CxWtState.RON.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} 
				else if (cellText == "Cities-Legs") {
					if (wBIdStateContent.CxWtState.CitiesLegs.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} 
				else if (cellText == "Overnight Cities - Bulk") {
					if (wBIdStateContent.CxWtState.BulkOC.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "PDO-after") {
					if (wBIdStateContent.CxWtState.PDAfter.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "PDO-before") {
					if (wBIdStateContent.CxWtState.PDBefore.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Position") {
					if (wBIdStateContent.CxWtState.Position.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Start Day of Week") {
					if (wBIdStateContent.CxWtState.SDOW.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Rest") {
					if (wBIdStateContent.CxWtState.Rest.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Time-Away-From-Base") {
					if (wBIdStateContent.CxWtState.PerDiem.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Trip Length") {
					if (wBIdStateContent.CxWtState.TL.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Work Blk Length") {
					if (wBIdStateContent.CxWtState.WB.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				} else if (cellText == "Work Days") {
					if (wBIdStateContent.CxWtState.WorkDay.Wt) {
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Commutable Line - Auto") {
					if (wBIdStateContent.CxWtState.CLAuto.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (cellText == "Commutability") {
					if (wBIdStateContent.CxWtState.Commute.Wt) {
						cell.Accessory = UITableViewCellAccessory.Checkmark;
					} else {
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				cell.TextLabel.Text = cellText;

			} else if (PopType == "sumColumn") {
				if (indexPath.Section == 0) {
					cell.TextLabel.Text = "Reset";
					cell.Accessory = UITableViewCellAccessory.None;
				} else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
						if (GlobalSettings.AdditionalvacationColumns [indexPath.Row].IsRequied) {
							cell.SelectionStyle = UITableViewCellSelectionStyle.None;
							cell.TextLabel.TextColor = UIColor.LightGray;
						} else {
							cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
							cell.TextLabel.TextColor = UIColor.Black;
						}

						if (GlobalSettings.AdditionalvacationColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;

						string displayTitle = GlobalSettings.AdditionalvacationColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;
					} else {
						if (GlobalSettings.AdditionalColumns [indexPath.Row].IsRequied) {
							cell.SelectionStyle = UITableViewCellSelectionStyle.None;
							cell.TextLabel.TextColor = UIColor.LightGray;
						} else {
							cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
							cell.TextLabel.TextColor = UIColor.Black;
						}

						if (GlobalSettings.AdditionalColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;

						string displayTitle = GlobalSettings.AdditionalColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;
					}
				}
			} else if (PopType == "BidlineColumns") {
				if (indexPath.Section == 0) {
					cell.TextLabel.Text = "Reset";
					cell.Accessory = UITableViewCellAccessory.None;
				} else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
						if (GlobalSettings.BidlineAdditionalvacationColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;
						string displayTitle = GlobalSettings.BidlineAdditionalvacationColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;

					} else {
						if (GlobalSettings.BidlineAdditionalColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;
						string displayTitle = GlobalSettings.BidlineAdditionalColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;				
					}
				}
			} else if (PopType == "ModernColumns") {
				if (indexPath.Section == 0) {
					cell.TextLabel.Text = "Reset";
					cell.Accessory = UITableViewCellAccessory.None;
				} 


				else {
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
						if (GlobalSettings.ModernAdditionalvacationColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;
						string displayTitle = GlobalSettings.ModernAdditionalvacationColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;
					} else {
						if (GlobalSettings.ModernAdditionalColumns [indexPath.Row].IsSelected)
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None;
						string displayTitle = GlobalSettings.ModernAdditionalColumns [indexPath.Row].DisplayName;
						if (displayTitle == "StartDOW")
							displayTitle = "SDOW";
						if (displayTitle == "EDomPush")
							displayTitle = "EDOM";
						if (displayTitle == "FA Posn")
							displayTitle = "FaPos";
						if (displayTitle == "LDomArr")
							displayTitle = "LDOM";
						if (displayTitle == "AvgLatestDomArrivalTime")
							displayTitle = "ALDA";
						if (displayTitle == "AvgEarliestDomPush")
							displayTitle = "AEDP";
						cell.TextLabel.Text = displayTitle;
					}
				}

			} else if (PopType == "blockSort") {
				BlockSort bs = lstblockData [indexPath.Row];
				cell.TextLabel.Text = bs.Name;
			}
            else if (PopType == "changeValueCommutabilitySort") {
                
                cell.TextLabel.Text = CommutabilitySortvalue [indexPath.Row];
            }
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 50;
		}

		static void LoadToStack ()
		{
			StateManagement stateManagement = new StateManagement ();
			stateManagement.UpdateWBidStateContent ();
			WBidHelper.PushToUndoStack ();
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
            UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
            WindowAlert.RootViewController = new UIViewController();
            UIAlertController okAlertController;
           
            // Summary view long tap popover options.
            if (PopType == "sumOpt")
            {
                Console.WriteLine("line??" + CommonClass.selectedLine);
                if (arrPopoverTitle[indexPath.Row] == "Top Lock this line and above")
                {
                    LoadToStack();
                    LineOperations.TopLockThisLineAndAbove(CommonClass.selectedLine);
                }
                else if (arrPopoverTitle[indexPath.Row] == "Bottom Lock this line and below")
                {
                    LoadToStack();
                    LineOperations.BottomLockThisLineAndBelow(CommonClass.selectedLine);
                }
                else if (arrPopoverTitle[indexPath.Row] == "Move selected lines - Above")
                {
                    LoadToStack();
                    bool isneedToshowmessage = LineOperations.MoveSelectedLineAbove(CommonClass.selectedRows, CommonClass.selectedLine);
                    if (isneedToshowmessage)
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        WindowAlert.Dispose();


                    }
                }
                else if (arrPopoverTitle[indexPath.Row] == "Move selected lines - Below")
                {
                    LoadToStack();
                    bool isneedToshowmessage = LineOperations.MoveSelectedLineBelow(CommonClass.selectedRows, CommonClass.selectedLine);
                    if (isneedToshowmessage)
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        WindowAlert.Dispose();
                    }
                }
                else if (arrPopoverTitle[indexPath.Row] == "Deselect all lines")
                {

                }

                if (arrPopoverTitle[indexPath.Row] == "Move selected lines - Above" || arrPopoverTitle[indexPath.Row] == "Move selected lines - Below")
                {
                    wBIdStateContent.SortDetails.SortColumn = "Manual";
                    CommonClass.columnID = 0;
                }

                if (arrPopoverTitle[indexPath.Row] != "Deselect all lines")
                {
                    GlobalSettings.isModified = true;
                    CommonClass.lineVC.UpdateSaveButton();
                }

                tableView.DeselectRow(indexPath, true);
                NSNotificationCenter.DefaultCenter.PostNotificationName("HidePopover", null);
                CommonClass.selectedRows.Clear();
                NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                NSString str = new NSString("none");
                NSNotificationCenter.DefaultCenter.PostNotificationName("ButtonEnableDisable", str);
            }

            // Constraints view + button popover.
            else if (PopType == "addConst")
            {
                PopoverListCell cell = (PopoverListCell)tableView.CellAt(indexPath);
                string cellText = ConstraintsApplied.MainList[indexPath.Row];
                WBidHelper.PushToUndoStack();
                if (cellText == "Aircraft Changes")
                {
                    wBIdStateContent.CxWtState.ACChg.Cx = true;
                    if (wBIdStateContent.CxWtState.ACChg.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyAirCraftChangesConstraint(wBIdStateContent.Constraints.AircraftChanges);
                    }
                    else
                    {
                    }

                }
                else if (cellText == "Blocks of Days Off")
                {
                    wBIdStateContent.CxWtState.BDO.Cx = true;
                    if (wBIdStateContent.CxWtState.BDO.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyBlockOfDaysOffConstraint(wBIdStateContent.Constraints.BlockOfDaysOff);
                    }
                    else
                    {
                    }

                }
                else if (cellText == "Cmut DHs")
                {
                    wBIdStateContent.CxWtState.DHD.Cx = true;
                    if (wBIdStateContent.CxWtState.DHD.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.CmutDHsConstraints.Add(cellText);
                        string second = wBIdStateContent.Constraints.DeadHeads.SecondcellValue;
                        string third = wBIdStateContent.Constraints.DeadHeads.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.DeadHeads.Type;
                        int value = wBIdStateContent.Constraints.DeadHeads.Value;
                        if (wBIdStateContent.Constraints.DeadHeads.LstParameter == null)
                            wBIdStateContent.Constraints.DeadHeads.LstParameter = new List<Cx4Parameter>();
                        wBIdStateContent.Constraints.DeadHeads.LstParameter.Add(new Cx4Parameter
                        {
                            SecondcellValue = second,
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyCommutableDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeads.LstParameter);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Commutable Lines - Manual")
                {
                    if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                            WindowAlert.Dispose();
                            }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        
                    }
                    else
                    {
                        wBIdStateContent.CxWtState.CL.Cx = true;
                        if (wBIdStateContent.CxWtState.CL.Cx)
                        {
                            cell.Accessory = UITableViewCellAccessory.Checkmark;
                            constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
                        }
                        else
                        {
                            cell.Accessory = UITableViewCellAccessory.None;
                        }
                    }
                }
                else if (cellText == "Commutable Line - Auto")
                {
                    if (wBIdStateContent.CxWtState.CL.Cx || wBIdStateContent.CxWtState.CL.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("36") || wBIdStateContent.SortDetails.BlokSort.Contains("37") || wBIdStateContent.SortDetails.BlokSort.Contains("38")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        
                    }
                    else
                    {
                        if (!wBIdStateContent.CxWtState.CLAuto.Cx)
                        {
                            var blocksort = wBIdStateContent.SortDetails.BlokSort;
                            if ((wBIdStateContent.CxWtState.CLAuto.Wt == true || blocksort.Contains("33") || blocksort.Contains("34") || blocksort.Contains("35")) && wBIdStateContent.Constraints.CLAuto != null && wBIdStateContent.Constraints.CLAuto.City != string.Empty)
                            {
                                //we need to show commutable auto pop up only when the values are not already set from constraints ,weights and sorts. 
                                wBIdStateContent.CxWtState.CLAuto.Cx = true;
                                ConstraintCalculations calc = new ConstraintCalculations();
                                calc.ApplyCommutableLinesAutoConstraint(wBIdStateContent.Constraints.CLAuto);
                            }
                            else
                            {
                                //we need to show commutable auto pop up only when the values are not already set from constraints ,weights and sorts
                                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableAuto", null);
                            }
                        }
                    }

                }
                else if (cellText == "Commutability")
                {
                    //					wBIdStateContent.CxWtState.CLAuto.Cx = true;
                    if (wBIdStateContent.Constraints.Commute.City != null && (wBIdStateContent.CxWtState.Commute.Cx || wBIdStateContent.CxWtState.Commute.Wt) || (wBIdStateContent.SortDetails.BlokSort.Contains("30")) || (wBIdStateContent.SortDetails.BlokSort.Contains("31")) || (wBIdStateContent.SortDetails.BlokSort.Contains("32")))
                    {
                        wBIdStateContent.CxWtState.Commute.Cx = true;
                        ConstraintCalculations calc = new ConstraintCalculations();
                        //wBIdStateContent.Constraints.Commute.SecondcellValue = (int)CommutabilitySecondCell.NoMiddle;
                        //wBIdStateContent.Constraints.Commute.ThirdcellValue = (int)CommutabilityThirdCell.Overall;
                        //wBIdStateContent.Constraints.Commute.Type = (int)ConstraintType.LessThan;
                        //wBIdStateContent.Constraints.Commute.Value = 100;
                        calc.ApplyCommuttabilityConstraint(wBIdStateContent.Constraints.Commute);
                    }
                    else
                    {
                        if (!wBIdStateContent.CxWtState.Commute.Cx)
                            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutabilityAuto", null);
                    }
                }
                else if (cellText == "Days of the Week")
                {
                    wBIdStateContent.CxWtState.DOW.Cx = true;
                    if (wBIdStateContent.CxWtState.DOW.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.daysOfWeekConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.DaysOfWeek.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.DaysOfWeek.Type;
                        int value = wBIdStateContent.Constraints.DaysOfWeek.Value;
                        if (wBIdStateContent.Constraints.DaysOfWeek.lstParameters == null)
                            wBIdStateContent.Constraints.DaysOfWeek.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.DaysOfWeek.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyDaysofWeekConstraint(wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Days of the Month")
                {
                    wBIdStateContent.CxWtState.SDO.Cx = true;
                    if (wBIdStateContent.CxWtState.SDO.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                else if (cellText == "DH - first - last")
                {
                    wBIdStateContent.CxWtState.DHDFoL.Cx = true;
                    if (wBIdStateContent.CxWtState.DHDFoL.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.DhFirstLastConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.DeadHeadFoL.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.DeadHeadFoL.Type;
                        int value = wBIdStateContent.Constraints.DeadHeadFoL.Value;
                        if (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters == null)
                            wBIdStateContent.Constraints.DeadHeadFoL.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Duty period")
                {
                    wBIdStateContent.CxWtState.DP.Cx = true;
                    if (wBIdStateContent.CxWtState.DP.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyDutyPeriodConstraint(wBIdStateContent.Constraints.DutyPeriod);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Equipment Type")
                {
                    wBIdStateContent.CxWtState.EQUIP.Cx = true;
                    if (wBIdStateContent.CxWtState.EQUIP.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.EQTypeConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.EQUIP.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.EQUIP.Type;
                        int value = wBIdStateContent.Constraints.EQUIP.Value;
                        if (wBIdStateContent.Constraints.EQUIP.lstParameters == null)
                            wBIdStateContent.Constraints.EQUIP.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.EQUIP.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyEquipmentTypeConstraint(wBIdStateContent.Constraints.EQUIP.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Flight Time")
                {
                    wBIdStateContent.CxWtState.FLTMIN.Cx = true;
                    if (wBIdStateContent.CxWtState.FLTMIN.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyFlightTimeConstraint(wBIdStateContent.Constraints.FlightMin);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Ground Time")
                {
                    wBIdStateContent.CxWtState.GRD.Cx = true;
                    if (wBIdStateContent.CxWtState.GRD.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyGroundTimeConstraint(wBIdStateContent.Constraints.GroundTime);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Intl – NonConus")
                {
                    if (GlobalSettings.WBidINIContent.Cities.Count(x => x.International == true) != 0 || GlobalSettings.WBidINIContent.Cities.Count(x => x.NonConus == true) != 0)
                        wBIdStateContent.CxWtState.InterConus.Cx = true;
                    if (wBIdStateContent.CxWtState.InterConus.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.IntlNonConusConstraints.Add(cellText);
                        List<City> Intlcity = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).ToList();
                        List<City> NonConcity = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).ToList();
                        int type = wBIdStateContent.Constraints.InterConus.Type;
                        int value = wBIdStateContent.Constraints.InterConus.Value;
                        if (type == (int)CityType.International)
                        {
                            if (!Intlcity.Any(x => x.Id == value))
                                value = Intlcity.First().Id;
                        }
                        else if (type == (int)CityType.NonConus)
                        {
                            if (!NonConcity.Any(x => x.Id == value))
                                value = NonConcity.First().Id;
                        }
                        if (wBIdStateContent.Constraints.InterConus.lstParameters == null)
                            wBIdStateContent.Constraints.InterConus.lstParameters = new List<Cx2Parameter>();
                        wBIdStateContent.Constraints.InterConus.lstParameters.Add(new Cx2Parameter
                        {
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyInternationalonConusConstraint(wBIdStateContent.Constraints.InterConus.lstParameters);
                    }

                }
                else if (cellText == "Legs Per Duty Period")
                {
                    wBIdStateContent.CxWtState.LEGS.Cx = true;
                    if (wBIdStateContent.CxWtState.LEGS.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyLegsPerDutyPeriodConstraint(wBIdStateContent.Constraints.LegsPerDutyPeriod);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Legs Per Pairing")
                {
                    wBIdStateContent.CxWtState.LegsPerPairing.Cx = true;
                    if (wBIdStateContent.CxWtState.LegsPerPairing.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyLegsPerPairingConstraint(wBIdStateContent.Constraints.LegsPerPairing);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Number of Days Off")
                {
                    wBIdStateContent.CxWtState.NODO.Cx = true;
                    if (wBIdStateContent.CxWtState.NODO.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyNumberofDaysOffConstraint(wBIdStateContent.Constraints.NumberOfDaysOff);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Overnight Cities")
                {
                    wBIdStateContent.CxWtState.RON.Cx = true;
                    if (wBIdStateContent.CxWtState.RON.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.OvernightCitiesConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.OverNightCities.ThirdcellValue;
                        if (third == "")
                            third = GlobalSettings.OverNightCitiesInBid[0].Name;
                        int type = wBIdStateContent.Constraints.OverNightCities.Type;
                        int value = wBIdStateContent.Constraints.OverNightCities.Value;
                        if (wBIdStateContent.Constraints.OverNightCities.lstParameters == null)
                            wBIdStateContent.Constraints.OverNightCities.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.OverNightCities.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyOvernightCitiesConstraint(wBIdStateContent.Constraints.OverNightCities.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Cities-Legs")
                {
                    wBIdStateContent.CxWtState.CitiesLegs.Cx = true;
                    if (wBIdStateContent.CxWtState.CitiesLegs.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.CitiesLegsConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.CitiesLegs.ThirdcellValue;
                        if (third == "")
                            third = GlobalSettings.WBidINIContent.Cities[0].Name;
                        int type = wBIdStateContent.Constraints.CitiesLegs.Type;
                        int value = wBIdStateContent.Constraints.CitiesLegs.Value;
                        if (wBIdStateContent.Constraints.CitiesLegs.lstParameters == null)
                            wBIdStateContent.Constraints.CitiesLegs.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.CitiesLegs.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyCitiesLegsConstraint(wBIdStateContent.Constraints.CitiesLegs.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Overnight Cities - Bulk")
                {
                    wBIdStateContent.CxWtState.BulkOC.Cx = true;
                    if (wBIdStateContent.CxWtState.BulkOC.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "PDO")
                {
                    wBIdStateContent.CxWtState.WtPDOFS.Cx = true;
                    if (wBIdStateContent.CxWtState.WtPDOFS.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.PDOConstraints.Add(cellText);
                        string second = wBIdStateContent.Constraints.PDOFS.SecondcellValue;
                        string third = wBIdStateContent.Constraints.PDOFS.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.PDOFS.Type;
                        int value = wBIdStateContent.Constraints.PDOFS.Value;
                        if (wBIdStateContent.Constraints.PDOFS.LstParameter == null)
                            wBIdStateContent.Constraints.PDOFS.LstParameter = new List<Cx4Parameter>();
                        wBIdStateContent.Constraints.PDOFS.LstParameter.Add(new Cx4Parameter
                        {
                            SecondcellValue = second,
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyPartialdaysOffConstraint(wBIdStateContent.Constraints.PDOFS.LstParameter);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Start Day of Week")
                {
                    wBIdStateContent.CxWtState.SDOW.Cx = true;
                    if (wBIdStateContent.CxWtState.SDOW.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.StartDayofWeekConstraints.Add(cellText);
                        string second = wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue;
                        string third = wBIdStateContent.Constraints.StartDayOftheWeek.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.StartDayOftheWeek.Type;
                        int value = wBIdStateContent.Constraints.StartDayOftheWeek.Value;
                        if (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters == null)
                            wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.Add(new Cx3Parameter
                        {
                            SecondcellValue = second,
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyStartDayOfWeekConstraint(wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Rest")
                {
                    wBIdStateContent.CxWtState.Rest.Cx = true;
                    if (wBIdStateContent.CxWtState.Rest.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.RestConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.Rest.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.Rest.Type;
                        int value = wBIdStateContent.Constraints.Rest.Value;
                        if (wBIdStateContent.Constraints.Rest.lstParameters == null)
                            wBIdStateContent.Constraints.Rest.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.Rest.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyRestConstraint(wBIdStateContent.Constraints.Rest.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Time-Away-From-Base")
                {
                    wBIdStateContent.CxWtState.PerDiem.Cx = true;
                    if (wBIdStateContent.CxWtState.PerDiem.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyTimeAwayFromBaseConstraint(wBIdStateContent.Constraints.PerDiem);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Trip Length")
                {
                    wBIdStateContent.CxWtState.TL.Cx = true;
                    if (wBIdStateContent.CxWtState.TL.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.TripLengthConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.TripLength.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.TripLength.Type;
                        int value = wBIdStateContent.Constraints.TripLength.Value;
                        if (wBIdStateContent.Constraints.TripLength.lstParameters == null)
                            wBIdStateContent.Constraints.TripLength.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.TripLength.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyTripLengthConstraint(wBIdStateContent.Constraints.TripLength.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Work Blk Length")
                {
                    wBIdStateContent.CxWtState.WB.Cx = true;
                    if (wBIdStateContent.CxWtState.WB.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.WorkBlockLengthConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.WorkBlockLength.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.WorkBlockLength.Type;
                        int value = wBIdStateContent.Constraints.WorkBlockLength.Value;
                        if (wBIdStateContent.Constraints.WorkBlockLength.lstParameters == null)
                            wBIdStateContent.Constraints.WorkBlockLength.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.WorkBlockLength.lstParameters.Add(new Cx3Parameter
                        {
                            ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyWorkBlockLengthConstraint(wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Work Days")
                {
                    wBIdStateContent.CxWtState.WorkDay.Cx = true;
                    if (wBIdStateContent.CxWtState.WorkDay.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyWorkDaysConstraint(wBIdStateContent.Constraints.WorkDay);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Min Pay")
                {
                    wBIdStateContent.CxWtState.MP.Cx = true;
                    if (wBIdStateContent.CxWtState.MP.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyMinimumPayConstraint(wBIdStateContent.Constraints.MinimumPay);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "3-on-3-off")
                {
                    wBIdStateContent.CxWtState.No3on3off.Cx = true;
                    if (wBIdStateContent.CxWtState.No3on3off.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyThreeOn3offConstraint(wBIdStateContent.Constraints.No3On3Off);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Overlap Days")
                {
                    //wBIdStateContent.CxWtState.NOL.Cx = true;
                    if (wBIdStateContent.CxWtState.NOL.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                else if (cellText == "Start Day")
                {
                    wBIdStateContent.CxWtState.StartDay.Cx = true;
                    if (wBIdStateContent.CxWtState.StartDay.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        ConstraintsApplied.StartDayConstraints.Add(cellText);
                        string third = wBIdStateContent.Constraints.StartDay.ThirdcellValue;
                        int type = wBIdStateContent.Constraints.StartDay.Type;
                        int value = wBIdStateContent.Constraints.StartDay.Value;
                        if (wBIdStateContent.Constraints.StartDay.lstParameters == null)
                            wBIdStateContent.Constraints.StartDay.lstParameters = new List<Cx3Parameter>();
                        wBIdStateContent.Constraints.StartDay.lstParameters.Add(new Cx3Parameter
                        {
                           ThirdcellValue = third,
                            Type = type,
                            Value = value
                        });
                        constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Report-Release")
                {
                    wBIdStateContent.CxWtState.ReportRelease.Cx = true;
                    if (wBIdStateContent.CxWtState.ReportRelease.Cx)
                    {
                        if (wBIdStateContent.Constraints.ReportRelease.lstParameters == null)
                            wBIdStateContent.Constraints.ReportRelease.lstParameters = new List<ReportRelease>();
                        wBIdStateContent.Constraints.ReportRelease.lstParameters.Add(new ReportRelease
                        {
                            AllDays = wBIdStateContent.Constraints.ReportRelease.AllDays,
                            First = wBIdStateContent.Constraints.ReportRelease.First,
                            Last=wBIdStateContent.Constraints.ReportRelease.Last,
                            NoMid =wBIdStateContent.Constraints.ReportRelease.NoMid,
                            Report=wBIdStateContent.Constraints.ReportRelease.Report,
                            Release=wBIdStateContent.Constraints.ReportRelease.Release
                                                                                
                        });
                       
                       // cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
                    }
                    else
                    {
                        //cell.Accessory = UITableViewCellAccessory.None;
                    }
                }
                else if(cellText == "Mixed Hard/Reserve")
                {
                    wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx = true;
                    if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        constCalc.ApplyMixedHardandReserveConstraint();
                    }
                    else
                    {
                    }

                }

                NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
                NSNotificationCenter.DefaultCenter.PostNotificationName("dismissPopover", null);

            }
            else if (PopType == "addWeights")
            {
                PopoverListCell cell = (PopoverListCell)tableView.CellAt(indexPath);
                string cellText = WeightsApplied.MainList[indexPath.Row];
                WBidHelper.PushToUndoStack();
                if (cellText == "Aircraft Changes")
                {
                    wBIdStateContent.CxWtState.ACChg.Wt = true;
                    if (wBIdStateContent.CxWtState.ACChg.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        //						int second = wBIdStateContent.Weights.AirCraftChanges.SecondlValue;
                        //						int third = wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue;
                        //						decimal weight = wBIdStateContent.Weights.AirCraftChanges.Weight;
                        weightCalc.ApplyAirCraftChangeWeight(wBIdStateContent.Weights.AirCraftChanges);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Commutable Line - Auto")
                {
                    if (wBIdStateContent.CxWtState.CL.Cx || wBIdStateContent.CxWtState.CL.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("36") || wBIdStateContent.SortDetails.BlokSort.Contains("37") || wBIdStateContent.SortDetails.BlokSort.Contains("38")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                       
                    }
                    else
                    {
                        if (!wBIdStateContent.CxWtState.CLAuto.Wt)
                        {
                            var blocksort = wBIdStateContent.SortDetails.BlokSort;
                            if ((wBIdStateContent.CxWtState.CLAuto.Cx == true || blocksort.Contains("33") || blocksort.Contains("34") || blocksort.Contains("35")) && wBIdStateContent.Weights.CLAuto != null && wBIdStateContent.Weights.CLAuto.City != string.Empty)
                            {
                                //we need to show commutable auto pop up only when the values are not already set from constraints ,weights and sorts. 
                                wBIdStateContent.CxWtState.CLAuto.Wt = true;
                                WeightCalculation calc = new WeightCalculation();
                                calc.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                            }
                            else
                            {

                                NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableWeightAuto", null);
                            }
                        }
                    }
                }
                else if (cellText == "Commutability")
                {
                    if (wBIdStateContent.Constraints.Commute.City != null && (wBIdStateContent.CxWtState.Commute.Cx || wBIdStateContent.CxWtState.Commute.Wt) || (wBIdStateContent.SortDetails.BlokSort.Contains("30")) || (wBIdStateContent.SortDetails.BlokSort.Contains("31")) || (wBIdStateContent.SortDetails.BlokSort.Contains("32")))
                    {

                        wBIdStateContent.CxWtState.Commute.Wt = true;
                        WeightCalculation calc = new WeightCalculation();
                        calc.ApplyCommuttabilityWeight(wBIdStateContent.Weights.Commute);
                    }
                    else
                    {
                        if (!wBIdStateContent.CxWtState.Commute.Wt)
                            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutabilityWeightAuto", null);
                    }
                }
                else if (cellText == "AM/PM")
                {
                    wBIdStateContent.CxWtState.AMPM.Wt = true;
                    if (wBIdStateContent.CxWtState.AMPM.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.AMPMWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.AM_PM.Type;
                        decimal weight = wBIdStateContent.Weights.AM_PM.Weight;
                        if (wBIdStateContent.Weights.AM_PM.lstParameters == null)
                            wBIdStateContent.Weights.AM_PM.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.AM_PM.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyAMPMWeight(wBIdStateContent.Weights.AM_PM.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Blocks of Days Off")
                {
                    wBIdStateContent.CxWtState.BDO.Wt = true;
                    if (wBIdStateContent.CxWtState.BDO.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.BlocksOfDaysOffWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.BDO.SecondlValue;
                        int third = wBIdStateContent.Weights.BDO.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.BDO.Weight;
                        if (wBIdStateContent.Weights.BDO.lstParameters == null)
                            wBIdStateContent.Weights.BDO.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.BDO.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyBlockOFFDaysOfWeight(wBIdStateContent.Weights.BDO.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Cmut DHs")
                {
                    wBIdStateContent.CxWtState.DHD.Wt = true;
                    if (wBIdStateContent.CxWtState.DHD.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.CmutDHsWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.DHD.SecondlValue;
                        int third = wBIdStateContent.Weights.DHD.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.DHD.Weight;
                        if (wBIdStateContent.Weights.DHD.lstParameters == null)
                            wBIdStateContent.Weights.DHD.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.DHD.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyCommutableDeadhead(wBIdStateContent.Weights.DHD.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Commutable Lines - Manual")
                {
                    if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        
                    }
                    else
                    {
                        wBIdStateContent.CxWtState.CL.Wt = true;
                        if (wBIdStateContent.CxWtState.CL.Wt)
                        {
                            cell.Accessory = UITableViewCellAccessory.Checkmark;
                        }
                        else
                        {
                        }
                    }
                }
                else if (cellText == "Days of the Month")
                {
                    wBIdStateContent.CxWtState.SDO.Wt = true;
                    if (wBIdStateContent.CxWtState.SDO.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Days of the Week")
                {
                    wBIdStateContent.CxWtState.DOW.Wt = true;
                    if (wBIdStateContent.CxWtState.DOW.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "DH - first - last")
                {
                    wBIdStateContent.CxWtState.DHDFoL.Wt = true;
                    if (wBIdStateContent.CxWtState.DHDFoL.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.dhFirstLastWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.DHDFoL.Type;
                        decimal weight = wBIdStateContent.Weights.DHDFoL.Weight;
                        if (wBIdStateContent.Weights.DHDFoL.lstParameters == null)
                            wBIdStateContent.Weights.DHDFoL.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.DHDFoL.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyDeadheadFisrtLastWeight(wBIdStateContent.Weights.DHDFoL.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Duty period")
                {
                    wBIdStateContent.CxWtState.DP.Wt = true;
                    if (wBIdStateContent.CxWtState.DP.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.DutyPeriodWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.DP.SecondlValue;
                        int third = wBIdStateContent.Weights.DP.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.DP.Weight;
                        if (wBIdStateContent.Weights.DP.lstParameters == null)
                            wBIdStateContent.Weights.DP.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.DP.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyDutyPeriodWeight(wBIdStateContent.Weights.DP.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Equipment Type")
                {
                    wBIdStateContent.CxWtState.EQUIP.Wt = true;
                    if (wBIdStateContent.CxWtState.EQUIP.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.EQTypeWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.EQUIP.SecondlValue;
                        int third = wBIdStateContent.Weights.EQUIP.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.EQUIP.Weight;
                        if (wBIdStateContent.Weights.EQUIP.lstParameters == null)
                            wBIdStateContent.Weights.EQUIP.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.EQUIP.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyEquipmentTypeWeights(wBIdStateContent.Weights.EQUIP.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "ETOPS")
                {
                    wBIdStateContent.CxWtState.ETOPS.Wt = true;
                    if (wBIdStateContent.CxWtState.ETOPS.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.ETOPSWeights.Add(cellText);

                        decimal weight = wBIdStateContent.Weights.ETOPS.Weight;
                        if (wBIdStateContent.Weights.ETOPS.lstParameters == null)
                            wBIdStateContent.Weights.ETOPS.lstParameters = new List<Wt1Parameter>();
                        wBIdStateContent.Weights.ETOPS.lstParameters.Add(new Wt1Parameter
                        {
                          
                            Weight = weight
                        });
                        weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "ETOPS-Res")
                {
                    wBIdStateContent.CxWtState.ETOPSRes.Wt = true;
                    if (wBIdStateContent.CxWtState.ETOPSRes.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.ETOPSResWeights.Add(cellText);

                        decimal weight = wBIdStateContent.Weights.ETOPSRes.Weight;
                        if (wBIdStateContent.Weights.ETOPSRes.lstParameters == null)
                            wBIdStateContent.Weights.ETOPSRes.lstParameters = new List<Wt1Parameter>();
                        wBIdStateContent.Weights.ETOPSRes.lstParameters.Add(new Wt1Parameter
                        {

                            Weight = weight
                        });
                        weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Flight Time")
                {
                    wBIdStateContent.CxWtState.FLTMIN.Wt = true;
                    if (wBIdStateContent.CxWtState.FLTMIN.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.FlightTimeWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.FLTMIN.SecondlValue;
                        int third = wBIdStateContent.Weights.FLTMIN.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.FLTMIN.Weight;
                        if (wBIdStateContent.Weights.FLTMIN.lstParameters == null)
                            wBIdStateContent.Weights.FLTMIN.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.FLTMIN.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyFlightTimeWeights(wBIdStateContent.Weights.FLTMIN.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Ground Time")
                {
                    wBIdStateContent.CxWtState.GRD.Wt = true;
                    if (wBIdStateContent.CxWtState.GRD.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.GroundTimeWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.GRD.SecondlValue;
                        int third = wBIdStateContent.Weights.GRD.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.GRD.Weight;
                        if (wBIdStateContent.Weights.GRD.lstParameters == null)
                            wBIdStateContent.Weights.GRD.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.GRD.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyGroundTimeWeight(wBIdStateContent.Weights.GRD.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Intl – NonConus")
                {
                    wBIdStateContent.CxWtState.InterConus.Wt = true;
                    if (wBIdStateContent.CxWtState.InterConus.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.IntlNonConusWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.InterConus.Type;
                        decimal weight = wBIdStateContent.Weights.InterConus.Weight;
                        if (wBIdStateContent.Weights.InterConus.lstParameters == null)
                            wBIdStateContent.Weights.InterConus.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.InterConus.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyInternationalNonConusWeight(wBIdStateContent.Weights.InterConus.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Largest Block of Days Off")
                {
                    wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt = true;
                    weightCalc.ApplyLargestBlockOfDaysoffWeight(wBIdStateContent.Weights.LrgBlkDayOff);
                    if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Legs Per Duty Period")
                {
                    wBIdStateContent.CxWtState.LEGS.Wt = true;
                    if (wBIdStateContent.CxWtState.LEGS.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.LegsPerDutyPeriodWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.LEGS.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.LEGS.Weight;
                        int second = wBIdStateContent.Weights.LEGS.SecondlValue;
                        if (wBIdStateContent.Weights.LEGS.lstParameters == null)
                            wBIdStateContent.Weights.LEGS.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.LEGS.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyLegsPerDutyPeriodWeight(wBIdStateContent.Weights.LEGS.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Legs Per Pairing")
                {
                    wBIdStateContent.CxWtState.LegsPerPairing.Wt = true;
                    if (wBIdStateContent.CxWtState.LegsPerPairing.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.LegsPerPairingWeights.Add(cellText);
                        int second = wBIdStateContent.Weights.WtLegsPerPairing.SecondlValue;
                        int third = wBIdStateContent.Weights.WtLegsPerPairing.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WtLegsPerPairing.Weight;
                        if (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters == null)
                            wBIdStateContent.Weights.WtLegsPerPairing.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyLegsPerPairingWeight(wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Normalize Days Off")
                {
                    wBIdStateContent.CxWtState.NormalizeDays.Wt = true;
                    weightCalc.ApplyNormalizeDaysOffWeight(wBIdStateContent.Weights.NormalizeDaysOff);
                    if (wBIdStateContent.CxWtState.NormalizeDays.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Number of Days Off")
                {
                    wBIdStateContent.CxWtState.NODO.Wt = true;
                    if (wBIdStateContent.CxWtState.NODO.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.NumOfDaysOffWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.NODO.Type;
                        decimal weight = wBIdStateContent.Weights.NODO.Weight;
                        if (wBIdStateContent.Weights.NODO.lstParameters == null)
                            wBIdStateContent.Weights.NODO.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.NODO.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyNumberOfDaysOfWeight(wBIdStateContent.Weights.NODO.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Overnight Cities")
                {
                    wBIdStateContent.CxWtState.RON.Wt = true;
                    if (wBIdStateContent.CxWtState.RON.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.OvernightCitiesWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.RON.Type;
                        decimal weight = wBIdStateContent.Weights.RON.Weight;
                        if (wBIdStateContent.Weights.RON.lstParameters == null)
                            wBIdStateContent.Weights.RON.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.RON.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyOverNightCitiesWeight(wBIdStateContent.Weights.RON.lstParameters);
                    }
                }
                else if (cellText == "Cities-Legs")
                {
                    wBIdStateContent.CxWtState.CitiesLegs.Wt = true;
                    if (wBIdStateContent.CxWtState.CitiesLegs.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.CitiesLegsWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.CitiesLegs.Type;
                        decimal weight = wBIdStateContent.Weights.CitiesLegs.Weight;
                        if (wBIdStateContent.Weights.CitiesLegs.lstParameters == null)
                            wBIdStateContent.Weights.CitiesLegs.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.CitiesLegs.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyCitiesLegsWeight(wBIdStateContent.Weights.CitiesLegs.lstParameters);
                    }
                }
                else if (cellText == "Overnight Cities - Bulk")
                {
                    wBIdStateContent.CxWtState.BulkOC.Wt = true;
                    if (wBIdStateContent.CxWtState.BulkOC.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                    }
                    else
                    {
                    }
                }
                else if (cellText == "PDO-after")
                {
                    wBIdStateContent.CxWtState.PDAfter.Wt = true;
                    if (wBIdStateContent.CxWtState.PDAfter.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.PDOAfterWeights.Add(cellText);
                        int first = wBIdStateContent.Weights.PDAfter.FirstValue;
                        int second = wBIdStateContent.Weights.PDAfter.SecondlValue;
                        int third = wBIdStateContent.Weights.PDAfter.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.PDAfter.Weight;
                        if (wBIdStateContent.Weights.PDAfter.lstParameters == null)
                            wBIdStateContent.Weights.PDAfter.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.PDAfter.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPartialDaysAfterWeight(wBIdStateContent.Weights.PDAfter.lstParameters);
                    }
                    else
                    {
                    }
                    cell.Accessory = UITableViewCellAccessory.None;
                }
                else if (cellText == "PDO-before")
                {
                    wBIdStateContent.CxWtState.PDBefore.Wt = true;
                    if (wBIdStateContent.CxWtState.PDBefore.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.PDOBeforeWeights.Add(cellText);
                        int first = wBIdStateContent.Weights.PDBefore.FirstValue;
                        int second = wBIdStateContent.Weights.PDBefore.SecondlValue;
                        int third = wBIdStateContent.Weights.PDBefore.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.PDBefore.Weight;
                        if (wBIdStateContent.Weights.PDBefore.lstParameters == null)
                            wBIdStateContent.Weights.PDBefore.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.PDBefore.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPartialDaysBeforeWeight(wBIdStateContent.Weights.PDBefore.lstParameters);
                    }
                    else
                    {
                    }
                    cell.Accessory = UITableViewCellAccessory.None;
                }
                else if (cellText == "Position")
                {
                    wBIdStateContent.CxWtState.Position.Wt = true;
                    if (wBIdStateContent.CxWtState.Position.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.PositionWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.POS.Type;
                        decimal weight = wBIdStateContent.Weights.POS.Weight;
                        if (wBIdStateContent.Weights.POS.lstParameters == null)
                            wBIdStateContent.Weights.POS.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.POS.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPositionWeight(wBIdStateContent.Weights.POS.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Start Day of Week")
                {
                    wBIdStateContent.CxWtState.SDOW.Wt = true;
                    if (wBIdStateContent.CxWtState.SDOW.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.StartDOWWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.SDOW.Type;
                        decimal weight = wBIdStateContent.Weights.SDOW.Weight;
                        if (wBIdStateContent.Weights.SDOW.lstParameters == null)
                            wBIdStateContent.Weights.SDOW.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.SDOW.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyStartDayOfWeekWeight(wBIdStateContent.Weights.SDOW.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Rest")
                {
                    wBIdStateContent.CxWtState.Rest.Wt = true;
                    if (wBIdStateContent.CxWtState.Rest.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.RestWeights.Add(cellText);
                        int first = wBIdStateContent.Weights.WtRest.FirstValue;
                        int second = wBIdStateContent.Weights.WtRest.SecondlValue;
                        int third = wBIdStateContent.Weights.WtRest.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WtRest.Weight;
                        if (wBIdStateContent.Weights.WtRest.lstParameters == null)
                            wBIdStateContent.Weights.WtRest.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.WtRest.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyRestWeight(wBIdStateContent.Weights.WtRest.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Time-Away-From-Base")
                {
                    wBIdStateContent.CxWtState.PerDiem.Wt = true;
                    if (wBIdStateContent.CxWtState.PerDiem.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.Checkmark;
                        //						int third = wBIdStateContent.Weights.PerDiem.Type;
                        //						decimal weight = wBIdStateContent.Weights.PerDiem.Weight;
                        weightCalc.ApplyTimeAwayFromBaseWeight(wBIdStateContent.Weights.PerDiem);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Trip Length")
                {
                    wBIdStateContent.CxWtState.TL.Wt = true;
                    if (wBIdStateContent.CxWtState.TL.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.TripLengthWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.TL.Type;
                        decimal weight = wBIdStateContent.Weights.TL.Weight;
                        if (wBIdStateContent.Weights.TL.lstParameters == null)
                            wBIdStateContent.Weights.TL.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.TL.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyTripLengthWeight(wBIdStateContent.Weights.TL.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Work Blk Length")
                {
                    wBIdStateContent.CxWtState.WB.Wt = true;
                    if (wBIdStateContent.CxWtState.WB.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.WorkBlockLengthWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.WB.Type;
                        decimal weight = wBIdStateContent.Weights.WB.Weight;
                        if (wBIdStateContent.Weights.WB.lstParameters == null)
                            wBIdStateContent.Weights.WB.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.WB.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyWorkBlockLengthWeight(wBIdStateContent.Weights.WB.lstParameters);
                    }
                    else
                    {
                    }
                }
                else if (cellText == "Work Days")
                {
                    wBIdStateContent.CxWtState.WorkDay.Wt = true;
                    if (wBIdStateContent.CxWtState.WorkDay.Wt)
                    {
                        cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                        WeightsApplied.WorkDaysWeights.Add(cellText);
                        int third = wBIdStateContent.Weights.WorkDays.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WorkDays.Weight;
                        int second = wBIdStateContent.Weights.WorkDays.SecondlValue;
                        if (wBIdStateContent.Weights.WorkDays.lstParameters == null)
                            wBIdStateContent.Weights.WorkDays.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.WorkDays.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyWorkDaysWeight(wBIdStateContent.Weights.WorkDays.lstParameters);
                    }
                    else
                    {
                    }
                }

                NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
                NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);

            }



            // Summary view long tap header, additional columns popover.
            else if (PopType == "sumColumn")
            {
                string cellText = GlobalSettings.AdditionalvacationColumns[indexPath.Row].DisplayName;
                if (indexPath.Section == 0)
                {

                    // when user clicks the reset option
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.SummaryVacationColumns = WBidCollection.GenerateDefaultVacationColumns();
                        XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                        GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                        foreach (var column in GlobalSettings.AdditionalvacationColumns)
                        {
                            column.IsSelected = false;
                        }
                        var selectedColumns = GlobalSettings.AdditionalvacationColumns.Where(x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(y => y.Id == x.Id)).ToList();
                        foreach (var selectedColumn in selectedColumns)
                        {
                            selectedColumn.IsSelected = true;
                        }
                        this.reload(tableView);
                    }


                    else
                    {
                        GlobalSettings.WBidINIContent.DataColumns = WBidCollection.GenerateDefaultColumns();
                        XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                        GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                        foreach (var column in GlobalSettings.AdditionalColumns)
                        {
                            column.IsSelected = false;
                        }
                        var selectedColumns = GlobalSettings.AdditionalColumns.Where(x => GlobalSettings.WBidINIContent.DataColumns.Any(y => y.Id == x.Id)).ToList();
                        foreach (var selectedColumn in selectedColumns)
                        {
                            selectedColumn.IsSelected = true;
                        }
                        this.reload(tableView);
                    }
                }
               

                else
                {
                    AddSummaryViewColumns(tableView, indexPath);
                }
            }
            else if (PopType == "BidlineColumns")
            {
                string cellText = GlobalSettings.AdditionalvacationColumns[indexPath.Row].DisplayName;
                if (indexPath.Section == 0)
                {

                    //List<AdditionalColumns> bidLineSelectedList = null;

                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };
                        LineSummaryBL.GetBidlineViewAdditionalVacationColumns();
                        LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList();
                        //bidLineSelectedList = GlobalSettings.BidlineAdditionalvacationColumns.Where(x => x.IsSelected).ToList ();
                        CommonClass.bidLineProperties = new List<string>();
                        foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns)
                        {
                            var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
                            if (col != null)
                            {
                                CommonClass.bidLineProperties.Add(col.DisplayName);
                            }

                        }

                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
                        LineSummaryBL.GetBidlineViewAdditionalColumns();
                        LineSummaryBL.SetSelectedBidLineColumnstoGlobalList();
                        //bidLineSelectedList = GlobalSettings.BidlineAdditionalColumns.Where(x => x.IsSelected).ToList ();
                        CommonClass.bidLineProperties = new List<string>();
                        foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns)
                        {
                            var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault(x => x.Id == item);
                            if (col != null)
                            {
                                CommonClass.bidLineProperties.Add(col.DisplayName);
                            }

                        }
                    }

                    tableView.ReloadData();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                }
                //else if (cellText == "Ratio")
                //{
                //	if ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) &&
                //		GlobalSettings.WBidINIContent.BidLineVacationColumns.Count >= 5)
                //	{



                //	}
                //	else if (GlobalSettings.WBidINIContent.BidLineNormalColumns.Count >= 5)
                //	{
               
                //	}
                //	else
                //	{
                //		NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
                //	}
                //}

                else
                {

                    AddBidlineViewColumns(tableView, indexPath);

                }
            }

            else if (PopType == "changeValueCommutabilitySort")
            {

               
                var commuteitem = wBIdStateContent.SortDetails.BlokSort.FirstOrDefault(x => x == "33" || x == "34" || x == "35");
                int index = wBIdStateContent.SortDetails.BlokSort.FindIndex(x => x == "33" || x == "34" || x == "35");
                if (commuteitem != null)
                {
                    if (indexPath.Row == 0)
                    {
                        wBIdStateContent.SortDetails.BlokSort[index] = "33";
                    }
                    else if (indexPath.Row == 1)
                    {
                        wBIdStateContent.SortDetails.BlokSort[index] = "34";
                    }
                    else if (indexPath.Row == 2)
                    {
                        wBIdStateContent.SortDetails.BlokSort[index] = "35";
                    }

                }
                else
                {
                    commuteitem = wBIdStateContent.SortDetails.BlokSort.FirstOrDefault(x => x == "36" || x == "37" || x == "38");
                    index = wBIdStateContent.SortDetails.BlokSort.FindIndex(x => x == "36" || x == "37" || x == "38");
                    if (commuteitem != null)
                    {
                        if (indexPath.Row == 0)
                        {
                            wBIdStateContent.SortDetails.BlokSort[index] = "36";
                        }
                        else if (indexPath.Row == 1)
                        {
                            wBIdStateContent.SortDetails.BlokSort[index] = "37";
                        }
                        else if (indexPath.Row == 2)
                        {
                            wBIdStateContent.SortDetails.BlokSort[index] = "38";
                        }

                    }
                }

                NSNotificationCenter.DefaultCenter.PostNotificationName("dismissSortPopover", null);
                NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);

                //CommutabilitySortvalue [indexPath.Row] == 1
            }

            else if (PopType == "ModernColumns")
            {
                string cellText = GlobalSettings.AdditionalvacationColumns[indexPath.Row].DisplayName;
                if (indexPath.Section == 0)
                {


                    List<AdditionalColumns> modernSelectedList = null;

                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };
                        LineSummaryBL.GetModernViewAdditionalVacationalColumns();
                        LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList();
                        //modernSelectedList = GlobalSettings.ModernAdditionalvacationColumns.Where(x => x.IsSelected).ToList();
                        CommonClass.modernProperties = new List<string>();
                        foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns)
                        {
                            var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
                            if (col != null)
                            {
                                CommonClass.modernProperties.Add(col.DisplayName);
                            }

                        }
                    }
                    else
                    {
                        GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
                        LineSummaryBL.GetModernViewAdditionalColumns();
                        LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList();
                        // modernSelectedList = GlobalSettings.ModernAdditionalColumns.Where(x => x.IsSelected).ToList();
                        CommonClass.modernProperties = new List<string>();
                        foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns)
                        {
                            var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault(x => x.Id == item);
                            if (col != null)
                            {
                                CommonClass.modernProperties.Add(col.DisplayName);
                            }

                        }

                    }



                    tableView.ReloadData();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                }
                //else if (cellText == "Ratio")
                //{
                //	if ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) &&
                //	    GlobalSettings.WBidINIContent.ModernVacationColumns.Count >= 5)
                //	{

              

                //	}
                //	else if (GlobalSettings.WBidINIContent.ModernNormalColumns.Count >= 5)
                //	{
            
                //	}
                //	else
                //	{
                //		NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
                //	}
                //}
                else
                {
                    AddModernBidlineviewcolumns(tableView, indexPath);
                }
            }
            else if (PopType == "blockSort")
            {

                if ((WBidCollection.GetBlockSortListDataCSW()[indexPath.Row].Name) == "Commutability")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);
                    if (wBIdStateContent.CxWtState.Commute.Cx == false && wBIdStateContent.CxWtState.Commute.Wt == false)
                    {
                        wBIdStateContent.Constraints.Commute = new Commutability
                        {
                            BaseTime = 10,
                            ConnectTime = 30,
                            CheckInTime = 120,
                            SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
                            ThirdcellValue = (int)CommutabilityThirdCell.Overall,
                            Type = (int)ConstraintType.MoreThan,
                            Value = 100
                        };
                        NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutabilitySort", null);
                    }
                    else
                    {
                        if (!((wBIdStateContent.SortDetails.BlokSort.Contains("30")) || (wBIdStateContent.SortDetails.BlokSort.Contains("31")) || (wBIdStateContent.SortDetails.BlokSort.Contains("32"))))
                        {

                            wBIdStateContent.SortDetails.BlokSort.Add("30");
                        }
                        NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
                    }
                }

                else if ((WBidCollection.GetBlockSortListDataCSW()[indexPath.Row].Name) == "Ratio")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreenSort", null);
                }

                else if ((WBidCollection.GetBlockSortListDataCSW()[indexPath.Row].Name) == "Commutable Line-Auto")
                {
                    if (wBIdStateContent.CxWtState.CL.Cx || wBIdStateContent.CxWtState.CL.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("36") ||
                        wBIdStateContent.SortDetails.BlokSort.Contains("37") || wBIdStateContent.SortDetails.BlokSort.Contains("38")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                        
                    }
                    else
                    {

                        if (wBIdStateContent.CxWtState.CLAuto.Cx == false && wBIdStateContent.CxWtState.CLAuto.Wt == false)
                        {
                            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableAutoSortPopUp", null);
                        }
                        else
                        {
                            if (!((wBIdStateContent.SortDetails.BlokSort.Contains("33")) || (wBIdStateContent.SortDetails.BlokSort.Contains("34")) || (wBIdStateContent.SortDetails.BlokSort.Contains("35"))))
                            {

                                wBIdStateContent.SortDetails.BlokSort.Add("33");
                            }
                            NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
                        }

                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);


                }
                else if ((WBidCollection.GetBlockSortListDataCSW()[indexPath.Row].Name) == "Commutable Line- Manual")
                {
                    if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") ||
                        wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                    {
                        okAlertController = UIAlertController.Create("WBidMax", "You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
                        WindowAlert.MakeKeyAndVisible();
                        WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                       
                    }
                    else
                    {

                        if (wBIdStateContent.CxWtState.CL.Cx == false && wBIdStateContent.CxWtState.CL.Wt == false) // Todo : remove the not operator
                        {
                            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableManualSortPopUp", null);
                        }
                        else
                        {
                            if (!((wBIdStateContent.SortDetails.BlokSort.Contains("36")) || (wBIdStateContent.SortDetails.BlokSort.Contains("37")) || (wBIdStateContent.SortDetails.BlokSort.Contains("38"))))
                            {

                                wBIdStateContent.SortDetails.BlokSort.Add("36");
                            }
                            NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
                        }

                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);


                }
                else
                {
                    WBidHelper.PushToUndoStack();
                    string value = lstblockData[indexPath.Row].Id.ToString();
                    if (!wBIdStateContent.SortDetails.BlokSort.Contains(value))
                    {
                        wBIdStateContent.SortDetails.BlokSort.Add(value);
                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("dismissWtPopover", null);

                }
            }
		}

		static void AddModernBidlineviewcolumns(UITableView tableView, NSIndexPath indexPath)
		{
			AdditionalColumns tappedColumn = null;
			bool isUntick = false;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				tappedColumn = GlobalSettings.ModernAdditionalvacationColumns[indexPath.Row];
				isUntick = tappedColumn.IsSelected;
				if (tappedColumn.DisplayName == "Ratio")
				{
					tappedColumn.IsSelected = false;

				}
				else
				{
					tappedColumn.IsSelected = !tappedColumn.IsSelected;

				}
				GlobalSettings.ModernAdditionalvacationColumns = GlobalSettings.ModernAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
			}
			else {
				tappedColumn = GlobalSettings.ModernAdditionalColumns[indexPath.Row];
				isUntick = tappedColumn.IsSelected;
				if (tappedColumn.DisplayName == "Ratio")
				{
					tappedColumn.IsSelected = false;

				}
				else
				{
					tappedColumn.IsSelected = !tappedColumn.IsSelected;

				}
					//tappedColumn.IsSelected = !tappedColumn.IsSelected;
				GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
				//}
			}

			if (!isUntick && CommonClass.modernProperties.Count >= 5)
			{
				tappedColumn.IsSelected = false;
                UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                WindowAlert.RootViewController = new UIViewController();
                UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    WindowAlert.Dispose();
                }));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
               



            }

            else {
				if (CommonClass.modernProperties.Contains(tappedColumn.DisplayName))
				{
					CommonClass.modernProperties.Remove(tappedColumn.DisplayName);
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
					{
						GlobalSettings.WBidINIContent.ModernVacationColumns.Remove(tappedColumn.Id);

					}
					else {
						GlobalSettings.WBidINIContent.ModernNormalColumns.Remove(tappedColumn.Id);
					}
					//tappedColumn.IsSelected = false;
				}
				else {
                    //string cellText = GlobalSettings.ad[indexPath.Row].DisplayName;
					if (tappedColumn.DisplayName == "Ratio")
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
					}
					else
					{
						CommonClass.modernProperties.Add(tappedColumn.DisplayName);
						// tappedColumn.IsSelected = true;
						if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
						{
							GlobalSettings.WBidINIContent.ModernVacationColumns.Add(tappedColumn.Id);

						}
						else
						{
							GlobalSettings.WBidINIContent.ModernNormalColumns.Add(tappedColumn.Id);
						}
					}

				}
				tableView.ReloadData();
			}

            //commented by Roshil on 26-8-2021 to allow the user to set less than 5 properties
            //         if (CommonClass.modernProperties.Count == 5)
            //{
            //	NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
            //}
            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
        }

		static void AddBidlineViewColumns(UITableView tableView, NSIndexPath indexPath)
		{
			AdditionalColumns tappedColumn = null;
			bool isUntick = false;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				tappedColumn = GlobalSettings.BidlineAdditionalvacationColumns[indexPath.Row];
				isUntick = tappedColumn.IsSelected;
				if (tappedColumn.DisplayName == "Ratio")
				{
					tappedColumn.IsSelected = false;

				}
				else
				{
					tappedColumn.IsSelected = !tappedColumn.IsSelected;

				}
				//tappedColumn = GlobalSettings.BidlineAdditionalvacationColumns[indexPath.Row];
				//tappedColumn.IsSelected = !tappedColumn.IsSelected;
				GlobalSettings.BidlineAdditionalvacationColumns = GlobalSettings.BidlineAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
			}
			else 
			{
				tappedColumn = GlobalSettings.BidlineAdditionalColumns[indexPath.Row];
				isUntick = tappedColumn.IsSelected;
				if (tappedColumn.DisplayName == "Ratio")
				{
					tappedColumn.IsSelected = false;

				}
				else
				{
					tappedColumn.IsSelected = !tappedColumn.IsSelected;

				}
				//tappedColumn = GlobalSettings.BidlineAdditionalColumns[indexPath.Row];
				//tappedColumn.IsSelected = !tappedColumn.IsSelected;
				GlobalSettings.BidlineAdditionalColumns = GlobalSettings.BidlineAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
			}


			if (tappedColumn.IsSelected && CommonClass.bidLineProperties.Count >= 5)
			{
				tappedColumn.IsSelected = false;
                
                    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                    WindowAlert.RootViewController = new UIViewController();
                    UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                        WindowAlert.Dispose();
                    }));
                    WindowAlert.MakeKeyAndVisible();
                    WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                   


            }
			else {
				if (CommonClass.bidLineProperties.Contains(tappedColumn.DisplayName))
				{
					CommonClass.bidLineProperties.Remove(tappedColumn.DisplayName);
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
					{
						GlobalSettings.WBidINIContent.BidLineVacationColumns.Remove(tappedColumn.Id);

					}
					else {
						GlobalSettings.WBidINIContent.BidLineNormalColumns.Remove(tappedColumn.Id);
					}

					//tappedColumn.IsSelected = false;
				}
				else {
                    // string cellText = GlobalSettings.AdditionalvacationColumns[indexPath.Row].DisplayName;
					if (tappedColumn.DisplayName == "Ratio")
					{
						
						NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
					}
					else
					{
						CommonClass.bidLineProperties.Add(tappedColumn.DisplayName);
						// tappedColumn.IsSelected = true;
						if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
						{
							GlobalSettings.WBidINIContent.BidLineVacationColumns.Add(tappedColumn.Id);

						}
						else
						{
							GlobalSettings.WBidINIContent.BidLineNormalColumns.Add(tappedColumn.Id);
						}
					}

				}
				tableView.ReloadData();
			}



			//if (CommonClass.bidLineProperties.Count == 5)
			//{
				NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			//}
		}

		void AddSummaryViewColumns(UITableView tableView, NSIndexPath indexPath)
		{
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				AdditionalColumns tappedColumn = GlobalSettings.AdditionalvacationColumns[indexPath.Row];
				if (!tappedColumn.IsRequied)
				{
					if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count < 18)
					{
						
						if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 0)
						{
							string cellText = GlobalSettings.AdditionalvacationColumns[indexPath.Row].DisplayName;
							if (cellText=="Ratio")
							{
								//GlobalSettings.AdditionalvacationColumns[indexPath.Row].IsSelected = true;
								NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
							}
							else
							{
								GlobalSettings.WBidINIContent.SummaryVacationColumns.Add(new DataColumn()
								{
									Id = tappedColumn.Id,
									Width = 50,
									Order = GlobalSettings.WBidINIContent.SummaryVacationColumns.Count
								});
								GlobalSettings.AdditionalvacationColumns[indexPath.Row].IsSelected = true;
								XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
								GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                                this.PerformSelector(new ObjCRuntime.Selector("reloadTable:"), tableView, 0.2);
                                this.reload(tableView); //Uncommented this on April 22 2021 
							}
							//this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
						}
						else {
							GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
							GlobalSettings.AdditionalvacationColumns[indexPath.Row].IsSelected = false;
							XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
							GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                            this.reload(tableView);//Uncommented Roshil  on April 22 2021 
                                                   //this.PerformSelector (new ObjCRuntime.Selector ("reloadTable:"), tableView, 0.2); //commented this on April 22 2021 
                        }
                    }
					else {
						if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 1)
						{
							GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
							GlobalSettings.AdditionalvacationColumns[indexPath.Row].IsSelected = false;
							XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
							GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
							this.reload(tableView);
							//this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
						}
						else {
                            UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                            WindowAlert.RootViewController = new UIViewController();
                            UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                                WindowAlert.Dispose();
                            }));
                            WindowAlert.MakeKeyAndVisible();
                            WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                           
                        }
					}
				}


			}
			else {
				AdditionalColumns tappedColumn = GlobalSettings.AdditionalColumns[indexPath.Row];
				if (!tappedColumn.IsRequied)
				{
					if (GlobalSettings.WBidINIContent.DataColumns.Count < 18)
					{
						if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 0)
						{
							string cellText = GlobalSettings.AdditionalColumns[indexPath.Row].DisplayName;
							if (cellText == "Ratio")
							{
								NSNotificationCenter.DefaultCenter.PostNotificationName("ShowRatioScreen", null);
							}
							else
							{
								GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()
								{
									Id = tappedColumn.Id,
									Width = 50,
									Order = GlobalSettings.WBidINIContent.DataColumns.Count
								});
								GlobalSettings.AdditionalColumns[indexPath.Row].IsSelected = true;
								XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
								GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
								this.reload(tableView);
							}
							//this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
						}
						else {
							GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
							GlobalSettings.AdditionalColumns[indexPath.Row].IsSelected = false;
							XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
							GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
							this.reload(tableView);
							//this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
						}
					}
					else {
						if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 1)
						{
							GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
							GlobalSettings.AdditionalColumns[indexPath.Row].IsSelected = false;
							XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
							GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
							this.reload(tableView);
                          
							//this.PerformSelector (new MonoTouch.ObjCRuntime.Selector ("reloadTable"), tableView, 0.2);
						}
						else {
                            UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                            WindowAlert.RootViewController = new UIViewController();
                            UIAlertController okAlertController = UIAlertController.Create("Additional Columns", "Cannot add more than 5 columns", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                                WindowAlert.Dispose();
                            }));
                            WindowAlert.MakeKeyAndVisible();
                            WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                           
                        }
					}
				}
			}
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
            UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
            WindowAlert.RootViewController = new UIViewController();
            UIAlertController okAlertController;
            if (PopType == "addConst") {
			    okAlertController = UIAlertController.Create("Constraints", "You can add multiple constraints of this type", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    WindowAlert.Dispose();
                }));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                



            }
            else if (PopType == "addWeights") {
                okAlertController = UIAlertController.Create("Weights", "You can add multiple weights of this type", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    WindowAlert.Dispose();
                }));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
               
            }
		}
				//		[Export("reloadTable")]
		private void reload (UITableView tableView)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName ("DataCulumnsUpdated", null);
			tableView.ReloadData ();
		}

        [Export("reloadTable:")]
        private void reloadTable (UITableView tableView)
        {
            tableView.ReloadData();
        }



    }
}

