
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class BAGroupCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("BAGroupCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BAGroupCell");

		public BAGroupCell (IntPtr handle) : base (handle)
		{
		}

		public static BAGroupCell Create ()
		{

			return (BAGroupCell)Nib.Instantiate (null, null) [0];

		}
		public void SetData(string Header,string Details)
		{
			lblDetails.Font = UIFont.FromName("Courier",13);
			lblDetails.Lines = 0;
			lblDetails.Text = Details;
			lblHeaders.Text = Header;
		}

	}
}

