﻿using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
    public partial class VacationDifferenceViewController : UIViewController,IServiceDelegate
    {
        LoadingOverlay ActivityIndicator;
        public List<VacationValueDifferenceOutputDTO> lstVacationDifferencedata { get; set; }
        public List<FlightDataChangeVacValues> lstFlightDataChangevalues { get; set; }
        VacationValueDifferenceInputDTO input;
        OdataBuilder ObjOdata;
        public VacationDifferenceViewController() : base("VacationDifferenceViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            ObjOdata = new OdataBuilder();
            input = new VacationValueDifferenceInputDTO();
            ObjOdata.RestService.Objdelegate = this;

            
            input.BidDetails = new UserBidDetails();
            input.BidDetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
            input.BidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
            input.BidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2;
            input.BidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
            input.BidDetails.Month = GlobalSettings.CurrentBidDetails.Month;


            input.IsDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
            input.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
            input.IsVAC = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
            input.FAEOMStartDate = GlobalSettings.FAEOMStartDate.Date.Day;
            input.FromApp = (int)WBid.WBidiPad.Core.Enum.FromApp.WbidmaxIpad;
            input.lstVacation = new List<VacationInfo>();

            input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = "05/29-06/04" });

            var vavacation = GlobalSettings.WBidStateCollection.Vacation;
            if (vavacation != null && vavacation.Count > 0)
            {
                foreach (var item in vavacation)
                {
                    var startdate = Convert.ToDateTime(item.StartDate);
                    var enddate = Convert.ToDateTime(item.EndDate);
                    var vacationstring = startdate.Month + "/" + startdate.Day + "-" + enddate.Month + "/" + enddate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = vacationstring });
                    
                }
            }
            var Fvvavacation = GlobalSettings.WBidStateCollection.FVVacation;
            if (Fvvavacation!=null && Fvvavacation.Count > 0)
            {
                foreach (var item in Fvvavacation)
                {
                    var vacationstring = item.StartAbsenceDate.Month + "/" + item.StartAbsenceDate.Day + "-" + item.EndAbsenceDate.Month + "/" + item.EndAbsenceDate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = item.AbsenceType, VacDate = vacationstring });
                }
            }

            ObjOdata.GetVacationDifferenceDetails(input);
            
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public void ServiceResponce(JsonValue jsonDoc)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("Service Success");
                lstVacationDifferencedata = CommonClass.ConvertJSonToObject<List<VacationValueDifferenceOutputDTO>>(jsonDoc.ToString());
                if (lstVacationDifferencedata.Count > 0)
                {
                    lstFlightDataChangevalues = lstVacationDifferencedata.FirstOrDefault().lstFlightDataChangeVacValues;
                }
                tblVacDifference.Source = new VacDiffTableViewControllerSource(lstFlightDataChangevalues); 
               // lstCAPOutputParameter = lstCAPOutputParameter.Where(x => x.CurrentMonthCap != null).ToList();
                //tblCAPDetails.Source = new CAPTableViewControllerSource(lstCAPOutputParameter);




            });
        }

        public void ResponceError(string Error)
        {
            InvokeOnMainThread(() => {
                ActivityIndicator.Hide();
                Console.WriteLine("Service Fail");

                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            });
        }
    }
}

