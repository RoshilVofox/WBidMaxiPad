using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Text.RegularExpressions;
using System.ServiceModel;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.Generic;
using iOSPasswordStorage;
using Security;
namespace WBid.WBidiPad.iOS
{
	public partial class loginViewController : UIViewController
	{
		public bool isFromSubmit;
		public loginViewController () : base ("loginViewController", null)
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

			foreach (UIView view in this.View.Subviews) {

				//DisposeClass.DisposeEx(view);
			}
			//this.View.Dispose ();

			Console.WriteLine("Sample");
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

			if (SystemVersion > 12)
			{
                ModalInPresentation = true;
			}
			this.setLoginCredentialsFromKeychaninToTextField ();
			this.btnCancel.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			this.btnLogin.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

		}

		#region IBAction
		partial void btnLoginTapped (Foundation.NSObject sender)
		{
			if (Regex.Match(this.txtUserName.Text, "^[e,E,x,X,0-9][0-9]*$").Success)
			{
				if(this.txtPassword.Text.Length > 0)
				{
					string uName = this.txtUserName.Text.ToLower();
					if(uName[0] != 'e' && uName[0] != 'x')
						uName = "e" + uName;

					this.saveToKeyChain(uName,this.txtPassword.Text,"WBid.WBidiPad.cwa");
					this.DismissViewController(true, () => {
						NSNotificationCenter.DefaultCenter.PostNotificationName("loginDetailsEntered",null);
					});
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
		#endregion
		#region SetLoginDetails
		private void setLoginCredentialsFromKeychaninToTextField()
		{
			try{
				this.txtUserName.Text = KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
				this.txtPassword.Text = KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);
			}catch{
				Console.WriteLine ("Setting credentials execprion");
			}
		}
		#endregion
		#region KeyChain Access

		public void saveToKeyChain(string uName, string pass, string service)
		{
			var userResult = KeychainHelpers.SetPasswordForUsername ("user", uName, service, SecAccessible.Always, false);
			var passResult = KeychainHelpers.SetPasswordForUsername ("pass", pass, service, SecAccessible.Always, false);
			if (!((userResult == Security.SecStatusCode.Success) && (passResult == Security.SecStatusCode.Success))) {
                UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
                WindowAlert.RootViewController = new UIViewController();
                UIAlertController okAlertController = UIAlertController.Create("Oops", "Couldn't save information sucurely, please try again.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                WindowAlert.MakeKeyAndVisible();
                WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                WindowAlert.Dispose();
                return;

            }
		}
		#endregion
		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(CommonClass.bidObserver);
			this.DismissViewController(true, null);
		}
	}
}

