
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class CAPCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CAPCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CAPCell");

		public CAPCell (IntPtr handle) : base (handle)
		{
		}

		public static CAPCell Create ()
		{
			return (CAPCell)Nib.Instantiate (null, null) [0];
		}
	}
}

