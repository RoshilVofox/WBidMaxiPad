                                                                                   
using System;

using Foundation;
using UIKit;
using System.Text.RegularExpressions;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidiPad.Core;
using System.Json;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.Model;
using System.IO;
using CoreGraphics;
using System.Collections.Generic;

namespace WBid.WBidiPad.iOS
{
	public partial class UserLoginviewController : UIViewController, IServiceDelegate
	{

		public UIPopoverController objpopover;
		public homeViewController SuperParent;
		RemoteUserInformation ObjremoteUserInfo;
		LoadingOverlay ActivityIndicator;
		public UIPopoverController popoverController;
		public MyPopDelegate objPopDelegate;
		public string UserId="";
		public string Password="";
		public UserLoginviewController () : base ("UserLoginviewController", null)
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
			//this.View.Dispose ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ActivityIndicator= new LoadingOverlay(View.Bounds, "Checking user account. \nPlease wait..");
			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

			if (SystemVersion > 12)
			{
                ModalInPresentation = true;
			}
			this.View.Add (ActivityIndicator);
			ActivityIndicator.Hidden = true;

//			txtEmpNo.ShouldChangeCharacters = (textField, range, replacement) =>
//			{
//				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
//				int number;
//				return newContent.Length <= 50 && (replacement.Length == 0 || int.TryParse(replacement, out number));
//			};

           
            txtEmpNo.ShouldChangeCharacters = (textField, range, replacement) =>
                          {
                              var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
                    int number;
                   
                    return newContent.Length <= 50 && (replacement.Length == 0 || replacement[0].ToString() == "x" || replacement[0].ToString() == "e" || int.TryParse(replacement, out number));
                          };
         
			var tap = new UITapGestureRecognizer (secretTap);
			tap.NumberOfTouchesRequired = 1; // Change this value to 3 before submitting!
			this.View.AddGestureRecognizer (tap);

			// Perform any additional setup after loading the view, typically from a nib.
		}
		// Perform any additional setup after loading the view, typically from a nib.
		public void secretTap(UITapGestureRecognizer gest) {
			txtEmpNo.ResignFirstResponder();
			txtPassword.ResignFirstResponder();
			if (gest.State == UIGestureRecognizerState.Ended) {
				int touchCount = 0;
				bool status = true;

				CGRect rect1 = new CGRect (new CGPoint (380,0 ), new CGSize (100, 100)); 
				//				RectangleF rect2 = new RectangleF (new PointF (0,570 ), new SizeF (100, 100));
				//				RectangleF rect3 = new RectangleF (new PointF (490,570 ), new SizeF (100, 100));

				for (touchCount = 0; touchCount < (int)gest.NumberOfTouchesRequired; touchCount++) {
					CGPoint pt = gest.LocationOfTouch (touchCount, gest.View);
					//					if (!(rect1.Contains (pt) || rect2.Contains (pt) || (rect3.Contains(pt)))) {
					//						status = false;
					//					}
					if (!(rect1.Contains (pt)))
					{
						status = false;
					}
					if (status) {
						//show admin
						InvokeOnMainThread (() => {
							UIAlertController alert = UIAlertController.Create("WBidMax", "Enter Admin Password", UIAlertControllerStyle.Alert);

                            alert.AddTextField(delegate (UITextField textField)
                            {

                            });
                            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {

                            }));

                            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                                string psw = alert.TextFields[0].Text;
                                if (psw == "Vofox2013-1")
                                {
                                    NSNotificationCenter.DefaultCenter.PostNotificationName("ShowReparseView", null);
                                    adminArea admin = new adminArea();
                                    admin.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                                    admin.fromHome = true;
                                    this.PresentViewController(admin, true, null);
                                }

                            }));

                            this.PresentViewController(alert, true, null); 
                        });
					} else {
						//leave it.
					}
				}
			}
		}
		void HandleAlertClicked (object sender, UIButtonEventArgs e)
		{
			UIAlertView alertt = (UIAlertView)sender;
			if (e.ButtonIndex == 1) {
				string psw = alertt.GetTextField (0).Text;
				if (psw == "Vofox2013-1") {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("ShowReparseView", null);
					adminArea admin = new adminArea ();
					admin.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					admin.fromHome = true;
					this.PresentViewController (admin,true,null);
					//admin.View.Superview.BackgroundColor = UIColor.Clear;
					//admin.View.Frame = new RectangleF (0,130,540,313);
					//admin.View.Layer.BorderWidth = 1;
				}
			}
		}

