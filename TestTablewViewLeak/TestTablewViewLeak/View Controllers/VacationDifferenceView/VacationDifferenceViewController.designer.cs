// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TestTablewViewLeak.ViewControllers.VacationDifferenceView
{
	[Register ("VacationDifferenceViewController")]
	partial class VacationDifferenceViewController
	{
		[Outlet]
		UIKit.UIButton btnOk { get; set; }

		[Outlet]
		UIKit.UITableView tblVacDifference { get; set; }

		[Action ("btnOkClick:")]
		partial void btnOkClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tblVacDifference != null) {
				tblVacDifference.Dispose ();
				tblVacDifference = null;
			}

			if (btnOk != null) {
				btnOk.Dispose ();
				btnOk = null;
			}
		}
	}
}
