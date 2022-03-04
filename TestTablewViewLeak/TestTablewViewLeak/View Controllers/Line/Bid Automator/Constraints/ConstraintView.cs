
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintView : UIViewController
	{
		public ConstraintsChangeViewController constraintsCont;

	
		public ConstraintView (IntPtr handle) : base (handle)
		{
		}


		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		private void loadConstraintsList ()
		{
			UIStoryboard storyboard = UIStoryboard.FromName ("Main", null);
			constraintsCont =storyboard.InstantiateViewController ("ConstraintsChangeViewController") as ConstraintsChangeViewController;
			constraintsCont.View.Frame = new CoreGraphics.CGRect(0,60,this.View.Bounds.Width,this.View.Bounds.Height - 60);
			this.AddChildViewController (constraintsCont);
			this.View.AddSubview (constraintsCont.View);

		}
		partial void AddConstraintsClicked (NSObject sender)
		{
			
			NSNotificationCenter.DefaultCenter.PostNotificationName("AddConstraintClick", null);

		}
		partial void ClearButtonClicked (NSObject sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("ClearButtonClicked",null);
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			loadConstraintsList ();
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

