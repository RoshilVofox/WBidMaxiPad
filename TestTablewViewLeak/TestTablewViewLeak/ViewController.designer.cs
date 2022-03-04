// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TestTablewViewLeak
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton funPresentView { get; set; }

		[Action ("funGotoUserAccountPage:")]
		partial void funGotoUserAccountPage (Foundation.NSObject sender);

		[Action ("GoToTablewView:")]
		partial void GoToTablewView (Foundation.NSObject sender);

		[Action ("PresentView:")]
		partial void PresentView (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (funPresentView != null) {
				funPresentView.Dispose ();
				funPresentView = null;
			}
		}
	}
}
