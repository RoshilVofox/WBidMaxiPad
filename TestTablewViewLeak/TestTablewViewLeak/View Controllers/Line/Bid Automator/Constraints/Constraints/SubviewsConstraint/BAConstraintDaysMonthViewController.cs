
using System;

using Foundation;
using UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class BAConstraintDaysMonthViewController : UIViewController
	{

		private int SCREEN_HEIGHT = 568;
		private int BORDER_CONNER = 10;
		private int BORDER_WIDTH = 1;
		public DaysOfMonthCx data;
		private PickerView _view;
		public ConstraintsChangeViewController ObjChangeController;
		public BAConstraintDaysMonthViewController () : base ("BAConstraintDaysMonthViewController", null)
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
		
	

			//foreach (UIView view in this.View.Subviews) {

			//	DisposeClass.DisposeEx(view);
			//}
			//this.View.Dispose ();
			//ObjChangeController = null;

		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
//			BorderBtns (btnOff);
//			BorderBtns (btnDefault);
//			BorderBtns (btnOn);
			View.BackgroundColor = UIColor.Clear;
			//UIHelpers.StyleForButtons (new UIButton[]{ btnDone, btnClear });
			// Perform any additional setup after loading the view, typically from a nib.
		}

		void BorderBtns (UIButton btn)
		{
			btn.Layer.BorderColor = UIColor.Black.CGColor;
			btn.Layer.BorderWidth = BORDER_WIDTH;
			btn.Layer.CornerRadius = BORDER_CONNER;
		}

//		partial void OnClearEvent (Foundation.NSObject sender)
//		{
//			_view.ClearAll ();
//		}
//
//		partial void OnDoneEvent (Foundation.NSObject sender)
//		{
//			data.OFFDays = _view.GetListOffDays ();
//			data.WorkDays = _view.GetListWorkDays ();
//			ObjChangeController.ViewDismissViewController(this);
//
//
//		}

		public override void ViewDidAppear (bool animated)
		{
			_view = new PickerView (UIColor.Gray, UIColor.Red, Colors.BidGreen,data);
			_view.BackgroundColor = UIColor.Clear;
			_view.Frame =viewCalendarShow.Bounds;
			viewCalendarShow.AddSubview (_view);

			base.ViewDidAppear (animated);
		}
		partial void ClearButtonClicked (NSObject sender)
		{
			_view.ClearAll ();
		}
		partial void DoneButtonClicked (NSObject sender)
		{
			data.OFFDays = _view.GetListOffDays ();
			data.WorkDays = _view.GetListWorkDays ();
			if(data.OFFDays.Count==0 && data.WorkDays.Count==0)
				SharedObject.Instance.ListConstraint.Remove(data);
			
			ObjChangeController.ViewDismissViewController(this);


		}
	}
}

