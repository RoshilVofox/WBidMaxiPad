
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Text.RegularExpressions;
using System.IO;
using System.Json;
using StoreKit;
using SharedCode;
using WBid.WBidiPad.SharedLibrary.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class SubscriptionViewController : UIViewController,IServiceDelegate
	{
		public static string oneMonthSubscription = "com.wbid.oneMonthSubscription";
		OdataBuilder ObjOdata = new OdataBuilder ();
		LoadingOverlay ActivityIndicator;
		CustomPaymentObserver theObserver;
		InAppPurchaseManager iap;
		string WebType;
		bool purchased;
		NSObject priceObserver, requestObserver;
		string transactionId;
		public SubscriptionViewController () : base ("SubscriptionViewController", null)
		{
			iap = new InAppPurchaseManager();
			theObserver = new CustomPaymentObserver(iap);
			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);
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
			// remove the observer when the view isn't visible
			//if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
            if (Reachability.CheckVPSAvailable()) {
					if(priceObserver!=null)
					NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);

					if(requestObserver!=null)
					NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);
				}
			//}


			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ObjOdata.RestService.Objdelegate = this;
			ActivityIndicator= new LoadingOverlay(View.Bounds, "Checking Subscription... \nPlease wait..");

					this.View.Add (ActivityIndicator);
			HideIndicator();


			//if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
            if (Reachability.CheckVPSAvailable()) {
					ActivityIndicator= new LoadingOverlay(View.Bounds, "Checking Subscription... \nPlease wait..");

					ActivityIndicator.Hidden = false;

					WebType="GetSubscriptionDate";
					InvokeInBackground (() => {
						ObjOdata.CheckRemoUserAccount (GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					});
				} else
					setThreeLineData ();
			//} else
				//setThreeLineData ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public void initializeInAppPurchase()
		{

//			lblFirstLine.Text="";
//			lblThirdLine.Text = "";
//			lblSecondLine.Text = "";
			// setup the observer to wait for prices to come back from StoreKit <- AppStore
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
				(notification) => {
					NSDictionary info = notification.UserInfo;
					var NSOneMonthProductId = new NSString (oneMonthSubscription);


					if (info == null) {
						// if info is null, probably NO valid prices returned, therefore it doesn't exist at all

						return;
					}

					// we only update the button with a price if the user hasn't already purchased it
					if (info.ContainsKey (NSOneMonthProductId)) {
						

						var product = (SKProduct)info [NSOneMonthProductId];
						//Print(product);
						//var btnTitle = string.Format ("Buy { 0}", product.LocalizedPrice ());
						//BtnOneMonthSubscription.SetTitle ("Subscribe for "+  product.LocalizedPrice (),UIControlState.Normal);
					}

				} );
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionSucceededNotification,
				(notification) => {
					try
					{
					SKPaymentTransaction sKPaymentTransaction = (SKPaymentTransaction)notification.UserInfo["transaction"];
						transactionId=sKPaymentTransaction.TransactionIdentifier;
					}
					catch(Exception ex)
					{
						transactionId=string.Empty;
					}
					// update the buttons after a successful purchase
                    HideIndicator();
					if(!purchased)
						UpdatePaidUntilDate(transactionId);


				} );

			requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerRequestFailedNotification,
				(notification) => {
					// TODO:
                    HideIndicator();
				WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
					obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "RequestFailed", "0", "0","");
				    Console.WriteLine ("Request Failed");
					InvokeOnMainThread(() => {
						UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Request failed", UIAlertControllerStyle.Alert);
						okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
						this.PresentViewController(okAlertController, true, null);
					});
                    
                } );
			requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionFailedNotification,
				(notification) => {
					// TODO:
				WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
					obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "inAppPmntFail", "0", "0","");
                    HideIndicator();
					Console.WriteLine ("Request Cancelled");

				} );
		

		}


		public void UpdatePaidUntilDate(string transactionId)
		{
			WebType="UpdateSubscriptionDate";
			ActivityIndicator= new LoadingOverlay(View.Bounds, "Updating data.. \nPlease wait..");

			this.View.Add (ActivityIndicator);
			ActivityIndicator.Hidden = false;
			PaymentUpdateModel ObjPaymentDetails = new PaymentUpdateModel ();
			ObjPaymentDetails.EmpNum =int.Parse( GlobalSettings.WbidUserContent.UserInformation.EmpNo);
			ObjPaymentDetails.Month = 1;
			ObjPaymentDetails.Message="PaymentReceived for Onetime Monthly";
			ObjPaymentDetails.TransactionNumber = transactionId;
			if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
                if (Reachability.CheckVPSAvailable()) {
					
					ObjOdata.UpdateSubscriptionDate (ObjPaymentDetails);
				}
				else
				{
					SaveOffLinePayment (transactionId);
				}

			}

