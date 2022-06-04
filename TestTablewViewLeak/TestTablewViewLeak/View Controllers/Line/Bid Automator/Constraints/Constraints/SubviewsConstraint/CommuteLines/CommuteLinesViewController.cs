using System;
using UIKit;
using WBid.WBidiPad.Model;
using Foundation;
using CoreGraphics;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using Microsoft.CSharp;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	

	public partial class CommuteLinesViewController : UIViewController
	{
		bool _isFirstTime;
		public ConstraintsChangeViewController ObjChangeController;
		private NSObject _DayOfcellNotification;
		LoadingOverlay loadingOverlay;
		UIPopoverController popoverController ;
	
		commutabilityCommon data;
		public Object data1 {
			get;
			set;
		}
	
		public CommuteFromView ObjFromView;
		public CommuteLinesViewController (IntPtr handle) : base (handle)
		{
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			////txtCellNumber.Dispose ();

			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();
			//this.View.UserInteractionEnabled = true;

		}


		public override void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			data = new commutabilityCommon();
			//btnNonStop.BackgroundColor = UIColor.White;
			//btnNonStop.Layer.BorderColor = UIColor.Black.CGColor;
			//btnNonStop.Layer.BorderWidth = 3;
			//btnNonStop.Layer.CornerRadius = btnNonStop.Frame.Width / 2;
			//btnNonStop.Layer.MasksToBounds = true;
			//nonStopCheckView.Layer.CornerRadius = nonStopCheckView.Frame.Width / 2;
			//nonStopCheckView.Layer.MasksToBounds = true;
			
			if (data1.GetType().Name == "FtCommutableLine")
			{
				var objdata = (FtCommutableLine)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.isNonStop =btnNonStop.Selected= objdata.IsNonStopOnly;
			
			}
			else if (data1.GetType().Name == "Commutability")
			{
				var objdata = (Commutability)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.SecondcellValue = objdata.SecondcellValue;
				data.ThirdcellValue = objdata.ThirdcellValue;
				data.Value = objdata.Value;
				data.Weight = objdata.Weight;
			}
			else
			{
				var objdata = (WtCommutableLineAuto)data1;
				data.BaseTime = objdata.BaseTime;
				data.CheckInTime = objdata.CheckInTime;
				data.City = objdata.City;
				data.CommuteCity = objdata.CommuteCity;
				data.ConnectTime = objdata.ConnectTime;
				data.NoNights = objdata.NoNights;
				data.ToHome = objdata.ToHome;
				data.ToWork = objdata.ToWork;
				data.Weight = objdata.Weight;
				data.isNonStop = btnNonStop.Selected = objdata.IsNonStopOnly;
			}
			Title = Constants.CONSTRAINTS;
			UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{
				btnCityName, btnBackToBase, btnTimeCheckIn, btnTimeConnect
			});
			UIHelpers.StyleForButtonsWithBorder (new UIButton[]{ btnViewCommuteTime, btnDoneSetting });
		
			var  wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            switch (ObjFromView)
            {


                case CommuteFromView.BidAutomator:
                    btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null && wBIdStateContent.BidAuto.DailyCommuteTimes.Count > 0 && wBIdStateContent.BidAuto.DailyCommuteTimes.Any(x => x.EarliestArrivel != DateTime.MinValue || x.LatestDeparture != DateTime.MinValue));
                    break;
                case CommuteFromView.CSWConstraints:
                case CommuteFromView.CSWCommutableAutoSort:
                case CommuteFromView.CSWWeight:
                    btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Count > 0 && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Any(x => x.EarliestArrivel != DateTime.MinValue || x.LatestDeparture != DateTime.MinValue));
                    break;

                

            }
				

				string city = data.City;
				if (city == null || city == "") {
					btnCityName.SetTitle ("Select", UIControlState.Normal);	
				btnDoneSetting.Enabled = false;
					_isFirstTime = true;

				} else {
					btnCityName.SetTitle (data.City, UIControlState.Normal);
				btnDoneSetting.Enabled = true;
					_isFirstTime = false;

				}
				if (data.BaseTime < 1) {
					data.BaseTime = 5;
				}
			
				if (data.ConnectTime < 1 ) {
					data.ConnectTime = btnNonStop.Selected? 0: 30;
				}
				if (data.CheckInTime < 1) {
					data.CheckInTime = 5;
				}
				SetTitleForButton (data.BaseTime, btnBackToBase);
				SetTitleForButton (data.ConnectTime, btnTimeConnect);
				SetTitleForButton (data.CheckInTime, btnTimeCheckIn);

          

        }
        partial void btnNonStopClick(UIButton sender)
        {
			btnNonStop.Selected=this.data.isNonStop = !sender.Selected;
			data.ConnectTime = btnNonStop.Selected ? 0 :30;
			SetTitleForButton(data.ConnectTime, btnTimeConnect);
			if (this.data.City!=null && this.data.City != "Select")
			{
				calculateDialyCommuteLines();
				//loadingOverlay = new LoadingOverlay(new CoreGraphics.CGRect(View.Bounds.X, 44, View.Bounds.Width, View.Bounds.Height), "Loading...");
				//View.Add(loadingOverlay);

				//InvokeInBackground(() =>
				//{

				//	BidAutoCalculateCommuteTimes bidAutoCalculateCommuteTimes = new BidAutoCalculateCommuteTimes();
				//	bidAutoCalculateCommuteTimes.ObjFromView = (CommuteFromView)ObjFromView;
				//	bidAutoCalculateCommuteTimes.CalculateDailyCommutableTimes(this.data.City, this.data.isNonStop);

				//	InvokeOnMainThread(() => {

				//		if (bidAutoCalculateCommuteTimes == null) return;

				//		if (bidAutoCalculateCommuteTimes.ErrorMessage != string.Empty)
				//		{
				//			DisplayAlertView("WBidMax", bidAutoCalculateCommuteTimes.ErrorMessage);
				//		}

				//		loadingOverlay.Hide();
						
				//	});
				//});
			}
		}
		private void DisplayAlertView(string caption, string message)
		{
			loadingOverlay.Hide();

			UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
			WindowAlert.RootViewController = new UIViewController();
			UIAlertController okAlertController = UIAlertController.Create(caption, message, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
			WindowAlert.MakeKeyAndVisible();
			WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
			//WindowAlert.Dispose();


		}
		void SetTitleForButton (double minutes, UIButton btn)
		{
			int hour = (int)minutes/60;
			int mins = (int)minutes%60;
			string minStr = mins.ToString("00");
			string hourStr = hour.ToString("00");

			if (mins < 10 && minutes>0)
			{
				minStr = string.Format("0{0}", mins);
			}
			else if(minutes == 0)
			{
 				hourStr ="--";
				minStr ="--";

			}
			
			btn.SetTitle(string.Format("{0}:{1}", hourStr, minStr), UIControlState.Normal);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);





		}
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		partial void OnBackToBaseEvent (Foundation.NSObject sender){
//			UIActionSheet sheet = new UIActionSheet("");
//			for (int i = 5; i <= 60; i+=5) {
//				string title = string.Format("{0}:{1}", i/60, i%60);
//				sheet.AddButton(title);
//			}
//			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
//				double minutes =(e.ButtonIndex+1)*5;
//				data.PadForBackToBaseInMinutes = minutes;
//				SetTitleForButton(minutes,btnBackToBase );
//			};
//			sheet.ShowInView(this.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnBackToBaseEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnBackToBaseEvent"),UpdateOnBackToBaseEventCell);
		}
		public void UpdateOnBackToBaseEventCell(NSNotification n)
		{


			double minutes =(int.Parse(n.Object.ToString ())+1)*5;
			data.BaseTime = (int) minutes;
			SetTitleForButton(minutes,btnBackToBase);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);


		}

        partial void OnDoneSettingEvent(Foundation.NSObject sender)
        {
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


            switch (ObjFromView)
            {

                case CommuteFromView.BidAutomator:
                    FtCommutableLine objbidauto = new FtCommutableLine();
                    objbidauto.BaseTime = this.data.BaseTime;
                    
                    objbidauto.CheckInTime = this.data.CheckInTime;
                    objbidauto.City = this.data.City;
                    objbidauto.CommuteCity = this.data.CommuteCity;
                    objbidauto.ConnectTime = this.data.ConnectTime;
                    objbidauto.NoNights = this.data.NoNights;
                    objbidauto.ToHome = this.data.ToHome;
                    objbidauto.ToWork = this.data.ToWork;
					objbidauto.IsNonStopOnly = this.data.isNonStop;
					if (_isFirstTime)
                    {
                        SharedObject.Instance.ListConstraint.Add(objbidauto);
                    }
                    else
                    {
                        foreach (var item in SharedObject.Instance.ListConstraint)
                        {
                            if (item is FtCommutableLine)
                            {
                                ((FtCommutableLine)item).BaseTime = this.data.BaseTime;
                                ((FtCommutableLine)item).BaseTime = this.data.BaseTime;
                                ((FtCommutableLine)item).CheckInTime = this.data.CheckInTime;
                                ((FtCommutableLine)item).City = this.data.City;
                                ((FtCommutableLine)item).CommuteCity = this.data.CommuteCity;
                                ((FtCommutableLine)item).ConnectTime = this.data.ConnectTime;
                                ((FtCommutableLine)item).NoNights = this.data.NoNights;
                                ((FtCommutableLine)item).ToHome = this.data.ToHome;
                                ((FtCommutableLine)item).ToWork = this.data.ToWork;
								((FtCommutableLine)item).IsNonStopOnly = this.data.isNonStop;
								break;
                            }
                        }
                    }

                    break;
                case CommuteFromView.CSWConstraints:
                case CommuteFromView.CSWWeight:
                case CommuteFromView.CSWCommutableAutoSort:
                    //wBIdStateContent.CxWtState.CLAuto.Cx = true;
                    FtCommutableLine objdata = new FtCommutableLine();
                    objdata.BaseTime = this.data.BaseTime;
                    objdata.BaseTime = this.data.BaseTime;
                    objdata.CheckInTime = this.data.CheckInTime;
                    objdata.City = this.data.City;
                    objdata.CommuteCity = this.data.CommuteCity;
                    objdata.ConnectTime = this.data.ConnectTime;
                    objdata.NoNights = this.data.NoNights;
                    objdata.ToHome = this.data.ToHome;
                    objdata.ToWork = this.data.ToWork;
					objdata.IsNonStopOnly = this.data.isNonStop;
					wBIdStateContent.Constraints.CLAuto = objdata;

                    WtCommutableLineAuto objwtdata = new WtCommutableLineAuto();
                    objwtdata.BaseTime = this.data.BaseTime;
                    objwtdata.BaseTime = this.data.BaseTime;
                    objwtdata.CheckInTime = this.data.CheckInTime;
                    objwtdata.City = this.data.City;
                    objwtdata.CommuteCity = this.data.CommuteCity;
                    objwtdata.ConnectTime = this.data.ConnectTime;
                    objwtdata.NoNights = this.data.NoNights;
                    objwtdata.ToHome = this.data.ToHome;
                    objwtdata.ToWork = this.data.ToWork;
                    objwtdata.Weight = this.data.Weight;
					objwtdata.IsNonStopOnly = this.data.isNonStop;

					wBIdStateContent.Weights.CLAuto = objwtdata;

                    if (ObjFromView == CommuteFromView.CSWConstraints)
                    {
                        wBIdStateContent.CxWtState.CLAuto.Cx = true;
                    }
                    else if (ObjFromView == CommuteFromView.CSWWeight)
                    {
                        wBIdStateContent.CxWtState.CLAuto.Wt = true;
                    }

                    break;
                    //           case CommuteFromView.CSWWeight:
                    //wBIdStateContent.CxWtState.CLAuto.Wt = true;
                    //	WtCommutableLineAuto objwtdata = new WtCommutableLineAuto();
                    //	objwtdata.BaseTime = this.data.BaseTime;
                    //	objwtdata.BaseTime = this.data.BaseTime;
                    //	objwtdata.CheckInTime = this.data.CheckInTime;
                    //	objwtdata.City = this.data.City;
                    //	objwtdata.CommuteCity = this.data.CommuteCity;
                    //	objwtdata.ConnectTime = this.data.ConnectTime;
                    //	objwtdata.NoNights = this.data.NoNights;
                    //	objwtdata.ToHome = this.data.ToHome;
                    //	objwtdata.ToWork = this.data.ToWork;
                    //	objwtdata.Weight = this.data.Weight;

                    //wBIdStateContent.Weights.CLAuto=objwtdata;
                    //break;


            }
			if (System.IO.File.Exists(WBidHelper.WBidCommuteFilePath))
			{
				System.IO.File.Delete(WBidHelper.WBidCommuteFilePath);
			}
            CalculateLineProperties lineproprty = new CalculateLineProperties();
            lineproprty.CalculateCommuteLineProperties(wBIdStateContent);

            switch (ObjFromView)
            {

                case CommuteFromView.BidAutomator:

                    ObjChangeController.ViewDismissViewControllerfromCommutableLine(this);
                    break;
                case CommuteFromView.CSWConstraints:
                    ConstraintCalculations constCalc = new ConstraintCalculations();
                    constCalc.ApplyCommutableLinesAutoConstraint(wBIdStateContent.Constraints.CLAuto);
                    if (wBIdStateContent.CxWtState.CLAuto.Wt)
                    {
                        //if the weight already set, changing pop up values will recalualte the commutable line properties. So we need to recalculate weight based on the new value
                        WeightCalculation constrainobj = new WeightCalculation();
                        constrainobj.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
                    this.DismissViewController(true, null);
                    break;
                case CommuteFromView.CSWWeight:
                    WeightCalculation constCalc1 = new WeightCalculation();
                    constCalc1.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    if (wBIdStateContent.CxWtState.CLAuto.Cx)
                    {
                        //if the constrain already set, changing pop up values will recalualte the commutable line properties. So we need to recalculate constrain based on the new value
                        WeightCalculation weightoobj = new WeightCalculation();
                        weightoobj.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
                    this.DismissViewController(true, null);
                    break;
                case CommuteFromView.CSWCommutableAutoSort:
                    if (!((wBIdStateContent.SortDetails.BlokSort.Contains("33")) || (wBIdStateContent.SortDetails.BlokSort.Contains("34")) || (wBIdStateContent.SortDetails.BlokSort.Contains("35"))))
                    {

                        wBIdStateContent.SortDetails.BlokSort.Add("33");
                    }
                    if (wBIdStateContent.CxWtState.CLAuto.Wt)
                    {
                        //if the weight already set, changing pop up values will recalualte the commutable line properties. So we need to recalculate weight based on the new value
                        WeightCalculation constrainobj = new WeightCalculation();
                        constrainobj.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    }
                    if (wBIdStateContent.CxWtState.CLAuto.Cx)
                    {
                        //if the constrain already set, changing pop up values will recalualte the commutable line properties. So we need to recalculate constrain based on the new value
                        WeightCalculation weightoobj = new WeightCalculation();
                        weightoobj.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LineCountReload", null);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
                    this.DismissViewController(true, null);
                    break;


            }

           

            if (wBIdStateContent.CxWtState.CLAuto.Cx == true)
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraints", null);
            }
            if (wBIdStateContent.CxWtState.CLAuto.Wt == true)
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("AddWeights", null);
            }

            if (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")) {

                NSNotificationCenter.DefaultCenter.PostNotificationName("reloadBlockSort", null);
            }


        }

		partial void OnPadCheckInEvent (Foundation.NSObject sender){
			

			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnPadCheckInEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnPadCheckInEvent"),UpdateOnLessthanEventCell);
		}
		public void UpdateOnLessthanEventCell(NSNotification n)
		{
			

			double minutes =(int.Parse(n.Object.ToString ())+1)*5;
			data.CheckInTime = (int)minutes;
			SetTitleForButton(minutes,btnTimeCheckIn);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);
			if (minutes < 120)
			{
                ShowPopUpInfo("WBidMax", "You are setting the take-off pad to less than 2:00, you may not meet the contractual requirement to take a flight that arrives 1 hours before check-in"); 
           
			}

		}
		CitiesPickerVC _picker;
		partial void OnSetCityNameEvent (Foundation.NSObject sender){
			 //show picker
			UIButton ObjOutton = (UIButton)sender;
		if (_picker == null) {
				_picker = Storyboard.InstantiateViewController ("CitiesPickerVC")as CitiesPickerVC;
				_picker.ObjFromView=(CommuteFromView)ObjFromView;
				_picker.isNonStop = btnNonStop.Selected;
				_picker.connectTime = this.data.ConnectTime;
				_picker.PickedItem += (string value) => {
					data.City = value;
					btnCityName.SetTitle(value,UIControlState.Normal);
					popoverController.Dismiss(true);
					var  wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					switch(ObjFromView)
					{
					case CommuteFromView.BidAutomator:	
						btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null && wBIdStateContent.BidAuto.DailyCommuteTimes.Count>0 && wBIdStateContent.BidAuto.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
						break;
					case CommuteFromView.CSWConstraints:
                    case CommuteFromView.CSWCommutableAutoSort:
                    case CommuteFromView.CSWWeight:
						btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimes != null && wBIdStateContent.Constraints.DailyCommuteTimes.Count>0 && wBIdStateContent.Constraints.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
						break;
					//case CommuteFromView.CSWWeight:
						//btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Weights != null && wBIdStateContent.Weights.DailyCommuteTimes != null && wBIdStateContent.Weights.DailyCommuteTimes.Count>0 && wBIdStateContent.Weights.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
						//break;
					}
				};
			}
            else
            {
				_picker.isNonStop = btnNonStop.Selected;
				_picker.connectTime = this.data.ConnectTime;
			}
            _picker.PreferredContentSize = new CGSize(500, 600);
			popoverController = new UIPopoverController (_picker);
			popoverController.PopoverContentSize = new CGSize (80, 380);
			popoverController.PresentFromRect(ObjOutton.Frame,this.View,0,true);

			//NavigationController.PushViewController (_picker, true);
		}

		partial void DismissCommuteViewController (NSObject sender)
		{
			this.DismissViewController(true,null);
		}
		partial void OnSetConnectTimeEvent (Foundation.NSObject sender){
			// show alert to select time
//			UIActionSheet sheet = new UIActionSheet("");
//			for (int i = 5; i <= 60; i+=5) {
//				string title = string.Format("{0}:{1}", i/60, i%60);
//				sheet.AddButton(title);
//			}
//			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
//				double minutes =(e.ButtonIndex+1)*5;
//				data.ConnectTimeInMinutes = minutes;
//				SetTitleForButton(minutes, btnTimeConnect);
//			};
//			sheet.ShowInView(this.View);
			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnBackToBaseEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnBackToBaseEvent"),UpdateOnSetConnectTimeEventCell);
		}
		public void UpdateOnSetConnectTimeEventCell(NSNotification n)
		{

			if(this.data.isNonStop )
            {
				this.data.isNonStop = btnNonStop.Selected = false;
			}
			
		
			
			double minutes =(int.Parse(n.Object.ToString ())+1)*5;
			data.ConnectTime = (int)minutes;
			SetTitleForButton(minutes,btnTimeConnect);
			calculateDialyCommuteLines();
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);


		}
		public void calculateDialyCommuteLines()
        {
			if (this.data.City != null && this.data.City != "Select")
			{

				loadingOverlay = new LoadingOverlay(new CoreGraphics.CGRect(View.Bounds.X, 44, View.Bounds.Width, View.Bounds.Height), "Loading...");
				View.Add(loadingOverlay);

				InvokeInBackground(() =>
				{

					BidAutoCalculateCommuteTimes bidAutoCalculateCommuteTimes = new BidAutoCalculateCommuteTimes();
					bidAutoCalculateCommuteTimes.ObjFromView = (CommuteFromView)ObjFromView;
					bidAutoCalculateCommuteTimes.CalculateDailyCommutableTimes(this.data.City, this.data.isNonStop, this.data.ConnectTime);

					InvokeOnMainThread(() => {

						if (bidAutoCalculateCommuteTimes == null) return;

						if (bidAutoCalculateCommuteTimes.ErrorMessage != string.Empty)
						{
							DisplayAlertView("WBidMax", bidAutoCalculateCommuteTimes.ErrorMessage);
						}

						loadingOverlay.Hide();

					});
				});
			}
		}

		partial void OnViewCommuteTime (Foundation.NSObject sender){

			UIButton ObjButton= (UIButton)sender;

			CommutableTimeViewController ObjPopOver=  Storyboard.InstantiateViewController ("CommutableTimeViewController")as CommutableTimeViewController;
			ObjPopOver.data1=data;
			ObjPopOver.ObjCommuteTimeFromView=(CommuteFromView)ObjFromView;
			popoverController = new UIPopoverController (ObjPopOver);
			ObjPopOver.popOver=popoverController;
			popoverController.PopoverContentSize = new CGSize (500, 600);
			popoverController.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);

		}

		partial void OnInfoCommuteCityEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Set_Commute_City, Constants.Set_Commute_City_Message);
		}

		partial void OnInfoConnectTimeEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Set_Connect_Times, Constants.Set_Connect_Times_Message);
		}

		partial void OnInfoPadCheckInEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Pad_For_Checkin, Constants.Pad_For_Checkin_Msg);
		}
		partial void OnInfoBackToBaseEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Pad_For_Back_To_Base, Constants.Pad_For_Back_To_Base_Msg);
		}

		partial void OnInfoViewCommuteTimeEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Get_Commute_Times, Constants.Get_Commute_Times_Message);
		}
		public void ShowPopUpInfo(string title, string message){
		
            UIAlertController okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create(Constants.OK, UIAlertActionStyle.Default, null));
            this.PresentViewController(okAlertController, true, null);
        }

	}
	public class commutabilityCommon
	{

		public int ConnectTime { get; set; }


		public int CommuteCity { get; set; }


		public string City { get; set; }


		public int CheckInTime { get; set; }


		public int BaseTime { get; set; }


		public bool NoNights { get; set; }


		public bool ToWork { get; set; }


		public bool ToHome { get; set; }


		public int SecondcellValue { get; set; }


		public int ThirdcellValue { get; set; }


		public int Type { get; set; }


		public int Value { get; set; }


		public decimal Weight { get; set; }

		public bool isNonStop { get; set; }

	}
}


