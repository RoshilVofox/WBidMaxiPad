using System;
using UIKit;
using Foundation;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core.Enum;

namespace  WBid.WBidiPad.iOS
{
	
	[Register("CommutablePickerView")]
	public class CommutablePickerView: UIView
	{
        //public enum CommuteFromView
        //{
        //    BidAutomator,
        //    CSWConstraints,
        //    CSWWeight,
        //    CSWCommutabilityConstraints,
        //    CSWCommutabilityWeights,
        //    CSWCommutabilitySort,
        //    CSWCommutableAutoSort,
        //    CSWCommutableManualSort,
        //};
		public CommuteFromView ObjCommutePickerFromView;

	
		int _numberItemPerRow = 7;
		int _numberRows = 6;
		nfloat _itemSize = 40;

       ObservableCollection<ConnectTimeHelper> _listCommuteTime=null;
	
		List<ConnectTimeView> _buttons;
       // FtCommutableLine objData;
		commutabilityCommon objData;
		public Object data1 {
			get;
			set;
		}
		public CommutablePickerView (commutabilityCommon data)
		{
			objData = data;
			//if (data.GetType ().Name == "FtCommutableLine") {
			//	objData = (FtCommutableLine)data;
			//} 
			//else if (data.GetType ().Name == "Commutability") {
			//	objData = (Commutability)data;
			//}else
			//	objData = (WtCommutableLineAuto)data;

			//data = new commutabilityCommon();
			//if (data1.GetType().Name == "FtCommutableLine")
			//{
			//	var objdata = (FtCommutableLine)data1;
			//	data.BaseTime = objdata.BaseTime;
			//	data.CheckInTime = objdata.CheckInTime;
			//	data.City = objdata.City;
			//	data.CommuteCity = objdata.CommuteCity;
			//	data.ConnectTime = objdata.ConnectTime;
			//	data.NoNights = objdata.NoNights;
			//	data.ToHome = objdata.ToHome;
			//	data.ToWork = objdata.ToWork;
			//}
			//else if (data1.GetType().Name == "Commutability")
			//{
			//	var objdata = (Commutability)data1;
			//	data.BaseTime = objdata.BaseTime;
			//	data.CheckInTime = objdata.CheckInTime;
			//	data.City = objdata.City;
			//	data.CommuteCity = objdata.CommuteCity;
			//	data.ConnectTime = objdata.ConnectTime;
			//	data.SecondcellValue = objdata.SecondcellValue;
			//	data.ThirdcellValue = objdata.ThirdcellValue;
			//	data.Value = objdata.Value;
			//	data.Weight = objdata.Weight;
			//}
			//else if (data1.GetType().Name == "WtCommutableLineAuto")
			//{
			//	var objdata = (WtCommutableLineAuto)data1;
			//	data.BaseTime = objdata.BaseTime;
			//	data.CheckInTime = objdata.CheckInTime;
			//	data.City = objdata.City;
			//	data.CommuteCity = objdata.CommuteCity;
			//	data.ConnectTime = objdata.ConnectTime;
			//	data.NoNights = objdata.NoNights;
			//	data.ToHome = objdata.ToHome;
			//	data.ToWork = objdata.ToWork;
			//	data.Weight = objdata.Weight;
			//}
			//else
			//{
			//	data = (commutabilityCommon)data1;
			//}

			
//			_labelColor = lb;
//			_oneTouchColor = one;
//			_twoTouchColor = two;
          //  objData = data;
		}
		public CommutablePickerView(IntPtr handle) : base(handle) {
			
		}


		public override void Draw (CoreGraphics.CGRect rect)
		{
			var height = CalculateHeight (rect.Width);
			this.Frame = new CoreGraphics.CGRect (rect.X, rect.Y, rect.Width, height);
			InitLayout ();
		}

		public override void SetNeedsLayout ()
		{
			base.SetNeedsLayout ();
		}

		/*
		 * Internal functions
		 */

		nfloat CalculateHeight(nfloat width)
		{
			_itemSize = width / 7;
			return (nfloat)(_itemSize * (_numberRows + 1) + 70);
		}

