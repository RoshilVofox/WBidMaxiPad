using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WBid.WBidiPad.PortableLibrary;
using VacationCorrection;
using System.IO;
using System.Net;
//using MiniZip.ZipArchive;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.IO.Compression;

namespace WBid.WBidiPad.iOS
{
    public partial class TestVacationViewController : UIViewController
    {
        Dictionary<string, Trip> trips = null;
        Dictionary<string, Line> lines = null;
        public TestVacationViewController()
            : base("TestVacationViewController", null)
        {
        }
        class MyPopDelegate : UIPopoverControllerDelegate
        {
            TestVacationViewController _parent;
            public MyPopDelegate(TestVacationViewController parent)
            {
                _parent = parent;
            }
            public override void DidDismiss(UIPopoverController popoverController)
            {
                _parent.popoverController = null;
                NSNotificationCenter.DefaultCenter.RemoveObserver(_parent.dateNotif);
                foreach (UIButton btn in _parent.btnStartDate)
                {
                    btn.Selected = false;
                }
                foreach (UIButton btn in _parent.btnEndDate)
                {
                    btn.Selected = false;
                }

                foreach (UIButton btn in _parent.btnVStartDate)
                {
                    btn.Selected = false;
                }
                foreach (UIButton btn in _parent.btnVEndDate)
                {
                    btn.Selected = false;
                }
                foreach (UIButton btn in _parent.btnCFVStartDate)
                {
                    btn.Selected = false;
                }

            }
        }
        NSObject dateNotif;
        UIPopoverController popoverController;
        List<int> lstSelected = new List<int>();
        List<int> lstFVSelected = new List<int>();
        List<int> lstCFVSelected = new List<int>();

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            foreach (UIButton btn in btnSelect)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
            }
            foreach (UIButton btn in btnStartDate)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            foreach (UIButton btn in btnEndDate)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }


            foreach (UIButton btn in btnVacationSelect)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
            }
            foreach (UIButton btn in btnVStartDate)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            foreach (UIButton btn in btnVEndDate)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            foreach (UIButton btn in btnCFVSelect)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("roundNormal.png"), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("roundActive.png"), UIControlState.Selected);
            }
            foreach (UIButton btn in btnCFVStartDate)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            setValues();

            this.BtnCancel.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            this.btnOK.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);



        }

        private void setValues()
        {
            DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);

            double days = 0;
            foreach (UIButton btn in btnStartDate)
            {
				btn.SetTitle (defDate.Date.AddDays (days).ToString ("MM-dd-yyyy"), UIControlState.Normal);
				btn.SetTitle (defDate.Date.AddDays (days).ToString ("MM-dd-yyyy"), UIControlState.Selected);
                days += 5;
            }
            days = 0;
            foreach (UIButton btn in btnVStartDate)
            {
                btn.SetTitle(defDate.Date.AddDays(days).ToString("MM-dd-yyyy"), UIControlState.Normal);
                btn.SetTitle(defDate.Date.AddDays(days).ToString("MM-dd-yyyy"), UIControlState.Selected);
                days += 5;
            }

            foreach (UIButton btn in btnCFVStartDate)
            {
                btn.SetTitle(defDate.Date.AddDays(days).ToString("MM-dd-yyyy"), UIControlState.Normal);
                btn.SetTitle(defDate.Date.AddDays(days).ToString("MM-dd-yyyy"), UIControlState.Selected);
                days += 5;
            }
            double days1 = 5;
            foreach (UIButton btn in btnEndDate)
            {
				btn.SetTitle (defDate.Date.AddDays (days1).ToString ("MM-dd-yyyy"), UIControlState.Normal);
				btn.SetTitle (defDate.Date.AddDays (days1).ToString ("MM-dd-yyyy"), UIControlState.Selected);
                days1 += 5;
            }
            days1 = 5;
            foreach (UIButton btn in btnVEndDate)
            {
                btn.SetTitle(defDate.Date.AddDays(days1).ToString("MM-dd-yyyy"), UIControlState.Normal);
                btn.SetTitle(defDate.Date.AddDays(days1).ToString("MM-dd-yyyy"), UIControlState.Selected);
                days1 += 5;
            }
