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
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidiPad.iOS
{
	public class SmallPopoverTableSource : UITableViewSource
	{
		public string PopType;
		public string SubPopType;
		public int index;
		List<ColumnDefinition> Numerator;
		// Constraints Param Popover
		string[] arrCompString1 = {"less than","more than"};
		string[] arrCompString6 = {"<=",">="};
		string[] arrCompString2 = {"less than","more than","equal to"};
		string[] arrCompString3 = {"at+after","at+before"};
		string[] arrCompString4 = {"Intl","NonConus"};
		string[] arrDays = {"mon","tue","wed","thu","fri","sat","sun"};
		string[] arrFirstLast = {"first","last","both"};
		string[] arrEQType = {"700","800","7Max","8Max"};
		string[] arrRest = {"All","InDom","AwayDom"};
		string[] arrTripLenth = {"1Day","2Day","3Day","4Day"};
		string[] arrCmut = {"begin","end","both"};
		string[] arrCmut2 = {"begin","end","either","both"};
		string[] arrCompString5 = {"NO 3-on-30ff","Only 3-on-3-off"};
		string[] arrCommute = {"less than","more than"};
		string[] arrCommutabilty = {"Front","Back","Overall"};
		string[] arrCommtblty2 = {"NoMiddle", "OKMiddle"};
		string[] arrSdow = { "Block", "Trip" };
        string[] startdayParam = { "Start On", "Does Not Start" };


		int[] arrAirCraft = Enumerable.Range (0, 20-0+1).ToArray ();
		int[] arrBDO = Enumerable.Range (3, 31-3+1).ToArray ();
		string[] arrDutyPeriodValue = {"08:00","08:15","08:30","08:45","09:00","09:15","09:30","09:45","10:00","10:15","10:30","10:45","11:00","11:15","11:30","11:45","12:00","12:15","12:30","12:45","13:00","13:15","13:30","13:45","14:00","14:15","14:30","14:45","15:00","15:15","15:30","15:45","16:00"};
		int[] arrFlightTimeValue = Enumerable.Range (50, 120-50+1).ToArray ();
		string[] arrGrndTimeValue = {"0:30","0:45","1:00","1:15","1:30","1:45","2:00","2:15","2:30","2:45","3:00","3:15","3:30","3:45","4:00"};
		int[] arrlegsPerPeriodValue = Enumerable.Range (1, 8-1+1).ToArray ();
		int[] arrlegsPerPairingValue = Enumerable.Range (1, 32-1+1).ToArray ();
		int[] arrNumOfDaysOffValue = Enumerable.Range (10, 31-10+1).ToArray ();
		int[] arrTimeAwayFrmBaseValue = Enumerable.Range (200, 400-200+1).ToArray ();
		int[] arrMinPayValue = Enumerable.Range (60, 130-60+1).ToArray ();
		int[] arrNo3on3offValue = Enumerable.Range (1, 7-1+1).ToArray ();
		int[] arrNoOverlapValue = Enumerable.Range (0, 4-0+1).ToArray ();
		int[] arrDOWValue = Enumerable.Range (1, 6-1+1).ToArray ();
		int[] arrDHFOLValue = Enumerable.Range (0, 5-0+1).ToArray ();
		int[] arrEQTypeValue = Enumerable.Range (0, 20-0+1).ToArray ();
		int[] arrStartDayValue = Enumerable.Range (0, 6-0+1).ToArray ();
		int[] arrRestValue = Enumerable.Range (8, 48-8+1).ToArray ();
		List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList();
		List<string> lstCommutability=ConstraintBL.GetCommutabilityValue();
		List<string> lstPDOTimes = ConstraintBL.GetPartialTimeList();
		int[] arrGrndTimeValue2 = Enumerable.Range (1, 25-1+1).ToArray ();
        int[] startdayValueParam = Enumerable.Range(1, 31).ToArray();


		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ConstraintCalculations constCalc = new ConstraintCalculations ();

		// Weights Param Popover
		WeightCalculation weightCalc = new WeightCalculation();
		string[] arrSecondParam1 = {"less","more","equal","not equal"};
		string[] arrSecondParam2 = {"relative","longer","shorter"};
		string[] arrSecondParam3 = {"less","more"};
		string[] arrSecondParam4 = {"all","more","less"};
		string[] arrSecondParam5 = {"shorter","+ & -","longer"};
		string[] arrSecondParam6 = {"all","away","inDom"};
		string[] arrSecondParam7 = {"less","more","equal"};


		string[] arrThirdParam1 = {"am","pm","nte"};
		string[] arrThirdParam2 = {"first","last"};
		string[] arrThirdParam3 = {"A","B","C","D"};
		string[] arrThirdParam4 = {"Turn","2 day","3 day","4 day"};


		int[] arrAirCraftThirdValue = Enumerable.Range (1, 30-1+1).ToArray ();
		int[] arrBDOThirdValue = Enumerable.Range (1, 31-1+1).ToArray ();
		List<string> lstDPTimes = WeightBL.GetTimeList(5,16,5);
		int[] arrFltTimeValue = Enumerable.Range (20, 140-20+1).ToArray ();
		List<string> lstGrdTimes = WeightBL.GetTimeList(0,6,5);
		int[] arrGrdValue = Enumerable.Range (1, 25-1+1).ToArray ();
		int[] arrLegsDPValue = Enumerable.Range (0, 9-0+1).ToArray ();
		int[] arrNODOThirdValue = Enumerable.Range (9, 31-9+1).ToArray ();
		int[] arrTimeAwayThirdValue = Enumerable.Range (100, 300-100+1).ToArray ();
		int[] arrWorkDaysThirdValue = Enumerable.Range (0, 20-0+1).ToArray ();
		List<string> lstPDOTimeSecValue = WeightBL.GetTimeList(3,27,15);
		List <IntlNonConusCity> intlNonConusCities = WeightBL.GetIntlNonConusCities ();
		List<string> lstRestTimeSecValue = WeightBL.GetTimeList(8,24,60);
		List<string> newPdoCitiesList =GlobalSettings.AllCitiesInBid;
		List<DateHelper> newPdoDateList = ConstraintBL.GetPartialDayList();
		List<string> newPdoDateStringList = new List<string>();

		public SmallPopoverTableSource ()
		{
			if (newPdoCitiesList.Contains ("Any City")) {
			} else {
				newPdoCitiesList.Insert (0, "Any City");
			}

			if (newPdoDateStringList.Count > 0)
			{
			} 
			else {
				newPdoDateStringList.Insert (0, "Any Date");
				for (int date = 0; date <newPdoDateList.Count ; date++) {
					DateTime date1 = newPdoDateList [date].Date;
					string str = date1.ToString ("dd - MMM");
					newPdoDateStringList.Insert (date + 1, str);
			}
			}

//			newPdoDateList.Insert (0, "Anydate");
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
            if (PopType == "changeCompareTermInConstraintsCell") {
                if (SubPopType == "Aircraft Changes")
                    return arrCompString1.Length;
                else if (SubPopType == "Blocks of Days Off")
                    return arrCompString2.Length;
                else if (SubPopType == "Duty period")
                    return arrCompString1.Length;
                else if (SubPopType == "Flight Time")
                    return arrCompString1.Length;
                else if (SubPopType == "Ground Time")
                    return arrCompString1.Length;
                else if (SubPopType == "Legs Per Duty Period")
                    return arrCompString1.Length;
                else if (SubPopType == "Legs Per Pairing")
                    return arrCompString1.Length;
                else if (SubPopType == "Number of Days Off")
                    return arrCompString1.Length;
                else if (SubPopType == "Time-Away-From-Base")
                    return arrCompString1.Length;
                else if (SubPopType == "Work Days")
                    return arrCompString2.Length;
                else if (SubPopType == "Min Pay")
                    return arrCompString1.Length;
                else if (SubPopType == "3-on-3-off")
                    return arrCompString5.Length;
                else if (SubPopType == "Overlap Days")
                    return arrCompString1.Length;
                else if (SubPopType == "Days of the Week")
                    return arrCompString1.Length;
                else if (SubPopType == "DH - first - last")
                    return arrCompString1.Length;
                else if (SubPopType == "Equipment Type")
                    return arrCompString1.Length;
                else if (SubPopType == "Overnight Cities")
                    return arrCompString1.Length;
                else if (SubPopType == "Cities-Legs")
                    return arrCompString1.Length;
                else if (SubPopType == "Start Day of Week")
                    return arrCompString1.Length;
                else if (SubPopType == "Rest")
                    return arrCompString1.Length;
                else if (SubPopType == "Trip Length")
                    return arrCompString1.Length;
                else if (SubPopType == "Work Blk Length")
                    return arrCompString1.Length;

                else if (SubPopType == "Cmut DHs")
                    return arrCompString1.Length;
                else if (SubPopType == "PDO")
                    return arrCompString3.Length;
                else if (SubPopType == "Intl – NonConus")
                    return arrCompString4.Length;
                else if (SubPopType == "Commutability")
                    return arrCompString6.Length;
                else if (SubPopType == "Start Day")
                    return startdayParam.Length;
                else
                    return 0;
            } else if (PopType == "changeContraintParam") {
                if (SubPopType == "Aircraft Changes")
                    return arrAirCraft.Length;
                else if (SubPopType == "Blocks of Days Off")
                    return arrBDO.Length;
                else if (SubPopType == "Duty period")
                    return arrDutyPeriodValue.Length;
                else if (SubPopType == "Flight Time")
                    return arrFlightTimeValue.Length;
                else if (SubPopType == "Ground Time")
                    return arrGrndTimeValue2.Length;
                else if (SubPopType == "Legs Per Duty Period")
                    return arrlegsPerPeriodValue.Length;
                else if (SubPopType == "Legs Per Pairing")
                    return arrlegsPerPairingValue.Length;
                else if (SubPopType == "Number of Days Off")
                    return arrNumOfDaysOffValue.Length;
                else if (SubPopType == "Time-Away-From-Base")
                    return arrTimeAwayFrmBaseValue.Length;
                else if (SubPopType == "Work Days")
                    return arrAirCraft.Length;
                else if (SubPopType == "Min Pay")
                    return arrMinPayValue.Length;
                else if (SubPopType == "3-on-3-off")
                    return arrNo3on3offValue.Length;
                else if (SubPopType == "Overlap Days")
                    return arrNoOverlapValue.Length;
                else if (SubPopType == "Days of the Week")
                    return arrDOWValue.Length;
                else if (SubPopType == "DH - first - last")
                    return arrDHFOLValue.Length;
                else if (SubPopType == "Equipment Type")
                    return arrEQTypeValue.Length;
                else if (SubPopType == "Overnight Cities")
                    return arrAirCraft.Length;
                else if (SubPopType == "Cities-Legs")
                    return arrAirCraft.Length;
                else if (SubPopType == "Start Day of Week")
                    return arrStartDayValue.Length;
                else if (SubPopType == "Rest")
                    return arrRestValue.Length;
                else if (SubPopType == "Trip Length")
                    return arrAirCraft.Length;
                else if (SubPopType == "Work Blk Length")
                    return arrAirCraft.Length;
                else if (SubPopType == "Cmut DHs")
                    return arrStartDayValue.Length;
                else if (SubPopType == "PDO")
                    return lstPDOTimes.Count;
                else if (SubPopType == "Commutability")
                    return lstCommutability.Count;
                else if (SubPopType == "Start Day")
                    return startdayValueParam.Length;
                else if (SubPopType == "Intl – NonConus") {
                    if (wBIdStateContent.Constraints.InterConus.lstParameters [index].Type == (int)CityType.International)
                        return GlobalSettings.WBidINIContent.Cities.Count (x => x.International) + 1;
                    else
                        return GlobalSettings.WBidINIContent.Cities.Count (x => x.NonConus);
                } else
                    return 0;
            } else if (PopType == "changeThirdCellParam") {
				if (SubPopType == "Days of the Week")
					return arrDays.Length;
				else if (SubPopType == "DH - first - last")
					return arrFirstLast.Length;
				else if (SubPopType == "Equipment Type")
					return arrEQType.Length;
				else if (SubPopType == "Overnight Cities")
					//return GlobalSettings.OverNightCitiesInBid.Count;
					return GlobalSettings.WBidINIContent.Cities.Count;
				else if (SubPopType == "Cities-Legs")
					return GlobalSettings.WBidINIContent.Cities.Count;
				else if (SubPopType == "Start Day of Week")
                    return arrDays.Length;
                else if (SubPopType == "Rest")
                    return arrRest.Length;
                else if (SubPopType == "Trip Length")
                    return arrTripLenth.Length;
                else if (SubPopType == "Work Blk Length")
                    return arrTripLenth.Length;
                else if (SubPopType == "Cmut DHs")
                    return arrCmut2.Length;
                else if (SubPopType == "PDO")
                    return newPdoCitiesList.Count;
                //					return GlobalSettings.AllCitiesInBid.Count;
                else if (SubPopType == "Ground Time")
                    return arrGrndTimeValue.Length;
                else if (SubPopType == "Commutability")
                    return arrCommutabilty.Length;
                else if (SubPopType == "Start Day")
                    return startdayParam.Length;
                else
                    return 0;
            } else if (PopType == "changeSecondCellParam") {
                if (SubPopType == "Cmut DHs")
                    return GlobalSettings.AllCitiesInBid.Count;
                else if (SubPopType == "PDO")
                    return newPdoDateStringList.Count;
                //					return lstPDODays.Count;
                else if (SubPopType == "Commutability")
                    return arrCommtblty2.Length;
				else if (SubPopType == "Start Day of Week")
					return arrSdow.Length;
                else
                    return 0;
            } else if (PopType == "changeThirdCellParamInWeightCell") {
				if (SubPopType == "Aircraft Changes")
					return arrAirCraftThirdValue.Length;
				else if (SubPopType == "AM/PM")
					return arrThirdParam1.Length;
				else if (SubPopType == "Blocks of Days Off")
					return arrBDOThirdValue.Length;
				else if (SubPopType == "Cmut DHs")
					return arrCmut.Length;
				else if (SubPopType == "DH - first - last")
					return arrThirdParam2.Length;
				else if (SubPopType == "Duty period")
					return lstDPTimes.Count;
				else if (SubPopType == "Equipment Type")
					return arrBDOThirdValue.Length;
				else if (SubPopType == "Flight Time")
					return arrFltTimeValue.Length;
				else if (SubPopType == "Ground Time")
					return arrGrdValue.Length;
				else if (SubPopType == "Intl – NonConus")
					return intlNonConusCities.Count;
				else if (SubPopType == "Legs Per Duty Period")
					return arrLegsDPValue.Length;
				else if (SubPopType == "Legs Per Pairing")
					return arrAirCraftThirdValue.Length;
				else if (SubPopType == "Number of Days Off")
					return arrNODOThirdValue.Length;
				else if (SubPopType == "Overnight Cities")
					//return GlobalSettings.OverNightCitiesInBid.Count;
					return GlobalSettings.WBidINIContent.Cities.Count;
				else if (SubPopType == "Cities-Legs")
					return GlobalSettings.WBidINIContent.Cities.Count;
				else if (SubPopType == "PDO-after")
					return newPdoCitiesList.Count;
				//return GlobalSettings.AllCitiesInBid.Count;
				else if (SubPopType == "PDO-before")
					return newPdoCitiesList.Count;
				//return GlobalSettings.AllCitiesInBid.Count;
				else if (SubPopType == "Position")
                    return arrThirdParam3.Length;
                else if (SubPopType == "Start Day of Week")
                    return arrDays.Length;
                else if (SubPopType == "Time-Away-From-Base")
                    return arrTimeAwayThirdValue.Length;
                else if (SubPopType == "Trip Length")
                    return arrThirdParam4.Length;
                else if (SubPopType == "Work Blk Length")
                    return arrThirdParam4.Length;
                else if (SubPopType == "Work Days")
                    return arrWorkDaysThirdValue.Length;
                else if (SubPopType == "Rest")
                    return arrSecondParam6.Length;
				else if (SubPopType == "Commutability")
					return arrCompString6.Length;
				else
					return 0;

			} else if (PopType == "changeSecondCellParamInWeightCell") {
				if (SubPopType == "Aircraft Changes")
					return arrSecondParam1.Length;
				else if (SubPopType == "Blocks of Days Off")
					return arrSecondParam1.Length;
				else if (SubPopType == "Cmut DHs")
					return GlobalSettings.AllCitiesInBid.Count;
				else if (SubPopType == "Duty period")
					return arrSecondParam2.Length;
				else if (SubPopType == "Equipment Type")
					return arrEQType.Length;
				else if (SubPopType == "Flight Time")
					return arrSecondParam3.Length;
				else if (SubPopType == "Ground Time")
					return lstGrdTimes.Count;
				else if (SubPopType == "Legs Per Pairing")
					return arrSecondParam4.Length;
				else if (SubPopType == "PDO-after")
					return lstPDOTimeSecValue.Count;
				else if (SubPopType == "PDO-before")
					return lstPDOTimeSecValue.Count;
				else if (SubPopType == "Rest")
					return lstRestTimeSecValue.Count;
				else if (SubPopType == "Work Days")
					return arrSecondParam7.Length;
				else if (SubPopType == "Legs Per Duty Period")
					return arrSecondParam7.Length;
				else if (SubPopType == "Commutability")
					return arrCommutabilty.Length;
				
				else
					return 0;
			}
			else if (PopType == "changeFirstCellParamInWeightCell") {
				if (SubPopType == "PDO-after")
					return newPdoDateStringList.Count;
					//return lstPDODays.Count;
				else if (SubPopType == "PDO-before")
					return newPdoDateStringList.Count;
					//return lstPDODays.Count;
				else if (SubPopType == "Rest")
					return arrSecondParam5.Length;
				else if (SubPopType == "Commutability")
					return arrCommtblty2.Length;
				else
					return 0;
			}
			else if (PopType == "changefourthcellparamWeightCell") {
				if (SubPopType == "Commutability")
					return lstCommutability.Count;
				else
					return 0;
			}
            else if (PopType == "BlockSortCommutability") {
                if (SubPopType == "ValueCommutability")
                    return arrCommutabilty.Length;
                else
                    return 0;
            }
			else if (PopType == "NumeratorData")
			{
				if (SubPopType == "Numerator" || SubPopType == "Denominator")
				{
					
 Numerator = WBidCollection.GetRatioFeatureColumn();
		Numerator.Insert(0, new ColumnDefinition { Id = 0, DisplayName = "Select" });
					return Numerator.Count;
				}
				else
					return 0;
			}



//			else if (PopType == "btnCommutabilityWeight") {
//				if (SubPopType == "Commutability")
//					return lstCommutability.Count;
//				else
//					return 0;
//			}
			else
				return 0;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (SmallPopoverTableCell.Key) as SmallPopoverTableCell;
			if (cell == null)
				cell = new SmallPopoverTableCell ();

			if (PopType == "changeCompareTermInConstraintsCell")
			{
				if (SubPopType == "Aircraft Changes")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Blocks of Days Off")
				{
					cell.TextLabel.Text = arrCompString2[indexPath.Row];
					cell.updateColorForText(arrCompString2[indexPath.Row]);
				}
				else if (SubPopType == "Duty period")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Flight Time")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Ground Time")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Legs Per Duty Period")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Legs Per Pairing")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Number of Days Off")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString2[indexPath.Row]);
				}
				else if (SubPopType == "Time-Away-From-Base")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Work Days")
				{
					cell.TextLabel.Text = arrCompString2[indexPath.Row];
					cell.updateColorForText(arrCompString2[indexPath.Row]);
				}
				else if (SubPopType == "Min Pay")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "3-on-3-off")
				{
					cell.TextLabel.Text = arrCompString5[indexPath.Row];
					//cell.updateColorForText (arrCompString1 [indexPath.Row]);
				}
				else if (SubPopType == "Overlap Days")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Days of the Week")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "DH - first - last")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Equipment Type")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Overnight Cities")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Cities-Legs")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Start Day of Week")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Rest")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Trip Length")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Work Blk Length")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "Cmut DHs")
				{
					cell.TextLabel.Text = arrCompString1[indexPath.Row];
					cell.updateColorForText(arrCompString1[indexPath.Row]);
				}
				else if (SubPopType == "PDO")
				{
					cell.TextLabel.Text = arrCompString3[indexPath.Row];
				}
				else if (SubPopType == "Intl – NonConus")
				{
					cell.TextLabel.Text = arrCompString4[indexPath.Row];
				}
				else if (SubPopType == "Commutability")
				{
					cell.TextLabel.Text = arrCompString6[indexPath.Row];
					cell.updateColorForText(arrCompString6[indexPath.Row]);
				}
                else if (SubPopType == "Start Day")
                {
                    cell.TextLabel.Text = startdayParam[indexPath.Row];
                    cell.updateColorForText(startdayParam[indexPath.Row]);
                }
				else {
					cell.TextLabel.Text = "";
				}


			}
			else if (PopType == "changeContraintParam")
			{
				if (SubPopType == "Aircraft Changes")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Blocks of Days Off")
					cell.TextLabel.Text = arrBDO[indexPath.Row].ToString();
				else if (SubPopType == "Duty period")
					cell.TextLabel.Text = arrDutyPeriodValue[indexPath.Row];
				else if (SubPopType == "Flight Time")
				{
					string txt = arrFlightTimeValue[indexPath.Row].ToString() + ":00";
					cell.TextLabel.Text = txt;
				}
				else if (SubPopType == "Ground Time")
					cell.TextLabel.Text = arrGrndTimeValue2[indexPath.Row].ToString();
				else if (SubPopType == "Legs Per Duty Period")
					cell.TextLabel.Text = arrlegsPerPeriodValue[indexPath.Row].ToString();
				else if (SubPopType == "Legs Per Pairing")
					cell.TextLabel.Text = arrlegsPerPairingValue[indexPath.Row].ToString();
				else if (SubPopType == "Number of Days Off")
					cell.TextLabel.Text = arrNumOfDaysOffValue[indexPath.Row].ToString();
				else if (SubPopType == "Time-Away-From-Base")
				{
					string txt = arrTimeAwayFrmBaseValue[indexPath.Row].ToString() + ":00";
					cell.TextLabel.Text = txt;
				}
				else if (SubPopType == "Work Days")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Min Pay")
					cell.TextLabel.Text = arrMinPayValue[indexPath.Row].ToString();
				else if (SubPopType == "3-on-3-off")
					cell.TextLabel.Text = arrNo3on3offValue[indexPath.Row].ToString();
				else if (SubPopType == "Overlap Days")
					cell.TextLabel.Text = arrNoOverlapValue[indexPath.Row].ToString();
				else if (SubPopType == "Days of the Week")
					cell.TextLabel.Text = arrDOWValue[indexPath.Row].ToString();
				else if (SubPopType == "DH - first - last")
					cell.TextLabel.Text = arrDHFOLValue[indexPath.Row].ToString();
				else if (SubPopType == "Equipment Type")
					cell.TextLabel.Text = arrEQTypeValue[indexPath.Row].ToString();
				else if (SubPopType == "Overnight Cities")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Cities-Legs")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Start Day of Week")
					cell.TextLabel.Text = arrStartDayValue[indexPath.Row].ToString();
				else if (SubPopType == "Rest")
					cell.TextLabel.Text = arrRestValue[indexPath.Row].ToString();
				else if (SubPopType == "Trip Length")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Work Blk Length")
					cell.TextLabel.Text = arrAirCraft[indexPath.Row].ToString();
				else if (SubPopType == "Cmut DHs")
					cell.TextLabel.Text = arrStartDayValue[indexPath.Row].ToString();
				else if (SubPopType == "PDO")
					cell.TextLabel.Text = lstPDOTimes[indexPath.Row];
				else if (SubPopType == "Commutability")
					cell.TextLabel.Text = lstCommutability[indexPath.Row] + " %";
                else if (SubPopType == "Start Day")
                    cell.TextLabel.Text = startdayValueParam[indexPath.Row].ToString();
				else if (SubPopType == "Intl – NonConus")
				{
					if (wBIdStateContent.Constraints.InterConus.lstParameters[index].Type == (int)CityType.International)
					{
						if (indexPath.Row == 0)
							cell.TextLabel.Text = "All";
						else {
							List<City> city = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).ToList();
							cell.TextLabel.Text = city[indexPath.Row - 1].Name;
						}
					}
					else {
						List<City> city = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).ToList();
						cell.TextLabel.Text = city[indexPath.Row].Name;
					}
				}
				else
					cell.TextLabel.Text = "";


			}
			else if (PopType == "changeThirdCellParam")
			{
				if (SubPopType == "Days of the Week")
					cell.TextLabel.Text = arrDays[indexPath.Row];
				else if (SubPopType == "DH - first - last")
					cell.TextLabel.Text = arrFirstLast[indexPath.Row];
				else if (SubPopType == "Equipment Type")
					cell.TextLabel.Text = arrEQType[indexPath.Row];
				else if (SubPopType == "Overnight Cities")
					//cell.TextLabel.Text = GlobalSettings.OverNightCitiesInBid [indexPath.Row].Name;
					cell.TextLabel.Text = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
				else if (SubPopType == "Cities-Legs")
					cell.TextLabel.Text = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
				else if (SubPopType == "Start Day of Week")
					cell.TextLabel.Text = arrDays[indexPath.Row];
				else if (SubPopType == "Rest")
					cell.TextLabel.Text = arrRest[indexPath.Row];
				else if (SubPopType == "Trip Length")
					cell.TextLabel.Text = arrTripLenth[indexPath.Row];
				else if (SubPopType == "Work Blk Length")
					cell.TextLabel.Text = arrTripLenth[indexPath.Row];
				else if (SubPopType == "Cmut DHs")
					cell.TextLabel.Text = arrCmut2[indexPath.Row];
				else if (SubPopType == "PDO")
					cell.TextLabel.Text = newPdoCitiesList[indexPath.Row];
				//				    cell.TextLabel.Text = GlobalSettings.AllCitiesInBid [indexPath.Row];
				else if (SubPopType == "Ground Time")
					cell.TextLabel.Text = arrGrndTimeValue[indexPath.Row];
				else if (SubPopType == "Commutability")
					cell.TextLabel.Text = arrCommutabilty[indexPath.Row];
                else if (SubPopType == "Start Day")
                    cell.TextLabel.Text = arrSdow[indexPath.Row];
                else
					cell.TextLabel.Text = "";


			}
			else if (PopType == "changeSecondCellParam")
			{
				if (SubPopType == "Cmut DHs")
					cell.TextLabel.Text = GlobalSettings.AllCitiesInBid[indexPath.Row];
				else if (SubPopType == "PDO")
				{

					//					DateTime date = newPdoDateList [indexPath.Row].Date;

					//					DateTime date = lstPDODays[indexPath.Row].Date;
					//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
					//					string strMonthName = mfi.GetMonthName(date.Month).ToString();
					//					cell.TextLabel.Text = date.Day.ToString () + " - " + strMonthName;
					//					cell.TextLabel.Text = date.ToString ("dd - MMM");
					cell.TextLabel.Text = newPdoDateStringList[indexPath.Row];

				}
				else if (SubPopType == "Commutability")
				{
					cell.TextLabel.Text = arrCommtblty2[indexPath.Row];
				}
				else if (SubPopType == "Start Day of Week")
					cell.TextLabel.Text = arrSdow[indexPath.Row];
				else
					cell.TextLabel.Text = "";


			}
			else if (PopType == "changeThirdCellParamInWeightCell")
			{
				if (SubPopType == "Aircraft Changes")
					cell.TextLabel.Text = arrAirCraftThirdValue[indexPath.Row].ToString() + " chg";
				else if (SubPopType == "AM/PM")
					cell.TextLabel.Text = arrThirdParam1[indexPath.Row];
				else if (SubPopType == "Blocks of Days Off")
					cell.TextLabel.Text = "blk " + arrBDOThirdValue[indexPath.Row].ToString();
				else if (SubPopType == "Cmut DHs")
					cell.TextLabel.Text = arrCmut[indexPath.Row];
				else if (SubPopType == "DH - first - last")
					cell.TextLabel.Text = arrThirdParam2[indexPath.Row];
				else if (SubPopType == "Duty period")
					cell.TextLabel.Text = lstDPTimes[indexPath.Row];
				else if (SubPopType == "Equipment Type")
					cell.TextLabel.Text = arrBDOThirdValue[indexPath.Row].ToString() + " legs";
				else if (SubPopType == "Flight Time")
					cell.TextLabel.Text = arrFltTimeValue[indexPath.Row].ToString();
				else if (SubPopType == "Ground Time")
					cell.TextLabel.Text = arrGrdValue[indexPath.Row].ToString() + " occurs";
				else if (SubPopType == "Intl – NonConus")
					cell.TextLabel.Text = intlNonConusCities[indexPath.Row].City;
				else if (SubPopType == "Legs Per Duty Period")
					cell.TextLabel.Text = arrLegsDPValue[indexPath.Row].ToString() + " legs";
				else if (SubPopType == "Legs Per Pairing")
					cell.TextLabel.Text = arrAirCraftThirdValue[indexPath.Row].ToString() + " legs";
				else if (SubPopType == "Number of Days Off")
					cell.TextLabel.Text = arrNODOThirdValue[indexPath.Row].ToString() + " off";
				else if (SubPopType == "Overnight Cities")
					//cell.TextLabel.Text = GlobalSettings.OverNightCitiesInBid [indexPath.Row].Name;
					cell.TextLabel.Text = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
				else if (SubPopType == "Cities-Legs")
					cell.TextLabel.Text = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
				else if (SubPopType == "PDO-after")
					cell.TextLabel.Text = newPdoCitiesList[indexPath.Row];
				//cell.TextLabel.Text = GlobalSettings.AllCitiesInBid [indexPath.Row];
				else if (SubPopType == "PDO-before")
					cell.TextLabel.Text = newPdoCitiesList[indexPath.Row];
				//cell.TextLabel.Text = GlobalSettings.AllCitiesInBid [indexPath.Row];
				else if (SubPopType == "Position")
					cell.TextLabel.Text = arrThirdParam3[indexPath.Row];
				else if (SubPopType == "Start Day of Week")
					cell.TextLabel.Text = arrDays[indexPath.Row];
				else if (SubPopType == "Time-Away-From-Base")
					cell.TextLabel.Text = arrTimeAwayThirdValue[indexPath.Row].ToString();
				else if (SubPopType == "Trip Length")
					cell.TextLabel.Text = arrThirdParam4[indexPath.Row];
				else if (SubPopType == "Work Blk Length")
					cell.TextLabel.Text = arrThirdParam4[indexPath.Row];
				else if (SubPopType == "Work Days")
					cell.TextLabel.Text = arrWorkDaysThirdValue[indexPath.Row].ToString() + " wk days";
				else if (SubPopType == "Rest")
					cell.TextLabel.Text = arrSecondParam6[indexPath.Row];
				else if (SubPopType == "Commutability")
				{
					cell.TextLabel.Text = arrCompString6[indexPath.Row];
					cell.updateColorForText(arrCompString6[indexPath.Row]);
				}
				else
					cell.TextLabel.Text = "";


			}
			else if (PopType == "changeSecondCellParamInWeightCell")
			{
				if (SubPopType == "Aircraft Changes")
				{
					cell.TextLabel.Text = arrSecondParam1[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "Blocks of Days Off")
				{
					cell.TextLabel.Text = arrSecondParam1[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "Cmut DHs")
					cell.TextLabel.Text = GlobalSettings.AllCitiesInBid[indexPath.Row];
				else if (SubPopType == "Duty period")
					cell.TextLabel.Text = arrSecondParam2[indexPath.Row];
				else if (SubPopType == "Equipment Type")
					cell.TextLabel.Text = arrEQType[indexPath.Row];
				else if (SubPopType == "Flight Time")
				{
					cell.TextLabel.Text = arrSecondParam3[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "Ground Time")
					cell.TextLabel.Text = lstGrdTimes[indexPath.Row];
				else if (SubPopType == "Legs Per Pairing")
				{
					cell.TextLabel.Text = arrSecondParam4[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "PDO-after")
					cell.TextLabel.Text = lstPDOTimeSecValue[indexPath.Row];
				else if (SubPopType == "PDO-before")
					cell.TextLabel.Text = lstPDOTimeSecValue[indexPath.Row];
				else if (SubPopType == "Rest")
					cell.TextLabel.Text = lstRestTimeSecValue[indexPath.Row];
				else if (SubPopType == "Work Days")
				{
					cell.TextLabel.Text = arrSecondParam7[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "Legs Per Duty Period")
				{
					cell.TextLabel.Text = arrSecondParam7[indexPath.Row];
					cell.updateColorForText2(cell.TextLabel.Text);
				}
				else if (SubPopType == "Commutability")
				{
					cell.TextLabel.Text = arrCommutabilty[indexPath.Row];
				}
				else
					cell.TextLabel.Text = "";


			}
			else if (PopType == "changeFirstCellParamInWeightCell")
			{
				if (SubPopType == "PDO-after")
				{
					cell.TextLabel.Text = newPdoDateStringList[indexPath.Row];
					//DateTime date = lstPDODays [indexPath.Row].Date;
					//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
					//					string strMonthName = mfi.GetMonthName (date.Month).ToString ();
					//					cell.TextLabel.Text = date.Day.ToString () + " - " + strMonthName;
					//cell.TextLabel.Text = date.ToString ("dd - MMM");
				}
				else if (SubPopType == "PDO-before")
				{
					cell.TextLabel.Text = newPdoDateStringList[indexPath.Row];
					//DateTime date = lstPDODays [indexPath.Row].Date;
					//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
					//					string strMonthName = mfi.GetMonthName (date.Month).ToString ();
					//					cell.TextLabel.Text = date.Day.ToString () + " - " + strMonthName;
					//cell.TextLabel.Text = date.ToString ("dd - MMM");
				}
				else if (SubPopType == "Rest")
					cell.TextLabel.Text = arrSecondParam5[indexPath.Row];
				else if (SubPopType == "Commutability")
				{
					cell.TextLabel.Text = arrCommtblty2[indexPath.Row];
				}
				else
					cell.TextLabel.Text = "";
			}

			else if (PopType == "changefourthcellparamWeightCell")
			{
				if (SubPopType == "Commutability")
					cell.TextLabel.Text = lstCommutability[indexPath.Row] + " %";

				else
					cell.TextLabel.Text = "";
			}

			else if (PopType == "BlockSortCommutability")
			{
				if (SubPopType == "ValueCommutability")
					cell.TextLabel.Text = arrCommutabilty[indexPath.Row];

				else
					cell.TextLabel.Text = "";
            }
			else if (PopType == "NumeratorData")
			{
				if (SubPopType == "Numerator" || SubPopType == "Denominator")
				{
					ColumnDefinition obj = Numerator[indexPath.Row] as ColumnDefinition;
                     cell.TextLabel.Text = obj.DisplayName;
				}
			}
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 40;
		}
        /// <summary>
        /// executes when an item from the drop down selected 
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			WBidHelper.PushToUndoStack ();
			// Constraints compare terms (more than / greater than)
			if (PopType == "changeCompareTermInConstraintsCell") {
				if (SubPopType == "Aircraft Changes")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.AircraftChanges.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyAirCraftChangesConstraint(wBIdStateContent.Constraints.AircraftChanges);
				}
				else if (SubPopType == "Blocks of Days Off")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString2[indexPath.Row]));
					if (arrCompString2[indexPath.Row] == "less than")
						wBIdStateContent.Constraints.BlockOfDaysOff.Type = (int)ConstraintType.LessThan;
					else if (arrCompString2[indexPath.Row] == "more than")
						wBIdStateContent.Constraints.BlockOfDaysOff.Type = (int)ConstraintType.MoreThan;
					else if (arrCompString2[indexPath.Row] == "equal to")
						wBIdStateContent.Constraints.BlockOfDaysOff.Type = (int)ConstraintType.EqualTo;
					constCalc.ApplyBlockOfDaysOffConstraint(wBIdStateContent.Constraints.BlockOfDaysOff);
				}
				else if (SubPopType == "Duty period")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.DutyPeriod.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyDutyPeriodConstraint(wBIdStateContent.Constraints.DutyPeriod);
				}
				else if (SubPopType == "Flight Time")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.FlightMin.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyFlightTimeConstraint(wBIdStateContent.Constraints.FlightMin);
				}
				else if (SubPopType == "Ground Time")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.GroundTime.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyGroundTimeConstraint(wBIdStateContent.Constraints.GroundTime);
				}
				else if (SubPopType == "Legs Per Duty Period")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.LegsPerDutyPeriod.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyLegsPerDutyPeriodConstraint(wBIdStateContent.Constraints.LegsPerDutyPeriod);
				}
				else if (SubPopType == "Legs Per Pairing")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.LegsPerPairing.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyLegsPerPairingConstraint(wBIdStateContent.Constraints.LegsPerPairing);
				}
				else if (SubPopType == "Number of Days Off")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					if (arrCompString1[indexPath.Row] == "less than")
						wBIdStateContent.Constraints.NumberOfDaysOff.Type = (int)ConstraintType.LessThan;
					else if (arrCompString1[indexPath.Row] == "more than")
						wBIdStateContent.Constraints.NumberOfDaysOff.Type = (int)ConstraintType.MoreThan;
					constCalc.ApplyNumberofDaysOffConstraint(wBIdStateContent.Constraints.NumberOfDaysOff);
				}
				else if (SubPopType == "Time-Away-From-Base")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.PerDiem.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyTimeAwayFromBaseConstraint(wBIdStateContent.Constraints.PerDiem);
				}
				else if (SubPopType == "Work Days")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString2[indexPath.Row]));
					if (arrCompString2[indexPath.Row] == "less than")
						wBIdStateContent.Constraints.WorkDay.Type = (int)ConstraintType.LessThan;
					else if (arrCompString2[indexPath.Row] == "more than")
						wBIdStateContent.Constraints.WorkDay.Type = (int)ConstraintType.MoreThan;
					else if (arrCompString2[indexPath.Row] == "equal to")
						wBIdStateContent.Constraints.WorkDay.Type = (int)ConstraintType.EqualTo;
					constCalc.ApplyWorkDaysConstraint(wBIdStateContent.Constraints.WorkDay);
				}
				else if (SubPopType == "Min Pay")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.MinimumPay.Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyMinimumPayConstraint(wBIdStateContent.Constraints.MinimumPay);
				}
				else if (SubPopType == "3-on-3-off")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString5[indexPath.Row]));
					if (arrCompString5[indexPath.Row] == "Only 3-on-3-off")
						wBIdStateContent.Constraints.No3On3Off.Type = (int)ThreeOnThreeOff.ThreeOnThreeOff;
					else
						wBIdStateContent.Constraints.No3On3Off.Type = (int)ThreeOnThreeOff.NoThreeOnThreeOff;
					constCalc.ApplyThreeOn3offConstraint(wBIdStateContent.Constraints.No3On3Off);
				}
				else if (SubPopType == "Overlap Days")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					//wBIdStateContent.Constraints.NoOverLap.Type = (arrCompString1[indexPath.Row]=="less than")?1:3;
					//constCalc.ApplyThreeOn3offConstraint (wBIdStateContent.Constraints.No3On3Off);
				}
				else if (SubPopType == "Days of the Week")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.DaysOfWeek.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyDaysofWeekConstraint(wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
				}
				else if (SubPopType == "DH - first - last")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.DeadHeadFoL.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
				}
				else if (SubPopType == "Equipment Type")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.EQUIP.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyEquipmentTypeConstraint(wBIdStateContent.Constraints.EQUIP.lstParameters);
				}
				else if (SubPopType == "Overnight Cities")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					if (arrCompString1[indexPath.Row] == "less than")
						wBIdStateContent.Constraints.OverNightCities.lstParameters[index].Type = (int)ConstraintType.LessThan;
					else if (arrCompString1[indexPath.Row] == "more than")
						wBIdStateContent.Constraints.OverNightCities.lstParameters[index].Type = (int)ConstraintType.MoreThan;
					constCalc.ApplyOvernightCitiesConstraint(wBIdStateContent.Constraints.OverNightCities.lstParameters);
				}
				else if (SubPopType == "Cities-Legs")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					if (wBIdStateContent.Constraints.CitiesLegs == null)
					{
						wBIdStateContent.Constraints.CitiesLegs = new Cx3Parameters
						{
							ThirdcellValue = "1",
							Type = (int)ConstraintType.LessThan,
							Value = 1,
							lstParameters = new List<Cx3Parameter>()
						};
					}

					if (arrCompString1[indexPath.Row] == "less than")
						wBIdStateContent.Constraints.CitiesLegs.lstParameters[index].Type = (int)ConstraintType.LessThan;
					else if (arrCompString1[indexPath.Row] == "more than")
						wBIdStateContent.Constraints.CitiesLegs.lstParameters[index].Type = (int)ConstraintType.MoreThan;
					constCalc.ApplyCitiesLegsConstraint(wBIdStateContent.Constraints.CitiesLegs.lstParameters);
				}
				else if (SubPopType == "Start Day of Week")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyStartDayOfWeekConstraint(wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
				}
				else if (SubPopType == "Rest")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.Rest.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyRestConstraint(wBIdStateContent.Constraints.Rest.lstParameters);
				}
				else if (SubPopType == "Trip Length")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.TripLength.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyTripLengthConstraint(wBIdStateContent.Constraints.TripLength.lstParameters);
				}
				else if (SubPopType == "Work Blk Length")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.WorkBlockLength.lstParameters[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyWorkBlockLengthConstraint(wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
				}
				else if (SubPopType == "Cmut DHs")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString1[indexPath.Row]));
					wBIdStateContent.Constraints.DeadHeads.LstParameter[index].Type = (arrCompString1[indexPath.Row] == "less than") ? 1 : 3;
					constCalc.ApplyCommutableDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeads.LstParameter);
				}
				else if (SubPopType == "PDO")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString3[indexPath.Row]));
					if (arrCompString3[indexPath.Row] == "at+after")
						wBIdStateContent.Constraints.PDOFS.LstParameter[index].Type = (int)ConstraintType.atafter;
					else if (arrCompString3[indexPath.Row] == "at+before")
						wBIdStateContent.Constraints.PDOFS.LstParameter[index].Type = (int)ConstraintType.atbefore;
					constCalc.ApplyPartialdaysOffConstraint(wBIdStateContent.Constraints.PDOFS.LstParameter);
				}
				else if (SubPopType == "Commutability")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString6[indexPath.Row]));
					wBIdStateContent.Constraints.Commute.Type = (arrCompString6[indexPath.Row] == "<=") ? 1 : 3;
					constCalc.ApplyCommuttabilityConstraint(wBIdStateContent.Constraints.Commute);
				}
				else if (SubPopType == "Intl – NonConus")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(arrCompString4[indexPath.Row]));
					if (arrCompString4[indexPath.Row] == "Intl")
					{
						wBIdStateContent.Constraints.InterConus.lstParameters[index].Type = (int)CityType.International;
						wBIdStateContent.Constraints.InterConus.lstParameters[index].Value = 0;
					}
					else if (arrCompString4[indexPath.Row] == "NonConus")
					{
						wBIdStateContent.Constraints.InterConus.lstParameters[index].Type = (int)CityType.NonConus;
						wBIdStateContent.Constraints.InterConus.lstParameters[index].Value = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).ToList()[0].Id;
					}
					constCalc.ApplyInternationalonConusConstraint(wBIdStateContent.Constraints.InterConus.lstParameters);
				}
                else if (SubPopType == "Start Day")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("changeCompareTermInConstraintsCell", new NSString(startdayParam[indexPath.Row]));
                    wBIdStateContent.Constraints.StartDay.lstParameters[index].Type = (startdayParam[indexPath.Row] == "Start On") ? (int)StartDay.StartOn : (int)StartDay.DoesnotStart;
                    constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
                }
				

				else {
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
			} 


			else if (PopType == "changeContraintParam") {
				if (SubPopType == "Aircraft Changes") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.AircraftChanges.Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyAirCraftChangesConstraint (wBIdStateContent.Constraints.AircraftChanges);
				} else if (SubPopType == "Blocks of Days Off") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrBDO [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.BlockOfDaysOff.Value = arrBDO [indexPath.Row];
					constCalc.ApplyBlockOfDaysOffConstraint (wBIdStateContent.Constraints.BlockOfDaysOff);
				} else if (SubPopType == "Duty period") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrDutyPeriodValue [indexPath.Row]));
					wBIdStateContent.Constraints.DutyPeriod.Value = Helper.ConvertHHMMtoMinute (arrDutyPeriodValue [indexPath.Row]);
					constCalc.ApplyDutyPeriodConstraint (wBIdStateContent.Constraints.DutyPeriod);
				} else if (SubPopType == "Flight Time") {
					string txt = arrFlightTimeValue [indexPath.Row].ToString () + ":00";
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (txt));
					wBIdStateContent.Constraints.FlightMin.Value = Helper.ConvertHHMMtoMinute (txt);
					constCalc.ApplyFlightTimeConstraint (wBIdStateContent.Constraints.FlightMin);
				} else if (SubPopType == "Ground Time") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrGrndTimeValue2 [indexPath.Row].ToString()));
					wBIdStateContent.Constraints.GroundTime.Value = arrGrndTimeValue2 [indexPath.Row];
					constCalc.ApplyGroundTimeConstraint (wBIdStateContent.Constraints.GroundTime);
				} else if (SubPopType == "Legs Per Duty Period") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrlegsPerPeriodValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.LegsPerDutyPeriod.Value = arrlegsPerPeriodValue [indexPath.Row];
					constCalc.ApplyLegsPerDutyPeriodConstraint (wBIdStateContent.Constraints.LegsPerDutyPeriod);
				} else if (SubPopType == "Legs Per Pairing") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrlegsPerPairingValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.LegsPerPairing.Value = arrlegsPerPairingValue [indexPath.Row];
					constCalc.ApplyLegsPerPairingConstraint (wBIdStateContent.Constraints.LegsPerPairing);
				} else if (SubPopType == "Number of Days Off") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrNumOfDaysOffValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.NumberOfDaysOff.Value = arrNumOfDaysOffValue [indexPath.Row];
					constCalc.ApplyNumberofDaysOffConstraint (wBIdStateContent.Constraints.NumberOfDaysOff);
				} else if (SubPopType == "Time-Away-From-Base") {
					string txt = arrTimeAwayFrmBaseValue [indexPath.Row].ToString () + ":00";
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (txt));
					wBIdStateContent.Constraints.PerDiem.Value = Helper.ConvertHHMMtoMinute (txt);
					constCalc.ApplyTimeAwayFromBaseConstraint (wBIdStateContent.Constraints.PerDiem);
				} else if (SubPopType == "Work Days") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.WorkDay.Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyWorkDaysConstraint (wBIdStateContent.Constraints.WorkDay);
				} else if (SubPopType == "Min Pay") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrMinPayValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.MinimumPay.Value = arrMinPayValue [indexPath.Row];
					constCalc.ApplyMinimumPayConstraint (wBIdStateContent.Constraints.MinimumPay);
				} else if (SubPopType == "3-on-3-off") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrNo3on3offValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.No3On3Off.Value = arrNo3on3offValue [indexPath.Row];
					constCalc.ApplyThreeOn3offConstraint (wBIdStateContent.Constraints.No3On3Off);
				} else if (SubPopType == "Overlap Days") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrNoOverlapValue [indexPath.Row].ToString ()));
					//wBIdStateContent.Constraints.NoOverLap.Value = arrNo3on3offValue [indexPath.Row];
					//constCalc.ApplyThreeOn3offConstraint (wBIdStateContent.Constraints.No3On3Off);
				} else if (SubPopType == "Days of the Week") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrDOWValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.DaysOfWeek.lstParameters[index].Value = arrDOWValue [indexPath.Row];
					constCalc.ApplyDaysofWeekConstraint (wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
				} else if (SubPopType == "DH - first - last") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrDHFOLValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.DeadHeadFoL.lstParameters[index].Value = arrDHFOLValue [indexPath.Row];
					constCalc.ApplyDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
				} else if (SubPopType == "Equipment Type") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrEQTypeValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.EQUIP.lstParameters[index].Value = arrEQTypeValue [indexPath.Row];
					constCalc.ApplyEquipmentTypeConstraint (wBIdStateContent.Constraints.EQUIP.lstParameters);
				} 
				else if (SubPopType == "Overnight Cities")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.OverNightCities.lstParameters[index].Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyOvernightCitiesConstraint (wBIdStateContent.Constraints.OverNightCities.lstParameters);
				}
				else if (SubPopType == "Cities-Legs")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					if (wBIdStateContent.Constraints.CitiesLegs == null) 
					{
						wBIdStateContent.Constraints.CitiesLegs = new Cx3Parameters {
							ThirdcellValue = "1",
							Type = (int)ConstraintType.LessThan,
							Value = 1 ,
							lstParameters = new List<Cx3Parameter> ()
						};
					}
					wBIdStateContent.Constraints.CitiesLegs.lstParameters[index].Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyCitiesLegsConstraint (wBIdStateContent.Constraints.CitiesLegs.lstParameters);
				} 
				else if (SubPopType == "Start Day of Week") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrStartDayValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[index].Value = arrStartDayValue [indexPath.Row];
					constCalc.ApplyStartDayOfWeekConstraint (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
				} else if (SubPopType == "Rest") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrRestValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.Rest.lstParameters[index].Value = arrRestValue [indexPath.Row];
					constCalc.ApplyRestConstraint (wBIdStateContent.Constraints.Rest.lstParameters);
				} else if (SubPopType == "Trip Length") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.TripLength.lstParameters[index].Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyTripLengthConstraint (wBIdStateContent.Constraints.TripLength.lstParameters);
				} else if (SubPopType == "Work Blk Length") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrAirCraft [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.WorkBlockLength.lstParameters[index].Value = arrAirCraft [indexPath.Row];
					constCalc.ApplyWorkBlockLengthConstraint (wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
				} else if (SubPopType == "Cmut DHs") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (arrStartDayValue [indexPath.Row].ToString ()));
					wBIdStateContent.Constraints.DeadHeads.LstParameter[index].Value = arrStartDayValue [indexPath.Row];
					constCalc.ApplyCommutableDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeads.LstParameter);
				} else if (SubPopType == "PDO") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (lstPDOTimes [indexPath.Row]));
					wBIdStateContent.Constraints.PDOFS.LstParameter [index].Value = Helper.ConvertHHMMtoMinute (lstPDOTimes [indexPath.Row]);
					constCalc.ApplyPartialdaysOffConstraint (wBIdStateContent.Constraints.PDOFS.LstParameter);
				} else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (lstCommutability [indexPath.Row]));
					wBIdStateContent.Constraints.Commute.Value = Int32.Parse(lstCommutability[indexPath.Row]);
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Constraints.Commute);
				} else if (SubPopType == "Intl – NonConus") {
					if (wBIdStateContent.Constraints.InterConus.lstParameters [index].Type == (int)CityType.International) {
						if (indexPath.Row == 0) {
							wBIdStateContent.Constraints.InterConus.lstParameters [index].Value = 0;
							NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString ("All"));
						}
						else {
							List<City> city = GlobalSettings.WBidINIContent.Cities.Where (x => x.International).ToList ();
							NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (city [indexPath.Row-1].Name));
							int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == city [indexPath.Row-1].Name).Id;
							wBIdStateContent.Constraints.InterConus.lstParameters [index].Value = cityId;
						}
					} else {
						List<City> city = GlobalSettings.WBidINIContent.Cities.Where (x => x.NonConus).ToList ();
						NSNotificationCenter.DefaultCenter.PostNotificationName ("changeParamTermInConstraintsCell", new NSString (city [indexPath.Row].Name));
						int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city[indexPath.Row].Name).Id;
						wBIdStateContent.Constraints.InterConus.lstParameters [index].Value = cityId;
					}
					constCalc.ApplyInternationalonConusConstraint (wBIdStateContent.Constraints.InterConus.lstParameters);

				}
                else if (SubPopType == "Start Day")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("changeParamTermInConstraintsCell", new NSString(startdayValueParam[indexPath.Row].ToString()));
                    wBIdStateContent.Constraints.StartDay.lstParameters[index].Value = startdayValueParam[indexPath.Row];
                    constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
                }
                else {
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);


			} else if (PopType == "changeThirdCellParam") {
				if (SubPopType == "Days of the Week") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrDays [indexPath.Row]));
					wBIdStateContent.Constraints.DaysOfWeek.lstParameters [index].ThirdcellValue = indexPath.Row.ToString ();
					constCalc.ApplyDaysofWeekConstraint (wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
				} else if (SubPopType == "DH - first - last") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrFirstLast [indexPath.Row]));
					if (arrFirstLast [indexPath.Row] == "first")
						wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [index].ThirdcellValue = ((int)DeadheadType.First).ToString ();
					else if (arrFirstLast [indexPath.Row] == "last")
						wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [index].ThirdcellValue = ((int)DeadheadType.Last).ToString ();
					else if (arrFirstLast [indexPath.Row] == "both")
						wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [index].ThirdcellValue = ((int)DeadheadType.Both).ToString ();
					constCalc.ApplyDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
				} else if (SubPopType == "Equipment Type") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrEQType [indexPath.Row]));
					var param= arrEQType[indexPath.Row];
					if (param == "8Max")
						param = "600";
					else if (param == "7Max")
						param = "200";
					wBIdStateContent.Constraints.EQUIP.lstParameters [index].ThirdcellValue = param;
					constCalc.ApplyEquipmentTypeConstraint (wBIdStateContent.Constraints.EQUIP.lstParameters);
				} 
				else if (SubPopType == "Overnight Cities")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name));
					//int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == GlobalSettings.OverNightCitiesInBid[indexPath.Row].Name).Id;
					int cityId = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Id;
					wBIdStateContent.Constraints.OverNightCities.lstParameters [index].ThirdcellValue = cityId.ToString();
					constCalc.ApplyOvernightCitiesConstraint (wBIdStateContent.Constraints.OverNightCities.lstParameters);
				} 
				else if (SubPopType == "Cities-Legs")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (GlobalSettings.WBidINIContent.Cities [indexPath.Row].Name));
					if (wBIdStateContent.Constraints.CitiesLegs == null) 
					{
						wBIdStateContent.Constraints.CitiesLegs = new Cx3Parameters {
							ThirdcellValue = "1",
							Type = (int)ConstraintType.LessThan,
							Value = 1 ,
							lstParameters = new List<Cx3Parameter> ()
						};
					}
					int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == GlobalSettings.WBidINIContent.Cities [indexPath.Row].Name).Id;

					wBIdStateContent.Constraints.CitiesLegs.lstParameters [index].ThirdcellValue = cityId.ToString();
					constCalc.ApplyCitiesLegsConstraint (wBIdStateContent.Constraints.CitiesLegs.lstParameters);
				} 
				else if (SubPopType == "Start Day of Week") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrDays [indexPath.Row]));
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters [index].ThirdcellValue = indexPath.Row.ToString ();
					constCalc.ApplyStartDayOfWeekConstraint (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
				} else if (SubPopType == "Rest") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrRest [indexPath.Row]));
					if (arrRest [indexPath.Row] == "All")
						wBIdStateContent.Constraints.Rest.lstParameters [index].ThirdcellValue = ((int)RestType.All).ToString ();
					else if (arrRest [indexPath.Row] == "InDom")
						wBIdStateContent.Constraints.Rest.lstParameters [index].ThirdcellValue = ((int)RestType.InDomicile).ToString ();
					else if (arrRest [indexPath.Row] == "AwayDom")
						wBIdStateContent.Constraints.Rest.lstParameters [index].ThirdcellValue = ((int)RestType.AwayDomicile).ToString ();
					constCalc.ApplyRestConstraint (wBIdStateContent.Constraints.Rest.lstParameters);
				} else if (SubPopType == "Trip Length") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrTripLenth [indexPath.Row]));
					wBIdStateContent.Constraints.TripLength.lstParameters [index].ThirdcellValue = (indexPath.Row+1).ToString ();
					constCalc.ApplyTripLengthConstraint (wBIdStateContent.Constraints.TripLength.lstParameters);
				} else if (SubPopType == "Work Blk Length") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrTripLenth [indexPath.Row]));
					wBIdStateContent.Constraints.WorkBlockLength.lstParameters [index].ThirdcellValue = (indexPath.Row+1).ToString ();
					constCalc.ApplyWorkBlockLengthConstraint (wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
				} else if (SubPopType == "Cmut DHs") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrCmut2 [indexPath.Row]));
					if (arrCmut2 [indexPath.Row] == "begin")
						wBIdStateContent.Constraints.DeadHeads.LstParameter [index].ThirdcellValue = ((int)DeadheadType.First).ToString ();
					else if (arrCmut2 [indexPath.Row] == "end")
						wBIdStateContent.Constraints.DeadHeads.LstParameter [index].ThirdcellValue = ((int)DeadheadType.Last).ToString ();
					else if (arrCmut2 [indexPath.Row] == "either")
						wBIdStateContent.Constraints.DeadHeads.LstParameter [index].ThirdcellValue = ((int)DeadheadType.Either).ToString ();
					else if (arrCmut2 [indexPath.Row] == "both")
						wBIdStateContent.Constraints.DeadHeads.LstParameter [index].ThirdcellValue = ((int)DeadheadType.Both).ToString ();
					constCalc.ApplyCommutableDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeads.LstParameter);
				} else if (SubPopType == "PDO") {

				
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (newPdoCitiesList[indexPath.Row]));
					if ((newPdoCitiesList [indexPath.Row]) == "Any City") {
						wBIdStateContent.Constraints.PDOFS.LstParameter [index].ThirdcellValue = "400";
					} else {
						int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == newPdoCitiesList [indexPath.Row]).Id;
						wBIdStateContent.Constraints.PDOFS.LstParameter [index].ThirdcellValue = cityId.ToString ();
					}
					constCalc.ApplyPartialdaysOffConstraint (wBIdStateContent.Constraints.PDOFS.LstParameter);



