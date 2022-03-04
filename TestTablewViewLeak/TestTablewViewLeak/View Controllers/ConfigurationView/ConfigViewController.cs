using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WBid.WBidiPad.iOS
{
	public partial class ConfigViewController : UIViewController
	{
		public ConfigViewController ()
			: base ("ConfigViewController", null)
		{
		}

		NSObject configNumNotif;
		UIPopoverController popoverController;

		class MyPopDelegate : UIPopoverControllerDelegate
		{
			ConfigViewController _parent;

			public MyPopDelegate (ConfigViewController parent)
			{
				_parent = parent;
			}

			public override void DidDismiss (UIPopoverController popoverController)
			{
				_parent.popoverController = null;
				NSNotificationCenter.DefaultCenter.RemoveObserver (_parent.configNumNotif);

				_parent.btnAMPush.Selected = false;
				_parent.btnAMArrive.Selected = false;
				_parent.btnPMPush.Selected = false;
				_parent.btnPMArrive.Selected = false;
				_parent.btnNTEPush.Selected = false;
				_parent.btnNTEArrive.Selected = false;
				_parent.btnMaxNumber.Selected = false;
				_parent.btnMaxPercentage.Selected = false;
				_parent.btnWeekMaxNumber.Selected = false;
				_parent.btnWeekMaxPercentage.Selected = false;
				_parent.btnMaxStartDay.Selected = false;

			}
		}
		partial void funBorderChanged(NSObject sender)
		{
			var switchData = (UISwitch)sender;

			//if (switchData.On)
				GlobalSettings.WBidINIContent.User.IsModernViewShade = switchData.On;
			
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

			if (CommonClass.isVPSServer == "FALSE") {
				lblServerType.Text = "VOFOX Server";
				lblServerType.Hidden = false;
				SegServerType.SelectedSegment=1;


			} else {
				lblServerType.Text = "VPS";
				SegServerType.SelectedSegment=0;
			}

			if (CommonClass.IsModernScrollClassic == "FALSE") {
				segModernScrollOptions.SelectedSegment = 1;

			} else {
				
				segModernScrollOptions.SelectedSegment = 0;
			}



			UITapGestureRecognizer UpsingleTap;
			UpsingleTap = new UITapGestureRecognizer(() =>
				{
					lblServerType.Hidden=false;
					if (CommonClass.isVPSServer == "TRUE") 
					{
						
						SegServerType.SelectedSegment=0;
						}
					else
					{
						SegServerType.SelectedSegment=1;
					}
					SegServerType.Hidden=false;

				});

			UpsingleTap.NumberOfTapsRequired = 2;
			btnSecretServerType.AddGestureRecognizer (UpsingleTap);
			// Perform any additional setup after loading the view, typically from a nib.
			navBar.Items [0].Title = "Configuration";//this.Title;

			if (this.Title == "Misc") {
				this.View.AddSubview (vwMisc);
				swShowCover.ValueChanged += swShowCover_ValueChanged;
				swCheckData.ValueChanged += swCheckData_ValueChanged;
				swGatherTrips.ValueChanged += swGatherTrips_ValueChanged;
				swMissingData.ValueChanged += swMissingData_ValueChanged;
			} else if (this.Title == "Pairing Export") {
				this.View.AddSubview (vwPairExp);
				swPairInSub.ValueChanged += swPairInSub_ValueChanged;
			} else if (this.Title == "AM/PM") {
				this.View.AddSubview (vwAMPM);

				sgAMPM.SelectedSegment = GlobalSettings.WBidINIContent.AmPmConfigure.HowCalcAmPm - 1;
				sgLineParam.SelectedSegment = GlobalSettings.WBidINIContent.AmPmConfigure.NumberOrPercentageCalc - 1;
				btnAMPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.AmPush.ToString (@"hh\:mm"), UIControlState.Normal);
				btnAMPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.AmPush.ToString (@"hh\:mm"), UIControlState.Selected);
				btnPMPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PmPush.ToString (@"hh\:mm"), UIControlState.Normal);
				btnPMPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PmPush.ToString (@"hh\:mm"), UIControlState.Normal);
				btnNTEPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NitePush.ToString (@"hh\:mm"), UIControlState.Normal);
				btnNTEPush.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NitePush.ToString (@"hh\:mm"), UIControlState.Normal);
				btnAMArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.AmLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnAMArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.AmLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnPMArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PmLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnPMArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PmLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnNTEArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NiteLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnNTEArrive.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NiteLand.ToString (@"hh\:mm"), UIControlState.Normal);
				btnMaxNumber.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NumOpposites.ToString (), UIControlState.Normal);
				btnMaxNumber.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.NumOpposites.ToString (), UIControlState.Normal);
				btnMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PctOpposities.ToString (), UIControlState.Normal);
				btnMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.AmPmConfigure.PctOpposities.ToString (), UIControlState.Normal);

				btnAMPush.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnPMPush.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnNTEPush.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnAMArrive.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnPMArrive.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnNTEArrive.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnMaxNumber.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnMaxPercentage.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnAmpmOK.SetBackgroundImage (UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);

				if (sgLineParam.SelectedSegment == 0) {
					btnMaxNumber.Enabled = true;
					btnMaxPercentage.Enabled = false;
				} else if (sgLineParam.SelectedSegment == 1) {
					btnMaxNumber.Enabled = false;
					btnMaxPercentage.Enabled = true;
				}

			} else if (this.Title == "Week") {
				this.View.AddSubview (vwWeek);

				btnWeekMaxNumber.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnWeekMaxNumber.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnWeekMaxPercentage.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnWeekMaxPercentage.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnMaxStartDay.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnMaxStartDay.SetBackgroundImage (UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);
				btnWeekOk.SetBackgroundImage (UIImage.FromBundle ("menuGreenActive.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5)), UIControlState.Normal);


				if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Week != null) {

					sgWeekendDays.SelectedSegment = (GlobalSettings.WBidINIContent.Week.IsMaxWeekend) ? 0 : 1;
					btnWeekMaxNumber.SetTitle (GlobalSettings.WBidINIContent.Week.MaxNumber, UIControlState.Normal);
					btnWeekMaxNumber.SetTitle (GlobalSettings.WBidINIContent.Week.MaxNumber, UIControlState.Selected);
					btnWeekMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.Week.MaxPercentage, UIControlState.Normal);
					btnWeekMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.Week.MaxPercentage, UIControlState.Selected);
					btnMaxStartDay.SetTitle (GlobalSettings.WBidINIContent.Week.StartDOW, UIControlState.Normal);
					btnMaxStartDay.SetTitle (GlobalSettings.WBidINIContent.Week.StartDOW, UIControlState.Selected);
                    
                  
				}
                  


				if (sgWeekendDays.SelectedSegment == 0) {
					btnWeekMaxNumber.Enabled = true;
					btnWeekMaxPercentage.Enabled = false;
				} else {
					btnWeekMaxNumber.Enabled = false;
					btnWeekMaxPercentage.Enabled = true;
				}

				btnMaxStartDay.UserInteractionEnabled = false;
				stpWeek.ValueChanged += (object sender, EventArgs e) => {
					//btnMaxStartDay.TitleLabel.Text = ((UIStepper)sender).Value.ToString();
					btnMaxStartDay.SetTitle (((UIStepper)sender).Value.ToString (), UIControlState.Normal);

				};
			} else if (this.Title == "Hotels") {
				this.View.AddSubview (vwHotels);
            

			} else if (this.Title == "User") {
				this.View.AddSubview (vwUser);

				//swSync.Enabled = false;
				//swAutoSave.Enabled = false;

				txtAutoSaveTime.Background = UIImage.FromBundle ("textField.png").CreateResizableImage (new UIEdgeInsets (5, 5, 5, 5));
				swBidEmail.ValueChanged += swBidEmailChanged;
				swSync.ValueChanged += swSyncChanged;
				swAutoSave.ValueChanged += swAutoSaveChanged;
				stprAutoSaveTime.ValueChanged += stprAutoSaveTimeChanged;
				swCrash.ValueChanged += swCrashChanged;
				swMIL.ValueChanged += swMILChanged;

				swBidEmail.On = GlobalSettings.WBidINIContent.User.IsNeedBidReceipt;
				swSync.On = GlobalSettings.WBidINIContent.User.SmartSynch;
				swSummaryShade.On = GlobalSettings.WBidINIContent.User.IsSummaryViewShade;

				swAutoSave.On = GlobalSettings.WBidINIContent.User.AutoSave;
				stprAutoSaveTime.Value = GlobalSettings.WBidINIContent.User.AutoSavevalue;
				txtAutoSaveTime.Text = GlobalSettings.WBidINIContent.User.AutoSavevalue.ToString ();
				swCrash.On = GlobalSettings.WBidINIContent.User.IsNeedCrashMail;
				swMIL.On = GlobalSettings.WBidINIContent.User.MIL;
				swBorder.On = GlobalSettings.WBidINIContent.User.IsModernViewShade;
				stprAutoSaveTime.Enabled = GlobalSettings.WBidINIContent.User.AutoSave;

			}

			vwMisc.Frame = new CGRect (0, 20, 768, 768);
			vwPairExp.Frame = new CGRect (0, 20, 768, 768);
			vwAMPM.Frame = new CGRect (0, 20, 768, 768);
			vwWeek.Frame = new CGRect (0, 20, 768, 768);
			vwHotels.Frame = new CGRect (0, 20, 768, 768);
			vwUser.Frame = new CGRect (0, 20, 768, 768);


			LoadTabDetails ();

			this.View.BringSubviewToFront (navBar);
		}
		partial void ServerChange (NSObject sender)
		{
			UISegmentedControl segment= (UISegmentedControl)sender;

			if(segment.SelectedSegment == 0)
			{
				lblServerType.Text= "VPS";
				CommonClass.isVPSServer="TRUE";
		
				NSUserDefaults.StandardUserDefaults.SetString("TRUE",@"isVPS");

							}
			else
			{
				lblServerType.Text= "VOFOX Server";
				CommonClass.isVPSServer= "FALSE";
				NSUserDefaults.StandardUserDefaults.SetString("FALSE",@"isVPS");
			}
		}

		partial void funModernScrollViewOptionsAction (NSObject sender)
		{
			UISegmentedControl segment= (UISegmentedControl)sender;

			if(segment.SelectedSegment == 0)
			{
				CommonClass.IsModernScrollClassic="TRUE";
				NSUserDefaults.StandardUserDefaults.SetString("TRUE",@"IsModernScrollClassic");
			}
			else
			{
				CommonClass.IsModernScrollClassic="FALSE";
				NSUserDefaults.StandardUserDefaults.SetString("FALSE",@"IsModernScrollClassic");
			}

		}
		void swMILChanged (object sender, EventArgs e)
		{

				GlobalSettings.WBidINIContent.User.MIL = ((UISwitch)sender).On;
			if (GlobalSettings.CurrentBidDetails!=null&&GlobalSettings.CurrentBidDetails.Postion != "FA")
				CommonClass.lineVC.btnMIL.Hidden = !GlobalSettings.WBidINIContent.User.MIL;
			else if (CommonClass.lineVC!=null)
				CommonClass.lineVC.btnMIL.Hidden = true;

		}

		void swCrashChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.IsNeedCrashMail = ((UISwitch)sender).On;
		}

		void stprAutoSaveTimeChanged (object sender, EventArgs e)
		{
			txtAutoSaveTime.Text = ((UIStepper)sender).Value.ToString ();
			GlobalSettings.WBidINIContent.User.AutoSavevalue = (int)((UIStepper)sender).Value;
		}

		void swAutoSaveChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.AutoSave = ((UISwitch)sender).On;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("AutoSaveTimer", null);
			if (GlobalSettings.WBidINIContent.User.AutoSave)
				stprAutoSaveTime.Enabled = true;
			else
				stprAutoSaveTime.Enabled = false;
		}

		void swSyncChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.SmartSynch = ((UISwitch)sender).On;
			GlobalSettings.SynchEnable = ((UISwitch)sender).On;
		}
		partial void SummaryViewShadeValueChange(NSObject sender)
		{
			//GlobalSettings.WBidINIContent.User.IsSummaryViewShade = ((UISwitch)sender).On;
		}
		void swBidEmailChanged (object sender, EventArgs e)
		{
			GlobalSettings.WBidINIContent.User.IsNeedBidReceipt = ((UISwitch)sender).On;
		}

		partial void btnTipTapped (UIKit.UIButton sender)
		{
			switch (sender.Tag) {
			case 1:
				{

				}
				break;
			case 2:
				{

				}
				break;
			case 3: 
				{

				}
				break;
			case 4:
				{

				}
				break;
			}
		}

        



		private void LoadTabDetails ()
		{
			if (GlobalSettings.WBidINIContent != null) {
				if (GlobalSettings.WBidINIContent.MiscellaneousTab != null) {
					swShowCover.On = GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter;
					swCheckData.On = GlobalSettings.WBidINIContent.MiscellaneousTab.DataUpdate;
					swGatherTrips.On = GlobalSettings.WBidINIContent.MiscellaneousTab.GatherData;
					swMissingData.On = GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData;
				}

				if (GlobalSettings.WBidINIContent.PairingExport != null) {
					sgPairTime.SelectedSegment = GlobalSettings.WBidINIContent.PairingExport.IsCentralTime ? 0 : 1;
					sgPairSpan.SelectedSegment = GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing ? 0 : 1;
					swPairInSub.On = GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected;
				}
				if (GlobalSettings.WBidINIContent.SourceHotel != null) {
					sgHotelList.SelectedSegment = GlobalSettings.WBidINIContent.SourceHotel.SourceType - 1;
                  
				}
                  
			}
		}

		partial void btnDoneTapped (UIKit.UIBarButtonItem sender)
		{
			if (GlobalSettings.WBidINIContent.User.IsSummaryViewShade != swSummaryShade.On)
			{
				GlobalSettings.WBidINIContent.User.IsSummaryViewShade = swSummaryShade.On;
				NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName("ReloadModernView", null);
			this.ParentViewController.DismissViewController (true, null);
            NSNotificationCenter.DefaultCenter.PostNotificationName("ServerNameChange",null);
		}

		partial void btnAmpmOKTapped (UIKit.UIButton sender)
		{
			AmPmConfigure AmPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
			TimeSpan timeSpan;
			if (TimeSpan.TryParse (btnAMPush.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.AmPush = timeSpan;
			}

			if (TimeSpan.TryParse (btnPMPush.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.PmPush = timeSpan;
			}
			if (TimeSpan.TryParse (btnNTEPush.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.NitePush = timeSpan;
			}

			if (TimeSpan.TryParse (btnAMArrive.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.AmLand = timeSpan;
			}

			if (TimeSpan.TryParse (btnPMArrive.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.PmLand = timeSpan;
			}

			if (TimeSpan.TryParse (btnNTEArrive.TitleLabel.Text, out timeSpan)) {
				AmPmConfigure.NiteLand = timeSpan;
			}
			AmPmConfigure.HowCalcAmPm = (int)sgAMPM.SelectedSegment + 1;
			AmPmConfigure.NumberOrPercentageCalc = (int)sgLineParam.SelectedSegment + 1;
			AmPmConfigure.NumOpposites = Convert.ToInt32 (btnMaxNumber.TitleLabel.Text);
			AmPmConfigure.PctOpposities = Convert.ToInt32 (btnMaxPercentage.TitleLabel.Text);

			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

			RecalculateAndSaveLineFile ();
		}

		private void RecalculateAndSaveLineFile ()
		{
			var lines =  RecalculateAMPMProperties ();
			LineInfo lineInfo = new LineInfo () {
				LineVersion = GlobalSettings.LineVersion,
				Lines = lines

			};
			//var filename = WBidHelper.GetCurrentlyLoadedLinefileName();
			//if (filename != string.Empty)
			//{
			//	var jsonLineString = JsonConvert.SerializeObject(lineInfo);
			//	File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + filename, jsonLineString);
			//}
			
			//var fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
			//var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
			//ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
			//linestream.Dispose ();
			//linestream.Close ();
			NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
		}

		private Dictionary<string, Line> RecalculateAMPMProperties ()
		{

			Dictionary<string, Line> lines = new Dictionary<string, Line> ();
			//CalculateLineProperties calculateLineProperties = new CalculateLineProperties ();
			//foreach (Line line in GlobalSettings.Lines) {
			//	line.AMPM = calculateLineProperties.CalcAmPmProp (line, true);
			//	line.AMPMSortOrder = calculateLineProperties.CalcAmPmSortOrder (line);
			//	lines.Add (line.LineNum.ToString (), line);
			//}
			RecalcalculateLineProperties calculateLineProperties = new RecalcalculateLineProperties();
			foreach (Line line in GlobalSettings.Lines)
			{
				calculateLineProperties.RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(line);
				lines.Add(line.LineNum.ToString(), line);
			}
			return lines;
		}

		partial void btnAMPMTimesTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			configNumNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ChangeDefineTimeText"), handleChangeAMPMTimeText);

			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "timePad";
			popoverContent.SubPopType = "Define";
			if (sender == btnAMPush)
				popoverContent.timeValue = btnAMPush.TitleLabel.Text;
			else if (sender == btnAMArrive)
				popoverContent.timeValue = btnAMArrive.TitleLabel.Text;
			else if (sender == btnPMPush)
				popoverContent.timeValue = btnPMPush.TitleLabel.Text;
			else if (sender == btnPMArrive)
				popoverContent.timeValue = btnPMArrive.TitleLabel.Text;
			else if (sender == btnNTEPush)
				popoverContent.timeValue = btnNTEPush.TitleLabel.Text;
			else if (sender == btnNTEArrive)
				popoverContent.timeValue = btnNTEArrive.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (200, 200);
			popoverController.PresentFromRect (sender.Frame, vwAMPM, UIPopoverArrowDirection.Any, true);
		}

		public void handleChangeAMPMTimeText (NSNotification n)
		{
			if (btnAMPush.Selected) {
				btnAMPush.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnAMPush.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnAMArrive.Selected) {
				btnAMArrive.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnAMArrive.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnPMPush.Selected) {
				btnPMPush.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnPMPush.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnPMArrive.Selected) {
				btnPMArrive.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnPMArrive.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnNTEPush.Selected) {
				btnNTEPush.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnNTEPush.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnNTEArrive.Selected) {
				btnNTEArrive.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnNTEArrive.SetTitle (n.Object.ToString (), UIControlState.Selected);
			}
		}

		partial void btnMaxInputTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			configNumNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ChangeMaxInputText"), handleChangeMaxInputText);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "Define";
			if (sender == btnMaxNumber)
				popoverContent.numValue = btnMaxNumber.TitleLabel.Text;
			else if (sender == btnMaxPercentage)
				popoverContent.numValue = btnMaxPercentage.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect (sender.Frame, vwAMPM, UIPopoverArrowDirection.Any, true);
		}

		public void handleChangeMaxInputText (NSNotification n)
		{
			if (btnMaxNumber.Selected) {
				btnMaxNumber.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnMaxNumber.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnMaxPercentage.Selected) {
				btnMaxPercentage.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnMaxPercentage.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnWeekMaxNumber.Selected) {
				btnWeekMaxNumber.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnWeekMaxNumber.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnWeekMaxPercentage.Selected) {
				btnWeekMaxPercentage.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnWeekMaxPercentage.SetTitle (n.Object.ToString (), UIControlState.Selected);
			} else if (btnMaxStartDay.Selected) {
				btnMaxStartDay.SetTitle (n.Object.ToString (), UIControlState.Normal);
				btnMaxStartDay.SetTitle (n.Object.ToString (), UIControlState.Selected);

			}
			btnMaxNumber.Selected = false;
			btnMaxPercentage.Selected = false;
			btnWeekMaxNumber.Selected = false;
			btnWeekMaxPercentage.Selected = false;
			btnMaxStartDay.Selected = false;

			NSNotificationCenter.DefaultCenter.RemoveObserver (configNumNotif);
			popoverController.Dismiss (true);
		}

		partial void sgAMPMChanged (UIKit.UISegmentedControl sender)
		{

		}

		partial void sgLineParamChanged (UIKit.UISegmentedControl sender)
		{

			if (sender.SelectedSegment == 0) {
				btnMaxNumber.Enabled = true;
				btnMaxPercentage.Enabled = false;
			} else if (sender.SelectedSegment == 1) {
				btnMaxNumber.Enabled = false;
				btnMaxPercentage.Enabled = true;
			}
		}

		partial void btnAMPMResetTapped (UIKit.UIButton sender)
		{
			btnAMPush.SetTitle (TimeSpan.FromHours (4).ToString (@"hh\:mm"), UIControlState.Normal);
			btnAMPush.SetTitle (TimeSpan.FromHours (4).ToString (@"hh\:mm"), UIControlState.Selected);
			btnPMPush.SetTitle (TimeSpan.FromHours (11).ToString (@"hh\:mm"), UIControlState.Normal);
			btnPMPush.SetTitle (TimeSpan.FromHours (11).ToString (@"hh\:mm"), UIControlState.Selected);
			btnNTEPush.SetTitle (TimeSpan.FromHours (22).ToString (@"hh\:mm"), UIControlState.Normal);
			btnNTEPush.SetTitle (TimeSpan.FromHours (22).ToString (@"hh\:mm"), UIControlState.Selected);
			btnAMArrive.SetTitle (TimeSpan.FromHours (19).ToString (@"hh\:mm"), UIControlState.Normal);
			btnAMArrive.SetTitle (TimeSpan.FromHours (19).ToString (@"hh\:mm"), UIControlState.Selected);
			btnPMArrive.SetTitle (TimeSpan.FromHours (2).ToString (@"hh\:mm"), UIControlState.Normal);
			btnPMArrive.SetTitle (TimeSpan.FromHours (2).ToString (@"hh\:mm"), UIControlState.Selected);
			btnNTEArrive.SetTitle (TimeSpan.FromHours (7).ToString (@"hh\:mm"), UIControlState.Normal);
			btnNTEArrive.SetTitle (TimeSpan.FromHours (7).ToString (@"hh\:mm"), UIControlState.Selected);
			btnMaxNumber.SetTitle ("3", UIControlState.Normal);
			btnMaxNumber.SetTitle ("3", UIControlState.Selected);
			btnMaxNumber.SetTitle ("3", UIControlState.Disabled);
			btnMaxPercentage.SetTitle ("20", UIControlState.Normal);
			btnMaxPercentage.SetTitle ("20", UIControlState.Selected);
			btnMaxPercentage.SetTitle ("20", UIControlState.Disabled);
		}


		void swShowCover_ValueChanged (object sender, EventArgs e)
		{
			if (GlobalSettings.WBidINIContent.MiscellaneousTab == null)
				GlobalSettings.WBidINIContent.MiscellaneousTab = new MiscellaneousTab ();
			GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter = ((UISwitch)sender).On;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
		}


		void swMissingData_ValueChanged (object sender, EventArgs e)
		{
			if (GlobalSettings.WBidINIContent.MiscellaneousTab == null)
				GlobalSettings.WBidINIContent.MiscellaneousTab = new MiscellaneousTab ();
			GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData = ((UISwitch)sender).On;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
		}

		void swGatherTrips_ValueChanged (object sender, EventArgs e)
		{
			if (GlobalSettings.WBidINIContent.MiscellaneousTab == null)
				GlobalSettings.WBidINIContent.MiscellaneousTab = new MiscellaneousTab ();
			GlobalSettings.WBidINIContent.MiscellaneousTab.GatherData = ((UISwitch)sender).On;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
		}

		void swCheckData_ValueChanged (object sender, EventArgs e)
		{

			if (GlobalSettings.WBidINIContent.MiscellaneousTab == null)
				GlobalSettings.WBidINIContent.MiscellaneousTab = new MiscellaneousTab ();
			GlobalSettings.WBidINIContent.MiscellaneousTab.DataUpdate = ((UISwitch)sender).On;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
		}


		void swPairInSub_ValueChanged (object sender, EventArgs e)
		{
			if (GlobalSettings.WBidINIContent.PairingExport == null)
				GlobalSettings.WBidINIContent.PairingExport = new PairingExport ();
			GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected = ((UISwitch)sender).On;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
		}

		partial void sgPairTimeTapped (UIKit.UISegmentedControl sender)
		{
			if (GlobalSettings.WBidINIContent.PairingExport == null)
				GlobalSettings.WBidINIContent.PairingExport = new PairingExport ();
			GlobalSettings.WBidINIContent.PairingExport.IsCentralTime = sgPairTime.SelectedSegment == 0 ? true : false;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());


		}

		partial void sgPairSpanTapped (UIKit.UISegmentedControl sender)
		{
			if (GlobalSettings.WBidINIContent.PairingExport == null)
				GlobalSettings.WBidINIContent.PairingExport = new PairingExport ();
			GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing = sgPairSpan.SelectedSegment == 0 ? true : false;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

		}

		partial void sgWeekendDaysTapped (UIKit.UISegmentedControl sender)
		{
			if (sender.SelectedSegment == 0) {
				btnWeekMaxNumber.Enabled = true;
				btnWeekMaxPercentage.Enabled = false;
			} else {
				btnWeekMaxNumber.Enabled = false;
				btnWeekMaxPercentage.Enabled = true;
			}

		}

		partial void btnWeekMaxNumberTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			configNumNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ChangeMaxInputText"), handleChangeMaxInputText);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "Define";
			popoverContent.numValue = btnWeekMaxNumber.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect (sender.Frame, vwWeek, UIPopoverArrowDirection.Any, true);
		}

		partial void btnWeekMaxPercentageTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			configNumNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ChangeMaxInputText"), handleChangeMaxInputText);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "Define";
			popoverContent.numValue = btnWeekMaxPercentage.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect (sender.Frame, vwWeek, UIPopoverArrowDirection.Any, true);
		}

		partial void btnMaxStartDayTapped (UIKit.UIButton sender)
		{
			sender.Selected = true;
			configNumNotif = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("ChangeMaxInputText"), handleChangeMaxInputText);
			PopoverViewController popoverContent = new PopoverViewController ();
			popoverContent.PopType = "numberPad";
			popoverContent.SubPopType = "Define";
			popoverContent.numValue = btnMaxNumber.TitleLabel.Text;
			popoverController = new UIPopoverController (popoverContent);
			popoverController.Delegate = new MyPopDelegate (this);
			popoverController.PopoverContentSize = new CGSize (210, 300);
			popoverController.PresentFromRect (sender.Frame, vwWeek, UIPopoverArrowDirection.Any, true);
		}

		partial void btnWeekResetTapped (UIKit.UIButton sender)
		{


			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Week != null) {

				sgWeekendDays.SelectedSegment = (GlobalSettings.WBidINIContent.Week.IsMaxWeekendDefault) ? 0 : 1;
				btnWeekMaxNumber.SetTitle (GlobalSettings.WBidINIContent.Week.MaxNumberDefault, UIControlState.Normal);
				btnWeekMaxNumber.SetTitle (GlobalSettings.WBidINIContent.Week.MaxNumberDefault, UIControlState.Selected);
				btnWeekMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.Week.MaxPercentageDefault, UIControlState.Normal);
				btnWeekMaxPercentage.SetTitle (GlobalSettings.WBidINIContent.Week.MaxPercentageDefault, UIControlState.Selected);
				btnMaxStartDay.SetTitle (GlobalSettings.WBidINIContent.Week.StartDOW, UIControlState.Normal);
				btnMaxStartDay.SetTitle (GlobalSettings.WBidINIContent.Week.StartDOW, UIControlState.Selected);


			}

		}

		partial void btnWeekOkTapped (UIKit.UIButton sender)
		{

			if (GlobalSettings.WBidINIContent.Week == null)
				GlobalSettings.WBidINIContent.Week = new Week ();

			GlobalSettings.WBidINIContent.Week.IsMaxWeekend = (sgWeekendDays.SelectedSegment == 0) ? true : false;
			GlobalSettings.WBidINIContent.Week.MaxNumber = btnWeekMaxNumber.TitleLabel.Text;
			GlobalSettings.WBidINIContent.Week.MaxPercentage = btnWeekMaxPercentage.TitleLabel.Text;
			GlobalSettings.WBidINIContent.Week.StartDOW = btnMaxStartDay.TitleLabel.Text;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());



			LoadWeekTabChanges ();



		}

		partial void sgHotelListChanged (UIKit.UISegmentedControl sender)
		{
			if (GlobalSettings.WBidINIContent.SourceHotel == null)
				GlobalSettings.WBidINIContent.SourceHotel = new SourceHotel ();
			GlobalSettings.WBidINIContent.SourceHotel.SourceType = (int)sgHotelList.SelectedSegment + 1;
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());



		}



		private void LoadWeekTabChanges ()
		{
           
			if (GlobalSettings.Lines != null) {
				if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsOverlap) {

					//CalculateLineProperties objCalculateLineProperties = new CalculateLineProperties();
					foreach (Line line in GlobalSettings.Lines) {
						// line.Weekend = objCalculateLineProperties.CalcWkEndProp(line);
						line.Weekend = CalcWkEndProp (line);
					}
				} else if (GlobalSettings.MenuBarButtonStatus.IsOverlap)
				{
				} else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop) {
					//CaculateVacationLineProperties objC?aculateVacationLineProperties = new CaculateVacationLineProperties();
					foreach (Line line in GlobalSettings.Lines) {
						line.Weekend = CalcWkEndPropDrop (line);
					}
				} else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
					// CaculateVacationLineProperties objCaculateVacationLineProperties = new CaculateVacationLineProperties();
					foreach (Line line in GlobalSettings.Lines) {
						line.Weekend = CalcWkEndPropVacation (line);
					}
				}

				//save line file


				Dictionary<string, Line> lines = new Dictionary<string, Line> ();
               
				foreach (Line line in GlobalSettings.Lines) {
                    
					lines.Add (line.LineNum.ToString (), line);
				}

				LineInfo lineInfo = new LineInfo () {
					LineVersion = GlobalSettings.LineVersion,
					Lines = lines

				};
				//var filename = WBidHelper.GetCurrentlyLoadedLinefileName();
				//if (filename != string.Empty)
				//{
				//	var jsonLineString = JsonConvert.SerializeObject(lineInfo);
				//	File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + filename, jsonLineString);
				//}
				//var fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
				//var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
				//ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
				//linestream.Dispose ();
				//linestream.Close ();

			}


		}

		#region WkEndProp calculation

		/// <summary>
		/// Purpose : Calculate wkend properties  
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string CalcWkEndProp (Line line)
		{
			Trip trip = null;
			DateTime tripDate = DateTime.MinValue;
			int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
			string wkDayWkEnd = string.Empty;
			bool isLastTrip = false;
			int paringCount = 0;
			foreach (string pairing in line.Pairings) {
				trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing.Substring (0, 4));
				if (trip == null) {
					trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing);

				}

				//trip = (GlobalSettings.Trip.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
				tripDay = Convert.ToInt16 (pairing.Substring (4, 2));
				tripLength = trip.PairLength;
				tripDate = DateTime.MinValue;
				dayOfWeek = 0;
				for (int index = 0; index < tripLength; index++) {
					isLastTrip = ((line.Pairings.Count - 1) == paringCount);
					paringCount++;
					//tripDate = new DateTime(Convert.ToInt32(GlobalSettings.CurrentBidDetails.Year), Convert.ToInt32(GlobalSettings.CurrentBidDetails.Month), tripDay);
					tripDate = WBidCollection.SetDate (tripDay, isLastTrip);
					dayOfWeek = (int)(tripDate.AddDays (index).DayOfWeek);
					// 0 = Sunday, 6 = Saturday
					if (dayOfWeek == 0 || dayOfWeek == 6) {
						wkEndCount++;
					}
					totDays++;
				}
			}


			// bool isMaxWeek = (isApply) ? IsMaxWeekend : GlobalSettings.WBidINIContent.Week.IsMaxWeekend;
			bool isMaxWeek = GlobalSettings.WBidINIContent.Week.IsMaxWeekend;

			if (isMaxWeek) {
				// int maxNumber = (isApply) ? int.Parse(MaxNumber) : int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
				int maxNumber = int.Parse (GlobalSettings.WBidINIContent.Week.MaxNumber);
				wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

			} else {
				// int maxPercentage = (isApply) ? int.Parse(MaxPercentage) : int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
				int maxPercentage = int.Parse (GlobalSettings.WBidINIContent.Week.MaxPercentage);
				wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
			}


			return wkDayWkEnd;
		}

		public string CalcWkEndPropVacation (Line line)
		{
			Trip trip = null;
			DateTime tripDate = DateTime.MinValue;
			int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
			string wkDayWkEnd = string.Empty;
			bool isLastTrip = false;
			int paringCount = 0;
			foreach (string pairing in line.Pairings) {
				//Get trip
				trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing.Substring (0, 4));
				if (trip == null) {
					trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing);

				}


				VacationStateTrip vacTrip = null;
				if (line.VacationStateLine.VacationTrips != null) {
					vacTrip = line.VacationStateLine.VacationTrips.Where (x => x.TripName == pairing).FirstOrDefault ();
				}
				if (vacTrip != null) {
					if (vacTrip.TripVacationStartDate == DateTime.MinValue) {
						continue;
					}

					tripDate = vacTrip.TripVacationStartDate;
					tripLength = vacTrip.VacationDutyPeriods.Where (x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count ();
				} else {
					isLastTrip = ((line.Pairings.Count - 1) == paringCount);
					paringCount++;
					tripDay = Convert.ToInt16 (pairing.Substring (4, 2));
					//tripDate = new DateTime(Convert.ToInt32(GlobalSettings.CurrentBidDetails.Year), Convert.ToInt32(GlobalSettings.CurrentBidDetails.Month), tripDay);
					tripDate = WBidCollection.SetDate (tripDay, isLastTrip);
					tripLength = trip.PairLength;
				}


				dayOfWeek = 0;
				for (int index = 0; index < tripLength; index++) {

					dayOfWeek = (int)(tripDate.AddDays (index).DayOfWeek);
					if (dayOfWeek == 0 || dayOfWeek == 6) {
						wkEndCount++;
					}
					totDays++;
				}
			}


			if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend) {
				int maxNumber = int.Parse (GlobalSettings.WBidINIContent.Week.MaxNumber);
				wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

			} else {
				int maxPercentage = int.Parse (GlobalSettings.WBidINIContent.Week.MaxPercentage);
				wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
			}


			return wkDayWkEnd;
		}

		public string CalcWkEndPropDrop (Line line)
		{
			Trip trip = null;
			DateTime tripDate = DateTime.MinValue;
			int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
			string wkDayWkEnd = string.Empty;
			bool isLastTrip = false;
			int paringCount = 0;
			foreach (string pairing in line.Pairings) {
				//Get trip
				trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing.Substring (0, 4));
				if (trip == null) {
					trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == pairing);

				}
				isLastTrip = ((line.Pairings.Count - 1) == paringCount);
				paringCount++;
				VacationStateTrip vacTrip = null;

				if (line.VacationStateLine.VacationTrips != null) {
					vacTrip = line.VacationStateLine.VacationTrips.Where (x => x.TripName == pairing).FirstOrDefault ();
				}
				if (vacTrip != null) {
					//we dont need to consider vacation trip
					continue;
				} else {
					tripDay = Convert.ToInt16 (pairing.Substring (4, 2));
					// tripDate = new DateTime(Convert.ToInt32(GlobalSettings.CurrentBidDetails.Year), Convert.ToInt32(GlobalSettings.CurrentBidDetails.Month), tripDay);
					tripDate = WBidCollection.SetDate (tripDay, isLastTrip);
					tripLength = trip.PairLength;
				}

				//    Convert.ToInt16(pairing.Substring(4, 2));
				//tripLength = trip.PairLength;
				//tripDate = DateTime.MinValue;
				dayOfWeek = 0;
				for (int index = 0; index < tripLength; index++) {

					dayOfWeek = (int)(tripDate.AddDays (index).DayOfWeek);
					if (dayOfWeek == 0 || dayOfWeek == 6) {
						wkEndCount++;
					}
					totDays++;
				}
			}


			if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend) {
				int maxNumber = int.Parse (GlobalSettings.WBidINIContent.Week.MaxNumber);
				wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

			} else {
				int maxPercentage = int.Parse (GlobalSettings.WBidINIContent.Week.MaxPercentage);
				wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
			}


			return wkDayWkEnd;
		}

		#endregion
	}
}

