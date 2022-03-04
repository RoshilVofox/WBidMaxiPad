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
	public partial class ConstraintsCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("ConstraintsCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ConstraintsCell");
		UIPopoverController popoverController;
		NSObject changeCompareTermInConstraintsCellNotif;
		NSObject changeContraintParamNotif;
		NSObject changeContraintThirdcellNotif;
		NSObject changeContraintSecondcellNotif;
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		WBidState wBIdStateContent;// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		NSIndexPath path;
		string[] arrDays = {"mon","tue","wed","thu","fri","sat","sun"};
		string[] arrTripLenth = {"1Day","2Day","3Day","4Day"};
		string[] arrSdow = {"Block","Trip" };
        string[] startdayparam = { "Start On", "Does Not Start" };
		List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList();

		public ConstraintsCell (IntPtr handle) : base (handle)
		{

		}
//		protected override void Dispose (bool disposing)
//		{
//			base.Dispose (disposing);
//		
//				foreach (UIView view in this.Subviews) {
//
//					DisposeClass.DisposeEx(view);
//				}
//				this.Dispose ();
//
//
//
//		}
		public static ConstraintsCell Create ()
		{
			return (ConstraintsCell)Nib.Instantiate (null, null) [0];
		}
		public void bindData (NSIndexPath indexpath) {
			btnCommutabltytextlbl.Hidden = true;
			path = indexpath;
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			this.btnRemove.Tag = indexpath.Row;
			this.Tag = indexpath.Row;
			btnSecondCell.Hidden = true;
			btnThirdCell.Hidden = true;
			btnCompareTerm.Hidden = false;
			btnParam.Hidden = false;
			btnHelpIcon.Tag = indexpath.Section;

			this.lblConstraintsName.Text = ConstraintsApplied.MainList [indexpath.Section];

			if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Aircraft Changes")) {
				int type = wBIdStateContent.Constraints.AircraftChanges.Type;
				int value = wBIdStateContent.Constraints.AircraftChanges.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Blocks of Days Off")) {
				int type = wBIdStateContent.Constraints.BlockOfDaysOff.Type;
				int value = wBIdStateContent.Constraints.BlockOfDaysOff.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				} else if (type == (int)ConstraintType.EqualTo) {
					this.btnCompareTerm.SetTitle ("equal to", UIControlState.Normal);
					this.updateColorForCompareTerm ("equal to");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Cmut DHs")) {
				btnSecondCell.Hidden = false;
				btnThirdCell.Hidden = false;
				string second = wBIdStateContent.Constraints.DeadHeads.LstParameter [path.Row].SecondcellValue;
				string third = wBIdStateContent.Constraints.DeadHeads.LstParameter [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.DeadHeads.LstParameter [path.Row].Type;
				int value = wBIdStateContent.Constraints.DeadHeads.LstParameter [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				if (thirdValue == (int)DeadheadType.First) {
					this.btnThirdCell.SetTitle ("begin", UIControlState.Normal);
				} else if (thirdValue == (int)DeadheadType.Last) {
					this.btnThirdCell.SetTitle ("end", UIControlState.Normal);
				} else if (thirdValue == (int)DeadheadType.Either) {
					this.btnThirdCell.SetTitle ("either", UIControlState.Normal);
				} else if (thirdValue == (int)DeadheadType.Both) {
					this.btnThirdCell.SetTitle ("both", UIControlState.Normal);
				}
				int secondValue = Convert.ToInt32 (second);
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == secondValue).Name;
				this.btnSecondCell.SetTitle (name, UIControlState.Normal);
//			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Lines")) {

			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Week")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.DaysOfWeek.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.DaysOfWeek.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.DaysOfWeek.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				this.btnThirdCell.SetTitle (arrDays[thirdValue], UIControlState.Normal);
//			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Days of the Month")) {

			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("DH - first - last")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
                }
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				if (thirdValue == (int)DeadheadType.First) {
					this.btnThirdCell.SetTitle ("first", UIControlState.Normal);
				} else if (thirdValue == (int)DeadheadType.Last) {
					this.btnThirdCell.SetTitle ("last", UIControlState.Normal);
				} else if (thirdValue == (int)DeadheadType.Both) {
					this.btnThirdCell.SetTitle ("both", UIControlState.Normal);
				}
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Duty period")) {
				int type = wBIdStateContent.Constraints.DutyPeriod.Type;
				int value = wBIdStateContent.Constraints.DutyPeriod.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (Helper.ConvertMinuteToHHMM(value), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Equipment Type")) {
				btnThirdCell.Hidden = false;
				if (wBIdStateContent.Constraints.EQUIP.lstParameters[path.Row].ThirdcellValue == "500")
					wBIdStateContent.Constraints.EQUIP.lstParameters[path.Row].ThirdcellValue = "300";
				string third = wBIdStateContent.Constraints.EQUIP.lstParameters [path.Row].ThirdcellValue;
				if (third == "600")
					third = "8Max";
				else if (third == "200")
					third = "7Max";
				int type = wBIdStateContent.Constraints.EQUIP.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.EQUIP.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				this.btnThirdCell.SetTitle (third, UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Flight Time")) {
				int type = wBIdStateContent.Constraints.FlightMin.Type;
				int value = wBIdStateContent.Constraints.FlightMin.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (Helper.ConvertMinuteToHHMM(value), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Ground Time")) {
				this.btnThirdCell.Hidden = false;
				int type = wBIdStateContent.Constraints.GroundTime.Type;
				int value = wBIdStateContent.Constraints.GroundTime.Value;
				string third = wBIdStateContent.Constraints.GroundTime.ThirdcellValue;
				int thirdValue = Convert.ToInt32 (third);
				this.btnThirdCell.SetTitle (Helper.ConvertMinuteToHHMM(thirdValue), UIControlState.Normal);
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString(), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Intl – NonConus")) {
				int type = wBIdStateContent.Constraints.InterConus.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.InterConus.lstParameters [path.Row].Value;
				if (type == (int)CityType.International) {
					this.btnCompareTerm.SetTitle ("Intl", UIControlState.Normal);
					this.updateColorForCompareTerm ("Intl");
				} else if (type == (int)CityType.NonConus) {
					this.btnCompareTerm.SetTitle ("NonConus", UIControlState.Normal);
					this.updateColorForCompareTerm ("NonConus");
				}
				if (value == 0)
					this.btnParam.SetTitle ("All", UIControlState.Normal);
				else {
					string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == value).Name;
					this.btnParam.SetTitle (name, UIControlState.Normal);
				}
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Legs Per Duty Period")) {
				int type = wBIdStateContent.Constraints.LegsPerDutyPeriod.Type;
				int value = wBIdStateContent.Constraints.LegsPerDutyPeriod.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Legs Per Pairing")) {
				int type = wBIdStateContent.Constraints.LegsPerPairing.Type;
				int value = wBIdStateContent.Constraints.LegsPerPairing.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Number of Days Off")) {
				int type = wBIdStateContent.Constraints.NumberOfDaysOff.Type;
				int value = wBIdStateContent.Constraints.NumberOfDaysOff.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			}
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Overnight Cities")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.OverNightCities.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.OverNightCities.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.OverNightCities.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == thirdValue).Name;
				this.btnThirdCell.SetTitle (name, UIControlState.Normal);
			} 
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Cities-Legs")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.CitiesLegs.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.CitiesLegs.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.CitiesLegs.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == thirdValue).Name;
				this.btnThirdCell.SetTitle (name, UIControlState.Normal);
			} 
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("PDO")) {
				btnSecondCell.Hidden = false;
				btnThirdCell.Hidden = false;
				string second = wBIdStateContent.Constraints.PDOFS.LstParameter [path.Row].SecondcellValue;
				string third = wBIdStateContent.Constraints.PDOFS.LstParameter [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.PDOFS.LstParameter [path.Row].Type;
				int value = wBIdStateContent.Constraints.PDOFS.LstParameter [path.Row].Value;
				if (type == (int)ConstraintType.atafter) {
					this.btnCompareTerm.SetTitle ("at+after", UIControlState.Normal);
					this.updateColorForCompareTerm ("at+after");
				} else if (type == (int)ConstraintType.atbefore) {
					this.btnCompareTerm.SetTitle ("at+before", UIControlState.Normal);
					this.updateColorForCompareTerm ("at+before");
				}
				this.btnParam.SetTitle (Helper.ConvertMinuteToHHMM(value), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				string name;
				if (thirdValue == 400) {
					name = "Any City";
					this.btnSecondCell.SetTitle (name, UIControlState.Normal);
				} else {
					name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == thirdValue).Name;
				}
				this.btnThirdCell.SetTitle (name, UIControlState.Normal);
				int secondValue = Convert.ToInt32 (second);
				string secondTitle;
				if (secondValue == 300) {
					secondTitle = "Any Date";
				} else {
					DateTime date = lstPDODays.FirstOrDefault (x => x.DateId == secondValue).Date;
					secondTitle = date.ToString ("dd - MMM");

				}
				this.btnSecondCell.SetTitle (secondTitle, UIControlState.Normal);
//				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
//				string strMonthName = mfi.GetMonthName(date.Month).ToString();
//				string secondTitle = date.Day.ToString () + " - " + strMonthName;


			} 
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Start Day of Week")) 
			{
				btnSecondCell.Hidden = false;
				btnThirdCell.Hidden = false;
				btnSecondCell.Frame = new CGRect(180, 20, 40, 10);
				string secondcell = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[path.Row].SecondcellValue;
				string third = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters [path.Row].Value;

				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				this.btnThirdCell.SetTitle (arrDays[thirdValue], UIControlState.Normal);
				string secondTitle;
				if (secondcell == "2")
				{
					secondTitle = arrSdow[1];
				}
				else {
					secondTitle = arrSdow[0];

				}
				this.btnSecondCell.SetTitle(secondTitle, UIControlState.Normal);

			} 
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Rest")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.Rest.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.Rest.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.Rest.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				if(third=="")
					third = "1";
				int thirdValue = Convert.ToInt32 (third);
				if (thirdValue == (int)RestType.All) {
					this.btnThirdCell.SetTitle ("All", UIControlState.Normal);
				} else if (thirdValue == (int)RestType.InDomicile) {
					this.btnThirdCell.SetTitle ("InDom", UIControlState.Normal);
				} else if (thirdValue == (int)RestType.AwayDomicile) {
                					this.btnThirdCell.SetTitle ("AwayDom", UIControlState.Normal);
				}
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Time-Away-From-Base")) {
				int type = wBIdStateContent.Constraints.PerDiem.Type;
				int value = wBIdStateContent.Constraints.PerDiem.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (Helper.ConvertMinuteToHHMM(value), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Trip Length")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.TripLength.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.TripLength.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.TripLength.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				this.btnThirdCell.SetTitle (arrTripLenth[thirdValue-1], UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Work Blk Length")) {
				btnThirdCell.Hidden = false;
				string third = wBIdStateContent.Constraints.WorkBlockLength.lstParameters [path.Row].ThirdcellValue;
				int type = wBIdStateContent.Constraints.WorkBlockLength.lstParameters [path.Row].Type;
				int value = wBIdStateContent.Constraints.WorkBlockLength.lstParameters [path.Row].Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
				int thirdValue = Convert.ToInt32 (third);
				this.btnThirdCell.SetTitle (arrTripLenth[thirdValue-1], UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Work Days")) {
				int type = wBIdStateContent.Constraints.WorkDay.Type;
				int value = wBIdStateContent.Constraints.WorkDay.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				} else if (type == (int)ConstraintType.EqualTo) {
					this.btnCompareTerm.SetTitle ("equal to", UIControlState.Normal);
					this.updateColorForCompareTerm ("equal to");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Min Pay")) {
				int type = wBIdStateContent.Constraints.MinimumPay.Type;
				int value = wBIdStateContent.Constraints.MinimumPay.Value;
				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("3-on-3-off")) {
				this.btnParam.Hidden = true;
				int type = wBIdStateContent.Constraints.No3On3Off.Type;
				//int value = wBIdStateContent.Constraints.No3On3Off.Value;
				if (type == (int)ThreeOnThreeOff.ThreeOnThreeOff) {
					this.btnCompareTerm.SetTitle ("Only 3-on-3-off", UIControlState.Normal);
					//this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ThreeOnThreeOff.NoThreeOnThreeOff) {
					this.btnCompareTerm.SetTitle ("NO 3-on-30ff", UIControlState.Normal);
					//this.updateColorForCompareTerm ("more than");
				}
				//this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Overlap Days")) {
//				int type = wBIdStateContent.Constraints.NoOverLap.Type;
//				int value = wBIdStateContent.Constraints.NoOverLap.Value;
//				if (type == (int)ConstraintType.LessThan) {
//					this.btnCompareTerm.SetTitle ("less than", UIControlState.Normal);
//					this.updateColorForCompareTerm ("less than");
//				} else if (type == (int)ConstraintType.MoreThan) {
//					this.btnCompareTerm.SetTitle ("more than", UIControlState.Normal);
//					this.updateColorForCompareTerm ("more than");
//				}
//				this.btnParam.SetTitle (value.ToString (), UIControlState.Normal);
			}
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Commutability")) {
				btnSecondCell.Hidden = false;
				btnThirdCell.Hidden = false;

				string second = wBIdStateContent.Constraints.Commute.SecondcellValue.ToString();
				string third = wBIdStateContent.Constraints.Commute.ThirdcellValue.ToString();

				int type = wBIdStateContent.Constraints.Commute.Type;
				int value = wBIdStateContent.Constraints.Commute.Value;

				//Second button
				int secondValue = Convert.ToInt32 (second);


				if (secondValue == (int)CommutabilitySecondCell.NoMiddle) {
					this.btnSecondCell.SetTitle ("No Middle", UIControlState.Normal);
					this.UpdateCommutablityButtonBackground ("No Middle");
				} else if (secondValue == (int)CommutabilitySecondCell.OKMiddle) {
					this.btnSecondCell.SetTitle ("Ok Middle", UIControlState.Normal);
					this.UpdateCommutablityButtonBackground ("Ok Middle");
				}

				//Third button

				int ThirdValue = Convert.ToInt32 (third);

				if (ThirdValue == (int)CommutabilityThirdCell.Front) {
					this.btnThirdCell.SetTitle ("Front", UIControlState.Normal);

				} else if (ThirdValue == (int)CommutabilityThirdCell.Back) {
					this.btnThirdCell.SetTitle ("Back", UIControlState.Normal);

				}
				else if (ThirdValue == (int)CommutabilityThirdCell.Overall) {
					this.btnThirdCell.SetTitle ("Overall", UIControlState.Normal);

				}

				// Type

				if (type == (int)ConstraintType.LessThan) {
					this.btnCompareTerm.SetTitle ("<=", UIControlState.Normal);
					this.updateColorForCompareTerm ("less than");
				} else if (type == (int)ConstraintType.MoreThan) {
					this.btnCompareTerm.SetTitle (">=", UIControlState.Normal);
					this.updateColorForCompareTerm ("more than");
				}
				this.btnParam.SetTitle (value.ToString () +"%", UIControlState.Normal);


		  //constraints name
				this.lblConstraintsName.Text ="comut %(" + wBIdStateContent.Constraints.Commute.City+")";
				btnCommutabltytextlbl.Hidden = false;

				//			} else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Commutable Lines")) {

			}
            else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf ("Start Day"))
            {
                btnThirdCell.Hidden = false;
                string third = wBIdStateContent.Constraints.StartDay.lstParameters [path.Row].ThirdcellValue;
                int type = wBIdStateContent.Constraints.StartDay.lstParameters [path.Row].Type;
                int value = wBIdStateContent.Constraints.StartDay.lstParameters [path.Row].Value;

                //bind button values
                string thirdTitle = (third == "2") ? arrSdow[1] : arrSdow[0];
                this.btnThirdCell.SetTitle(thirdTitle, UIControlState.Normal);
                this.btnCompareTerm.SetTitle(startdayparam[type-1], UIControlState.Normal);
                this.btnParam.SetTitle(value.ToString(), UIControlState.Normal);

            }
			else if (indexpath.Section == ConstraintsApplied.MainList.IndexOf("Mixed Hard/Reserve"))
			{
				btnCompareTerm.Hidden = true;
				btnParam.Hidden = true;
				//int type = wBIdStateContent.Constraints.AircraftChanges.Type;
				//int value = wBIdStateContent.Constraints.AircraftChanges.Value;
				//if (type == (int)ConstraintType.LessThan)
				//{
				//	this.btnCompareTerm.SetTitle("less than", UIControlState.Normal);
				//	this.updateColorForCompareTerm("less than");
				//}
				//else if (type == (int)ConstraintType.MoreThan)
				//{
				//	this.btnCompareTerm.SetTitle("more than", UIControlState.Normal);
				//	this.updateColorForCompareTerm("more than");
				//}
				//this.btnParam.SetTitle(value.ToString(), UIControlState.Normal);
			}

		}

		partial void funCcommutblytextlbl (NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowCommutabilityAuto", null);
		}
		partial void btnRemoveTap (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if (path.Section == ConstraintsApplied.MainList.IndexOf("Aircraft Changes"))
			{
				wBIdStateContent.CxWtState.ACChg.Cx = false;
				constCalc.RemoveAirCraftChangesConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Blocks of Days Off"))
			{
				wBIdStateContent.CxWtState.BDO.Cx = false;
				constCalc.RemoveBlockOfDaysOffConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Cmut DHs"))
			{
				if (ConstraintsApplied.CmutDHsConstraints.Count > sender.Tag && wBIdStateContent.Constraints.DeadHeads.LstParameter.Count > sender.Tag)
				{
					ConstraintsApplied.CmutDHsConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.DeadHeads.LstParameter.RemoveAt((int)sender.Tag);
					constCalc.ApplyCommutableDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeads.LstParameter);
					if (wBIdStateContent.Constraints.DeadHeads.LstParameter.Count == 0)
						wBIdStateContent.CxWtState.DHD.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Commutable Lines - Manual"))
			{
				wBIdStateContent.CxWtState.CL.Cx = false;
				//constCalc.remove ();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Commutable Lines"))
			{
				wBIdStateContent.CxWtState.CL.Cx = false;
				//constCalc.remove ();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Days of the Week"))
			{
				if (ConstraintsApplied.daysOfWeekConstraints.Count > sender.Tag && wBIdStateContent.Constraints.DaysOfWeek.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.daysOfWeekConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.DaysOfWeek.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyDaysofWeekConstraint(wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
					if (wBIdStateContent.Constraints.DaysOfWeek.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.DOW.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Days of the Month"))
			{
				wBIdStateContent.CxWtState.SDO.Cx = false;
				wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Clear();
				wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Clear();
				constCalc.ApplyDaysOfMonthConstraint(wBIdStateContent.Constraints.DaysOfMonth);
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("DH - first - last"))
			{
				if (ConstraintsApplied.DhFirstLastConstraints.Count > sender.Tag && wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.DhFirstLastConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
					if (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.DHDFoL.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Duty period"))
			{
				wBIdStateContent.CxWtState.DP.Cx = false;
				constCalc.RemoveDutyPeriodConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Equipment Type"))
			{
				if (ConstraintsApplied.EQTypeConstraints.Count > sender.Tag && ConstraintsApplied.EQTypeConstraints.Count > sender.Tag)
				{
					ConstraintsApplied.EQTypeConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.EQUIP.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyEquipmentTypeConstraint(wBIdStateContent.Constraints.EQUIP.lstParameters);
					if (wBIdStateContent.Constraints.EQUIP.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.EQUIP.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Flight Time"))
			{
				wBIdStateContent.CxWtState.FLTMIN.Cx = false;
				constCalc.RemoveFlightTimeConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Ground Time"))
			{
				wBIdStateContent.CxWtState.GRD.Cx = false;
				constCalc.RemoveGroundTimeConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Intl – NonConus"))
			{
				if (ConstraintsApplied.IntlNonConusConstraints.Count > sender.Tag && wBIdStateContent.Constraints.InterConus.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.IntlNonConusConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.InterConus.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyInternationalonConusConstraint(wBIdStateContent.Constraints.InterConus.lstParameters);
					if (wBIdStateContent.Constraints.InterConus.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.InterConus.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Legs Per Duty Period"))
			{
				wBIdStateContent.CxWtState.LEGS.Cx = false;
				constCalc.RemoveLegsPerDutyPeriodConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Legs Per Pairing"))
			{
				wBIdStateContent.CxWtState.LegsPerPairing.Cx = false;
				constCalc.RemoveLegsPerPairingConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Number of Days Off"))
			{
				wBIdStateContent.CxWtState.NODO.Cx = false;
				constCalc.RemoveNumberofDaysOffConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Overnight Cities"))
			{
				if (ConstraintsApplied.OvernightCitiesConstraints.Count > sender.Tag && wBIdStateContent.Constraints.OverNightCities.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.OvernightCitiesConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.OverNightCities.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyOvernightCitiesConstraint(wBIdStateContent.Constraints.OverNightCities.lstParameters);
					if (wBIdStateContent.Constraints.OverNightCities.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.RON.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Cities-Legs"))
			{
				if (ConstraintsApplied.CitiesLegsConstraints.Count > sender.Tag && wBIdStateContent.Constraints.CitiesLegs.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.CitiesLegsConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.CitiesLegs.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyCitiesLegsConstraint(wBIdStateContent.Constraints.CitiesLegs.lstParameters);
					if (wBIdStateContent.Constraints.CitiesLegs.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.CitiesLegs.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("PDO"))
			{
				if (ConstraintsApplied.PDOConstraints.Count > sender.Tag && wBIdStateContent.Constraints.PDOFS.LstParameter.Count > sender.Tag)
				{
					ConstraintsApplied.PDOConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.PDOFS.LstParameter.RemoveAt((int)sender.Tag);
					constCalc.ApplyPartialdaysOffConstraint(wBIdStateContent.Constraints.PDOFS.LstParameter);
					if (wBIdStateContent.Constraints.PDOFS.LstParameter.Count == 0)
						wBIdStateContent.CxWtState.WtPDOFS.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Start Day of Week"))
			{
				if (ConstraintsApplied.StartDayofWeekConstraints.Count > sender.Tag && wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.StartDayofWeekConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyStartDayOfWeekConstraint(wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
					if (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.SDOW.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Rest"))
			{
				if (ConstraintsApplied.RestConstraints.Count > sender.Tag && wBIdStateContent.Constraints.Rest.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.RestConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.Rest.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyRestConstraint(wBIdStateContent.Constraints.Rest.lstParameters);
					if (wBIdStateContent.Constraints.Rest.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.Rest.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Time-Away-From-Base"))
			{
				wBIdStateContent.CxWtState.PerDiem.Cx = false;
				constCalc.RemoveTimeAwayFromBaseConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Trip Length"))
			{
				if (ConstraintsApplied.TripLengthConstraints.Count > sender.Tag && wBIdStateContent.Constraints.TripLength.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.TripLengthConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.TripLength.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyTripLengthConstraint(wBIdStateContent.Constraints.TripLength.lstParameters);
					if (wBIdStateContent.Constraints.TripLength.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.TL.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Work Blk Length"))
			{
				if (ConstraintsApplied.WorkBlockLengthConstraints.Count > sender.Tag && wBIdStateContent.Constraints.WorkBlockLength.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.WorkBlockLengthConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.WorkBlockLength.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyWorkBlockLengthConstraint(wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
					if (wBIdStateContent.Constraints.WorkBlockLength.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.WB.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Work Days"))
			{
				wBIdStateContent.CxWtState.WorkDay.Cx = false;
				constCalc.RemoveWorkDayConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Min Pay"))
			{
				wBIdStateContent.CxWtState.MP.Cx = false;
				constCalc.RemoveMinimumPayConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("3-on-3-off"))
			{
				wBIdStateContent.CxWtState.No3on3off.Cx = false;
				constCalc.RemoveThreeOn3offConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Overlap Days"))
			{
				wBIdStateContent.CxWtState.NOL.Cx = false;
				//constCalc.remove ();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Commutability"))
			{
				wBIdStateContent.CxWtState.Commute.Cx = false;
				constCalc.RemoveCommutabilityConstraint();
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Start Day"))
			{
				if (ConstraintsApplied.StartDayConstraints.Count > sender.Tag && wBIdStateContent.Constraints.StartDay.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.StartDayConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.StartDay.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
					if (wBIdStateContent.Constraints.StartDay.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.StartDay.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Report-Release"))
			{
				if (ConstraintsApplied.ReportReleaseConstraints.Count > sender.Tag && wBIdStateContent.Constraints.ReportRelease.lstParameters.Count > sender.Tag)
				{
					ConstraintsApplied.ReportReleaseConstraints.RemoveAt((int)sender.Tag);
					wBIdStateContent.Constraints.ReportRelease.lstParameters.RemoveAt((int)sender.Tag);
					constCalc.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
					if (wBIdStateContent.Constraints.ReportRelease.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.ReportRelease.Cx = false;
				}
			}
			else if (path.Section == ConstraintsApplied.MainList.IndexOf("Mixed Hard/Reserve"))
			{
				wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx = false;
				constCalc.RemoveMixedHardandReserveConstraint();
			}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);


		}
		partial void btnHelpIconTapped (UIKit.UIButton sender)
		{
			HelpViewController helpVC = new HelpViewController ();
			helpVC.pdfFileName = "Constraints";
			helpVC.pdfOffset = ConstraintsApplied.HelpPageOffset[ConstraintsApplied.MainList[(int)sender.Tag]];
			UINavigationController navCont = new UINavigationController (helpVC);
			navCont.NavigationBar.BarStyle = UIBarStyle.Black;
			navCont.NavigationBar.Hidden = true;
			navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
			CommonClass.cswVC.PresentViewController (navCont, true, null);
		}
		partial void btnCompareTaped (UIKit.UIButton sender)
		{
			changeCompareTermInConstraintsCellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeCompareTermInConstraintsCell"), handleCompareTermChange);
			string supPopType = ConstraintsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeCompareTermInConstraintsCell";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			if(supPopType=="Blocks of Days Off"||supPopType=="Number of Days Off"||supPopType=="Work Days")
				popoverController.PopoverContentSize = new CGSize (125, 200);
            else if (supPopType=="3-on-3-off" || supPopType == "Start Day")
				popoverController.PopoverContentSize = new CGSize (150, 160);
			else
				popoverController.PopoverContentSize = new CGSize (125, 160);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
		}
		partial void btnConstraintParamTapped (UIKit.UIButton sender)
		{
			changeContraintParamNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeParamTermInConstraintsCell"), handleParamTermChange);
			string supPopType = ConstraintsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeContraintParam";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (125, 500);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
		}
		partial void btnThirdCellTap (UIKit.UIButton sender)
		{
			changeContraintThirdcellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeThirdCellParamInConstraintsCell"), handleThirdCellParamChange);

			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeThirdCellParam";
			popoverContent.SubPopType = ConstraintsApplied.MainList [path.Section];
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (125, 250);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);

		}
		partial void btnSecondCellTapped (UIKit.UIButton sender)
		{
			changeContraintSecondcellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeSecondCellParamInConstraintsCell"), handleSecondCellParamChange);

			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeSecondCellParam";
			popoverContent.SubPopType = ConstraintsApplied.MainList [path.Section];
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (125, 250);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);

		}

		public void handleCompareTermChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeCompareTermInConstraintsCellNotif); 
			popoverController.Dismiss (true);
			btnCompareTerm.SetTitle (n.Object.ToString(), UIControlState.Normal);
			updateColorForCompareTerm (n.Object.ToString ());
		}
		public void handleParamTermChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeContraintParamNotif); 
			popoverController.Dismiss (true);
			btnParam.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}
		public void handleThirdCellParamChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeContraintThirdcellNotif); 
			popoverController.Dismiss (true);
			btnThirdCell.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}
		public void handleSecondCellParamChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeContraintSecondcellNotif); 
			popoverController.Dismiss (true);
			btnSecondCell.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}

		private void updateColorForCompareTerm(string text)
		{
			if (text == "more than") {
				btnCompareTerm.SetTitleColor (UIColor.Green, UIControlState.Normal);
			} else if (text == "less than") {
				btnCompareTerm.SetTitleColor (UIColor.Red, UIControlState.Normal);
			} else if (text == "equal to") {
				btnCompareTerm.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			} else if (text == "not equal") {
				btnCompareTerm.SetTitleColor (UIColor.Orange, UIControlState.Normal);
			} else {
				btnCompareTerm.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}

		private void UpdateCommutablityButtonBackground(string text)
		{
			if (text == "No Middle") {
				btnSecondCell.BackgroundColor = UIColor.Orange;
				btnCompareTerm.SetTitleColor (UIColor.Black, UIControlState.Normal);
			} else if (text == "Ok Middle") {
				btnSecondCell.BackgroundColor = UIColor.Green;
				btnCompareTerm.SetTitleColor (UIColor.Black, UIControlState.Normal);
			} 
		}
	}
}

