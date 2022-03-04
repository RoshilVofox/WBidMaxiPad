
using System;

using Foundation;
using UIKit;
using System.Linq;

namespace WBid.WBidiPad.iOS
{
	public partial class CellCarrierPicker : UIViewController
	{
		public string selectedCellCarrier;
		public UIView viewbase=new UIView();
		public UIPopoverController objpopover;
		public userRegistrationViewController SuperParent;
		public CellCarrierPicker () : base ("CellCarrierPicker", null)
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
			viewbase = ViewBg;
			pickerView.Model = new pickerViewModel (this);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class pickerViewModel : UIPickerViewModel
		{
			CellCarrierPicker parent;
			string[] arr =  CommonClass.CellCarrier;
			public pickerViewModel(CellCarrierPicker parentVC)
			{
				parent = parentVC;
			}

			public override nint GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView picker, nint component)
			{
				return arr.Count();
			}

			public override string GetTitle (UIPickerView picker, nint row, nint component)
			{
				return arr[row];
			}
			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				parent.selectedCellCarrier = arr[row];
				parent.SuperParent.setCarriername(parent.selectedCellCarrier);
				parent.objpopover.Dismiss (true);
			}

		}
	}
}