		void InitLayout ()
		{
			DateTime currentTime = DateTime.Now;
			// add date label - first row
			for (int i = 0; i < 7; i++) {
				UILabel lb = new UILabel ();
				lb.Frame = new CoreGraphics.CGRect (i * _itemSize, 31,  _itemSize , 30);
				lb.Text = Constants.DaysInWeek.ElementAt (i);
				lb.TextAlignment = UITextAlignment.Center;
				lb.ClipsToBounds = true;
				//lb.Layer.CornerRadius = _itemSize / 2;
				lb.BackgroundColor =UIColor.DarkGray; //UIColor.FromRGB((nfloat)(94.0 / 255.0), (nfloat)(158.0 / 255.0), (nfloat)(60.0 / 255.0));;
				lb.Layer.BorderWidth = 0.6f;
				lb.Layer.BorderColor = UIColor.White.CGColor;
				this.AddSubview (lb);
			}
			// add month
			UILabel lbMonth = new UILabel();
			lbMonth.Frame = new CoreGraphics.CGRect (0, 0, this.Bounds.Width, 30);
			lbMonth.TextAlignment = UITextAlignment.Center;
			lbMonth.TextColor = UIColor.Black;
			lbMonth.Font = UIFont.BoldSystemFontOfSize (15);
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //string[] monthNames = 
            //    System.Globalization.CultureInfo.CurrentCulture
            //        .DateTimeFormat.MonthGenitiveNames;
			
            //lbMonth.Text = string.Format ("{0} {1}", monthNames[currentTime.Month], currentTime.Year);

            lbMonth.Text  = new DateTime(DateTime.Now.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM", CultureInfo.InvariantCulture) + " " + GlobalSettings.CurrentBidDetails.Year;

			this.AddSubview (lbMonth);
			// draw the table
			UIView viewTable = new UIView();
			viewTable.BackgroundColor = UIColor.Clear;
			viewTable.Frame = new CoreGraphics.CGRect (0, 61, this.Bounds.Width, _itemSize * _numberRows);
			_buttons = new List<ConnectTimeView> ();
            GenerateCommuteTimes();
            int id = 1;

			for (int row = 0; row < _numberRows; row++) {
				for (int col = 0; col < _numberItemPerRow; col++) {
					ConnectTimeView View = new ConnectTimeView ();
					View = ConnectTimeView.Create();

                    var item = _listCommuteTime.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        View.SetValues(item.Day, item.Departure, item.Arrival,item.IsEnabled);
                    }
					//View.SetValues (row.ToString(), "1111", "2222");
					View.Frame = new CoreGraphics.CGRect (col * _itemSize, row * (_itemSize), _itemSize, _itemSize );
					View.BackgroundColor = UIColor.White; // default color;
					View.Layer.BorderColor = UIColor.LightGray.CGColor;
					View.Layer.BorderWidth = 0.6f;
					//btn.Layer.CornerRadius = _itemSize / 2;
					View.Tag = 0;
					//btn.Font = UIFont.BoldSystemFontOfSize(13);
					//btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
					_buttons.Add (View);
					viewTable.AddSubview (View);
                    id++;
				}
			}



          List<DaysOfMonth>  dayOfMonthList = WBidCollection.GetDaysOfMonthList();

//          foreach (var day in dayOfMonthList)
//          {
//				CommutableCalendarButton btn = _buttons.ElementAt(day.Id-1);
//				btn.BackgroundColor = UIColor.White;
//              if(day.Day!=string.Empty)
//              {
//				btn.SetTitle (day.Day, UIControlState.Normal);
//              }
//
//              btn.Enabled=day.IsEnabled;
//				btn.ID =day.Id;
//				btn.TouchUpInside += (object sender, EventArgs e) => {
//					CommutableCalendarButton sd = (CommutableCalendarButton)sender;
//					sd.Tag++;
//					if (sd.Tag%3 == 0) {
//						// gray
//						sd.State = 0;
//						sd.BackgroundColor = UIColor.White;
//						btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
//					}else if(sd.Tag%3 == 1){
//						// red
//						sd.State = 1;
//						sd.BackgroundColor = _oneTouchColor;
//						btn.SetTitleColor (UIColor.White, UIControlState.Normal);
//					}else{
//						// green
//						sd.State = 2;
//						sd.BackgroundColor = _twoTouchColor;
//						btn.SetTitleColor (UIColor.White, UIControlState.Normal);
//					}
//				};
//			}


//          if (objData.OFFDays != null && objData.OFFDays.Count > 0)
//          {
//              SetOffDays(objData.OFFDays);
//          }
//
//          if (objData.WorkDays != null && objData.WorkDays.Count > 0)
//          {
//              SetWorkDays(objData.WorkDays);
//          }



			// get the first and last date of current month;
            //DateTime nexMonth = currentTime.AddMonths(1);
            //var startDate = new DateTime(nexMonth.Year, nexMonth.Month, 1);
            //var endDate = startDate.AddMonths(1).AddDays(-1);
            //int currentDateOfWeek = (int)startDate.Date.DayOfWeek;
            //for (int i = 0; i < endDate.Day; i++) {
            //    CalendarButton btn = _buttons.ElementAt (currentDateOfWeek + i);
            //    btn.BackgroundColor = _labelColor;
            //    btn.SetTitle ((i + 1).ToString (), UIControlState.Normal);
            //    btn.ID = i + 1;
            //    btn.TouchUpInside += (object sender, EventArgs e) => {
            //        CalendarButton sd = (CalendarButton)sender;
            //        sd.Tag++;
            //        if (sd.Tag%3 == 0) {
            //            // gray
            //            sd.State = 0;
            //            sd.BackgroundColor = _labelColor;
            //        }else if(sd.Tag%3 == 1){
            //            // red
            //            sd.State = 1;
            //            sd.BackgroundColor = _oneTouchColor;
            //        }else{
            //            // green
            //            sd.State = 2;
            //            sd.BackgroundColor = _twoTouchColor;
            //        }
            //    };
            //}
            //// set 3 days of the next month
            //_buttons.ElementAt (currentDateOfWeek + endDate.Day).SetTitle ("1", UIControlState.Normal);
            //_buttons.ElementAt (currentDateOfWeek + endDate.Day+1).SetTitle ("2", UIControlState.Normal);
            //_buttons.ElementAt (currentDateOfWeek + endDate.Day+2).SetTitle ("3", UIControlState.Normal);
            ////

			this.AddSubview (viewTable);


		}

