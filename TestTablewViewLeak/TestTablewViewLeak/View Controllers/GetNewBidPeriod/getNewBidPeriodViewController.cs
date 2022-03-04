using System;
using CoreGraphics;
using Foundation;
using UIKit;
//using WBid.WBidiPad.PortableLibrary.Core;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Windows;
using System.Text.RegularExpressions;
using WBid.WBidiPad.Core;
using System.Linq;
//using WBid.WBidiPad.iOS.Common;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using System.Globalization;
using System.Json;
using iOSPasswordStorage;


namespace WBid.WBidiPad.iOS
{
	public partial class getNewBidPeriodViewController : UIViewController,IServiceDelegate
	{
		List<Domicile> listDomicile;
		List<BidRound> listBidRound;
		List<Position> listPosition;
		BidDetails bidDetail = new BidDetails ();
		List<HistoricalBidData> lstHistoricalData=null;
		//NSObject notif;
		List<MonthYear> _monthYearList;

		public getNewBidPeriodViewController () : base ("getNewBidPeriodViewController", null)
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

			refreshHelpButton.Layer.CornerRadius = refreshHelpButton.Frame.Size.Width / 2;
			refreshHelpButton.Layer.MasksToBounds = true;

			vwOverlapPrep.Frame = new CGRect (20, 700, 500, 536);
			viewOverlapAlert.Frame = new CGRect(0, 700, 540, 576);
			vwOverlapPrep.Layer.BorderWidth = 1;
			btnOverlapCorrectionOk.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
			btnOverlapOK.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			btnOverlapSkip.SetBackgroundImage(UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			btnOverlapCancel.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
			swImport.Enabled = false;
			swOverlap.ValueChanged += swOverlapChanged;
			btnOverlapSkip.Enabled = true;
			btnOverlapOK.Enabled = false;
			GlobalSettings.IsOverlapCorrection = false;
			if (GlobalSettings.SelectedLine != null)
				lblImportLine.Text = "Import Line " + GlobalSettings.SelectedLine.LineNum + " times from the last month";
			else
				lblImportLine.Text = "Import Line - times from the last month";

			this.setupViews ();
			if(GlobalSettings.isHistorical)
            this.CurrentMonth ();
			// Perform any additional setup after loading the view, typically from a nib.
		}
        void CurrentMonth ()
        {
            string sMonth = DateTime.Now.ToString ("MM");
            Console.WriteLine (sMonth);
            UIButton btn =(UIButton) this.View.ViewWithTag(nint.Parse(sMonth));
            btn.Selected = true;
            bidDetail.Month = (int)btn.Tag;
            btnLogin.Enabled = true;
            //string str = btn.Tag.ToString ();
        }
        partial void btnRefreshTapped(UIButton sender)
        {

			if (Reachability.CheckVPSAvailable())
			{
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				WBidDataDwonloadAuthServiceClient client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				client.GetAvailableHistoricalListCompleted += Client_GetAvailableHistoricalListCompleted;
				client.GetAvailableHistoricalListAsync();
			}
		}

		partial void refreshHelpBtnTapped(UIButton sender)
        {
			UIPopoverController popoverController;
			PopoverViewController popoverContent = new PopoverViewController();
			popoverContent.PopType = "refreshBtnHelp";
			popoverController = new UIPopoverController(popoverContent);
			popoverController.PopoverContentSize = new CGSize(380, 120);
			popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Up, true);
		}

		void swOverlapChanged (object sender, EventArgs e)
		{
			if (GlobalSettings.SelectedLine != null)
				swImport.Enabled = swOverlap.On;

			if (!swOverlap.On)
				swImport.SetState (swOverlap.On, true);

			btnOverlapOK.Enabled = false;
			btnOverlapSkip.Enabled = false;
			if (swOverlap.On)
				btnOverlapOK.Enabled = true;
			else
				btnOverlapSkip.Enabled = true;
		}

		#region Setup views
        private void setupViews()
        {
			btnRefresh.Hidden = true;

			
            listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).ToList();

