using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using System.Linq;
using System.Collections.Generic;


namespace WBid.WBidiPad.iOS
{
	public partial class BADaysOfMonthCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("BADaysOfMonthCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _parentVC;
		DaysOfMonthCx _cellData;

		static BADaysOfMonthCell ()
		{
			Nib = UINib.FromName ("BADaysOfMonthCell", NSBundle.MainBundle);
		}

		public BADaysOfMonthCell (IntPtr handle) : base (handle)
		{
		}

		public static BADaysOfMonthCell Create()
		{
			return (BADaysOfMonthCell)Nib.Instantiate(null, null)[0];
		}

		void UpdateUI()
		{
//			string offDays = "";
//			if (_cellData.OFFDays == null) {
//				offDays = "off[]";
//			} else {
//				offDays = "off[";
//				for (int i = 0; i < _cellData.OFFDays.Count; i++) {
//					if (i == _cellData.OFFDays.Count - 1) {
//						offDays = offDays + _cellData.OFFDays[i]; // last element
//					} else {
//						offDays = offDays + _cellData.OFFDays[i] + ",";
//					}
//				}
//				offDays = offDays + "]";
//			}
//
//
//			string workDays = "work[";
//			if (_cellData.WorkDays == null) {
//				workDays = "work[]";
//			} else {
//				for (int i = 0; i < _cellData.WorkDays.Count; i++) {
//					if (i == _cellData.WorkDays.Count - 1) {
//						workDays = workDays + _cellData.WorkDays[i]; // last element
//					} else {
//						workDays = workDays + _cellData.WorkDays[i] + ",";
//					}
//				}	
//				workDays = workDays + "]";
//			}
			List<DaysOfMonth> dayOfMonthList = WBidCollection.GetDaysOfMonthList();

			string offDays = string.Empty;
			if (_cellData.OFFDays != null)
			{
				foreach (int offDayId in _cellData.OFFDays)
				{
					if (offDays != string.Empty)
						offDays += ",";

					offDays += dayOfMonthList.FirstOrDefault(x => x.Id == offDayId).Day;

				}

			}

			offDays = "OFF[" + offDays + "]";

			string workDays = string.Empty;
			if (_cellData.WorkDays != null)
			{
				foreach (int workDayId in _cellData.WorkDays)
				{
					if (workDays != string.Empty)
						workDays += ",";

					workDays += dayOfMonthList.FirstOrDefault(x => x.Id == workDayId).Day;

				}

			}
			workDays = "FLY[" + workDays + "]";
			lbDayOfMonth.Text = string.Format ("{0} {1}",offDays, workDays);
		}

		public void Filldata (ConstraintsChangeViewController constraintsChangeViewController, DaysOfMonthCx daysOfMonthCx)
		{
			_parentVC = constraintsChangeViewController;
			_cellData = daysOfMonthCx;
			UpdateUI ();
		}

		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if (_parentVC != null) {
				_parentVC.DeleteObject(_cellData);
			}
		}

		partial void OnOpenPickerEvent (Foundation.NSObject sender){
			BAConstraintDaysMonthViewController viewController= new BAConstraintDaysMonthViewController();
			viewController.data = _cellData;
			viewController.ObjChangeController = _parentVC;
			viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			_parentVC.NavigationController.PresentViewController (viewController, true, null);

//			ConstraintDaysMonthViewController viewController = _parentVC.Storyboard.InstantiateViewController("ConstraintDaysMonthViewController") as ConstraintDaysMonthViewController;
//			viewController.data = _cellData;
//			viewController.ObjChangeController = _parentVC;
//			viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//			_parentVC.NavigationController.PresentViewController (viewController, true, null);
		}
	}
}
