using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
namespace WBid.WBidiPad.iOS
{
	public partial class AMPMCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("AMPMCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _parentVC;
		AMPMConstriants _cellData;
		static AMPMCell ()
		{
			Nib = UINib.FromName ("AMPMCell", NSBundle.MainBundle);
		}

		public AMPMCell (IntPtr handle) : base (handle)
		{
		}

		public static AMPMCell Create()
		{
			return (AMPMCell)Nib.Instantiate(null, null)[0];
		}

//		public override void LayoutSubviews ()
//		{
//			// set default view
//
//			btnAm.TouchUpInside -= BtnAm_TouchUpInside;
//			btnAm.TouchUpInside += BtnAm_TouchUpInside;
//
//			btnPm.TouchUpInside -= BtnPm_TouchUpInside;
//			btnPm.TouchUpInside += BtnPm_TouchUpInside;
//
//			btnMix.TouchUpInside -= BtnMix_TouchUpInside;
//			btnMix.TouchUpInside += BtnMix_TouchUpInside;
//
//		}

		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if (_parentVC != null) {
				_parentVC.DeleteObject(_cellData);
			}
		}

		partial void OnAmEvent (Foundation.NSObject sender){
			if (_cellData != null) {
				_cellData.AM = !_cellData.AM;
				UpdateUI();
			}
		}

		partial void OnMixEvent (Foundation.NSObject sender){
			if (_cellData != null) {
				_cellData.MIX = !_cellData.MIX;
				UpdateUI();
			}	
		}

		partial void OnPmEvent (Foundation.NSObject sender)
		{
			if (_cellData != null) {
				_cellData.PM = !_cellData.PM;
				UpdateUI();
			}
		}
		public void Filldata(ConstraintsChangeViewController parentVC, AMPMConstriants cellData)
		{
			_parentVC = parentVC;
			_cellData = cellData;
			lbName.Text = "Am - Pm";
//			_cellData.AM = false;
//			_cellData.PM = false;
//			_cellData.MIX = false;
			UpdateUI ();
		}

		void UpdateUI()
		{
			if (_cellData.AM) {
				btnAm.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnAm.BackgroundColor = Colors.BidOrange;
			}
			if (_cellData.PM) {
				btnPm.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnPm.BackgroundColor = Colors.BidOrange;
			}
			if (_cellData.MIX) {
				btnMix.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnMix.BackgroundColor = Colors.BidOrange;
			}
		}

	}
}