//			DateTime date = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now;
//			date.AddMonths (1);
//			GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = date;
		
//		WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

		}
		private void SaveOffLinePayment(string transactionId)
		{
			PaymentUpdateModel ObjPaymentDetails = new PaymentUpdateModel ();
			ObjPaymentDetails.EmpNum =int.Parse( GlobalSettings.WbidUserContent.UserInformation.EmpNo);
			ObjPaymentDetails.Month = 1;
			ObjPaymentDetails.TransactionNumber = transactionId;
			ObjPaymentDetails.Message = "PaymentReceived for Onetime Monthly-Offline";
			XmlHelper.SerializeToXml<PaymentUpdateModel>(ObjPaymentDetails, WBidHelper.GetWBidOfflinePaymentFilePath());

			string message="You have successfully completed an in-app purchase. Unfortunately, an error happened while updating the WBid database.  We will update the subscription details later.";

			message += Environment.NewLine + "If you have any question with the subscription then please contact Admin.";
			InvokeOnMainThread (() => {
				WarnningAlert (message);
			});


		}
		public override void ViewWillAppear (bool animated)
		{
			if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
                initializeInAppPurchase ();
                if (Reachability.CheckVPSAvailable()) {
					
					List <string> listProduct = new List<string> ();
					listProduct.Add (oneMonthSubscription);

					iap.RequestProductData (listProduct);
					return;
				}
				else
				{
                    if (WBidHelper.IsSouthWestWifiOr2wire())
					{
						WarnningAlert("You are using the Free Wifi on the plane.  You cannot subscribe with iTunes while using the Free Wifi.  Please try again when you have established a good Wifi connection on the ground.");
					}
					else
					{
                    
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    }

			}
			}

		

		}
		public void ServiceResponce(JsonValue jsonDoc)
		{
			Console.WriteLine ("Service Success");

			InvokeOnMainThread (() => {
			if (WebType == "GetSubscriptionDate") {
HideIndicator();
				if (jsonDoc ["FirstName"] != null || jsonDoc ["FirstName"].ToString ().Length > 0) 
					{
					RemoteUserInformation remoteUserdetails = new RemoteUserInformation ();
					remoteUserdetails = CommonClass.ConvertJSonToObject<RemoteUserInformation> (jsonDoc.ToString ());
					if (remoteUserdetails.WBExpirationDate != null) 
					{
						GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
						GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = remoteUserdetails.TopSubscriptionLine;
						GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = remoteUserdetails.SecondSubscriptionLine;
						GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = remoteUserdetails.ThirdSubscriptionLine;
						GlobalSettings.WbidUserContent.UserInformation.IsFree = remoteUserdetails.IsFree;
						GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed = remoteUserdetails.IsMonthlySubscribed;
						GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed = remoteUserdetails.IsYearlySubscribed;




					}
					WBidHelper.SaveUserFile (GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

				}
								
				setThreeLineData ();
								
			} else if (WebType == "UpdateSubscriptionDate")
			{
				purchased=true;

                HideIndicator();

						CustomServiceResponse remoteUserdetails = new CustomServiceResponse ();
						remoteUserdetails = CommonClass.ConvertJSonToObject<CustomServiceResponse> (jsonDoc.ToString ());
					if(remoteUserdetails!=null)
					{
						if(remoteUserdetails.Status==true)
						{
						if (remoteUserdetails.WBExpirationDate != null) 
						{
							GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
							GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = remoteUserdetails.TopSubscriptionLine;
							GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = remoteUserdetails.SecondSubscriptionLine;
							GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = remoteUserdetails.ThirdSubscriptionLine;
							

						}
						WBidHelper.SaveUserFile (GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
						}
						else
						{
							//log offline payment information
							SaveOffLinePayment (transactionId);
						}

					}
					else
					{
						//log offline payment information
						SaveOffLinePayment (transactionId);
					}

					setThreeLineData ();


				//WebType="GetSubscriptionDate";
				//ObjOdata.CheckRemoUserAccount (GlobalSettings.WbidUserContent.UserInformation.EmpNo);

			}
			});
		}

		void HideIndicator()
		{


            InvokeOnMainThread(() => {

		
			if(ActivityIndicator != null)
			{
				ActivityIndicator.Hidden = true;
				ActivityIndicator.Hide();
			}
					});
		}
		public void ResponceError(string Error)
		{    
							InvokeOnMainThread (() => {
								setThreeLineData ();
								HideIndicator();
			//setThreeLineData ();

			Console.WriteLine ("Service Fail");
            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
            });
		}
		public void setThreeLineData()
		{
			

			if ((GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine == null)) {
				lblFirstLine.Text="Subscription Details not available";
				return;
			}
			lblFirstLine.Text=GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine;
			if (GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine.ToString ().Contains ("Expire")) {
				lblFirstLine.TextColor = UIColor.Red;
			} else
				lblFirstLine.TextColor = UIColor.FromRGB((nfloat)(94.0/255.0),(nfloat)(158.0/255.0),(nfloat)(60.0/255.0));
			lblSecondLine.Text = GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine;
			lblThirdLine.Text = GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine;

			//if (GlobalSettings.WbidUserContent.UserInformation.IsFree || GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed) {
			//	BtnOneMonthSubscription.Hidden = true;

				if (GlobalSettings.WbidUserContent.UserInformation.IsFree || ((GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed || GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed)&&((GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate?? DateTime.MinValue).AddDays(10)>=DateTime.Now))) 
				{
				BtnOneMonthSubscription.Hidden = true;
				}

		}
		partial void OneMonthSubscriptionClicked (NSObject sender)
		{

			//UpdatePaidUntilDate();
			purchased=false;

			//if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
					
					SubscriptionProcess();
				}
				else
				{if (WBidHelper.IsSouthWestWifiOr2wire ()) 
					{
					WarnningAlert("You are using the Free Wifi on the plane.  You cannot subscribe with iTunes while using the Free Wifi.  Please try again when you have established a good Wifi connection on the ground.");
					}
            else  WarnningAlert (Constants.VPSDownAlert);

			}
            //} else WarnningAlert("You are using the Free Wifi on the plane.  You cannot subscribe with iTunes while using the Free Wifi.  Please try again when you have established a good Wifi connection on the ground.");
		}
		public void WarnningAlert(string message)
		{
		    UIAlertController okAlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
            return;
		}
		public void SubscriptionProcess()
		{
			DateTime PaidUntilDate =  GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate ?? DateTime.Now ;
			int days =CommonClass.DaysBetween(DateTime.Now,PaidUntilDate );
			if (days < 30) {
				ActivityIndicator= new LoadingOverlay(View.Bounds, "Processing... \nPlease wait..");

				this.View.Add (ActivityIndicator);
				iap.PurchaseProduct (oneMonthSubscription);
			}
			else
			{
				WarnningAlert ("Unable to purchase. There must be 30 or fewer days remaining on your subscription before renewals can be processed.");
			}
		}
		partial void DoneButtonClicked (NSObject sender)
		{
			this.DismissViewController(true,null);
		}
	}
}

