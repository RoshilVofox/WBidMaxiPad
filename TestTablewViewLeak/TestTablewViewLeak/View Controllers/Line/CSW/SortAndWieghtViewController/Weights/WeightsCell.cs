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
	public partial class WeightsCell : UITableViewCell
	{
		class MyPopDelegate : UIPopoverControllerDelegate
		{
			WeightsCell _parent;
			public MyPopDelegate (WeightsCell parent)
			{
				_parent = parent;
			}
			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.changeDOWWeightNotif);
				foreach (UIButton btn in _parent.btnDOWwt) {
					btn.Enabled = true;
				}
			}
		}

		public static readonly UINib Nib = UINib.FromName ("WeightsCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("WeightsCell");
		UIPopoverController popoverController;
		WeightCalculation weightCalc = new WeightCalculation();
		WBidState wBIdStateContent;// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		NSIndexPath path;
		NSObject changeFristCellNotif;
		NSObject changeSecondCellNotif;
		NSObject changeThirdCellNotif;
		NSObject changeFourthCellNotif;
		NSObject changeWeightNotif;
		NSObject changeDOWWeightNotif;
		NSObject changeWeightParamNotif;
		UIButton btnCommutabilityWeight;
		List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList();

		public WeightsCell (IntPtr handle) : base (handle)
		{
		}

		public static WeightsCell Create ()
		{
			return (WeightsCell)Nib.Instantiate (null, null) [0];
		}
		public void bindData (NSIndexPath indexpath) {
			btnCommuteTitleLabel.Hidden = true;
			btnHelpIcon.Hidden = false;
			btnCommuteWeight.Hidden = true;
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			path = indexpath;
			this.btnRemove.Tag = indexpath.Row;
			btnHelpIcon.Tag = indexpath.Section;
			this.Tag = indexpath.Row;
			this.btnFirstCell.Hidden = true;
			this.btnSecondCell.Hidden = true;
			this.lblWeightsTitle.Text = WeightsApplied.MainList [indexpath.Section];
			this.vwDOW.Hidden = true;
			this.vwWrkOff.Hidden = true;
			this.btnThirdCell.Hidden = false;
			this.btnWeight.Hidden = false;
			foreach (UIButton btn in btnDOWwt) {
				btn.Layer.BorderWidth = 1;
				btn.BackgroundColor = UIColor.Clear;
				btn.TitleLabel.TextColor = UIColor.Black;
				btn.SetTitle ("0", UIControlState.Normal);

			}
			this.btnWork.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
			this.btnWork.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
			this.btnOff.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
			this.btnOff.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);

			if (indexpath.Section == WeightsApplied.MainList.IndexOf("Aircraft Changes"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.AirCraftChanges.SecondlValue;
				int third = wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.AirCraftChanges.Weight;
				if (second == (int)WeightType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				else if (second == (int)WeightType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else if (second == (int)WeightType.Equal)
				{
					this.btnSecondCell.SetTitle("equal", UIControlState.Normal);
					updateColorForCompareTerm("equal", btnSecondCell);
				}
				else if (second == (int)WeightType.NotEqual)
				{
					this.btnSecondCell.SetTitle("not equal", UIControlState.Normal);
					updateColorForCompareTerm("not equal", btnSecondCell);
				}
				this.btnThirdCell.SetTitle(third.ToString() + " chg", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("AM/PM"))
			{
				int third = wBIdStateContent.Weights.AM_PM.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.AM_PM.lstParameters[indexpath.Row].Weight;
				if (third == (int)AMPMType.AM)
					this.btnThirdCell.SetTitle("am", UIControlState.Normal);
				else if (third == (int)AMPMType.PM)
					this.btnThirdCell.SetTitle("pm", UIControlState.Normal);
				else if (third == (int)AMPMType.NTE)
					this.btnThirdCell.SetTitle("nte", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Blocks of Days Off"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.BDO.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.BDO.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.BDO.lstParameters[indexpath.Row].Weight;
				if (second == (int)WeightType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				else if (second == (int)WeightType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else if (second == (int)WeightType.Equal)
				{
					this.btnSecondCell.SetTitle("equal", UIControlState.Normal);
					updateColorForCompareTerm("equal", btnSecondCell);
				}
				else if (second == (int)WeightType.NotEqual)
				{
					this.btnSecondCell.SetTitle("not equal", UIControlState.Normal);
					updateColorForCompareTerm("not equal", btnSecondCell);
				}
				this.btnThirdCell.SetTitle("blk " + third.ToString(), UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Cmut DHs"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.DHD.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.DHD.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.DHD.lstParameters[indexpath.Row].Weight;
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == second).Name;
				this.btnSecondCell.SetTitle(name, UIControlState.Normal);
				if (third == (int)DeadheadType.First)
					this.btnThirdCell.SetTitle("begin", UIControlState.Normal);
				else if (third == (int)DeadheadType.Last)
					this.btnThirdCell.SetTitle("end", UIControlState.Normal);
				else if (third == (int)DeadheadType.Both)
					this.btnThirdCell.SetTitle("both", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
				//			}  else if (indexpath.Section == WeightsApplied.MainList.IndexOf ("Commutable Lines")) {
				//
				//			}  else if (indexpath.Section == WeightsApplied.MainList.IndexOf ("Days of the Month")) {

			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Days of the Week"))
			{
				this.vwDOW.Hidden = false;
				this.vwWrkOff.Hidden = false;
				this.btnThirdCell.Hidden = true;
				this.btnWeight.Hidden = true;
				List<Wt> lstWeight = wBIdStateContent.Weights.DOW.lstWeight;
				foreach (Wt wt in lstWeight)
				{
					btnDOWwt.FirstOrDefault(x => x.Tag == wt.Key).SetTitle(wt.Value.ToString(), UIControlState.Normal);
					if (wt.Value < 0)
					{
						btnDOWwt.FirstOrDefault(x => x.Tag == wt.Key).BackgroundColor = ColorClass.OffDayColor;
						btnDOWwt.FirstOrDefault(x => x.Tag == wt.Key).TitleLabel.TextColor = UIColor.White;
					}
					else if (wt.Value > 0)
					{
						btnDOWwt.FirstOrDefault(x => x.Tag == wt.Key).BackgroundColor = ColorClass.WorkDayColor;
						btnDOWwt.FirstOrDefault(x => x.Tag == wt.Key).TitleLabel.TextColor = UIColor.Black;
					}
				}
				if (wBIdStateContent.Weights.DOW.IsOff)
				{
					this.btnWork.Selected = false;
					this.btnOff.Selected = true;
				}
				else
				{
					this.btnOff.Selected = false;
					this.btnWork.Selected = true;
				}
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("DH - first - last"))
			{
				int third = wBIdStateContent.Weights.DHDFoL.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.DHDFoL.lstParameters[indexpath.Row].Weight;
				if (third == (int)DeadheadType.First)
					this.btnThirdCell.SetTitle("first", UIControlState.Normal);
				else if (third == (int)DeadheadType.Last)
					this.btnThirdCell.SetTitle("last", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Duty period"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.DP.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.DP.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.DP.lstParameters[indexpath.Row].Weight;
				if (second == (int)DutyPeriodType.Relative)
					this.btnSecondCell.SetTitle("relative", UIControlState.Normal);
				else if (second == (int)DutyPeriodType.Longer)
					this.btnSecondCell.SetTitle("longer", UIControlState.Normal);
				else if (second == (int)DutyPeriodType.Shorter)
					this.btnSecondCell.SetTitle("shorter", UIControlState.Normal);
				this.btnThirdCell.SetTitle(Helper.ConvertMinuteToHHMM(third), UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Equipment Type"))
			{
				this.btnSecondCell.Hidden = false;
				if (wBIdStateContent.Weights.EQUIP.lstParameters[indexpath.Row].SecondlValue == 500)
					wBIdStateContent.Weights.EQUIP.lstParameters[indexpath.Row].SecondlValue = 300;
				string second = wBIdStateContent.Weights.EQUIP.lstParameters[indexpath.Row].SecondlValue.ToString();
				int third = wBIdStateContent.Weights.EQUIP.lstParameters[indexpath.Row].ThrirdCellValue;
				if (second == "600")
					second = "8Max";
				else if (second == "200")
					second = "7Max";

				decimal weight = wBIdStateContent.Weights.EQUIP.lstParameters[indexpath.Row].Weight;
				this.btnSecondCell.SetTitle(second.ToString(), UIControlState.Normal);
				this.btnThirdCell.SetTitle(third.ToString() + " legs", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("ETOPS"))
			{

				this.btnSecondCell.Hidden = true;
				this.btnThirdCell.Hidden = true;
				decimal weight = wBIdStateContent.Weights.ETOPS.lstParameters[indexpath.Row].Weight;
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("ETOPS-Res"))
			{
				this.btnSecondCell.Hidden = true;
				this.btnThirdCell.Hidden = true;
				decimal weight = wBIdStateContent.Weights.ETOPSRes.lstParameters[indexpath.Row].Weight;
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Flight Time"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.FLTMIN.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.FLTMIN.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.FLTMIN.lstParameters[indexpath.Row].Weight;
				if (second == (int)WeightType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				else if (second == (int)WeightType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else
				{
					this.btnSecondCell.SetTitle("-", UIControlState.Normal);
					updateColorForCompareTerm("-", btnSecondCell);
				}
				this.btnThirdCell.SetTitle(third.ToString(), UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Ground Time"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.GRD.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.GRD.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.GRD.lstParameters[indexpath.Row].Weight;
				this.btnSecondCell.SetTitle(Helper.ConvertMinuteToHHMM(second), UIControlState.Normal);
				this.btnThirdCell.SetTitle(third.ToString() + " occurs", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Intl – NonConus"))
			{
				int third = wBIdStateContent.Weights.InterConus.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.InterConus.lstParameters[indexpath.Row].Weight;
				if (third == -1)
					this.btnThirdCell.SetTitle("All Intl", UIControlState.Normal);
				else if (third == 0)
					this.btnThirdCell.SetTitle("All NonConus", UIControlState.Normal);
				else
				{
					string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
					this.btnThirdCell.SetTitle(name, UIControlState.Normal);
				}
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Largest Block of Days Off"))
			{
				this.btnThirdCell.Hidden = true;
				decimal weight = wBIdStateContent.Weights.LrgBlkDayOff.Weight;
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Legs Per Duty Period"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.LEGS.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.LEGS.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.LEGS.lstParameters[indexpath.Row].Weight;
				if (second == (int)WeightType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				else if (second == (int)WeightType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else if (second == (int)WeightType.Equal)
				{
					this.btnSecondCell.SetTitle("equal", UIControlState.Normal);
					updateColorForCompareTerm("equal", btnSecondCell);
				}
				this.btnThirdCell.SetTitle(third.ToString() + " legs", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Legs Per Pairing"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[indexpath.Row].Weight;
				if (second == (int)LegsPerPairingType.All)
				{
					this.btnSecondCell.SetTitle("all", UIControlState.Normal);
					updateColorForCompareTerm("all", btnSecondCell);
				}
				else if (second == (int)LegsPerPairingType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else if (second == (int)LegsPerPairingType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				this.btnThirdCell.SetTitle(third.ToString() + " legs", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Normalize Days Off"))
			{
				this.btnThirdCell.Hidden = true;
				decimal weight = wBIdStateContent.Weights.NormalizeDaysOff.Weight;
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Number of Days Off"))
			{
				int third = wBIdStateContent.Weights.NODO.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.NODO.lstParameters[indexpath.Row].Weight;
				this.btnThirdCell.SetTitle(third.ToString() + " off", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Overnight Cities"))
			{
				int third = wBIdStateContent.Weights.RON.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.RON.lstParameters[indexpath.Row].Weight;
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
				this.btnThirdCell.SetTitle(name, UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Cities-Legs"))
			{
				int third = wBIdStateContent.Weights.CitiesLegs.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.CitiesLegs.lstParameters[indexpath.Row].Weight;
				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
				this.btnThirdCell.SetTitle(name, UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("PDO-after"))
			{
				this.btnFirstCell.Hidden = false;
				this.btnSecondCell.Hidden = false;
				int first = wBIdStateContent.Weights.PDAfter.lstParameters[indexpath.Row].FirstValue;
				int second = wBIdStateContent.Weights.PDAfter.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.PDAfter.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.PDAfter.lstParameters[indexpath.Row].Weight;
				if (first == 0)
					first = 1;
				string firstTitle;
				if (first == 300)
				{
					firstTitle = "Any Date";
				}
				else
				{
					DateTime date = lstPDODays.FirstOrDefault(x => x.DateId == first).Date;
					//				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
					//				string strMonthName = mfi.GetMonthName(date.Month).ToString();
					//				string firstTitle = date.Day.ToString () + " - " + strMonthName;
					firstTitle = date.ToString("dd - MMM");
				}
				this.btnFirstCell.SetTitle(firstTitle, UIControlState.Normal);
				this.btnSecondCell.SetTitle(Helper.ConvertMinuteToHHMM(second), UIControlState.Normal);
				string name;
				if (third == 400)
					name = "Any City";
				else
					name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;

				this.btnThirdCell.SetTitle(name, UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("PDO-before"))
			{
				this.btnFirstCell.Hidden = false;
				this.btnSecondCell.Hidden = false;
				int first = wBIdStateContent.Weights.PDBefore.lstParameters[indexpath.Row].FirstValue;
				int second = wBIdStateContent.Weights.PDBefore.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.PDBefore.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.PDBefore.lstParameters[indexpath.Row].Weight;
				if (first == 0)
					first = 1;

				string firstTitle;
				if (first == 300)
				{
					firstTitle = "Any Date";
				}
				else
				{
					DateTime date = lstPDODays.FirstOrDefault(x => x.DateId == first).Date;
					//				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
					//				string strMonthName = mfi.GetMonthName(date.Month).ToString();
					//				string firstTitle = date.Day.ToString () + " - " + strMonthName;
					firstTitle = date.ToString("dd - MMM");
				}

				this.btnFirstCell.SetTitle(firstTitle, UIControlState.Normal);
				this.btnSecondCell.SetTitle(Helper.ConvertMinuteToHHMM(second), UIControlState.Normal);
				string name;
				if (third == 400)
					name = "Any City";
				else
					name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
				this.btnThirdCell.SetTitle(name, UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Position"))
			{
				int third = wBIdStateContent.Weights.POS.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.POS.lstParameters[indexpath.Row].Weight;
				if (third == (int)FAPositon.A)
					this.btnThirdCell.SetTitle("A", UIControlState.Normal);
				else if (third == (int)FAPositon.B)
					this.btnThirdCell.SetTitle("B", UIControlState.Normal);
				else if (third == (int)FAPositon.C)
					this.btnThirdCell.SetTitle("C", UIControlState.Normal);
				else if (third == (int)FAPositon.D)
					this.btnThirdCell.SetTitle("D", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Start Day of Week"))
			{

				//in wbidmax we are heeping the value 1,2,3,4,5,6 etc for MOn,Tue,Wed,Thu,Fri,Sat and Sun
				// Bud  wbid ipad having the enum value 0,1,2,3,...      MOn,Tue,Wed,Thu,Fri,Sat and Sun
				//That is why we are subtracting 1 . Since we need to implement Synch feature
				int third = wBIdStateContent.Weights.SDOW.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.SDOW.lstParameters[indexpath.Row].Weight;
				if (third == (int)Dow.Sun)
					this.btnThirdCell.SetTitle("sun", UIControlState.Normal);
				else if (third == (int)Dow.Mon)
					this.btnThirdCell.SetTitle("mon", UIControlState.Normal);
				else if (third == (int)Dow.Tue)
					this.btnThirdCell.SetTitle("tue", UIControlState.Normal);
				else if (third == (int)Dow.Wed)
					this.btnThirdCell.SetTitle("wed", UIControlState.Normal);
				else if (third == (int)Dow.Thu)
					this.btnThirdCell.SetTitle("thu", UIControlState.Normal);
				else if (third == (int)Dow.Fri)
					this.btnThirdCell.SetTitle("fri", UIControlState.Normal);
				else if (third == (int)Dow.Sat)
					this.btnThirdCell.SetTitle("sat", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Rest"))
			{
				this.btnFirstCell.Hidden = false;
				this.btnSecondCell.Hidden = false;
				int first = wBIdStateContent.Weights.WtRest.lstParameters[indexpath.Row].FirstValue;
				int second = wBIdStateContent.Weights.WtRest.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.WtRest.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.WtRest.lstParameters[indexpath.Row].Weight;
				if (first == 0)
					first = 1;
				if (first == (int)RestOptions.Shorter)
					this.btnFirstCell.SetTitle("shorter", UIControlState.Normal);
				else if (first == (int)RestOptions.Longer)
					this.btnFirstCell.SetTitle("longer", UIControlState.Normal);
				else if (first == (int)RestOptions.Both)
					this.btnFirstCell.SetTitle("+ & -", UIControlState.Normal);
				this.btnSecondCell.SetTitle(Helper.ConvertMinuteToHHMM(second), UIControlState.Normal);
				if (third == (int)RestType.All)
					this.btnThirdCell.SetTitle("all", UIControlState.Normal);
				else if (third == (int)RestType.InDomicile)
					this.btnThirdCell.SetTitle("inDom", UIControlState.Normal);
				else if (third == (int)RestType.AwayDomicile)
					this.btnThirdCell.SetTitle("away", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Time-Away-From-Base"))
			{
				int third = wBIdStateContent.Weights.PerDiem.Type;
				decimal weight = wBIdStateContent.Weights.PerDiem.Weight;
				this.btnThirdCell.SetTitle(third.ToString(), UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Trip Length"))
			{
				int third = wBIdStateContent.Weights.TL.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.TL.lstParameters[indexpath.Row].Weight;
				if (third == 1)
					this.btnThirdCell.SetTitle("Turn", UIControlState.Normal);
				else
					this.btnThirdCell.SetTitle(third.ToString() + " day", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Work Blk Length"))
			{
				int third = wBIdStateContent.Weights.WB.lstParameters[indexpath.Row].Type;
				decimal weight = wBIdStateContent.Weights.WB.lstParameters[indexpath.Row].Weight;
				if (third == 1)
					this.btnThirdCell.SetTitle("Turn", UIControlState.Normal);
				else
					this.btnThirdCell.SetTitle(third.ToString() + " day", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Work Days"))
			{
				this.btnSecondCell.Hidden = false;
				int second = wBIdStateContent.Weights.WorkDays.lstParameters[indexpath.Row].SecondlValue;
				int third = wBIdStateContent.Weights.WorkDays.lstParameters[indexpath.Row].ThrirdCellValue;
				decimal weight = wBIdStateContent.Weights.WorkDays.lstParameters[indexpath.Row].Weight;
				if (second == (int)WeightType.Less)
				{
					this.btnSecondCell.SetTitle("less", UIControlState.Normal);
					updateColorForCompareTerm("less", btnSecondCell);
				}
				else if (second == (int)WeightType.More)
				{
					this.btnSecondCell.SetTitle("more", UIControlState.Normal);
					updateColorForCompareTerm("more", btnSecondCell);
				}
				else if (second == (int)WeightType.Equal)
				{
					this.btnSecondCell.SetTitle("equal", UIControlState.Normal);
					updateColorForCompareTerm("equal", btnSecondCell);
				}
				this.btnThirdCell.SetTitle(third.ToString() + " wk days", UIControlState.Normal);
				this.btnWeight.SetTitle(weight.ToString(), UIControlState.Normal);
			}
			else if (indexpath.Section == WeightsApplied.MainList.IndexOf("Commutability"))
			{
				btnCommuteWeight.Hidden = false;
				this.btnSecondCell.Hidden = false;
				this.btnFirstCell.Hidden = false;
				int second = wBIdStateContent.Weights.Commute.SecondcellValue;
				int third = wBIdStateContent.Weights.Commute.ThirdcellValue;
				int type = wBIdStateContent.Weights.Commute.Type;
				int value = wBIdStateContent.Weights.Commute.Value;
				decimal weight = wBIdStateContent.Weights.Commute.Weight;


				//first button
				int secondValue = Convert.ToInt32(second);


				if (secondValue == (int)CommutabilitySecondCell.NoMiddle)
				{
					this.btnFirstCell.SetTitle("No Middle", UIControlState.Normal);
					this.UpdateCommutablityButtonBackground("No Middle");
				}
				else if (secondValue == (int)CommutabilitySecondCell.OKMiddle)
				{
					this.btnFirstCell.SetTitle("Ok Middle", UIControlState.Normal);
					this.UpdateCommutablityButtonBackground("Ok Middle");
				}

				//Second button

				int ThirdValue = Convert.ToInt32(third);

				if (ThirdValue == (int)CommutabilityThirdCell.Front)
				{
					this.btnSecondCell.SetTitle("Front", UIControlState.Normal);

				}
				else if (ThirdValue == (int)CommutabilityThirdCell.Back)
				{
					this.btnSecondCell.SetTitle("Back", UIControlState.Normal);

				}
				else if (ThirdValue == (int)CommutabilityThirdCell.Overall)
				{
					this.btnSecondCell.SetTitle("Overall", UIControlState.Normal);

				}

				// Type

				if (type == (int)ConstraintType.LessThan)
				{
					this.btnThirdCell.SetTitle("<=", UIControlState.Normal);
					this.updateColorForCompareTerm("less than");
				}
				else if (type == (int)ConstraintType.MoreThan)
				{
					this.btnThirdCell.SetTitle(">=", UIControlState.Normal);
					this.updateColorForCompareTerm("more than");
				}

				this.btnWeight.SetTitle(value.ToString() + "%", UIControlState.Normal);



				//constraints name
				this.lblWeightsTitle.Text = "comut %(" + wBIdStateContent.Constraints.Commute.City + ")";

				btnCommuteTitleLabel.Hidden = false;
				btnHelpIcon.Hidden = true;
				btnCommuteWeight.SetTitle(weight.ToString(), UIControlState.Normal);
				btnCommuteWeight.BackgroundColor = UIColor.Clear;
				btnCommuteWeight.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btnCommuteWeight.TitleLabel.Font = UIFont.BoldSystemFontOfSize((nfloat)12.0);



				btnCommuteWeight.TouchUpInside += delegate
				{


					changeWeightParamNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("changeWeightParamInWeightsCell"), handleParamTermChange);
					string supPopType = "CommutabilityPopUp";
					PopoverViewController popoverContent1 = new PopoverViewController();
					popoverContent1.PopType = "changeWeightParamInWeightsCell";
					popoverContent1.SubPopType = supPopType;
					popoverContent1.index = (int)this.Tag;
					popoverController = new UIPopoverController(popoverContent1);
					popoverController.PopoverContentSize = new CGSize(210, 300);
					popoverController.PresentFromRect(btnWeight.Frame, this, UIPopoverArrowDirection.Any, true);

				};



				//btnCommutabilityWeight.Frame = new CGRect (btnHelpIcon.Frame.X, btnWeight.Frame.Y, 40, btnWeight.Frame.Height);






				//
				//				string name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == second).Name;
				//				this.btnSecondCell.SetTitle (name, UIControlState.Normal);
				//				if (third == (int)DeadheadType.First)
				//					this.btnThirdCell.SetTitle ("begin", UIControlState.Normal);
				//				else if (third == (int)DeadheadType.Last)
				//					this.btnThirdCell.SetTitle ("end", UIControlState.Normal);
				//				else if (third == (int)DeadheadType.Both)
				//					this.btnThirdCell.SetTitle ("both", UIControlState.Normal);
				//				this.btnWeight.SetTitle (weight.ToString (), UIControlState.Normal);
				//			}  else if (indexpath.Section == WeightsApplied.MainList.IndexOf ("Commutable Lines")) {
				//
				//			}  else if (indexpath.Section == WeightsApplied.MainList.IndexOf ("Days of the Month")) {

			}
		}
		partial void funCommutabilityLineClicked (NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowCommutabilityWeightAuto", null);
		}
		private void updateColorForCompareTerm(string text)
		{
			if (text == "more than") {
				btnThirdCell.SetTitleColor (UIColor.Green, UIControlState.Normal);
			}  else if (text == "less than") {
				btnThirdCell.SetTitleColor (UIColor.Red, UIControlState.Normal);
			}  else if (text == "equal to") {
				btnThirdCell.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			}  else if (text == "not equal") {
				btnThirdCell.SetTitleColor (UIColor.Orange, UIControlState.Normal);
			}  else {
				btnThirdCell.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}
		public void handleParamTermChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeWeightParamNotif); 
			popoverController.Dismiss (true);
			//wBIdStateContent.Weights.Commute.Weight = decimal.Parse( n.Object.ToString());
			btnCommuteWeight.SetTitle(n.Object.ToString() , UIControlState.Normal);
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}
		private void UpdateCommutablityButtonBackground(string  text)
		{
			if (text == "No Middle") {
				btnFirstCell.BackgroundColor = UIColor.Orange;
				btnFirstCell.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}  else if (text == "Ok Middle") {
				btnFirstCell.BackgroundColor = UIColor.Green;
				btnFirstCell.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}  
		}
		partial void btnHelpIconTapped (UIKit.UIButton sender)
		{
			HelpViewController helpVC = new HelpViewController ();
			helpVC.pdfFileName = "Weights";
			helpVC.selectRow = 1;
			helpVC.pdfOffset = WeightsApplied.HelpPageOffset[WeightsApplied.MainList[(int)sender.Tag]];
			UINavigationController navCont = new UINavigationController (helpVC);
			navCont.NavigationBar.BarStyle = UIBarStyle.Black;
			navCont.NavigationBar.Hidden = true;
			navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
			CommonClass.cswVC.PresentViewController (navCont, true, null);
		}

		partial void btnWorkOffTapped (UIKit.UIButton sender)
		{
			if (sender.TitleLabel.Text == "Work") {
				btnWork.Selected = true;
				btnOff.Selected = false;
				wBIdStateContent.Weights.DOW.IsOff = false;
			}  else {
				btnOff.Selected = true;
				btnWork.Selected = false;
				wBIdStateContent.Weights.DOW.IsOff = true;
			}
			weightCalc.ApplyDaysOfWeekWeight (wBIdStateContent.Weights.DOW);
		}

		partial void btnDOWWeightsTapped (UIKit.UIButton sender)
		{

			sender.Enabled = false;
			changeDOWWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInDOWCell"), handleDOWWeightChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeWeightParamInDOWCell";
			popoverContent.SubPopType = supPopType;
			popoverContent.numValue = sender.TitleLabel.Text;
			popoverContent.index = (int)sender.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect(sender.Frame,this.vwDOW,UIPopoverArrowDirection.Any,true);

		}
		public void handleDOWWeightChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeDOWWeightNotif); 
			popoverController.Dismiss (true);
			foreach (UIButton btn in btnDOWwt) {
				if (!btn.Enabled)
					btn.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btn.Enabled = true;
			}
		}

		partial void btnWeightTapped (UIKit.UIButton sender)
		{
			if (path.Section == WeightsApplied.MainList.IndexOf ("Commutability")) 
			{
				changeFourthCellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changefourthcellparamWeightCell"), handleFourthCellChange);
				string supPopType = WeightsApplied.MainList [path.Section];
				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "changefourthcellparamWeightCell";
				popoverContent.SubPopType = supPopType;
				popoverContent.index = (int)this.Tag;
				popoverController = new UIPopoverController (popoverContent);
				popoverController.PopoverContentSize = new CGSize (135, 300);
				popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
			}
			else{
				changeWeightNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeWeightParamInWeightsCell"), handleWeightChange);
				string supPopType = WeightsApplied.MainList [path.Section];
				PopoverViewController popoverContent = new PopoverViewController ();
				popoverContent.PopType = "changeWeightParamInWeightsCell";
				popoverContent.SubPopType = supPopType;
				popoverContent.index = (int)this.Tag;
				popoverController = new UIPopoverController (popoverContent);
				popoverController.PopoverContentSize = new CGSize (210, 300);
				popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
			}
		}
		partial void btnThirdCellTapped (UIKit.UIButton sender)
		{
			changeThirdCellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeThirdCellParamInWeightCell"), handleThirdCellChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeThirdCellParamInWeightCell";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (135, 300);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
		}
		partial void btnSecondCellTapped (UIKit.UIButton sender)
		{
			changeSecondCellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeSecondCellParamInWeightCell"), handleSecondCellChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeSecondCellParamInWeightCell";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (125, 200);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
		}
		partial void btnFirstCellTapped (UIKit.UIButton sender)
		{
			changeFristCellNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("changeFirstCellParamInWeightCell"), handleFirstCellChange);
			string supPopType = WeightsApplied.MainList [path.Section];
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "changeFirstCellParamInWeightCell";
			popoverContent.SubPopType = supPopType;
			popoverContent.index = (int)this.Tag;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.PopoverContentSize = new CGSize (125, 300);
			popoverController.PresentFromRect(sender.Frame,this,UIPopoverArrowDirection.Any,true);
		}
		partial void btnRemoveTapped (UIKit.UIButton sender)
		{
			WBidHelper.PushToUndoStack ();
			if (path.Section == WeightsApplied.MainList.IndexOf ("Aircraft Changes")) {
				wBIdStateContent.CxWtState.ACChg.Wt = false;
				weightCalc.RemoveAirCraftChangesWeight();
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("AM/PM")) {
				if(WeightsApplied.AMPMWeights.Count>sender.Tag&&wBIdStateContent.Weights.AM_PM.lstParameters.Count>sender.Tag){
					WeightsApplied.AMPMWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.AM_PM.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyAMPMWeight (wBIdStateContent.Weights.AM_PM.lstParameters);
					if(wBIdStateContent.Weights.AM_PM.lstParameters.Count==0)
						wBIdStateContent.CxWtState.AMPM.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Blocks of Days Off")) {
				if(WeightsApplied.BlocksOfDaysOffWeights.Count>sender.Tag&&wBIdStateContent.Weights.BDO.lstParameters.Count>sender.Tag){
					WeightsApplied.BlocksOfDaysOffWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.BDO.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyBlockOFFDaysOfWeight (wBIdStateContent.Weights.BDO.lstParameters);
					if(wBIdStateContent.Weights.BDO.lstParameters.Count==0)
						wBIdStateContent.CxWtState.BDO.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Cmut DHs")) {
				if(WeightsApplied.CmutDHsWeights.Count>sender.Tag&&wBIdStateContent.Weights.DHD.lstParameters.Count>sender.Tag){
					WeightsApplied.CmutDHsWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.DHD.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyCommutableDeadhead (wBIdStateContent.Weights.DHD.lstParameters);
					if(wBIdStateContent.Weights.DHD.lstParameters.Count==0)
						wBIdStateContent.CxWtState.DHD.Wt = false;
				}
			}else if (path.Section == WeightsApplied.MainList.IndexOf ("Commutability")) {
				wBIdStateContent.CxWtState.Commute.Wt = false;
				weightCalc.RemoveCommutabilityWeight ();
			}

			else if (path.Section == WeightsApplied.MainList.IndexOf ("Days of the Week")) {
				wBIdStateContent.Weights.DOW.lstWeight.Clear();
				weightCalc.ApplyDaysOfWeekWeight (wBIdStateContent.Weights.DOW);
				wBIdStateContent.Weights.DOW.IsOff = true;
				wBIdStateContent.CxWtState.DOW.Wt = false;
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("DH - first - last")) {
				if(WeightsApplied.dhFirstLastWeights.Count>sender.Tag&&wBIdStateContent.Weights.DHDFoL.lstParameters.Count>sender.Tag){
					WeightsApplied.dhFirstLastWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.DHDFoL.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyDeadheadFisrtLastWeight (wBIdStateContent.Weights.DHDFoL.lstParameters);
					if(wBIdStateContent.Weights.DHDFoL.lstParameters.Count==0)
						wBIdStateContent.CxWtState.DHDFoL.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Duty period")) {
				if(WeightsApplied.DutyPeriodWeights.Count>sender.Tag&&wBIdStateContent.Weights.DP.lstParameters.Count>sender.Tag){
					WeightsApplied.DutyPeriodWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.DP.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyDutyPeriodWeight (wBIdStateContent.Weights.DP.lstParameters);
					if(wBIdStateContent.Weights.DP.lstParameters.Count==0)
						wBIdStateContent.CxWtState.DP.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Equipment Type")) {
				if(WeightsApplied.EQTypeWeights.Count>sender.Tag&&wBIdStateContent.Weights.EQUIP.lstParameters.Count>sender.Tag){
					WeightsApplied.EQTypeWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.EQUIP.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyEquipmentTypeWeights (wBIdStateContent.Weights.EQUIP.lstParameters);
					if(wBIdStateContent.Weights.EQUIP.lstParameters.Count==0)
						wBIdStateContent.CxWtState.EQUIP.Wt = false;
				}
			}
			else if (path.Section == WeightsApplied.MainList.IndexOf("ETOPS"))
			{
				if (WeightsApplied.ETOPSWeights.Count > sender.Tag && wBIdStateContent.Weights.ETOPS.lstParameters.Count > sender.Tag)
				{
					WeightsApplied.ETOPSWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.ETOPS.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
					if (wBIdStateContent.Weights.ETOPS.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.ETOPS.Wt = false;
				}
			}
			else if (path.Section == WeightsApplied.MainList.IndexOf("ETOPS-Res"))
			{
				if (WeightsApplied.ETOPSResWeights.Count > sender.Tag && wBIdStateContent.Weights.ETOPSRes.lstParameters.Count > sender.Tag)
				{
					WeightsApplied.ETOPSResWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.ETOPSRes.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
					if (wBIdStateContent.Weights.ETOPSRes.lstParameters.Count == 0)
						wBIdStateContent.CxWtState.ETOPSRes.Wt = false;
				}
			}
			else if (path.Section == WeightsApplied.MainList.IndexOf ("Flight Time")) {
				if(WeightsApplied.FlightTimeWeights.Count>sender.Tag&&wBIdStateContent.Weights.FLTMIN.lstParameters.Count>sender.Tag){
					WeightsApplied.FlightTimeWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.FLTMIN.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyFlightTimeWeights (wBIdStateContent.Weights.FLTMIN.lstParameters);
					if(wBIdStateContent.Weights.FLTMIN.lstParameters.Count==0)
						wBIdStateContent.CxWtState.FLTMIN.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Ground Time")) {
				if(WeightsApplied.GroundTimeWeights.Count>sender.Tag&&wBIdStateContent.Weights.GRD.lstParameters.Count>sender.Tag){
					WeightsApplied.GroundTimeWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.GRD.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyGroundTimeWeight (wBIdStateContent.Weights.GRD.lstParameters);
					if(wBIdStateContent.Weights.GRD.lstParameters.Count==0)
						wBIdStateContent.CxWtState.GRD.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Intl – NonConus")) {
				if(WeightsApplied.IntlNonConusWeights.Count>sender.Tag&&wBIdStateContent.Weights.InterConus.lstParameters.Count>sender.Tag){
					WeightsApplied.IntlNonConusWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.InterConus.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyInternationalNonConusWeight (wBIdStateContent.Weights.InterConus.lstParameters);
					if(wBIdStateContent.Weights.InterConus.lstParameters.Count==0)
						wBIdStateContent.CxWtState.InterConus.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Largest Block of Days Off")) {
				weightCalc.RemoveLargestBlockDaysWeight();
				wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt = false;
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Legs Per Duty Period")) {
				if(WeightsApplied.LegsPerDutyPeriodWeights.Count>sender.Tag&&wBIdStateContent.Weights.LEGS.lstParameters.Count>sender.Tag){
					WeightsApplied.LegsPerDutyPeriodWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.LEGS.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyLegsPerDutyPeriodWeight (wBIdStateContent.Weights.LEGS.lstParameters);
					if(wBIdStateContent.Weights.LEGS.lstParameters.Count==0)
						wBIdStateContent.CxWtState.LEGS.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Legs Per Pairing")) {
				if(WeightsApplied.LegsPerPairingWeights.Count>sender.Tag&&wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.Count>sender.Tag){
					WeightsApplied.LegsPerPairingWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyLegsPerPairingWeight (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
					if(wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.Count==0)
						wBIdStateContent.CxWtState.LegsPerPairing.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Normalize Days Off")) {
				weightCalc.RemoveNormalizeDaysOffWeight();
				wBIdStateContent.CxWtState.NormalizeDays.Wt = false;
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Number of Days Off")) {
				if(WeightsApplied.NumOfDaysOffWeights.Count>sender.Tag&&wBIdStateContent.Weights.NODO.lstParameters.Count>sender.Tag){
					WeightsApplied.NumOfDaysOffWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.NODO.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyNumberOfDaysOfWeight (wBIdStateContent.Weights.NODO.lstParameters);
					if(wBIdStateContent.Weights.NODO.lstParameters.Count==0)
						wBIdStateContent.CxWtState.NODO.Wt = false;
				}
			}  
			else if (path.Section == WeightsApplied.MainList.IndexOf ("Overnight Cities")) 
			{
				if(WeightsApplied.OvernightCitiesWeights.Count>sender.Tag&&wBIdStateContent.Weights.RON.lstParameters.Count>sender.Tag){
					WeightsApplied.OvernightCitiesWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.RON.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyOverNightCitiesWeight (wBIdStateContent.Weights.RON.lstParameters);
					if(wBIdStateContent.Weights.RON.lstParameters.Count==0)
						wBIdStateContent.CxWtState.RON.Wt = false;
				}
			}  
			else if (path.Section == WeightsApplied.MainList.IndexOf ("Cities-Legs")) 
			{
				if(WeightsApplied.CitiesLegsWeights.Count>sender.Tag&&wBIdStateContent.Weights.CitiesLegs.lstParameters.Count>sender.Tag){
					WeightsApplied.CitiesLegsWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.CitiesLegs.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyCitiesLegsWeight (wBIdStateContent.Weights.CitiesLegs.lstParameters);
					if(wBIdStateContent.Weights.CitiesLegs.lstParameters.Count==0)
						wBIdStateContent.CxWtState.CitiesLegs.Wt = false;
				}
			}  
			else if (path.Section == WeightsApplied.MainList.IndexOf ("PDO-after")) {
				if(WeightsApplied.PDOAfterWeights.Count>sender.Tag&&wBIdStateContent.Weights.PDAfter.lstParameters.Count>sender.Tag){
					WeightsApplied.PDOAfterWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.PDAfter.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyPartialDaysAfterWeight (wBIdStateContent.Weights.PDAfter.lstParameters);
					if(wBIdStateContent.Weights.PDAfter.lstParameters.Count==0)
						wBIdStateContent.CxWtState.PDAfter.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("PDO-before")) {
				if(WeightsApplied.PDOBeforeWeights.Count>sender.Tag&&wBIdStateContent.Weights.PDBefore.lstParameters.Count>sender.Tag){
					WeightsApplied.PDOBeforeWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.PDBefore.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyPartialDaysBeforeWeight (wBIdStateContent.Weights.PDBefore.lstParameters);
					if(wBIdStateContent.Weights.PDBefore.lstParameters.Count==0)
						wBIdStateContent.CxWtState.PDBefore.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Position")) {
				if(WeightsApplied.PositionWeights.Count>sender.Tag&&wBIdStateContent.Weights.POS.lstParameters.Count>sender.Tag){
					WeightsApplied.PositionWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.POS.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyPositionWeight (wBIdStateContent.Weights.POS.lstParameters);
					if(wBIdStateContent.Weights.POS.lstParameters.Count==0)
						wBIdStateContent.CxWtState.Position.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Start Day of Week")) {
				if(WeightsApplied.StartDOWWeights.Count>sender.Tag&&wBIdStateContent.Weights.SDOW.lstParameters.Count>sender.Tag){
					WeightsApplied.StartDOWWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.SDOW.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyStartDayOfWeekWeight (wBIdStateContent.Weights.SDOW.lstParameters);
					if(wBIdStateContent.Weights.SDOW.lstParameters.Count==0)
						wBIdStateContent.CxWtState.SDOW.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Rest")) {
				if(WeightsApplied.RestWeights.Count>sender.Tag&&wBIdStateContent.Weights.WtRest.lstParameters.Count>sender.Tag){
					WeightsApplied.RestWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.WtRest.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyRestWeight (wBIdStateContent.Weights.WtRest.lstParameters);
					if(wBIdStateContent.Weights.WtRest.lstParameters.Count==0)
						wBIdStateContent.CxWtState.Rest.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Time-Away-From-Base")) {
				wBIdStateContent.CxWtState.PerDiem.Wt = false;
				weightCalc.RemoveTimeAwayFromBaseWeight();
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Trip Length")) {
				if(WeightsApplied.TripLengthWeights.Count>sender.Tag&&wBIdStateContent.Weights.TL.lstParameters.Count>sender.Tag){
					WeightsApplied.TripLengthWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.TL.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyTripLengthWeight (wBIdStateContent.Weights.TL.lstParameters);
					if(wBIdStateContent.Weights.TL.lstParameters.Count==0)
						wBIdStateContent.CxWtState.TL.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Work Blk Length")) {
				if(WeightsApplied.WorkBlockLengthWeights.Count>sender.Tag&&wBIdStateContent.Weights.WB.lstParameters.Count>sender.Tag){
					WeightsApplied.WorkBlockLengthWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.WB.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyWorkBlockLengthWeight (wBIdStateContent.Weights.WB.lstParameters);
					if(wBIdStateContent.Weights.WB.lstParameters.Count==0)
						wBIdStateContent.CxWtState.WB.Wt = false;
				}
			}  else if (path.Section == WeightsApplied.MainList.IndexOf ("Work Days")) {
				if(WeightsApplied.WorkDaysWeights.Count>sender.Tag&&wBIdStateContent.Weights.WorkDays.lstParameters.Count>sender.Tag){
					WeightsApplied.WorkDaysWeights.RemoveAt((int)sender.Tag);
					wBIdStateContent.Weights.WorkDays.lstParameters.RemoveAt((int)sender.Tag);
					weightCalc.ApplyWorkDaysWeight (wBIdStateContent.Weights.WorkDays.lstParameters);
					if(wBIdStateContent.Weights.WorkDays.lstParameters.Count==0)
						wBIdStateContent.CxWtState.WorkDay.Wt = false;
				}
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);

		}
		public void handleFirstCellChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeFristCellNotif); 
			popoverController.Dismiss (true);
			btnFirstCell.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}
		public void handleSecondCellChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeSecondCellNotif); 
			popoverController.Dismiss (true);
			btnSecondCell.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}
		public void handleThirdCellChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeThirdCellNotif); 
			popoverController.Dismiss (true);
			btnThirdCell.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}
		public void handleFourthCellChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeFourthCellNotif); 
			popoverController.Dismiss (true);
			btnWeight.SetTitle (n.Object.ToString()+"%", UIControlState.Normal);
			GlobalSettings.isModified = true;
			CommonClass.cswVC.UpdateSaveButton ();
		}
		public void handleWeightChange (NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (changeWeightNotif); 
			popoverController.Dismiss (true);
			btnWeight.SetTitle (n.Object.ToString(), UIControlState.Normal);
		}

		private void updateColorForCompareTerm(string text, UIButton btn)
		{
			if (text == "more") {
				btn.SetTitleColor (UIColor.Green, UIControlState.Normal);
			}  else if (text == "less") {
				btn.SetTitleColor (UIColor.Red, UIControlState.Normal);
			}  else if (text == "equal") {
				btn.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			}  else if (text == "not equal") {
				btn.SetTitleColor (UIColor.Orange, UIControlState.Normal);
			}  else {
				btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
			}
		}


	}
}


