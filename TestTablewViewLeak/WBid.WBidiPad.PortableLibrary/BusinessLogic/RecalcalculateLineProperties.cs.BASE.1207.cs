using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class RecalcalculateLineProperties
    {
        #region Properties
        private string _month;
        private string _year;
        private AmPmConfigure _amPmConfigure;
        private DateTime _bpStartDate;
        private DateTime _bpEndDate;
        private decimal _flyPayInLine;

        private DateTime _eomDate;
        #endregion


        public void CalcalculateLineProperties()
        {
            try
            {
                _month = GlobalSettings.CurrentBidDetails.Month.ToString();
                _year = GlobalSettings.CurrentBidDetails.Year.ToString();
                _amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
                _bpStartDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
                _bpEndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;

                if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    _eomDate = WBidCollection.GetnextSunday();
                }
                else
                {
                    _eomDate = GlobalSettings.FAEOMStartDate;
                }

                foreach (Line line in GlobalSettings.Lines)
                {

                    if (line.BlankLine)
                    {
                        line.TafbInBp = "0:00";
                        line.Equip8753 = string.Empty;
                        continue;
                    }

                    if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        line.VacPay = 0;
                        line.VacationDrop = 0;
                        line.VacationOverlapBack = 0;
                        line.VacationOverlapFront = 0;
                        line.AMPM = CalcAmPmProp(line);
                        line.AMPMSortOrder = CalcAmPmSortOrder(line);
                        line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine));
                        line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;

                        line.BlkHrsInBp = line.TempBlkHrsInBp;
                        line.Tfp = line.TempTfp;
                        line.DaysOff = line.TempDaysOff;
                        line.TafbInBp = line.TempTafbInBp;
                        line.Legs = line.TempLegs;
                        line.DaysWorkInLine = line.TempDaysWorkInLine;
                        line.TfpInLine = line.TempTfpInLine;
                        line.BlkHrsInLine = line.TempBlkHrsInLine;


                        line.TfpPerFltHr = CalcTfpPerFltHr(line);
                        line.TfpPerDay = CalculateTfpPerDay(line);
                        line.TfpPerTafb = CalcTafbTime(line);
                        line.DutyHrsInBp = CalcDutyHrs(line, false);
                        line.DutyHrsInLine = CalcDutyHrs(line, true);
                        line.TfpPerDhr = CalcTfpPerDhr(line);
                        line.LegsPerDay = CalcLegsPerDay(line);
                        //line.LegsPerPair = (line.ReserveLine || line.BlankLine) ? 0 : (line.Pairings.Count == 0) ? 0 : line.Pairings.Count == 0 ? 0 : Math.Round(Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.Pairings.Count), 2, MidpointRounding.AwayFromZero);
                        line.LegsPerPair = (line.ReserveLine || line.BlankLine) ? 0 : (line.Pairings.Count == 0) ? 0 : line.Pairings.Count == 0 ? 0 : Math.Round(Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.Pairings.Count), 2);
                        line.Weekend = CalcWkEndProp(line);
                        line.TotDutyPds = CalcTotDutPds(line, true);
                        line.TotDutyPdsInBp = CalcTotDutPds(line, false);
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line);
                        line.LongestGrndTime = CalcLongGrndTime(line);
                        line.MostLegs = CalcMostlegs(line);
                        //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                        CalcTripLength(line);
                        //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                        CalcNumLegsOfEachType(line);
                        line.Equip8753 = line.LegsIn800.ToString() + "-" + line.LegsIn700.ToString() + "-" + line.LegsIn500.ToString() + "-" + line.LegsIn300.ToString();
                        line.AcftChanges = CalcNumAcftChanges(line);
                        line.DaysWorkInLine = CalcDaysWorkInLine(line);
                        //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                        line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                        line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                        line.LastArrTime = CalcLastArrTime(line);
                        line.LastDomArrTime = CalcLastDomArrTime(line);
                        line.StartDow = CalcStartDow(line);
                        line.T234 = CalcT234(line);
                        // line.VacationDrop = 0.00m;
                        line.BlkOfDaysOff = CalcBlkOfDaysOff(line);
                        line.LegsPerDutyPeriod = CalcLegsPerDutyPeriod(line);
                        line.DaysOfWeekWork = CalcWeekDaysWork(line);
                        line.DaysOfMonthWorks = CalcDaysOfMonthOff(line);
                        CalcOvernightCities(line);
                        line.RestPeriods = CalcRestPeriods(line);
                        CalculateWorkBlockLength(line);
                        line.EPush = CalcEPush(line).ToString(@"hh\:mm");
                        line.EDomPush = CalcEDomPush(line).ToString(@"hh\:mm");
                        line.TotPairings = line.Pairings.Count();
                        CalculateWorkBlockDetails(line);
                    }
                    //if vacation  only
                    else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        if (line.VacationStateLine != null)
                        {
                            //===========================
                            int tafbInBpInt = 0;
                            line.BlkHrsInBp = line.TempBlkHrsInBp;
                            line.DaysOff = line.TempDaysOff;
                            line.FlyPay = line.TempTfp;
                            _flyPayInLine = line.TempTfpInLine;
                            line.Legs = line.TempLegs;
                            line.TafbInBp = line.TempTafbInBp;
                            line.VacPay = 0;
                            line.VacationDrop = 0;

                            line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                            line.DaysOff += line.VacationStateLine.VacationDaysOff;
                            line.FlyPay = line.VacationStateLine.FlyPay;
                            _flyPayInLine = line.VacationStateLine.FlyPayInLine;
                            line.Legs -= line.VacationStateLine.VacationLegs;
                            tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                            if (tafbInBpInt < 0) tafbInBpInt = 0;
                            line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                            line.VacPay = line.VacationStateLine.VacationTfp;
                            line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationFront)), 2);
                            line.VacationOverlapBack = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationBack)), 2); ;
                            line.TfpInLine = line.VacPay + line.TfpInLine;
                            line.Tfp = line.VacPay + line.FlyPay;
                            line.CarryOverTfp = line.TfpInLine - line.Tfp;
                            if (line.CarryOverTfp < 0 && line.CarryOverTfp > -1.0m)
                            {
                                line.CarryOverTfp = 0;
                            }

                            line.VacPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacPay)), 2);
                            line.FlyPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.FlyPay)), 2);

                            line.Tfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.Tfp)), 2);
                            line.CarryOverTfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.CarryOverTfp)), 2);

                            //======================

                            line.AMPM = CalcAmPmPropVacation(line);
                            line.AMPMSortOrder = CalcAmPmSortOrder(line);
                            line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                            line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                            line.TfpPerFltHr = CalcTfpPerFltHrVacation(line);
                            line.TfpPerDay = CalculateTfpPerDay(line);
                            line.TfpPerTafb = CalcTafbTime(line);
                            line.DutyHrsInBp = CalcDutyHrsVacation(line, false);
                            line.DutyHrsInLine = CalcDutyHrsVacation(line, true);
                            line.TfpPerDhr = CalcTfpPerDhr(line);
                            line.LegsPerDay = CalcLegsPerDay(line);
                            line.LegsPerPair = CalcLegsPerPairVacation(line);
                            line.Weekend = CalcWkEndProp(line);
                            line.TotDutyPds = CalcTotDutPdsVacation(line, true);
                            line.TotDutyPdsInBp = CalcTotDutPdsVacation(line, false);
                            line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacation(line);
                            line.LongestGrndTime = CalcLongGrndTimeVacation(line);
                            line.MostLegs = CalcMostlegsVacation(line);
                            //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                            CalcTripLengthVacation(line);
                            //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                            CalcNumLegsOfEachTypeforVacation(line);
                            line.Equip8753 = line.LegsIn800.ToString() + "-" + line.LegsIn700.ToString() + "-" + line.LegsIn500.ToString() + "-" + line.LegsIn300.ToString();
                            line.AcftChanges = CalcNumAcftChangesVacation(line);
                            line.DaysWorkInLine = CalcDaysWorkInLineVacation(line);
                            //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0.00m : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                            line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0.00m : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                            line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                            line.LastArrTime = CalcLastArrTimeVacation(line);
                            line.LastDomArrTime = CalcLastDomArrTimeVacation(line);
                            line.StartDow = CalcStartDowVacation(line);
                            line.T234 = CalcT234(line);
                            //  line.VacationDrop = 0.0m;
                            //line.VacationStateLine.VacationDropTfp;
                            line.BlkOfDaysOff = CalcBlkOfDaysOffVacation(line);
                            line.LegsPerDutyPeriod = CalcLegsPerDutyPeriodVacation(line);
                            line.DaysOfWeekWork = CalcWeekDaysWorkVacation(line);
                            line.DaysOfMonthWorks = CalcDaysOfMonthOffVacation(line);
                            CalcOvernightCitiesVacation(line);
                            line.RestPeriods = CalcRestPeriodsVacation(line);
                            CalculateWorkBlockLengthVacation(line);
                            line.EPush = CalcEPushVacation(line).ToString(@"hh\:mm");
                            line.EDomPush = CalcEDomPushVacation(line).ToString(@"hh\:mm");
                            //TotPairings-- calculated value in CalcLegsPerPairVacation() method

                            CalculateWorkBlockVacation(line);


                        }



                    }
                    //if vacation and vacation drop and 
                    else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {

                        //=========================================
                        int tafbInBpInt = 0;
                        line.BlkHrsInBp = line.TempBlkHrsInBp;
                        line.DaysOff = line.TempDaysOff;
                        line.FlyPay = line.TempTfp;
                        _flyPayInLine = line.TempTfpInLine;
                        line.Legs = line.TempLegs;
                        line.TafbInBp = line.TempTafbInBp;
                        line.VacPay = 0;
                        line.VacationDrop = 0;


                        line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                        line.DaysOff += line.VacationStateLine.VacationDaysOff;
                        line.FlyPay = line.VacationStateLine.FlyPay;
                        _flyPayInLine = line.VacationStateLine.FlyPayInLine;
                        line.Legs -= line.VacationStateLine.VacationLegs;
                        tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                        if (tafbInBpInt < 0) tafbInBpInt = 0;
                        line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                        line.VacPay = line.VacationStateLine.VacationTfp;


                        line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs));
                        line.DaysOff += line.VacationStateLine.VacationDropDaysOff;
                        line.FlyPay -= line.VacationStateLine.VacationDropTfp;
                        _flyPayInLine -= line.VacationStateLine.VacationDropTfp;
                        line.Legs -= line.VacationStateLine.VacationDropLegs;
                        tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropTafb);
                        if (tafbInBpInt < 0) tafbInBpInt = 0;
                        line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                        //line.Tfp -= line.VacationStateLine.VacationDropTfp;
                        line.TfpInLine -= line.VacationStateLine.VacationDropTfp;
                        line.VacationDrop = line.VacationStateLine.VacationDropTfp;
                        line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationFront)), 2);
                        line.VacationOverlapBack = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationBack)), 2); ;
                        line.TfpInLine = line.VacPay + _flyPayInLine;
                        line.Tfp = line.VacPay + line.FlyPay;
                        line.CarryOverTfp = line.TfpInLine - line.Tfp;
                        if (line.CarryOverTfp < 0 && line.CarryOverTfp > -1.0m)
                        {
                            line.CarryOverTfp = 0;
                        }

                        line.VacPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacPay)), 2);
                        line.FlyPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.FlyPay)), 2);

                        line.Tfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.Tfp)), 2);
                        line.CarryOverTfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.CarryOverTfp)), 2);

                        //=================================

                        line.AMPM = CalcAmPmPropDrop(line);
                        line.AMPMSortOrder = CalcAmPmSortOrder(line);
                        line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine) - (Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs) + Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs)));
                        line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                        line.TfpPerFltHr = CalcTfpPerFltHrVacation(line);
                        line.TfpPerDay = CalculateTfpPerDay(line);
                        line.TfpPerTafb = CalcTafbTime(line);
                        line.TotDutyPds = CalcTotDutPdsDrop(line, true);
                        line.TotDutyPdsInBp = CalcTotDutPdsDrop(line, false);
                        line.TfpPerDhr = CalcTfpPerDhr(line);
                        line.LegsPerDay = CalcLegsPerDay(line);
                        line.LegsPerPair = CalcLegsPerPairDrop(line);
                        line.Weekend = CalcWkEndPropDrop(line);
                        line.DutyHrsInBp = CalcDutyHrsDrop(line, false);
                        line.DutyHrsInLine = CalcDutyHrsDrop(line, true);
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffDrop(line);
                        line.LongestGrndTime = CalcLongGrndTimeVacation(line);
                        line.MostLegs = CalcMostlegsDrop(line);
                        //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                        CalcTripLengthDrop(line);
                        //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                        CalcNumLegsOfEachTypeforDrop(line);
                        line.Equip8753 = line.LegsIn800.ToString() + "-" + line.LegsIn700.ToString() + "-" + line.LegsIn500.ToString() + "-" + line.LegsIn300.ToString();
                        line.AcftChanges = CalcNumAcftChangesDrop(line);
                        line.DaysWorkInLine = CalcDaysWorkInLineDrop(line);
                        //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                        line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                        line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                        line.LastArrTime = CalcLastArrTimeDrop(line);
                        line.LastDomArrTime = CalcLastDomArrTimeDrop(line);
                        line.StartDow = CalcStartDowDrop(line);
                        line.T234 = CalcT234(line);
                        // line.VacationDrop = line.VacationStateLine.VacationDropTfp;
                        line.BlkOfDaysOff = CalcBlkOfDaysOffDrop(line);
                        line.LegsPerDutyPeriod = CalcLegsPerDutyPeriodDrop(line);
                        line.DaysOfWeekWork = CalcWeekDaysWorkDrop(line);
                        line.DaysOfMonthWorks = CalcDaysOfMonthOffDrop(line);
                        CalcOvernightCitiesDrop(line);
                        line.RestPeriods = CalcRestPeriodsDrop(line);
                        CalculateWorkBlockLengthDrop(line);
                        line.EPush = CalcEPushDrop(line).ToString(@"hh\:mm");
                        line.EDomPush = CalcEDomPushDrop(line).ToString(@"hh\:mm");
                        CalculateWorkBlockDrop(line);




                    }
                       //--------------
                    else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {


                        line.BlkHrsInBp = line.TempBlkHrsInBp;
                        line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));


                        line.FlyPay = line.TempTfp;
                        line.FlyPay = line.VacationStateLine.FlyPay;
                        decimal eomPay = CalcFlyPayEOM(line);
                        line.FlyPay -= eomPay;
                        int tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                        if (tafbInBpInt < 0) tafbInBpInt = 0;
                        line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                        //line.Legs = line.TempLegs;


                        line.VacPay = line.VacationStateLine.VacationTfp;
                        line.VacPay += eomPay;
                        line.Tfp = line.VacPay + line.FlyPay;
                        line.VacationDrop = 0;
                        line.DaysOff = line.TempDaysOff;
                        line.DaysOff += line.VacationStateLine.VacationDaysOff;
                        line.DaysOff += CalDaysOffEOM(line);

                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacationEOM(line);
                        //line.DaysOff += CalDaysOffEOMDrop(line);

                    }
                    else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        line.BlkHrsInBp = line.TempBlkHrsInBp;
                        line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                        line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs));

                        line.FlyPay = line.TempTfp;
                        line.FlyPay = line.VacationStateLine.FlyPay;
                        line.FlyPay -= line.VacationStateLine.VacationDropTfp;
                        decimal eomPay = CalcFlyPayEOM(line);
                        line.FlyPay -= eomPay;
						CalcFlyPayEOMDrop(line);
                        int tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropTafb);
                        if (tafbInBpInt < 0) tafbInBpInt = 0;
                        line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                        //line.Legs = line.TempLegs;


                        line.VacPay = line.VacationStateLine.VacationTfp;
                        line.VacPay += eomPay;
                        line.VacationDrop = 0;
                        line.DaysOff = line.TempDaysOff;
                        line.DaysOff += line.VacationStateLine.VacationDaysOff;
                        line.DaysOff += line.VacationStateLine.VacationDropDaysOff;
						line.DaysOff += CalDaysOffEOM(line);
						line.DaysOff += CalDaysOffEOMDrop(line);
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacationEOMDrop(line);
                        line.Tfp = line.VacPay + line.FlyPay;




                    }
                    else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        //=========================================
                        decimal eomPay = CalcFlyPayEOM(line);
                        decimal eomvacationtfp= CalcvACPayEOM(line);
                        line.BlkHrsInBp =line.TempBlkHrsInBp;
                        line.FlyPay = line.TempTfp - eomPay;
                        line.Legs = line.TempLegs;
                        line.TafbInBp = line.TempTafbInBp;
                        line.VacPay = eomvacationtfp;
                        line.VacationDrop = 0;
                        line.Tfp = line.VacPay + line.FlyPay;
                        line.DaysOff = line.TempDaysOff;
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line);

                        line.VacationOverlapFront =  eomvacationtfp;
                        line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationOverlapFront)), 2); 





                    }
                    else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                         decimal eomvacationtfp= CalcvACPayEOM(line);
                         decimal eomPay = CalcFlyPayEOM(line);
                         
                        line.BlkHrsInBp = line.TempBlkHrsInBp;
                        line.FlyPay = line.TempTfp - eomPay;
                        line.Legs = line.TempLegs;
                        line.TafbInBp = line.TempTafbInBp;
                        line.VacPay = eomvacationtfp; 
                        line.VacationDrop = CalcvACDropTfpEOM(line);
						CalcFlyPayEOMDrop(line);
                        line.Tfp = line.VacPay + line.FlyPay;
                        line.DaysOff = line.TempDaysOff;
                        line.DaysOff += CalDaysOffEOMDrop(line);

                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffEOMDrop(line);

                        line.VacationOverlapFront = eomvacationtfp;
                        line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationOverlapFront)), 2);
                        line.VacationDrop = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationDrop)), 2);
                    }



                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #region Private Methods
        #region Weekends
        private string CalcWkEndProp(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                tripLength = trip.PairLength;
                tripDate = DateTime.MinValue;
                dayOfWeek = 0;
                tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    // 0 = Sunday, 6 = Saturday
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }

        public string CalcWkEndPropVacation(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    tripDate = vacTrip.TripVacationStartDate;
                    tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                }
                else
                {
                    tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                    tripLength = trip.PairLength;
                }

                dayOfWeek = 0;
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }

        public string CalcWkEndPropDrop(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                    tripLength = trip.PairLength;
                }

                dayOfWeek = 0;
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }
        #endregion

        #region AMPM
        private string CalcAmPmProp(Line line)
        {
            if (line.BlankLine) return "blankLine";

            // initialize
            string ampm = "AM";
            Trip trip = null;
            int howCalc = _amPmConfigure.HowCalcAmPm;
            int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
            int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
            int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
            int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
            pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
            int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
            ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
            int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
            nteLand = nteLand + 1440;
            int amCentroid = (amPush + amLand) / 2;
            int pmCentroid = (pmPush + pmLand) / 2;
            int nteCentroid = (ntePush + nteLand) / 2;
            int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
            int numDiff = _amPmConfigure.NumOpposites;
            decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
            int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
            amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;


            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                int dpCount = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    if (line.ReserveLine == true)
                    {
                        int reservePush = dp.ReserveOut % 1440;
                        if (reservePush < GlobalSettings.ReserveAmPmClassification)
                        {
                            amTermCnt++;
                            case2AmCnt++;
                            amCentCnt++;
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "1";
                        }
                        else
                        {
                            pmTermCnt++;
                            case2PmCnt++;
                            pmCentCnt++;
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "2";
                        }
                    }
                    else
                    {
                        int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                        if (landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "1";
                            amTermCnt++;
                        }
                        else if (landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            pmTermCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            nteTermCnt++;
                        }

                        int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                        if (pushMinutes > ntePush) ntePushCnt++;
                        else if (pushMinutes > pmPush) pmPushCnt++;
                        else amPushCnt++;

                        if (pushMinutes > amPush && landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "1";
                            case2AmCnt++;

                        }
                        else if (pushMinutes > pmPush && landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2PmCnt++;
                        }
                        else if (pushMinutes > ntePush && landMinutes < nteLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2NteCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2MixCnt++;
                        }

                        int centroid = (pushMinutes + landMinutes) / 2;

                        if (centroid < amCentroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                            amCentCnt++;
                        }

                        else if (centroid < pmCentroid)
                        {
                            if (centroid - amCentroid < pmCentroid - centroid)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                                pmCentCnt++;
                            }
                        }
                        else if (centroid - pmCentroid < nteCentroid - centroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                            pmCentCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "3";
                            nteCentCnt++;
                        }


                        dpCount++;
                    }
                }
            }

            int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
            int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
            int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



            switch (howCalc)
            {
                case 1:     // AM-Terminate/PM-Arrival
                    // AM terminates before amLand and pushes before pmPush
                    // PM terminates before pmLand and pushes before ntePush
                    // NTE terminates before nteLand and pushes after ntePush
                    // Mix is none of the above

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalTerm - numDiff < amTermCnt) return "AM";
                            else if (totalTerm - numDiff < pmTermCnt) return " PM";
                            else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                            else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                            else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                            else return "Mix";
                    }


                    break;
                case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                            else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                            else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                            else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                            else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;

                case 3:     // Banded Centroid
                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCentCnt - numDiff < amCentCnt) return "AM";
                            else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                            else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                            else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                            else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;
            }

            return ampm;
        }

        private string CalcAmPmPropVacation(Line line)
        {


            try
            {


                if (line.BlankLine) return "---";

                // initialize
                string ampm = "AM";
                Trip trip = null;
                int howCalc = _amPmConfigure.HowCalcAmPm;
                int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
                int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
                int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
                int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
                pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
                int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
                ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
                int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
                nteLand = nteLand + 1440;
                int amCentroid = (amPush + amLand) / 2;
                int pmCentroid = (pmPush + pmLand) / 2;
                int nteCentroid = (ntePush + nteLand) / 2;
                int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
                int numDiff = _amPmConfigure.NumOpposites;
                decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
                int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
                amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;
                int tripDay, tripLength;
                DateTime tripDate = DateTime.MinValue;

                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {

                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }

                        tripDate = vacTrip.TripVacationStartDate;
                        tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                    }
                    else
                    {
                        tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                        //tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), tripDay);
                        tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                        tripLength = trip.PairLength;
                    }


                    int dpCount = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (vacTrip != null)
                        {
                            if (vacTrip.VacationDutyPeriods[dpCount].DutyPeriodType == "VO" || vacTrip.VacationDutyPeriods[dpCount].DutyPeriodType == "VA")
                            {
                                dpCount++;
                                continue;
                            }
                        }

                        if (line.ReserveLine == true)
                        {
                            int reservePush = dp.ReserveOut % 1440;
                            if (reservePush < GlobalSettings.ReserveAmPmClassification)
                            {
                                amTermCnt++;
                                case2AmCnt++;
                                amCentCnt++;
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "1";
                            }
                            else
                            {
                                pmTermCnt++;
                                case2PmCnt++;
                                pmCentCnt++;
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "2";
                            }
                        }
                        else
                        {
                            int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                            if (landMinutes < amLand)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "1";
                                amTermCnt++;
                            }
                            else if (landMinutes < pmLand)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                                pmTermCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                                nteTermCnt++;
                            }

                            int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                            if (pushMinutes > ntePush) ntePushCnt++;
                            else if (pushMinutes > pmPush) pmPushCnt++;
                            else amPushCnt++;

                            if (pushMinutes > amPush && landMinutes < amLand)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "1";
                                case2AmCnt++;
                            }
                            else if (pushMinutes > pmPush && landMinutes < pmLand)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                                case2PmCnt++;
                            }
                            else if (pushMinutes > ntePush && landMinutes < nteLand)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                                case2NteCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                                case2MixCnt++;
                            }

                            int centroid = (pushMinutes + landMinutes) / 2;

                            if (centroid < amCentroid)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else if (centroid < pmCentroid)
                            {
                                if (centroid - amCentroid < pmCentroid - centroid)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                                    amCentCnt++;
                                }
                                else
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                                    pmCentCnt++;
                                }
                            }
                            else if (centroid - pmCentroid < nteCentroid - centroid)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                                pmCentCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "3";
                                nteCentCnt++;
                            }

                            dpCount++;
                        }
                    }
                }

                int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
                int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
                int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



                switch (howCalc)
                {
                    case 1:     // AM-Terminate/PM-Arrival
                        // AM terminates before amLand and pushes before pmPush
                        // PM terminates before pmLand and pushes before ntePush
                        // NTE terminates before nteLand and pushes after ntePush
                        // Mix is none of the above

                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalTerm - numDiff < amTermCnt) return "AM";
                                else if (totalTerm - numDiff < pmTermCnt) return " PM";
                                else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalTerm == 0) return "AM"; ;
                                if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                                else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                                else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                                else return "Mix";
                        }


                        break;
                    case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                                else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                                else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalCase2Cnt == 0) return "AM"; ;
                                if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                                else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                                else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                                else return "Mix";
                        }
                        break;

                    case 3:     // Banded Centroid
                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalCentCnt - numDiff < amCentCnt) return "AM";
                                else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                                else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalCentCnt == 0) return "AM"; ;
                                if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                                else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                                else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                                else return "Mix";
                        }
                        break;
                }

                return ampm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string CalcAmPmPropDrop(Line line)
        {
            if (line.BlankLine) return "---";

            // initialize
            string ampm = "AM";
            Trip trip = null;
            int howCalc = _amPmConfigure.HowCalcAmPm;
            int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
            int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
            int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
            int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
            pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
            int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
            ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
            int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
            nteLand = nteLand + 1440;
            int amCentroid = (amPush + amLand) / 2;
            int pmCentroid = (pmPush + pmLand) / 2;
            int nteCentroid = (ntePush + nteLand) / 2;
            int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
            int numDiff = _amPmConfigure.NumOpposites;
            decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
            int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
            amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;


            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }

                int dpCount = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    if (line.ReserveLine == true)
                    {
                        int reservePush = dp.ReserveOut % 1440;
                        if (reservePush < GlobalSettings.ReserveAmPmClassification)
                        {
                            amTermCnt++;
                            case2AmCnt++;
                            amCentCnt++;
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "1";
                        }
                        else
                        {
                            pmTermCnt++;
                            case2PmCnt++;
                            pmCentCnt++;
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "2";
                        }
                    }
                    else
                    {
                        int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                        if (landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "1";
                            amTermCnt++;
                        }
                        else if (landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            pmTermCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            nteTermCnt++;
                        }

                        int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                        if (pushMinutes > ntePush) ntePushCnt++;
                        else if (pushMinutes > pmPush) pmPushCnt++;
                        else amPushCnt++;

                        if (pushMinutes > amPush && landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "1";
                            case2AmCnt++;
                        }
                        else if (pushMinutes > pmPush && landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2PmCnt++;
                        }
                        else if (pushMinutes > ntePush && landMinutes < nteLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2NteCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2MixCnt++;
                        }

                        int centroid = (pushMinutes + landMinutes) / 2;

                        if (centroid < amCentroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                            amCentCnt++;
                        }
                        else if (centroid < pmCentroid)
                        {
                            if (centroid - amCentroid < pmCentroid - centroid)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                                pmCentCnt++;
                            }
                        }
                        else if (centroid - pmCentroid < nteCentroid - centroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                            pmCentCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "3";
                            nteCentCnt++;
                        }

                        dpCount++;
                    }
                }
            }

            int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
            int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
            int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



            switch (howCalc)
            {
                case 1:     // AM-Terminate/PM-Arrival
                    // AM terminates before amLand and pushes before pmPush
                    // PM terminates before pmLand and pushes before ntePush
                    // NTE terminates before nteLand and pushes after ntePush
                    // Mix is none of the above

                    switch (numOrPct)
                    {
                        case 1:         // number of differences

                            if (totalTerm - numDiff < amTermCnt) return "AM";
                            else if (totalTerm - numDiff < pmTermCnt) return " PM";
                            else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalTerm == 0) return "AM"; ;
                            if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                            else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                            else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                            else return "Mix";
                    }


                    break;
                case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                            else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                            else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalCase2Cnt == 0) return "AM"; ;
                            if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                            else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                            else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;

                case 3:     // Banded Centroid
                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCentCnt - numDiff < amCentCnt) return "AM";
                            else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                            else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalCentCnt == 0) return "AM"; ;
                            if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                            else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                            else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;
            }

            return ampm;
        }
        #endregion

        #region FltHr
        private decimal CalcTfpPerFltHr(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;

            decimal blkHours = 0.0m;
            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            if (line.Block.ToString().Length == 5)
                blkHours = (Convert.ToDecimal(line.Block.ToString().Substring(0, 3)) +
                  Convert.ToDecimal(line.Block.ToString().Substring(3, 2)) / 60m);
            else
                blkHours = (Convert.ToDecimal(line.Block.ToString().Substring(0, 2)) +
                Convert.ToDecimal(line.Block.ToString().Substring(2, 2)) / 60m);

            tfp = (blkHours == 0) ? 0.00m : line.Tfp / blkHours;

            return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
            // return Math.Round(1200m / 100, 2);


        }

        private decimal CalcTfpPerFltHrVacation(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;


            string blockHoursInBp = line.BlkHrsInBp.ToString().Replace(":", "");
            decimal blkHours = 0.0m;
            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            try
            {
                if (blockHoursInBp.Length == 5)

                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 3)) + Convert.ToDecimal(blockHoursInBp.Substring(3, 2)) / 60m);
                else
                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 2)) + Convert.ToDecimal(blockHoursInBp.Substring(2, 2)) / 60m);

                tfp = (blkHours == 0) ? 0.00m : line.Tfp / blkHours;

                return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            // return Math.Round(1200m / 100, 2);


        }
        #endregion

        #region TfpPerDay
        private decimal CalculateTfpPerDay(Line line)
        {

            decimal tfpPerDay = 0.00m;

            if (line.ReserveLine)
            {

                tfpPerDay = (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAReserveDayPay : GlobalSettings.ReserveDailyGuarantee;
            }
            else if (line.BlankLine)
            {
                tfpPerDay = 0.00m;
            }
            else
            {
                tfpPerDay = Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", line.DaysWork == 0 ? 0.00m : (line.Tfp / line.DaysWork))), 2);
            }

            return tfpPerDay;
            //<0?0:tfpPerDay;
        }
        #endregion

        #region DutPdsCount
        private int CalcTotDutPds(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                if (inLine)
                {
                    totDutPds += trip.DutyPeriods.Count();
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (tripDate <= _bpEndDate)
                        {
                            totDutPds += 1;

                        }
                        tripDate = tripDate.AddDays(1); ;

                    }
                }
            }



            return totDutPds;
        }

        private int CalcTotDutPdsVacation(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;
            int dutyperiodCount = 0;

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    tripDate = vacTrip.TripVacationStartDate;
                    dutyperiodCount = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                    if (inLine)
                    {
                        totDutPds += dutyperiodCount;
                    }
                    else
                    {
                        if (tripDate.AddDays(dutyperiodCount - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;

                        }
                        else
                        {
                            totDutPds += dutyperiodCount;
                        }

                    }
                }

                else
                {
                    if (inLine)
                    {
                        totDutPds += trip.DutyPeriods.Count();
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                        if (tripDate.AddDays(trip.PairLength - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;
                        }
                        else
                        {
                            totDutPds += trip.DutyPeriods.Count();
                        }

                    }
                }
            }



            return totDutPds;
        }

        private int CalcTotDutPdsDrop(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;

            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    if (inLine)
                    {
                        totDutPds += trip.DutyPeriods.Count();
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        if (tripDate.AddDays(trip.PairLength - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;

                        }
                        else
                        {
                            totDutPds += trip.DutyPeriods.Count();
                        }

                    }
                }
            }

            return totDutPds;
        }
        #endregion

        #region TafbTime
        private decimal CalcTafbTime(Line line)
        {

            decimal tfpPerTafb = 0.0m;
            //TfpPerTafb
            if (!line.ReserveLine && !line.BlankLine)
            {
                string[] tafbTime = line.TafbInBp.Split(':');
                decimal tafbInMin = Convert.ToDecimal(tafbTime[0]) * 60 + Convert.ToDecimal(tafbTime[1]);
                tfpPerTafb = line.ReserveLine || line.BlankLine ? 0m : (tafbInMin == 0) ? 0 : Math.Round(line.Tfp / (tafbInMin / 60m), 2);
            }

            return tfpPerTafb;

        }
        #endregion

        #region  TFP in Bp
        /// <summary>
        /// PURPOSE : Calculate Overnight Cities
        /// </summary>
        /// <param name="line"></param>
        private decimal CalcTfpInBP(Line line)
        {
            // frank add
            decimal tfp = line.Tfp;

            foreach (var s in line.LineSips)
            {
                if (s.Dropped)
                {
                    tfp -= s.SipTfp;
                }
            }
            return Math.Round(decimal.Parse(String.Format("{0:0.00}", tfp)), 2); ;
        }
        #endregion

        #region DutyHrs
        private string CalcDutyHrs(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                foreach (var dp in trip.DutyPeriods)
                {
                    if (inLine)
                    {
                        dutyHrs += dp.DutyTime;

                    }
                    else if (tripDate <= _bpEndDate)
                    {
                        dutyHrs += dp.DutyTime;

                    }
                    tripDate = tripDate.AddDays(1); ;

                }
            }

            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');

        }

        private string CalcDutyHrsVacation(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            int dpIndex = 0;
            int flightIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    dpIndex = 0;
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (tripDate <= _bpEndDate)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                dutyHrs += dp.DutyTime;
                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        dutyHrs += flt.Block;

                                    }

                                    flightIndex++;
                                }

                            }

                        }
                        dpIndex++;
                        tripDate = tripDate.AddDays(1);
                    }
                }

                else
                {
                    if (inLine)
                    {
                        dutyHrs += trip.DutyPeriods.Sum(x => x.DutyTime);
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        dpIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (tripDate <= _bpEndDate)
                            {
                                dutyHrs += dp.DutyTime;
                            }

                            dpIndex++;
                            tripDate = tripDate.AddDays(1);
                        }


                    }
                }
            }

            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');
        }

        private string CalcDutyHrsDrop(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            int dpIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    dpIndex = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (inLine)
                        {
                            dutyHrs += dp.DutyTime;

                        }
                        else if (tripDate <= _bpEndDate)
                        {
                            dutyHrs += dp.DutyTime;

                        }
                        tripDate = tripDate.AddDays(1); ;

                    }
                    dpIndex++;


                }
            }


            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');
        }

        #endregion

        #region TfpPerDhr
        private decimal CalcTfpPerDhr(Line line)
        {

            decimal tfpPerDhr = 0.0m;
            //TfpPerDhr
            string[] dhrTime = line.DutyHrsInBp.Split(':');
            decimal dhrInMin = Convert.ToDecimal(dhrTime[0]) * 60 + Convert.ToDecimal(dhrTime[1]);
            tfpPerDhr = line.ReserveLine || line.BlankLine ? 0m : dhrInMin == 0 ? 0 : Math.Round(line.Tfp / (dhrInMin / 60m), 2);
            return tfpPerDhr;
        }

        #endregion

        #region LegsPerDay
        private decimal CalcLegsPerDay(Line line)
        {
            decimal legsPerDay = 0.0m;
            // line.LegsPerDay
            //legsPerDay = line.ReserveLine || line.BlankLine ? 0.00m : line.DaysWork == 0 ? 0.00m : Math.Round(decimal.Parse(String.Format("{0:0.00}", Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork))), 2, MidpointRounding.AwayFromZero); ;
            legsPerDay = line.ReserveLine || line.BlankLine ? 0.00m : line.DaysWork == 0 ? 0.00m : Math.Round(decimal.Parse(String.Format("{0:0.00}", Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork))), 2); ;

            return legsPerDay;
        }

        #endregion

        #region LargestBlkDaysOff
        private int CalcLargestBlkDaysOff(Line line)
        {
            int largestDaysOff = 0;
            int tripOff = 0;


            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            Trip trip = null;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                if (tripDate > oldTripdate)
                {
                    tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                    if (tripOff > largestDaysOff)
                    {
                        largestDaysOff = tripOff;

                    }

                }

                oldTripdate = tripDate.AddDays(trip.PairLength - 1);

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.Subtract(oldTripdate).Days < 0) ? 0 : _bpEndDate.Subtract(oldTripdate).Days;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }
            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffVacation(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (vacTrip.TripType == "VOB")
                    {
                        tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
                    }
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);
                }

                else
                {
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;
            DateTime tripDate = DateTime.MinValue;
            DateTime oldTripdate = _bpStartDate.AddDays(-1);

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }
                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }
            }

            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }
            }

            return largestDaysOff;


        }


        private int CalcLargestBlkDaysOffEOMDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;
            DateTime tripDate = DateTime.MinValue;
            DateTime oldTripdate = _bpStartDate.AddDays(-1);

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                if (tripEndDate >= _eomDate)
                {
                    VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                    DateTime date = tripStartDate;

                    if (vacationTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }

                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }
                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }
            }

            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }
            }

            return largestDaysOff;


        }


        private int CalcLargestBlkDaysOffVacationEOM(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            VacationTrip vacationTrip = null;
            foreach (var pairing in line.Pairings)
            {
                vacationTrip = null;
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (vacTrip.TripType == "VOB")
                    {
                        tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
                    }


                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                }

                else
                {
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        if (GlobalSettings.VacationData.Keys.Contains(pairing))
                        {
                            vacationTrip = GlobalSettings.VacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
                            if (vacationTrip != null)
                            {
                                tripDate = _bpEndDate.AddDays(1);
                            }
                        }
                    }



                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);

                }


            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }


        //private int CalcLargestBlkDaysOffVacationEOM(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }
        //            if (vacTrip.TripType == "VOB")
        //            {
        //                tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
        //            }


        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }


        //            //oldTripdate = tripDate.AddDays(trip.PairLength - 1);
        //            oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = tripDate;
        //                        //_bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        private int CalcLargestBlkDaysOffVacationEOMDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            VacationTrip vacationTrip = null;
            foreach (var pairing in line.Pairings)
            {
                vacationTrip = null;
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                }

                else
                {
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        if (GlobalSettings.VacationData.Keys.Contains(pairing))
                        {
                            vacationTrip = GlobalSettings.VacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
                            if (vacationTrip != null)
                            {
                                tripDate = _bpEndDate.AddDays(1);
                            }
                        }
                    }



                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);

                }


            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffEOM(Line line)
        {

            int largestDaysOff = 0;
            int tripOff = 0;


            DateTime oldTripdate = _bpStartDate.AddDays(-1);
        
            Trip trip = null;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);

                if (tripEndDate >= _eomDate)
                {
                    VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                    DateTime date = tripStartDate;
                    if (vacationTrip != null)
                    {
                        continue;
                    }
                }




                if (tripStartDate > oldTripdate)
                {
                    tripOff = (tripStartDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripStartDate.Subtract(oldTripdate).Days - 1;
                    if (tripOff > largestDaysOff)
                    {
                        largestDaysOff = tripOff;

                    }

                }

                oldTripdate = tripStartDate.AddDays(trip.PairLength - 1);

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.Subtract(oldTripdate).Days < 0) ? 0 : _bpEndDate.Subtract(oldTripdate).Days;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }
            return largestDaysOff;
            
            
            
            
            
            
            
            
       
        }


        #endregion

        #region LongGrndTime
        private TimeSpan CalcLongGrndTime(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                // string tripName = pairing.Substring(0, 4);
                foreach (var dp in trip.DutyPeriods)
                {
                    int lastLandTime = 0;
                    foreach (var flt in dp.Flights)
                    {
                        if (lastLandTime != 0)
                        {
                            turnTime = flt.DepTime - lastLandTime;
                            maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                        }
                        lastLandTime = flt.ArrTime;
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }

        private TimeSpan CalcLongGrndTimeVacation(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            int dpId = 0;
            int flightId = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);




                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            int lastLandTime = 0;
                            dpId = 0;
                            if (vacTrip.VacationDutyPeriods[dpId].DutyPeriodType == "VO" || vacTrip.VacationDutyPeriods[dpId].DutyPeriodType == "VA")
                            {
                                dpId++;
                                continue;
                            }
                            flightId = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (vacTrip.VacationDutyPeriods[dpId].VacationFlights[flightId].FlightType == "VO" || vacTrip.VacationDutyPeriods[dpId].VacationFlights[flightId].FlightType == "VA")
                                {
                                    flightId++;
                                    continue;
                                }

                                if (lastLandTime != 0)
                                {
                                    turnTime = flt.DepTime - lastLandTime;
                                    maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                                }
                                lastLandTime = flt.ArrTime;
                            }
                            dpId++;
                        }

                    }

                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastLandTime = 0;
                        foreach (var flt in dp.Flights)
                        {
                            if (lastLandTime != 0)
                            {
                                turnTime = flt.DepTime - lastLandTime;
                                maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                            }
                            lastLandTime = flt.ArrTime;
                        }
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }

        private TimeSpan CalcLongGrndTimeDrop(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastLandTime = 0;
                        foreach (var flt in dp.Flights)
                        {
                            if (lastLandTime != 0)
                            {
                                turnTime = flt.DepTime - lastLandTime;
                                maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                            }
                            lastLandTime = flt.ArrTime;
                        }
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }
        #endregion

        #region Mostlegs
        private int CalcMostlegs(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;


            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                int legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                if (mostlegs < legsintrip)
                {
                    mostlegs = legsintrip;
                }
            }
            return mostlegs;
        }

        private int CalcMostlegsVacation(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;
            int legsintrip = 0;

            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        legsintrip = vacTrip.VacationDutyPeriods.SelectMany(x => x.VacationFlights.Where(y => y.FlightType == "VD")).Count();
                        if (mostlegs < legsintrip)
                        {
                            mostlegs = legsintrip;
                        }
                    }
                }
                else
                {


                    legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
            }
            return mostlegs;
        }

        private int CalcMostlegsDrop(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;


            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {

                    int legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
            }
            return mostlegs;
        }
        #endregion

        #region TripLength
        private void CalcTripLength(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                tripLength = trip.PairLength;

                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;



                }
            }


        }

        private void CalcTripLengthVacation(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();

                    }
                }
                else
                {
                    tripLength = trip.PairLength;
                }

                tripLength = trip.PairLength;
                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;

                }
            }


        }


        private void CalcTripLengthDrop(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripLength = trip.PairLength;
                }

                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;



                }
            }


        }
        #endregion

        #region NumLegs
        private void CalcNumLegsOfEachType(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            line.LegsIn500 = 0;
            line.LegsIn300 = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                string tripName = pairing.Substring(0, 4);
                // if (tripName.Substring(1, 1) != "W" && tripName.Substring(1, 1) != "Y")     // does not look inside reserver trip
                if (!trip.ReserveTrip)
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.Equip != null)
                            {

                                switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                {
                                    case "8":
                                        line.LegsIn800++;
                                        break;
                                    case "7":
                                        line.LegsIn700++;
                                        break;
                                    case "5":
                                        line.LegsIn500++;
                                        break;
                                    case "3":
                                        line.LegsIn300++;
                                        break;


                                }

                            }
                        }

                    }
                }
            }


        }

        private void CalcNumLegsOfEachTypeforVacation(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            line.LegsIn500 = 0;
            line.LegsIn300 = 0;
            int dpIndex = 0;
            int flightIndex = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                string tripName = pairing.Substring(0, 4);
                if (!trip.ReserveTrip)
                // does not look inside reserver trip
                {

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }

                    //Vacation trip
                    if (vacTrip != null)
                    {
                        //vacation trip with out VD
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            dpIndex = 0;
                            foreach (var dp in trip.DutyPeriods)
                            {
                                if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VO")
                                {
                                    dpIndex++;
                                    continue;
                                }

                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VO")
                                    {
                                        flightIndex++;
                                        continue;
                                    }

                                    if (flt.Equip != null)
                                    {

                                        switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                        {
                                            case "8":
                                                line.LegsIn800++;
                                                break;
                                            case "7":
                                                line.LegsIn700++;
                                                break;
                                            case "5":
                                                line.LegsIn500++;
                                                break;
                                            case "3":
                                                line.LegsIn300++;
                                                break;


                                        }

                                    }
                                    flightIndex++;
                                }
                                dpIndex++;
                            }


                        }
                    }
                    else
                    {  //Normal trip
                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                if (flt.Equip != null)
                                {

                                    switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                    {
                                        case "8":
                                            line.LegsIn800++;
                                            break;
                                        case "7":
                                            line.LegsIn700++;
                                            break;
                                        case "5":
                                            line.LegsIn500++;
                                            break;
                                        case "3":
                                            line.LegsIn300++;
                                            break;


                                    }

                                }
                            }

                        }

                    }

                }
            }


        }

        private void CalcNumLegsOfEachTypeforDrop(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            line.LegsIn500 = 0;
            line.LegsIn300 = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                string tripName = pairing.Substring(0, 4);
                if (!trip.ReserveTrip)
                // if (tripName.Substring(1, 1) != "W" && tripName.Substring(1, 1) != "Y")     // does not look inside reserver trip
                {

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {


                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                if (flt.Equip != null)
                                {

                                    switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                    {
                                        case "8":
                                            line.LegsIn800++;
                                            break;
                                        case "7":
                                            line.LegsIn700++;
                                            break;
                                        case "5":
                                            line.LegsIn500++;
                                            break;
                                        case "3":
                                            line.LegsIn300++;
                                            break;


                                    }

                                }
                            }

                        }
                    }
                }
            }


        }


        #endregion

        #region AcftChanges
        private int CalcNumAcftChanges(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                foreach (var dp in trip.DutyPeriods)
                {
                    foreach (var flt in dp.Flights)
                    {
                        if (flt.AcftChange == true)
                            numAcftChanges++;
                    }
                }
            }

            return numAcftChanges;
        }

        private int CalcNumAcftChangesVacation(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;
            int dpIndex = 0;
            int flightIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                //Get trip
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        dpIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VO")
                            {
                                dpIndex++;
                                continue;
                            }

                            flightIndex = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VO")
                                {
                                    flightIndex++;
                                    continue;
                                }
                                if (flt.AcftChange)
                                    numAcftChanges++;
                                flightIndex++;
                            }
                            dpIndex++;
                        }

                    }
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.AcftChange == true)
                                numAcftChanges++;
                        }
                    }
                }
            }

            return numAcftChanges;
        }

        private int CalcNumAcftChangesDrop(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;

            foreach (var pairing in line.Pairings)
            {
                ///Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.AcftChange == true)
                                numAcftChanges++;
                        }
                    }
                }
            }

            return numAcftChanges;
        }


        #endregion

        #region DaysWorkInLine
        private int CalcDaysWorkInLine(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                daysWorkInLine += trip.PairLength;
            }

            return daysWorkInLine;
        }

        private int CalcDaysWorkInLineVacation(Line line)
        {

            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    daysWorkInLine += vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();

                }
                else
                {
                    daysWorkInLine += trip.PairLength;
                }




            }

            return daysWorkInLine;
        }

        private int CalcDaysWorkInLineDrop(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    daysWorkInLine += trip.PairLength;
                }
            }

            return daysWorkInLine;
        }

        #endregion

        #region LastArrTime
        private TimeSpan CalcLastArrTime(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                dutPd = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    int lastFlt = dp.Flights.Count - 1;
                    arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                    dutPd++;
                }
            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }

        private TimeSpan CalcLastArrTimeVacation(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            int fltIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dutPd = 0;
                        int tempArrTime = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "Split")
                            {
                                fltIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "VD")
                                    {
                                        tempArrTime = dp.Flights[fltIndex].ArrTime - 1440 * dutPd;
                                    }
                                    fltIndex++;
                                }
                                arrTime = (tempArrTime > arrTime) ? tempArrTime : arrTime;

                            }

                            dutPd++;
                        }
                    }

                }
                else
                {
                    dutPd = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastFlt = dp.Flights.Count - 1;
                        arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                        dutPd++;
                    }
                }




            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }


        private TimeSpan CalcLastArrTimeDrop(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    dutPd = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastFlt = dp.Flights.Count - 1;
                        arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                        dutPd++;
                    }
                }
            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }
        #endregion

        #region LastDomArrTime
        private TimeSpan CalcLastDomArrTime(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                int lastDp = trip.DutyPeriods.Count - 1;
                int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            return new TimeSpan(hours % 24, minutes, 0);
        }

        private TimeSpan CalcLastDomArrTimeVacation(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            int tempDomArr = 0;
            int dpIndex = 0;
            int flightIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        dpIndex = 0;

                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        tempDomArr = trip.DutyPeriods[dpIndex].Flights[flightIndex].ArrTime - dpIndex * 1440;
                                    }
                                    flightIndex++;
                                }
                            }


                            dpIndex++;
                        }

                        lastDomArr = (tempDomArr > lastDomArr) ? tempDomArr : lastDomArr;


                    }
                }
                else
                {


                    int lastDp = trip.DutyPeriods.Count - 1;
                    int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                    lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                    trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
                }
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            return new TimeSpan(hours % 24, minutes, 0);
        }

        private TimeSpan CalcLastDomArrTimeDrop(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    int lastDp = trip.DutyPeriods.Count - 1;
                    int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                    lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                    trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
                }
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            return new TimeSpan(hours % 24, minutes, 0);
        }


        #endregion

        #region LegsPerPair

        private decimal CalcLegsPerPairVacation(Line line)
        {

            decimal legsPerPair = 0.0m;
            Trip trip = null;

            int pairingCount = line.Pairings.Count;
            if (!line.ReserveLine && !line.BlankLine)
            {

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }


                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            pairingCount--;
                        }

                    }
                }


                // legsPerPair = pairingCount == 0 ? 0.00m : Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2, MidpointRounding.AwayFromZero);
                legsPerPair = pairingCount == 0 ? 0.00m : Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2);
                line.TotPairings = pairingCount;


            }

            return legsPerPair;


        }


        private decimal CalcLegsPerPairDrop(Line line)
        {

            decimal legsPerPair = 0.0m;
            Trip trip = null;

            int pairingCount = line.Pairings.Count;
            if (!line.ReserveLine && !line.BlankLine)
            {

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        pairingCount--;
                    }
                }


                //                legsPerPair = Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2, MidpointRounding.AwayFromZero);
                legsPerPair = Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2);
                line.TotPairings = pairingCount;


            }

            return legsPerPair;


        }

        #endregion

        #region StartDow

        private string CalcStartDow(Line line)
        {
            Trip trip = null;
            string sdow = " "; ;
            int startDowInt = 0;
            int oldDowInt = 9;
            int nextDate = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                int lenghtOfTrip = trip.PairLength;
                if (date != nextDate)
                {
                    nextDate = date + lenghtOfTrip;
                    startDowInt = Convert.ToInt32(WBidCollection.SetDate(date, isLastTrip).DayOfWeek);

                    //startDowInt = Convert.ToInt32(new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), date).DayOfWeek);
                    oldDowInt = oldDowInt == 9 ? startDowInt : oldDowInt;
                }
                else nextDate = date + lenghtOfTrip;

                if (startDowInt != oldDowInt)
                {
                    return "mix";
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        private string CalcStartDowVacation(Line line)
        {
            Trip trip = null;
            VacationStateTrip vacTrip = null;
            DateTime oldTripEndDate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            int lenghtOfTrip = 0;
            int dpIndex = 0;
            int paringCount = 0;
            int startDowInt = 0;
            int oldDowInt = 9;
            bool isLastTrip = false;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                vacTrip = null;

                //Check lines having vacation trips
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    // VA trip
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dpIndex = 0;
                        tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        foreach (var dp in trip.DutyPeriods)
                        {

                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                tripStartDate = tripStartDate.AddDays(dpIndex);
                                lenghtOfTrip = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                                break;
                            }

                            dpIndex++;

                        }
                    }
                }
                else
                {
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                    lenghtOfTrip = trip.PairLength;
                }


                if (tripStartDate != oldTripEndDate.AddDays(1))
                {

                    startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                    oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;
                }

                oldTripEndDate = tripStartDate.AddDays(lenghtOfTrip - 1);

                if (startDowInt != oldDowInt)
                {
                    return "mix";
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        private string CalcStartDowDrop(Line line)
        {
            Trip trip = null;

            int startDowInt = 0;
            int oldDowInt = 9;

            DateTime oldTripEndDate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {


                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                    int lenghtOfTrip = trip.PairLength;
                    if (tripStartDate != oldTripEndDate.AddDays(1))
                    {

                        startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                        oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;
                    }

                    oldTripEndDate = tripStartDate.AddDays(lenghtOfTrip - 1);

                    if (startDowInt != oldDowInt)
                    {
                        return "mix";
                    }
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        #endregion

        #region T234


        private string CalcT234(Line line)
        {

            string T234 = string.Empty;

            T234 = line.Trips1Day > 9 ? "*" : line.Trips1Day.ToString();
            T234 += line.Trips2Day > 9 ? "*" : line.Trips2Day.ToString();
            T234 += line.Trips3Day > 9 ? "*" : line.Trips3Day.ToString();
            T234 += line.Trips4Day > 9 ? "*" : line.Trips4Day.ToString();
            return T234;
        }

        #endregion

        #region BlkOfDaysOff
        private List<int> CalcBlkOfDaysOff(Line line)
        {
            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            Trip trip = null;
            if (line.BlankLine) return blkOff;

            DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            int daysOff = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //Get trip
                trip = GetTrip(pairing);
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                // tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                if (daysOff > 0)
                {
                    blkOff[daysOff]++;
                }
                oldPairingEndDate = tripDate.AddDays(trip.PairLength - 1);

            }

            if (oldPairingEndDate < _bpEndDate)
            {
                daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                blkOff[daysOff]++;
            }

            return blkOff;


        }

        private List<int> CalcBlkOfDaysOffVacation(Line line)
        {


            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            try
            {
                Trip trip = null;
                if (line.BlankLine) return blkOff;
                bool isLastTrip = false; int paringCount = 0;
                DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
                DateTime tripDate = DateTime.MinValue;
                int daysOff = 0;
                int pairLength = 0;
                foreach (string pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }


                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            tripDate = vacTrip.TripVacationStartDate;
                            pairLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count(); ;
                        }
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        // tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        pairLength = trip.PairLength;
                    }



                    daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                    if (daysOff > 0)
                    {
                        blkOff[daysOff]++;
                    }
                    oldPairingEndDate = tripDate.AddDays(pairLength - 1);

                }

                if (oldPairingEndDate < _bpEndDate)
                {
                    daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                    blkOff[daysOff]++;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return blkOff;


        }

        private List<int> CalcBlkOfDaysOffDrop(Line line)
        {
            //int[] blkOff = new int[20];
            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            Trip trip = null;
            if (line.BlankLine) return blkOff;

            DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            int daysOff = 0;
            try
            {
                bool isLastTrip = false; int paringCount = 0;

                foreach (string pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);

                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        //tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                        if (daysOff > 0)
                        {
                            blkOff[daysOff]++;
                        }
                        oldPairingEndDate = tripDate.AddDays(trip.PairLength - 1);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (oldPairingEndDate < _bpEndDate)
            {
                daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                blkOff[daysOff]++;
            }

            return blkOff;


        }

        #endregion

        #region LegsPerDutyPeriod
        private List<int> CalcLegsPerDutyPeriod(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                {
                    arrayOfDeadheads[dutyPeriod.TotFlights]++;
                }

            }

            return arrayOfDeadheads;
        }

        private List<int> CalcLegsPerDutyPeriodVacation(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            int dpIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dpIndex = 0;
                        foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                arrayOfDeadheads[dutyPeriod.TotFlights]++;
                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                arrayOfDeadheads[vacTrip.VacationDutyPeriods[dpIndex].VacationFlights.Where(x => x.FlightType == "VD").Count()]++;
                            }
                            dpIndex++;
                        }
                    }
                }
                else
                {
                    foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                    {
                        arrayOfDeadheads[dutyPeriod.TotFlights]++;
                    }
                }



            }

            return arrayOfDeadheads;
        }

        private List<int> CalcLegsPerDutyPeriodDrop(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                    {
                        arrayOfDeadheads[dutyPeriod.TotFlights]++;
                    }
                }

            }

            return arrayOfDeadheads;
        }


        #endregion

        #region WeekDaysWork

        private List<int> CalcWeekDaysWork(Line line)
        {
            Trip trip = null;
            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime wkDay;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                int lengthTrip = trip.PairLength;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip);
                dayOfWeek = (int)wkDay.DayOfWeek - 1;
                dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                weekWorkingDays[dayOfWeek]++;

                for (int count = 1; count < lengthTrip; count++)
                {
                    wkDay = wkDay.AddDays(1);
                    dayOfWeek = (int)wkDay.DayOfWeek - 1;
                    dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                    weekWorkingDays[dayOfWeek]++;
                }
            }
            return weekWorkingDays;
        }

        private List<int> CalcWeekDaysWorkVacation(Line line)
        {
            Trip trip = null;

            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime tripDate;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                dayOfWeek = (int)tripDate.DayOfWeek - 1;
                                dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                                weekWorkingDays[dayOfWeek]++;
                            }

                            tripDate = tripDate.AddDays(1);
                        }

                    }
                }
                else
                {

                    for (int count = 0; count < trip.PairLength; count++)
                    {
                        dayOfWeek = (int)tripDate.DayOfWeek - 1;
                        dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                        weekWorkingDays[dayOfWeek]++;
                        tripDate = tripDate.AddDays(1);
                    }

                }



            }

            return weekWorkingDays;
        }

        private List<int> CalcWeekDaysWorkDrop(Line line)
        {
            Trip trip = null;

            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime tripDate;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                    for (int count = 0; count < trip.PairLength; count++)
                    {
                        dayOfWeek = (int)tripDate.DayOfWeek - 1;
                        dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                        weekWorkingDays[dayOfWeek]++;
                        tripDate = tripDate.AddDays(1);
                    }
                }


            }

            return weekWorkingDays;
        }


        #endregion

        #region DaysOfMonthOff
        private List<WorkDaysOfBidLine> CalcDaysOfMonthOff(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //  wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                for (int i = 1; i <= trip.PairLength; i++)
                {
                    workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                    daysWork.Add(workDay);
                    wkDay = wkDay.AddDays(1);
                }
            }

            return daysWork;
        }

        private List<WorkDaysOfBidLine> CalcDaysOfMonthOffVacation(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                                daysWork.Add(workDay);
                            }
                            wkDay = wkDay.AddDays(1);
                        }

                    }
                }
                else
                {

                    for (int i = 1; i <= trip.PairLength; i++)
                    {
                        workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                        daysWork.Add(workDay);
                        wkDay = wkDay.AddDays(1);
                    }
                }
            }

            return daysWork;
        }

        private List<WorkDaysOfBidLine> CalcDaysOfMonthOffDrop(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {

                    wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                    for (int i = 1; i <= trip.PairLength; i++)
                    {
                        workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                        daysWork.Add(workDay);
                        wkDay = wkDay.AddDays(1);
                    }
                }
            }

            return daysWork;
        }

        private int CalDaysOffEOMDrop(Line line)
        {
            int daysOff = 0;

            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            daysOff += vacationTrip.DutyPeriodsDetails.Where(x => (x.VacationType == "VD" || x.VacationType == "Split") && x.isInBp).Count(); ;
                        }




                    }



                }

            }
            return daysOff;

        }
		private int CalDaysOffEOM(Line line)
		{
			int daysOff = 0;
			if (line.LineNum == 15)
			{
			}
			if (_eomDate != DateTime.MinValue)
			{
				Trip trip = null;
				decimal tfp = 0m;
				bool isLastTrip = false; int paringCount = 0;


				foreach (var pairing in line.Pairings)
				{
					trip = GetTrip(pairing);


					isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
					DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
					DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
					if (tripEndDate >= _eomDate)
					{
						VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
						DateTime date = tripStartDate;
						if (vacationTrip != null)
						{
							daysOff += vacationTrip.DutyPeriodsDetails.Where(x => (x.VacationType == "VO" || x.VacationType == "Split") && x.isInBp).Count(); ;
						}




					}



				}

			}
			return daysOff;

		}

        #endregion

        #region OvernightCities
        private void CalcOvernightCities(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                foreach (var dp in trip.DutyPeriods)
                {
                    if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                    {   // only adds overnights and not the last day of trip
                        line.OvernightCities.Add(dp.ArrStaLastLeg);
                    }
                }
            }
        }

        private void CalcOvernightCitiesVacation(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                                {   // only adds overnights and not the last day of trip
                                    line.OvernightCities.Add(dp.ArrStaLastLeg);
                                }

                            }

                        }

                    }
                }
                else
                {

                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                        {   // only adds overnights and not the last day of trip
                            line.OvernightCities.Add(dp.ArrStaLastLeg);
                        }
                    }
                }
            }
        }

        private void CalcOvernightCitiesDrop(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                        {   // only adds overnights and not the last day of trip
                            line.OvernightCities.Add(dp.ArrStaLastLeg);
                        }
                    }
                }
            }
        }
        #endregion

        #region RestPeriods

        private List<RestPeriod> CalcRestPeriods(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //DateTime tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                // int rpt = 0;
                int rls = GlobalSettings.debrief;
                foreach (var dp in trip.DutyPeriods)
                {
                    //if not the first dutyperiod then we need to add one  to the tripstartdate
                    if (dp.DutPerSeqNum != 1)
                    {
                        tripStartDate = tripStartDate.AddDays(1);

                    }
                    // rpt = (dp.DutPerSeqNum == 1) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;

                    restPeriod = new RestPeriod();
                    restPeriod.PeriodId = periodId++;
                    restPeriod.IsInTrip = IsInTrip;
                    // restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.DepTimeFirstLeg - ((dp.DutPerSeqNum - 1) * 1440) - rpt).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                    restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                    lstRestPeriod.Add(restPeriod);

                    //Finding the status ,is in trip or 'between trip'
                    IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                    lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                }
            }
            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }
            return lstRestPeriod;
        }

        private List<RestPeriod> CalcRestPeriodsVacation(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            int rpt = 0;
            int rls = GlobalSettings.debrief;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        int depTimeFirstLeg = 0;
                        int landTimeLastLeg = 0;
                        int flightSequenceNumber = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            depTimeFirstLeg = 0;
                            flightSequenceNumber = 0;
                            landTimeLastLeg = 0;
                            //rpt = (dp.DutPerSeqNum == 1) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;

                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                try
                                {
                                    flightSequenceNumber = vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].VacationFlights.FirstOrDefault(x => x.FlightType == "VD").FlightSeqNo;
                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                                restPeriod = new RestPeriod();
                                restPeriod.PeriodId = periodId++;
                                restPeriod.IsInTrip = IsInTrip;
                                depTimeFirstLeg = dp.Flights[flightSequenceNumber - 1].DepTime;
                                restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(depTimeFirstLeg - ((dp.DutPerSeqNum - 1) * 1440) - rpt).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                                lstRestPeriod.Add(restPeriod);
                                //Finding the status ,is in trip or 'between trip'
                                IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                                flightSequenceNumber = vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].VacationFlights.LastOrDefault(x => x.FlightType == "VD").FlightSeqNo;
                                landTimeLastLeg = dp.Flights[flightSequenceNumber - 1].ArrTime;
                                lastDutyEndDate = tripStartDate.AddMinutes(landTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);

                            }

                            if (dp.DutPerSeqNum != 1)
                            {
                                tripStartDate = tripStartDate.AddDays(1);

                            }

                        }

                    }
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        //if not the first dutyperiod then we need to add one  to the tripstartdate
                        if (dp.DutPerSeqNum != 1)
                        {
                            tripStartDate = tripStartDate.AddDays(1);

                        }
                        restPeriod = new RestPeriod();
                        restPeriod.PeriodId = periodId++;
                        restPeriod.IsInTrip = IsInTrip;
                        restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                        lstRestPeriod.Add(restPeriod);

                        //Finding the status ,is in trip or 'between trip'
                        IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                        lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                    }
                }
            }


            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }
            return lstRestPeriod;
        }

        private List<RestPeriod> CalcRestPeriodsDrop(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    int rls = GlobalSettings.debrief;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        //if not the first dutyperiod then we need to add one  to the tripstartdate
                        if (dp.DutPerSeqNum != 1)
                        {
                            tripStartDate = tripStartDate.AddDays(1);

                        }

                        restPeriod = new RestPeriod();
                        restPeriod.PeriodId = periodId++;
                        restPeriod.IsInTrip = IsInTrip;
                        restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                        lstRestPeriod.Add(restPeriod);

                        //Finding the status ,is in trip or 'between trip'
                        IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                        lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                    }
                }
            }


            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }

            return lstRestPeriod;




        }

        #endregion

        #region WorkBlockLength

        private void CalculateWorkBlockLength(Line line)
        {

            Trip trip = null;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            DateTime tripStartDate, tripPreviousEndDate;
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                workBlockLength += trip.DutyPeriods.Count;
            }

            line.WorkBlockLengths[workBlockLength]++;
        }


        private void CalculateWorkBlockLengthVacation(Line line)
        {
            Trip trip = null;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            DateTime tripStartDate, tripPreviousEndDate;
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            int tripDays = 0;
            int dpIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //  tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    dpIndex = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                        {
                            tripStartDate = tripStartDate.AddDays(dpIndex);
                            tripDays = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                            break;
                        }
                        dpIndex++;
                    }

                }
                else
                {
                    tripDays = trip.DutyPeriods.Count;
                }

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(tripDays - 1);
                workBlockLength += tripDays;
            }

            if (workBlockLength != 0)
            {
                line.WorkBlockLengths[workBlockLength]++;
            }
        }


        private void CalculateWorkBlockLengthDrop(Line line)
        {

            Trip trip = null;
            DateTime tripStartDate, tripPreviousEndDate;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            int tripDays = 0;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDays = trip.DutyPeriods.Count;
                }

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(tripDays - 1);
                workBlockLength += tripDays;
            }

            if (workBlockLength != 0)
            {
                line.WorkBlockLengths[workBlockLength]++;
            }
        }

        #endregion

        #region EPush
        private TimeSpan CalcEPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                dutPd = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    ePush = (dp.Flights[0].DepTime - 1440 * dutPd < ePush) ? dp.Flights[0].DepTime - dutPd * 1440 : ePush;
                    dutPd++;
                }
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);
        }

        private TimeSpan CalcEPushVacation(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    dutPdIndex = 0;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }

                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "VD")
                            {
                                ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            }
                            else if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "Split")
                            {
                                for (int flightIndex = 0; flightIndex < dp.Flights.Count; flightIndex++)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPdIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        ePush = (dp.Flights[flightIndex].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                        break;
                                    }

                                }
                            }
                            dutPdIndex++;
                        }

                    }

                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            dutPdIndex++;
                        }
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {
                return CalcEPush(line);

                // throw;
            }
        }

        private TimeSpan CalcEPushDrop(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    dutPdIndex = 0;


                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            dutPdIndex++;
                        }
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);
            }
            catch (Exception)
            {
                return CalcEPush(line);
            }
        }

        #endregion

        #region EDomPush
        private TimeSpan CalcEDomPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);

        }


        private TimeSpan CalcEDomPushVacation(Line line)
        {
            try
            {

                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        dutPdIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "VD")
                            {
                                ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                break;
                            }
                            else if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "Split")
                            {
                                for (int flightIndex = 0; flightIndex < dp.Flights.Count; flightIndex++)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPdIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        ePush = (dp.Flights[flightIndex].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                        break;
                                    }

                                }
                                break;
                            }
                            dutPdIndex++;
                        }
                    }
                    else
                    {

                        ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {

                return CalcEDomPush(line);
            }

        }


        private TimeSpan CalcEDomPushDrop(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {

                return CalcEDomPush(line);
            }

        }

        #endregion

        #region WorkBlockDetails

        /// <summary>
        /// PURPOSE : Calculate work block details, count backToback (btb) events for each work block
        /// </summary>
        /// <param name="line"></param>
        //private void CalculateWorkBlockDetails(Line line)
        //{

        //    if (line.BlankLine) return;
        //    List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
        //    WorkBlockDetails workBlockDetails = new WorkBlockDetails();
        //    Trip trip = null;
        //    DateTime tripStartDate, tripEndDate, tripPreviousEndDate;

        //    tripPreviousEndDate = DateTime.Now;
        //    int tripPreviousLandTime = 0;
        //    int count = 0;

        //    int backToBack = 0;
        //    bool isLastTrip = false; int paringCount = 0;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

        //        trip = GetTrip(pairing);
        //        tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
        //        //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;
        //        if (count == 0)
        //        {
        //            workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
        //            workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
        //        }
        //        else if (count != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
        //        {
        //            workBlockDetails.EndTime = tripPreviousLandTime;
        //            workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
        //            workBlockDetails.BackToBackCount = backToBack - 1;
        //            lstWorkBlockDetails.Add(workBlockDetails);

        //            workBlockDetails = new WorkBlockDetails();
        //            workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
        //            workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;

        //            backToBack = 0;

        //        }

        //        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
        //        tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
        //        tripPreviousEndDate = tripEndDate;
        //        count++;
        //        backToBack++;
        //    }

        //    workBlockDetails.EndTime = tripPreviousLandTime;
        //    workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
        //    workBlockDetails.BackToBackCount = backToBack - 1;
        //    lstWorkBlockDetails.Add(workBlockDetails);

        //    line.WorkBlockList = lstWorkBlockDetails;


        //}


        private void CalculateWorkBlockDetails(Line line)
        {

            if (line.BlankLine) return;
            List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
            WorkBlockDetails workBlockDetails = new WorkBlockDetails();
            Trip trip = null;
            DateTime tripStartDate, tripEndDate, tripPreviousEndDate;

            tripPreviousEndDate = DateTime.MinValue;
            int tripPreviousLandTime = 0;
            int count = 0;

            int backToBack = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                //                tripStartDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2).Trim(' ')));
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                if (count == 0)
                {
                    if (trip.ReserveTrip)
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.show1stDay; ; ;
                    }
                    else
                    {

                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
                    }
                    workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                }
                else if (count != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    workBlockDetails.EndTime = tripPreviousLandTime;
                    workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                    workBlockDetails.BackToBackCount = backToBack - 1;
                    lstWorkBlockDetails.Add(workBlockDetails);

                    workBlockDetails = new WorkBlockDetails();
                    if (trip.ReserveTrip)
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.show1stDay; ;
                    }
                    else
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
                    }
                    //trip.BriefTime;
                    workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;

                    backToBack = 0;

                }

                tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                if (trip.ReserveTrip)
                {
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.debrief;
                }
                else
                {
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                }
                tripPreviousEndDate = tripEndDate;
                count++;
                backToBack++;
            }

            workBlockDetails.EndTime = tripPreviousLandTime;
            workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
            workBlockDetails.BackToBackCount = backToBack - 1;
            lstWorkBlockDetails.Add(workBlockDetails);

            line.WorkBlockList = lstWorkBlockDetails;


        }
       
        
        
        private void CalculateWorkBlockVacation(Line line)
        {
            try
            {



                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0;
                int backToBack = 0;
                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;
                int dpIndex = 0;
                int fltIndex = 0;
                bool isLastTrip = false; int paringCount = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        tripStartTime = 0;
                        tripEndTime = 0;
                        dpIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                if (tripStartTime == 0)
                                {
                                    tripStartTime = dp.DepTimeFirstLeg - trip.BriefTime;
                                    tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                }
                                tripEndTime = dp.LandTimeLastLeg - (1440 * (dp.DutPerSeqNum - 1));

                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                fltIndex = 0;
                                foreach (Flight flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[fltIndex].FlightType == "VD")
                                    {
                                        if (tripStartTime == 0)
                                        {
                                            tripStartTime = dp.Flights[fltIndex].DepTime - trip.BriefTime;
                                            tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                        }
                                        tripEndTime = dp.Flights[fltIndex].ArrTime - (1440 * (dp.DutPerSeqNum - 1));
                                    }
                                    fltIndex++;

                                }

                            }

                            if (tripStartTime == 0)
                            {
                                tripStartDate = tripStartDate.AddDays(1);
                            }

                            dpIndex++;
                        }
                    }
                    else
                    {
                        tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                        tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    }




                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;
                        workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                        workBlockDetails.BackToBackCount = backToBack - 1;
                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        backToBack = 0;

                    }


                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    isFirstTrip = false;
                    backToBack++;
                }

                workBlockDetails.EndTime = tripPreviousLandTime;
                workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                workBlockDetails.BackToBackCount = backToBack - 1;
                lstWorkBlockDetails.Add(workBlockDetails);
                line.WorkBlockList = lstWorkBlockDetails;
            }
            catch (Exception)
            {

                // throw;
            }

        }


        private void CalculateWorkBlockDrop(Line line)
        {
            try
            {


                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0;
                int backToBack = 0;
                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;
                //int dpIndex = 0;
                //int fltIndex = 0;

                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    //  tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;

                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }

                    else
                    {
                        tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
                        tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    }




                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;
                        workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                        workBlockDetails.BackToBackCount = backToBack - 1;
                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        backToBack = 0;

                    }


                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    isFirstTrip = false;
                    backToBack++;
                }

                workBlockDetails.EndTime = tripPreviousLandTime;
                workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                workBlockDetails.BackToBackCount = backToBack - 1;
                lstWorkBlockDetails.Add(workBlockDetails);
                line.WorkBlockList = lstWorkBlockDetails;

            }
            catch (Exception)
            {

                //throw;
            }


        }

        #endregion


        #region FlyPay


        private decimal CalcFlyPayEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VO_TfpInBpTot;
                        }




                    }



                }

            }
            return result;
        }

        private void CalcFlyPayEOMDrop(Line line)
        {


            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            line.FlyPay -= vacationTrip.VacationGrandTotal.VD_TfpInBpTot;
                        }




                    }



                }

            }


        }

        private decimal CalcvACPayEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VO_TfpInBpTot;
                        }




                    }



                }

            }
            return result;
        }

        private decimal CalcvACDropTfpEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count-1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VD_TfpInBpTot;
                        }




                    }



                }

            }
            return result;
        }
        #endregion





        //private int CalcLargestBlkDaysOffEOMDrop(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0, tripOff = 0, paringCount = 0;
        //    bool isLastTrip = false;
        //    DateTime tripDate = DateTime.MinValue;
        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    VacationTrip vacationTrip = null;

        //    foreach (var pairing in line.Pairings)
        //    {
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        if (trip == null)
        //        {
        //            continue;
        //        }
        //        vacationTrip = null;

        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

        //        if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //        {

        //            if (_vacationData.Keys.Contains(pairing))
        //            {
        //                if (_vacationData.FirstOrDefault(x => x.Key == pairing).Value != null)
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                }
        //                if (vacationTrip != null)
        //                {
        //                    tripDate = _bpEndDate.AddDays(1);
        //                }
        //            }
        //        }

        //        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;
        //        }

        //        oldTripdate = tripDate.AddDays(trip.PairLength - 1);
        //    }

        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;
        //        }

        //    }

        //    return largestDaysOff;

        //}


        //private int CalcLargestBlkDaysOffVacationEOM(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }
        //            if (vacTrip.TripType == "VOB")
        //            {
        //                tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
        //            }


        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = _bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        //private int CalcLargestBlkDaysOffVacationEOMDrop(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = _bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        /// <summary>
        /// Get Trip using trip name.
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="pairing"></param>
        private Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }

            return trip;

        }

        public int CalcAmPmSortOrder(Line line)
        {



            if (line.AMPM == "AM")
                return 1;
            else if (line.AMPM == "Mix")
                return 2;
            else if (line.AMPM == "---")
                return 3;
            else if (line.AMPM == " PM")
                return 4;
            else return 5;
        }

        //private void GetNextBidPeriodVacationStartDate()
        //{

        //    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
        //    {
        //        _nextBidPeriodVacationStartDate = _eOMStartdate;
        //    }
        //    else
        //    {
        //        int daysToADD = 0;
        //        int dayOfWeek = (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.DayOfWeek;
        //        daysToADD = 7 - dayOfWeek;
        //        _nextBidPeriodVacationStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(daysToADD);
        //    }

        //}
        #endregion

    }
}
