using System;
using CoreGraphics;
using Foundation;
using UIKit;
using iOSPasswordStorage;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Linq;
using System.IO;
using Security;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidiPad.iOS
{
    public partial class RetrieveAwardViewController : UIViewController
    {
		//NSObject notif;
        LoadingOverlay overlay;
        WBidDataDwonloadAuthServiceClient client;
        private DownloadInfo _downloadFileDetails;
        private string _sessionCredentials = string.Empty;
        public string domicileName;
		public UIPopoverController popoverController;
		public MyPopDelegate objPopDelegate;
        public RetrieveAwardViewController()
            : base("RetrieveAwardViewController", null)
        {
        }
		public override void ViewWillDisappear(bool animated)
		{

			base.ViewWillDisappear(animated);
			

		}
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            overlay = new LoadingOverlay(this.View.Frame, "Retrieving Award. Please wait...");
            BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
            client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
            //  client.GetAuthorizationDetailsCompleted += client_GetAuthorizationDetailsCompleted;
            client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
            // Perform any additional setup after loading the view, typically from a nib.
            pckrDomicilePick.Model = new pickerViewModel(this);
            int indexToSelect = GlobalSettings.WBidINIContent.Domiciles.IndexOf(GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile));
            pckrDomicilePick.Select(indexToSelect, 0, true);
            domicileName = GlobalSettings.CurrentBidDetails.Domicile;
            if (DateTime.Now.Day <= 19)

                sgAwards.SelectedSegment = 0;
            else
                sgAwards.SelectedSegment = 1;

            if (GlobalSettings.CurrentBidDetails.Postion == "CP")
                sgPosition.SelectedSegment = 0;
            else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
                sgPosition.SelectedSegment = 1;
            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                sgPosition.SelectedSegment = 2;

        }


        //		partial void btnSelectDomicleTapped (MonoTouch.Foundation.NSObject sender)
        //		{
        //			UIButton btn = (UIButton)sender;
        //			string[] arr = GlobalSettings.WBidINIContent.Domiciles.Select(x=>x.DomicileName).ToArray();
        //			UIActionSheet sheet = new UIActionSheet("Select Domicile",null,"Cancel",null,arr);
        //			sheet.ShowFrom(btn.Frame,this.View,true);
        //			sheet.Clicked += handleDomicleSelect;
        //		}
        //		void handleDomicleSelect (object sender, UIButtonEventArgs e)
        //		{
        //			List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles;
        //			btnSelectedDomicile.SetTitle (listDomicile [e.ButtonIndex].DomicileName, UIControlState.Normal);
        //
        //			if (e.ButtonIndex == 0) {
        //				//listdomicle[]
        //			}
        //
        //		}


        public class pickerViewModel : UIPickerViewModel
        {
            RetrieveAwardViewController parent;
            public pickerViewModel(RetrieveAwardViewController parentVC)
            {
                parent = parentVC;
            }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView picker, nint component)
            {
                string[] arr = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
                return arr.Count();
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray()[row];
            }
            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                parent.domicileName = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray()[row];
            }
        }



        partial void btnAwardsTapped(Foundation.NSObject sender)
        {

            loginViewController login = new loginViewController();
            login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(login, true, () =>
                {
					CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), loginCredetialsEntered);
                });

        }
        public void loginCredetialsEntered(NSNotification n)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    this.View.AddSubview(overlay);

                });
                InvokeInBackground(() =>
                {
                    InitiateDownloadProcess();

                });
				NSNotificationCenter.DefaultCenter.RemoveObserver(CommonClass.bidObserver);
            }
            catch (Exception ex)
            {

                InvokeOnMainThread(() =>
                                {

                                    throw ex;
                                });
            }
        }



        /// <summary>
        /// Create the Filename for the Award file based on teh UI selection
        /// </summary>
        private void GenarateAwardFileName()
        {
            List<string> downLoadList = new List<string>();
            try
            {

                if (sgPosition.SelectedSegment == 0)
                {
                    string fileName = domicileName + "CP" + ((sgAwards.SelectedSegment == 0) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                    fileName = domicileName + "FO" + ((sgAwards.SelectedSegment == 0) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                }
                else if (sgPosition.SelectedSegment == 1)
                {
                    string fileName = domicileName + "FO" + ((sgAwards.SelectedSegment == 0) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                    fileName = domicileName + "CP" + ((sgAwards.SelectedSegment == 0) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                }
                else if (sgPosition.SelectedSegment == 2)
                {
                    string fileName = domicileName + "FA" + ((sgAwards.SelectedSegment == 0) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                }
                else
                {
                    return;
                }
                _downloadFileDetails.DownloadList = downLoadList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		void showTimeOutAlert()
		{
			//-----
			TimeOutAlertView regs = new TimeOutAlertView();
			popoverController = new UIPopoverController(regs);
			objPopDelegate = new MyPopDelegate(this);
			objPopDelegate.CanDismiss = false;
			popoverController.Delegate = objPopDelegate;
			regs.objRetriveAwards = this;
			regs.objpopover = popoverController;

			CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 175, 150, 350);
			popoverController.PopoverContentSize = new CGSize(regs.View.Frame.Width, regs.View.Frame.Height);
			popoverController.PresentFromRect(frame, View, 0, true);

			//------
		}

		public void DismissView()
		{
			InvokeOnMainThread(() =>
					   {
							
						   overlay.RemoveFromSuperview();
						   this.DismissViewController(true, null);
					   });
		}
        /// <summary>
        /// Donwload the awards and show it ot the file viewer.
        /// </summary>
        private void AwardDownlaod()
        {
            try
            {
                DownloadAward downloadAward = new SharedLibrary.SWA.DownloadAward();
                List<DownloadedFileInfo> lstDownloadedFiles = downloadAward.DownloadAwardDetails(_downloadFileDetails);
                if (lstDownloadedFiles != null)
                {
                    if (lstDownloadedFiles[0].IsError)
                    {
                        List<string> lstMessages = new List<string>();

                        InvokeOnMainThread(() =>
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            overlay.RemoveFromSuperview();

                        });
                    }
                    else
                    {
                        bool isNeedtoShowFileViewer = true;
                        foreach (DownloadedFileInfo fileinfo in lstDownloadedFiles)
                        {
                            FileStream fStream = new FileStream(Path.Combine(WBidHelper.GetAppDataPath(), fileinfo.FileName), FileMode.Create);
                            fStream.Write(fileinfo.byteArray, 0, fileinfo.byteArray.Length);
                            fStream.Dispose();
                        }
                        try
                        {

                            var filename = lstDownloadedFiles[0].FileName;
                            if (filename.Substring(5, 1) == "M")
                            {
                                UserBidDetails biddetails = new UserBidDetails();
                                biddetails.Domicile = filename.Substring(0, 3);
                                biddetails.Position = filename.Substring(3, 2);
                                biddetails.Round = filename.Substring(5, 1) == "M" ? 1 : 2;
                                biddetails.Year = DateTime.Now.AddMonths(1).Year;
                                biddetails.Month = DateTime.Now.AddMonths(1).Month;


                                if (GlobalSettings.IsDifferentUser)
                                {
                                    biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0'), @"\d+").Value);
                                }
                                else
                                {
                                    biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                                }
                                string alertmessage = WBidHelper.GetAwardAlert(biddetails);
                                if (alertmessage != string.Empty)
                                {
                                    isNeedtoShowFileViewer = false;
                                    alertmessage = alertmessage.Insert(0, "\n\n");
                                    alertmessage += "\n\n";
                                    InvokeOnMainThread(() =>
                                    {
                                        UIAlertController AlertController = UIAlertController.Create("WBidMax", alertmessage, UIAlertControllerStyle.Alert);
                                        //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                        AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                                        {
                                            //ProcessAfterSeniorityListParse();
                                            webPrint fileViewer = new webPrint();
                                            fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                            this.PresentViewController(fileViewer, true, () =>
                                            {
                                                fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                                            });
                                        }));
                                        this.PresentViewController(AlertController, true, null);
                                        overlay.RemoveFromSuperview();

                                        //webPrint fileViewer = new webPrint();
                                        //fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                        //this.PresentViewController(fileViewer, true, () =>
                                        //{
                                        //    fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                                        //});
                                    });

                                }
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                        if (isNeedtoShowFileViewer)
                        {
                            InvokeOnMainThread(() =>
                        {
                            webPrint fileViewer = new webPrint();
                            fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            this.PresentViewController(fileViewer, true, () =>
                            {
                                fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                            });

                        });
                        }
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Please try again after some time", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            overlay.RemoveFromSuperview();

                        });
                }
                InvokeOnMainThread(() =>
                {
                    overlay.RemoveFromSuperview();

                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// start the download process ..
        /// </summary>
        public void InitiateDownloadProcess()
        {

		
            try
            {

                _downloadFileDetails = new DownloadInfo();
                _downloadFileDetails.UserId = KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
                _downloadFileDetails.Password = KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);

                //checking  the internet connection available
                //==================================================================================================================
                if (Reachability.CheckVPSAvailable())
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
                    //checking CWA credential
                    //==================================================================================================================

                    //this.startProgress();
                    Authentication authentication = new Authentication();
                    string authResult = authentication.CheckCredential(_downloadFileDetails.UserId, _downloadFileDetails.Password);
                    if (authResult.Contains("ERROR: "))
                    {
						WBidLogEvent obj = new WBidLogEvent();
						obj.LogBadPasswordUsage(_downloadFileDetails.UserId, false, authResult);
                        InvokeOnMainThread(() =>
                        {
							KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);

                            CustomAlertView customAlert = new CustomAlertView();
                            UINavigationController nav = new UINavigationController(customAlert);
                            nav.NavigationBarHidden = true;
                            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            customAlert.objRetriveAwards = this;
                            customAlert.AlertType = "InvalidCredential";
                            this.PresentViewController(nav, true, null);

                            //UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Invalid Username or Password", UIAlertControllerStyle.Alert);
                            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            //this.PresentViewController(okAlertController, true, null);
                            //overlay.RemoveFromSuperview();

                        });
                    }
                    else if (authResult.Contains("Exception"))
                    {
                        InvokeOnMainThread(() =>
                        {
							

							showTimeOutAlert();
                            //overlay.RemoveFromSuperview();
                            //this.DismissViewController(true, null);
                        });
                    }
                    else
                    {
                        NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
                        // this.startProgress();

                        _sessionCredentials = authResult;

                        ClientRequestModel clientRequestModel = new ClientRequestModel();
                        clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
                        clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                        clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
						clientRequestModel.OperatingSystem = "iPad OS";
                        clientRequestModel.RequestType = (int)RequestTypes.DownloadAward;
                        clientRequestModel.Platform = "iPad";
                        clientRequestModel.Token = new Guid();
                        clientRequestModel.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                        client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        //UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Connectivity not available", UIAlertControllerStyle.Alert);
                        //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        //this.PresentViewController(okAlertController, true, null);
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
                        this.DismissViewController(true, null);
                    });
                }
            }
            catch (Exception ex)
            {



                InvokeOnMainThread(() =>
                    {

                        throw ex;
                    });
            }
            //}
        }
        partial void btnCancelTapped(Foundation.NSObject sender)
        {
            this.DismissViewController(true, null);
            foreach (UIView view in this.View.Subviews) {

                DisposeClass.DisposeEx (view);
            }
        }
        #region WCF CompletedEvent
        private void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
        {
            try
            {

                if (e.Result != null)
                {
                    ServiceResponseModel serviceResponseModel = e.Result;

                    if (serviceResponseModel.IsAuthorized)
                    {
                        NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);
                        //this.startProgress();
                        _downloadFileDetails.SessionCredentials = _sessionCredentials;
                        InvokeOnMainThread(() =>
                        {
                            GenarateAwardFileName();
                        });
                        AwardDownlaod();
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Error", serviceResponseModel.Message, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            overlay.RemoveFromSuperview();

                        });
                    }
                }
            }
            catch (Exception ex)
            {

                InvokeOnMainThread(() =>
                    {

                        throw ex;
                    });
            }
        }

		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			RetrieveAwardViewController _parent;
			public bool CanDismiss;
			public MyPopDelegate(RetrieveAwardViewController parent)
			{
				_parent = parent;
			}

			public override bool ShouldDismiss(UIPopoverController popoverController)
			{
				if (CanDismiss)
				{
					return true;
				}
				else {

					return false;
				}
			}
		}
        #endregion
    }
}

