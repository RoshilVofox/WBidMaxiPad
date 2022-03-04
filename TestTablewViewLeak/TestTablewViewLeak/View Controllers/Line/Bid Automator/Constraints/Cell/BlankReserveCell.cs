using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.iOS
{
	public partial class BlankReserveCell : UITableViewCell
	{
		
		public static readonly NSString Key = new NSString ("BlankReserveCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _parentVC;
		BlankReserveConstraint _cellData;
		static BlankReserveCell ()
		{
			Nib = UINib.FromName ("BlankReserveCell", NSBundle.MainBundle);
		}

		public BlankReserveCell (IntPtr handle) : base (handle)
		{
		}

		public static BlankReserveCell Create()
		{
			return (BlankReserveCell)Nib.Instantiate(null, null)[0];
		}


		public void Filldata (ConstraintsChangeViewController constraintsChangeViewController, BlankReserveConstraint blankReserveConstraint)
		{
			_cellData = blankReserveConstraint;
			_parentVC = constraintsChangeViewController;
			UpdateUI ();
		}

		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if (_parentVC != null) {
				_parentVC.DeleteObject(_cellData);
			}
		}
		partial void OnReserveBeforeBlankEvent (Foundation.NSObject sender)
		{
			_cellData.IsBlank = false;
			UpdateUI();
		}

		partial void OnBlankBeforeReserveEvent (Foundation.NSObject sender)
		{
			_cellData.IsBlank = true;
			UpdateUI();
		}

		void UpdateUI()
		{
			UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{btnBlankBeforeReserve, btnReserveBeforeBlank});
			if (_cellData.IsBlank) {
				btnBlankBeforeReserve.BackgroundColor = Colors.BidDarkGreen;
				btnReserveBeforeBlank.BackgroundColor = UIColor.White;
				btnBlankBeforeReserve.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnReserveBeforeBlank.SetTitleColor (UIColor.Black, UIControlState.Normal);	
			} else {
				btnReserveBeforeBlank.BackgroundColor = Colors.BidDarkGreen;
				btnBlankBeforeReserve.BackgroundColor = UIColor.White;
				btnReserveBeforeBlank.SetTitleColor(UIColor.White, UIControlState.Normal);
				btnBlankBeforeReserve.SetTitleColor(UIColor.Black, UIControlState.Normal);
			}
		}
	}
}