			refreshHelpButton.Hidden = btnRefresh.Hidden;
			listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).ToList();

            listBidRound = WBidCollection.GetBidRounds();
            listPosition = WBidCollection.GetPositions();
			//this.txtEmployeeNumber.Background = UIImage.FromBundle ("textField.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5));
            //Loading base buttons
            foreach (UIView vw in this.scrlVwBaseButton)
            {
				
					vw.RemoveFromSuperview ();

            }
            const int kxOffset = 10;
            const int kyOffset = 10;
            int kxCount = 5;
			int kWidth = 60;
            const int kHeight = 40;
            for (int i = 0; i < listDomicile.Count; i++)
            {
                UIButton btn = new UIButton();
                btn.Frame = new CGRect(i % kxCount * (kWidth + kxOffset), i / kxCount * (kyOffset + kHeight), kWidth, kHeight);
                btn.SetTitle(listDomicile[i].DomicileName, UIControlState.Normal);
				btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
				btn.SetTitleColor (UIColor.White, UIControlState.Selected);
				btn.TitleLabel.Font = UIFont.SystemFontOfSize (15);
				//btn.BackgroundColor = UIColor.White;
				//btn.Layer.BorderWidth = 1.0f;
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
                btn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    foreach (UIView aView in this.scrlVwBaseButton.Subviews)
                    {

                        try
                        {
                            UIButton abtn = (UIButton)aView;
                            abtn.Selected = false;
							abtn.BackgroundColor = UIColor.White;
                        }
                        catch
                        {
                        }

                    }
                    UIButton theBtn = (UIButton)sender;
					//theBtn.BackgroundColor = UIColor.Cyan;
                    theBtn.Selected = true;
                    //					GlobalSettings.DownloadBidDetails.Domicile = theBtn.TitleLabel.Text;
                    bidDetail.Domicile = theBtn.TitleLabel.Text;
					SetUiBasedOnHistoricalList ();
                };
				if (GlobalSettings.WbidUserContent.UserInformation.Domicile == null)
					GlobalSettings.WbidUserContent.UserInformation.Domicile = "ATL";
				if (btn.TitleLabel.Text == GlobalSettings.WbidUserContent.UserInformation.Domicile) {
					btn.Selected = true;
					bidDetail.Domicile = btn.TitleLabel.Text;
				}
                this.scrlVwBaseButton.AddSubview(btn);

            }
            //load positions
            foreach (UIView vw in this.scrlVwPositionButton)
            {
                vw.RemoveFromSuperview();
            }
            kxCount = 3;
            for (int i = 0; i < this.listPosition.Count; i++)
            {
                UIButton btn = new UIButton();
                btn.Frame = new CGRect(i % kxCount * (kWidth + kxOffset), i / kxCount * (kyOffset + kHeight), kWidth, kHeight);
                btn.SetTitle(listPosition[i].LongStr, UIControlState.Normal);
                btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btn.SetTitleColor (UIColor.White, UIControlState.Selected);
				btn.TitleLabel.Font = UIFont.SystemFontOfSize (15);
				//btn.BackgroundColor = UIColor.White;
				//btn.Layer.BorderWidth = 1.0f;
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
                btn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    foreach (UIView aView in this.scrlVwPositionButton.Subviews)
                    {

                        try
                        {
                            UIButton abtn = (UIButton)aView;
                            abtn.Selected = false;
							//abtn.BackgroundColor = UIColor.White;
                        }
                        catch
                        {
                        }

                    }
                    UIButton theBtn = (UIButton)sender;
					//theBtn.BackgroundColor = UIColor.Cyan;
                    theBtn.Selected = true;
                    bidDetail.Postion = theBtn.TitleLabel.Text;
					SetUiBasedOnHistoricalList ();
                };
				if (GlobalSettings.WbidUserContent.UserInformation.BidSeat == null)
					GlobalSettings.WbidUserContent.UserInformation.BidSeat = "FO";
				if (btn.TitleLabel.Text == GlobalSettings.WbidUserContent.UserInformation.BidSeat) {
					btn.Selected = true;
					bidDetail.Postion = btn.TitleLabel.Text;
				}
                this.scrlVwPositionButton.AddSubview(btn);
            }
            //Load Round
            foreach (UIView vw in this.scrlVwRoundButton)
            {
                vw.RemoveFromSuperview();
            }
            kxCount = 2;
            kWidth = 100;
            for (int i = 0; i < this.listBidRound.Count; i++)
            {
                UIButton btn = new UIButton();
                btn.Tag = i;
                btn.Frame = new CGRect(i % kxCount * (kWidth + kxOffset), i / kxCount * (kyOffset + kHeight), kWidth, kHeight);
                btn.SetTitle(listBidRound[i].Round, UIControlState.Normal);
				btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btn.SetTitleColor (UIColor.White, UIControlState.Selected);
				btn.TitleLabel.Font = UIFont.SystemFontOfSize (15);
				//btn.BackgroundColor = UIColor.White;
				//btn.Layer.BorderWidth = 1.0f;
				btn.SetBackgroundImage(UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Normal);
				btn.SetBackgroundImage(UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5,5,5,5)), UIControlState.Selected);
                btn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    foreach (UIView aView in this.scrlVwRoundButton.Subviews)
                    {

                        try
                        {
                            UIButton abtn = (UIButton)aView;
                            abtn.Selected = false;
							//abtn.BackgroundColor = UIColor.White;
                        }
                        catch
                        {
                        }
                    }
                    UIButton theBtn = (UIButton)sender;
					//theBtn.BackgroundColor = UIColor.Cyan;
                    theBtn.Selected = true;
                    if (theBtn.Tag == 0)
                    {
                        bidDetail.Round = "D";
                    }
                    else
                        bidDetail.Round = "B";
					SetUiBasedOnHistoricalList ();
                };
				if (btn.Tag == 0) {
					btn.Selected = true;
					bidDetail.Round = "D";
				}
                this.scrlVwRoundButton.AddSubview(btn);
            }

			foreach (UIButton aBtn in this.btnMonthCollection) {
//				aBtn.Layer.BorderWidth = 1.0f;
//				aBtn.BackgroundColor = UIColor.White;
				aBtn.SetTitleColor (UIColor.White, UIControlState.Selected);
				aBtn.TitleLabel.Font = UIFont.SystemFontOfSize (15);
				aBtn.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				aBtn.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);

