using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class ContactUsViewController : UIViewController
    {
        LoadingOverlay loadingOverlay;
		public ContactUsViewController () : base ("ContactUsViewController", null)
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
			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			txtEmailField.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			txtNameField.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
			txtPhoneField.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
            txtEmpNum.Background = UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5));
			btnCancel.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			btnSend.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

            txtEmailField.Text = GlobalSettings.WbidUserContent.UserInformation.Email;
            txtEmpNum.Text = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
            txtPhoneField.Text = GlobalSettings.WbidUserContent.UserInformation.CellNumber;
            txtNameField.Text = GlobalSettings.WbidUserContent.UserInformation.FirstName + " " + GlobalSettings.WbidUserContent.UserInformation.LastName;
			lblVersion.Text = "Version: " + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
            wifiVersion.Text = WBidHelper.CurrentSSID();

			// Perform any additional setup after loading the view, typically from a nib.
		}
        partial void btnSendTapped(UIKit.UIButton sender)
        {
            loadingOverlay = new LoadingOverlay(View.Bounds, "Sending..Please Wait..");
           

			if(!RegXHandler.EmailValidation(txtEmailField.Text))
			{
			
                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Email Id.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
            if (!RegXHandler.EmployeeNumberValidation(txtEmpNum.Text))
            {

                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
            }
			if(!RegXHandler.PhoneNumberValidation(txtPhoneField.Text))
			{
			
                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Phone Number.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
			if(!RegXHandler.NameValidation(txtNameField.Text))
			{
			
                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Name.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
			//if(!RegXHandler.AnythingValidation(txtDescription.Text))
            if(txtDescription.Text.Length<=0)
			{
				
                UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Description.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                return;
			}
           
             //Check internet wavailable
			//if(GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest==false)
			//{
                if (Reachability.CheckVPSAvailable())
			{
				View.Add(loadingOverlay);
              	SendMailToAdmin();
           }
           else
           {
                    if (WBidHelper.IsSouthWestWifiOr2wire()) {
						
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", Constants.SouthWestConnectionAlert, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);

                    } else {
             
                    UIAlertController okAlertController = UIAlertController.Create("Error",Constants.VPSDownAlert , UIAlertControllerStyle.Alert);
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
            
		
     /// <summary>
        /// send mail to the admin
        /// </summary>
        /// <param name="email"></param>
        public void SendMailToAdmin()
        {

            var sb = new StringBuilder();
            sb.Append("<table width=\"500\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Hi Admin ,</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">&nbsp;</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px; padding: 0 0 10px 0;\">");
            sb.Append(txtDescription.Text);
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("<table width=\"250\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td></td><td></td><td></td></tr><tr><td colspan=\"3\">&nbsp;</td></tr><tr><td colspan=\"3\">&nbsp;</td></tr>");
            sb.Append("<tr><td align=\"left\" width=\"127px\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Name</td>");
            sb.Append("<td>:</td><td width=\"373px\" align=\"left\" valign=\"top\">");
            sb.Append(txtNameField.Text);
            sb.Append("</td></tr><tr>");
            sb.Append("<td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Emp no </td><td width=\"10\">:</td><td align=\"left\" valign=\"top\">");
            sb.Append(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
            sb.Append("</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Email </td><td width=\"10\">:</td><td align=\"left\" valign=\"top\">");
            sb.Append(txtEmailField.Text);

            sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Phone No </td><td>:</td>");
            sb.Append("<td align=\"left\" valign=\"top\">");
            sb.Append(txtPhoneField.Text);
            sb.Append("</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"middle\"  style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Version</td><td>:</td>");
            sb.Append("<td align=\"left\" valign=\"top\">");
            sb.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\"  style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Installation Date </td><td>:</td>");
            //sb.Append("<td align=\"left\" valign=\"top\">");
            //sb.Append(installeddate);
            //sb.Append("</td></tr>");

            //sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" >");
            //sb.Append("<br/>");
            //sb.Append(SortState());
            //sb.Append(WeightStates().ToString());
            //sb.Append(ConstraintsStates().ToString());
            //sb.Append(GetOSInformation());
            //UIDevice.CurrentDevice.SystemVersion
            sb.Append("</td></tr>");
            if (GlobalSettings.CurrentBidDetails != null)
            {
                sb.Append("</td></tr>");

                sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Domicile </td><td>:</td>");
                sb.Append("<td align=\"left\" valign=\"top\">");
                sb.Append(GlobalSettings.CurrentBidDetails.Domicile);
                sb.Append("</td></tr>");
                sb.Append("</td></tr>");

                sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Position </td><td>:</td>");
                sb.Append("<td align=\"left\" valign=\"top\">");
                sb.Append(GlobalSettings.CurrentBidDetails.Postion);
                sb.Append("</td></tr>");
            }
            sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">iOS Version </td><td>:</td>");
            sb.Append("<td align=\"left\" valign=\"top\">");
            sb.Append(UIDevice.CurrentDevice.SystemVersion);
            sb.Append("</td></tr>");

            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\"><br/><br/>Sincerely ,</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\">");
            sb.Append(txtNameField.Text);
            sb.Append("</td></tr>");
            sb.Append("</table>");
            string email = txtEmailField.Text;
            WBidMail objMailAgent = new WBidMail();

            InvokeInBackground(() =>
            {

                objMailAgent.SendContactUsMailtoAdmin(sb.ToString(), email, "WBidMax Support");

                InvokeOnMainThread(() =>
                {
                    loadingOverlay.Hide();
                    this.DismissViewController(true, null);
                });
            });


        }

		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			this.DismissViewController(true,null);
		}
	}
}

