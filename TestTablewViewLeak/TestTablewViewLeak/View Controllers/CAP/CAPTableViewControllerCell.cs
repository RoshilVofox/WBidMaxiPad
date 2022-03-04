using System;
using Foundation;
using UIKit;


namespace WBid.WBidiPad.iOS
{
	public class CAPTableViewControllerCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("CAPTableViewControllerCell");

		public CAPTableViewControllerCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

