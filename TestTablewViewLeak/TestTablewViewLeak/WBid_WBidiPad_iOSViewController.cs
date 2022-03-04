using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;

namespace WBid.WBidiPad.iOS
{
	public partial class WBid_WBidiPad_iOSViewController : UIViewController
	{
		public WBid_WBidiPad_iOSViewController () : base ("WBid_WBidiPad_iOSViewController", null)
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

            if (!Reachability.IsHostReachable("google.com"))
            {
                UIAlertController okAlertController = UIAlertController.Create("Error", "No Internet", UIAlertControllerStyle.Alert);                 okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                 this.PresentViewController(okAlertController, true, null);
            }
            else
            {
               
                UIAlertController okAlertController = UIAlertController.Create("Error", "Yes Internet", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }


			// Perform any additional setup after loading the view, typically from a nib.
		}

       
	}
}

