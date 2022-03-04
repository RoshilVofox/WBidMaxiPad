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
using System.Collections.Generic;


namespace WBid.WBidiPad.iOS
{
	public partial class CommutabilityLinesViewController : UIViewController
	{
		bool _isFirstTime;
		public ConstraintsChangeViewController ObjChangeController;
		private NSObject _DayOfcellNotification;
		UIPopoverController popoverController ;
		dynamic data;
		public Object data1 {
			get;
			set;
		}
//		public WtCommutableLineAuto data1 {
//			get;
//			set;
//		}
		public enum CommuteFromView
		{
			BidAutomator,
			CSWConstraints,
			CSWWeight,
			CSWCommutabilityConstraints,
			CSWCommutabilityWeights,
		};
		public CommuteFromView ObjFromView;
		public CommutabilityLinesViewController (IntPtr handle) : base (handle)
		{
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			//txtCellNumber.Dispose ();

			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			if (data1.GetType ().Name == "Commutability") {
				data = (Commutability)data1;
			} else
				data = (Commutability)data1;
			Title = Constants.CONSTRAINTS;
//			UIHelpers.StyleForButtonsBorderBlackRectange (new UIButton[]{btnViewCommuteTime, btnDoneSetting});
			UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{
				btnCityName, btnBackToBase, btnTimeCheckIn, btnTimeConnect
			});
			UIHelpers.StyleForButtonsWithBorder (new UIButton[]{ btnViewCommuteTime, btnDoneSetting });
		
			var  wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			switch(ObjFromView)
			{


//			case CommuteFromView.BidAutomator:	
//				btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null  && wBIdStateContent.BidAuto.DailyCommuteTimes.Count>0 && wBIdStateContent.BidAuto.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
//				break;
			case CommuteFromView.CSWCommutabilityConstraints:case CommuteFromView.CSWCommutabilityWeights:
				btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Count>0 && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
				break;
//			case CommuteFromView.CSWCommutabilityWeights:
//				btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Weights != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Count>0 && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
//				break;

			}
				
//			if (ObjFromView == CommuteFromView.CSWWeight) {
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

				if (data.ConnectTime < 1) {
					data.ConnectTime = 5;
				}
				if (data.CheckInTime < 1) {
					data.CheckInTime = 5;
				}
				SetTitleForButton (data.BaseTime, btnBackToBase);
				SetTitleForButton (data.ConnectTime, btnTimeConnect);
				SetTitleForButton (data.CheckInTime, btnTimeCheckIn);
//			} else {
//				string city = data.City;
//				if (city == null || city == "") {
//					btnCityName.SetTitle ("Select", UIControlState.Normal);	
//					_isFirstTime = true;
//
//				} else {
//					btnCityName.SetTitle (data.City, UIControlState.Normal);
//					_isFirstTime = false;
//
//				}
//				if (data.BaseTime < 1) {
//					data.BaseTime = 5;
//				}
//
//				if (data.ConnectTime < 1) {
//					data.ConnectTime = 5;
//				}
//				if (data.CheckInTime < 1) {
//					data.CheckInTime = 5;
//				}
//				SetTitleForButton (data.BaseTime, btnBackToBase);
//				SetTitleForButton (data.ConnectTime, btnTimeConnect);
//				SetTitleForButton (data.CheckInTime, btnTimeCheckIn);
//			}
		}

