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



namespace WBid.WBidiPad.iOS
{
	public partial class userRegistrationViewController : UIViewController,IServiceDelegate
	{
		public bool fromHome;
		public string EmployeeNumber;
		UIPopoverController popoverController;
		public string domicileName;
		public string Password="";
		NSObject	notif;
		WbidUser wbidUser;
		LoadingOverlay ActivityIndicator;
		enum WebServiceType{CreateUseraccount,UpdateUserAccount,VerifyPassword,UpdatePassword,checkUser};
		WebServiceType webType;
		RemoteUpdateUserInformation ObjremoteUserInfo;
		public userRegistrationViewController () : base ("userRegistrationViewController", null)
		{
		}


		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
		public void setCarriername(string carrier)
		{
			btnCellCarrier.SetTitle (carrier, UIControlState.Normal);
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();

			//foreach (UIView view in this.View.Subviews) {
				
			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();


		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
	
			pckrSelectDomicile.Model = new pickerViewModel (this);
			vwUserManage.Frame = new CGRect (20, 650, 500, 536);
			vwUserManage.Layer.BorderWidth = 1;

			txtFirstName.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			txtLastName.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			txtEmail.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			txtEmployeeNumber.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			//			txtSeniorityNumber.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			this.btnCancel.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnSubmit.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnAccept.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			notif	=NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString("ReloadUserPage"), Reloadpage);
			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)
			{
				//set label content "Change user Information"
				txtFirstName.Text = GlobalSettings.WbidUserContent.UserInformation.FirstName;
				txtLastName.Text = GlobalSettings.WbidUserContent.UserInformation.LastName;
				txtEmail.Text = GlobalSettings.WbidUserContent.UserInformation.Email;
				txtEmployeeNumber.Text = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				txtCellNumber.Text = GlobalSettings.WbidUserContent.UserInformation.CellNumber;


				int localCarrier =GlobalSettings.WbidUserContent.UserInformation.CellCarrier; 


				if(localCarrier >=0 && localCarrier <CommonClass.CellCarrier.Length )
					btnCellCarrier.SetTitle (CommonClass.CellCarrier[GlobalSettings.WbidUserContent.UserInformation.CellCarrier ], UIControlState.Normal);
				else
					btnCellCarrier.SetTitle (CommonClass.CellCarrier[0], UIControlState.Normal);



				if (GlobalSettings.WbidUserContent.UserInformation.isAcceptMail)
					waitchAcceptMail.SetState (true, false);
				else 
					waitchAcceptMail.SetState (false, false);

				if (GlobalSettings.WbidUserContent.UserInformation.isAcceptMail == null) 
				{
					waitchAcceptMail.SetState (true, false);
				}
				if (GlobalSettings.WbidUserContent.UserInformation.isAcceptUserTermsAndCondition)
					switchTermsAndCondition.SetState (true, false);
				else 
					switchTermsAndCondition.SetState (false, false);

				//                txtSeniorityNumber.Text = GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber.ToString();
				domicileName = GlobalSettings.WbidUserContent.UserInformation.Domicile;
				//				int indexToSelect = GlobalSettings.WBidINIContent.Domiciles.IndexOf(GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.WbidUserContent.UserInformation.Domicile));
				//                pckrSelectDomicile.Select(indexToSelect, 0, true);
				List<string> domList = GlobalSettings.WBidINIContent.Domiciles.OrderBy (x => x.DomicileName).Select (y => y.DomicileName).ToList ();
				int index = domList.IndexOf (domicileName);
				pckrSelectDomicile.Select(index, 0, true);

				if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
				{
					sgPosition.SelectedSegment = 0;
				}

				else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
				{
					sgPosition.SelectedSegment = 1;
				}

				if (GlobalSettings.WbidUserContent.UserInformation.IsFemale)
					sgMaleFemale.SelectedSegment = 1;
				else
					sgMaleFemale.SelectedSegment = 0;

				lblTitle.Text = "Change User Information";
				btnSubmit.SetTitle ("Update", UIControlState.Normal);
				
				
				btnCancel.Hidden = false;
				lblHeaderUserTerms.Hidden = true;
				switchTermsAndCondition.Hidden = true;
			}
			else
			{
				txtEmployeeNumber.Text = EmployeeNumber;

				domicileName = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).FirstOrDefault().DomicileName;
				//set label content "User Registration"
				lblTitle.Text = "User Registration";
				btnCancel.Hidden = true;
				btnSubmit.SetTitle ("Create", UIControlState.Normal);
				

				
				//btnSubmit.Frame = new CGRect (new CGPoint (240, 500), btnSubmit.Frame.Size);
			}
			txtEmployeeNumber.UserInteractionEnabled = false;
			//			txtCellNumber.ShouldChangeCharacters = (textField, range, replacement) =>
			//			{
			//				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
			//				int number;
			//				return newContent.Length <= 50 && (replacement.Length == 0 || int.TryParse(replacement, out number));
			//			};

