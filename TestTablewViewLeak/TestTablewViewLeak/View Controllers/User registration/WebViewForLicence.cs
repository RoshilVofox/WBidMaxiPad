
using System;

using Foundation;
using UIKit;
using System.IO;

namespace WBid.WBidiPad.iOS
{
	public partial class WebViewForLicence : UIViewController
	{
		public bool isLicence;
		public WebViewForLicence () : base ("WebViewForLicence", null)
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

			if (isLicence) {
				
				string localDocUrl = Path.Combine (NSBundle.MainBundle.ResourcePath, "WBid License Agreement.pdf");
				WebView.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
				lblTitle.Text = "License Agreement";
			} else {
				string localDocUrl = Path.Combine (NSBundle.MainBundle.ResourcePath, "WBid Personal Information and Privacy Policy.pdf");
				WebView.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
				lblTitle.Text = "Privacy Policy";
			}
			// Perform any additional setup after loading the view, typically from a nib.
		}
		partial void btnDoneClicked (NSObject sender)
		{
			this.DismissViewController(true,null);
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();
			this.View.Dispose ();


		}

	}
}

