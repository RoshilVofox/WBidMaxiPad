using System;

using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.iOS;
using System.Collections.ObjectModel;
using WBid.WBidiPad.SharedLibrary.Utility;
using Foundation;
using CoreGraphics;
using System.Drawing;

namespace TestTablewViewLeak.ViewControllers
{
    public partial class SecretDataDownload : UIViewController
    {

        List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).ToList();
        List<string> listBases = GlobalSettings.WBidINIContent.Domiciles.Select(x => x.DomicileName).OrderBy(y => y).ToList();
        List<string> SelectedBases = new List<string>();
        string UserId = String.Empty;
        string Password = String.Empty;
        string Selectedround = String.Empty;
        string Selectedposition = String.Empty;
        string Selectedmonth = String.Empty;
        private string SessionCredential = String.Empty;
        UIPopoverController popoverController;
        private DownloadInfo _downloadFileDetails;
        List<string> LogData;
        Dictionary<string, Trip> trips;
        Dictionary<string, Line> lines;
        bool IsMissingTripFailed = false;
        /// <summary>
        /// create single instance of TripTtpParser class
        /// </summary>
        private TripTtpParser _tripTtpParser;

        private UIView activeview;             // Controller that activated the keyboard
        private float scrollamount = 0.0f;    // amount to scroll 
        private float bottom = 0.0f;           // bottom point
        private float offset = 10.0f;          // extra offset
        private bool moveViewUp = false;

        public TripTtpParser TripTtpParser
        {
            get
            {
                return _tripTtpParser ?? (_tripTtpParser = new TripTtpParser());
            }
        }

        private CalculateTripProperties _calculateTripProperties;
        public CalculateTripProperties CalculateTripProperties
        {
            get
            {
                return _calculateTripProperties ?? (_calculateTripProperties = new CalculateTripProperties());
            }
        }


        private CalculateLineProperties _calculateLineProperties;
        public CalculateLineProperties CalculateLineProperties
        {
            get
            {
                return _calculateLineProperties ?? (_calculateLineProperties = new CalculateLineProperties());
            }
        }
        public SecretDataDownload() : base("SecretDataDownload", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tblDomiciles.Source = new SecretDataTableViewSource(this);
            int bidmonth = DateTime.Now.AddMonths(1).Month;
            string month = (WBidCollection.GetBidPeriods().FirstOrDefault(x => x.BidPeriodId == bidmonth).Period);
            btnMonth.SetTitle(month, UIControlState.Normal);
            Selectedmonth = month;
            Selectedround = "1";
            Selectedposition = "Both";
            btnFirstRound.Selected = true;
            btnBoth.Selected = true;
            ObjActivity.Hidden = true;


            txtProgressView.Layer.BorderColor = UIColor.Gray.CGColor;
            txtProgressView.Layer.BorderWidth = (System.nfloat)1.0;



            // Keyboard popup
            NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.DidShowNotification, KeyBoardUpNotification);

