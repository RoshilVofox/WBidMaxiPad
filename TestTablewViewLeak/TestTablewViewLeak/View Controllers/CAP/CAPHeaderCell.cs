
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class CAPHeaderCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CAPHeaderCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CAPHeaderCell");

		public CAPHeaderCell (IntPtr handle) : base (handle)
		{
		}

		public static CAPHeaderCell Create ()
		{
			return (CAPHeaderCell)Nib.Instantiate (null, null) [0];
		}
	}
}

