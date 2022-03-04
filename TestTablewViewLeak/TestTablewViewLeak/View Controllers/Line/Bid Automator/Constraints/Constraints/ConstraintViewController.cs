
using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintViewController : UIViewController
	{
       
		public ConstraintsChangeViewController constraintsCont;
		public ConstraintViewController () : base ("ConstraintViewController", null)
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
			loadConstraintsList ();
			// Perform any additional setup after loading the view, typically from a nib.
		}
		private void loadConstraintsList ()
		{
			UIStoryboard storyboard = UIStoryboard.FromName ("Main", null);
			constraintsCont =storyboard.InstantiateViewController ("ConstraintsChangeViewController") as ConstraintsChangeViewController;
			constraintsCont.View.Frame = new CoreGraphics.CGRect(0,60,this.View.Bounds.Width,this.View.Bounds.Height - 60);
			this.AddChildViewController (constraintsCont);
			this.View.AddSubview (constraintsCont.View);

		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("CalPopHide", null);
			//txtCellNumber.Dispose ();

//			foreach (UIView view in this.View.Subviews) {
//
//				DisposeClass.DisposeEx(view);
//			}
//			this.View.Dispose ();
			this.View.UserInteractionEnabled = true;

		}

		partial void AddContraintsFunction (NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraintClick", null);

		}
		partial void ClearConstraintsFunction (NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("ClearButtonClicked",null);
		}


	}
}