//		void CWACredential_Checking(string UserId, string Password)
//		{
//		}
			[Export ("AuthenticationCheck")]
			private void AuthenticationCheck ()
			{
            UIAlertController okAlertController;

            if (WBidHelper.IsSouthWestWifiOr2wire()==false) 
			{
				//checking  the internet connection available
				//==================================================================================================================
                if (Reachability.CheckVPSAvailable()) {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckSuccess", null);
					//checking CWA credential
					//==================================================================================================================

			
					Authentication authentication = new Authentication ();
					string authResult = authentication.CheckCredential (UserId, Password);
					if (authResult.Contains ("ERROR: ")) 
					{
						WBidLogEvent obj = new WBidLogEvent();
						obj.LogBadPasswordUsage(UserId, false, authResult);
						InvokeOnMainThread (() => {
							ActivityIndicator.Hide ();
							CustomAlertView customAlert = new CustomAlertView();
							customAlert.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							UINavigationController nav = new UINavigationController(customAlert);
							customAlert.AlertType = "InvalidCredential";
							nav.NavigationBarHidden = true;
							nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							this.PresentViewController(nav, true, null);
							//okAlertController = UIAlertController.Create("WBidMax", "Invalid Username or Password", UIAlertControllerStyle.Alert);
       //                     okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
       //                     this.PresentViewController(okAlertController, true, null);


                        });
					} else if (authResult.Contains ("Exception")) {


						//Need to log submit time out

						//WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						//obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber);


						InvokeOnMainThread (() => {
							ActivityIndicator.Hide ();
							
							showTimeOutAlert();
						});
					} else {
						InvokeOnMainThread (() => {
						
							checkUserExist ();
						});

						//CWA Success
					}
				}
				else 
				{
					InvokeOnMainThread (() => {
						ActivityIndicator.Hide ();
                        if (WBidHelper.IsSouthWestWifiOr2wire())
                        {
                             okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                             okAlertController = UIAlertController.Create("WBidMax", Constants.VPSDownAlert, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                       
                    });
				}
			}

		
		else
		{
				InvokeOnMainThread (() => {
					ActivityIndicator.Hide ();
                    okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    //okAlertController = UIAlertController.Create("WBidMax", GlobalSettings.SouthWestWifiMessage, UIAlertControllerStyle.Alert);
                    //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    //this.PresentViewController(okAlertController, true, null);
                });
			
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

			regs.objpopover = popoverController;

			CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 175, 150, 350);
			popoverController.PopoverContentSize = new CGSize(regs.View.Frame.Width, regs.View.Frame.Height);
			popoverController.PresentFromRect(frame, View, 0, true);

			//------
		}
		void checkUserExist()
		{
            if (Reachability.CheckVPSAvailable()) {



				OdataBuilder ObjOdata = new OdataBuilder ();
				ObjOdata.RestService.Objdelegate = this;


				this.View.Add (ActivityIndicator);

				ObjOdata.CheckRemoUserAccount (txtEmpNo.Text.ToString());

			
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
			Console.WriteLine ("Service Success");
			ActivityIndicator.Hide ();
            UIAlertController alert;

            int empNo = int.Parse (jsonDoc ["EmpNum"].ToString ());
			if (empNo != 0) 
			{
                alert = UIAlertController.Create("Great!", "We found a previous account from  WbidMax.\nWe've imported those settings.\nPlease verify the settings and change as needed", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                    ObjremoteUserInfo = CommonClass.ConvertJSonToObject<RemoteUserInformation>(jsonDoc.ToString());
                    ImportUserInfo(ObjremoteUserInfo);
                }));

                this.PresentViewController(alert, true, null);
            } else 
			{
                alert = UIAlertController.Create("No Existing Account", "\nWe checked , but no previous account exists for you.\n\nThe next view will let you create your account.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Create Account", UIAlertActionStyle.Default, (actionOK) => {
                    SuperParent.GoToCreateUserAccountPage(GetValidEmpNo(txtEmpNo.Text));
                }));

                this.PresentViewController(alert, true, null);
            }
		}
		public void ImportUserInfo(RemoteUserInformation ObjremoteUserInfo)
		{
			List<AppDataBidFileNames> prevappdataBidfilenames = new List<AppDataBidFileNames>();
			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.AppDataBidFiles.Count > 0)
				prevappdataBidfilenames = GlobalSettings.WbidUserContent.AppDataBidFiles;

			WbidUser	wbidUser = new WbidUser();
			wbidUser.AppDataBidFiles = prevappdataBidfilenames;
			wbidUser.UserInformation = new WBid.WBidiPad.Model.UserInformation();
			wbidUser.UserInformation.isAcceptMail=ObjremoteUserInfo.AcceptEmail ;
			wbidUser.UserInformation.FirstName=ObjremoteUserInfo.FirstName;
			wbidUser.UserInformation.LastName=ObjremoteUserInfo.LastName ;
			wbidUser.UserInformation.Email=ObjremoteUserInfo.Email ;
			wbidUser.UserInformation.CellNumber=ObjremoteUserInfo.CellPhone;
			wbidUser.UserInformation.PaidUntilDate = ObjremoteUserInfo.WBExpirationDate;
			wbidUser.UserInformation.EmpNo=ObjremoteUserInfo.EmpNum.ToString();
			wbidUser.UserInformation.CellCarrier=ObjremoteUserInfo.CarrierNum ;

			if (ObjremoteUserInfo.Position == 4)
			{
				wbidUser.UserInformation.Position = "Pilot";
			}
			else if (ObjremoteUserInfo.Position == 3)
			{
				wbidUser.UserInformation.Position = "FA";
			}
			wbidUser.UserInformation.TopSubscriptionLine = ObjremoteUserInfo.TopSubscriptionLine;
			wbidUser.UserInformation.SecondSubscriptionLine = ObjremoteUserInfo.SecondSubscriptionLine;
			wbidUser.UserInformation.ThirdSubscriptionLine = ObjremoteUserInfo.ThirdSubscriptionLine;
			wbidUser.UserInformation.IsFree = ObjremoteUserInfo.IsFree;
			wbidUser.UserInformation.IsMonthlySubscribed = ObjremoteUserInfo.IsMonthlySubscribed;
			wbidUser.UserInformation.IsYearlySubscribed = ObjremoteUserInfo.IsYearlySubscribed;
		
			//wbidUser.UserInformation.Domicile = ObjremoteUserInfo.BidBase;
			UpdateUserDetails (wbidUser);
		}

		void UpdateUserDetails(WbidUser wbidUser)
		{
			GlobalSettings.WbidUserContent = wbidUser;

				WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);

            SuperParent.GoToCreateUserAccountPage(GetValidEmpNo(txtEmpNo.Text));
		}

        string GetValidEmpNo(string EmpNo)
        {
            string ValidEmp;

            ValidEmp=Regex.Replace(EmpNo, "[^0-9]+", string.Empty);;


            return ValidEmp;
        }
		public void ResponceError(string Error)
		{
			ActivityIndicator.Hide ();
			Console.WriteLine ("Service Fail");
            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
        }

		public class MyPopDelegate : UIPopoverControllerDelegate
		{
			UserLoginviewController _parent;
			public bool CanDismiss;
			public MyPopDelegate(UserLoginviewController parent)
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
		partial void LogInButtonClicked (NSObject sender)
		{
			
			if (Regex.Match(this.txtEmpNo.Text, "^[e,E,x,X,0-9][0-9]*$").Success)
			{
				if(this.txtPassword.Text.Length > 0)
				{
					string uName = this.txtEmpNo.Text.ToLower();
					if(uName[0] != 'e' && uName[0] != 'x')
						uName = "e" + uName;
					ActivityIndicator.Hidden = false;
					UserId=uName;
					Password= txtPassword.Text.ToString();

					this.PerformSelector (new ObjCRuntime.Selector ("AuthenticationCheck"), null, 0);
					
				}else{
                    UIAlertController okAlertController = UIAlertController.Create("Instruction", "Password is required.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }
			}
			else
			{
                UIAlertController okAlertController = UIAlertController.Create("Instruction", "Invalid Employee Number", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);
            }


		}
	}
}