			txtCellNumber.ShouldChangeCharacters = (textField, range, replacement) =>
			{				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
				int number;

				if(!(replacement.Length == 0 || int.TryParse(replacement, out number)))
				{
					return false;
				}
				int length=CheckMobileNumberLength(textField.Text.ToString());

				if(length ==10)
				{
					if(range.Length ==0)
						return false;
				}

				if(length == 3)
				{
					string formatnumber=numberFormat(textField.Text);
					textField.Text=formatnumber + "-";
					if(range.Length >0)
					{
						textField.Text=formatnumber.Substring(0,3);
					}
				}
				else if(length == 6)
				{
					string formatnumber=numberFormat(textField.Text);
					Console.WriteLine (formatnumber.Substring(0,3) );
					Console.WriteLine (formatnumber.Substring(3,3) );
					textField.Text=formatnumber.Substring(0,3) + "-"+formatnumber.Substring(3,3)+"-" ;

					if(range.Length >0)
					{
						textField.Text=formatnumber.Substring(0,3) +"-"+formatnumber.Substring(3,formatnumber.Length-3) ;
					}

				}
				return true ;
			};

			txtFirstName.ShouldChangeCharacters = (textField, range, replacement) =>
			{
				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
				//int number;
				return newContent.Length <= 50 ;
			};

			txtLastName.ShouldChangeCharacters = (textField, range, replacement) =>
			{
				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
				//int number;
				return newContent.Length <= 50 ;
			};
			txtEmail.ShouldChangeCharacters = (textField, range, replacement) =>
			{
				var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
				//int number;
				return newContent.Length <= 50 ;
			};