            // Keyboard Down
            NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.WillHideNotification, KeyBoardDownNotification);


            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ModalInPopover = true;
        }


        public void KeyBoardUpNotification(NSNotification notification)
        {
            // get the keyboard size
            //var val = new NSValue(notification.UserInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey).Handle);
            //RectangleF r = val.RectangleFValue;

                var val = (NSValue)notification.UserInfo.ValueForKey(UIKeyboard.FrameBeginUserInfoKey);
                RectangleF r = val.RectangleFValue;
                // CGRect r = val.CGRectValue;


                // Find what opened the keyboard
                foreach (UIView view in this.View.Subviews)
                {
                    if (view.IsFirstResponder)
                        activeview = view;
                }
            if (activeview != null)
            {
                // Bottom of the controller = initial position + height + offset      
                bottom = ((float)(activeview.Frame.Y + activeview.Frame.Height));



                // Calculate how far we need to scroll
                //if (scrollamount == 0) {
                scrollamount = ((float)(r.Height - (View.Frame.Size.Height - bottom)));
                // }




                // Perform the scrolling
                if (scrollamount > 0)
                {
                    moveViewUp = true;
                    ScrollTheView(moveViewUp);


                }
                else
                {
                    moveViewUp = false;

                }
            }
           
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            if (moveViewUp) { ScrollTheView(false); }
        }

        private void ScrollTheView(bool move)
        {

            // scroll the view up or down
            UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
            UIView.SetAnimationDuration(0.3);

            RectangleF frame = (System.Drawing.RectangleF)View.Frame;

            if (move)
            {
                frame.Y -= scrollamount;
                frame.Height += 337;
            }
            else
            {
                frame.Y += scrollamount;
                scrollamount = 0;
            }

            View.Frame = frame;
            UIView.CommitAnimations();

        }

        partial void btnAllFltAttAction(Foundation.NSObject sender)
        {
           
            SelectedBases = new List<string>();
            List<string> listFlts = new List<string>();

            if (!((UIButton)sender).Selected) {

                for (int i = 1; i <= listDomicile.Count; i++)
                {
                    listFlts.Add(listBases[i - 1]);
                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = true;
                }
               ((UIButton)sender).BackgroundColor = UIColor.FromRGB((nfloat)(154.0 / 255.0), (nfloat)(154.0 / 255.0), (nfloat)(154.0 / 255.0));
                ((UIButton)sender).Selected = true;

            }
            else {
                for (int i = 1; i <= listDomicile.Count; i++)
                {
                    listFlts.Remove(listBases[i - 1]);
                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = false;
                }
                ((UIButton)sender).BackgroundColor = UIColor.FromRGB((nfloat)(67.0 / 255.0), (nfloat)(67.0 / 255.0), (nfloat)(67.0 / 255.0));
                ((UIButton)sender).Selected = false;
            }
            SelectedBases = listBases;
        }

        partial void btnAllPilotAction(Foundation.NSObject sender)
        {
            SelectedBases = new List<string>();
            List<string> listPilots = new List<string>();
            if (!((UIButton)sender).Selected)
            {
                for (int i = 1; i <= listDomicile.Count; i++)
                {
                    if (i == 2 || i == 6)
                    {
                        var view = this.View.ViewWithTag(i);
                        UIButton button = (UIButton)view.ViewWithTag(i);
                        button.Selected = false;

                    }
                    else
                    {
                        listPilots.Add(listBases[i - 1]);
                        var view = this.View.ViewWithTag(i);
                        UIButton button = (UIButton)view.ViewWithTag(i);
                        button.Selected = true;
                    }
                }

                ((UIButton)sender).BackgroundColor = UIColor.FromRGB((nfloat)(154.0 / 255.0), (nfloat)(154.0 / 255.0), (nfloat)(154.0 / 255.0));
                ((UIButton)sender).Selected = true;

            }
            else
            {
                for (int i = 1; i <= listDomicile.Count; i++)
                {
                    if (i == 2 || i == 6)
                    {
                        var view = this.View.ViewWithTag(i);
                        UIButton button = (UIButton)view.ViewWithTag(i);
                        button.Selected = false;

                    }
                    else
                    {
                        listPilots.Remove(listBases[i - 1]);
                        var view = this.View.ViewWithTag(i);
                        UIButton button = (UIButton)view.ViewWithTag(i);
                        button.Selected = false;
                    }
                }

                ((UIButton)sender).BackgroundColor = UIColor.FromRGB((nfloat)(67.0 / 255.0), (nfloat)(67.0 / 255.0), (nfloat)(67.0 / 255.0));
                ((UIButton)sender).Selected = false;
            }

                SelectedBases = listPilots;
        }

       
        partial void btnMonthAction(NSObject sender)
        {

            UIButton btn = (UIButton)sender;
            MonthsViewController ObjPickerr = new MonthsViewController();
            popoverController = new UIPopoverController(ObjPickerr);
            ObjPickerr.objpopover = popoverController;
            ObjPickerr.SuperParent = this;
            popoverController.PopoverContentSize = new CGSize(ObjPickerr.View.Frame.Width, ObjPickerr.View.Frame.Height);
            popoverController.PresentFromRect(btn.Frame, View, UIPopoverArrowDirection.Any, true); 
        }


        partial void btnPositionsClick(Foundation.NSObject sender) {

            for (int i = 21; i < 24; i++)
            {
                if (i == ((UIButton)sender).Tag)
                {
                    switch (i)
                    {
                        case 21:
                            Selectedposition = "Both";
                            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                            break;
                        case 22:
                            Selectedposition = "CP";
                            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                            break;
                        case 23:
                            Selectedposition = "FO";
                            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                            break;

                        default:
                            break;
                    }


                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = true;
                }
                else
                {
                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = false;
                }
            }

        }

      
        partial void btnRoundClick(Foundation.NSObject sender) {


            for (int i = 18; i < 20; i++)
            {
                if (i == ((UIButton)sender).Tag)
                {
                    switch (i)
                    {
                        case 18:
                            Selectedround = "1";
                            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                            break;
                        case 19:
                            Selectedround = "2";
                            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
                            break;

                        default:
                            break;
                    }
                  

                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = true;
                }
                else {
                    var view = this.View.ViewWithTag(i);
                    UIButton button = (UIButton)view.ViewWithTag(i);
                    button.Selected = false;
                }
            }

        }



        public void setMonthname(string month)
        {
            Selectedmonth = month;
            btnMonth.SetTitle(month, UIControlState.Normal);
        }

        public void setSelectedBases(int tag)
        {
            SelectedBases.Add(listBases[tag - 1]);
        }

        public void removeSelectedBases(int tag)
        {
            SelectedBases.Remove(listBases[tag - 1]);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void btnDownloadAction(Foundation.NSObject sender)
        {


            UserId = txtUserId.Text;
            Password = txtPassword.Text;
            UserId = "X21221";
           // Password = "Vofox2021-3";
            bool ISFA, ISPilot = false;
            ISFA = btnAllFA.Selected;
            ISPilot = btnAllPilot.Selected;
            string errorinfo = string.Empty;
            string Successinfo = string.Empty;
            bool isErrorOnDownload = false;
            new System.Threading.Thread(new System.Threading.ThreadStart(() => {



                InvokeOnMainThread(() => {
           
                    float centerX = (float)this.View.Frame.Width / 2;
                float centerY = (float)this.View.Frame.Height / 2;
                    ObjActivity.Hidden = false;
                ObjActivity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

                ObjActivity.StartAnimating();


                UserId = txtUserId.Text;
                Password = txtPassword.Text;
                
               

            });
           

                 UserId = "x21221";
               // Password = "Vofox2021-3";
                bool isAutneticated = checkAuthentication();
            if (isAutneticated)
            {

                var allpositions = WBidCollection.GetPositions().ToList();
                if (ISFA && !ISPilot)
                {
                    allpositions = allpositions.Where(x => x.LongStr == "FA").ToList();
                }
                else if (ISFA && ISPilot)
                {
                    if (Selectedposition == "Capt")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FO");
                    }
                    else if (Selectedposition == "FO")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "CP");
                    }
                }
                else if (ISPilot)
                {
                    if (Selectedposition == "Capt")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FO" || x.LongStr == "FA");
                    }
                    else if (Selectedposition == "FO")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "CP" || x.LongStr == "FA");
                    }
                    else
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FA");
                    }
                }
                    InvokeOnMainThread(() =>
                                            {
                                              
                                                downloadBidDataViewController objDownload = new downloadBidDataViewController();
                                                foreach (var position in allpositions)
                                                {
                                                    foreach (var domicile in SelectedBases)
                                                    {
                                                        AddlogDisplay("======================================");
                                                        //AddlogDisplay("Download started for " + domicile + " - " + position.LongStr + " - " + SelectedBidMonth.Period + " - " + SelectedBidRound.Round);
                                                        _downloadFileDetails = new DownloadInfo();

                                                        _downloadFileDetails.SessionCredentials = SessionCredential;
                                                        GlobalSettings.DownloadBidDetails = new BidDetails();
                                                        GlobalSettings.DownloadBidDetails.Month = WBidCollection.GetBidPeriods().FirstOrDefault(x => x.Period == Selectedmonth).BidPeriodId;
                                                        GlobalSettings.DownloadBidDetails.Domicile = domicile;
                                                        GlobalSettings.DownloadBidDetails.Postion = position.LongStr;
                                                        GlobalSettings.DownloadBidDetails.Round = (Selectedround == "1") ? "D" : "B";
                                                        GlobalSettings.DownloadBidDetails.Year = DateTime.Now.AddMonths(1).Year;
                                                        objDownload._downloadFileDetails = new DownloadInfo();
                                                        objDownload._downloadFileDetails.UserId= UserId;
                                                        bool isSuccess = objDownload.DownloadAndSaveBidDataFromWBid(false, true);
                                                        if (isSuccess == false)
                                                        {
                                                            isErrorOnDownload = true;
                                                            errorinfo += domicile + "- " + position.LongStr + "- Round" + Selectedround + "\r\n";
                                                        }
                                                        else
                                                        {
                                                            Successinfo += domicile + "- " + position.LongStr + "- Round" + Selectedround +"--"+DateTime.Now.ToShortTimeString()+ "\r\n";
                                                        }
                                                        //_downloadFileDetails.DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);
                                                        //DownloadDataFromSWA(_downloadFileDetails.DownloadList);
                                                    }
                                                }
                                                
                                            });
            }

         
            

                InvokeOnMainThread(() => {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("HandleReload", null);

                    string message = "Following datas were downloaded\n\r" + Successinfo + "\n\r\n\rFollowing datas were not downloaded\n\r" + errorinfo;
                        UIAlertController AlertController = UIAlertController.Create("Download Error", message, UIAlertControllerStyle.Alert);
                        AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {
                            ObjActivity.StopAnimating();
                            this.DismissViewController(true, null);
                        }));

                        this.PresentViewController(AlertController, true, null);
                    
                });

                InvokeOnMainThread(() =>
                {
                   
                });

            })).Start();
           
        }

        partial void btnCancel(NSObject sender)
        {
            this.DismissViewController(true,null);
        }


        private bool checkAuthentication()
        {
            if (Reachability.CheckVPSAvailable())
            {

                Authentication authentication = new Authentication();
                string authResult = authentication.CheckCredential(UserId, Password);
                if (authResult.Contains("ERROR: ") || authResult.Contains("Exception"))
                {
                    return false;
                }
                else
                {
                    SessionCredential = authResult;
                    return true;
                }
            }
            else
                return false;

        }
        private void StartDataDownload()
        {
            _downloadFileDetails.SessionCredentials = SessionCredential;
            _downloadFileDetails.DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);
        }
        private void DownloadDataFromSWA(List<string> downloadFiles)
        {
            try
            {
                DownloadInfo downloadinfo = new DownloadInfo();
                downloadinfo.SessionCredentials = SessionCredential;
                downloadinfo.UserId = UserId;
                downloadinfo.Password = Password;
                string zipFileName = downloadFiles.Where(x => x.Contains(".737")).FirstOrDefault();

                DownloadedFileInfo sWAFileInfo;
                List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();
                DownloadBid _downloadBidObject = new DownloadBid();
                string packetType = string.Empty;

                foreach (string filename in downloadFiles)
                {
                    sWAFileInfo = new DownloadedFileInfo();

                    AddlogDisplay("Downloading  " + filename + "...");
                    packetType = string.Empty;
                    //if length <10 or > 11 will add an error message  and continue to download next file. 
                    if (filename.Length < 10 || filename.Length > 11)
                    {
                        downloadedFileDetails.Add(new DownloadedFileInfo() { IsError = true, Message = "", FileName = filename });
                        continue;
                    }
                    //finding packet type
                    packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";

                    sWAFileInfo = _downloadBidObject.DownloadBidFile(downloadinfo, filename.ToUpper(), packetType);

                    if (sWAFileInfo.IsError && sWAFileInfo.FileName.Contains("737"))
                    {
                        AddlogDisplay("Data Transfer Failed for Domcile");
                        List<string> lstMessages = new List<string>();
                        lstMessages.Add("Bid Package Data");
                        if (sWAFileInfo.Message.Contains("BIDINFO DATA NOT AVAILABLE"))
                        {
                            AddlogDisplay("Data Transfer Failed for Domcile");
                            //MessageBox.Show("The Requested data doesnot exist on  the SWA servers  for Domcile " + bidDetails.Domicile + ". make sure proper month is selected and you are within the normal timeframe for the request");
                        }
                        else
                        {
                            AddlogDisplay("Data Transfer Failed for Domcile");
                            // MessageBox.Show("Data Transfer Failed for Domcile  " + bidDetails.Domicile);
                        }
                        return;
                    }
                    else
                    {
                        downloadedFileDetails.Add(sWAFileInfo);

                    }
                }

                foreach (DownloadedFileInfo sWAFile in downloadedFileDetails)
                {
                    if (sWAFile.FileName != null)
                    {
                        //If the file is error status, we dont need to save the file
                        if (sWAFile.IsError)
                        {
                            AddlogDisplay("Error File download " + sWAFile.FileName);
                            // Message("Error   " + sWAFile.FileName + " Download...");
                            continue;
                        }

                        FileStream fStream = new FileStream(Path.Combine(WBidHelper.GetAppDataPath(), sWAFile.FileName), FileMode.Create);
                        fStream.Write(sWAFile.byteArray, 0, sWAFile.byteArray.Length);
                        fStream.Dispose();
                        AddlogDisplay("Saved   " + sWAFile.FileName + "...");



                        //Extract tIhe zip file
                        if (Path.GetExtension(sWAFile.FileName) == ".737")
                        {
                            Extract737File(sWAFile.FileName);

                            //Delete the .737 file
                            string path = Path.Combine(WBidHelper.GetAppDataPath(), sWAFile.FileName);
                            //if (File.Exists(path))
                            // File.Delete(path);
                        }

                    }
                }


                DownloadedFileInfo zipFile = downloadedFileDetails.FirstOrDefault(x => x.FileName == zipFileName);
                if (zipFile.IsError)
                {
                    AddlogDisplay("The Requested data doesnot exist on  the SWA servers ");
                }
                else
                {

                    string path = WBidHelper.GetAppDataPath() + "/" + Path.GetFileNameWithoutExtension(zipFileName);

                    if (!(File.Exists(path + "/" + "TRIPS") && File.Exists(path + "/" + "PS")))
                    {
                        AddlogDisplay("There is an error while downloading the data. Please check your internet connection and try again ");
                    }
                    else
                    {
                        WBidHelper.SetCurrentBidInformationfromZipFileName(zipFileName, false);
                        AddlogDisplay("Started parsing of line and trip file ");
                        ParseData(zipFile.FileName);
                    }

                }
            }
            catch(Exception ex)
            {

            }
        }
        /// <summary>
        /// Extract the 737 file to trip and PS file
        /// </summary>
        /// <param name="fileName"></param>
        public void Extract737File(string fileName)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidMax";
                string folderPath = appDataPath + "/" + Path.GetFileNameWithoutExtension(fileName);
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);

                Directory.CreateDirectory(folderPath);

                string target = Path.Combine(appDataPath, appDataPath + "/" + Path.GetFileNameWithoutExtension(fileName)) + "/";
                string zipFile = Path.Combine(appDataPath, fileName);

                if (File.Exists(zipFile))
                {

                    ZipFile.ExtractToDirectory(zipFile, target);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void AddlogDisplay(string message)
        {
            LogData = LogData ?? new List<string>();
            LogData.Insert(0, message);


        }
        /// <summary>
        /// Parse the Downloaded Data.This will Parse trip file,line file etc.
        /// </summary>
        /// <param name="employeeNumber"></param>
        /// <param name="password"></param>
        /// <param name="zipFilename"></param>
        private void ParseData(string zipFilename)
        {

            GlobalSettings.ExtraErrorInfo += "INside Parse data\n<br>";
            try
            {
                //Prase Trip files

                trips = ParseTripFile(zipFilename);
                GlobalSettings.ExtraErrorInfo += "Parse  trip file completed\n<br>";
                if (trips != null)
                {

                    if (zipFilename.Substring(0, 1) == "A" && zipFilename.Substring(1, 1) == "B")
                    {
                        FASecondRoundParser fASecondRound = new FASecondRoundParser();
                        lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + zipFilename.Substring(0, 6).ToString() + "/PS", ref trips, GlobalSettings.FAReserveDayPay, zipFilename.Substring(2, 3));
                    }
                    else
                    {
                        lines = ParseLineFiles(zipFilename);
                    }
                    ParseDataExecution(zipFilename);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        void ParseDataExecution(string zipFilename)
        {
            //InvokeInBackground(() =>
            //{
            try
            {

                //bid round is second round
                if (GlobalSettings.CurrentBidDetails.Round == "S")
                {
                    //Finding if any missed trip exists
                    List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
                    var pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

                    //Checking any missed trip  exist
                    if (pairingwHasNoDetails.Count > 0)
                    {

                        try
                        {
                            if (GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData)
                            {
                                List<string> temppairingwHasNoDetails = new List<string>();
                                bool isscrapRequired = true;
                                MonthlyBidDetails biddetails = new MonthlyBidDetails();
                                biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                                biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                                biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                                biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                                biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                                var missedtrips = WBidHelper.GetMissingtripFromVPS(biddetails);
                                if (missedtrips.Count() >= pairingwHasNoDetails.Count())
                                {

                                    var temptrips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                                    temppairingwHasNoDetails = allPair.Where(x => !temptrips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x))).ToList();
                                    if (temppairingwHasNoDetails.Count == 0)
                                    {
                                        trips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                                        isscrapRequired = false;
                                    }

                                }

                                if (isscrapRequired)
                                {
                                                              
                                    if (GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(-1).Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(1).Month)
                                    {

                                        GlobalSettings.parsedDict = new Dictionary<string, Trip>();


                                        string password = (GlobalSettings.buddyBidTest) ? GlobalSettings.QAScrapPassword : _downloadFileDetails.Password;
                                        scrap(_downloadFileDetails.UserId, password, pairingwHasNoDetails, GlobalSettings.DownloadBidDetails.Month, GlobalSettings.DownloadBidDetails.Year, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay);

                                        if (GlobalSettings.parsedDict == null || GlobalSettings.parsedDict.Count == 0)
                                        {

                                            IsMissingTripFailed = true;
                                            string bidFileName = string.Empty;
                                            bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                            BidLineParser bidLineParser = new BidLineParser();
                                            var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                            trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                            TripExecution(zipFilename);

                                            return;
                                        }
                                        else
                                        {
                                            //IsMissingTripFailed = true;
                                            trips = trips.Concat(GlobalSettings.parsedDict).ToDictionary(pair => pair.Key, pair => pair.Value);
                                        }

                                    }
                                    else
                                    {

                                        string bidFileName = string.Empty;
                                        IsMissingTripFailed = true;
                                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                        BidLineParser bidLineParser = new BidLineParser();
                                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                        trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);

                                    }
                                }
                            }
                            else
                            {
                                IsMissingTripFailed = true;
                                string bidFileName = string.Empty;
                                bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                BidLineParser bidLineParser = new BidLineParser();
                                var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);

                            }
                        }
                        catch (Exception ex)
                        {

                            IsMissingTripFailed = true;
                            string bidFileName = string.Empty;
                            bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                            BidLineParser bidLineParser = new BidLineParser();
                            var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                            trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                            TripExecution(zipFilename);

                            return;
                        }
                    }
                }
                TripExecution(zipFilename);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            // });
        }
        void TripExecution(string zipFilename)
        {

            //InvokeInBackground(() =>
            //{
                try
                {

                    List<CityPair> ListCityPair = TripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
                    GlobalSettings.TtpCityPairs = ListCityPair;

                    // Additional processing needs to be done to FA trips before CalculateTripPropertyValues
                    if (zipFilename.Substring(0, 1) == "A")
                        CalculateTripProperties.PreProcessFaTrips(trips, ListCityPair);

                    CalculateTripProperties.CalculateTripPropertyValues(trips, ListCityPair);

                    try
                    {
                        CalculateLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);
                    }
                    catch (Exception ex)
                    {

                    }
                    SaveParsedFiles(trips, lines);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
           // });

        }
        /// <summary>
        /// Parse Trip Files
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Dictionary<string, Trip> ParseTripFile(string fileName)
        {

            Dictionary<string, Trip> Trips = new Dictionary<string, Trip>();
            try
            {
                TripParser tripParser = new TripParser();
                string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6).ToString() + "/TRIPS";
                byte[] byteArray = File.ReadAllBytes(filePath);

                DateTime[] dSTProperties = DSTProperties.SetDSTProperties();
                if (dSTProperties[0] != null && dSTProperties[0] != DateTime.MinValue)
                {
                    GlobalSettings.FirstDayOfDST = dSTProperties[0];
                }
                if (dSTProperties[1] != null && dSTProperties[1] != DateTime.MinValue)
                {
                    GlobalSettings.LastDayOfDST = dSTProperties[1];
                }
                //WBidHelper.SetDSTProperties();
                Trips = tripParser.ParseTrips(fileName, byteArray, GlobalSettings.FirstDayOfDST, GlobalSettings.LastDayOfDST);
            }
            catch (Exception ex)
            {

                throw;
            }
            return Trips;
        }

        private Dictionary<string, Line> ParseLineFiles(string fileName)
        {
            Dictionary<string, Line> Lines = new Dictionary<string, Line>();
            LineParser lineParser = new LineParser();
            string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6).ToString() + "/PS";
            byte[] byteArray = File.ReadAllBytes(filePath);
            Lines = lineParser.ParseLines(fileName, byteArray);
            return Lines;
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

                    WBidIntialState wbidintialState = null;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception ex)
                    {
                        WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                        try
                        {
                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        }
                        catch (Exception exx)
                        {

                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());

                            obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0","");

                        }
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());

                        obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0","");
                        //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");

                    }
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    if (GlobalSettings.isHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = "Original";

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                try
                {
                    //Read the state file object and store it to global settings.
                    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                }
                catch (Exception ex)
                {

                    //Recreate state file
                    //--------------------------------------------------------------------------------
                    WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");
                    WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                    obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0","");
                    if (GlobalSettings.isHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = "Original";
                }

                GlobalSettings.WBidStateCollection.DataSource = "Original";
            }

            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBIdStateContent.MenuBarButtonState.IsMIL = false;


            wBIdStateContent.IsOverlapCorrection = GlobalSettings.IsOverlapCorrection;


            GlobalSettings.WBidStateCollection.CompanyVA = GlobalSettings.CompanyVA;

            if (IsMissingTripFailed)
            {
                wBIdStateContent.IsMissingTripFailed = true;
            }
            else
            {
                wBIdStateContent.IsMissingTripFailed = false;
            }

            //GlobalSettings.WBidStateCollection.IsModified = true;
            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

            //WBidHelper.GenerateDynamicOverNightCitiesList();

            //  GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
            GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList(); var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

            if (wBIdStateContent.CxWtState.AMPMMIX == null)
                wBIdStateContent.CxWtState.AMPMMIX = new AMPMConstriants();
            if (wBIdStateContent.CxWtState.FaPosition == null)
                wBIdStateContent.CxWtState.FaPosition = new PostionConstraint();
            if (wBIdStateContent.CxWtState.TripLength == null)
                wBIdStateContent.CxWtState.TripLength = new TripLengthConstraints();
            if (wBIdStateContent.CxWtState.DaysOfWeek == null)
                wBIdStateContent.CxWtState.DaysOfWeek = new DaysOfWeekConstraints();
            if (wBIdStateContent.Constraints.DaysOfMonth == null)
                wBIdStateContent.Constraints.DaysOfMonth = new DaysOfMonthCx() { };


            if (wBIdStateContent.Weights.NormalizeDaysOff == null)
            {
                wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter() { Type = 1, Weight = 0 };

            }
            if (wBIdStateContent.CxWtState.NormalizeDays == null)
            {
                wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

            }

            StateManagement statemanagement = new StateManagement();
            //statemanagement.ReloadDataFromStateFile();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
            //Setting  status to Global variables
            statemanagement.SetVacationOrOverlapExists(wBidStateContent);
            //St the line order based on the state file.
            statemanagement.ReloadStateContent(wBidStateContent);


            SortCalculation sort = new SortCalculation();


            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }

        }
        private void scrap(string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
        {
            GlobalSettings.IsScrapStart = true;

            InvokeOnMainThread(() =>
                {
                    if (userName == "x21221")
                    {
                        ContractorEmpScrap scrap = new ContractorEmpScrap(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
                        this.AddChildViewController(scrap);
                        scrap.View.Hidden = true;

                        this.View.AddSubview(scrap.View);
                    }
                    else
                    {
                        webView scrapper = new webView(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
                        this.AddChildViewController(scrapper);
                        scrapper.View.Hidden = true;

                        this.View.AddSubview(scrapper.View);
                    }
                });

            while (GlobalSettings.IsScrapStart)
            {
            };
        }

       
    }
}

