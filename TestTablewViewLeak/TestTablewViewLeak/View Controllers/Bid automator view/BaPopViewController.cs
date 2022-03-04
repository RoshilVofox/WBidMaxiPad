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
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.iOS
{
	public partial class BaPopViewController : UIViewController
	{

		#region  properties
		private ObservableCollection<AppliedStates> _allAppliedStates;
		public ObservableCollection<AppliedStates> AllAppliedStates
		{
			get
			{
				return _allAppliedStates;
			}
			set
			{
				_allAppliedStates = value;

			}
		}


		private ObservableCollection<string> _groupNameList;
		public ObservableCollection<string> GroupNameList
		{
			get
			{
				return _groupNameList;
			}
			set
			{
				_groupNameList = value;

			}
		}
		private string _selectedGroupName;
		public string SelectedGroupName
		{
			get
			{
				return _selectedGroupName;
			}
			set
			{

				_selectedGroupName = value;
				SetContent();

			}
		}
		private int _totalLines;
		public int TotalLines
		{
			get
			{
				return _totalLines;
			}
			set
			{

				_totalLines = value;

			}
		}
		private AppliedStates appliedStates;
		private AppliedStateType appliedStateType;
		private List<AppliedStateType> WeightType;
		private List<AppliedStateType> ConstraintsType;
		QuickSetColumn QuickSetColumn;
		QuickSetCSW QuickSetCSW;



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

//        public  ObservableCollection<DateTime> DaysList { get; set; }
//
//        private string _selectedGroupName;
//        public string SelectedGroupName
//        {
//            get
//            {
//				return _selectedGroupName;
//            }
//            set
//            {
//				_selectedGroupName = value;
//				if (_selectedGroupName != string.Empty)
//                {
//                    GeneratePairingDetails();
//                }
//               // RaisePropertyChanged(() => SelectedTripName);
//            }
//        }

		static ObservableCollection<TripData> tripData = new ObservableCollection<TripData>();

        public DateTime SelectedDate { get; set; }
		WBidState wBidStateContent;

		#endregion

		public BaPopViewController () : base ("BaPopViewController", null)
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

			try
			{
				wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				// load datas

				AllAppliedStates = new ObservableCollection<AppliedStates>();
				//DaysList = new ObservableCollection<DateTime>();

				//Set Group Name list here
				//			GroupNameList = GlobalSettings.Trip.Select(x => x.TripNum).ToList();
				//
				//			if (GroupNameList.Count > 0)
				//            {
				//				SelectedGroupName = GroupNameList[0];
				//            }
				// Perform any additional setup after loading the view, typically from a nib.
				tblTripView.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
				tblTripView.TableFooterView = new UIView(CGRect.Empty);
				tblTripView.Layer.BorderWidth = 1;
				tblTripView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;



				tblTripPairingView.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
				tblTripPairingView.TableFooterView = new UIView(CGRect.Empty);
				tblTripPairingView.Layer.BorderWidth = 1;
				tblTripPairingView.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
				tblTripPairingView.TableHeaderView = new UIView(new CGRect(0, 0, 550, 30));

				tblTripView.Source = new GrouopViewSource(this);
				//tblDayView.Source = new DayViewSource (this);
				tblTripPairingView.Source = new TripPairingViewSource(this);
				tblTripPairingView.EstimatedRowHeight = 50;
				tblTripPairingView.RowHeight = UITableView.AutomaticDimension;







				if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAGroup != null)
				{
					GroupNameList = new ObservableCollection<string>(wBidStateContent.CalculatedBA.BAGroup.Select(x => x.GroupName).Distinct());
					//GroupNameList = new ObservableCollection<string>(ServiceLocator.Current.GetInstance<MainViewModel>().Lines.Select(x => x.BAGroup).Distinct());

					SelectedGroupName = GroupNameList.FirstOrDefault();
				}

				tblTripView.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);
				SetGroupName(SelectedGroupName);
			}
			catch (Exception ex)
			{ 
			}
		}
		public void SetGroupName(string grpName)
		{
			SelectedGroupName = GroupNameList.FirstOrDefault(x=>x==grpName);
			lblGroup.Text = SelectedGroupName;
			lblTotalLines.Text = TotalLines.ToString ();
		}
