
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class CAPDetailCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CAPDetailCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CAPDetailCell");

		public CAPDetailCell (IntPtr handle) : base (handle)
		{
		}

		public static CAPDetailCell Create ()
		{
			return (CAPDetailCell)Nib.Instantiate (null, null) [0];
		}

		public void LabelValues(string Domicile,string Position,string CurrMonth,string NextMonth)
		{
			lblBase.Text = Domicile;
			lblCurrMonth.Text = CurrMonth;
			lblNextMon.Text = NextMonth;
			lblSeat.Text = Position;
		}
	}
}

