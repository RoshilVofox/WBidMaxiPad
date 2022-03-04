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
using WBid.WBidiPad.Core.Enum;
using CoreText;


namespace WBid.WBidiPad.iOS
{
	public partial class NumberPadView : UIViewController
	{
		public string PopType;
		public string SubPopType;
		public int index;
		public string numValue;

		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		WeightCalculation weightCalc = new WeightCalculation();

		public NumberPadView () : base ("NumberPadView", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnAdd.Hidden = true;
			btnInsert.Hidden = true;
			setInitialValues ();

			// Perform any additional setup after loading the view, typically from a nib.
		}
		private void setInitialValues () {
			if (PopType == "changeWeightParamInWeightsCell") {
				if (SubPopType == "Aircraft Changes") {
					numValue = wBIdStateContent.Weights.AirCraftChanges.Weight.ToString ();
				} else if (SubPopType == "AM/PM") {
					numValue = wBIdStateContent.Weights.AM_PM.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Blocks of Days Off") {
					numValue = wBIdStateContent.Weights.BDO.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Cmut DHs") {
					numValue = wBIdStateContent.Weights.DHD.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "DH - first - last") {
					numValue = wBIdStateContent.Weights.DHDFoL.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Duty period") {
					numValue = wBIdStateContent.Weights.DP.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Equipment Type") {
					numValue = wBIdStateContent.Weights.EQUIP.lstParameters [index].Weight.ToString ();
				}
				else if (SubPopType == "ETOPS")
				{
					numValue = wBIdStateContent.Weights.ETOPS.lstParameters[index].Weight.ToString();
				}

				else if (SubPopType == "ETOPS-Res")
				{
					numValue = wBIdStateContent.Weights.ETOPSRes.lstParameters[index].Weight.ToString();
				}
				else if (SubPopType == "Flight Time") {
					numValue = wBIdStateContent.Weights.FLTMIN.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Ground Time") {
					numValue = wBIdStateContent.Weights.GRD.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Intl – NonConus") {
					numValue = wBIdStateContent.Weights.InterConus.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Legs Per Duty Period") {
					numValue = wBIdStateContent.Weights.LEGS.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Legs Per Pairing") {
					numValue = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Number of Days Off") {
					numValue = wBIdStateContent.Weights.NODO.lstParameters [index].Weight.ToString ();
				} 
				else if (SubPopType == "Overnight Cities") {
					numValue = wBIdStateContent.Weights.RON.lstParameters [index].Weight.ToString ();
				}
				else if (SubPopType == "Cities-Legs") {
					numValue = wBIdStateContent.Weights.CitiesLegs.lstParameters [index].Weight.ToString ();
				}
				else if (SubPopType == "PDO-after") {
					numValue = wBIdStateContent.Weights.PDAfter.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "PDO-before") {
					numValue = wBIdStateContent.Weights.PDBefore.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Position") {
					numValue = wBIdStateContent.Weights.POS.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Start Day of Week") {
					numValue = wBIdStateContent.Weights.SDOW.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Time-Away-From-Base") {
					numValue = wBIdStateContent.Weights.PerDiem.Weight.ToString ();
				} else if (SubPopType == "Trip Length") {
					numValue = wBIdStateContent.Weights.TL.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Work Blk Length") {
					numValue = wBIdStateContent.Weights.WB.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Work Days") {
					numValue = wBIdStateContent.Weights.WorkDays.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Rest") {
					numValue = wBIdStateContent.Weights.WtRest.lstParameters [index].Weight.ToString ();
				} else if (SubPopType == "Largest Block of Days Off") {
					numValue = wBIdStateContent.Weights.LrgBlkDayOff.Weight.ToString ();
				} else if (SubPopType == "Normalize Days Off") {
					numValue = wBIdStateContent.Weights.NormalizeDaysOff.Weight.ToString ();

				}
				else if (SubPopType == "Commutable Line - Auto") {
					numValue = wBIdStateContent.Weights.CLAuto.Weight.ToString ();

				}
				else if (SubPopType == "CommutabilityPopUp") {
					numValue = wBIdStateContent.Weights.Commute.Weight.ToString ();

				}

				this.lblNumDisplay.Text = numValue;
			} else if (PopType == "changeWeightParamInDOWCell") {
				if (SubPopType == "Days of the Week") {
					this.lblNumDisplay.Text = numValue;
				}
			} else if (PopType == "changeWeightParamInDOMCell") {
				if (SubPopType == "Days of the Month") {
					this.lblNumDisplay.Text = numValue;
				}
			} else if (PopType == "changeWeightParamInCommutableLine") {
				if (SubPopType == "Commutable Lines - Manual") {
//					if (index == 0)
//						this.lblNumDisplay.Text = wBIdStateContent.Weights.CL.BothEnds.ToString ();
//					else
//						this.lblNumDisplay.Text = wBIdStateContent.Weights.CL.InDomicile.ToString ();
					this.lblNumDisplay.Text = numValue;

				}
			} else if (PopType == "changeOvernightBulkWeight") {
				if (SubPopType == "Overnight Cities - Bulk") {
					this.lblNumDisplay.Text = numValue;
				}
			} else if (PopType == "numberPad") {
				if (SubPopType == "Define") {
					this.lblNumDisplay.Text = numValue;
					this.btnMinus.Enabled = false;
					this.btnDot.Enabled = false;
				} else if (SubPopType == "ManualBidEntry") {
					this.lblNumDisplay.Text = numValue;
					this.btnMinus.Hidden = true;
					this.btnDot.Hidden = true;
					btnAdd.Hidden = false;
					btnInsert.Hidden = false;
					btnAdd.Selected = true;
				}
			}
		}
		partial void btnAddInsertTapped (UIKit.UIButton sender)
		{
			if(sender==btnAdd){
				btnAdd.Selected=true;
				btnInsert.Selected = false;
			} else if (sender==btnInsert){
				btnAdd.Selected=false;
				btnInsert.Selected = true;
			}
		}
		partial void btnClearTapped (UIKit.UIButton sender)
		{
			if(numValue.Length>=1)
				numValue = numValue.Substring(0,numValue.Length-1);
			numValue = (numValue == string.Empty)?"0":numValue;
			this.lblNumDisplay.Text = numValue ;

		}
		partial void btnNumPadTapped (UIKit.UIButton sender)
		{
			numberCalculation (sender);
		}
		partial void btnCLRTapped (UIKit.UIButton sender)
		{
			numValue = "0";
			this.lblNumDisplay.Text = numValue;
		}
		partial void btnOKTapped (UIKit.UIButton sender)
		{
			var str = numValue.Replace("-","");
			str = str.Replace(".","");
			if(str.Length>5)
				return;

			if(PopType=="changeWeightParamInWeightsCell"){
				if(numValue!="-"){
					applyWeights ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeWeightParamInWeightsCell", new NSString (numValue));
				}
			} else if (PopType == "changeWeightParamInDOWCell") {
				if(numValue!="-"){
					applyWeights ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeWeightParamInDOWCell", new NSString (numValue));
				}
			} else if (PopType == "changeWeightParamInDOMCell") {
				if(numValue!="-"){
					applyWeights ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeWeightParamInDOMCell", new NSString (numValue));
				}
			} else if (PopType == "changeWeightParamInCommutableLine") {
				if(numValue!="-"){
					applyWeights ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeWeightParamInCommutableLine", new NSString (numValue));
				}
			} else if (PopType == "changeOvernightBulkWeight") {
				if(numValue!="-"){
					applyWeights ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeOvernightBulkWeight", new NSString (numValue));
				}
			} else if (PopType == "numberPad"){
				if (SubPopType == "Define"){
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ChangeMaxInputText", new NSString (numValue));
				} else if(SubPopType=="ManualBidEntry"&&numValue!="0"){
					if(btnAdd.Selected)
						NSNotificationCenter.DefaultCenter.PostNotificationName ("ManualBidEntry", new NSString (numValue+"+add"));
					else if (btnInsert.Selected)
						NSNotificationCenter.DefaultCenter.PostNotificationName ("ManualBidEntry", new NSString (numValue+"+insert"));
				}
			}
		}
		private void numberCalculation (UIButton button) 
		{
			string num = button.TitleLabel.Text;
			if (numValue == "0")
				numValue = "";
			if(numValue==""&&num=="-")
				numValue = "-";
//			if (numValue == null)
//				numValue = "";
			else
				numValue = WeightBL.ValidateDecimal (numValue, num);
			this.lblNumDisplay.Text = numValue;
		}
		private void applyWeights ()
		{
			WBidHelper.PushToUndoStack ();
			if (SubPopType == "Aircraft Changes") {
				wBIdStateContent.Weights.AirCraftChanges.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyAirCraftChangeWeight (wBIdStateContent.Weights.AirCraftChanges);
			} else if (SubPopType == "AM/PM") {
				wBIdStateContent.Weights.AM_PM.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyAMPMWeight (wBIdStateContent.Weights.AM_PM.lstParameters);
			} else if (SubPopType == "Blocks of Days Off") {
				wBIdStateContent.Weights.BDO.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyBlockOFFDaysOfWeight (wBIdStateContent.Weights.BDO.lstParameters);
			} else if (SubPopType == "Cmut DHs") {
				wBIdStateContent.Weights.DHD.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyCommutableDeadhead (wBIdStateContent.Weights.DHD.lstParameters);
			} else if (SubPopType == "DH - first - last") {
				wBIdStateContent.Weights.DHDFoL.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyDeadheadFisrtLastWeight (wBIdStateContent.Weights.DHDFoL.lstParameters);
			} else if (SubPopType == "Duty period") {
				wBIdStateContent.Weights.DP.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyDutyPeriodWeight (wBIdStateContent.Weights.DP.lstParameters);
			} else if (SubPopType == "Equipment Type") {
				wBIdStateContent.Weights.EQUIP.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyEquipmentTypeWeights (wBIdStateContent.Weights.EQUIP.lstParameters);
			}
			else if (SubPopType == "ETOPS")
			{
				wBIdStateContent.Weights.ETOPS.lstParameters[index].Weight = Decimal.Parse(numValue);
				weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
			}

			else if (SubPopType == "ETOPS-Res")
			{
				wBIdStateContent.Weights.ETOPSRes.lstParameters[index].Weight = Decimal.Parse(numValue);
				weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
			}
			else if (SubPopType == "Flight Time") {
				wBIdStateContent.Weights.FLTMIN.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyFlightTimeWeights (wBIdStateContent.Weights.FLTMIN.lstParameters);
			} else if (SubPopType == "Ground Time") {
				wBIdStateContent.Weights.GRD.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyGroundTimeWeight (wBIdStateContent.Weights.GRD.lstParameters);
			} else if (SubPopType == "Intl – NonConus") {
				wBIdStateContent.Weights.InterConus.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyInternationalNonConusWeight (wBIdStateContent.Weights.InterConus.lstParameters);
			} else if (SubPopType == "Legs Per Duty Period") {
				wBIdStateContent.Weights.LEGS.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyLegsPerDutyPeriodWeight (wBIdStateContent.Weights.LEGS.lstParameters);
			} else if (SubPopType == "Legs Per Pairing") {
				wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyLegsPerPairingWeight (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
			} else if (SubPopType == "Number of Days Off") {
				wBIdStateContent.Weights.NODO.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyNumberOfDaysOfWeight (wBIdStateContent.Weights.NODO.lstParameters);
			} 
			else if (SubPopType == "Overnight Cities") {
				wBIdStateContent.Weights.RON.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyOverNightCitiesWeight (wBIdStateContent.Weights.RON.lstParameters);
			} 
			else if (SubPopType == "Cities-Legs") {
				wBIdStateContent.Weights.CitiesLegs.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyCitiesLegsWeight (wBIdStateContent.Weights.CitiesLegs.lstParameters);
			} 
			else if (SubPopType == "PDO-after") {
				wBIdStateContent.Weights.PDAfter.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyPartialDaysAfterWeight (wBIdStateContent.Weights.PDAfter.lstParameters);
			} else if (SubPopType == "PDO-before") {
				wBIdStateContent.Weights.PDBefore.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyPartialDaysBeforeWeight (wBIdStateContent.Weights.PDBefore.lstParameters);
			} else if (SubPopType == "Position") {
				wBIdStateContent.Weights.POS.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyPositionWeight (wBIdStateContent.Weights.POS.lstParameters);
			} else if (SubPopType == "Start Day of Week") {
				wBIdStateContent.Weights.SDOW.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyStartDayOfWeekWeight (wBIdStateContent.Weights.SDOW.lstParameters);
			} else if (SubPopType == "Time-Away-From-Base") {
				wBIdStateContent.Weights.PerDiem.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyTimeAwayFromBaseWeight (wBIdStateContent.Weights.PerDiem);
			} else if (SubPopType == "Trip Length") {
				wBIdStateContent.Weights.TL.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyTripLengthWeight (wBIdStateContent.Weights.TL.lstParameters);
			} else if (SubPopType == "Work Blk Length") {
				wBIdStateContent.Weights.WB.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyWorkBlockLengthWeight (wBIdStateContent.Weights.WB.lstParameters);
			} else if (SubPopType == "Work Days") {
				wBIdStateContent.Weights.WorkDays.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyWorkDaysWeight (wBIdStateContent.Weights.WorkDays.lstParameters);
			} else if (SubPopType == "Days of the Week") {
				List<Wt> lstWeight = wBIdStateContent.Weights.DOW.lstWeight;
				if (lstWeight.Any (x => x.Key == index)) {
					if (numValue != "0")
						lstWeight.FirstOrDefault (x => x.Key == index).Value = Decimal.Parse (numValue);
					else
						lstWeight.RemoveAll (x => x.Key == index);
				} else {
					if (numValue != "0")
						lstWeight.Add (new Wt{ Key = index, Value = Decimal.Parse (numValue) });
				}
				weightCalc.ApplyDaysOfWeekWeight (wBIdStateContent.Weights.DOW);
			} else if (SubPopType == "Rest") {
				wBIdStateContent.Weights.WtRest.lstParameters [index].Weight = Decimal.Parse (numValue);
				weightCalc.ApplyRestWeight (wBIdStateContent.Weights.WtRest.lstParameters);
			} else if (SubPopType == "Days of the Month") {
				List<Wt> lstWeight = wBIdStateContent.Weights.SDO.Weights;
				if (lstWeight.Any (x => x.Key == index)) {
					if (numValue != "0")
						lstWeight.FirstOrDefault (x => x.Key == index).Value = Decimal.Parse (numValue);
					else
						lstWeight.RemoveAll (x => x.Key == index);
				} else {
					if (numValue != "0")
						lstWeight.Add (new Wt{ Key = index, Value = Decimal.Parse (numValue) });
				}
				weightCalc.ApplyDaysOfMonthWeight (wBIdStateContent.Weights.SDO);
			} else if (SubPopType == "Largest Block of Days Off") {
				wBIdStateContent.Weights.LrgBlkDayOff.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyLargestBlockOfDaysoffWeight (wBIdStateContent.Weights.LrgBlkDayOff);
			} 

			else if (SubPopType == "Commutable Line - Auto") {
				wBIdStateContent.Weights.CLAuto.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyCommutableLineAuto (wBIdStateContent.Weights.CLAuto);
			} 
			else if (SubPopType == "CommutabilityPopUp") {
				wBIdStateContent.Weights.Commute.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyCommuttabilityWeight (wBIdStateContent.Weights.Commute);
				return;
			} 
			else if (SubPopType == "Normalize Days Off") {
				wBIdStateContent.Weights.NormalizeDaysOff.Weight = Decimal.Parse (numValue);
				weightCalc.ApplyNormalizeDaysOffWeight (wBIdStateContent.Weights.NormalizeDaysOff);
			} else if (PopType == "changeWeightParamInCommutableLine") {
				if (SubPopType == "Commutable Lines - Manual") {
					if (index == 0)
						wBIdStateContent.Weights.CL.BothEnds = Decimal.Parse (numValue);
					else
						wBIdStateContent.Weights.CL.InDomicile = Decimal.Parse (numValue);
					weightCalc.ApplyCommutableLine (wBIdStateContent.Weights.CL);
				}
			} else if (SubPopType == "Overnight Cities - Bulk") {
				List<Wt2Parameter> lstWeight = wBIdStateContent.Weights.OvernightCitybulk;
				if (lstWeight.Any (x => x.Type == index)) {
					if (numValue != "0")
						lstWeight.FirstOrDefault (x => x.Type == index).Weight = Decimal.Parse (numValue);
					else
						lstWeight.RemoveAll (x => x.Type == index);
				} else {
					if (numValue != "0")
						lstWeight.Add (new Wt2Parameter { Type = index, Weight = Decimal.Parse (numValue) });
				}
				weightCalc.ApplyOvernightCityBulkWeight (wBIdStateContent.Weights.OvernightCitybulk);

			}
			NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
		}
	}
}