//		void btnExportTapped (object sender, EventArgs e)
//		{
//			if(GlobalSettings.CurrentBidDetails.Postion=="CP"||GlobalSettings.CurrentBidDetails.Postion=="FO"){
//				UIActionSheet sheet = new UIActionSheet("Select", null, null, null, new string[] { "Export to Calendar", "Export to FFDO" });
//				sheet.ShowFrom(btnExport.Frame, tbTopBar, true);
//				sheet.Clicked += (object senderSheet, UIButtonEventArgs ee) => {
//					//var tripDate = DaysList[tblDayView.IndexPathForSelectedRow.Row].Date;
//					var tripName = GroupNameList[tblTripView.IndexPathForSelectedRow.Row];
//					//tripName = tripName + tripDate.Date.Day.ToString().PadLeft(2,' ');
////					if(ee.ButtonIndex==0){
////						//ExportTripDetails (tripName,tripDate);
////					} else {
////						//FFDO
////						//string ffdoData= GetFlightDataforFFDB(tripName,tripDate);
////						UIPasteboard clipBoard = UIPasteboard.General;
////						clipBoard.String = ffdoData;
////					}
//				};
//			} else {
//				//var tripDate = DaysList[tblDayView.IndexPathForSelectedRow.Row].Date;
//				var tripName = GroupNameList[tblTripView.IndexPathForSelectedRow.Row];
//				//tripName = tripName + tripDate.Date.Day.ToString().PadLeft(2,' ');
//				//ExportTripDetails (tripName,tripDate);
//			}
//		}

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
//            foreach (var item in gtripOpVector)
//            {
//                if (startDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//                    break;
//                if (item == '1')
//                {
//                    DaysList.Add(startDate);
//                }
//                startDate = startDate.AddDays(1);
//            }
        }

