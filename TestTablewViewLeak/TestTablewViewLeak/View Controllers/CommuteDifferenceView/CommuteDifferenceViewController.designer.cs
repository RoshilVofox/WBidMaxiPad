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
	[Register ("CommuteDifferenceViewController")]
	partial class CommuteDifferenceViewController
	{
		[Outlet]
		UIKit.UILabel lblLine { get; set; }

		[Outlet]
		UIKit.UILabel lblOldCmtOv { get; set; }

		[Outlet]
		UIKit.UILabel lbNewCmtOv { get; set; }

		[Outlet]
		UIKit.UITableView tblCommuteDifference { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblLine != null) {
				lblLine.Dispose ();
				lblLine = null;
			}

			if (lblOldCmtOv != null) {
				lblOldCmtOv.Dispose ();
				lblOldCmtOv = null;
			}

			if (lbNewCmtOv != null) {
				lbNewCmtOv.Dispose ();
				lbNewCmtOv = null;
			}

			if (tblCommuteDifference != null) {
				tblCommuteDifference.Dispose ();
				tblCommuteDifference = null;
			}
		}
	}
}
