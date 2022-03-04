using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidiPad.iOS
{
	public partial class ChangeEmpNumberView : UIViewController
	{
		public ChangeEmpNumberView () : base ("ChangeEmpNumberView", null)
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

				//DisposeClass.DisposeEx(view);
			}
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);

            if(SystemVersion > 12)
            {
				ModalInPresentation = true;
			}
            


			// Perform any additional setup after loading the view, typically from a nib.
			txtEmpNo.Text = GlobalSettings.TemporaryEmployeeNumber ?? string.Empty;
			txtEmpNo.BecomeFirstResponder ();
			txtEmpNo.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				string text = textField.Text;
				string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
				int val;
				if (newText == "" && newText.Length<10)
					return true;
				else
					return Int32.TryParse(newText, out val);
			};
		}

		partial void btnCancelTapped (UIKit.UIButton sender)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("ChangeEmpNumber",null);
			this.DismissViewController(true,null);
		}

        
        partial void backaction(NSObject sender)
        {
			
			this.DismissViewController(true, null);
		}
        partial void btnOKTapped (UIKit.UIButton sender)
		{
			GlobalSettings.TemporaryEmployeeNumber = txtEmpNo.Text;
			NSNotificationCenter.DefaultCenter.PostNotificationName("ChangeEmpNumber",null);
			this.DismissViewController(true,null);
		}
	}
}

