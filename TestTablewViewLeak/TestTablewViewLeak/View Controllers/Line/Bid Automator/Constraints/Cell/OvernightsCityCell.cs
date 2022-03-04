using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
namespace WBid.WBidiPad.iOS
{
	public partial class OvernightsCityCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("OvernightsCityCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		OvernightCitiesCx _cellObject;

		static OvernightsCityCell ()
		{
			Nib = UINib.FromName ("OvernightsCityCell", NSBundle.MainBundle);
		}

		public OvernightsCityCell (IntPtr handle) : base (handle)
		{
		}
		public static OvernightsCityCell Create()
		{
			return (OvernightsCityCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, OvernightCitiesCx cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
			UpdateUI ();
		}
		private void UpdateUI (){
			if (_cellObject == null) {
				lbOvernightCityValue.Text ="yes[] no[]";
				return;
			}
			string yesStr = "yes[";
			if (_cellObject.Yes == null) {
				yesStr = "yes[]";
			} else {
				for (int i = 0; i < _cellObject.Yes.Count; i++) {
					if (i == _cellObject.Yes.Count - 1) {
						// last element
						yesStr = yesStr + _cellObject.Yes [i]+ "]";
					} else {
						yesStr = yesStr + _cellObject.Yes [i]+ ",";
					}
				}
			}

			string noStr = "no[";
			if (_cellObject.No == null) {
				noStr = "no[]";
			} else {
				for (int i = 0; i < _cellObject.No.Count; i++) {
					if (i == _cellObject.No.Count - 1) {
						// last element
						noStr = noStr + _cellObject.No [i]+ "]";
					} else {
						noStr = noStr + _cellObject.No [i]+ ",";
					}
				}
			}
			lbOvernightCityValue.Text = string.Format ("{0} {1}", yesStr, noStr);
		}

		partial void OnDeleteEvent (UIButton sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnTouchEvent (Foundation.NSObject sender)
		{
			CitiesOvernightsViewController viewController = _viewController.Storyboard.InstantiateViewController("CitiesOvernightsViewController") as CitiesOvernightsViewController;
			viewController.data = _cellObject;
			viewController.ObjChangeContraints=_viewController;
			viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			_viewController.PresentViewController (viewController, true, null);
		}
	}
}
