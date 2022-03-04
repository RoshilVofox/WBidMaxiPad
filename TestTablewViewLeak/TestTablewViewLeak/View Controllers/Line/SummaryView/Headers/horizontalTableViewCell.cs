using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public class horizontalTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("horizontalTableViewCell");

		public horizontalTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			if (this != null) {
				const double M_PI = 3.14159265358979323846;
				const float k90DegreesCounterClockwiseAngle = (float) (90 * M_PI / 180.0);
				RectangleF frame = new RectangleF(0,0,0,0);
				CGAffineTransform transform = CGAffineTransform.MakeRotation (k90DegreesCounterClockwiseAngle);
				this.Transform = transform;
				this.Frame = frame;
			}
		}
	}
}