//					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (GlobalSettings.AllCitiesInBid [indexPath.Row]));
//					int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == GlobalSettings.AllCitiesInBid[indexPath.Row]).Id;
//					wBIdStateContent.Constraints.PDOFS.LstParameter[index].ThirdcellValue = cityId.ToString();
//					constCalc.ApplyPartialdaysOffConstraint (wBIdStateContent.Constraints.PDOFS.LstParameter);
				}else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell",new NSString (arrCommutabilty [indexPath.Row]));
					wBIdStateContent.Constraints.Commute.ThirdcellValue = indexPath.Row+1;
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Constraints.Commute);
				} else if (SubPopType == "Ground Time") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInConstraintsCell", new NSString (arrGrndTimeValue [indexPath.Row]));
					wBIdStateContent.Constraints.GroundTime.ThirdcellValue = Helper.ConvertHHMMtoMinute (arrGrndTimeValue [indexPath.Row]).ToString ();
					constCalc.ApplyGroundTimeConstraint (wBIdStateContent.Constraints.GroundTime);
				
                }
                else if (SubPopType == "Start Day")
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("changeThirdCellParamInConstraintsCell", new NSString(arrSdow[indexPath.Row]));
                    wBIdStateContent.Constraints.StartDay.lstParameters[index].ThirdcellValue = (indexPath.Row == 0) ? ((int)StartDayType.Block).ToString() : ((int)StartDayType.Trip).ToString();
                    constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
                }
                else {
				}

				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);


			} else if (PopType == "changeSecondCellParam") {
				if (SubPopType == "Cmut DHs") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInConstraintsCell", new NSString (GlobalSettings.AllCitiesInBid [indexPath.Row]));
                    int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == GlobalSettings.AllCitiesInBid[indexPath.Row]).Id;
                    wBIdStateContent.Constraints.DeadHeads.LstParameter[index].SecondcellValue = cityId.ToString();
					constCalc.ApplyCommutableDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeads.LstParameter);
				} else if (SubPopType == "PDO") {
//					DateTime date = lstPDODays[indexPath.Row].Date;
//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
//					string strMonthName = mfi.GetMonthName(date.Month).ToString();
//					string title = date.Day.ToString () + " - " + strMonthName;
//					string title = date.ToString ("dd - MMM");
					string title =newPdoDateStringList [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInConstraintsCell", new NSString (title));
					if (title == "Any Date") 
					{
						wBIdStateContent.Constraints.PDOFS.LstParameter [index].SecondcellValue = "300";
					}else {
					wBIdStateContent.Constraints.PDOFS.LstParameter [index].SecondcellValue = lstPDODays [indexPath.Row-1].DateId.ToString();
					}
					constCalc.ApplyPartialdaysOffConstraint (wBIdStateContent.Constraints.PDOFS.LstParameter);

				} 
				else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInConstraintsCell",new NSString (arrCommtblty2 [indexPath.Row]));
					wBIdStateContent.Constraints.Commute.SecondcellValue = indexPath.Row+1;
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Constraints.Commute);
				}
				else if (SubPopType == "Start Day of Week")
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("changeSecondCellParamInConstraintsCell", new NSString(arrCommtblty2[indexPath.Row]));
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[index].SecondcellValue = (indexPath.Row + 1).ToString();
					constCalc.ApplyStartDayOfWeekConstraint(wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
				}
				else {
				}

				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);


			} else if (PopType == "changeThirdCellParamInWeightCell") {
				if (SubPopType == "Aircraft Changes") {
					int param = arrAirCraftThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue = param;
					weightCalc.ApplyAirCraftChangeWeight (wBIdStateContent.Weights.AirCraftChanges);
				} else if (SubPopType == "AM/PM") {
					string param = arrThirdParam1 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "am")
						wBIdStateContent.Weights.AM_PM.lstParameters [index].Type = (int)AMPMType.AM;
					else if (param == "pm")
						wBIdStateContent.Weights.AM_PM.lstParameters [index].Type = (int)AMPMType.PM;
					else if (param == "nte")
						wBIdStateContent.Weights.AM_PM.lstParameters [index].Type = (int)AMPMType.NTE;
					weightCalc.ApplyAMPMWeight (wBIdStateContent.Weights.AM_PM.lstParameters);
				} else if (SubPopType == "Blocks of Days Off") {
					int param = arrBDOThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.BDO.lstParameters[index].ThrirdCellValue = param;
					weightCalc.ApplyBlockOFFDaysOfWeight (wBIdStateContent.Weights.BDO.lstParameters);
				} else if (SubPopType == "Cmut DHs") {
					string param = arrCmut [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "begin")
						wBIdStateContent.Weights.DHD.lstParameters [index].ThrirdCellValue = (int)DeadheadType.First;
					else if (param == "end")
						wBIdStateContent.Weights.DHD.lstParameters [index].ThrirdCellValue = (int)DeadheadType.Last;
					else if (param == "both")
						wBIdStateContent.Weights.DHD.lstParameters [index].ThrirdCellValue = (int)DeadheadType.Both;
					weightCalc.ApplyCommutableDeadhead (wBIdStateContent.Weights.DHD.lstParameters);
				} else if (SubPopType == "DH - first - last") {
					string param = arrThirdParam2 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "frist")
						wBIdStateContent.Weights.DHDFoL.lstParameters [index].Type = (int)DeadheadType.First;
					else if (param == "last")
						wBIdStateContent.Weights.DHDFoL.lstParameters [index].Type = (int)DeadheadType.Last;
					weightCalc.ApplyDeadheadFisrtLastWeight (wBIdStateContent.Weights.DHDFoL.lstParameters);
				} else if (SubPopType == "Duty period") {
					string param = lstDPTimes [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.DP.lstParameters [index].ThrirdCellValue = Helper.ConvertHHMMtoMinute (param);
					weightCalc.ApplyDutyPeriodWeight (wBIdStateContent.Weights.DP.lstParameters);
				} else if (SubPopType == "Equipment Type") {
					int param = arrBDOThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.EQUIP.lstParameters [index].ThrirdCellValue = param;
					weightCalc.ApplyEquipmentTypeWeights (wBIdStateContent.Weights.EQUIP.lstParameters);
				} else if (SubPopType == "Flight Time") {
					int param = arrFltTimeValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.FLTMIN.lstParameters [index].ThrirdCellValue = param;
					weightCalc.ApplyFlightTimeWeights (wBIdStateContent.Weights.FLTMIN.lstParameters);
				} else if (SubPopType == "Ground Time") {
					int param = arrGrdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.GRD.lstParameters [index].ThrirdCellValue = param;
					weightCalc.ApplyGroundTimeWeight (wBIdStateContent.Weights.GRD.lstParameters);
				} else if (SubPopType == "Intl – NonConus") {
					int param = intlNonConusCities [indexPath.Row].CityId;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.InterConus.lstParameters [index].Type = param;
					weightCalc.ApplyInternationalNonConusWeight (wBIdStateContent.Weights.InterConus.lstParameters);
				} else if (SubPopType == "Legs Per Duty Period") {
					int param = arrLegsDPValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.LEGS.lstParameters [index].ThrirdCellValue = param;
					weightCalc.ApplyLegsPerDutyPeriodWeight (wBIdStateContent.Weights.LEGS.lstParameters);
				} else if (SubPopType == "Legs Per Pairing") {
					int param = arrAirCraftThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].ThrirdCellValue = param;
					weightCalc.ApplyLegsPerPairingWeight (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
				} else if (SubPopType == "Number of Days Off") {
					int param = arrNODOThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.NODO.lstParameters [index].Type = param;
					weightCalc.ApplyNumberOfDaysOfWeight (wBIdStateContent.Weights.NODO.lstParameters);
				}
				else if (SubPopType == "Overnight Cities") 
				{
					string param = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					//int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == param).Id;
					int cityId = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Id;
					wBIdStateContent.Weights.RON.lstParameters [index].Type = cityId;
					weightCalc.ApplyOverNightCitiesWeight (wBIdStateContent.Weights.RON.lstParameters);
				} 
				else if (SubPopType == "Cities-Legs") 
				{
					if (wBIdStateContent.Weights.CitiesLegs == null) 
					{
						wBIdStateContent.Weights.CitiesLegs = new Wt2Parameters {
							Type = 1,
							Weight = 0,
							lstParameters = new List<Wt2Parameter> ()
						};
					}
					string param = GlobalSettings.WBidINIContent.Cities[indexPath.Row].Name;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == param).Id;
					wBIdStateContent.Weights.CitiesLegs.lstParameters [index].Type = cityId;
					weightCalc.ApplyCitiesLegsWeight (wBIdStateContent.Weights.CitiesLegs.lstParameters);
				} 
				else if (SubPopType == "PDO-after") {
					
					string param =newPdoCitiesList [indexPath.Row];
					//string param = GlobalSettings.AllCitiesInBid [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "Any City") {
						wBIdStateContent.Weights.PDAfter.lstParameters[index].ThrirdCellValue = 400;
					} else {
						int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == param).Id;
						wBIdStateContent.Weights.PDAfter.lstParameters[index].ThrirdCellValue = cityId;
					}

					weightCalc.ApplyPartialDaysAfterWeight (wBIdStateContent.Weights.PDAfter.lstParameters);
				} else if (SubPopType == "PDO-before") {
					string param =newPdoCitiesList [indexPath.Row];
					//string param = GlobalSettings.AllCitiesInBid [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "Any City") {
						wBIdStateContent.Weights.PDBefore.lstParameters [index].ThrirdCellValue = 400;
					} else {
						int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == param).Id;
						wBIdStateContent.Weights.PDBefore.lstParameters [index].ThrirdCellValue = cityId;
					}
					weightCalc.ApplyPartialDaysBeforeWeight (wBIdStateContent.Weights.PDBefore.lstParameters);
				} else if (SubPopType == "Position") {
					string param = arrThirdParam3 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "A")
						wBIdStateContent.Weights.POS.lstParameters [index].Type = (int)FAPositon.A;
					else if (param == "B")
						wBIdStateContent.Weights.POS.lstParameters [index].Type = (int)FAPositon.B;
					else if (param == "C")
						wBIdStateContent.Weights.POS.lstParameters [index].Type = (int)FAPositon.C;
					else if (param == "D")
						wBIdStateContent.Weights.POS.lstParameters [index].Type = (int)FAPositon.D;
					weightCalc.ApplyPositionWeight (wBIdStateContent.Weights.POS.lstParameters);
				} else if (SubPopType == "Start Day of Week") {

                    //in wbidmax we are heeping the value 1,2,3,4,5,6 etc for MOn,Tue,Wed,Thu,Fri,Sat and Sun
                    // Bud  wbid ipad having the enum value 0,1,2,3,...      MOn,Tue,Wed,Thu,Fri,Sat and Sun
                    //That is why we are adding 1 . Since we need to implement Synch feature
					string param = arrDays [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "sun")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Sun;
					else if (param == "mon")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Mon;
					else if (param == "tue")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Tue;
					else if (param == "wed")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Wed;
					else if (param == "thu")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Thu;
					else if (param == "fri")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Fri;
					else if (param == "sat")
						wBIdStateContent.Weights.SDOW.lstParameters [index].Type = (int)Dow.Sat;
					weightCalc.ApplyStartDayOfWeekWeight (wBIdStateContent.Weights.SDOW.lstParameters);
				} else if (SubPopType == "Time-Away-From-Base") {
					int param = arrTimeAwayThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.PerDiem.Type = param;
					weightCalc.ApplyTimeAwayFromBaseWeight (wBIdStateContent.Weights.PerDiem);
				} else if (SubPopType == "Trip Length") {
					string param = arrThirdParam4 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.TL.lstParameters [index].Type = indexPath.Row + 1;
					weightCalc.ApplyTripLengthWeight (wBIdStateContent.Weights.TL.lstParameters);
				} else if (SubPopType == "Work Blk Length") {
					string param = arrThirdParam4 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.WB.lstParameters [index].Type = indexPath.Row + 1;
					weightCalc.ApplyWorkBlockLengthWeight (wBIdStateContent.Weights.WB.lstParameters);
				} else if (SubPopType == "Work Days") {
					int param = arrWorkDaysThirdValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param.ToString()));
					wBIdStateContent.Weights.WorkDays.lstParameters [index].ThrirdCellValue = arrWorkDaysThirdValue [indexPath.Row];
					weightCalc.ApplyWorkDaysWeight (wBIdStateContent.Weights.WorkDays.lstParameters);
				}else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (arrCompString6 [indexPath.Row]));
					wBIdStateContent.Weights.Commute.Type = (arrCompString6[indexPath.Row]=="<=")?1:3;
					//constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Weights.Commute);
					weightCalc.ApplyCommuttabilityWeight(wBIdStateContent.Weights.Commute);
				}
				else if (SubPopType == "Rest") {
					string param = arrSecondParam6 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeThirdCellParamInWeightCell", new NSString (param));
					if (param == "all")
						wBIdStateContent.Weights.WtRest.lstParameters [index].ThrirdCellValue = (int)RestType.All;
					else if (param == "away")
						wBIdStateContent.Weights.WtRest.lstParameters [index].ThrirdCellValue = (int)RestType.AwayDomicile;
					else if (param == "inDom")
						wBIdStateContent.Weights.WtRest.lstParameters [index].ThrirdCellValue = (int)RestType.InDomicile;
					weightCalc.ApplyRestWeight (wBIdStateContent.Weights.WtRest.lstParameters);

				
				
				}

				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);


			} else if (PopType == "changeSecondCellParamInWeightCell") {
				if (SubPopType == "Aircraft Changes") {
					string param = arrSecondParam1 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "less")
						wBIdStateContent.Weights.AirCraftChanges.SecondlValue = (int)WeightType.Less;
					else if (param == "more")
						wBIdStateContent.Weights.AirCraftChanges.SecondlValue = (int)WeightType.More;
					else if (param == "equal")
						wBIdStateContent.Weights.AirCraftChanges.SecondlValue = (int)WeightType.Equal;
					else if (param == "not equal")
						wBIdStateContent.Weights.AirCraftChanges.SecondlValue = (int)WeightType.NotEqual;
					weightCalc.ApplyAirCraftChangeWeight (wBIdStateContent.Weights.AirCraftChanges);
				} else if (SubPopType == "Blocks of Days Off") {
					string param = arrSecondParam1 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "less")
						wBIdStateContent.Weights.BDO.lstParameters [index].SecondlValue = (int)WeightType.Less;
					else if (param == "more")
						wBIdStateContent.Weights.BDO.lstParameters [index].SecondlValue = (int)WeightType.More;
					else if (param == "equal")
						wBIdStateContent.Weights.BDO.lstParameters [index].SecondlValue = (int)WeightType.Equal;
					else if (param == "not equal")
						wBIdStateContent.Weights.BDO.lstParameters [index].SecondlValue = (int)WeightType.NotEqual;
					weightCalc.ApplyBlockOFFDaysOfWeight (wBIdStateContent.Weights.BDO.lstParameters);
				} else if (SubPopType == "Cmut DHs") {
					string param = GlobalSettings.AllCitiesInBid [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					int cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == param).Id;
					wBIdStateContent.Weights.DHD.lstParameters[index].SecondlValue = cityId;
					weightCalc.ApplyCommutableDeadhead (wBIdStateContent.Weights.DHD.lstParameters);
				} else if (SubPopType == "Duty period") {
					string param = arrSecondParam2 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "relative")
						wBIdStateContent.Weights.DP.lstParameters [index].SecondlValue = (int)DutyPeriodType.Relative;
					else if (param == "longer")
						wBIdStateContent.Weights.DP.lstParameters [index].SecondlValue = (int)DutyPeriodType.Longer;
					else if (param == "shorter")
						wBIdStateContent.Weights.DP.lstParameters [index].SecondlValue = (int)DutyPeriodType.Shorter;
					weightCalc.ApplyDutyPeriodWeight (wBIdStateContent.Weights.DP.lstParameters);
				} else if (SubPopType == "Equipment Type") {
					string param = arrEQType [indexPath.Row];
					if (param == "8Max")
						param = "600";
					else if (param == "7Max")
						param = "200";
					//param = (param == "MAX") ? "600" : param;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.EQUIP.lstParameters [index].SecondlValue = Convert.ToInt32 (param);
					weightCalc.ApplyEquipmentTypeWeights (wBIdStateContent.Weights.EQUIP.lstParameters);
				} else if (SubPopType == "Flight Time") {
					string param = arrSecondParam3 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "less")
						wBIdStateContent.Weights.FLTMIN.lstParameters [index].SecondlValue = (int)WeightType.Less;
					else if (param == "more")
						wBIdStateContent.Weights.FLTMIN.lstParameters [index].SecondlValue = (int)WeightType.More;
					weightCalc.ApplyFlightTimeWeights (wBIdStateContent.Weights.FLTMIN.lstParameters);
				} else if (SubPopType == "Ground Time") {
					string param = lstGrdTimes [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.GRD.lstParameters [index].SecondlValue = Helper.ConvertHHMMtoMinute (param);
					weightCalc.ApplyGroundTimeWeight (wBIdStateContent.Weights.GRD.lstParameters);
				} else if (SubPopType == "Legs Per Pairing") {
					string param = arrSecondParam4 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "all")
						wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].SecondlValue = (int)LegsPerPairingType.All;
					else if (param == "more")
						wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].SecondlValue = (int)LegsPerPairingType.More;
					else if (param == "less")
						wBIdStateContent.Weights.WtLegsPerPairing.lstParameters [index].SecondlValue = (int)LegsPerPairingType.Less;
					weightCalc.ApplyLegsPerPairingWeight (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
				} else if (SubPopType == "PDO-after") {
					string param = lstPDOTimeSecValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.PDAfter.lstParameters [index].SecondlValue = Helper.ConvertHHMMtoMinute (lstPDOTimeSecValue [indexPath.Row]);
					weightCalc.ApplyPartialDaysAfterWeight (wBIdStateContent.Weights.PDAfter.lstParameters);
				} else if (SubPopType == "PDO-before") {
					string param = lstPDOTimeSecValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.PDBefore.lstParameters [index].SecondlValue = Helper.ConvertHHMMtoMinute (lstPDOTimeSecValue [indexPath.Row]);
					weightCalc.ApplyPartialDaysBeforeWeight (wBIdStateContent.Weights.PDBefore.lstParameters);
				} else if (SubPopType == "Rest") {
					string param = lstRestTimeSecValue [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					wBIdStateContent.Weights.WtRest.lstParameters [index].SecondlValue = Helper.ConvertHHMMtoMinute (lstRestTimeSecValue [indexPath.Row]);
					weightCalc.ApplyRestWeight (wBIdStateContent.Weights.WtRest.lstParameters);
				} else if (SubPopType == "Work Days") {
					string param = arrSecondParam7 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "less")
						wBIdStateContent.Weights.WorkDays.lstParameters[index].SecondlValue = (int)WeightType.Less;
					else if (param == "more")
						wBIdStateContent.Weights.WorkDays.lstParameters[index].SecondlValue = (int)WeightType.More;
					else if (param == "equal")
						wBIdStateContent.Weights.WorkDays.lstParameters[index].SecondlValue = (int)WeightType.Equal;
					weightCalc.ApplyWorkDaysWeight (wBIdStateContent.Weights.WorkDays.lstParameters);
				} else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell",new NSString (arrCommutabilty [indexPath.Row]));
					wBIdStateContent.Weights.Commute.ThirdcellValue = indexPath.Row+1;
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Weights.Commute);
				}
				else if (SubPopType == "Legs Per Duty Period") {
					string param = arrSecondParam7 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInWeightCell", new NSString (param));
					if (param == "less")
						wBIdStateContent.Weights.LEGS.lstParameters[index].SecondlValue = (int)WeightType.Less;
					else if (param == "more")
						wBIdStateContent.Weights.LEGS.lstParameters[index].SecondlValue = (int)WeightType.More;
					else if (param == "equal")
						wBIdStateContent.Weights.LEGS.lstParameters[index].SecondlValue = (int)WeightType.Equal;
					weightCalc.ApplyLegsPerDutyPeriodWeight (wBIdStateContent.Weights.LEGS.lstParameters);

				
				}

				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);


			} 
				else if (PopType == "changefourthcellparamWeightCell") {
				if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changefourthcellparamWeightCell", new NSString (lstCommutability [indexPath.Row]));
					wBIdStateContent.Weights.Commute.Value = Int32.Parse (lstCommutability [indexPath.Row]);
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Constraints.Commute);
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
			}

			else if (PopType == "changeFirstCellParamInWeightCell") {
				if (SubPopType == "PDO-after") {

					string title =newPdoDateStringList [indexPath.Row];
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInConstraintsCell", new NSString (title));
					if (title == "Any Date") 
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
						wBIdStateContent.Weights.PDAfter.lstParameters [index].FirstValue = 300;
					}else {
						DateTime date = lstPDODays[indexPath.Row-1].Date;
						title = date.ToString ("dd - MMM");
						NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
						wBIdStateContent.Weights.PDAfter.lstParameters [index].FirstValue = lstPDODays [indexPath.Row-1].DateId;
					}
					//DateTime date = lstPDODays[indexPath.Row].Date;
//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
//					string strMonthName = mfi.GetMonthName(date.Month).ToString();
//					string title = date.Day.ToString () + " - " + strMonthName;
					//string title = date.ToString ("dd - MMM");
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
					//wBIdStateContent.Weights.PDAfter.lstParameters [index].FirstValue = lstPDODays [indexPath.Row].DateId;
					weightCalc.ApplyPartialDaysAfterWeight (wBIdStateContent.Weights.PDAfter.lstParameters);
				} else if (SubPopType == "PDO-before") {


					string title =newPdoDateStringList [indexPath.Row];
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("changeSecondCellParamInConstraintsCell", new NSString (title));
					if (title == "Any Date") 
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
						wBIdStateContent.Weights.PDBefore.lstParameters [index].FirstValue = 300;
					}else {
						DateTime date = lstPDODays[indexPath.Row-1].Date;
						title = date.ToString ("dd - MMM");
						NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
						wBIdStateContent.Weights.PDBefore.lstParameters [index].FirstValue = lstPDODays [indexPath.Row-1].DateId;
					}
					//DateTime date = lstPDODays[indexPath.Row].Date;
