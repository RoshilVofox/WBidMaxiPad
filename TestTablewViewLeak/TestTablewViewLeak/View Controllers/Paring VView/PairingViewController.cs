using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.Linq;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;
using EventKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class PairingViewController : UIViewController
	{
        private List<string> _tripNameList;

        public Trip SelectedTrip { get; set; }

        public List<string> TripNameList
        {
            get
            {
                return _tripNameList;
			}
            set
            {
                _tripNameList = value;
                
            }
        }

        public  ObservableCollection<DateTime> DaysList { get; set; }

        private string _selectedTripName;
        public string SelectedTripName
        {
            get
            {
                return _selectedTripName;
            }
            set
            {
                _selectedTripName = value;
                if (_selectedTripName != string.Empty)
                {
                    GeneratePairingDetails();
                }
               // RaisePropertyChanged(() => SelectedTripName);
            }
        }

		static ObservableCollection<TripData> tripData = new ObservableCollection<TripData>();

        public DateTime SelectedDate { get; set; }
		public PairingViewController () : base ("PairingViewController", null)
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

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnExport.Enabled = false;
			btnExport.TouchUpInside+= btnExportTapped;
            DaysList = new ObservableCollection<DateTime>();
            TripNameList = GlobalSettings.Trip.Select(x => x.TripNum).ToList();

            if (TripNameList.Count > 0)
            {
                SelectedTripName = TripNameList[0];
            }
			// Perform any additional setup after loading the view, typically from a nib.
			tblTripView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			tblTripView.TableFooterView = new UIView (CGRect.Empty);
			tblTripView.Layer.BorderWidth = 1;
			tblTripView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;

			tblDayView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			tblDayView.TableFooterView = new UIView (CGRect.Empty);
			tblDayView.Layer.BorderWidth = 1;
			tblDayView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;

			tblTripPairingView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			tblTripPairingView.TableFooterView = new UIView (CGRect.Empty);
			tblTripPairingView.Layer.BorderWidth = 1;
			tblTripPairingView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
			tblTripPairingView.TableHeaderView = new UIView (new CGRect (0, 0, 550, 30));

			tblTripView.Source = new TripViewSource (this);
			tblDayView.Source = new DayViewSource (this);
			tblTripPairingView.Source = new TripPairingViewSource (this);

			tblTripView.SelectRow (NSIndexPath.FromRowSection (0, 0), false, UITableViewScrollPosition.None);
		  

		}

		void btnExportTapped (object sender, EventArgs e)
		{
			if(GlobalSettings.CurrentBidDetails.Postion=="CP"||GlobalSettings.CurrentBidDetails.Postion=="FO"){
				UIActionSheet sheet = new UIActionSheet("Select", null, null, null, new string[] { "Export to Calendar", "Export to FFDO" });
				sheet.ShowFrom(btnExport.Frame, tbTopBar, true);
				sheet.Clicked += (object senderSheet, UIButtonEventArgs ee) => {
					var tripDate = DaysList[tblDayView.IndexPathForSelectedRow.Row].Date;
					var tripName = TripNameList[tblTripView.IndexPathForSelectedRow.Row];
					tripName = tripName + tripDate.Date.Day.ToString().PadLeft(2,' ');
					if(ee.ButtonIndex==0){
						ExportTripDetails (tripName,tripDate);
					} else {
						//FFDO
						string ffdoData= GetFlightDataforFFDB(tripName,tripDate);
						UIPasteboard clipBoard = UIPasteboard.General;
						clipBoard.String = ffdoData;
					}
				};
			} else {
				var tripDate = DaysList[tblDayView.IndexPathForSelectedRow.Row].Date;
				var tripName = TripNameList[tblTripView.IndexPathForSelectedRow.Row];
				tripName = tripName + tripDate.Date.Day.ToString().PadLeft(2,' ');
				ExportTripDetails (tripName,tripDate);
			}
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB(string tripNum,DateTime tripDate)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0,4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			//DateTime dutPeriodDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			DateTime dutPeriodDate = tripDate;

			foreach (var dp in trip.DutyPeriods)
			{
				string datestring = dutPeriodDate.ToString("MM'/'dd'/'yyyy");
				if (trip.ReserveTrip)
				{

					result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveOut % 1440)).Replace(":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveIn % 1440)).Replace(":", "") + " \n";
				}
				else
				{
					foreach (var flt in dp.Flights)
					{
						result += datestring + " " + flt.FltNum.ToString().PadLeft(4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).Replace(":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).Replace(":", "") + " \n";
					}
				}
				dutPeriodDate = dutPeriodDate.AddDays(1);
			}
			return result;
		}


        private void ExportTripDetails(string tripName,DateTime tripDate)
        {
            List<ExportCalendar> lstExportCalendar = new List<ExportCalendar>();
           
            Trip trip = null;
           
            trip = GetTrip(tripName);

            if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing)
            {
                lstExportCalendar.Add(ExportTripDetails(trip, tripDate, tripName));
            }
            else
            {

                var dutyDetails = ExportDutyPeriodDetails(trip, tripDate, tripName);
                if (dutyDetails != null)
                {
                    foreach (var item in dutyDetails)
                    {
                        lstExportCalendar.Add(item);

                    }
                }

            }

			Calendar cal = new Calendar();
			cal.EventStore.RequestAccess(EKEntityType.Event,
				(bool granted, NSError e) =>
				{
					if (granted)
					{
						foreach (ExportCalendar exp in lstExportCalendar)
						{
							NSError err;
							try
							{
								if (exp.StarDdate > exp.EndDate)
									exp.EndDate = exp.EndDate.AddDays(1);
								EKEvent newEvent = EKEvent.FromStore(cal.EventStore);
								newEvent.StartDate = exp.StarDdate.DateTimeToNSDate();
								newEvent.EndDate = exp.EndDate.DateTimeToNSDate();
								newEvent.Title = exp.Title;
								newEvent.Notes = exp.TripDetails;
								newEvent.Calendar = cal.EventStore.DefaultCalendarForNewEvents;
								cal.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out err);
							}
							catch
							{
								err = new NSError();
							}
							if (err == null&&e==null)
							{
								InvokeOnMainThread(() =>
									{
										
                                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "The trip was added to the calendar.", UIAlertControllerStyle.Alert);
                                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                        this.PresentViewController(okAlertController, true, null);
                                    });
							} else {
								InvokeOnMainThread(() =>
									{
										
                                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", "Trip Export Failed", UIAlertControllerStyle.Alert);
                                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                        this.PresentViewController(okAlertController, true, null);
                                    });
							}
						}
						Console.WriteLine("Calendar access granted.");
					}
					else
					{
						InvokeOnMainThread(() =>
							{
							
                                UIAlertController okAlertController = UIAlertController.Create("Access Denied", "User Denied Access to Calendar \nPlease change the privacy settings for WbidiPad in Settings/Privacy/Calendars/WbidiPad", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            });
					}
				});
         

        }


        private ExportCalendar ExportTripDetails(Trip trip, DateTime tripStartDate, string tripName)
        {
            ExportCalendar exportCalendar = new ExportCalendar();

            string startTime = CalculateStartTimeBasedOnTime(trip, tripStartDate);
            exportCalendar.StarDdate = DateTime.Parse(tripStartDate.ToShortDateString() + " " + startTime);


            //Generate Subject
            //-----------------------------------------
            string subject = string.Empty;
            foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
            {

                if (subject != string.Empty)
                {
                    subject += "/";
                }
                subject += dutyPeriod.ArrStaLastLeg;
            }

            if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
            {
                subject = trip.TripNum.Substring(0, 4) + " " + subject;
            }

            //-----------------------------------------

            exportCalendar.Title = subject;

            exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(trip.DutyPeriods.Count - 1).ToShortDateString() + " " + CalculateEndTimeBasedOnTime(trip, tripStartDate));


            //CorrectionParams correctionParams = new Model.CorrectionParams();
            //correctionParams.selectedLineNum = lineNum;
            var tripDetails = TripViewBL.GenerateTripDetailsFiltered(tripName);

            string body = string.Empty;
            foreach (var item in tripDetails)
            {
                body += item.Content + Environment.NewLine;

            }

            exportCalendar.TripDetails = "REPORT " + CalculateStartTime(trip) + " CST/CDT";
            body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
                body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
            }
            exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;




            return exportCalendar;

        }

        private List<ExportCalendar> ExportDutyPeriodDetails(Trip trip, DateTime tripStartDate, string tripName)
        {
            List<ExportCalendar> lstExportCalendar = new List<ExportCalendar>();


          
            foreach (DutyPeriod dp in trip.DutyPeriods)
            {
                ExportCalendar exportCalendar = new ExportCalendar();
                int day = 0;
                //string startTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                string startTime = FormatTime((dp.ShowTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.StarDdate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + startTime);
                }
                else
                {
                    exportCalendar.StarDdate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + ConvertTimeToDomicile(startTime, tripStartDate.AddDays(day)));
                }
                // string endTime = GetTime(Helper.ConvertMinuteToHHMM(dp.ReleaseTime - (1440 * (dp.DutPerSeqNum - 1))), out day);
                string endTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + endTime);
                }
                else
                {
                    exportCalendar.EndDate = DateTime.Parse(tripStartDate.AddDays(day).ToShortDateString() + " " + ConvertTimeToDomicile(endTime, tripStartDate.AddDays(day)));
                }
                string subject = dp.ArrStaLastLeg;





                if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    subject = dp.ArrStaLastLeg + " " + FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) % 1440, out day);
                }
                else
                {

                    string domicileTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);
                    subject = dp.ArrStaLastLeg + " " + ConvertTimeToDomicile(domicileTime, tripStartDate.AddDays(day));

                }

                if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
                {
                    subject = trip.TripNum.Substring(0, 4) + " " + subject;
                }
                exportCalendar.Title = subject;

                ObservableCollection<TripData> lstDutyperiodDetails = TripViewBL.GenerateDutyPeriodDetails(trip,  dp.DutPerSeqNum);


                string body = string.Empty;
                foreach (var item in lstDutyperiodDetails)
                {
                    body += item.Content + Environment.NewLine;

                }

                exportCalendar.TripDetails = "REPORT " + startTime + " CST/CDT";
                body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
                if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
                {
                    exportCalendar.TripDetails += " (" + startTime + " Domicile Time)";
                    body += Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
                }
                exportCalendar.TripDetails += Environment.NewLine + Environment.NewLine + body;



                lstExportCalendar.Add(exportCalendar);
                tripStartDate = tripStartDate.AddDays(1);

            }

            return lstExportCalendar;


        }

        private string ConvertTimeToDomicile(string time, DateTime date)
        {




            int hours = 0;
            int minutes = 0;
            int result = 0;
            string strTime = string.Empty;

            if (time.Substring(6, 2) == "PM")
            {
                hours = 12;
            }

            hours += int.Parse(time.Substring(0, 2));
            minutes = int.Parse(time.Substring(3, 2));

            result = hours * 60 + minutes;

            result = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, result);

            hours = result / 60;
            minutes = result % 60;

            if (hours == 24)
            {
                hours = 0;
                strTime = "00";
            }
            else
            {
                strTime = (hours > 12) ? (hours - 12).ToString("d2") : hours.ToString("d2");
            }
            strTime += ":" + minutes.ToString("d2");

            strTime += ((hours >= 12) ? " PM" : " AM");

            return strTime;
        }
        private string FormatTime(int time, out int day)
        {
            day = 0;

            string stringTime = string.Empty;
            int hour = 0;
            int minutes = 0;


            hour = time / 60;
            minutes = time % 60;

            //if (hour > 24)
            //{
            //    day = 1;
            //    hour = hour - 24;
            //}
            if (hour >= 24)
            {
                if (minutes > 0)
                {
                    day = 1;
                }

                hour = hour - 24;
                stringTime = hour.ToString().PadLeft(2, '0');
            }
            else
            {
                stringTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            stringTime += ":" + minutes.ToString("d2");

            stringTime += ((hour >= 12) ? " PM" : " AM");

            return stringTime;

        }

        private string CalculateStartTime(Trip trip)
        {

            string startTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int startTimeMinute = 0;
            //int depTimeMinutes =  int.Parse(trip.DepTime);
            int depTimeMinutes = ConvertHhmmToMinutes(trip.DepTime);

            //Int16.Parse(trip.DepTime.Substring(0, 2)) * 60 + Int16.Parse(trip.DepTime.Substring(2, 2));
            //  startTimeMinute = depTimeMinutes - trip.BriefTime;
            startTimeMinute = trip.DutyPeriods[0].ShowTime;
            hour = startTimeMinute / 60;
            minutes = startTimeMinute % 60;

            if (hour == 24)
            {
                hour = 0;
                startTime = "00";
            }
            else
            {
                startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            startTime += ":" + minutes.ToString("d2");

            startTime += ((hour >= 12) ? " PM" : " AM");

            return startTime;

        }

        private string CalculateStartTimeBasedOnTime(Trip trip, DateTime tripStartDate)
        {

            string startTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int startTimeMinute = 0;
            startTimeMinute = trip.DutyPeriods[0].ShowTime;
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                startTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, startTimeMinute);
            }

            hour = startTimeMinute / 60;
            minutes = startTimeMinute % 60;

            if (hour == 24)
            {
                hour = 0;
                startTime = "00";
            }
            else
            {
                startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            startTime += ":" + minutes.ToString("d2");

            startTime += ((hour >= 12) ? " PM" : " AM");
            return startTime;
        }

        private string CalculateEndTimeBasedOnTime(Trip trip, DateTime tripStartDate)
        {

            string endtTime = string.Empty;
            int hour = 0;
            int minutes = 0;
            int startTimeMinute = 0;
            //int repTimeMinutes = int.Parse( trip.RetTime)%1440;
            int repTimeMinutes = ConvertHhmmToMinutes(trip.RetTime);
            //Int16.Parse(trip.RetTime.Substring(0, 2)) * 60 + Int16.Parse(trip.RetTime.Substring(2, 2));
            startTimeMinute = repTimeMinutes + trip.DebriefTime;
            if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
            {
                startTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, startTimeMinute);
            }

            hour = startTimeMinute / 60;
            minutes = startTimeMinute % 60;

            if (hour == 24)
            {
                hour = 0;
                endtTime = "00";
            }
            else
            {
                endtTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
            }
            endtTime += ":" + minutes.ToString("d2");

            endtTime += ((hour >= 12) ? " PM" : " AM");

            return endtTime;

        }


        private int ConvertHhmmToMinutes(string hhmm)
        {
            hhmm = hhmm.PadLeft(4, '0');
            int hours = Convert.ToInt32(hhmm.Substring(0, 2));
            int minutes = Convert.ToInt32(hhmm.Substring(2, 2));
            return hours * 60 + minutes;
        }
        private Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4));
            if (trip == null)
            {
                trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
            }

            return trip;

        }

		partial void btnDoneTapped (UIKit.UIBarButtonItem sender)
		{
			this.DismissViewController(true,null);
		}



        private void GenerateDates(string gtripOpVector)
        {

            //if (gtripOpVector.Trim() == string.Empty)
            if (string.IsNullOrEmpty(gtripOpVector))
            {
                return;
            }
            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            foreach (var item in gtripOpVector)
            {
                if (startDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                    break;
                if (item == '1')
                {
                    DaysList.Add(startDate);
                }
                startDate = startDate.AddDays(1);
            }
        }

        private void GeneratePairingDetails()
        {

           // TitleDetails = "Pairing " + SelectedTripName;
           // MainViewModel mainView = ((MainViewModel)ServiceLocator.Current.GetInstance<MainViewModel>());
          //  ObservableCollection<Line> lines = mainView.Lines;

            SelectedTrip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == SelectedTripName.Substring(0, 4));

            if (SelectedTrip == null)
            {
                SelectedTrip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == SelectedTripName);
 
            }
            DaysList = new ObservableCollection<DateTime>();
            if (SelectedTrip != null)
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    GenerateDates(SelectedTrip.GtripOpVector);
                }
                else
                {

                    List<string> tempList = GlobalSettings.Lines.SelectMany(x => x.Pairings).Where(x => x.StartsWith(SelectedTripName)).OrderBy(x => x.ToString()).ToList();
                    for (int count = 0; count < tempList.Count; count++)
                    {
                        DaysList.Add(new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(tempList[count].Substring(4, 2))));
                    }
                }


            }

            if (DaysList.Count == 0)
            {


                DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
                while (startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                {
                    DaysList.Add(startDate);
                    startDate = startDate.AddDays(1);
                }

            }
            CorrectionParams correctionParams = new Model.CorrectionParams();
            correctionParams.selectedLineNum = CommonClass.selectedLine;
			tripData = TripViewBL.GenerateTripDetails (SelectedTripName + SelectedDate.Day.ToString ().PadLeft (2, '0'), correctionParams, false);



        }


		// Trip List view data source.
		public class TripViewSource : UITableViewSource
		{

			PairingViewController parentVC;
			public TripViewSource (PairingViewController parent )
			{
				parentVC = parent;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				//return 10;

                return parentVC.TripNameList.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cellIdentifier");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();
				//cell.TextLabel.Text = "Trip " + (indexPath.Row + 1);
                cell.TextLabel.Text = parentVC.TripNameList[indexPath.Row];
				return cell;
			}
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
                parentVC.SelectedTripName = parentVC.TripNameList[indexPath.Row];

              
				parentVC.tblDayView.ReloadData ();
				parentVC.tblTripPairingView.ReloadData ();
				parentVC.btnExport.Enabled = false;
			}
		}

		// Days List view data source.
		public class DayViewSource : UITableViewSource
		{

			PairingViewController parentVC;
			public DayViewSource (PairingViewController parent )
			{
				parentVC = parent;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				//return 5;

                return parentVC.DaysList.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cellIdentifier");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();
				//cell.TextLabel.Text = (indexPath.Row + 1).ToString ();
                cell.TextLabel.Text = parentVC.DaysList[indexPath.Row].Day.ToString();
				return cell;
			}
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                parentVC.SelectedDate = parentVC.DaysList[indexPath.Row];
				parentVC.tblTripPairingView.ReloadData ();
				parentVC.btnExport.Enabled = true;
			}
		}

		// Pairing view data source.
		public class TripPairingViewSource : UITableViewSource
		{

			PairingViewController parentVC;
			public TripPairingViewSource (PairingViewController parent )
			{
				parentVC = parent;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				//return 1;
                return tripData.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cellIdentifier");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();
				//cell.TextLabel.Text = "Pairing";

                cell.TextLabel.Text = tripData[indexPath.Row].Content;
				cell.TextLabel.Font = UIFont.FromName("Courier",13);
                cell.TextLabel.Lines = 0;
				return cell;
			}
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{

			}
		}

        public class ExportCalendar
        {
            public string Title { get; set; }

            public DateTime StarDdate { get; set; }

            public DateTime EndDate { get; set; }

            public String TripDetails { get; set; }
        }
	}
}

