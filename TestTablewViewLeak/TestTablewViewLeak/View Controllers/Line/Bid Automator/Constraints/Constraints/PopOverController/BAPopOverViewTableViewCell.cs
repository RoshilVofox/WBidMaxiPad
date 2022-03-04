
using System;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class BAPopOverViewTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("BAPopOverViewTableViewCell");

		public BAPopOverViewTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}

		public void SetCell(string Value)
		{
			TextLabel.Text=Value;
		}
	}
}

