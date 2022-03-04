using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Linq;


namespace WBid.WBidiPad.iOS
{
	public partial class CityCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("CityCell");
		public static readonly UINib Nib;

		static CityCell ()
		{
			Nib = UINib.FromName ("CityCell", NSBundle.MainBundle);
		}
		OvernightCitiesCx _data;
		public CityCell (IntPtr handle) : base (handle)
		{
		}
		string _city;
		public void FillData(string city, OvernightCitiesCx data)
		{
			_data = data;
			_city = city;
			UpdateUI ();
		}

		partial void OnCellClicked (Foundation.NSObject sender)
		{
			btnCount.Tag ++;
			if (_data == null) {
				_data = new OvernightCitiesCx();
			}
			if (btnCount.Tag%3 == 0) {
				// remove
				if (_data != null && _data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}

				if (_data != null && _data.Yes != null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
			}
			if (btnCount.Tag%3 == 1) {
				if (_data.Yes!= null && !_data.Yes.Contains(_city)) {
					_data.Yes.Add(_city);
				}
				if (_data.Yes == null) {
					_data.Yes = new List<string>();
					_data.Yes.Add(_city);
				}
				if (_data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}
			}
			if (btnCount.Tag %3  == 2) {
				// no
				if (_data.Yes!= null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
				if (_data.No != null && !_data.No.Contains(_city)) {
					_data.No.Add(_city);
				}
				if (_data.No == null) {
					_data.No = new List<string>();
					_data.No.Add(_city);
				}
			}
			UpdateUI();
		}

		void UpdateUI()
		{
			lbTitle.Text = _city;


			if (GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Name == _city) == null)
			{
				BackgroundColor = UIColor.Black;
				btnCount.Tag = 0;
				lbTitle.TextColor = UIColor.White;
			}


			else if (_data == null) {
				BackgroundColor = UIColor.White;
				lbTitle.TextColor = UIColor.Black;
				btnCount.Tag = 0;// green=> 1touch/YES, red => 2touch/NO
			} else {
				bool isFound = false;
				if (_data.No != null && _data.No.Contains(_city)) {
					btnCount.Tag = 2;
					BackgroundColor = Colors.BidRed;
					lbTitle.TextColor = UIColor.White;
					isFound = true;
				}
				if (_data.Yes != null && _data.Yes.Contains(_city)) {
					btnCount.Tag = 1;
					BackgroundColor = Colors.BidRowGreen;
					lbTitle.TextColor = UIColor.White;
					isFound = true;
				}
				if (!isFound) {
					BackgroundColor = UIColor.White;
					lbTitle.TextColor = UIColor.Black;
					btnCount.Tag = 0;// green=> 1touch/YES, red => 2touch/NO
				}
			}
		}
	}
}
