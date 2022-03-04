using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
    public partial class OverlapCorrectionViewController : UIViewController
    {

        /// <summary>
        ///  List of Positions
        /// </summary>
        public List<Day> LeadOutDays { get; set; }


        public string LastLegArrTime { get; set; }


        public bool importLine1;
        public OverlapCorrectionViewController()
            : base("OverlapCorrectionViewController", null)
        {
        }

        NSObject notif;
        UIPopoverController popoverController;

        class MyPopDelegate : UIPopoverControllerDelegate
        {
            OverlapCorrectionViewController _parent;
            public MyPopDelegate(OverlapCorrectionViewController parent)
            {
                _parent = parent;
            }

            public override void DidDismiss(UIPopoverController popoverController)
            {
                _parent.popoverController = null;
                NSNotificationCenter.DefaultCenter.RemoveObserver(_parent.notif);

                foreach (UIButton btn in _parent.btnBlockTime)
                {
                    btn.Selected = false;
                }
                _parent.btnRestTime.Selected = false;

				for (int i = 0; i < _parent.LeadOutDays.Count; i++) {
					_parent.LeadOutDays [i].FlightTime = Helper.ConvertHhmmToMinutes (_parent.btnBlockTime [i].TitleLabel.Text);
					_parent.LeadOutDays [i].FlightTimeHour = _parent.btnBlockTime [i].TitleLabel.Text;
				}
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            btnOK.SetBackgroundImage(UIImage.FromBundle("menuGreenActive.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            btnCancel.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);

            foreach (UIButton btn in btnBlockTime)
            {
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
                btn.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            }
            btnRestTime.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            btnRestTime.SetBackgroundImage(UIImage.FromBundle("textField.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            //get the dates
            GetDates();

           // bool IsPreviousMonthLineSelected = false;
            if (importLine1)
            {
              
                //get the selected line
                if (GlobalSettings.SelectedLine != null)
                {
                    Line selectedline = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == GlobalSettings.SelectedLine.LineNum);
                    if (selectedline != null)
                    {
                        GetBlockTime(selectedline);
                        SetlastLegArrivalTime(selectedline);
                    }
                }
            }

			for (int i = 0; i < LeadOutDays.Count; i++) {
				lblBlockTime[i].Text = LeadOutDays [i].Date.ToString ("MMM dd");
				btnBlockTime [i].SetTitle (LeadOutDays [i].FlightTimeHour, UIControlState.Normal);
				btnBlockTime [i].SetTitle (LeadOutDays [i].FlightTimeHour, UIControlState.Selected);
			}

			if (LastLegArrTime == null)
				LastLegArrTime = "00:00";
			btnRestTime.SetTitle (LastLegArrTime, UIControlState.Normal);
			btnRestTime.SetTitle (LastLegArrTime, UIControlState.Selected);

			sgOverlapChoice.SelectedSegment = 1;
        }

        partial void btnBlockTimeTapped(UIKit.UIButton sender)
        {
            sender.Selected = true;
			notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleBlockTimeChange);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "timePad";
            popoverContent.timeValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(200, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }

        public void handleBlockTimeChange(NSNotification n)
        {
            foreach (UIButton btn in btnBlockTime)
            {
                if (btn.Selected)
                {
                    btn.SetTitle(n.Object.ToString(), UIControlState.Normal);
                    btn.SetTitle(n.Object.ToString(), UIControlState.Selected);
                }
            }

            if (btnRestTime.Selected)
            {
                btnRestTime.SetTitle(n.Object.ToString(), UIControlState.Normal);
                btnRestTime.SetTitle(n.Object.ToString(), UIControlState.Selected);
            }
        }

        partial void btnRestTimeTapped(UIKit.UIButton sender)
        {
            sender.Selected = true;
			notif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ChangeTimeText"), handleBlockTimeChange);

            PopoverViewController popoverContent = new PopoverViewController();
            popoverContent.PopType = "timePad";
            popoverContent.timeValue = sender.TitleLabel.Text;
            popoverController = new UIPopoverController(popoverContent);
            popoverController.Delegate = new MyPopDelegate(this);
            popoverController.PopoverContentSize = new CGSize(200, 200);
            popoverController.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Any, true);
        }

        partial void sgOverlapChoiceChanged(UIKit.UISegmentedControl sender)
        {

        }

        partial void btnOKTapped(UIKit.UIButton sender)
        {

            LeadOutDays.ToList().ForEach(x => x.FlightTime = Helper.ConvertHhmmToMinutes(x.FlightTimeHour));
            GlobalSettings.LeadOutDays = LeadOutDays.ToList();


			LastLegArrTime = btnRestTime.Title(UIControlState.Normal);
            GlobalSettings.LastLegArrivalTime = Helper.ConvertHhmmToMinutes(LastLegArrTime);
			if(sgOverlapChoice.SelectedSegment==1)
				GlobalSettings.IsOverlapCorrection = true;
			else
				GlobalSettings.IsOverlapCorrection = false;



            loginViewController login = new loginViewController();
            login.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(login, true, () =>
				{
					//if(CommonClass.bidObserver!=null)
					CommonClass.bidObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("loginDetailsEntered"), furtherAfterLogin);
				});
        }

		private void furtherAfterLogin(NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (CommonClass.bidObserver);
			downloadBidDataViewController downloadBid = new downloadBidDataViewController();
			downloadBid.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			this.PresentViewController(downloadBid, true, null);
		}

        partial void btnCancelTapped(UIKit.UIButton sender)
        {
			if(CommonClass.bidObserver!=null)
				NSNotificationCenter.DefaultCenter.RemoveObserver (CommonClass.bidObserver);
            this.NavigationController.PopViewController(true);
        }




        #region Private Methods
        /// <summary>
        /// Get the dates for the overlap (add last 6 days from the current month and first 6 days  from the next month to the list)
        /// </summary>
        private void GetDates()
        {
            //Get the end of the current bid month.
            DateTime date = new DateTime(GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).AddDays(-1);
            //  DateTime date = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).AddMonths(1).AddDays(-1);

            LeadOutDays = new List<Day>();

            //add last 6 days from the current month and first 6 days  from the next month to the list
            Day day;
            date = date.AddDays(-5);
            for (int count = 0; count <= 11; count++)
            {
                day = new Day();
                day.Date = date;
                day.FlightTimeHour = "00:00";
                date = date.AddDays(1);
                LeadOutDays.Add(day);

            }
        }

        /// <summary>
        /// set the arrival time for the last leg in the duty period
        /// </summary>
        /// <param name="selectedline"></param>
        private void SetlastLegArrivalTime(Line selectedline)
        {
            if (selectedline != null && selectedline.Pairings.Count > 0)
            {
                Trip trip = GlobalSettings.Trip.Where(x => x.TripNum == selectedline.Pairings[selectedline.Pairings.Count - 1].Substring(0, 4)).FirstOrDefault();
                if (trip == null)
                {
                    trip = GlobalSettings.Trip.Where(x => x.TripNum == selectedline.Pairings[selectedline.Pairings.Count - 1]).FirstOrDefault();
                }
                if (trip != null && trip.DutyPeriods.Count > 0)
                {
                    var lastpairingdutyperiods = trip.DutyPeriods;

                    LastLegArrTime = Helper.ConvertMinutesToFormattedHour(lastpairingdutyperiods[lastpairingdutyperiods.Count - 1].LandTimeLastLeg - ((lastpairingdutyperiods.Count - 1) * 1440));
                }
            }
        }

        /// <summary>
        /// get the block time for the days
        /// </summary>
        /// <param name="selectedline"></param>
        private void GetBlockTime(Line selectedline)
        {
            bool isLastTrip = false; int paringCount = 0;
            ///iterate through all the pairings in the selected line
            foreach (var pairing in selectedline.Pairings)
            {

                //start date of the trip
                int pairingstartdate = Convert.ToInt32(pairing.Substring(4, 2));

                //get the duty period for the pairings
                List<DutyPeriod> dutyperiod = GlobalSettings.Trip.Where(x => x.TripNum.StartsWith(pairing.Substring(0, 4))).ToList().FirstOrDefault().DutyPeriods;
                int midnightimeconvert = 0;
                for (int count = 0; count < dutyperiod.Count; count++)
                {
                    isLastTrip = ((selectedline.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime newdate = WBidCollection.SetDate(pairingstartdate, isLastTrip);
                    newdate = newdate.AddDays(count);

                    if (LeadOutDays.Any(x => x.Date == newdate))
                    {
                        Day objday = LeadOutDays.Where(x => x.Date == newdate).FirstOrDefault();
                        objday.FlightTime = dutyperiod[count].Block;
                        objday.FlightTimeHour = Helper.ConvertMinutesToFormattedHour(dutyperiod[count].Block);
                        objday.DepartutreTime = dutyperiod[count].DepTimeFirstLeg - midnightimeconvert;
                        objday.ArrivalTime = dutyperiod[count].LandTimeLastLeg - midnightimeconvert;
                    }
                    midnightimeconvert += 1440;
                }

            }
        }
        #endregion
    }



}

