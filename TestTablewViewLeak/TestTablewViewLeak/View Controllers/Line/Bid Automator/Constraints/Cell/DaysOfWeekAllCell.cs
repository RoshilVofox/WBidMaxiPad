using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public partial class DaysOfWeekAllCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("DaysOfWeekAllCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		DaysOfWeekAll _cellObject;

		static DaysOfWeekAllCell ()
		{
			Nib = UINib.FromName ("DaysOfWeekAllCell", NSBundle.MainBundle);
		}

		public DaysOfWeekAllCell (IntPtr handle) : base (handle)
		{
		}

		public void Filldata(ConstraintsChangeViewController parentVC, DaysOfWeekAll cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
			UpdateUI ();
		}

		public static DaysOfWeekAllCell Create()
		{
			return (DaysOfWeekAllCell)Nib.Instantiate(null, null)[0];
		}

		partial void OnDeleteEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnFriButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Fr = !_cellObject.Fr;
				UpdateUI();
			}	
		}

		partial void OnMonButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Mo = !_cellObject.Mo;
				UpdateUI();
			}	
		}

		partial void OnSatButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Sa = !_cellObject.Sa;
				UpdateUI();
			}
		}

		partial void OnSunButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Su = !_cellObject.Su;
				UpdateUI();
			}
		}


		partial void OnThuButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Th= !_cellObject.Th;
				UpdateUI();
			}
		}

		partial void OnTueButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.Tu = !_cellObject.Tu;
				UpdateUI();
			}
		}
		partial void OnWedButtonEvent (Foundation.NSObject sender){
			if (_cellObject != null) {
				_cellObject.We = !_cellObject.We;
				UpdateUI();
			}
		}
		private void UpdateUI (){
			lbDayOfWeekAll.Text = "DOW - All";
			if (_cellObject.Su) {
				btnSunday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSunday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Mo) {
				btnMonday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnMonday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Tu) {
				btnTuesday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnTuesday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.We) {
				btnWednesday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnWednesday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Th) {
				btnThursday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnThursday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Fr) {
				btnFriday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnFriday.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Sa) {
				btnSaturday.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSaturday.BackgroundColor = Colors.BidOrange;
			}
		}
	}
}