//					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
//					string strMonthName = mfi.GetMonthName(date.Month).ToString();
//					string title = date.Day.ToString () + " - " + strMonthName;
					//string title = date.ToString ("dd - MMM");
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (title));
					//wBIdStateContent.Weights.PDBefore.lstParameters [index].FirstValue = lstPDODays [indexPath.Row].DateId;
					weightCalc.ApplyPartialDaysBeforeWeight (wBIdStateContent.Weights.PDBefore.lstParameters);
				} else if (SubPopType == "Rest") {
					string param = arrSecondParam5 [indexPath.Row];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell", new NSString (param));
					if (param == "shorter")
						wBIdStateContent.Weights.WtRest.lstParameters [index].FirstValue = (int)RestOptions.Shorter;
					else if (param == "+ & -")
						wBIdStateContent.Weights.WtRest.lstParameters [index].FirstValue = (int)RestOptions.Both;
					if (param == "longer")
						wBIdStateContent.Weights.WtRest.lstParameters [index].FirstValue = (int)RestOptions.Longer;
					weightCalc.ApplyRestWeight (wBIdStateContent.Weights.WtRest.lstParameters);
				} 
				else if (SubPopType == "Commutability") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("changeFirstCellParamInWeightCell",new NSString (arrCommtblty2 [indexPath.Row]));
					wBIdStateContent.Weights.Commute.SecondcellValue = indexPath.Row+1;
					constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Weights.Commute);
				}

				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);

			}

			else if (PopType == "NumeratorData")
			{
				ColumnDefinition obj = Numerator[indexPath.Row] as ColumnDefinition;
				if (SubPopType == "Numerator")
				{
					



					NSNotificationCenter.DefaultCenter.PostNotificationName("NumeratorSelection", new NSString(obj.DisplayName + "," + obj.DataPropertyName + "," + obj.Id));

				}
				else
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("DenominatorSelection", new NSString(obj.DisplayName + "," + obj.DataPropertyName + "," + obj.Id));
				}
			}
		}
	}
}

