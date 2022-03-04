using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Parser;
using TestTablewViewLeak.ViewControllers;

namespace WBid.WBidiPad.iOS
{
	public partial class adminArea : UIViewController
	{
		public bool fromHome;
        LoadingOverlay loadingOverlay;
        UIPopoverController popoverController;
        public adminArea () : base ("adminArea", null)
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
			lblWifiName.Text= WBidHelper.CurrentSSID ().ToString ();
			base.ViewDidLoad ();
			if (GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest) {
				segWifi.SelectedSegment = 0;
			} 
			else
			{
				segWifi.SelectedSegment = 1;
			}


            if (GlobalSettings.buddyBidTest)
            {
                sgQATest.SelectedSegment = 0;
            }
            else
            {
                sgQATest.SelectedSegment = 1;

            }
			if (GlobalSettings.IsSouthWestPaidCheck)
			{
				SegIsPaid.SelectedSegment = 0;
			}
			else
			{
				SegIsPaid.SelectedSegment = 1;
			}

            if (GlobalSettings.WBidINIContent.Data.IsCompanyData)
                sgMock.SelectedSegment = 1;
            else
                sgMock.SelectedSegment = 0;

			SgSenList.SelectedSegment = (GlobalSettings.IsNewSeniorityListFormat) ? 0 : 1;
			// Perform any additional setup after loading the view, typically from a nib.
			this.btnSubmit.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);

			if (fromHome)
				btnReparse.Enabled = false;
		}
	
		partial void WifiValueChanged (NSObject sender)
		{


		} 
		partial void SenListValueChanged(NSObject sender)
		{
			
		}
		partial void btnBackTapped (Foundation.NSObject sender)
		{
			this.DismissViewController(true,null);
		}
        partial void btnCrash(NSObject sender)
        {
            int a = 0;
            int b = 0;
            int c = a / b;
        }
        partial void btnSelectDomicilesAction(UIButton sender)
        {

            SecretDataDownload ObjSecretDataDownlaodView = new SecretDataDownload();
            popoverController = new UIPopoverController(ObjSecretDataDownlaodView);
//            ObjSecretDataDownlaodView.SuperParent = this;
            CGRect frame = new CGRect((View.Frame.Size.Width / 2) - 75, (View.Frame.Size.Height / 2) - 150, 150, 350);
            popoverController.PopoverContentSize = new CGSize(ObjSecretDataDownlaodView.View.Frame.Width, ObjSecretDataDownlaodView.View.Frame.Height);
            popoverController.PresentFromRect(frame, View, 0, true);
 

        }




        partial void btnReparseTapped (Foundation.NSObject sender)
        {
		    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
            WindowAlert.RootViewController = new UIViewController();
            UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Do you want to test the vacation correction?", UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (actionCancel) => {
                if (btnUserIdCheckBox.Selected)
                {
                    GlobalSettings.IsDifferentUser = true;
                    GlobalSettings.ModifiedEmployeeNumber = txtUserId.Text;
                }
                TripTtpParser tripTtpParser = new TripTtpParser();
                List<CityPair> ListCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
                GlobalSettings.TtpCityPairs = ListCityPair;

                loadingOverlay = new LoadingOverlay(View.Bounds, "Reparsing..Please Wait..");
                View.Add(loadingOverlay);
                InvokeInBackground(() => {
                    string zipFilename = WBidHelper.GenarateZipFileName();
                    ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
                    WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                    if (wBIdStateContent.IsOverlapCorrection)
                    {
                        string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                        if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL"))
                        {

                            OverlapData overlapData;
                            using (FileStream filestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL"))
                            {

                                OverlapData overlapdataobj = new OverlapData();
                                overlapData = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapdataobj, filestream);
                            }
                           
                            if (overlapData != null)
                            {
                                GlobalSettings.LeadOutDays = overlapData.LeadOutDays;
                                GlobalSettings.LastLegArrivalTime = Convert.ToInt32(overlapData.LastLegArrivalTime);
                            }
                        }
                    }


                    ReparseBL.ReparseTripAndLineFiles(reparseParams);
                    InvokeOnMainThread(() => {
                        loadingOverlay.Hide();
                    });
                });

            }));              okAlertController.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (actionOK) => {
                if (btnUserIdCheckBox.Selected)
                {
                    GlobalSettings.IsDifferentUser = true;
                    GlobalSettings.ModifiedEmployeeNumber = txtUserId.Text;
                }
                TripTtpParser tripTtpParser = new TripTtpParser();
                List<CityPair> ListCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
                GlobalSettings.TtpCityPairs = ListCityPair;

                TestVacationViewController testVacVC = new TestVacationViewController();
                testVacVC.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                this.PresentViewController(testVacVC, true, null);

            }));
            WindowAlert.MakeKeyAndVisible();
            WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
           // WindowAlert.Dispose();

        }

		
		partial void btnSubmitTap (UIKit.UIButton sender)
		{
			if(btnUserIdCheckBox.Selected)
			{
				if(!RegXHandler.EmployeeNumberValidation(txtUserId.Text))
				{
				    UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);                     WindowAlert.RootViewController = new UIViewController();                     UIAlertController okAlertController = UIAlertController.Create("Error", "Invalid Employee Number", UIAlertControllerStyle.Alert);                     okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                     WindowAlert.MakeKeyAndVisible();                     WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
                    WindowAlert.Dispose();
                    return;
				}
			}

			GlobalSettings.QAScrapPassword=txtPassword.Text.ToString();
            if (sgQATest.SelectedSegment == 0)
                GlobalSettings.buddyBidTest = true;
            else
                GlobalSettings.buddyBidTest = false;

            if (sgMock.SelectedSegment == 0)
                GlobalSettings.WBidINIContent.Data.IsCompanyData = false;
            else
                GlobalSettings.WBidINIContent.Data.IsCompanyData = true;

			if(segWifi.SelectedSegment==0)
			{
				//GlobalSettings.IsWifiTestOn=true;
				GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest=true;
			}
			if(segWifi.SelectedSegment==1)
			{
				//GlobalSettings.IsWifiTestOn=false;
				GlobalSettings.WBidINIContent.User.IsSouthWestWifiTest=false;
			}
            if (btnUserIdCheckBox.Selected)
            {
                GlobalSettings.IsDifferentUser = true;
                GlobalSettings.ModifiedEmployeeNumber = txtUserId.Text;
            }
			if(SgSenList.SelectedSegment==0)
			{
				GlobalSettings.IsNewSeniorityListFormat=true;
			}
			if(SgSenList.SelectedSegment==1)
			{
				GlobalSettings.IsNewSeniorityListFormat=false;
			}
            XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
            this.DismissViewController(true, null);


		}
		partial void WifiPaidOrNot (NSObject sender)
		{
			UISegmentedControl ObjSeg=(UISegmentedControl)sender;
			switch(ObjSeg.SelectedSegment)
			{
			case 0:
				GlobalSettings.IsSouthWestPaidCheck= true;
				  break;
			case 1:
				GlobalSettings.IsSouthWestPaidCheck=false;
				  break;

			}
		}
		partial void btnUserIdCheckBoxTapped (Foundation.NSObject sender)
		{
            ((UIButton)sender).Selected = !((UIButton)sender).Selected;
		}
		partial void sgMockTapped (Foundation.NSObject sender)
		{
           
		}
		partial void sgQATestTapped (Foundation.NSObject sender)
		{

		}
	
	}
}