		void SetTitleForButton (double minutes, UIButton btn)
		{
			int hour = (int)minutes/60;
			int mins = (int)minutes%60;
			string minStr = mins.ToString("00");
			if (mins < 10) {
				minStr = string.Format("0{0}",mins);
			}
			btn.SetTitle(string.Format("{0}:{1}", hour.ToString("00") , minStr), UIControlState.Normal);
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

		partial void OnDoneSettingEvent (Foundation.NSObject sender)
		{
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (_isFirstTime)
			{

				switch (ObjFromView)
				{

//					case CommuteFromView.BidAutomator:	
//					SharedObject.Instance.ListConstraint.Add(this.data);
//
//					break;
				case CommuteFromView.CSWCommutabilityConstraints:
				case CommuteFromView.CSWCommutabilityWeights:
					btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Count>0 && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
					break;

				
				}

			}
			//calculate line property values for the commutability constraints. This line property will also be display in the suumary and bid line views
			CalculateLineProperties(wBIdStateContent);
			switch (ObjFromView)
			{
		case CommuteFromView.CSWCommutabilityConstraints:
				ConstraintCalculations constCalc = new ConstraintCalculations ();
				wBIdStateContent.CxWtState.Commute.Cx = true;
				wBIdStateContent.Constraints.Commute=this.data;
				constCalc.ApplyCommuttabilityConstraint (wBIdStateContent.Constraints.Commute);
				if(wBIdStateContent.CxWtState.Commute.Wt)
				{
					WeightCalculation constCal = new WeightCalculation ();
					constCal.ApplyCommuttabilityWeight (wBIdStateContent.Weights.Commute);
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddConstraints", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				this.DismissViewController(true,null);
				break;
			case CommuteFromView.CSWCommutabilityWeights:
				WeightCalculation constCalc1 = new WeightCalculation ();
				wBIdStateContent.CxWtState.Commute.Wt = true;
				wBIdStateContent.Constraints.Commute=this.data;
				constCalc1.ApplyCommuttabilityWeight (wBIdStateContent.Weights.Commute);
				if(wBIdStateContent.CxWtState.Commute.Cx)
				{
					ConstraintCalculations consta = new ConstraintCalculations ();
					consta.ApplyCommuttabilityConstraint (wBIdStateContent.Weights.Commute);
				}
				NSNotificationCenter.DefaultCenter.PostNotificationName ("AddWeights", null);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LineCountReload", null);
				this.DismissViewController(true,null);
				break;


			}


			//NavigationController.PopViewController(true);
		}

		partial void OnPadCheckInEvent (Foundation.NSObject sender){
			UIActionSheet sheet = new UIActionSheet("");
//			for (int i = 5; i <= 120; i+=5) {
//				string title = string.Format("{0}:{1}", i/60, i%60);
//				sheet.AddButton(title);
//			}
//			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
//				double minutes =(e.ButtonIndex+1)*5;
//				data.PadForCheckInInMinutes = minutes;
//				SetTitleForButton(minutes,btnTimeCheckIn);
//			};
//			sheet.ShowInView(this.View);

			UIButton ObjButton= (UIButton)sender;
			BAPopOverViewController ObjPopOver= new BAPopOverViewController();
			ObjPopOver.PopOverType="OnPadCheckInEvent";
			popoverController = new UIPopoverController (ObjPopOver);
			popoverController.PopoverContentSize = new CGSize (80, 380);
			popoverController.PresentFromRect(ObjButton.Frame,this.View,UIPopoverArrowDirection.Any,true);
			_DayOfcellNotification=NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("OnPadCheckInEvent"),UpdateOnLessthanEventCell);
		}
		private void CalculateLineProperties(WBidState wBIdStateContent)
		{
			int checkinTime = 0;
			int BaseTime = 0;
			var lines = GlobalSettings.Lines.Where(x => x.BlankLine==false).ToList();
			Trip trip;
			foreach (var line in lines)
			{
				DateTime tripStartDate = DateTime.MinValue;

				foreach (string pairing in line.Pairings)
				{
					trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
					trip = trip ?? GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
					List<Flight> lstflights= trip.DutyPeriods.SelectMany(x=>x.Flights).ToList();
					lstflights.RemoveAt(lstflights.Count()-1);
					line.NightsInMid = lstflights.Where(x => x.ArrSta == GlobalSettings.CurrentBidDetails.Domicile).Count();
				}
				if (line.WorkBlockList != null)
				{
					bool isCommuteFrontEnd = false;
					bool isCommuteBackEnd = false;

					foreach (WorkBlockDetails workBlock in line.WorkBlockList)
					{
						//Checking the  corresponding Commute based on Workblock Start time
						CommuteTime commutTimes = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

						// if (commutTimes != null && StateContent.ToWork)
						if (commutTimes != null )
						{
							if (commutTimes.EarliestArrivel != DateTime.MinValue)
							{
//								
								double value = Convert.ToDouble (data.CheckInTime);
//								isCommuteFrontEnd = ((commutTimes.EarliestArrivel.AddMinutes(value)) <= workBlock.StartDateTime);
								if (isCommuteFrontEnd)
								{
									line.commutableFronts++;
								}
							}
						}


						//Checking the  corresponding Commute based on Workblock End time
						//commutTimes = GlobalSettings.WBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDateTime.Date);
						// using EndDate to account for irregular datetimes in company time keeping method.
						commutTimes = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

						// if (commutTimes != null && StateContent.ToHome)
						if (commutTimes != null)
						{
							if (commutTimes.LatestDeparture != DateTime.MinValue)
							{
								double value = Convert.ToDouble (data.BaseTime);
								isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes(-value) >= workBlock.EndDateTime;
								if (isCommuteBackEnd)
								{
									line.CommutabilityBack++;
								}
							}
						}



					}

				}
				line.TotalCommutes = line.WorkBlockList.Count;
				line.CommutabilityFront = line.commutableFronts / line.TotalCommutes;
				line.CommutabilityBack = line.CommutableBacks / line.TotalCommutes;
				line.CommutabilityOverall = (line.commutableFronts + line.CommutableBacks)/(2 * line.TotalCommutes);
			}
		}
		public void UpdateOnLessthanEventCell(NSNotification n)
		{
			

			double minutes =(int.Parse(n.Object.ToString ())+1)*5;
			data.CheckInTime = (int)minutes;
			SetTitleForButton(minutes,btnTimeCheckIn);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);


		}
		CitiesPickerVC _picker;
		partial void OnSetCityNameEvent (Foundation.NSObject sender){
			 //show picker
			UIButton ObjOutton = (UIButton)sender;
			if (_picker == null) {
				_picker = Storyboard.InstantiateViewController ("CitiesPickerVC")as CitiesPickerVC;

				_picker.ObjFromView=(CitiesPickerVC.CommuteFromView)ObjFromView;
				_picker.PickedItem += (string value) => {
					data.City = value;
					btnCityName.SetTitle(value,UIControlState.Normal);
						
				popoverController.Dismiss(true);

					var  wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					switch(ObjFromView)
					{
//					case CommuteFromView.BidAutomator:	
//						btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null && wBIdStateContent.BidAuto.DailyCommuteTimes.Count>0 && wBIdStateContent.BidAuto.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
//						break;
					case CommuteFromView.CSWCommutabilityConstraints:case CommuteFromView.CSWCommutabilityWeights:
						btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Constraints != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Count>0 && wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
						break;
//					case CommuteFromView.CSWCommutabilityWeights:
//						btnViewCommuteTime.Enabled = btnDoneSetting.Enabled = (wBIdStateContent.Weights != null && wBIdStateContent.Weights.DailyCommuteTimes != null && wBIdStateContent.Weights.DailyCommuteTimes.Count>0 && wBIdStateContent.Weights.DailyCommuteTimes.Any(x=>x.EarliestArrivel!=DateTime.MinValue || x.LatestDeparture !=DateTime.MinValue));
//						break;
					}
				};
			}
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


			double minutes =(int.Parse(n.Object.ToString ())+1)*5;
			data.ConnectTime = (int)minutes;
			SetTitleForButton(minutes,btnTimeConnect);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_DayOfcellNotification);
			popoverController.Dismiss(true);


		}

		partial void OnViewCommuteTime (Foundation.NSObject sender){

			UIButton ObjButton= (UIButton)sender;
//			UIAlertView alert =new UIAlertView(Constants.AppName, "Not yet functional", null, "OK",null);
//			alert.Show();
			CommutableTimeViewController ObjPopOver=  Storyboard.InstantiateViewController ("CommutableTimeViewController")as CommutableTimeViewController;
			ObjPopOver.data1=data;
			ObjPopOver.ObjCommuteTimeFromView=(CommutableTimeViewController.CommuteFromView)ObjFromView;
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
			new UIAlertView(title, message, null, Constants.OK, null).Show();
		}

	}
}


