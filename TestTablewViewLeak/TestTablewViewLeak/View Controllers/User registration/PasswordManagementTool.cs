
using System;

using Foundation;
using UIKit;
using System.Json;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.iOS
{
	public partial class PasswordManagementTool : UIViewController , IServiceDelegate
	{
		enum WebServiceType{VerifyPassword, UpdatePassword,ForgotPassword };
		enum PasswordRecoveryType {
			mail,text,site
		};
		PasswordRecoveryType recoveryType;
		WebServiceType WebType;
		public UIPopoverController objpopover;
		public userRegistrationViewController SuperParent;
		public string Type;
		LoadingOverlay ActivityIndicator;

		public PasswordManagementTool () : base ("PasswordManagementTool", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ViewCP.Hidden=true;
			ViewEditPassword.Hidden = true;
			ViewVerifyPassword.Hidden = true;
			ViewForgotPassword.Hidden = true;
			if (!(GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)) {
				btnForgotPassword.Hidden = true;
			}
			if (Type == "CREATE") {

				ViewCP.Hidden = false;
				CPtxtPassword.EditingDidEnd += (object sender, EventArgs e) => {
					if (CPtxtPassword.Text.Length > 0) {

						if (!((CPtxtPassword.Text.Length > 5) && (CPtxtPassword.Text.Length < 13))) {

							
                            UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Password must be 6 to 12 characters", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                            CPtxtPassword.Text = @"";
							return;
						}
					} else {
						CPtxtPassword.Text = @"";
						CPtxtPassword.Placeholder = @"Enter password ";



					}
				};

				CPtxtrepeatPassword.EditingDidEnd += (object sender, EventArgs e) => {
					if (!(CPtxtrepeatPassword.Text == CPtxtPassword.Text)) {

						CPtxtrepeatPassword.Text = @"";
						CPtxtrepeatPassword.Placeholder = @"Re-Enter password ";

					} else
						CPtxtrepeatPassword.Placeholder = @"";
				};
			} else if (Type == "EDIT") 
			{
				
				ViewVerifyPassword.Hidden = false;

				EPtxtNewPassword.EditingDidEnd += (object sender, EventArgs e) => {
					if (EPtxtNewPassword.Text.Length > 0) {

						if (!((EPtxtNewPassword.Text.Length > 5) && (EPtxtNewPassword.Text.Length < 13))) {

							
                            UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Password must be 6 to 12 characters", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                            EPtxtNewPassword.Text = @"";
							return;
						}
					} else {
						EPtxtNewPassword.Text = @"";
						EPtxtNewPassword.Placeholder = @"Enter password ";



					}
				};

				EPtxtRepeatPassword.EditingDidEnd += (object sender, EventArgs e) => {
					if (!(EPtxtRepeatPassword.Text == EPtxtNewPassword.Text)) {

						EPtxtRepeatPassword.Text = @"";
						EPtxtRepeatPassword.Placeholder = @"Re-Enter password ";

					} else
						EPtxtRepeatPassword.Placeholder = @"";
				};

			}
			// Perform any additional setup after loading the view, typically from a nib.
		}

		// EditPassword
		partial void EPbtnSavePasswordClicked (NSObject sender)
		{


			if (!(EPtxtNewPassword.Text.Length>0)) {


                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please enter password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
			if (!(EPtxtRepeatPassword.Text.Length>0)) {

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please re-enter password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}

			if (!(EPtxtNewPassword.Text == EPtxtRepeatPassword.Text)) {

				EPtxtRepeatPassword.Text=@"";

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please repeat same password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}

			else
			{
				if (!((EPtxtRepeatPassword.Text.Length>5) && (EPtxtRepeatPassword.Text.Length<13))) {

                    UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Password must be 6 to 12 characters.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}
            if (Reachability.CheckVPSAvailable()) {


				ActivityIndicator= new LoadingOverlay(View.Bounds, "Updating your password.. \nPlease wait..");

				this.View.Add (ActivityIndicator);
				OdataBuilder ObjOdata = new OdataBuilder ();
				WebType=WebServiceType.UpdatePassword;
				ObjOdata.RestService.Objdelegate = this;
				CheckPassword ObjPassword = new CheckPassword();
				ObjPassword.EmpNumber=GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				ObjPassword.Password=EPtxtNewPassword.Text.ToString();
				ObjOdata.UpdatePassword (ObjPassword);


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


			objpopover.Dismiss(true);
		}
		partial void EPbtnCloseBtnClicked (NSObject sender)
		{
			objpopover.Dismiss(true);

		}


		//VerifyPassword
		partial void VPbtnVerifyPasswordClicked (NSObject sender)
		{
			if (!((VPtxtPassword.Text.Length>5) && (VPtxtPassword.Text.Length<13))) {

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Password must be 6 to 12 characters.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                VPtxtPassword.Text=@"";
				return;
			}

            if (Reachability.CheckVPSAvailable()) {


				ActivityIndicator= new LoadingOverlay(View.Bounds, "Verifying password.. \nPlease wait..");

				this.ViewEditPassword.Add (ActivityIndicator);

				OdataBuilder ObjOdata = new OdataBuilder ();
				WebType=WebServiceType.VerifyPassword;
				ObjOdata.RestService.Objdelegate = this;
				CheckPassword ObjPassword = new CheckPassword();
				ObjPassword.EmpNumber=GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				ObjPassword.Password=VPtxtPassword.Text.ToString();
				ObjOdata.VerifyPassword (ObjPassword);


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
            UIAlertController okAlertController;

            switch (WebType) {
			case WebServiceType.VerifyPassword:

				if (jsonDoc ["IsValid"] == true) {
					ShowEditPasswordScreen ();
				} else {
                    okAlertController = UIAlertController.Create("WBidMax", "Incorrect Password, Please try again", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                }
				break;
			case WebServiceType.UpdatePassword:
				if (jsonDoc ["Status"] == true) {
					okAlertController = UIAlertController.Create("WBidMax", "You have saved password successfully", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
                            //  lblPasswordHeader.text=@"Password(is set)";
                            SuperParent.Password = EPtxtRepeatPassword.Text;
                            
                            objpopover.Dismiss(true);

                     }));
                      this.PresentViewController(okAlertController, true, null);
                    }
                    else {
                    okAlertController = UIAlertController.Create("WBidMax", "Incorrect Password, Please try again", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                }
				break;

			case WebServiceType.ForgotPassword:
				if (jsonDoc ["Status"] == true)
					{

					string Message="";
					switch (recoveryType) {
					case PasswordRecoveryType.text:
						Message="Password sent via sms";
						break;
					case PasswordRecoveryType.mail:
						Message="Password sent via email,Please check your mail.";
						break;
					case PasswordRecoveryType.site:
						Message="Password sent via wnco.com.";
						break;

					default:
						break;
					}
					okAlertController = UIAlertController.Create("WBidMax", Message, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) => {
                        objpopover.Dismiss(true);

                    }));
                        this.PresentViewController(okAlertController, true, null);

                    } else {
						if (jsonDoc["Message"] == "NoUserAccount")
						{
							
                            okAlertController = UIAlertController.Create("WBidMax", " We are unable to send your password, as you do not have an account.  Please create an account.", UIAlertControllerStyle.Alert);

                        }
						else if (jsonDoc["Message"] == "CheckEmail")
						{
                            okAlertController = UIAlertController.Create("WBidMax", "We are unable to send your password,Please try again or contact administrator.", UIAlertControllerStyle.Alert);

                        }
						else if (jsonDoc["Message"] == "CheckPhoneNumber")
						{
						
                            okAlertController = UIAlertController.Create("WBidMax", " We are unable to send your password, Please check your mobile number or contact administrator", UIAlertControllerStyle.Alert);

                        }
						else
						{ 	
                            okAlertController = UIAlertController.Create("WBidMax", " We are unable to send your password, Please contact administrator", UIAlertControllerStyle.Alert);
                        }
                       
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }

				break;
			}

		}
		public void ResponceError(string Error)
		{
			ActivityIndicator.Hide ();
			Console.WriteLine ("Service Fail");
            UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
        }
		public void ShowEditPasswordScreen()
		{
			ViewVerifyPassword.Hidden=true;
			ViewEditPassword.Hidden=false;
		}

		partial void VPbtnForgotPasswordClicked (NSObject sender)
		{
			ViewVerifyPassword.Hidden=true;
			ViewForgotPassword.Hidden=false;
		}
		//ForgotPassword 

		partial void btnSendViaMailClicked (NSObject sender)
		{
			recoveryType=PasswordRecoveryType.mail;
			PasswordRecoverySupport(1);
		}
		partial void btnSendViaTextButtonClicked (NSObject sender)
		{
			recoveryType=PasswordRecoveryType.text;
			PasswordRecoverySupport(3);
		}
		partial void btnSendViawncoClicked (NSObject sender)
		{
			recoveryType=PasswordRecoveryType.site;
			PasswordRecoverySupport(2);
		}
		void PasswordRecoverySupport(int Type)
		{
            if (Reachability.CheckVPSAvailable()) {


				ActivityIndicator= new LoadingOverlay(View.Bounds, "Verifying password.. \nPlease wait..");

				this.ViewForgotPassword.Add (ActivityIndicator);

				OdataBuilder ObjOdata = new OdataBuilder ();
				WebType=WebServiceType.ForgotPassword;
				ObjOdata.RestService.Objdelegate = this;
				ForgotPasswordDetails ObjPassword = new ForgotPasswordDetails();
				ObjPassword.EmpNum=int.Parse( GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToString());
				ObjPassword.Type=Type;
				ObjOdata.PasswordRecovery (ObjPassword);


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
		partial void btnForgotPasswordDismiss (NSObject sender)
		{
			ViewVerifyPassword.Hidden = false;
			ViewForgotPassword.Hidden=true;

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();
			this.View.Dispose ();


		}

		//Create Password
		partial void btnCreatePasswordClicked (NSObject sender)
		{
			

			if (!(CPtxtPassword.Text.Length>0)) {
				

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please enter password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
			if (!(CPtxtrepeatPassword.Text.Length>0)) {
				

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please re-enter password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}

			if (!(CPtxtPassword.Text == CPtxtrepeatPassword.Text)) {
				
				CPtxtrepeatPassword.Text=@"";

                UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Please repeat same password", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}

			else
			{
				if (!((CPtxtrepeatPassword.Text.Length>5) && (CPtxtrepeatPassword.Text.Length<13))) {
					
                    UIAlertController okAlertController = UIAlertController.Create("Warning!!", "Password must be 6 to 12 characters.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                    return;
				}
			}
		//	lblPasswordHeader.text=@"Password(is set)";
			SuperParent.Password=CPtxtrepeatPassword.Text;
			


			objpopover.Dismiss(true);
		}
		partial void btnDismissPopover (NSObject sender)
		{
			objpopover.Dismiss(true);
			//SuperParent.Password=CPtxtrepeatPassword.Text;
		}

	}
}

