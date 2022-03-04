
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class SynchConflictViewController : UIViewController
	{
		public DateTime serverSynchTime;
		public bool noServer;

		public SynchConflictViewController () : base ("SynchConflictViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);



			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			btnCancel.Clicked += (object sender, EventArgs e) => {
				this.DismissViewController(true,()=>{
					NSNotificationCenter.DefaultCenter.PostNotificationName("SyncConflict",new NSString(string.Empty));
				});
			};
			btnTakeServer.TouchUpInside += btnTakeServerTapped;
			btnTakeLocal.TouchUpInside += btnTakeLocalTapped;

			if (serverSynchTime != DateTime.MinValue)
			{
				var serverTimeCST = TimeZoneInfo.ConvertTimeFromUtc (serverSynchTime, TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));

				if (noServer)
					btnTakeServer.Enabled = false;
				else {
					btnTakeServer.Enabled = true;
					lblServerDate.Text = serverTimeCST.ToShortDateString ();
					lblServerTime.Text = serverTimeCST.ToString ("hh:mm:ss tt");
				}
			}
			if (GlobalSettings.WBidStateCollection.StateUpdatedTime != DateTime.MinValue)
			{
				var localTimeCST = DateTime.MinValue;
				try {
					localTimeCST = TimeZoneInfo.ConvertTimeFromUtc (GlobalSettings.WBidStateCollection.StateUpdatedTime, TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));
				} catch {
					localTimeCST = TimeZoneInfo.ConvertTimeFromUtc (GlobalSettings.WBidStateCollection.StateUpdatedTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));
				}

				lblLocalDate.Text = localTimeCST.ToShortDateString();
				lblLocalTime.Text = localTimeCST.ToString ("hh:mm:ss tt");
			}
		}

		void btnTakeLocalTapped (object sender, EventArgs e)
		{
			this.DismissViewController(true,()=>{
				NSNotificationCenter.DefaultCenter.PostNotificationName("SyncConflict",new NSString("local"));
			});
		}

		void btnTakeServerTapped (object sender, EventArgs e)
		{
			this.DismissViewController(true,()=>{
				NSNotificationCenter.DefaultCenter.PostNotificationName("SyncConflict",new NSString("server"));
			});
		}
	}
}

