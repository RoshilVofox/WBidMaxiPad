// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TestTablewViewLeak.ViewControllers.CommuteDifferenceView
{
	[Register ("FltDiffButtonViewController")]
	partial class FltDiffButtonViewController
	{
		[Outlet]
		UIKit.UIButton btnCmtDiff { get; set; }

		[Outlet]
		UIKit.UIButton btnVacDiff { get; set; }

		[Action ("btnCmtDiffClick:")]
		partial void btnCmtDiffClick (Foundation.NSObject sender);

		[Action ("btnVacDiffClick:")]
		partial void btnVacDiffClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnVacDiff != null) {
				btnVacDiff.Dispose ();
				btnVacDiff = null;
			}

			if (btnCmtDiff != null) {
				btnCmtDiff.Dispose ();
				btnCmtDiff = null;
			}
		}
	}
}