			txtEmail.EditingDidEnd += (object sender, EventArgs e) => {
				if(GlobalSettings.WbidUserContent !=null && txtEmail.Text!=GlobalSettings.WbidUserContent.UserInformation.Email&&txtRePass.Text!=txtEmail.Text) {
					vwRePass.Layer.BorderWidth = 1.0f;
					this.View.AddSubview(vwRePass);
					vwRePass.Center = this.View.Center;
					txtRePass.BecomeFirstResponder();
					btnSubmit.Enabled = false;
					txtRePass.Text = string.Empty;
				}
				
			};
			btnRePass.TouchUpInside += (object sender, EventArgs e) => {
				if(txtRePass.Text!=txtEmail.Text)
					txtEmail.Text = string.Empty;
				vwRePass.RemoveFromSuperview();
				btnSubmit.Enabled = true;
			};
			// Perform any additional setup after loading the view, typically from a nib.
			var tap = new UITapGestureRecognizer (secretTap);
			tap.NumberOfTouchesRequired = 1; // Change this value to 3 before submitting!
			this.View.AddGestureRecognizer (tap);

		}
		int CheckMobileNumberLength(string number)
		{
			number= number.Replace ("(", "");
			number= number.Replace (")", "");
			number= number.Replace (" ", "");
			number= number.Replace ("-", "");
			number= number.Replace ("+", "");
			int length = number.Length;
			return length;
		}

		string numberFormat(string number)
		{
			number= number.Replace ("(", "");
			number= number.Replace (")", "");
			number= number.Replace (" ", "");
			number= number.Replace ("-", "");
			number= number.Replace ("+", "");
			int length = number.Length;

			if(length >10)
			{
				number = number.Substring (length+1 - 10);
			}
			return number;
		}
		void Reloadpage (NSNotification obj)
		{
			Console.WriteLine("notif");
			ViewDidLoad ();
			//			collectionVw.View.RemoveFromSuperview ();
			//			collectionVw = null;
			//			setViews ();

			NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
		}
		
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			//if (fromHome)
			//	NSNotificationCenter.DefaultCenter.PostNotificationName ("ReloadHome", null);


		}


		public void secretTap(UITapGestureRecognizer gest) {
			if (gest.State == UIGestureRecognizerState.Ended) {
				int touchCount = 0;
				bool status = true;

				CGRect rect1 = new CGRect (new CGPoint (490,0 ), new CGSize (100, 100)); 
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
                                admin.fromHome = fromHome;
                                this.PresentViewController(admin, true, null);
                            }

                        }));

                        this.PresentViewController(alert, true, null);


                    } else {
						//leave it.
					}
				}
			}
		}
        	partial void btnCancelTapped (UIKit.UIButton sender)
		{
			this.DismissViewController(true,null);
		}
		partial void sgMaleFemaleTapped (UIKit.UISegmentedControl sender)
		{
		}
		partial void sgPositionValueChanged (UIKit.UISegmentedControl sender)
		{
		}

		partial void btnSubmitTapped(UIKit.UIButton sender)
		{

			//if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest == false) {
			if (Reachability.CheckVPSAvailable())
			{

				if (!(txtFirstName.Text.Length > 0))
				{
					UIAlertController okAlertController = UIAlertController.Create("Error", "Please enter First Name.", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}
				if (!(txtLastName.Text.Length > 0))
				{
					UIAlertController okAlertController = UIAlertController.Create("Error", "Please enter Last Name.", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}

				if (!RegXHandler.EmailValidation(txtEmail.Text))
				{

					UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Email ", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}

				if (!RegXHandler.EmployeeNumberValidation(txtEmployeeNumber.Text))
				{
					UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}




				if (!(btnCellCarrier.TitleLabel.Text.Length > 2))
				{
					UIAlertController okAlertController = UIAlertController.Create("Warning!", "Please select Cell Carrier", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}

				int cellNumberLength = CheckMobileNumberLength(txtCellNumber.Text.ToString());

				if ((cellNumberLength < 10))
				{
					UIAlertController okAlertController = UIAlertController.Create("Warning!", "Invalid cell number, Please enter cell number in xxx-xxx-xxxx format", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					this.PresentViewController(okAlertController, true, null);
					return;
				}
				List<AppDataBidFileNames> prevappdataBidfilenames = new List<AppDataBidFileNames>();
				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.AppDataBidFiles.Count > 0)
					prevappdataBidfilenames = GlobalSettings.WbidUserContent.AppDataBidFiles;
				wbidUser = new WbidUser();
				wbidUser.AppDataBidFiles = prevappdataBidfilenames;
				wbidUser.UserInformation = new UserInformation();
				wbidUser.UserInformation.FirstName = txtFirstName.Text;
				wbidUser.UserInformation.LastName = txtLastName.Text;
				wbidUser.UserInformation.Email = txtEmail.Text;
				wbidUser.UserInformation.CellNumber = txtCellNumber.Text;

				//TODO
				//	wbidUser.UserInformation.CellCarrier=btnCellCarrier.TitleLabel.Text;
				if (waitchAcceptMail.On)
					wbidUser.UserInformation.isAcceptMail = true;
				else wbidUser.UserInformation.isAcceptMail = false;

				if (switchTermsAndCondition.On)
					wbidUser.UserInformation.isAcceptUserTermsAndCondition = true;
				else wbidUser.UserInformation.isAcceptUserTermsAndCondition = false;

				//            if (domicileName == null)
				//            {
				//				domicileName="ATL";
				//                //return;
				//            }

				//            wbidUser.UserInformation.Domicile = domicileName;
				wbidUser.UserInformation.EmpNo = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(txtEmployeeNumber.Text, @"\d+").Value).ToString();


				if (sgPosition.SelectedSegment == 0)
				{
					wbidUser.UserInformation.Position = "Pilot";
				}
				else if (sgPosition.SelectedSegment == 1)
				{
					wbidUser.UserInformation.Position = "FA";
				}


				int index1 = Array.IndexOf(CommonClass.CellCarrier, btnCellCarrier.TitleLabel.Text);
				wbidUser.UserInformation.CellCarrier = index1;

				if (sgMaleFemale.SelectedSegment == 0)
					wbidUser.UserInformation.IsFemale = false;
				else
					wbidUser.UserInformation.IsFemale = true;

				//            wbidUser.UserInformation.SeniorityNumber = Convert.ToInt32(txtSeniorityNumber.Text);


				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)
				{
					//GlobalSettings.WbidUserContent.UserInformation.Domicile=wbidUser.UserInformation.Domicile;
					GlobalSettings.WbidUserContent.UserInformation.IsFemale = wbidUser.UserInformation.IsFemale;
					//GlobalSettings.WbidUserContent.UserInformation.Position=wbidUser.UserInformation.Position;

					//Update User account
					//updated paid until date--need to check Siva
					wbidUser.UserInformation.PaidUntilDate = GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate;

					CheckUserInformations(wbidUser);


					return;
				}
				else
				{
					//Create User account
					//if(Password!= null)
					//{
					//	if (!(Password.Length >0))
					//	{
					//                          UIAlertController okAlertController = UIAlertController.Create("Warning! ", "Please set the password", UIAlertControllerStyle.Alert);
					//                          okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					//                          this.PresentViewController(okAlertController, true, null);
					//                          return;
					//	}
					//}


					CreateUserAccount();
				}

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
		}

		void checkUserExist()
		{
            if (Reachability.CheckVPSAvailable()) {



				OdataBuilder ObjOdata = new OdataBuilder ();
				ObjOdata.RestService.Objdelegate = this;

				//				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)
				//				{
				ActivityIndicator= new LoadingOverlay(View.Bounds, "Checking user exist. \nPlease wait..");

				this.View.Add (ActivityIndicator);
				webType = WebServiceType.checkUser;
				InvokeInBackground (() => {
					ObjOdata.CheckRemoUserAccount (txtEmployeeNumber.Text.ToString ());
				});
				//				}
			} else {

                if(WBidHelper.IsSouthWestWifiOr2wire())
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

		void CreateUserAccount()
		{
			if(!switchTermsAndCondition.On)
			{
                UIAlertController okAlertController = UIAlertController.Create("Warning!", "You have to accept the terms and condition in order to use WBidMax App", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
            if (Reachability.CheckVPSAvailable()) {
				ObjremoteUserInfo = new RemoteUpdateUserInformation ();

				ObjremoteUserInfo.FirstName = txtFirstName.Text;
				ObjremoteUserInfo.LastName = txtLastName.Text;
				ObjremoteUserInfo.Email = txtEmail.Text;
				ObjremoteUserInfo.CellPhone = txtCellNumber.Text;
				ObjremoteUserInfo.AcceptEmail = waitchAcceptMail.On;
				ObjremoteUserInfo.EmpNum = int.Parse (txtEmployeeNumber.Text);
				ObjremoteUserInfo.CarrierNum = Array.IndexOf(CommonClass.CellCarrier,btnCellCarrier.TitleLabel.Text);
				//ObjremoteUserInfo.BidBase=pckrSelectDomicile.

				//ObjremoteUserInfo.Position =GlobalSettings.WbidUserContent.UserInformation.Position;
				if (sgPosition.SelectedSegment == 0) {
					ObjremoteUserInfo.Position = 4;
				} else if (sgPosition.SelectedSegment == 1) {
					ObjremoteUserInfo.Position = 3;
				} 

				ActivityIndicator= new LoadingOverlay(View.Bounds, "Creating  User Account. \nPlease wait..");

				this.View.Add (ActivityIndicator);
				OdataBuilder ObjOdata = new OdataBuilder ();
				webType = WebServiceType.CreateUseraccount;
				ObjOdata.RestService.Objdelegate = this;
				//ObjremoteUserInfo.Password = Password;
				InvokeInBackground (() => {
					ObjOdata.CreateUserAccount (ObjremoteUserInfo);
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
                //UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Connectivity not available", UIAlertControllerStyle.Alert);
                //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //this.PresentViewController(okAlertController, true, null);

            }
		}
		void CheckUserInformations(WbidUser wbiduser)
		{
			var DifferenceList = new List<KeyValuePair<string, string>> ();
			if (txtFirstName.Text != GlobalSettings.WbidUserContent.UserInformation.FirstName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("FirstName",txtFirstName.Text+","+GlobalSettings.WbidUserContent.UserInformation.FirstName));
			}

			if (txtLastName.Text != GlobalSettings.WbidUserContent.UserInformation.LastName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("LastName",txtLastName.Text+","+ GlobalSettings.WbidUserContent.UserInformation.LastName));
			}


			if (txtEmail.Text != GlobalSettings.WbidUserContent.UserInformation.Email)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Email",txtEmail.Text+","+GlobalSettings.WbidUserContent.UserInformation.Email));
			}

			if (txtCellNumber.Text != GlobalSettings.WbidUserContent.UserInformation.CellNumber)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("CellPhone",txtCellNumber.Text+","+GlobalSettings.WbidUserContent.UserInformation.CellNumber));
			}

			if (wbidUser.UserInformation.CellCarrier != GlobalSettings.WbidUserContent.UserInformation.CellCarrier)
			{
				int remoteCarrier =GlobalSettings.WbidUserContent.UserInformation.CellCarrier; 
				int localCarrier = wbidUser.UserInformation.CellCarrier;
				int CellcarrierCount = CommonClass.CellCarrier.Length;
				if(remoteCarrier >=0 && localCarrier >=0 && remoteCarrier < CellcarrierCount && localCarrier < CellcarrierCount)
					DifferenceList.Add(new KeyValuePair<string, string>("CarrierNum",localCarrier+","+GlobalSettings.WbidUserContent.UserInformation.CellCarrier));
			}

			string LocalPosition = "";
			string RemotePosition = "";
			//			if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP")
			//			{
			//				RemotePosition = "Captain";
			//			}
			//			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FO")
			//			{
			//				RemotePosition = "First Officer";
			//			}
			//			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
			//			{
			//				RemotePosition = "Flight Attendant";
			//			}
			//
			//			if (sgPosition.SelectedSegment == 0)
			//			{
			//				LocalPosition = "Captain";
			//			}
			//			else if (sgPosition.SelectedSegment == 1)
			//			{
			//				LocalPosition = "First Officer";
			//			}
			//			else if (sgPosition.SelectedSegment == 2)
			//			{
			//				LocalPosition = "Flight Attendant";
			//			}

			if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
			{
				RemotePosition = "Pilot";
			}
			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
			{
				RemotePosition = "Flight Attendant";
			}

			if (sgPosition.SelectedSegment == 0)
			{
				LocalPosition = "Pilot";
			}

			else if (sgPosition.SelectedSegment == 1)
			{
				LocalPosition = "Flight Attendant";
			}

			if (LocalPosition != RemotePosition)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Position",LocalPosition+","+ RemotePosition));
			}


			if (DifferenceList.Count > 0) {
				UserAccountDifferenceScreen ObjUserDifference = new UserAccountDifferenceScreen ();
				ObjUserDifference.DifferenceList = DifferenceList;
				ObjUserDifference.iSfromHome = false;
				ObjUserDifference.isAcceptMail=waitchAcceptMail.On;
				ObjUserDifference.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController (ObjUserDifference, true, null);

			} else {
				ChangeInAcceptmailOnly (wbiduser);
			}
		}

		void ChangeInAcceptmailOnly(WbidUser wbiduser)
		{
			if (waitchAcceptMail.On != GlobalSettings.WbidUserContent.UserInformation.isAcceptMail) 
			{
				ObjremoteUserInfo = new RemoteUpdateUserInformation ();
				ObjremoteUserInfo.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.isAcceptMail;
				ObjremoteUserInfo.FirstName = GlobalSettings.WbidUserContent.UserInformation.FirstName;
				ObjremoteUserInfo.LastName = GlobalSettings.WbidUserContent.UserInformation.LastName;
				ObjremoteUserInfo.Email = GlobalSettings.WbidUserContent.UserInformation.Email;
				ObjremoteUserInfo.CellPhone = GlobalSettings.WbidUserContent.UserInformation.CellNumber;
				ObjremoteUserInfo.AcceptEmail = waitchAcceptMail.On;
				ObjremoteUserInfo.EmpNum = int.Parse (GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToString ());
				ObjremoteUserInfo.CarrierNum = GlobalSettings.WbidUserContent.UserInformation.CellCarrier;
				//ObjremoteUserInfo.BidBase = GlobalSettings.WbidUserContent.UserInformation.Domicile;
				//ObjremoteUserInfo.BidSeats = GlobalSettings.WbidUserContent.UserInformation.BidSeat;
				//ObjremoteUserInfo.Position =GlobalSettings.WbidUserContent.UserInformation.Position;
				//				if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP") {
				//					ObjremoteUserInfo.Position = 5;
				//				} else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FO") {
				//					ObjremoteUserInfo.Position = 4;
				//				} else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA") {
				//					ObjremoteUserInfo.Position = 3;
				//				}

				if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot") 
				{
					ObjremoteUserInfo.Position = 4;
				} 
				else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA") {
					ObjremoteUserInfo.Position = 3;
				}

				UpdateCWAMAsterDB (ObjremoteUserInfo);

				//ObjremoteUserInfo.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.isAcceptMail;

			} else {
				UpdateUserDetails (wbiduser);
			}
		}

		void UpdateCWAMAsterDB(RemoteUpdateUserInformation ObjremoteUserInfo)
		{
            if (Reachability.CheckVPSAvailable()) {


				ActivityIndicator= new LoadingOverlay(View.Bounds, "Updating User Information. \nPlease wait..");

				this.View.Add (ActivityIndicator);
				OdataBuilder ObjOdata = new OdataBuilder ();
				webType = WebServiceType.UpdateUserAccount;
				ObjOdata.RestService.Objdelegate = this;
				InvokeInBackground (() => {
					ObjOdata.UpdateUserAccount (ObjremoteUserInfo);
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
			Console.WriteLine ("Service Success");
			InvokeOnMainThread(()=>{
				ActivityIndicator.Hide ();
                UIAlertController okAlertController;


                switch (webType) {
				case WebServiceType.UpdateUserAccount:
					if (jsonDoc ["Status"] == true) {
						GlobalSettings.WbidUserContent.UserInformation.isAcceptMail = waitchAcceptMail.On;
                        
                           okAlertController = UIAlertController.Create("WBidMax", "User account Updated successfully", UIAlertControllerStyle.Alert);
                           okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        } else 
					{
                            okAlertController = UIAlertController.Create("WBidMax", jsonDoc["Message"], UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                        }
					break;

				case WebServiceType.checkUser:
					int empNo = int.Parse (jsonDoc ["EmpNum"].ToString ());
					if (empNo != 0) {
						ObjremoteUserInfo = CommonClass.ConvertJSonToObject<RemoteUpdateUserInformation> (jsonDoc.ToString ());
						SaveLocalUserDetails ();
						CheckUserInformations(wbidUser);
					}
					else
						CreateUserAccount ();
					break;
				case WebServiceType.CreateUseraccount:
					if (jsonDoc ["Status"] == true) {
						UpdateUserDetails (wbidUser);
						
					}
					else 
					{
                            okAlertController = UIAlertController.Create("WBidMax", jsonDoc["Message"], UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);

                        }
					break;

				}


			});

		}
		void SaveNewUserDetails()
		{

		}
		void SaveLocalUserDetails()
		{

			GlobalSettings.WbidUserContent.UserInformation.isAcceptMail=ObjremoteUserInfo.AcceptEmail ;
			GlobalSettings.WbidUserContent.UserInformation.FirstName=ObjremoteUserInfo.FirstName;
			GlobalSettings.WbidUserContent.UserInformation.LastName=ObjremoteUserInfo.LastName ;
			GlobalSettings.WbidUserContent.UserInformation.Email=ObjremoteUserInfo.Email ;
			GlobalSettings.WbidUserContent.UserInformation.CellNumber=ObjremoteUserInfo.CellPhone;

			GlobalSettings.WbidUserContent.UserInformation.EmpNo=ObjremoteUserInfo.EmpNum.ToString();
			GlobalSettings.WbidUserContent.UserInformation.CellCarrier=ObjremoteUserInfo.CarrierNum ;
			//GlobalSettings.WbidUserContent.UserInformation.Domicile = ObjremoteUserInfo.BidBase;
			//GlobalSettings.WbidUserContent.UserInformation.Position = ObjremoteUserInfo.BidSeats;
			//			if (ObjremoteUserInfo.Position == 5)
			//			{
			//				GlobalSettings.WbidUserContent.UserInformation.Position = "CP";
			//			}
			//			else if (ObjremoteUserInfo.Position == 4)
			//			{
			//				GlobalSettings.WbidUserContent.UserInformation.Position = "FO";
			//			}
			//			else if (ObjremoteUserInfo.Position == 3)
			//			{
			//				GlobalSettings.WbidUserContent.UserInformation.Position = "FA";
			//			}
			if (ObjremoteUserInfo.Position == 4)
			{
				GlobalSettings.WbidUserContent.UserInformation.Position = "Pilot";
			}
			else if (ObjremoteUserInfo.Position == 3)
			{
				GlobalSettings.WbidUserContent.UserInformation.Position = "FA";
			}

		}
		public void ResponceError(string Error)
		{
			InvokeOnMainThread (() => {
				ActivityIndicator.Hide ();
				Console.WriteLine ("Service Fail");
                UIAlertController okAlertController = UIAlertController.Create("WBidMax", Error, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                //ViewDidLoad();
            });
		}
		void UpdateUserDetails(WbidUser wbidUser)
		{
			GlobalSettings.WbidUserContent = wbidUser;
			if (File.Exists(WBidHelper.WBidUserFilePath))
			{
				WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);
				this.DismissViewController(true, null);
				//				this.DismissViewController(true, ()=>{
				//					UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				//					UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
				//				});
			}
			else
			{
				showUserManagement();
			}
		}

		private void showUserManagement ()
		{
			UIView.Animate(.25, 0, UIViewAnimationOptions.CurveLinear, ()=>
				{
					vwUserManage.Frame = new CGRect(20, 64, 500, 536);
				}, null );

			//swSync.Enabled = false;
			//swAutoSave.Enabled = false;

			txtAutoSaveValue.Background = UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5));
			swEmail.ValueChanged += swBidEmailChanged;
			swSync.ValueChanged += swSyncChanged;
			swAutoSave.ValueChanged += swAutoSaveChanged;
			stprAutoSave.ValueChanged += stprAutoSaveTimeChanged;

			swEmail.On = true;// GlobalSettings.WBidINIContent.User.IsOn;
			swSync.On = false;//GlobalSettings.WBidINIContent.User.SmartSynch;
			swAutoSave.On = false;// GlobalSettings.WBidINIContent.User.AutoSave;
			stprAutoSave.Value = 5;// GlobalSettings.WBidINIContent.User.AutoSavevalue;
			txtAutoSaveValue.Text = "5";// GlobalSettings.WBidINIContent.User.AutoSavevalue.ToString();
			swCrashReport.On = true;

			if (swAutoSave.On)
				stprAutoSave.Enabled = true;
			else
				stprAutoSave.Enabled = false;
		}

		void stprAutoSaveTimeChanged (object sender, EventArgs e)
		{
			txtAutoSaveValue.Text = ((UIStepper)sender).Value.ToString ();
			GlobalSettings.WBidINIContent.User.AutoSavevalue = (int)((UIStepper)sender).Value;
		}

		void swAutoSaveChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.AutoSave = ((UISwitch)sender).On;
			if (((UISwitch)sender).On)
				stprAutoSave.Enabled = true;
			else
				stprAutoSave.Enabled = false;
		}

		void swSyncChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.SmartSynch = ((UISwitch)sender).On;
			GlobalSettings.SynchEnable = ((UISwitch)sender).On;

		}

		void swBidEmailChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.IsOn = ((UISwitch)sender).On;
		}

		partial void btnTipTapped (UIKit.UIButton sender)
		{
			switch(sender.Tag)
			{
			case 1:
				{
					TooltipViewController toolTip = new TooltipViewController ();
					toolTip.tipMessage = "Setting this to On will send a bid receipt to the email address in your User Information settings.";
					popoverController = new UIPopoverController (toolTip);
					popoverController.PopoverContentSize = new CGSize (200, 100);
					popoverController.PresentFromRect(sender.Frame,vwUserManage,UIPopoverArrowDirection.Any,true);
				}
				break;
			case 2:
				{
					TooltipViewController toolTip = new TooltipViewController ();
					toolTip.tipMessage = "Setting this on will send your state file to the WBid cloud.  This will allow you to synchronize your current bid state between different platforms.  For example: start at home on a desktop with WBidMax and continue the same bid on your iPad with WBid-iPad. (Not available now)";
					popoverController = new UIPopoverController (toolTip);
					popoverController.PopoverContentSize = new CGSize (300, 150);
					popoverController.PresentFromRect(sender.Frame,vwUserManage,UIPopoverArrowDirection.Any,true);
				}
				break;
			case 3: 
				{
					TooltipViewController toolTip = new TooltipViewController ();
					toolTip.tipMessage = "Setting this feature on will automatically save your bid state at the set interval in minutes.";
					popoverController = new UIPopoverController (toolTip);
					popoverController.PopoverContentSize = new CGSize (250, 100);
					popoverController.PresentFromRect(sender.Frame,vwUserManage,UIPopoverArrowDirection.Any,true);
				}
				break;
			case 4:
				{
					TooltipViewController toolTip = new TooltipViewController ();
					toolTip.tipMessage = "Every modern program has bugs.  WBid-iPad is no exception. 99% of the operation will be bug free, but since there are trillions of possible combinations with all the features of WBid-iPad, an undiscovered bug may exist.  WBid-iPad will capture any crash, and send important computer code information (where the crash occurred, settings of WBid-iPad, etc) to the WBid mail server.  Setting this feature on, helps us maintain WBid-iPad to the highest of standards.  We do not capture any personal information.";
					popoverController = new UIPopoverController (toolTip);
					popoverController.PopoverContentSize = new CGSize (200, 400);
					popoverController.PresentFromRect(sender.Frame,vwUserManage,UIPopoverArrowDirection.Any,true);
				}
				break;
			}
		}

		private bool Validate()
		{
			if (txtFirstName.Text == string.Empty)
			{

			}
			if (txtLastName.Text == string.Empty)
			{ 
			}
			if (txtEmployeeNumber.Text == string.Empty)
			{ 
			}
			//            if (txtSeniorityNumber.Text == string.Empty)
			//            { 
			//            }
			return true;
		}


		public bool EmailValidation(string value)
		{
			string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

			if (!Regex.IsMatch(value, matchpattern))
			{
				return false;
			}
			return true;
		}
		private bool EmployeeNumberValidation()
		{
			if (!Regex.Match(txtEmployeeNumber.Text, "^[e,E,x,X,0-9][0-9]*$").Success)
			{
				return false;
			}
			return true;
		}

		partial void btnAcceptTapped (UIKit.UIButton sender)
		{

			WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
			//this.DismissViewController(true, null);
			this.DismissViewController(true, ()=>{
				var version = UIDevice.CurrentDevice.SystemVersion.Split ('.');
				string compVersion = version [0] + "." + version [1];
				if (float.Parse (compVersion) < 8.0) {
					UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
					UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
				} else {
					UIUserNotificationSettings notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings (notificationSettings);
					UIApplication.SharedApplication.RegisterForRemoteNotifications ();
				}
			});

			GlobalSettings.WBidINIContent.User.AutoSave = swAutoSave.On;
			GlobalSettings.WBidINIContent.User.AutoSavevalue = (txtAutoSaveValue.Text.Trim() == string.Empty) ? 0 : int.Parse(txtAutoSaveValue.Text);
			GlobalSettings.WBidINIContent.User.IsNeedBidReceipt = swEmail.On;
			GlobalSettings.WBidINIContent.User.IsNeedCrashMail = swCrashReport.On;
			GlobalSettings.WBidINIContent.User.SmartSynch = swSync.On;

			WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());



		}
		//		partial void btnSelectDomicleTapped (MonoTouch.Foundation.NSObject sender)
		//		{
		//			UIButton btn = (UIButton)sender;
		//			string[] arr = GlobalSettings.WBidINIContent.Domiciles.Select(x=>x.DomicileName).ToArray();
		//			UIActionSheet sheet = new UIActionSheet("Select Domicile",null,"Cancel",null,arr);
		//			sheet.ShowFrom(btn.Frame,this.View,true);
		//			sheet.Clicked += handleDomicleSelect;}
		//		void handleDomicleSelect (object sender, UIButtonEventArgs e)
		//		{
		//			List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles;
		//			btnSelectDomicle.SetTitle (listDomicile [e.ButtonIndex].DomicileName, UIControlState.Normal);
		//			if (e.ButtonIndex == 0) {
		//				//listdomicle[]
		//			}
		//
		//		}
		partial void btnCellCarrierClicked (NSObject sender)
		{
			UIButton btn = (UIButton)sender;
			CellCarrierPicker ObjPickerr= new CellCarrierPicker();
			popoverController = new UIPopoverController (ObjPickerr);
			ObjPickerr.objpopover=popoverController;
			ObjPickerr.SuperParent=this;
			popoverController.PopoverContentSize = new CGSize (ObjPickerr.View.Frame.Width, ObjPickerr.View.Frame.Height);
			popoverController.PresentFromRect(btn.Frame,View,UIPopoverArrowDirection.Any,true);

		}
		
	

		partial void ConditionsBtnClicked (NSObject sender)
		{
			UIButton btn = (UIButton)sender;

			WebViewForLicence webViewController=new WebViewForLicence();


			if(btn.Tag == 20)
			{
				webViewController.isLicence=false;
			}
			else if (btn.Tag ==21)
			{
				webViewController.isLicence=true;
			}

			webViewController.ModalPresentationStyle= UIModalPresentationStyle.FormSheet;
			this.PresentViewController(webViewController,true,null);
		}

		partial void btnHelpTapped (UIKit.UIButton sender)
		{
			if(sender.Tag == 1)//Seniority Number help
			{
				UILabel tip = new UILabel();
				tip.Frame = new CGRect(sender.Center.X + 5,sender.Frame.Y - 220 ,150,350);
				tip.Lines = 30;
				tip.Text = @"The Seniority Number is your relative position in Domicile.  It is used to suggest the number of bid lines to submit.  If you are 30 in domicile, WBid will suggest you can submit either 30 bids lines or all of the lines.";
				tip.Layer.BorderWidth = 1;
				UIEdgeInsets inset = new UIEdgeInsets(10,10,0,0);
				//				CGRect paddedFrame = UIEdgeInsetsInsetRect(initialFrame, contentInsets);
				//				RectangleF frame = inset.InsetRect(tip.Frame);
				//				tip.Frame = frame;

				tip.Layer.CornerRadius = 3.0f;
				tip.Layer.BackgroundColor = UIColor.White.CGColor;
				tip.Layer.ShadowColor = UIColor.Black.CGColor;
				tip.Layer.ShadowOpacity = 0.8f;
				tip.Layer.ShadowRadius = 5.0f;
				tip.Layer.ShadowOffset = new CGSize(3f, 3f);
				this.View.Add(tip);
				this.PerformSelector(new ObjCRuntime.Selector("removePopUp:"), tip, 4);
			}
		}
		[Export("removePopUp:")]
		private void removePopUp(UILabel label)
		{
			label.RemoveFromSuperview ();
		}

		public class pickerViewModel : UIPickerViewModel
		{
			userRegistrationViewController parent;
			string[] arr = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();

			public pickerViewModel(userRegistrationViewController parentVC)
			{
				parent = parentVC;
			}

			public override nint GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView picker, nint component)
			{
				return arr.Count();
			}

			public override string GetTitle (UIPickerView picker, nint row, nint component)
			{
				return arr[row];
			}
			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				parent.domicileName = arr[row];
			}

		}

	}
}

