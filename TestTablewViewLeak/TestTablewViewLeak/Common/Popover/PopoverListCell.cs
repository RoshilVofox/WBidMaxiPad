using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class PopoverListCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("PopoverListCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("PopoverListCell");

		public PopoverListCell (IntPtr handle) : base (handle)
		{
		}

		public static PopoverListCell Create ()
		{
			return (PopoverListCell)Nib.Instantiate (null, null) [0];
		}

		public void bindData(string text, NSIndexPath indextpath)
		{
			this.TextLabel.Text = text;
			this.TextLabel.Font = UIFont.SystemFontOfSize (15);
		}
		public void updateColorForText(string text)
		{
			if (text == "more than") {
				TextLabel.TextColor = UIColor.Green;
			} else if (text == "equal to") {
				TextLabel.TextColor = UIColor.Blue;
			} else if (text == "not equal") {
				TextLabel.TextColor = UIColor.Orange;
			} else if (text == "less than") {
				TextLabel.TextColor = UIColor.Red;
			} else {
				return;
			}
		}
	}
}

