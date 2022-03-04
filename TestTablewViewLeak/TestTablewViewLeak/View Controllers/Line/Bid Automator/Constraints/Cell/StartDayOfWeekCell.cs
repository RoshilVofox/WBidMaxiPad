using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public partial class StartDayOfWeekCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("StartDayOfWeekCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		StartDayOfWeek _cellObject;

		static StartDayOfWeekCell ()
		{
			Nib = UINib.FromName ("StartDayOfWeekCell", NSBundle.MainBundle);
		}

		public StartDayOfWeekCell (IntPtr handle) : base (handle)
		{
		}
		public static StartDayOfWeekCell Create()
		{
			return (StartDayOfWeekCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, StartDayOfWeek cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
			lbStartDayOfWeek.Text = "SDOW"; 
			UpdateUI ();
		}

		partial void OnDeleteEvent (UIButton sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnSunButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Su = !_cellObject.Su;
				_cellObject.Mo=false;
				_cellObject.Tu=false;
				_cellObject.We=false;
				_cellObject.Th=false;
				_cellObject.Fr=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnMonButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Mo = !_cellObject.Mo;
				_cellObject.Su=false;
				_cellObject.Tu=false;
				_cellObject.We=false;
				_cellObject.Th=false;
				_cellObject.Fr=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnTueButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Tu = !_cellObject.Tu;
				_cellObject.Su=false;
				_cellObject.Mo=false;
				_cellObject.We=false;
				_cellObject.Th=false;
				_cellObject.Fr=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnWedButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.We = !_cellObject.We;
				_cellObject.Su=false;
				_cellObject.Tu=false;
				_cellObject.Mo=false;
				_cellObject.Th=false;
				_cellObject.Fr=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnThuButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Th = !_cellObject.Th;
				_cellObject.Su=false;
				_cellObject.Tu=false;
				_cellObject.We=false;
				_cellObject.Mo=false;
				_cellObject.Fr=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnFriButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Fr = !_cellObject.Fr;
				_cellObject.Su=false;
				_cellObject.Tu=false;
				_cellObject.We=false;
				_cellObject.Th=false;
				_cellObject.Mo=false;
				_cellObject.Sa=false;
				UpdateUI();
			}
		}

		partial void OnSatButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Sa = !_cellObject.Sa;
				_cellObject.Su=false;
				_cellObject.Tu=false;
				_cellObject.We=false;
				_cellObject.Th=false;
				_cellObject.Fr=false;
				_cellObject.Mo=false;
				UpdateUI();
			}
		}

		private void UpdateUI (){
			if (_cellObject.Su) {
				btnSun.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSun.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Mo) {
				btnMon.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnMon.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Tu) {
				btnTue.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnTue.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.We) {
				btnWed.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnWed.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Th) {
				btnThu.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnThu.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Fr) {
				btnFri.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnFri.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Sa) {
				btnSat.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSat.BackgroundColor = Colors.BidOrange;
			}
		}
	}
}
