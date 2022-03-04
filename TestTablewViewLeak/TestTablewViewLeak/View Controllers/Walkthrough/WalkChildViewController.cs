using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class WalkChildViewController : UIViewController
	{
		public WalkChildViewController () : base ("WalkChildViewController", null)
		{
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
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			UILabel lblView = new UILabel ();
			lblView.Frame = new CGRect (0, 0, 100, 30);
			lblView.Center = this.View.Center;
			lblView.Text = "Child View " + this.View.Tag;
			this.View.AddSubview (lblView);
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();

			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			//this.View.Dispose ();


		}
	}
}

