using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public class horizontalTableViewController : UITableViewController
	{
		public horizontalTableViewController () : base (UITableViewStyle.Plain)
		{
			if (this != null) {
				const double M_PI = 3.14159265358979323846;
				const float k90DegreesCounterClockwiseAngle = (float) -(90 * M_PI / 180.0);
				RectangleF frame = new RectangleF(0,0,1024,45);
				CGAffineTransform transform = CGAffineTransform.MakeRotation (k90DegreesCounterClockwiseAngle);
				this.View.Transform = transform;
				this.View.Frame = frame;

			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source
			TableView.Source = new horizontalTableViewSource ();
		}
	}
}

