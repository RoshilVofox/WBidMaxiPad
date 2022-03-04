
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class WalkthroughViewController : UIViewController
	{
		public homeViewController home;
		public WalkthroughViewController () : base ("WalkthroughViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			this.DangerousRelease ();
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//txtCellNumber.Dispose ();

			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// example of 5 page scrollable walkthrough view. each view is controlled by WalkChildViewController with view tag. 
			int count = 32;
			Console.WriteLine(this.scrlWalkthr.Frame.Size.Width);
			this.pgCtrlWalkthr.Pages = count;
			nfloat siz = this.View.Frame.Size.Width;
			nfloat hhh = siz;

			this.scrlWalkthr.ContentSize = new CGSize (768 * count, 0);
			for (int i = 0; i < count; i++) {
//				UIView vwWalkthr = new UIView ();
//				vwWalkthr.Frame = new RectangleF (50 + i * this.scrlWalkthr.Frame.Size.Width, 100, 668, 500);
//				WalkChildViewController vwContWalkthr = new WalkChildViewController ();
//				vwContWalkthr.View.Frame = new RectangleF (0, 0, 668, 500);
//				vwContWalkthr.View.BackgroundColor = UIColor.LightGray;
//				vwContWalkthr.View.Tag = i;
//				vwWalkthr.AddSubview (vwContWalkthr.View);
//				this.scrlWalkthr.AddSubview (vwWalkthr);

				UIImageView imgWalkthr = new UIImageView ();

				int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

				if (SystemVersion == 13)
				{
					imgWalkthr.Frame = new CGRect(i * 712, 0, 712, 565);
				}
				else if (SystemVersion >= 14)
                {
					imgWalkthr.Frame = new CGRect(i * 704, 0, 704, 565);
				}
                else
				{
                    imgWalkthr.Frame = new CGRect(i * 768, 20, 768, 665);
                }
				
				
				string img = string.Empty;
				if (i == count - 1)
					img = "Tutorial Screens/Tut " + "99" + ".png";
				else
					img = "Tutorial Screens/Tut " + i + ".png";
				imgWalkthr.Image = UIImage.FromFile (img);
				
				imgWalkthr.ContentMode = UIViewContentMode.ScaleAspectFit;
				scrlWalkthr.AddSubview (imgWalkthr);
			}
			this.scrlWalkthr.Scrolled += pageScrolled;
			this.scrlWalkthr.ContentSize = new CGSize(this.scrlWalkthr.ContentSize.Width - 800 * 2, this.scrlWalkthr.ContentSize.Height);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		void pageScrolled (object sender, EventArgs e)
		{
			// changes current page indication of page control.
			this.pgCtrlWalkthr.CurrentPage = (int)System.Math.Floor((this.scrlWalkthr.ContentOffset.X) / this.scrlWalkthr.Frame.Size.Width);
		}
       
		partial void btnDoneTap (UIKit.UIBarButtonItem sender) {
			foreach (var vw in scrlWalkthr.Subviews) {
				vw.RemoveFromSuperview();
			}
			// Set a value for key "First" which is checked again when app is launched next time. This prevents display of Walkthrough View.
			NSUserDefaults.StandardUserDefaults.SetString("Done","First");
			NSUserDefaults.StandardUserDefaults.Synchronize();
			this.DismissViewController(true,()=>{
				home.PerformSelector(new ObjCRuntime.Selector("walkthroughDismissed"), null, 0.5);
			});
		}
	}
}

