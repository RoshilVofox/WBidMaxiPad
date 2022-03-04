using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public class TripPopListViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("TripPopListViewCell");

		public TripPopListViewCell () : base (UITableViewCellStyle.Value1, Key)
		{

		}
	}
}

