using System;
using UIKit;
using Foundation;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core;


namespace WBid.WBidiPad.iOS
{
	[Register("PickerView")]
	public class PickerView: UIView
	{


		int _numberItemPerRow = 7;
		int _numberRows = 6;
		nfloat _itemSize = 40;

		UIColor _labelColor;
		UIColor _oneTouchColor;
		UIColor _twoTouchColor;
		List<CalendarButton> _buttons;
		DaysOfMonthCx objData;
		public PickerView (UIColor lb, UIColor one, UIColor two,DaysOfMonthCx data)
		{
			_labelColor = lb;
			_oneTouchColor = one;
			_twoTouchColor = two;
			objData = data;
		}
		public PickerView(IntPtr handle) : base(handle) {

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
				lb.Frame = new CoreGraphics.CGRect (i * _itemSize, _itemSize +20, _itemSize , _itemSize -10);
				lb.Text = Constants.DaysInWeek.ElementAt (i);
				lb.TextAlignment = UITextAlignment.Center;
				lb.ClipsToBounds = true;
				//lb.Layer.CornerRadius = _itemSize / 2;
				lb.BackgroundColor = UIColor.LightGray;
				lb.Layer.BorderWidth = 0.6f;
				lb.Layer.BorderColor = UIColor.LightGray.CGColor;
				this.AddSubview (lb);
			}
			// add month
			UILabel lbMonth = new UILabel();
			lbMonth.Frame = new CoreGraphics.CGRect (0, 15, this.Bounds.Width, 70);
			lbMonth.TextAlignment = UITextAlignment.Center;
			lbMonth.TextColor = UIColor.Black;
			lbMonth.Font = UIFont.BoldSystemFontOfSize (22);
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			string[] monthNames = 
				System.Globalization.CultureInfo.CurrentCulture
					.DateTimeFormat.MonthGenitiveNames;

			lbMonth.Text = string.Format ("{0} {1}", monthNames[currentTime.Month], currentTime.Year);
			this.AddSubview (lbMonth);
			// draw the table
			UIView viewTable = new UIView();
			viewTable.BackgroundColor = UIColor.Clear;
			viewTable.Frame = new CoreGraphics.CGRect (0, _itemSize + 70, this.Bounds.Width, _itemSize * _numberRows);
			_buttons = new List<CalendarButton> ();

			for (int row = 0; row < _numberRows; row++) {
				for (int col = 0; col < _numberItemPerRow; col++) {
					CalendarButton btn = new CalendarButton ();
					btn.Frame = new CoreGraphics.CGRect (col * _itemSize, row *( _itemSize-17), _itemSize , _itemSize-17);
					btn.BackgroundColor = UIColor.White; // default color;
					btn.Layer.BorderColor = UIColor.DarkGray.CGColor;
					btn.Layer.BorderWidth = 0.6f;
					//btn.Layer.CornerRadius = _itemSize / 2;
					btn.Tag = 0;
					btn.Font = UIFont.BoldSystemFontOfSize(13);
					btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
					_buttons.Add (btn);
					viewTable.AddSubview (btn);
				}
			}



			List<DaysOfMonth>  dayOfMonthList = WBidCollection.GetDaysOfMonthList();

			foreach (var day in dayOfMonthList)
			{
				CalendarButton btn = _buttons.ElementAt(day.Id-1);
				//btn.BackgroundColor = UIColor.White;
				if(day.Day!=string.Empty)
				{
					btn.SetTitle (day.Day, UIControlState.Normal);
				}

				btn.Enabled=day.IsEnabled;
				btn.ID =day.Id;
				btn.Date = day.Date;
				if (btn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) || btn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) || btn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3))
				{
					btn.BackgroundColor = UIColor.LightGray;
				}
				else
				{
					btn.BackgroundColor = UIColor.White;
				}
				btn.TouchUpInside += (object sender, EventArgs e) => {
					CalendarButton sd = (CalendarButton)sender;
					sd.Tag++;
					if (sd.Tag%3 == 0) {
						// gray
						sd.State = 0;
						if (sd.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) || sd.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) || sd.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3))
						{
							sd.BackgroundColor = UIColor.LightGray;

						}
						else
						{
							sd.BackgroundColor = UIColor.White;
						}
						btn.SetTitleColor (UIColor.Black, UIControlState.Normal);
					}else if(sd.Tag%3 == 1){
						// red
						sd.State = 1;
						sd.BackgroundColor = _oneTouchColor;
						btn.SetTitleColor (UIColor.White, UIControlState.Normal);
					}else{
						// green
						sd.State = 2;
						sd.BackgroundColor = _twoTouchColor;
						btn.SetTitleColor (UIColor.White, UIControlState.Normal);
					}
				};
			}


			if (objData.OFFDays != null && objData.OFFDays.Count > 0)
			{
				SetOffDays(objData.OFFDays);
			}

			if (objData.WorkDays != null && objData.WorkDays.Count > 0)
			{
				SetWorkDays(objData.WorkDays);
			}



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

		/// <summary>
		/// Gets the list off days. => Red
		/// </summary>
		/// <returns>The list off days.</returns>
		public List<int> GetListOffDays(){
			List<int> rs = new List<int> ();
			foreach (CalendarButton btn in _buttons) {
				if (btn.State == 1) {
					rs.Add (btn.ID);
				}
			}
			return rs;
		}


		/// <summary>
		///  Gets the list work days => green
		/// </summary>
		/// <returns>The list work days.</returns>
		public List<int> GetListWorkDays()
		{
			List<int> rs = new List<int> ();
			foreach (CalendarButton btn in _buttons) {
				if (btn.State == 2) {
					rs.Add (btn.ID);
				}
			}
			return rs;
		}

		private void SetOffDays(List<int> offDays)
		{
			foreach (int offDay in offDays)
			{

				CalendarButton btn = _buttons.ElementAt(offDay - 1);
				btn.State = 1;
				btn.BackgroundColor = _oneTouchColor;
				btn.SetTitleColor (UIColor.White, UIControlState.Normal);

			}

		}

		private void SetWorkDays(List<int> workDays)
		{
			foreach (int workDay in workDays)
			{

				CalendarButton btn = _buttons.ElementAt(workDay - 1);
				btn.State =2;
				btn.BackgroundColor = _twoTouchColor;
				btn.SetTitleColor (UIColor.White, UIControlState.Normal);
			}

		}


		public void ClearAll()
		{

			foreach (CalendarButton calBtn in _buttons)
			{
				if (calBtn.State == 1 || calBtn.State == 2)
				{
					calBtn.State = 0;
					if (calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) || calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) || calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3))
					{
						calBtn.BackgroundColor = UIColor.LightGray; ;
					}
					else
					{
						calBtn.BackgroundColor = UIColor.White; ;
					}
					calBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
					calBtn.Tag = 0;
				}

			}
		}
	}
}

