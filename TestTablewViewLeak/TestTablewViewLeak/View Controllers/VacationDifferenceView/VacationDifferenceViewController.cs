using System;
using System.Collections.Generic;
using System.Globalization;
using System.Json;
using System.Linq;
using Foundation;
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

            ActivityIndicator = new LoadingOverlay(View.Bounds, "Retrieving data. \n Please wait..");
            this.View.Add(ActivityIndicator);

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

            
            var vavacation = GlobalSettings.WBidStateCollection.Vacation;
            if (vavacation != null && vavacation.Count > 0)
            {
                foreach (var item in vavacation)
                {
                   
                    var startdate = DateTime.Parse(item.StartDate,CultureInfo.InvariantCulture);
                    var enddate = DateTime.Parse(item.EndDate, CultureInfo.InvariantCulture);
                    var vacationstring = startdate.Month + "/" + startdate.Day + "-" + enddate.Month + "/" + enddate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = vacationstring });

                }
            }
            var Fvvavacation = GlobalSettings.WBidStateCollection.FVVacation;
            if (Fvvavacation != null && Fvvavacation.Count > 0)
            {
                foreach (var item in Fvvavacation)
                {
                    var vacationstring = item.StartAbsenceDate.Month + "/" + item.StartAbsenceDate.Day + "-" + item.EndAbsenceDate.Month + "/" + item.EndAbsenceDate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = item.AbsenceType, VacDate = vacationstring });
                }
            }
            InvokeInBackground(() =>
            {
                ObjOdata.GetVacationDifferenceDetails(input);
            });
            
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void btnOkClick(NSObject sender)
        {
            this.DismissViewController(true,null);
        }
        public void ServiceResponce(JsonValue jsonDoc)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("Service Success");
                lstVacationDifferencedata = CommonClass.ConvertJSonToObject<List<VacationValueDifferenceOutputDTO>>(jsonDoc.ToString());
                if (lstVacationDifferencedata.Count > 0)
                {
                   ActivityIndicator.Hide();
                    lstFlightDataChangevalues = lstVacationDifferencedata.FirstOrDefault().lstFlightDataChangeVacValues;
                    tblVacDifference.Source = new VacDiffTableViewControllerSource(lstFlightDataChangevalues);
                    tblVacDifference.ReloadData();
                }
                else
                {
                    InvokeOnMainThread(() => {
                        ActivityIndicator.Hide();
                        string message = string.Empty;
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            message = "There are no differences in pay for your vacation with the new Flight Data.";
                        }
                        else
                        {
                            message = "There are no differences in pay with the new Flight Data. But if you have vacation, please turn ON vacation and check the vacation difference";
                        }
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {

                            this.DismissViewController(true, null);
                        }));
                        this.PresentViewController(okAlertController, true, null);
                    });
                }
                
                
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

