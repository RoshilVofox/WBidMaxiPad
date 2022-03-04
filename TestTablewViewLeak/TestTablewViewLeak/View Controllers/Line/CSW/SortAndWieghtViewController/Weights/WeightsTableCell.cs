using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class WeightsTableCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("WeightsTableCell");

		public WeightsTableCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

