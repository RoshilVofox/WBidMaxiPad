using System;

using UIKit;

namespace WBid.WBidiPad.iOS
{
	public partial class TimeOutAlertView : UIViewController
	{
		public UIPopoverController objpopover;
		public RetrieveAwardViewController objRetriveAwards;
		public SubmitBidViewController objSubmitBid;
		public downloadBidDataViewController ObjDownload;
		public queryViewController objQueryView;
		public TimeOutAlertView() : base("TimeOutAlertView", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}
		partial void funCloseView(Foundation.NSObject sender)
		{
			objpopover.Dismiss(true);

			if (objRetriveAwards != null)
			{
				objRetriveAwards.DismissView();
			}
			else if (ObjDownload != null)
			{
				ObjDownload.DismissCurrentView();
			}
			else if (objQueryView != null)
			{
				objQueryView.dismissView();
			}
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