//            NSDateFormatter dateFormat = new NSDateFormatter();
//			dateFormat.DateFormat = "MM-dd-yyyy";
            if (GlobalSettings.SeniorityListMember!=null && GlobalSettings.SeniorityListMember.Absences != null)
            {
                int index = 0;
                foreach (Absense abs in GlobalSettings.SeniorityListMember.Absences)
                {
					btnStartDate [index].SetTitle (abs.StartAbsenceDate.ToString ("MM-dd-yyyy"), UIControlState.Normal);
					btnStartDate [index].SetTitle (abs.StartAbsenceDate.ToString ("MM-dd-yyyy"), UIControlState.Selected);

                    btnVStartDate[index].SetTitle(abs.StartAbsenceDate.ToString("MM-dd-yyyy"), UIControlState.Normal);
                    btnVStartDate[index].SetTitle(abs.StartAbsenceDate.ToString("MM-dd-yyyy"), UIControlState.Selected);


					btnEndDate [index].SetTitle (abs.EndAbsenceDate.ToString ("MM-dd-yyyy"), UIControlState.Normal);
					btnEndDate [index].SetTitle (abs.EndAbsenceDate.ToString ("MM-dd-yyyy"), UIControlState.Selected);

                    btnVEndDate[index].SetTitle(abs.EndAbsenceDate.ToString("MM-dd-yyyy"), UIControlState.Normal);
                    btnVEndDate[index].SetTitle(abs.EndAbsenceDate.ToString("MM-dd-yyyy"), UIControlState.Selected);

                    btnSelect[index].Selected = true;
                    btnVacationSelect[index].Selected = true;
                    index++;
                }
            }
            else
            {
                GlobalSettings.SeniorityListMember = new SeniorityListMember();
                GlobalSettings.SeniorityListMember.Absences = new List<Absense>();
            }

        }

        partial void btnOKTapped(UIKit.UIButton sender)
        {
            try
            {
                lstSelected.Clear();
                lstFVSelected.Clear();
                int FVindex = 0;
                int CFVindex = 0;
                int index = 0;
                foreach (UIButton btn in btnSelect)
                {
                    if (btn.Selected)
                    {
                        lstSelected.Add(index);
                    }
                    index++;
                }
                GlobalSettings.SeniorityListMember = new SeniorityListMember();
                GlobalSettings.SeniorityListMember.Absences = new List<Absense>();

                foreach (int i in lstSelected)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense
                        {
                            StartAbsenceDate = DateTime.Parse(btnStartDate[i].TitleLabel.Text),
                            EndAbsenceDate = DateTime.Parse(btnEndDate[i].TitleLabel.Text),
                            AbsenceType = "VA"
                        }
                    );
                }

                var outofBidPeriod = GlobalSettings.SeniorityListMember.Absences.Where(x => !(x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate));
                // Get the ordered absence date.If the multiple vacation exists in the seniority list with consequitive dates ,then will combine the adjacent vacation dates.
                GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
                GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
                if (outofBidPeriod.Count() > 0)
                {
                    // MessageBox.Show(outofBidPeriod.Count() + " Test Vacation Week is not in the Bid Period");
                    GlobalSettings.SeniorityListMember.Absences.RemoveAll(x => !(x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate));

                }
                if (GlobalSettings.OrderedVacationDays.Count > 0)
                {
                    GlobalSettings.IsVacationCorrection = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                }
                string zipFilename = WBidHelper.GenarateZipFileName();



                foreach (UIButton btn in btnVacationSelect)
                {
                    if (btn.Selected)
                    {
                        lstFVSelected.Add(FVindex);
                    }
                    FVindex++;
                }
                foreach (UIButton btn in btnCFVSelect)
                {
                    if (btn.Selected)
                    {
                        lstCFVSelected.Add(CFVindex);
                    }
                    CFVindex++;
                }
                GlobalSettings.FVVacation = new List<Absense>();


                foreach (int i in lstFVSelected)
                {
                    GlobalSettings.FVVacation.Add(
                        new Absense
                        {
                            StartAbsenceDate = DateTime.Parse(btnVStartDate[i].TitleLabel.Text),
                            EndAbsenceDate = DateTime.Parse(btnVEndDate[i].TitleLabel.Text),
                            AbsenceType = "FV"
                        }
                    );
                }
                foreach (int i in lstCFVSelected)
                {
                    GlobalSettings.FVVacation.Add(
                        new Absense
                        {
                            StartAbsenceDate = DateTime.Parse(btnCFVStartDate[i].TitleLabel.Text),
                                                        AbsenceType = "CFV"
                        }
                    );
                }
                if (GlobalSettings.FVVacation.Count > 0)
                {
                    GlobalSettings.IsFVVacation = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                }

                ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
                LoadingOverlay overlay = new LoadingOverlay(this.View.Bounds, "Reparsing.. Please wait.");
                this.Add(overlay);
                InvokeInBackground(() =>
                    {
                        try
                        {
                            ReparseLineAndTripFileForvacation(reparseParams);
                            InvokeOnMainThread(() =>
                                {
                                    NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
									NSNotificationCenter.DefaultCenter.PostNotificationName("ResetButtonStates", null);
                                    this.DismissViewController(true, null);
                                });
                        }
                        catch (Exception ex)
                        {
                            
                             InvokeOnMainThread(() =>
                                {

                                    throw ex;
                                });
                        }
                    });
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        partial void btnCancelTapped(UIKit.UIButton sender)
        {
            this.DismissViewController(true, null);
        }

        partial void btnSelectTapped(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;

        }
        partial void btnVacationSelectTapped(UIButton sender)
        {
            sender.Selected = !sender.Selected;
        }
        
        partial void btnVacationStartDateTapped(UIButton sender)
        {
            sender.Selected = true;
            dateNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDateText"), handleVacationChangeDateText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "datePad";
            popoverContent.dateValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }
        partial void btnStartDateTapped(UIKit.UIButton sender)
        {
            sender.Selected = true;
			dateNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDateText"), handleChangeDateText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "datePad";
            popoverContent.dateValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }

        partial void btnVacationEndDateTapped(UIButton sender)
        {
            sender.Selected = true;
            dateNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDateText"), handleVacationChangeDateText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "datePad";
            popoverContent.dateValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }

        partial void btnEndDateTapped(UIKit.UIButton sender)
        {
            sender.Selected = true;
			dateNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDateText"), handleChangeDateText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "datePad";
            popoverContent.dateValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }
        partial void btnCFVStartDateTapped(UIKit.UIButton sender)
        {
            sender.Selected = true;
            dateNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeDateText"), handleCFVVacationChangeDateText);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "datePad";
            popoverContent.dateValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(300, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }

        partial void btnCFVSelectTapped(UIKit.UIButton sender)
        {
            sender.Selected = !sender.Selected;
        }


        public void handleChangeDateText(NSNotification n)
        {
            foreach (UIButton btn in btnStartDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }
            foreach (UIButton btn in btnVStartDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }

            foreach (UIButton btn in btnEndDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }

            foreach (UIButton btn in btnVEndDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }
        }
        public void handleVacationChangeDateText(NSNotification n)
        {
         
            foreach (UIButton btn in btnVStartDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }

          
            foreach (UIButton btn in btnVEndDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }
        }
        public void handleCFVVacationChangeDateText(NSNotification n)
        {

            foreach (UIButton btn in btnCFVStartDate)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }


            //foreach (UIButton btn in btnVEndDate)
            //{
            //    if (btn.Selected)
            //    {
            //        btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
            //        btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
            //    }
            //}
        }
        private void ReparseLineAndTripFileForvacation(ReparseParameters reparseParams)
        {


            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationCorrection  = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationDrop= false;
            List<string> pairingwHasNoDetails = new List<string>();
            string fileToSave = string.Empty;

            //Parse trip and Line file
            trips = ReparseBL.ParseTripFile(reparseParams.ZipFileName);


            if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
            {
                FASecondRoundParser fASecondRound = new FASecondRoundParser();
                lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + reparseParams.ZipFileName.Substring(0, 6).ToString() + "/PS", ref trips, GlobalSettings.FAReserveDayPay, reparseParams.ZipFileName.Substring(2, 3));

            }
            else
            {
                lines = ReparseBL.ParseLineFiles(reparseParams.ZipFileName);
            }

           // if (trips == null) return null;

            TripTtpParser tripTtpParser = new TripTtpParser();
            List<CityPair> listCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");

            //Second Round missing trip management
            //---------------------------------------------------------------------------

            if (GlobalSettings.CurrentBidDetails.Round == "S")
            {   //If  the round is second round ,some times trip list contains  missing trip. So we need  take these trip details from old .WBP file.
                //Otherwise we again need to scrap the missing details from website. The issue is if the bid data is older one, we cannot scrap it from website.
                //tempTrip = reparseParams.Trips;


                //List<string> allPair = lines.SelectMany(x => x.Value.Pairings).ToList();
                //pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => z == x.Substring(0, 4))).ToList();
                List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
                pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

                if (pairingwHasNoDetails.Count > 0)
                {
                    Dictionary<string, Trip> missingTrips = new Dictionary<string, Trip>();
                    //missingTrips = missingTrips.Concat(tempTrip.Where(x => pairingwHasNoDetails.Contains(x.Key.ToString()))).ToDictionary(pair => pair.Key, pair => pair.Value);
                    missingTrips = missingTrips.Concat(GlobalSettings.Trip.Where(x => pairingwHasNoDetails.Contains(x.TripNum)).ToDictionary(s => s.TripNum, s => s)).ToDictionary(pair => pair.Key, pair => pair.Value);
                    if (missingTrips.Count == 0)
                    {
                        string bidFileName = string.Empty;
                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                        BidLineParser bidLineParser = new BidLineParser();
                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
                        missingTrips = missingTrips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                    }
                    // trips = trips.Concat().ToDictionary(pair => pair.Key, pair => pair.Value);
                    foreach (var trip in missingTrips)
                    {
                        if (!trips.Keys.Contains(trip.Key) && !string.IsNullOrEmpty(trip.Key))
                            trips.Add(trip.Key, trip.Value);
                    }

                }


            }

            //---------------------------------------------------------------------------



            // Additional processing needs to be done to FA trips before CalculateTripPropertyValues
            CalculateTripProperties calcProperties = new CalculateTripProperties();
            if (reparseParams.ZipFileName.Substring(0, 1) == "A")
                calcProperties.PreProcessFaTrips(trips, listCityPair);

            calcProperties.CalculateTripPropertyValues(trips, listCityPair);


            GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));

            CalculateLineProperties calcLineProperties = new CalculateLineProperties();
            calcLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);

            GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

            if (GlobalSettings.IsVacationCorrection|| GlobalSettings.IsFVVacation)
            {
                if (GlobalSettings.IsVacationCorrection)
                {
                    PerformVacation();
                    SaveParsedFiles(trips, lines);
                }
                if (GlobalSettings.IsFVVacation)
                {
                    performFVVacation();
                    WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                    //SaveParsedFiles(trips, lines);
                }

                
            }

          


        }
        private void performFVVacation()
        {
            GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;
            FVVacation objvac = new FVVacation();
            GlobalSettings.Lines = new ObservableCollection<Line>(objvac.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList()));
        }
        private void PerformVacation()
        {
            try
            {
                VacationCorrectionParams vacationParams = new VacationCorrectionParams();
                  

                if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                    string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
                    string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

                    FlightPlan flightPlan = null;
                    WebClient wcClient = new WebClient();
                    //Downloading networkdat file
                    wcClient.DownloadFile(serverPath, zipLocalFile);

                    //Extracting the zip file
//                    var zip = new ZipArchive();
//                    zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");

					string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");// + Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";


					if (!File.Exists(networkDataPath))
					{
						if (File.Exists(zipLocalFile))
						ZipFile.ExtractToDirectory(zipLocalFile,target);
					}

					//ZipStorer.

//					// Open an existing zip file for reading
//					ZipStorer zip = ZipStorer.Open(zipLocalFile, FileAccess.Read);
//
//					// Read the central directory collection
//					List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
//
//					// Look for the desired file
//					foreach (ZipStorer.ZipFileEntry entry in dir)
//					{
//						zip.ExtractFile(entry, target+entry);
//					}
//					zip.Close();

                    //Deserializing data to FlightPlan object
                    FlightPlan fp = new FlightPlan();
                    using (FileStream networkDatatream = File.OpenRead(networkDataPath))
                    {

                        FlightPlan objineinfo = new FlightPlan();
                        flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);

                    }

                    if (File.Exists(zipLocalFile))
                    {
                        File.Delete(zipLocalFile);
                    }
//                    if (File.Exists(networkDataPath))
//                    {
//                        File.Delete(networkDataPath);
//                    }




                     vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                                (fr, f) =>
                               new FlightRouteDetails
                               {
                                   Flight = f.FlightId,
                                   FlightDate = fr.FlightDate,
                                   Orig = f.Orig,
                                   Dest = f.Dest,
                                   Cdep = f.Cdep,
                                   Carr = f.Carr,
                                   Ldep = f.Ldep,
                                   Larr = f.Larr,
                                   RouteNum = fr.RouteNum,

                               }).ToList();

                }


                vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
                vacationParams.Trips = trips;
                vacationParams.Lines = lines;
                //  VacationData = new Dictionary<string, TripMultiVacData>();


                //Performing vacation correction algoritham
                VacationCorrectionBL vacationBL = new VacationCorrectionBL();

                if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);
                }
                else
                {
                    GlobalSettings.VacationData = vacationBL.PerformFAVacationCorrection(vacationParams);
                }



                if (GlobalSettings.VacationData != null)
                {

                    string fileToSave = string.Empty;
                    fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


                    // save the VAC file to app data folder

                    var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
                    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                    stream.Dispose();
                    stream.Close();
                }
                else
                {
                    GlobalSettings.IsVacationCorrection = false;
                }



            }
            catch (Exception ex)
            {
                GlobalSettings.IsVacationCorrection = false;
                throw ex;
            }
        }

        private void SaveParsedFiles(Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
        {

            string fileToSave = string.Empty;

            fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


            TripInfo tripInfo = new TripInfo()
            {
                TripVersion = GlobalSettings.TripVersion,
                Trips = trips

            };
            
            var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP");
            ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP", tripInfo, stream);
            stream.Dispose();
            stream.Close();
           
            GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));


            if (GlobalSettings.IsVacationCorrection && GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
            {//set  vacation details  to line object. 

                CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                calVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
            }

            LineInfo lineInfo = new LineInfo()
            {
                LineVersion = GlobalSettings.LineVersion,
                Lines = lines

            };

            GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

            try
            {
                var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
                ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
                linestream.Dispose();
                linestream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            foreach (Line line in GlobalSettings.Lines)
            {
                line.ConstraintPoints = new ConstraintPoints();
                line.WeightPoints = new WeightPoints();
            }

            //Read the intial state file value from DWC file and create state file
            if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS"))
            {
                try
                {

                    WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                //Read the state file object and store it to global settings.
                GlobalSettings.WBidStateCollection = XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
            }
            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.IsVacationCorrection)
            {
                var vacation = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "VA").Select(y => new Vacation { StartDate = y.StartAbsenceDate.ToShortDateString(), EndDate = y.EndAbsenceDate.ToShortDateString() });

                GlobalSettings.WBidStateCollection.Vacation.AddRange(vacation.ToList());

                wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
            }
            else
                wBIdStateContent.IsVacationOverlapOverlapCorrection = false;
            WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
           
//            NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
//            this.DismissViewController(true, null);
           
        }
    }
    public class VacationTestWeek
    {
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

