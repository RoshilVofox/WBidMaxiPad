
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintModalCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("ConstraintModalCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ConstraintModalCell");

		public ConstraintModalCell (IntPtr handle) : base (handle)
		{
		}

		public static ConstraintModalCell Create ()
		{
			return (ConstraintModalCell)Nib.Instantiate (null, null) [0];
		}

	
		public void FillData (int index, bool isChecked)
		{
			lbTitle.Text =" " + Constants.listConstraints[index];
			this.Accessory = UITableViewCellAccessory.None;
			this.TintColor = Colors.BidDarkGreen;
			if (isChecked) {
//				imgCheck.Hidden = false;
                if (lbTitle.Text == "Commutable Lines - Manual" || lbTitle.Text == "Rest" || lbTitle.Text=="Commutable Lines")
					this.Accessory = UITableViewCellAccessory.Checkmark;
				
				else
					this.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
			}
			if (index == 1 || index == 2 || index == 9 || index == 10) {
				vContent.BackgroundColor = UIColor.White;
			}
		}
	}
}