//        private void GeneratePairingDetails()
//        {
//
//           // TitleDetails = "Pairing " + SelectedTripName;
//           // MainViewModel mainView = ((MainViewModel)ServiceLocator.Current.GetInstance<MainViewModel>());
//          //  ObservableCollection<Line> lines = mainView.Lines;
//
//			SelectedTrip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == SelectedGroupName.Substring(0, 4));
//
//            if (SelectedTrip == null)
//            {
//				SelectedTrip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == SelectedGroupName);
// 
//            }
//            //DaysList = new ObservableCollection<DateTime>();
//            if (SelectedTrip != null)
//            {
//                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
//                {
//                    GenerateDates(SelectedTrip.GtripOpVector);
//                }
//                else
//                {
//
//					List<string> tempList = GlobalSettings.Lines.SelectMany(x => x.Pairings).Where(x => x.StartsWith(SelectedGroupName)).OrderBy(x => x.ToString()).ToList();
//                    for (int count = 0; count < tempList.Count; count++)
//                    {
//                        DaysList.Add(new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(tempList[count].Substring(4, 2))));
//                    }
//                }
//
//
//            }
//
//            if (DaysList.Count == 0)
//            {
//
//
//                DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
//                while (startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//                {
//                    DaysList.Add(startDate);
//                    startDate = startDate.AddDays(1);
//                }
//
//            }
//            CorrectionParams correctionParams = new Model.CorrectionParams();
//            correctionParams.selectedLineNum = CommonClass.selectedLine;
//			tripData = TripViewBL.GenerateTripDetails (SelectedGroupName + SelectedDate.Day.ToString ().PadLeft (2, '0'), correctionParams, false);
//
//
//
//        }


		// Trip List view data source.
		public class GrouopViewSource : UITableViewSource
		{

			BaPopViewController parentVC;
			public GrouopViewSource (BaPopViewController parent )
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

				return parentVC.GroupNameList.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				NSString cellIdentifier = new NSString ("cellIdentifier");
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell ();
				//cell.TextLabel.Text = "Trip " + (indexPath.Row + 1);
				cell.TextLabel.Text = parentVC.GroupNameList[indexPath.Row];
				return cell;
			}
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				parentVC.SelectedGroupName = parentVC.GroupNameList[indexPath.Row];
				parentVC.SetGroupName (parentVC.GroupNameList [indexPath.Row]);
              
				//parentVC.tblDayView.ReloadData ();
				parentVC.tblTripPairingView.ReloadData ();
				//parentVC.btnExport.Enabled = false;
			}
		}

	
		// Pairing view data source.
		public class TripPairingViewSource : UITableViewSource
		{

			BaPopViewController parentVC;
			public TripPairingViewSource (BaPopViewController parent )
			{
				parentVC = parent;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return parentVC.AllAppliedStates.Count;
			}

			public override string TitleForHeader (UITableView tableView, nint section)
			{
				if (parentVC.AllAppliedStates.Count > section)
				{
					return parentVC.AllAppliedStates [(int)section].Key;
				}
				else
					return string.Empty;
			}
		
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (parentVC.AllAppliedStates.Count > section)
				{
					return parentVC.AllAppliedStates[(int)section].AppliedStateTypes.Count;
				}
				else
					return 0;
//				
			}

			public override UIView GetViewForHeader (UITableView tableView, nint section)
			{

				UIView v = new UIView(new System.Drawing.RectangleF(0, 0, (float)tableView.Frame.Width,22));
				v.BackgroundColor = UIColor.FromRGB((nfloat)(234.0 / 255.0), (nfloat)(237.0 / 255.0), (nfloat)(180.0 / 255.0));
				UILabel label = new UILabel (new CGRect (30, 0, 100, 22));
				label.Font = UIFont.BoldSystemFontOfSize ((nfloat)12.0);
				v.Layer.BorderWidth = (nfloat)2.0;
				v.Layer.BorderColor = UIColor.Gray.CGColor;

				label.Text = parentVC.AllAppliedStates [(int)section].Key;
				v.AddSubview (label);

				return v;
			}
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{


				BAGroupCell cell = BAGroupCell.Create ();
//
//
//
				List<AppliedStateType> AppliedStateTypes =parentVC.AllAppliedStates[indexPath.Section].AppliedStateTypes;
				string concat="\n\n\n";
				if(AppliedStateTypes[indexPath.Row].Value !=null)
				 concat = String.Join("\n", AppliedStateTypes[indexPath.Row].Value.ToArray());
				cell.SetData (AppliedStateTypes[indexPath.Row].Key,concat);
				return cell;

//				NSString cellIdentifier = new NSString ("cellIdentifier");
//				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as UITableViewCell;
//				if (cell == null)
//					cell = new UITableViewCell ();
//				//cell.TextLabel.Text = "Pairing";
//				string concat = String.Join("\n", AppliedStateTypes[indexPath.Row].Value.ToArray());
//				cell.TextLabel.Text = concat;
//				cell.TextLabel.Font = UIFont.FromName("Courier",13);
//				cell.TextLabel.Lines = 0;
//				return cell;

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


		//Bid automator
		#region Private Methods
		private void SetContent()
		{
			if (SelectedGroupName != string.Empty)
			{
				int totLines = 0;
				//TotalLines = ServiceLocator.Current.GetInstance<MainViewModel>().Lines.Where(x => x.BAGroup == SelectedGroupName).Count();
				var grpInfo = wBidStateContent.CalculatedBA.BAGroup.FirstOrDefault(x => x.GroupName == SelectedGroupName);
				if (grpInfo != null)
				{
					totLines = (grpInfo.Lines == null) ? 0 : grpInfo.Lines.Count();
				}
				TotalLines = totLines;
				AllAppliedStates = new ObservableCollection<AppliedStates>();
				SetSortInformation();
				SetFilterInformation();
			}
		}

		private void SetSortInformation()
		{
			AppliedStates sortState = new AppliedStates();
			sortState.Key = "Sort Options";

			sortState.AppliedStateTypes = new List<AppliedStateType>();
			AppliedStateType appliedStateType = new AppliedStateType();
			if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BASort != null)
			{
				var currentsort = wBidStateContent.CalculatedBA.BASort.SortColumn;

				if (currentsort == "LinePay")
					appliedStateType.Key = "Bottom Line Pay Per Total";
				else if (currentsort == "PayPerDay")
					appliedStateType.Key = "Most Pay Per Day";
				else if (currentsort == "PayPerDutyHour")
					appliedStateType.Key = "Most Pay Duty Hour";
				else if (currentsort == "PayPerFlightHour")
					appliedStateType.Key = "Most Pay Per Flight Hour";
				else if (currentsort == "PayPerTimeAway")
					appliedStateType.Key = "Most Pay Per Time Away From Base";
				else if (currentsort == "BlockSort")
					appliedStateType.Key = "Block Sort";


				if (appliedStateType.Key == "Block Sort")
				{
					//get the Block sort names from the block sort code stored in the global settings value
					List<string> blockSortitems = new List<string>();
					wBidStateContent.CalculatedBA.BASort.BlokSort.ForEach(x =>
						{
							if (x != string.Empty)
								blockSortitems.Add(WBidCollection.GetBlockSortListData().First(y => y.Id.ToString() == x).Name);
						});
					appliedStateType.Value = blockSortitems;

				}


				sortState.AppliedStateTypes.Add(appliedStateType);

				AllAppliedStates.Add(sortState);
			}

		}

		private void SetFilterInformation()
		{
			try
			{
				AppliedStates filterState = new AppliedStates();
				filterState.Key = "Filters";

				filterState.AppliedStateTypes = new List<AppliedStateType>();

				AppliedStateType appliedStateType = null; ;
				if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAFilter != null && wBidStateContent.CalculatedBA.BAFilter.Count() > 0)
				{
					int filterCount = wBidStateContent.CalculatedBA.BAFilter.Count();
					string groupNum = SelectedGroupName.Replace("G", "");
					int grpNum = groupNum == string.Empty ? 0 : int.Parse(groupNum);
					int totalCombinations = (int)Math.Pow(2, filterCount);
					string combinationString = Convert.ToString((totalCombinations) - grpNum, 2).PadLeft(filterCount, '0');

					for (int pos = 0; pos < combinationString.Length; pos++)
					{
						if (combinationString[pos] == '1')
						{
							appliedStateType = new AppliedStateType();
							appliedStateType.Key = StateFilterNameToAcualName(wBidStateContent.CalculatedBA.BAFilter[pos].Name);
							appliedStateType.Value = SetFilterParameterFromState(wBidStateContent.CalculatedBA.BAFilter[pos]);
							filterState.AppliedStateTypes.Add(appliedStateType);
						}
					}





					AllAppliedStates.Add(filterState);
				}

			}
			catch (Exception ex)
			{ }
		}

		private List<string> SetFilterParameterFromState(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			switch (bidAutoItem.Name)
			{


			case "AP":
				tempString = ReadAMPMStateContent(bidAutoItem);
				break;
			case "CL":
				tempString = ReadCommutableLineStateContent(bidAutoItem);
				break;
			case "DOM":
				tempString = ReadDaysOfMonthStateContent(bidAutoItem);
				break;
			case "DOWA":
				tempString = ReadDaysOfWeekAllStateContent(bidAutoItem);
				break;
			case "DOWS":
				tempString = ReadDaysOfWeekSomeStateContent(bidAutoItem);
				break;
			case "DHFL":
				tempString =ReadDeadHeadFirstLastStateContent(bidAutoItem);
				break;
			case "ET":
				tempString=ReadEquipmentTypeStateContent(bidAutoItem);
				break;
			case "LT":
				tempString = ReadLineTypeStateContent(bidAutoItem);
				break;

			case "OC":
				tempString = ReadOvernightCitiesStateContent(bidAutoItem);
				break;
			case "RT":
				tempString=ReadRestTypeStateContent(bidAutoItem);
				break;
			case "SDOW":
				tempString = ReadStartDayOfOfWeekAllStateContent(bidAutoItem);
				break;
			case "TBL":
				tempString = ReadTripBlockLengthStateContent(bidAutoItem);
				break;


			}
			return tempString;

		}

		private List<string> ReadAMPMStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var ampmitem = (AMPMConstriants)bidAutoItem.BidAutoObject;
			if (ampmitem.AM)
			{
				str = "AM";
			}

			if (ampmitem.PM)
			{
				if (str != string.Empty)
					str += " , ";
				str += "PM";
			}
			if (ampmitem.MIX)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Mix";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDaysOfMonthStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			DaysOfMonthCx daysOfMonthState = (DaysOfMonthCx)bidAutoItem.BidAutoObject;
			List<DaysOfMonth> lstDaysOfMonth = WBidCollection.GetDaysOfMonthList();

			DaysOfMonth result;

			//Work days
			if (daysOfMonthState.WorkDays != null && daysOfMonthState.WorkDays.Count > 0)
			{

				foreach (int id in daysOfMonthState.WorkDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (result != null)
					{
						if (str != string.Empty)
							str += " , ";
						str += result.Date.ToString("MM/dd/yyyy");
					}

				}
				tempString.Add("Work Days :- ");
				tempString.Add(str);


			}

			//Off Days
			if (daysOfMonthState.OFFDays != null && daysOfMonthState.OFFDays.Count > 0)
			{

				foreach (int id in daysOfMonthState.OFFDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (str != string.Empty)
						str += " , ";
					str += result.Date.ToString("MM/dd/yyyy");

				}

				if (tempString.Count > 0)
				{
					tempString.Add("");
				}

				tempString.Add("Off Days :- ");
				tempString.Add(str);
			}



			return tempString;
		}

		private List<string> ReadDaysOfWeekAllStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			CxDays cxDay = (CxDays)bidAutoItem.BidAutoObject;
			if (cxDay.IsSun)
			{
				str = "Sunday";
			}

			if (cxDay.IsMon)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Monday";
			}
			if (cxDay.IsTue)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Tuesday";
			}

			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Wednesday";
			}

			if (cxDay.IsThu)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Thursday";
			}
			if (cxDay.IsFri)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Friday";
			}
			if (cxDay.IsSat)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Saturday";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDaysOfWeekSomeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell=string.Empty;
			string type = string.Empty;
			switch(int.Parse( cx3Parameter.ThirdcellValue))
			{
			case  (int)DayofTheWeek.Monday:
				thirdCell="Monday";
				break;
			case (int)DayofTheWeek.Tuesday:
				thirdCell = "Tuesday";
				break;
			case (int)DayofTheWeek.Wednesday:
				thirdCell = "Wednesday";
				break;
			case (int)DayofTheWeek.Thursday :
				thirdCell = "Thursday";
				break;
			case (int)DayofTheWeek.Friday:
				thirdCell = "Friday";
				break;
			case (int)DayofTheWeek.Saturday:
				thirdCell = "Saturday";
				break;
			case (int)DayofTheWeek.Sunday:
				thirdCell = "Sunday";
				break;


			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type="less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDeadHeadFirstLastStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (int.Parse(cx3Parameter.ThirdcellValue))
			{
			case (int)DeadheadType.First:
				thirdCell = "First";
				break;
			case (int)DeadheadType.Last:
				thirdCell = "Last";
				break;
			case (int)DeadheadType.Both:
				thirdCell = "Both";
				break;



			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadEquipmentTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (cx3Parameter.ThirdcellValue)
			{
				//case "300":
				//	thirdCell = "300";
				//	break;
				//case "500":
				//	thirdCell = "300";
				//	break;
				//case "35":
				//	//thirdCell = "300 & 500";
				//	thirdCell = "300";
				//	break;
				case "700":
					thirdCell = "700";
					break;
				case "800":
					thirdCell = "800";
					break;
				case "600":
					thirdCell = "8Max";
					break;
				case "200":
					thirdCell = "7Max";
					break;



			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadLineTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var lineTypeItem = (CxLine)bidAutoItem.BidAutoObject;
			if (lineTypeItem.Hard)
			{
				str = "Hard";
			}

			if (lineTypeItem.Reserve)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Reserve";
			}

			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && lineTypeItem.Ready)
			{ 
				if (str != string.Empty)
					str += " , ";
				str+= "Ready";

			}
			else if (GlobalSettings.CurrentBidDetails.Postion != "FA" && lineTypeItem.Blank)
			{

				if (str != string.Empty)
					str += " , ";
				str+= "Blank";

			}


			if (lineTypeItem.International)
			{
				if (str != string.Empty)
					str += " , ";
				str += "International";
			}

			if (lineTypeItem.NonConus)
			{
				if (str != string.Empty)
					str += " , ";
				str += "NonConus";
			}



			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadStartDayOfOfWeekAllStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			CxDays cxDay = (CxDays)bidAutoItem.BidAutoObject;
			if (cxDay.IsSun)
			{
				str = "Sunday";
			}

			if (cxDay.IsMon)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Monday";
			}
			if (cxDay.IsTue)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Tuesday";
			}

			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Wednesday";
			}

			if (cxDay.IsThu)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Thursday";
			}
			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Friday";
			}
			if (cxDay.IsSat)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Saturday";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadOvernightCitiesStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			BulkOvernightCityCx bulkOvernightCityCx = (BulkOvernightCityCx)bidAutoItem.BidAutoObject;
			if (bulkOvernightCityCx.OverNightYes != null && bulkOvernightCityCx.OverNightYes.Count > 0)
			{
				List<string> lstYesCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightYes.Contains(x.Id)).Select(y => y.Name).ToList();
				if (lstYesCityNames.Count() > 0)
				{
					tempString.Add("YES :-");

					foreach (var item in lstYesCityNames)
					{
						if (str != string.Empty)
							str += " , ";
						str += item;

					}
					tempString.Add(str);

				}

			}


			if (bulkOvernightCityCx.OverNightNo != null && bulkOvernightCityCx.OverNightNo.Count > 0)
			{
				List<string> lstNoCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightNo.Contains(x.Id)).Select(y => y.Name).ToList();
				if (lstNoCityNames.Count() > 0)
				{
					if (tempString.Count() > 0)
					{
						tempString.Add("");
					}

					tempString.Add("No :-");

					foreach (var item in lstNoCityNames)
					{
						if (str != string.Empty)
							str += " , ";
						str += item;

					}
					tempString.Add(str);

				}
			}




			return tempString;
		}

		private List<string> ReadRestTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (int.Parse(cx3Parameter.ThirdcellValue))
			{
			case (int)RestType.All:
				thirdCell = "All";
				break;
			case (int)RestType.InDomicile:
				thirdCell = "InDomicile";
				break;
			case (int)RestType.AwayDomicile:
				thirdCell = "AwayDomicile";
				break;



			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadTripBlockLengthStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var tripBlockLengthItem = (CxTripBlockLength)bidAutoItem.BidAutoObject;
			if (tripBlockLengthItem.IsBlock)
			{
				tempString.Add("Block");
			}
			else
			{
				tempString.Add("Trip");
			}

			if (tripBlockLengthItem.Turns)
			{

				str += "Turns";
			}




			if (tripBlockLengthItem.Twoday)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Twoday";
			}

			if (tripBlockLengthItem.ThreeDay)
			{
				if (str != string.Empty)
					str += " , ";
				str += "ThreeDay";
			}


			if (tripBlockLengthItem.FourDay)
			{
				if (str != string.Empty)
					str += " , ";
				str += "FourDay";
			}



			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadCommutableLineStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;

			var ftCommutableLine = (FtCommutableLine)bidAutoItem.BidAutoObject;

			str = "Commute City\t\t"+ ": " + ftCommutableLine.City;
			tempString.Add(str);

			str = "Take-Off Pad Time\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.CheckInTime);
			tempString.Add(str);

			str = "Back-to-Base Pad Time\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.BaseTime);
			tempString.Add(str);

			str = "Connect Time\t\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.ConnectTime);
			tempString.Add(str);

			str = "No Nights in middle\t" + ": " + ((ftCommutableLine.NoNights) ? "TRUE" : "FALSE");
			tempString.Add(str);

			str = string.Empty;
			if (ftCommutableLine.ToHome)
			{
				str += "To Home";
			}
			if (ftCommutableLine.ToWork)
			{
				if (str != string.Empty)
					str += " , ";
				str += "To Work";
			}
			tempString.Add(str);
			return tempString;
		}

		private string ConvertMinuteToHHMM(int minute)
		{
			string result = string.Empty;
			result = Convert.ToString(minute / 60).PadLeft(2, '0');
			result += ":";
			result += Convert.ToString(minute % 60).PadLeft(2, '0');
			return result;


		}

		private string StateFilterNameToAcualName(string name)
		{
			string actaulName = string.Empty;

			switch (name)
			{
			case "AP":
				actaulName = "Am - Pm";
				break;
			case "CL":
				actaulName = "Commutable Lines";
				break;
			case "DOM":
				actaulName = "Days of the Month";
				break;
			case "DOWA":
				actaulName = "Days of the Week - All";
				break;
			case "DOWS":
				actaulName = "Days of the Week - Some";
				break;
			case "DHFL":
				actaulName = "DH First - Last";
				break;
			case "ET":
				actaulName = "Equipment Type";
				break;
			case "LT":
				actaulName = "Line Type";
				break;
			case "OC":
				actaulName = "Overnight Cities";
				break;
			case "RT":
				actaulName = "Rest";
				break;

			case "SDOW":
				actaulName = "Start Day of Week";
				break;
			case "TBL":
				actaulName = "Trip-Block Length";
				break;
			}

			return actaulName;

		}

		#endregion


	}
}

