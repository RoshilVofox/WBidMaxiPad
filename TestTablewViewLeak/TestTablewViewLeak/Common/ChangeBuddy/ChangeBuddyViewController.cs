using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;

namespace WBid.WBidiPad.iOS
{
	public partial class ChangeBuddyViewController : UIViewController
	{
		LoadingOverlay overlay;
        public Dictionary<int, string> EmployeeList { get; set; }
		public ChangeBuddyViewController () : base ("ChangeBuddyViewController", null)
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
            LoadBidderDetails();
			overlay = new LoadingOverlay(this.View.Frame, "Checking Buddy Bidder Authorization.");
			// Perform any additional setup after loading the view, typically from a nib.

			txtBuddy1.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                if (EmployeeList != null)
                {
                    var buddyname = EmployeeList.FirstOrDefault(x => x.Key.ToString() == newText).Value;
                    lblBuddy1.Text = (buddyname == string.Empty || buddyname == null) ? "< No Matching Element >" : buddyname;
                }
               
				int val;
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};

			txtBuddy2.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                if (EmployeeList != null)
                {
                    var buddyname = EmployeeList.FirstOrDefault(x => x.Key.ToString() == newText).Value;
                    lblBuddy2.Text = (buddyname == string.Empty || buddyname == null) ? "< No Matching Element >" : buddyname;
                }
				int val;
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};


		}

        private void LoadBidderDetails()
        {
            if (GlobalSettings.ClearBuddyBid)
            {
                txtBuddy1.Text = txtBuddy2.Text = "0";
            }
            else
            {
                if (File.Exists(WBidHelper.GetAppDataPath() + "/falistwb4.dat"))
                {
                    EmployeeList = (Dictionary<int, string>)WBidHelper.DeSerializeObject(WBidHelper.GetAppDataPath() + "/falistwb4.dat");
                    txtBuddy1.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
                    txtBuddy2.Text = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;
                }
            }
        }

		void CheckValidSubscriptionForEmployeesCompleted (object sender, CheckValidSubscriptionForEmployeesCompletedEventArgs e)
		{
			
			if (e.Result != null) {
				List<AuthStatusModel> authStatus = e.Result.ToList();
				var authfailedmembers = authStatus.Where(x => x.IsValid == false).ToList();
				if (authfailedmembers.Count () > 0) 
				{
					string message = string.Empty;
					foreach (var item in authfailedmembers) {
						message += item.EmployeeNumber + " : " + item.Message + "\n\n";
					}


					InvokeOnMainThread (() => {
					
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "All bidders must have a WBidMax account .See the details."+message, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        overlay.RemoveFromSuperview();
					});


				} else {
					InvokeOnMainThread (() => {
						overlay.RemoveFromSuperview();
						GlobalSettings.WBidINIContent.BuddyBids.Buddy1 = txtBuddy1.Text.Trim ();
						GlobalSettings.WBidINIContent.BuddyBids.Buddy2 = txtBuddy2.Text.Trim ();
						//save the state of the INI File
						WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

						NSNotificationCenter.DefaultCenter.PostNotificationName ("addedBuddyBidder", null);
						this.DismissViewController (true, null);
					});

				}
			}
		}

		partial void btnOKTapped (UIKit.UIButton sender)
		{
            try
            {
				//EmployeeDetails empdetails = new EmployeeDetails();
				//empdetails.EmployeeNumbers=new Employee[2];
				//var obj=new List<Employee>();
				if(string.IsNullOrEmpty(txtBuddy1.Text.Trim()))
					txtBuddy1.Text="0";
				if(string.IsNullOrEmpty(txtBuddy2.Text.Trim()))
					txtBuddy2.Text="0";

				GlobalSettings.WBidINIContent.BuddyBids.Buddy1 = txtBuddy1.Text.Trim();
				GlobalSettings.WBidINIContent.BuddyBids.Buddy2 = txtBuddy2.Text.Trim();
				                //save the state of the INI File
				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				
				NSNotificationCenter.DefaultCenter.PostNotificationName("addedBuddyBidder",null);
				this.DismissViewController(true, null);


            }
            catch (Exception ex)
            {
                throw ex;
               
            }
                            
		}
		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			this.DismissViewController(true,null);
		}
		partial void btnDismiss (Foundation.NSObject sender)
		{
			this.DismissViewController(true,null);
		}
	}
}

