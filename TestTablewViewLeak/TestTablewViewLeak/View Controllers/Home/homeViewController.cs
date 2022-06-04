using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary;
using System.Linq;
using WBid.WBidiPad.Core;
using System.Collections.Generic;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.ServiceModel;
using System.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace WBid.WBidiPad.iOS
{
	public partial class homeViewController : UIViewController , IServiceDelegate
	{
		bidDataCollectionController collectionVw;
		NSObject notif;
		NSObject notif2;
		NSObject notif3;
		NSObject notifNavigateToLineView;
		getNewBidPeriodViewController newBid;
		WBidDataDwonloadAuthServiceClient client;
		LoadingOverlay ActivityIndicator;
		public UIPopoverController	popoverController;
		public MyPopDelegate objPopDelegate ;
        string subscriptionMessageContent = string.Empty;
		string WebType;
		string empnumber;
		OdataBuilder ObjOdata;
		public homeViewController () : base ("homeViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
            //txtCellNumber.Dispose ();

            //			foreach (UIView view in this.View.Subviews) {
            //
            //				DisposeClass.DisposeEx(view);
            //			}
            //			//this.View.Dispose ();

        }
		public void DimissDownloadView(NSNotification notification)
		{
			if (newBid != null)
				newBid.DismissViewController(true, null);
		}
		public void NavigateToLineView(NSNotification notification)
		{
			if(newBid != null )
			newBid.DismissViewController (true, null);
			

			PerformSelector(new ObjCRuntime.Selector("NavigateToLineView2"), null, 0.4);

		}
		[Export("NavigateToLineView2")]
		void NavigateToLineView2()
		{
						

			if (notif != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
				notif = null;
			}
			if (notif2 != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notif2);
				//notif2 = null;
			}
			if (notifNavigateToLineView != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notifNavigateToLineView);
				notifNavigateToLineView = null;
			}

			//lineViewController lineController = new lineViewController();
			//CommonClass.lineVC = lineController;
			//NavigationController.PushViewController (lineController, true);

			UIStoryboard storyboard = UIStoryboard.FromName("LineStoryboardView", null);
			lineViewController lineview = storyboard.InstantiateViewController("lineviewcontroller") as lineViewController;
			CommonClass.lineVC = lineview;
			this.NavigationController.PushViewController(lineview, true);
		}

		void Callback(NSNotification notification)
		{
			//			//bool isPresented =UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.IsBeingPresented;
			//			if (!(UIApplication.SharedApplication.KeyWindow.RootViewController.GetType() == typeof(homeViewController) || UIApplication.SharedApplication.KeyWindow.RootViewController.GetType() == typeof(lineViewController) )) {
			//				return;
			//			}

			if (Reachability.CheckVPSAvailable())
			{
				var value = NSUserDefaults.StandardUserDefaults["First"];
				if (value != null)
				{
					UserAccountChecking();
				}
				GetApplicationLoadDataFromServer();


			}

			// DisplaySubscriptionAlert();
			NSNotificationCenter.DefaultCenter.RemoveObserver(notification);
		}

        private void DisplaySubscriptionAlert()
        {
            

            bool isAppInReviewMode = (subscriptionMessageContent == "Subscription Expired") ? true : false;
            //Console.WriteLine("Received a notification UIApplication", notification);
            bool isSubScriptionOnlyFor5Days = CommonClass.isSubScriptionOnlyFor5Days();
            bool IsUserdataAvailable = CommonClass.isUserInformationAvailable();
            if (IsUserdataAvailable)
            {

                if (GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsFree)
                    return;

                DateTime PaidUntilDate = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now;
                int day = CommonClass.DaysBetween(DateTime.Now, PaidUntilDate);
                if (isSubScriptionOnlyFor5Days)
                {
                    string message = "";
                    if (day == 1)
                        message = "Your subscription will expire in 1 day";
                    else
                        message = "Your subscription will expire in " + day + " days.";

                    UIAlertController alert = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);


                    if (isAppInReviewMode == false)
                    {
                        message += " Go to www.wbidmax.com and resubscribe";
                        alert.AddAction(UIAlertAction.Create("Go to Wbidmax.com", UIAlertActionStyle.Default, (actionOK) =>
                        {
                            NSUrlRequest request = new NSUrlRequest(new NSUrl("http://www.wbidmax.com"));
                            UIApplication.SharedApplication.OpenUrl(request.Url);
                        }));
                    }
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                    {

                    }));

                    this.PresentViewController(alert, true, null);

                }
                else if (day < 1)
                {
                    //UIAlertController alert = UIAlertController.Create("WBidMax", "Your subscription expired", UIAlertControllerStyle.Alert);
                    //alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {

                    //}));

                    //alert.AddAction(UIAlertAction.Create("Go To Subscription", UIAlertActionStyle.Default, (actionOK) => {
                    //    SubscriptionViewController ObjSubscription = new SubscriptionViewController();
                    //    ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    //    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(ObjSubscription, true, null);
                    //}));
                    subscriptionMessageContent = (subscriptionMessageContent == string.Empty) ? "Your subscription expired" : subscriptionMessageContent;

                    UIAlertController alert = UIAlertController.Create("WBidMax", subscriptionMessageContent, UIAlertControllerStyle.Alert);

                    if (isAppInReviewMode == false)
                    {

                        alert.AddAction(UIAlertAction.Create("Go to Wbidmax.com", UIAlertActionStyle.Default, (actionOK) =>
                        {
                            NSUrlRequest request = new NSUrlRequest(new NSUrl("http://www.wbidmax.com"));
                            UIApplication.SharedApplication.OpenUrl(request.Url);
                        }));
                        alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {

                        }));
                    }
                    else
                    {
                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {

                        }));
                    }



                    this.PresentViewController(alert, true, null);


                }
               // NSNotificationCenter.DefaultCenter.RemoveObserver(notification);
            }
        }

        public void NavigateToBA()
		{
			BAViewController baController = new BAViewController ();
			//CommonClass.baVC = baController;
			UINavigationController navController = new UINavigationController (baController);
			navController.NavigationBar.BarStyle = UIBarStyle.Black;
			navController.NavigationBar.Hidden = true;
			this.PresentViewController (navController, true, null);
		}

        void ChangeServerName (NSNotification notifications)
        {
            if (CommonClass.isVPSServer == "FALSE") {
                NavigationBar.TopItem.Title = "WBidMax - Home (" + NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString () + ")(VOFOX SERVER)";
            } else {
                NavigationBar.TopItem.Title = "WBidMax - Home (" + NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString () + ")";
            }
            NSNotificationCenter.DefaultCenter.RemoveObserver (notifications);
        }


		public override void ViewDidLoad ()
		{
			////for testing Roshil
   //         TestTablewViewLeak.ViewControllers.DownloadBidData.downloadBidDataViewController1 obj = new TestTablewViewLeak.ViewControllers.DownloadBidData.downloadBidDataViewController1();
			//obj.DownloadBidData();
			////End test code

			base.ViewDidLoad ();
			this.observeNotification ();

			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification, Callback);
           	// Perform any additional setup after loading the view, typically from a nib.
			NavigationBar.TopItem.Title = "WBidMax - Home (" + NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString () + ")";
			LoadingOverlay ActivityIndicator;


		


			// Saving additional header colunmns.
			LineSummaryBL.GetAdditionalColumns ();

			LineSummaryBL.GetAdditionalVacationColumns ();

			// Saving additional bidline colunmns.
			LineSummaryBL.GetBidlineViewAdditionalColumns ();

			// Saving additional modern colunmns.
			LineSummaryBL.GetModernViewAdditionalColumns ();

			LineSummaryBL.GetBidlineViewAdditionalVacationColumns ();

			// Saving additional modern colunmns.
			LineSummaryBL.GetModernViewAdditionalVacationalColumns ();

			LineSummaryBL.SetSelectedBidLineColumnstoGlobalList ();

			LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList ();

			LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList ();

			LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList ();


			

			//CommonClass.bidLineProperties = new List<string>() {
			//    "Pay",
			//    "PDiem",
			//    "Flt",
			//    "Off",
			//    "+Off"
			//};

			//			var tap = new UITapGestureRecognizer (secretTap);
			//			tap.NumberOfTouchesRequired = 3; // Change this value to 3 before submitting!
			//			this.View.AddGestureRecognizer (tap);

			//			List<string> ss = new List<string> () { "AP1125", "AP1301"};
			//			scrap ("e22028", "Vofox2015-2", ss, 6, 2015, 60, 30);

			//			try {
			//				int i = 0;
			//				i = 1 / i;
			//			} catch (Exception ex) {
			//				throw ex;
			//			}


			getAvaibleHistoryList ();
			//temporary to test the wifi test feature
			//GlobalSettings.IsWifiTestOn = true;
			//checks whether the secret Wifi  swich is on or off
			//if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest== false) 
			//{
            if (Reachability.CheckVPSAvailable ()) 
				{
					OfflineEventChecking ();
					OfflinePaymentChecking ();
					var value = NSUserDefaults.StandardUserDefaults["First"];
					if (value != null) {
						UserAccountChecking ();
					}
				GetApplicationLoadDataFromServer();
			}
				else 
				{
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
				}
			//}
			//else
			//{
            //    UIAlertController okAlertController = UIAlertController.Create("WBidMax", GlobalSettings.SouthWestWifiMessage, UIAlertControllerStyle.Alert);
            //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //    this.PresentViewController(okAlertController, true, null);
            //}

   //         bool isSubScriptionOnlyFor5Days=	CommonClass.isSubScriptionOnlyFor5Days ();

   //         bool isAppInReviewMode = (subscriptionMessageContent == "Subscription Expired") ? true : false;

			//bool IsUserdataAvailable=	CommonClass.isUserInformationAvailable ();
			//if (IsUserdataAvailable) {
			//	if (GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed)
			//		return;
                
			//	DateTime PaidUntilDate = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now;
			//	int day = CommonClass.DaysBetween (DateTime.Now, PaidUntilDate);
   //              if (isSubScriptionOnlyFor5Days) {
			//		string message = "";
			//		if (day == 1)
			//			message = "Your subscription will expire in 1 day";
			//		else
			//			message = "Your subscription will expire in " + day + " days.";

			//	    UIAlertController alert = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);


   //                 if (isAppInReviewMode == false)
   //                 {
   //                     alert.AddAction(UIAlertAction.Create("Go to Wbidmax.com", UIAlertActionStyle.Default, (actionOK) => {
   //                         NSUrlRequest request = new NSUrlRequest(new NSUrl("http://www.wbidmax.com"));
   //                         UIApplication.SharedApplication.OpenUrl(request.Url);
   //                     }));
   //                 }
   //                 alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

   //                 }));
   //                 //alert.AddAction(UIAlertAction.Create("Go To Subscription", UIAlertActionStyle.Default, (actionOK) => {
   //                 //    SubscriptionViewController ObjSubscription = new SubscriptionViewController();
   //                 //    ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
   //                 //    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(ObjSubscription, true, null);
   //                 //}));

   //                 this.PresentViewController(alert, true, null);

   //             } else if (day < 1) {

   //                 subscriptionMessageContent = (subscriptionMessageContent == string.Empty) ? "Your subscription expired" : subscriptionMessageContent;
   //                 //UIAlertController alert = UIAlertController.Create("WBidMax", "Your subscription expired", UIAlertControllerStyle.Alert);
   //                 //alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {

   //                 //}));
   //                 UIAlertController alert = UIAlertController.Create("WBidMax", subscriptionMessageContent, UIAlertControllerStyle.Alert);

   //                 if (isAppInReviewMode == false)
   //                 {
                       
   //                     alert.AddAction(UIAlertAction.Create("Go to Wbidmax.com", UIAlertActionStyle.Default, (actionOK) => {
   //                         NSUrlRequest request = new NSUrlRequest(new NSUrl("http://www.wbidmax.com"));
   //                         UIApplication.SharedApplication.OpenUrl(request.Url);
   //                     }));
   //                     alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

   //                     }));
   //                 }
   //                 else
   //                 {
   //                     alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {

   //                     }));
   //                 }
                   

   //                 //alert.AddAction(UIAlertAction.Create("Go To Subscription", UIAlertActionStyle.Default, (actionOK) => {
   //                 //    SubscriptionViewController ObjSubscription = new SubscriptionViewController();
   //                 //    ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
   //                 //    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(ObjSubscription, true, null);
   //                 //}));

   //                 this.PresentViewController(alert, true, null);
   //             }
			//}
		
		}
		private void GetApplicationLoadDataFromServer()
		{
			InvokeInBackground(() =>
			{
				ObjOdata = new OdataBuilder();
				ApplicationData info = new ApplicationData();
				ObjOdata.RestService.Objdelegate = this;
				info.FromApp = (int)AppNum.WBidMaxApp;
				WebType = "ApplicationLoadDataFromServer";
				ObjOdata.GetApplicationLoadData(info);
			});
		}
		private void OfflinePaymentChecking()
		{
			try {
				if (File.Exists (WBidHelper.GetWBidOfflinePaymentFilePath ())) {

					PaymentUpdateModel paymentUpdateModel = XmlHelper.DeserializeFromXml<PaymentUpdateModel> (WBidHelper.GetWBidOfflinePaymentFilePath ());
					OdataBuilder ObjOdata = new OdataBuilder ();
					ObjOdata.RestService.Objdelegate = this;
					WebType="UpdateSubscriptionDate";
					ObjOdata.UpdateSubscriptionDate (paymentUpdateModel);

				}

			} catch (Exception ex) {

			}
		}
		private void OfflineEventChecking()
		{
			if (File.Exists (WBidHelper.WBidOfflineEventFilePath))
			{
				GlobalSettings.OfflineEvents = (OfflineEvents)XmlHelper.DeserializeFromXml<OfflineEvents> (WBidHelper.WBidOfflineEventFilePath);
				if (GlobalSettings.OfflineEvents.EventLogs.Count > 0) 
				{
					WBidDataDownloadAuthorizationService.Model.OffLineEvents offlineEvents = new WBidDataDownloadAuthorizationService.Model.OffLineEvents ();
					offlineEvents.EventLogs = new List<WBidDataDownloadAuthorizationService.Model.LogData> ();
					foreach (var item in GlobalSettings.OfflineEvents.EventLogs) 
					{

						WBidDataDownloadAuthorizationService.Model.LogData logdata= new WBidDataDownloadAuthorizationService.Model.LogData ();
						logdata.date = item.date;
						logdata.LogDetails = new WBidDataDownloadAuthorizationService.Model.LogDetails ();
						logdata.LogDetails.Base = item.LogDetails.Base;
						logdata.LogDetails.BidForEmpNum = item.LogDetails.BidForEmpNum;
						logdata.LogDetails.BuddyBid1 = item.LogDetails.BuddyBid1;
						logdata.LogDetails.BuddyBid2 = item.LogDetails.BuddyBid2;
						logdata.LogDetails.EmployeeNumber = item.LogDetails.EmployeeNumber;
						logdata.LogDetails.Event = item.LogDetails.Event;
						logdata.LogDetails.Message = item.LogDetails.Message;
						logdata.LogDetails.Month = item.LogDetails.Month;
						logdata.LogDetails.OperatingSystemNum = item.LogDetails.OperatingSystemNum;
						logdata.LogDetails.PlatformNumber = item.LogDetails.PlatformNumber;
						logdata.LogDetails.Position = item.LogDetails.Position;
						logdata.LogDetails.Round = item.LogDetails.Round;
						logdata.LogDetails.VersionNumber = item.LogDetails.VersionNumber;


						offlineEvents.EventLogs.Add (logdata);
					}
					WBidDataDwonloadAuthServiceClient client;
					BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
					client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
					client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);

					client.LogOffLineEventsCompleted+=Client_LogOperationCompleted;
					client.LogOffLineEventsAsync (offlineEvents);
				}

				File.Delete (WBidHelper.WBidOfflineEventFilePath);
			}
		}
		void Client_LogOperationCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{

		}
		private void UserAccountChecking()
		{
			bool IsUserdataAvailable=	CommonClass.isUserInformationAvailable ();

            if (!IsUserdataAvailable)
            {

                PerformSelector(new ObjCRuntime.Selector("walkthroughDismissed"), null, 0.5);
                //UserLoginviewController regs = new UserLoginviewController();
                //popoverController = new UIPopoverController(regs);
                //objPopDelegate = new MyPopDelegate(this);
                //objPopDelegate.CanDismiss = false;
                //popoverController.Delegate = objPopDelegate;

                //regs.objpopover = popoverController;
                //regs.SuperParent = this;
                //CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 175, 150, 350);
                //popoverController.PopoverContentSize = new CGSize(regs.View.Frame.Width, regs.View.Frame.Height);
                //popoverController.PresentFromRect(frame, View, 0, true);

            }
          
			else 
			{



				OdataBuilder ObjOdata = new OdataBuilder ();
				ObjOdata.RestService.Objdelegate = this;

				WebType = "CheckRemoUserAccount";
				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
					//							ActivityIndicator = new LoadingOverlay (View.Bounds, "Checking user Information. \nPlease wait..");
					//
					//							this.View.Add (ActivityIndicator);
					ObjOdata.CheckRemoUserAccount(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					InvokeInBackground (() => {
						//ObjOdata.CheckRemoUserAccount (GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					});
				}



			}

		}
		public void ServiceResponce(JsonValue jsonDoc)
		{
			Console.WriteLine ("Service Success");
			//ActivityIndicator.Hide ();
			if (WebType == "UpdateSubscriptionDate")
			{
				CustomServiceResponse remoteUserdetails = new CustomServiceResponse();
				remoteUserdetails = CommonClass.ConvertJSonToObject<CustomServiceResponse>(jsonDoc.ToString());
				if (remoteUserdetails != null)
				{
					if (remoteUserdetails.Status == true)
					{
						if (remoteUserdetails.WBExpirationDate != null)
						{
							GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
							GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = remoteUserdetails.TopSubscriptionLine;
							GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = remoteUserdetails.SecondSubscriptionLine;
							GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = remoteUserdetails.ThirdSubscriptionLine;

						}
						WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
						File.Delete(WBidHelper.GetWBidOfflinePaymentFilePath());

					}

				}

			}
			else if (WebType == "CheckRemoUserAccount")
			{
				if (jsonDoc["FirstName"] != null || jsonDoc["FirstName"].ToString().Length > 0)
				{
					RemoteUserInformation remoteUserdetails = new RemoteUserInformation();
					remoteUserdetails = CommonClass.ConvertJSonToObject<RemoteUserInformation>(jsonDoc.ToString());
					int DateTimeDifference = DateTime.Compare(remoteUserdetails.UserAccountDateTime, GlobalSettings.WbidUserContent.UserInformation.UserAccountDateTime);

					if (DateTimeDifference != 0)
					{
						CheckUserInformations(remoteUserdetails);
					}

					GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
					GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = remoteUserdetails.TopSubscriptionLine;
					GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = remoteUserdetails.SecondSubscriptionLine;
					GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = remoteUserdetails.ThirdSubscriptionLine;
					GlobalSettings.WbidUserContent.UserInformation.IsFree = remoteUserdetails.IsFree;
					GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed = remoteUserdetails.IsMonthlySubscribed;
					GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed = remoteUserdetails.IsYearlySubscribed;
					subscriptionMessageContent = remoteUserdetails.SubscriptionMessage;
					WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
					DisplaySubscriptionAlert();
				}
			}
			else if(WebType== "ApplicationLoadDataFromServer")
			{
				ApplicationLoadData appLoadData = CommonClass.ConvertJSonToObject<ApplicationLoadData>(jsonDoc.ToString());
				GlobalSettings.IsNeedToEnableVacDiffButton = appLoadData.IsNeedtoEnableVacationDifference;
				GlobalSettings.ServerFlightDataVersion = appLoadData.FlightDataVersion;
				InvokeOnMainThread(() => {
					NSNotificationCenter.DefaultCenter.PostNotificationName("SetFlightDataDifferenceButton", null);
				});
				

			}
		}
		public void ResponceError(string Error)
		{
			InvokeOnMainThread(()=>{
				//ActivityIndicator.Hide ();
			});
			//			Console.WriteLine ("Service Fail");
			
		}

		void CheckUserInformations(RemoteUserInformation remoteUserdetails)
		{
			var DifferenceList = new List<KeyValuePair<string, string>>();


			if (remoteUserdetails.FirstName != GlobalSettings.WbidUserContent.UserInformation.FirstName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("FirstName", GlobalSettings.WbidUserContent.UserInformation.FirstName + "," + remoteUserdetails.FirstName));
			}

			if (remoteUserdetails.LastName != GlobalSettings.WbidUserContent.UserInformation.LastName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("LastName", GlobalSettings.WbidUserContent.UserInformation.LastName + "," + remoteUserdetails.LastName));
			}


			if (remoteUserdetails.Email != GlobalSettings.WbidUserContent.UserInformation.Email)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Email", GlobalSettings.WbidUserContent.UserInformation.Email + "," + remoteUserdetails.Email));
			}

			if (remoteUserdetails.CellPhone != GlobalSettings.WbidUserContent.UserInformation.CellNumber)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("CellPhone", GlobalSettings.WbidUserContent.UserInformation.CellNumber + "," + remoteUserdetails.CellPhone));
			}

			if (remoteUserdetails.CarrierNum != GlobalSettings.WbidUserContent.UserInformation.CellCarrier)
			{
				int remoteCarrier = remoteUserdetails.CarrierNum;
				int localCarrier = GlobalSettings.WbidUserContent.UserInformation.CellCarrier;
				int CellcarrierCount = CommonClass.CellCarrier.Length;
				if (remoteCarrier > 0 && localCarrier > 0 && remoteCarrier < CellcarrierCount && localCarrier < CellcarrierCount)
					DifferenceList.Add(new KeyValuePair<string, string>("CarrierNum", GlobalSettings.WbidUserContent.UserInformation.CellCarrier + "," + remoteUserdetails.CarrierNum));
			}

			string LocalPosition = "";
			string RemotePosition = "";
			if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
			{
				LocalPosition = "Pilot";
			}

			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
			{
				LocalPosition = "Flight Attendant";
			}



			//			if (remoteUserdetails.Position.ToString() == "5")
			//			{
			//				RemotePosition = "Captain";
			//			}
			if (remoteUserdetails.Position.ToString() == "4")
			{

				RemotePosition = "Pilot";

			}
			else if (remoteUserdetails.Position.ToString() == "3")
			{
				RemotePosition = "Flight Attendant";
			}

			if (LocalPosition != RemotePosition)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Position", LocalPosition + "," + RemotePosition));
			}


			if (DifferenceList.Count > 0)
			{
				InvokeOnMainThread(() =>
				{
					UserAccountDifferenceScreen ObjUserDifference = new UserAccountDifferenceScreen();
					ObjUserDifference.DifferenceList = DifferenceList;
					ObjUserDifference.iSfromHome = true;
					ObjUserDifference.isAcceptMail = remoteUserdetails.AcceptEmail;


					//commented on 20 /6/2017 to solve sharing violation
					//WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

					ObjUserDifference.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					this.PresentViewController(ObjUserDifference, true, null);
				});
			}
			else
			{
				GlobalSettings.WbidUserContent.UserInformation.isAcceptMail = remoteUserdetails.AcceptEmail;
			}

		}

		private void getAvaibleHistoryList()
		{
			if (!System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath))
			{
                if (Reachability.CheckVPSAvailable ()) 
				{
					BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
					client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
					client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
					client.GetAvailableHistoricalListCompleted+= Client_GetAvailableHistoricalListCompleted;
					client.GetAvailableHistoricalListAsync();
				}
			}
		}
		void Client_GetAvailableHistoricalListCompleted (object sender, GetAvailableHistoricalListCompletedEventArgs e)
		{
			try
			{
				if (e.Result != null) 
				{

					List<WBidDataDownloadAuthorizationService.Model.BidData> lstBid = e.Result.ToList ();
					List<HistoricalBidData> lstHistoricalData = new List<HistoricalBidData> ();
					foreach (var item in lstBid) {
						lstHistoricalData.Add (new HistoricalBidData {Domicile = item.Domicile, Month = item.Month, Position = item.Position, Round = item.Round
								, Year = item.Year
						});
					}


					var previousfile = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.HST", SearchOption.AllDirectories).Select(Path.GetFileName);
					if (previousfile != null && previousfile.Count()>0)
					{
						foreach (var item in previousfile)
						{
							File.Delete(WBidHelper.GetAppDataPath() + "//" + item);
						}
					}
					var stream = File.Create (WBidHelper.HistoricalFilesInfoPath);
					ProtoSerailizer.SerializeObject (WBidHelper.HistoricalFilesInfoPath, lstHistoricalData, stream);
					stream.Dispose ();
					stream.Close ();
				}
			}catch(Exception ex){
			}

		}

		void HandleEditButtonEnable (NSNotification obj)
		{
			if (obj.Object.ToString () == "enable")
				btnEdit.Enabled = true;
			else {
				btnEdit.Enabled = false;
				btnEdit.Title = "Edit";
				btnEdit.Tag = 0;
			}
		}

		private void setViews ()
		{
			var layout = new UICollectionViewFlowLayout ();
			layout.SectionInset = new UIEdgeInsets (20,20,20,20);
			layout.MinimumInteritemSpacing = 20;
			layout.MinimumLineSpacing = 20;
			layout.ItemSize = new CGSize (250, 60);
			collectionVw = new bidDataCollectionController (layout);
			collectionVw.View.Frame = this.vWBidDataCollectionContainer.Frame;
			this.vWBidDataCollectionContainer.RemoveFromSuperview ();
			this.AddChildViewController (collectionVw);
			this.Add (collectionVw.View);
		}
		void HandleJiggle (NSNotification obj)
		{
			Console.WriteLine("gest jiggling");
			//			collectionVw.View.RemoveFromSuperview ();
			//			collectionVw = null;
			//			setViews ();
			collectionVw.shouldJiggle = true;
			btnEdit.Title = "Done";
			btnEdit.Tag = 1;
			collectionVw.CollectionView.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
		}

		private void scrap(string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
		{
			if (userName.ToLower () == "x21221") {
				ContractorEmpScrap scrapper = new ContractorEmpScrap (userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay,GlobalSettings.CurrentBidDetails.Postion);
				this.AddChildViewController (scrapper);
				//			scrapper.View.Hidden = true;
				this.View.AddSubview (scrapper.View);
			} 
			else
			{
				webView scrapper = new webView (userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
				this.AddChildViewController (scrapper);
				//			scrapper.View.Hidden = true;
				this.View.AddSubview (scrapper.View);
			}
		}

		//		public void secretTap(UITapGestureRecognizer gest) {
		//			if (gest.State == UIGestureRecognizerState.Ended) {
		//				int touchCount = 0;
		//				bool status = true;
		//
		//				RectangleF rect1 = new RectangleF (new PointF (924,64 ), new SizeF (100, 100)); 
		////				RectangleF rect2 = new RectangleF (new PointF (0,668 ), new SizeF (100, 100));
		////				RectangleF rect3 = new RectangleF (new PointF (924,668 ), new SizeF (100, 100));
		////
		//				for (touchCount = 0; touchCount < gest.NumberOfTouchesRequired; touchCount++) {
		//					PointF pt = gest.LocationOfTouch (touchCount, gest.View);
		////					if (!(rect1.Contains (pt) || rect2.Contains (pt) || (rect3.Contains(pt)))) {
		////						status = false;
		//					//					}
		//					if (!(rect1.Contains (pt)))
		//						{
		//							status = false;
		//						}
		//					if (status) {
		//						//show admin
		//						adminArea admin = new adminArea ();
		//						admin.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
		//						this.PresentViewController (admin,true,null);
		//						admin.View.Superview.BackgroundColor = UIColor.Clear;
		//						admin.View.Frame = new RectangleF (0,130,540,313);
		//						admin.View.Layer.BorderWidth = 1;
		//					} else {
		//						//leave it.
		//					}
		//				}
		//			}
		//		}


		//		public void cancelJiggle(UITapGestureRecognizer gest) {
		//
		//			collectionVw.shouldJiggle = false;
		//			btnEdit.Title = "Edit";
		//			btnEdit.Tag = 0;
		//			collectionVw.CollectionView.ReloadData();
		//
		//		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

        }
		partial void btnEditTapped (UIKit.UIBarButtonItem sender)
		{
			if(sender.Tag == 0){
				collectionVw.shouldJiggle = true;
				sender.Title = "Done";
				sender.Tag = 1;
				NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
			} else {
				collectionVw.shouldJiggle = false;
				sender.Title = "Edit";
				sender.Tag = 0;
				notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("StartJiggle"), HandleJiggle);
			}
			collectionVw.CollectionView.ReloadData();

		}

		partial void btnMiscTapped (UIKit.UIButton sender)
		{
			string[] arr = { "Configuration", "Change User Information", "Latest News", "My Subscription" };
			UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.X;
            sheet.ShowFrom(senderframe,sender, true);
			sheet.Dismissed += handleMIscTap;
		}
		public void handleMIscTap(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				
				ConfigTabBarControlller config = new ConfigTabBarControlller (true);
				config.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;

				this.PresentViewController (config, true, null);
                NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ServerNameChange"), ChangeServerName);
			} else if (e.ButtonIndex == 1) {

				userRegistrationViewController regs = new userRegistrationViewController ();
				regs.fromHome = true;
				regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (regs, true, null);
				notif3 = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("ReloadHome"), handleReloadHome);
			} else if (e.ButtonIndex == 2) 
			{
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + "news.pdf")) {
					webPrint fileViewer = new webPrint ();
					fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					this.PresentViewController (fileViewer, true, () => {
						fileViewer.LoadPDFdocument ("news.pdf");
					});
				}
				else {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", "No latest News found!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);

                }







            }
			else if (e.ButtonIndex == 3) 
			{
				SubscriptionViewController ObjSubscription = new SubscriptionViewController ();
				ObjSubscription.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (ObjSubscription, true, null);

			}
			UIActionSheet sheet = (UIActionSheet)sender;
			DisposeClass.DisposeEx(sheet);
		}
	
		void handleReloadHome (NSNotification obj)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (notif3);
			//			btnEdit.Title = "Edit";
			//			btnEdit.Tag = 0;
			//			collectionVw.View.RemoveFromSuperview ();
			//			collectionVw = null;
			//			setViews ();
			collectionVw.CollectionView.ReloadData ();

		}

        void ReloadHomeView(NSNotification obj)
        {
         
            collectionVw.CollectionView.ReloadData();

        }

        partial void btnHelpTapped (UIKit.UIButton sender)
		{
			string[] arr = { "Help", "Walkthrough", "Contact Us", "About" };
			UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
            CGRect senderframe = sender.Frame;
            senderframe.X = sender.Frame.X;
            sheet.ShowFrom(senderframe,sender, true);
			sheet.Dismissed += handleHelp;
		}
		void handleHelp(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				HelpViewController helpVC = new HelpViewController ();
				helpVC.pdfFileName = "Constraints";
				UINavigationController navCont = new UINavigationController (helpVC);
				navCont.NavigationBar.BarStyle = UIBarStyle.Black;
				navCont.NavigationBar.Hidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.Custom;
				this.PresentViewController (navCont, true, null);
			} else if (e.ButtonIndex == 1) {
				WalkthroughViewController introV = new WalkthroughViewController ();
				introV.home = new homeViewController ();
				introV.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
				this.PresentViewController (introV, true, null);
			} else if (e.ButtonIndex == 2) {
				ContactUsViewController contactVC = new ContactUsViewController ();
				contactVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (contactVC, true, null);
			} else if (e.ButtonIndex == 3) {
				AboutViewController aboutVC = new AboutViewController ();
				aboutVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (aboutVC, true, null);
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			btnEdit.Enabled = false;
			notif2 = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("EditEnableDisable"), HandleEditButtonEnable);
            //			CommonClass.SaveFormatBidReceipt ("");
            if (CommonClass.isVPSServer == "FALSE") {
                NavigationBar.TopItem.Title = "WBidMax - Home (" + NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString () + ")(VOFOX SERVER)";
            } else {
                NavigationBar.TopItem.Title = "WBidMax - Home (" + NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString () + ")";
            }

           if (notif == null)
				notif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("StartJiggle"), HandleJiggle);

			if (notifNavigateToLineView == null)
			notifNavigateToLineView = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("NavigateToLineView"), NavigateToLineView);
			this.View.UserInteractionEnabled = true;
			// Here when the app is launched first time by checking key value in nsuserdefualts and shows up the Walkthrough view.
			var value = NSUserDefaults.StandardUserDefaults["First"];
			if (value == null)
			{
				btnRetrive.Enabled = false;
				WalkthroughViewController introV = new WalkthroughViewController();
				introV.home = this;
				introV.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
				this.PresentViewController(introV, true, null);

			}
			else
			{
				if (!File.Exists(WBidHelper.WBidUserFilePath))
				{
					//					UserLoginviewController regs = new UserLoginviewController();
					//						popoverController = new UIPopoverController (regs);
					//					objPopDelegate = new MyPopDelegate (this);
					//					objPopDelegate.CanDismiss = false;
					//					popoverController.Delegate = objPopDelegate;
					//
					//					regs.objpopover=popoverController;
					//					regs.SuperParent=this;
					//					CGRect frame = new CGRect((View.Frame.Size.Width/2) - 75 ,(View.Frame.Size.Height/2) - 175 ,150,350);
					//					popoverController.PopoverContentSize = new CGSize (regs.View.Frame.Width, regs.View.Frame.Height);
					//					popoverController.PresentFromRect(frame,View,0,true);
					//                    userRegistrationViewController regs = new userRegistrationViewController();
					//                    regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					//                    this.PresentViewController(regs, true, null);
				}
				else
				{
					//GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser>(WBidHelper.WBidUserFilePath);
				}
			}

			setViews ();
		}
		public void dismisPopOver()
		{
			objPopDelegate.CanDismiss = true;
			popoverController.Dismiss (true);
		}
		//string empnumber=string.Empty;
		public void GoToCreateUserAccountPage(string EmpNo)
		{
			objPopDelegate.CanDismiss = true;
			popoverController.Dismiss (true);
			//PerformSelector (new ObjCRuntime.Selector ("navigateToUserEditpage:withObject"), new NSString (EmpNo), 3);
			//navigateToUserEditpage(EmpNo);
			empnumber = EmpNo;
			PerformSelector(new ObjCRuntime.Selector("navigateToUserEditpage"), null, 2);

		}
	[Export("navigateToUserEditpage")]
		void navigateToUserEditpage()
		{

			userRegistrationViewController regs = new userRegistrationViewController ();
			regs.EmployeeNumber = empnumber;
			regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			btnRetrive.Enabled = true;
			this.PresentViewController (regs, true, null);
		}
		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			homeViewController _parent;
			public bool CanDismiss;
			public MyPopDelegate (homeViewController parent)
			{
				_parent = parent;
			}

			public override bool ShouldDismiss (UIPopoverController popoverController)
			{
				if (CanDismiss) {
					return true;
				} else {

					return false;
				}
			}
		}

		[Export ("walkthroughDismissed")]
		public void walkthroughDismissed ()
		{

			if (!File.Exists (WBidHelper.WBidUserFilePath)) {

				UserLoginviewController regs = new UserLoginviewController();
				popoverController = new UIPopoverController (regs);
				objPopDelegate = new MyPopDelegate (this);
				objPopDelegate.CanDismiss = false;
				popoverController.Delegate = objPopDelegate;

				regs.objpopover=popoverController;
				regs.SuperParent=this;
				CGRect frame = new CGRect((View.Frame.Size.Width/2) - 75 ,(View.Frame.Size.Height/2) - 175 ,150,350);
				popoverController.PopoverContentSize = new CGSize (regs.View.Frame.Width, regs.View.Frame.Height);
				popoverController.PresentFromRect(frame,View,0,true);
				//				userRegistrationViewController regs = new userRegistrationViewController ();
				//				regs.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				btnRetrive.Enabled = true;
				//				this.PresentViewController (regs, true, null);
			} else {
				//GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
			}
		}

		#region IBActions
		partial void btnNewBidPeriodTapped (UIKit.UIButton sender)
		{
			string[] arr = { "New Bid Period", "Historical Bid Period" };
			UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
			sheet.ShowFrom(sender.Frame,this.NavigationController.NavigationBar, true);
			sheet.Dismissed += HandleRetrieveBidPeriod;
		}
		private void SouthWestWifiAlert()
		{
            if (WBidHelper.IsSouthWestWifiOr2wire()==false) {
                if (Reachability.CheckVPSAvailable ()) {



				} 
				else 
				{
					if (WBidHelper.IsSouthWestWifiOr2wire()) {
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);


                    }
                    else {
                    
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    }
                }
			}
			else
			{
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }

		}
		void HandleRetrieveBidPeriod (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0) {
				SouthWestWifiAlert ();
				GlobalSettings.isHistorical = false;
				newBid = new getNewBidPeriodViewController ();
				
				UINavigationController navCont = new UINavigationController (newBid);
				navCont.NavigationBarHidden = true;
				navCont.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (navCont, true, null);
			} else if (e.ButtonIndex == 1) {
                if (WBidHelper.IsSouthWestWifiOr2wire() == false) {
                    if (Reachability.CheckVPSAvailable()) {

                        UIAlertController alert = UIAlertController.Create("WBidMax", "When viewing Historical Bid Data, Vacation Correction, EOM and MIL Corrections will not be available.\n\nNor will you be able to accidentally submit a bid using the Historical Bid Data.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
                            GlobalSettings.isHistorical = true;
                             newBid = new getNewBidPeriodViewController ();
                            UINavigationController navCont = new UINavigationController (newBid);
							
							navCont.NavigationBarHidden = true;
                            navCont.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                            this.PresentViewController (navCont, true, null);
                        }));
                        this.PresentViewController(alert, true, null);
}
                    else {
						if (WBidHelper.IsSouthWestWifiOr2wire ()) {
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                        }
                        else {
                        
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
					}
				} else {
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
			}
		}
		partial void btnDemoLIneTapped (Foundation.NSObject sender)
		{
			OverlapCorrectionViewController overlap = new OverlapCorrectionViewController();
			overlap.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(overlap,true,null);
		}





		#endregion
		#region observe notification
		private void observeNotification()
		{
			//			NSNotificationCenter.DefaultCenter.AddObserver ("loginSuccess", startDownloadingBidData);
		}
		//		private void startDownloadingBidData(NSNotification n)
		//		{
		//
		//		}

		#endregion

		private void DeleteBidPeriod(string domcile,string position,string round,int bidperiod)
		{
			try
			{
				//domicle==bwi
				//position==Cp
				//round=D
				//bidperiod=1

				//string message = "Are you sure you want to delete the" + currentOpenBid + " WBid data files \nfor " + SelectedUser.DomicileName + " " + SelectedPosition.LongStr + " " + SelectedEquipment.EquipmentNumber.ToString() + " ";
				// message += SelectedBidRound.RoundDescription + " for " + SelectedBidPeriod.Period + " ?";

				//if (MessageBox.Show(message, "WBidMax", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
				// {
				string fileName = domcile + position + bidperiod.ToString("d2") + (round == "D" ? "M" : "S") + "737";
				string CommutefileName = domcile + position + bidperiod.ToString("d2") + (round == "D" ? "M" : "S") + "Cmt.COM";

				string folderName = WBidCollection.GetPositions().FirstOrDefault(x => x.LongStr == fileName.Substring(3, 2)).ShortStr + round + fileName.Substring(0, 23) + WBidCollection.GetBidPeriods().FirstOrDefault(x=>x.BidPeriodId==bidperiod).HexaValue;
				//Delete WBL file
				if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBL"))
				{
					File.Delete(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBL");

				}

				//Delete WBP file
				if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBP"))
				{
					File.Delete(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBP");

				}

				//Delete WBS file
				if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBS"))
				{
					File.Delete(WBidHelper.GetAppDataPath() + "\\" + fileName + ".WBS");

				}

				//Delete VAC file
				if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + fileName + ".VAC"))
				{
					File.Delete(WBidHelper.GetAppDataPath() + "\\" + fileName + ".VAC");

				}
				//delete the folder.
				if (Directory.Exists(WBidHelper.GetAppDataPath() + "\\" + folderName))
				{
					Directory.Delete(WBidHelper.GetAppDataPath() + "\\" + folderName, true);
				}
				if (System.IO.File.Exists(CommutefileName))
				{
					System.IO.File.Delete(CommutefileName);
				}
			}

			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
	public static class Extensions {
		//		public static string LocalValue (this NSDate date) {
		//			var d = DateTime.Parse (date.ToString());
		//			var convertedTime = TimeZoneInfo.ConvertTime (d, TimeZoneInfo.Local);
		//			return convertedTime.ToString ("HH:mm");
		//		}

		//		public static DateTime NSDateToDateTime(this NSDate date)
		//		{
		//			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( 
		//				new DateTime(2001, 1, 1, 0, 0, 0) );
		//			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		//		}
		//
		//		public static NSDate DateTimeToNSDate(this DateTime date)
		//		{
		//			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
		//				new DateTime(2001, 1, 1, 0, 0, 0) );
		//			return NSDate.FromTimeIntervalSinceReferenceDate(
		//				(date - reference).TotalSeconds);
		//		}

		public static DateTime NSDateToDateTime(this NSDate date)
		{
			// NSDate has a wider range than DateTime, so clip
			// the converted date to DateTime.Min|MaxValue.
			double secs = date.SecondsSinceReferenceDate;
			if (secs < -63113904000)
				return DateTime.MinValue;
			if (secs > 252423993599)
				return DateTime.MaxValue;
			return ((DateTime)date).ToLocalTime ();
		}

		public static NSDate DateTimeToNSDate(this DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind (date, DateTimeKind.Local);
			return (NSDate) date;
		}

	}
}

