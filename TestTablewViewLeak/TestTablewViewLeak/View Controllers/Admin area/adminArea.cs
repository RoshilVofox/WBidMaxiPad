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
using System.Json;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace WBid.WBidiPad.iOS
{
	public partial class adminArea : UIViewController, IServiceDelegate
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
            GlobalSettings.IsNeedToEnableVacDiffButton = ((UIButton)sender).Selected;
		}
		partial void sgMockTapped (Foundation.NSObject sender)
		{
           
		}
		partial void sgQATestTapped (Foundation.NSObject sender)
		{

		}
        public RestServiceUtil RestService=new RestServiceUtil();
        OdataBuilder ObjOdata = new OdataBuilder();
        partial void btnService1Tap(NSObject sender)
        {
            ObjOdata.RestService.Objdelegate = this;
            string EmployeeNo = "21221";
            ObjOdata.CheckRemoUserAccount(EmployeeNo);
            string UrlString = "GetEmployeeDetails/" + EmployeeNo + "/4";
          
           // RestService.ConstructHttpsURL(UrlString);
           // RestService.Get();
        }
        partial void btnService2Tap(NSObject sender)
        {
            ObjOdata.RestService.Objdelegate = this;
            string EmployeeNo = "21221";
            ObjOdata.CheckRemoUserAccountTest(EmployeeNo);
            string UrlString = "GetEmployeeDetails/" + EmployeeNo + "/4";
        }
        partial void btnService3Tap(NSObject sender)
        {
            try
            {
                
                try
                {
                    UIAlertController alert;
                    string EmployeeNo = "21221";
                    string UrlString = "GetEmployeeDetails/" + EmployeeNo + "/4";


                    //using (var client = new HttpClient())
                    //{
                    //    client.BaseAddress = new Uri("http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/");
                    //    //HTTP GET
                    //    var responseTask = client.GetAsync("GetEmployeeDetails/" + EmployeeNo + "/4");
                    //    responseTask.Wait();

                    //    var result = responseTask.Result;
                    //    if (result.IsSuccessStatusCode)
                    //    {

                    //        //var readTask = result.Content.ReadAsAsync<Student[]>();
                    //        //readTask.Wait();

                    //        //var students = readTask.Result;

                    //        //foreach (var student in students)
                    //        //{
                    //        //    Console.WriteLine(student.Name);
                    //        //}
                    //    }
                    //}
                    //Console.ReadLine();

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.BaseAddress = "https://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/Rest/";
                        var json = webClient.DownloadString("GetEmployeeDetails/" + EmployeeNo + "/4" );
                        var ObjremoteUserInfo = CommonClass.ConvertJSonToObject<RemoteUserInformation>(json.ToString());
                        alert = UIAlertController.Create("Great!", json, UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                           
                        }));

                        this.PresentViewController(alert, true, null);

                    }
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        partial void btnService4(NSObject sender)
        {
            UIAlertController alert;
            string EmployeeNo = "21221";
            string UrlString = "GetEmployeeDetails/" + EmployeeNo + "/4";


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/");
                //HTTP GET
                var responseTask = client.GetAsync("GetEmployeeDetails/" + EmployeeNo + "/4");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    alert = UIAlertController.Create("Great!","", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {

                    }));

                    this.PresentViewController(alert, true, null);
                }
            }
            Console.ReadLine();
        }
        public void ServiceResponce(JsonValue jsonDoc)
        {
            Console.WriteLine("Service Success");
           
            UIAlertController alert;

            int empNo = int.Parse(jsonDoc["EmpNum"].ToString());
            if (empNo != 0)
            {
                alert = UIAlertController.Create("Great!", "We found a previous account from  WbidMax.\nWe've imported those settings.\nPlease verify the settings and change as needed", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {
                  var  ObjremoteUserInfo = CommonClass.ConvertJSonToObject<RemoteUserInformation>(jsonDoc.ToString());
                    
                }));

                this.PresentViewController(alert, true, null);
            }
            else
            {
                alert = UIAlertController.Create("No Existing Account", "\nWe checked , but no previous account exists for you.\n\nThe next view will let you create your account.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Create Account", UIAlertActionStyle.Default, (actionOK) => {
                    
                }));

                this.PresentViewController(alert, true, null);
            }
        }

        public void ResponceError(string Error)
        {
            throw new NotImplementedException();
        }
    }
}

