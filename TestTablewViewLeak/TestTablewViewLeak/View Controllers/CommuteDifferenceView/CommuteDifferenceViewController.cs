using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;
using TestTablewViewLeak.Utility;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;

namespace TestTablewViewLeak.ViewControllers.CommuteDifferenceView
{
    public partial class CommuteDifferenceViewController : UIViewController
    {
        WBidState wBidStateContent;
        UIAlertController AlertController;
        LoadingOverlay ActivityIndicator;
        public CommuteDifferenceViewController() : base("CommuteDifferenceViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            try
            {
                ActivityIndicator = new LoadingOverlay(View.Bounds, "Retrieving data. \n Please wait..");
                this.View.Add(ActivityIndicator);
                wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                List<CommuteFltChangeValues> lstComuteLineProperties = new List<CommuteFltChangeValues>();
                CommuteFltChange objCommuteFltChange = new CommuteFltChange();
                string localFlightDataVersion = string.Empty;
                //check file exists on the app data folder.
                var filePath = WBidHelper.WBidCommuteFilePath;

                if (File.Exists(filePath))
                {
                    //Deserailze file and get the flight data version
                    
                    var jsondata=File.ReadAllText(filePath);
                    objCommuteFltChange = WBidCollection.ConvertJSonStringToObject<CommuteFltChange>(jsondata);                   
                    localFlightDataVersion = objCommuteFltChange.FlightDataVersion;
                }

                if (localFlightDataVersion != GlobalSettings.ServerFlightDataVersion)
                {
                    InvokeInBackground(() =>
                    {
                        bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                        if (isConnectionAvailable)
                        {

                            FtCommutableLine commutableObj = wBidStateContent.Constraints.CLAuto;

                            // Download new flight data and Get Flight route details from a temporary flight data 
                            NetworkData objnetwork = new NetworkData();
                            var FlightRouteDetails = objnetwork.GetFlightRoutesForTempCalculation();

                            //Calculate daily commutes using the new flight data
                            CommuteCalculations objCommuteCalculations = new CommuteCalculations();
                            objCommuteCalculations.FtCommutable = commutableObj;
                            List<CommuteTime> lstDailyCommuteTimes = objCommuteCalculations.CalculateDailyCommutableTimesForVacationDifference(commutableObj, FlightRouteDetails);

                            //Calculate new commute line properties
                            lstComuteLineProperties = CalculateCommutableLineProperties(lstDailyCommuteTimes);

                            //need to filter only the difference data fromt list, but for temporary ,we are showing all values.
                            List<CommuteFltChangeValues> lstdifferencedata = new List<CommuteFltChangeValues>();
                            foreach (var item in lstComuteLineProperties)
                            {
                                //if (item.NewCmtBa != item.OldCmtBa || item.NewCmtFr != item.OldCmtFr || item.NewCmtOV != item.OldCmtOV)
                                // {
                                lstdifferencedata.Add(item);
                                // }
                            }


                            //save Commute difference values into the Json file
                            objCommuteFltChange = new CommuteFltChange();
                            objCommuteFltChange.FlightDataVersion = GlobalSettings.ServerFlightDataVersion;
                            objCommuteFltChange.LstCommuteFltChangeValues = lstdifferencedata;
                            lstComuteLineProperties = lstdifferencedata;

                            var jsonData = ServiceUtils.JsonSerializer(objCommuteFltChange);
                            File.WriteAllText(filePath, jsonData);

                            var CommutDiffData = new List<CommuteFltChangeValues>(lstComuteLineProperties);
                            InvokeOnMainThread(() =>
                            {
                                tblCommuteDifference.Source = new CommuteDifferenceTableViewSource(CommutDiffData);
                                tblCommuteDifference.ReloadData();
                                ActivityIndicator.Hide();
                            });
                            
                        }
                        else
                        {
                            if (WBidHelper.IsSouthWestWifiOr2wire())
                            {
                                InvokeOnMainThread(() =>
                                {
                                    AlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                                    {
                                        this.DismissViewController(true, null);
                                    }));
                                    this.PresentViewController(AlertController, true, null);
                                    NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);


                                });


                            }
                            else
                            {

                                InvokeOnMainThread(() =>
                                {

                                    AlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                                    {
                                        this.DismissViewController(true, null);
                                    }));
                                    this.PresentViewController(AlertController, true, null);
                                    NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
                                });
                            }
                        }
                    });
                }
                else
                {
                    lstComuteLineProperties = objCommuteFltChange.LstCommuteFltChangeValues;
                    var CommutDiffData = new List<CommuteFltChangeValues>(lstComuteLineProperties);
                    tblCommuteDifference.Source = new CommuteDifferenceTableViewSource(CommutDiffData);
                    tblCommuteDifference.ReloadData();
                    ActivityIndicator.Hide();
                }


                

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void btnOKClick(NSObject sender)
        {
            this.DismissViewController(true, null);

        }

        private List<CommuteFltChangeValues> CalculateCommutableLineProperties(List<CommuteTime> lstDailyCommuteTimes)
        {
            List<CommuteFltChangeValues> lstCommuteFtData = new List<CommuteFltChangeValues>();
            CommuteFltChangeValues objCommutData;

            try
            {
                var lines = GlobalSettings.Lines.ToList();
                lines = lines.Where(x => x.BlankLine == false).ToList();
                foreach (var line in lines)
                {
                    decimal CommutableBacks = 0;
                    decimal commutableFronts = 0;
                    decimal CommutabilityFront = 0;
                    decimal CommutabilityBack = 0;
                    decimal CommutabilityOverall = 0;

                    DateTime tripStartDate = DateTime.MinValue;
                    objCommutData = new CommuteFltChangeValues();

                    if (line.WorkBlockList != null)
                    {
                        bool isCommuteFrontEnd = false;
                        bool isCommuteBackEnd = false;
                        int NightsInMid = 0;
                        foreach (WorkBlockDetails workBlock in line.WorkBlockList)
                        {
                            //Checking the  corresponding Commute based on Workblock Start time
                            var commutTimes = lstDailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

                            if (commutTimes != null)
                            {
                                if (commutTimes.EarliestArrivel != DateTime.MinValue)
                                {
                                    //In order for the line to be commutable on the front, you have to arrive (plus check in time) before the workblock startdatetime.
                                    //In order for the line to be commutable on the front, you have to arrive (plus check in time) before the workblock startdatetime.
                                    //  isCommuteFrontEnd = (commutTimes.EarliestArrivel.AddMinutes(GlobalSettings.WBidStateContent.Constraints.Commute.CheckInTime)) <= (workBlock.StartDateTime.AddMinutes(GlobalSettings.show1stDay));
                                    isCommuteFrontEnd = (commutTimes.EarliestArrivel.AddMinutes(wBidStateContent.Constraints.CLAuto.CheckInTime)) <= (workBlock.StartDateTime);
                                    if (isCommuteFrontEnd)
                                    {
                                        commutableFronts++;
                                    }
                                }
                            }


                            //Checking the  corresponding Commute based on Workblock End time
                            // using EndDate to account for irregular datetimes in company time keeping method.
                            //commutTimes = GlobalSettings.WBidStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);
                            commutTimes = lstDailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

                            if (commutTimes != null)
                            {
                                if (commutTimes.LatestDeparture != DateTime.MinValue)
                                {
                                    isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes(-wBidStateContent.Constraints.CLAuto.BaseTime) >= workBlock.EndDateTime;
                                    if (isCommuteBackEnd)
                                    {
                                        CommutableBacks++;
                                    }
                                }
                            }

                            NightsInMid += workBlock.nightINDomicile;

                        }

                    }
                    int TotalCommutes = line.WorkBlockList.Count;
                    CommutabilityFront = Math.Round((commutableFronts / TotalCommutes) * 100, 2);
                    CommutabilityBack = Math.Round((CommutableBacks / TotalCommutes) * 100, 2);
                    CommutabilityOverall = Math.Round((commutableFronts + CommutableBacks) / (2 * TotalCommutes)  *100, 2);

                    objCommutData.LineNum = line.LineNum;
                    objCommutData.NewCmtOV = Math.Round(decimal.Parse(String.Format("{0:0.00}",CommutabilityOverall)),2);
                    objCommutData.NewCmtFr = Math.Round(decimal.Parse(String.Format("{0:0.00}",CommutabilityFront)),2);
                    objCommutData.NewCmtBa = Math.Round(decimal.Parse(String.Format("{0:0.00}",CommutabilityBack)),2);

                    objCommutData.OldCmtOV = Math.Round(decimal.Parse(String.Format("{0:0.00}",line.CommutabilityOverall)),2);
                    objCommutData.OldCmtFr = Math.Round(decimal.Parse(String.Format("{0:0.00}",line.CommutabilityFront)),2);
                    objCommutData.OldCmtBa = Math.Round(decimal.Parse(String.Format("{0:0.00}",line.CommutabilityBack)), 2);

                    lstCommuteFtData.Add(objCommutData);

                }
            }
            catch (Exception ex)
            {
            }
            return lstCommuteFtData;
        }
    }
}

