using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class SmallPopoverTableCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("SmallPopoverTableCell");

		public SmallPopoverTableCell () : base (UITableViewCellStyle.Value1, Key)
		{
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
				TextLabel.TextColor = UIColor.Black;
			}
		}

		public void updateColorForText2(string text)
		{
			if (text == "more") {
				TextLabel.TextColor = UIColor.Green;
			} else if (text == "equal") {
				TextLabel.TextColor = UIColor.Blue;
			} else if (text == "not equal") {
				TextLabel.TextColor = UIColor.Orange;
			} else if (text == "less") {
				TextLabel.TextColor = UIColor.Red;
			} else {
				TextLabel.TextColor = UIColor.Black;
			}
		}


	}
}