        public void GenerateCommuteTimes()
        {


            try
            {
                _listCommuteTime = new ObservableCollection<ConnectTimeHelper>();
                DateTime calendarDate;
                DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;

                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.CurrentBidDetails.Month == 3 || GlobalSettings.CurrentBidDetails.Month == 2))
                {
                    startDate = startDate.AddDays(-1);
                }

                int iterationCount = (int)startDate.DayOfWeek;

                int id = 1;
                calendarDate = startDate;
                for (int count = 0; count < iterationCount; count++)
                {
                    _listCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = false, Day = null ,Id=id});
                    calendarDate = calendarDate.AddDays(1);
                    id++;
                }

                iterationCount = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Subtract(startDate).Days + 1;
                calendarDate = startDate;
                for (int count = 1; count <= iterationCount; count++)
                {
                    _listCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = true, Day = calendarDate.Day.ToString(), Date = calendarDate.Date,Id=id });
                    calendarDate = calendarDate.AddDays(1);
                    id++;
                }


                bool status = true;
                for (int count = 1; count <= 35 - iterationCount; count++)
                {
                    _listCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = status, Day = calendarDate.Day.ToString(), Date = calendarDate.Date ,Id=id});
                    calendarDate = calendarDate.AddDays(1);
                    if (status && calendarDate.Month != GlobalSettings.CurrentBidDetails.Month)
                        status = false;
                    id++;
                }

                for (int count = id; count <= 42; count++)
                {
                    _listCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = false, Day = calendarDate.Day.ToString(), Date = calendarDate.Date ,Id=id});
                    calendarDate = calendarDate.AddDays(1);
                    id++;

                }


                WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                foreach (var item in _listCommuteTime)
                {
					//var obj = wBIdStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
					CommuteTime obj;
					switch(ObjCommutePickerFromView)
					{
					case CommuteFromView.BidAutomator:	
						 obj = wBIdStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						if (obj != null)
						{
							item.Arrival = (obj.EarliestArrivel==DateTime.MinValue)?string.Empty: obj.EarliestArrivel.ToString("HH:mm").Replace(":", "");
							item.Departure = (obj.LatestDeparture==DateTime.MinValue)?string.Empty: obj.LatestDeparture.ToString("HH:mm").Replace(":", "");
						}

						break;
					case CommuteFromView.CSWConstraints:
                    case CommuteFromView.CSWCommutableAutoSort:
                    case CommuteFromView.CSWWeight:
                            obj = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						if (obj != null)
						{
							item.Arrival =(obj.EarliestArrivel==DateTime.MinValue)?string.Empty: obj.EarliestArrivel.ToString("HH:mm").Replace(":", "");
							item.Departure = (obj.LatestDeparture==DateTime.MinValue)?string.Empty:obj.LatestDeparture.ToString("HH:mm").Replace(":", "");
						}

						break;
					//case CommuteFromView.CSWWeight:
						//obj = wBIdStateContent.Weights.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						//if (obj != null)
						//{
						//	item.Arrival = (obj.EarliestArrivel==DateTime.MinValue)?string.Empty:obj.EarliestArrivel.ToString("HH:mm").Replace(":", "");
						//	item.Departure = (obj.LatestDeparture==DateTime.MinValue)?string.Empty:obj.LatestDeparture.ToString("HH:mm").Replace(":", "");
						//}

						break;
      //                  case CommuteFromView.CSWCommutabilityConstraints:
      //                  case CommuteFromView.CSWCommutabilityWeights:
      //                  case CommuteFromView.CSWCommutabilitySort:
						//obj = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						//if (obj != null)
						//{
						//	item.Arrival = (obj.EarliestArrivel==DateTime.MinValue)?string.Empty:obj.EarliestArrivel.ToString("HH:mm").Replace(":", "");
						//	item.Departure = (obj.LatestDeparture==DateTime.MinValue)?string.Empty:obj.LatestDeparture.ToString("HH:mm").Replace(":", "");
						//}

						//break;
					
					}


				}
                    
                   


               

            }
            catch (Exception ex)
            {
               
            }
        }



		/// <summary>
		/// Gets the list off days. => Red
		/// </summary>
		/// <returns>The list off days.</returns>
