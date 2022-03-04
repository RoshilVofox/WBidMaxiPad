
using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class BrightnessViewController : UIViewController
	{
		public BrightnessViewController () : base ("BrightnessViewController", null)
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
			sldrBrightness.MinValue = 0.0f;
			sldrBrightness.MaxValue = 0.9f;
			sldrBrightness.Value = (float)UIScreen.MainScreen.Brightness;
		}

		partial void sldrBrightnessChanged (UIKit.UISlider sender)
		{
			UIScreen.MainScreen.Brightness = sender.Value;
		}
	}
}

