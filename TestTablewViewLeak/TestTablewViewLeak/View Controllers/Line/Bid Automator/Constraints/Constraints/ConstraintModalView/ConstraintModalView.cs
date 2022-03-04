
using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class ConstraintModalView : UIViewController
	{
		ConstraintsChangeViewController _parentVC;
		public ConstraintModalView (ConstraintsChangeViewController parentVC) : base ("ConstraintModalView", null)
		{
			
			_parentVC = parentVC;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tvConstraint.TableFooterView = new UIView ();
			tvConstraint.Source = new ConstraintTableSource (this);
		}
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);


			foreach (UIView view in this.View.Subviews) {

				DisposeClass.DisposeEx(view);
			}
			this.View.Dispose ();


		}
		public void AddConstraintAtIndex (int row)
		{
			_parentVC.DismissViewController(true,()=>{
				_parentVC.AddConstraintAtIndex(row);	
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				_parentVC.DismissViewController (true, null);
			}
		}
	}
}

