
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core;
using System.Json;

namespace WBid.WBidiPad.iOS
{
	public partial class UserAccountDifferenceScreen : UIViewController ,IServiceDelegate
	{
		UserDifferenceTableDataSource ObjDataSource;
		public bool isAcceptMail;
		int Position;
		public bool iSfromHome;
		public UserAccountDifferenceScreen () : base ("UserAccountDifferenceScreen", null)
		{
		}
		LoadingOverlay ActivityIndicator;
		public List<KeyValuePair<string, string>> DifferenceList = new List<KeyValuePair<string, string>> ();
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ObjDataSource = new UserDifferenceTableDataSource (DifferenceList);
			tableView.Source = ObjDataSource;
			tableView.ReloadData ();
			//this.NavigationController.NavigationBar.TintColor=UIColor.Green;
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void btnCancelClicked (NSObject sender)
		{
			this.DismissViewController(true,null);
		}
		partial void btnUpdateClicked (NSObject sender)
		{
			

            if (Reachability.CheckVPSAvailable()) {


				ActivityIndicator= new LoadingOverlay(View.Bounds, "Updating User Information. \nPlease wait..");

				this.View.Add (ActivityIndicator);
				OdataBuilder ObjOdata = new OdataBuilder ();
				ObjOdata.RestService.Objdelegate = this;
				ObjDataSource.ObjremoteUserInfo.AcceptEmail=isAcceptMail;
				//Position=ObjDataSource.ObjremoteUserInfo.Position;
				Position=ObjDataSource.ObjremoteUserInfo.Position;
				InvokeInBackground(()=>{
				ObjOdata.UpdateUserAccount (ObjDataSource.ObjremoteUserInfo);

				});
			} else {

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

        }

		public void ServiceResponce(JsonValue jsonDoc)
		{
			InvokeOnMainThread (() => {
				Console.WriteLine ("Service Success");
				ActivityIndicator.Hide ();
                UIAlertController okAlertController;

                SaveLocalUserDetails ();
				if (jsonDoc ["Status"] == true) {
				    okAlertController = UIAlertController.Create("WBidMax", "User account Updated successfully", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                        this.DismissViewController(true, null);
                        if (!iSfromHome)
                            NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadUserPage", null);
                        else
                            NSNotificationCenter.DefaultCenter.PostNotificationName("USerDifferenceUpdated", null);

                    }));

                    this.PresentViewController(okAlertController, true, null);

                }
                else {
				    okAlertController = UIAlertController.Create("WBidMax", jsonDoc["Message"], UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);


                }
            });
		}
		void SaveLocalUserDetails()
		{
			GlobalSettings.WbidUserContent.UserInformation.isAcceptMail=ObjDataSource.ObjremoteUserInfo.AcceptEmail ;
			 GlobalSettings.WbidUserContent.UserInformation.FirstName=ObjDataSource.ObjremoteUserInfo.FirstName;
			GlobalSettings.WbidUserContent.UserInformation.LastName=ObjDataSource.ObjremoteUserInfo.LastName ;
			GlobalSettings.WbidUserContent.UserInformation.Email=ObjDataSource.ObjremoteUserInfo.Email ;
			GlobalSettings.WbidUserContent.UserInformation.CellNumber=ObjDataSource.ObjremoteUserInfo.CellPhone;

			GlobalSettings.WbidUserContent.UserInformation.EmpNo=ObjDataSource.ObjremoteUserInfo.EmpNum.ToString();
			GlobalSettings.WbidUserContent.UserInformation.CellCarrier=ObjDataSource.ObjremoteUserInfo.CarrierNum ;
			GlobalSettings.WbidUserContent.UserInformation.Domicile = ObjDataSource.ObjremoteUserInfo.BidBase;

			if (Position != null) 
			{
//				if (Position == 5) {
//					GlobalSettings.WbidUserContent.UserInformation.Position = "CP";
//				} else if (Position == 4) {
//					GlobalSettings.WbidUserContent.UserInformation.Position = "FO";
//				} else if (Position == 3) {
//					GlobalSettings.WbidUserContent.UserInformation.Position = "FA";
//				}
				if (Position == 4) {
					GlobalSettings.WbidUserContent.UserInformation.Position = "Pilot";
				}  else if (Position == 3) {
					GlobalSettings.WbidUserContent.UserInformation.Position = "FA";
				}

			}

			GlobalSettings.WbidUserContent.UserInformation.isAcceptMail = isAcceptMail;
			WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();
			this.View.Dispose ();


		}

		public void ResponceError(string Error)
		{
			InvokeOnMainThread (() => {
				ActivityIndicator.Hide ();
				Console.WriteLine ("Service Fail");
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);

            });
		}

	}
}