//		public List<int> GetListOffDays(){
//			List<int> rs = new List<int> ();
//			foreach (CommutableCalendarButton btn in _buttons) {
//				if (btn.State == 1) {
//					rs.Add (btn.ID);
//				}
//			}
//			return rs;
//		}


		/// <summary>
		///  Gets the list work days => green
		/// </summary>
		/// <returns>The list work days.</returns>
//		public List<int> GetListWorkDays()
//		{
//			List<int> rs = new List<int> ();
//			foreach (ConnectTimeView btn in _buttons) {
//				if (btn.State == 2) {
//					rs.Add (btn.ID);
//				}
//			}
//			return rs;
//		}

//        private void SetOffDays(List<int> offDays)
//        {
////            foreach (int offDay in offDays)
////            {
////                
////				CommutableCalendarButton btn = _buttons.ElementAt(offDay - 1);
////                btn.State = 1;
////                btn.BackgroundColor = _oneTouchColor;
////				btn.SetTitleColor (UIColor.White, UIControlState.Normal);
////
////            }
//
//        }

//        private void SetWorkDays(List<int> workDays)
//        {
//            foreach (int workDay in workDays)
//            {
//
//                CalendarButton btn = _buttons.ElementAt(workDay - 1);
//                btn.State =2;
//                btn.BackgroundColor = _twoTouchColor;
//				btn.SetTitleColor (UIColor.White, UIControlState.Normal);
//            }
//
//        }


//		public void ClearAll()
//		{
//			
//			// get the first and last date of current month;
//			DateTime nexMonth = DateTime.Now.AddMonths(1);
//			var startDate = new DateTime(nexMonth.Year, nexMonth.Month, 1);
//			var endDate = startDate.AddMonths(1).AddDays(-1);
//			int currentDateOfWeek = (int)startDate.Date.DayOfWeek;
//			for (int i = 0; i < endDate.Day; i++) {
//				CommutableCalendarButton btn = _buttons.ElementAt (currentDateOfWeek + i);
//				btn.State = 0;
//				btn.Tag = 0;
//				btn.BackgroundColor = UIColor.White;
//			}
//		}

        public class ConnectTimeHelper
        {
            public int Id { get; set; }

            public string Day { get; set; }

            public string Arrival { get; set; }

            public string Departure { get; set; }

            public bool IsEnabled { get; set; }

            public DateTime Date { get; set; }


        }
	}
}