//				NSDate currentDate = NSDate.Now;
//				NSDateFormatter formatter = new NSDateFormatter ();
//				formatter.DateFormat = "MM";
				if (!GlobalSettings.isHistorical)
				{
				if (aBtn.Tag == DateTime.Now.Date.AddMonths (1).Month) {
					aBtn.Selected = true;
					bidDetail.Month = (int)aBtn.Tag;
				}
			}
            }

			if (GlobalSettings.isHistorical) {

				btnRefresh.Hidden = false;
				refreshHelpButton.Hidden = btnRefresh.Hidden;
				lblTitle.Text = "Retrieve Historical Bid Period";
				vwYear.Hidden = false;
				string[] year = new string[] {
					(DateTime.Now.Year - 2).ToString (),
					(DateTime.Now.Year - 1).ToString (),
					DateTime.Now.Year.ToString ()
				};
				foreach (var btn in btnYear) {
					btn.SetTitleColor (UIColor.White, UIControlState.Selected);
					btn.TitleLabel.Font = UIFont.SystemFontOfSize (15);
					btn.SetBackgroundImage (UIImage.FromBundle ("menuGreenNormal.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
					btn.SetBackgroundImage (UIImage.FromBundle ("activeButtonOrange.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Selected);
					btn.SetTitle (year [btn.Tag], UIControlState.Normal);
				}
				btnYear [2].Selected = true;
				bidDetail.Year = Convert.ToInt32 (btnYear [2].CurrentTitle);
				if (System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath)) 
				{
					using (FileStream fileStream = File.OpenRead (WBidHelper.HistoricalFilesInfoPath)) {
						List<HistoricalBidData> historicalBidDataList = new List<HistoricalBidData> ();
						lstHistoricalData = ProtoSerailizer.DeSerializeObject (WBidHelper.HistoricalFilesInfoPath, historicalBidDataList, fileStream);
						SetUiBasedOnHistoricalList ();
					}
				} else 
				{
					getAvaibleHistoryList ();

				}
				

			}
			else 
			{
				lblTitle.Text = "Retrieve New Bid Period";
				vwYear.Hidden = true;
			}
        }
		void SetUiBasedOnHistoricalList()
		{
			if (GlobalSettings.isHistorical) {
				int year = Convert.ToInt32 (bidDetail.Year);
				string domicile = bidDetail.Domicile;
				string position = bidDetail.Postion;
				int round = (bidDetail.Round=="D")?1:2;

				if (lstHistoricalData != null) 
				{
					var downloadedMonths = lstHistoricalData.Where (x => x.Domicile == domicile && x.Position == position && x.Round == round && x.Year == year).Select(y=>y.Month).ToList ();
					foreach (var button in btnMonthCollection) 
					{
                        button.Enabled = false;
                        //button.Selected = false;
					}

					var objdata = btnMonthCollection.Where(x => downloadedMonths.Any(y => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(y).Substring(0,3) == x.CurrentTitle)).ToList();
					foreach (var button in objdata) 
					{
      						button.Enabled = true;
					}

				}
				btnLogin.Enabled = btnMonthCollection.Any (x => x.Selected);


			}
		}
		private void getAvaibleHistoryList()
		{
			if (!System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath))
			{
                if (Reachability.CheckVPSAvailable ()) 
				{
					BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
					WBidDataDwonloadAuthServiceClient client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
					client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
					client.GetAvailableHistoricalListCompleted+= Client_GetAvailableHistoricalListCompleted;
					client.GetAvailableHistoricalListAsync();
				}
			}
		}
		void Client_GetAvailableHistoricalListCompleted (object sender, GetAvailableHistoricalListCompletedEventArgs e)
		{
			try
			{
				if (e.Result != null) 
				{

					List<WBidDataDownloadAuthorizationService.Model.BidData> lstBid = e.Result.ToList ();
					lstHistoricalData = new List<HistoricalBidData> ();
					foreach (var item in lstBid) {
						lstHistoricalData.Add (new HistoricalBidData {Domicile = item.Domicile, Month = item.Month, Position = item.Position, Round = item.Round
								, Year = item.Year
						});
					}


					var previousfile = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.HST", SearchOption.AllDirectories).Select(Path.GetFileName);
					if (previousfile != null && previousfile.Count()>0)
					{
						foreach (var item in previousfile)
						{
							File.Delete(WBidHelper.GetAppDataPath() + "//" + item);
						}
					}
					var stream = File.Create (WBidHelper.HistoricalFilesInfoPath);
					ProtoSerailizer.SerializeObject (WBidHelper.HistoricalFilesInfoPath, lstHistoricalData, stream);
					stream.Dispose ();
					stream.Close ();
					SetUiBasedOnHistoricalList ();
				}
			}catch(Exception ex){
			}

		}

		#endregion
		#region IBAction
		partial void btnYearTapped (UIKit.UIButton sender)
		{
			foreach (var btn in btnYear) {
				btn.Selected = false;
			}
			btnYear[sender.Tag].Selected = true;
			bidDetail.Year=Convert.ToInt32(btnYear[sender.Tag].CurrentTitle);
			SetUiBasedOnHistoricalList ();
		}
		partial void btnCloseTapped (Foundation.NSObject sender)
		{
			this.DismissViewController(true, null);

		}
        partial void btnLoginTapped(Foundation.NSObject sender)
        {
            //this.observeNotification ();
            //All Fields are mandatory, including emp number.




            //			if(DateTime.Now.Date.Month==12)
            //				bidDetail.Year = DateTime.Now.AddYears(1).Year;
            //			else
            //				bidDetail.Year = DateTime.Now.Year;

            SetBidYear();

            if (bidDetail.Domicile == null || bidDetail.Postion == null || bidDetail.Round == null ||
                bidDetail.Month == 0 || bidDetail.Year == 0)
            {
                UIAlertController okAlertController = UIAlertController.Create("Instruction", "All options are mandatory", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);

                return;
            }


            GlobalSettings.DownloadBidDetails = bidDetail;

			if (!GlobalSettings.isHistorical)
			{
				ShowEarlyBidWarning();
			}
			else
			{
				ShowLoginWindow();
			}

            //if (!GlobalSettings.isHistorical){

            //	ShowEarlyBidWarning();
            //	loginViewController login = new loginViewController();
            //	login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            //	this.PresentViewController(login, true, () =>
            //	{
            //		CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
            //	});
            //             if (GlobalSettings.DownloadBidDetails.Round == "B" && (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO"))
            //                 vwOverlapPrep2ndRnd.Hidden = false;
            //             else
            //                 vwOverlapPrep2ndRnd.Hidden = true;
            //             UIView.Animate(.25, 0, UIViewAnimationOptions.CurveLinear, () =>
            //                 {
            //                     vwOverlapPrep.Frame = new CGRect(20, 64, 500, 536);
            //                 }, null);
            //         } 
            //else 
            //{

            //	loginViewController login = new loginViewController();
            //	login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            //	this.PresentViewController(login, true, () =>
            //		{
            //			CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
            //		});
            //}

        }

        private void ShowLoginWindow()
        {
            loginViewController login = new loginViewController();
            login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(login, true, () =>
            {
                CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
            });
        }

        private void ShowEarlyBidWarning()
		{
			DateTime utctime= TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
			string date = string.Empty;
			if (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO")
			{ 
				if(GlobalSettings.DownloadBidDetails.Round=="D")
				{
					if(utctime < new DateTime(utctime.Year,utctime.Month,4,12,0,0))
						date = " 4th ";
				}
				else
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 17, 12, 0, 0))
						date = " 17th ";
				}
			}
			else if (GlobalSettings.DownloadBidDetails.Postion == "FA")
			{
				if (GlobalSettings.DownloadBidDetails.Round == "D")
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 2, 12, 0, 0))
						date = " 2nd ";
				}
				else
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 12, 12, 0, 0))
						date = " 12th ";
				}
			}
			if (date != string.Empty)
			{
				var message="SWA guarantees that the lines will be realeased by noon Central Time on the" +date+".\n\nSometimes SWA releases the lines earlier. If SWA has not released the lines early,then attempting to downlaod them now will result in a BID INFO UNAVAILABLE error.\n\nSo If you receive this error,try again later ";
			    UIAlertController okAlertController = UIAlertController.Create("Early Bid Warning", message, UIAlertControllerStyle.Alert);                 //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));  				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
				{
					ShowLoginWindow();

				}));
				this.PresentViewController(okAlertController, true, null);

            }
			else
			{
				ShowLoginWindow();
			}
        }
		private void GenerateYaerOfEachMonth()
		{
			//month list is hold the year  of each month 
			_monthYearList = new List<MonthYear>();
			MonthYear monthYear = null;

			for (int month = 1; month <= 12; month++)
			{
				_monthYearList.Add(new MonthYear() { Month = month });

			}


			if (DateTime.Now.Month < 6)
			{
				int previousMonth = (DateTime.Now.Month == 1) ? 12 : DateTime.Now.Month - 1;
				for (int count = 1; count <= 5; count++)
				{
					monthYear = _monthYearList.FirstOrDefault(x => x.Month == previousMonth);
					monthYear.Year = (previousMonth > DateTime.Now.Month) ? DateTime.Now.Year - 1 : DateTime.Now.Year;
					previousMonth = (previousMonth == 1) ? 12 : previousMonth - 1;

				}
			}


			else if (DateTime.Now.Month > 6)
			{
				int nextMonth = (DateTime.Now.Month == 12) ? 1 : DateTime.Now.Month + 1;
				for (int count = 1; count <= 6; count++)
				{
					monthYear = _monthYearList.FirstOrDefault(x => x.Month == nextMonth);
					monthYear.Year = (nextMonth < DateTime.Now.Month) ? DateTime.Now.Year + 1 : DateTime.Now.Year;
					nextMonth = (nextMonth == 12) ? 1 : nextMonth + 1;

				}
			}

			for (int month = 1; month <= 12; month++)
			{
				monthYear = _monthYearList.FirstOrDefault(x => x.Month == month);
				if (monthYear.Year == 0)
				{
					monthYear.Year = DateTime.Now.Year;

				}

			}
		}


        private void SetBidYear()
        {
            if (GlobalSettings.isHistorical)
            {
                var btnYr = btnYear.FirstOrDefault(x => x.Selected == true);
                bidDetail.Year = int.Parse(btnYr.Title(UIControlState.Normal));
            }
            else
            {
                if (_monthYearList == null)
                {
                    GenerateYaerOfEachMonth();
                }
                bidDetail.Year = _monthYearList.FirstOrDefault(x => x.Month == bidDetail.Month).Year;
            }
        }


		public class MonthYear
		{
			public int Month { get; set; }

			public int Year { get; set; }
		}
		partial void btnMonthTapped (Foundation.NSObject sender)
		{
			foreach(UIButton aBtn in this.btnMonthCollection)
			{
				aBtn.Selected = false;
				//aBtn.BackgroundColor = UIColor.White;
			}
			UIButton theBtn = (UIButton)sender;
			theBtn.Selected = true;
			//theBtn.BackgroundColor = UIColor.Cyan;
			bidDetail.Month = (int)theBtn.Tag;
			//btnLogin.Enabled = btnMonthCollection.Any (x => x.Selected);
		}
		#endregion


		partial void btnOverlapAlertOkTapped(Foundation.NSObject sender)
		{

			UIView.Animate(.15, 0, UIViewAnimationOptions.CurveLinear, () =>
				{
					viewOverlapAlert.Frame = new CGRect(0, 700, 540, 576);
				}, null);

			UIView.Animate(.25, 0, UIViewAnimationOptions.CurveLinear, () =>
			{
				vwOverlapPrep.Frame = new CGRect(20, 700, 500, 536);
			}, null);
			GlobalSettings.IsOverlapCorrection = true;
			OverlapCorrectionViewController overCorr = new OverlapCorrectionViewController();
			overCorr.importLine1 = swImport.On;
			//CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver("loginDetailsEntered", furtherAfterLogin);
			this.NavigationController.PushViewController(overCorr, true);
		}

		partial void btnOverlapOKTapped (UIKit.UIButton sender)
		{


			if (GlobalSettings.DownloadBidDetails.Round == "B" && (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO"))
			{
				swOverlap.On = false;
				swImport.On = false;
				loginViewController login = new loginViewController();
				login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController(login, true, () =>
					{
						CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
					});
			}
			else if (swOverlap.On)
			{
				UIView.Animate(.15, 0, UIViewAnimationOptions.CurveLinear, () =>
				{
					viewOverlapAlert.Frame = new CGRect(0, 44, 540, 576);
				}, null);

			
			}
			else {
				loginViewController login = new loginViewController();
				login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.PresentViewController(login, true, () =>
					{

						CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
					});
			}




		
		}

		partial void btnOverlapCancelTapped (UIKit.UIButton sender)
		{
			UIView.Animate(.15, 0, UIViewAnimationOptions.CurveLinear, ()=>
				{
					vwOverlapPrep.Frame = new CGRect(20, 700, 500, 536);
				}, null );
		}

//		#region observe notification
//		private void observeNotification()
//		{
//			notif = NSNotificationCenter.DefaultCenter.AddObserver ("loginDetailsEntered", furtherAfterLogin);
//		}
		private void furtherAfterLogin(NSNotification n)
		{
			OdataBuilder ObjOdata = new OdataBuilder ();
			ObjOdata.RestService.Objdelegate = this;
			NSNotificationCenter.DefaultCenter.RemoveObserver (CommonClass.bidObserver);
			//checking  the internet connection available
			//==================================================================================================================
            if (Reachability.CheckVPSAvailable ()) 
			{
				NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckSuccess", null);
				//checking CWA credential
				//==================================================================================================================



				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {

					string usernum = KeychainHelpers.GetPasswordForUsername ("user", "WBid.WBidiPad.cwa", false);
					string loginemployee=Regex.Match (usernum, @"\d+").Value;
					if (GlobalSettings.WbidUserContent.UserInformation.EmpNo == loginemployee) 
					{
						ObjOdata.CheckRemoUserAccount (GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					} 
					else
					{
						StartDownload ();
					}

				}

			}
			else
			{
				//if (WBidHelper.IsSouthWestWifi ()) 
				//{
				//	NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckSuccess", null);
				//	StartDownload ();
				//	return;
				//}
				//InvokeOnMainThread(() =>
					//{
					    //UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Connectivity not available", UIAlertControllerStyle.Alert);
                    //    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    //    this.PresentViewController(okAlertController, true, null);

                    //});

                InvokeOnMainThread(() =>
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

                });
			}



		}

		public void ServiceResponce(JsonValue jsonDoc)
		{
			Console.WriteLine ("Service Success");
			//ActivityIndicator.Hide ();

			if (jsonDoc ["FirstName"] != null || jsonDoc ["FirstName"].ToString ().Length > 0) 
			{
				RemoteUserInformation remoteUserdetails = new RemoteUserInformation ();
				remoteUserdetails = CommonClass.ConvertJSonToObject<RemoteUserInformation> (jsonDoc.ToString ());
				if (remoteUserdetails.WBExpirationDate != null) {
					GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
				}
				//WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

				GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = remoteUserdetails.WBExpirationDate;
				GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = remoteUserdetails.TopSubscriptionLine;
				GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = remoteUserdetails.SecondSubscriptionLine;
				GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = remoteUserdetails.ThirdSubscriptionLine;
				GlobalSettings.WbidUserContent.UserInformation.IsFree = remoteUserdetails.IsFree;
				GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed = remoteUserdetails.IsMonthlySubscribed;
				GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed = remoteUserdetails.IsYearlySubscribed;
				WBidHelper.SaveUserFile (GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);

				int DateTimeDifference = DateTime.Compare(remoteUserdetails.UserAccountDateTime, GlobalSettings.WbidUserContent.UserInformation.UserAccountDateTime);
				if (DateTimeDifference != 0) 
				{
					CheckUserInformations (remoteUserdetails);
				} 
				else 
				{
					StartDownload ();
				}
			
				//				InvokeInBackground(() =>
				//					{
				//						this.initiateDownloadProcess();
				//					});
			}

		}
		public void StartDownload()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (CommonClass.bidObserver);
			downloadBidDataViewController downloadBid = new downloadBidDataViewController();
			downloadBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(downloadBid, true, null);
		}

		private void StartDownloadGetNotification(NSNotification n)
		{
			StartDownload ();
		}
		public void ResponceError(string Error)
		{
			StartDownload ();
			//ActivityIndicator.Hide ();

		}
		void CheckUserInformations(RemoteUserInformation remoteUserdetails)
		{
			var DifferenceList = new List<KeyValuePair<string, string>> ();


			if (remoteUserdetails.FirstName != GlobalSettings.WbidUserContent.UserInformation.FirstName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("FirstName",GlobalSettings.WbidUserContent.UserInformation.FirstName+","+ remoteUserdetails.FirstName));
			}

			if (remoteUserdetails.LastName != GlobalSettings.WbidUserContent.UserInformation.LastName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("LastName",GlobalSettings.WbidUserContent.UserInformation.LastName+","+ remoteUserdetails.LastName));
			}


			if (remoteUserdetails.Email != GlobalSettings.WbidUserContent.UserInformation.Email)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Email",GlobalSettings.WbidUserContent.UserInformation.Email+","+ remoteUserdetails.Email));
			}

			if (remoteUserdetails.CellPhone != GlobalSettings.WbidUserContent.UserInformation.CellNumber)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("CellPhone",GlobalSettings.WbidUserContent.UserInformation.CellNumber+","+ remoteUserdetails.CellPhone));
			}

			if (remoteUserdetails.CarrierNum != GlobalSettings.WbidUserContent.UserInformation.CellCarrier)
			{
				int remoteCarrier = remoteUserdetails.CarrierNum;
				int localCarrier = GlobalSettings.WbidUserContent.UserInformation.CellCarrier;
				int CellcarrierCount = CommonClass.CellCarrier.Length;
				if(remoteCarrier >0 && localCarrier >0 && remoteCarrier < CellcarrierCount && localCarrier < CellcarrierCount)
					DifferenceList.Add(new KeyValuePair<string, string>("CarrierNum",GlobalSettings.WbidUserContent.UserInformation.CellCarrier+","+ remoteUserdetails.CarrierNum ));
			}

			string LocalPosition = "";
			string RemotePosition = "";
			if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
			{
				LocalPosition = "Pilot";
			}

			else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
			{
				LocalPosition = "Flight Attendant";
			}
			if (remoteUserdetails.Position.ToString() == "4" )
			{
//				if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP")
//				{
//					RemotePosition = "Captain";
//				}
//				else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FO")
//				{
//					RemotePosition = "First Officer";
//				}
				RemotePosition = "Pilot";
			}
			else if (remoteUserdetails.Position.ToString() == "3")
			{
				RemotePosition = "Flight Attendant";
			}

			if (LocalPosition != RemotePosition)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Position",LocalPosition+","+ RemotePosition));
			}


			if (DifferenceList.Count > 0) 
			{
				CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("USerDifferenceUpdated"), StartDownloadGetNotification);
				UserAccountDifferenceScreen ObjUserDifference = new UserAccountDifferenceScreen ();
				ObjUserDifference.DifferenceList = DifferenceList;
				ObjUserDifference.iSfromHome = true;
				ObjUserDifference.isAcceptMail = remoteUserdetails.AcceptEmail;
				ObjUserDifference.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				this.NavigationController.PresentViewController (ObjUserDifference, true,null);

			} 
			else 
			{
				GlobalSettings.WbidUserContent.UserInformation.isAcceptMail = remoteUserdetails.AcceptEmail;
				StartDownload ();
			}

		}

//		#endregion
	}
}
	