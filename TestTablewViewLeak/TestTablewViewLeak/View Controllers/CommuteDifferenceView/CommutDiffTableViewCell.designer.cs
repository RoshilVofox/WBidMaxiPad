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
	[Register ("CommutDiffTableViewCell")]
	partial class CommutDiffTableViewCell
	{
		[Outlet]
		UIKit.UILabel lblLine { get; set; }

		[Outlet]
		UIKit.UILabel lblNewCmtBa { get; set; }

		[Outlet]
		UIKit.UILabel lblNewCmtFr { get; set; }

		[Outlet]
		UIKit.UILabel lblNewCmtOv { get; set; }

		[Outlet]
		UIKit.UILabel lblOldCmtBa { get; set; }

		[Outlet]
		UIKit.UILabel lblOldCmtFr { get; set; }

		[Outlet]
		UIKit.UILabel lblOldCmtOv { get; set; }
		
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

			if (lblNewCmtOv != null) {
				lblNewCmtOv.Dispose ();
				lblNewCmtOv = null;
			}

			if (lblOldCmtFr != null) {
				lblOldCmtFr.Dispose ();
				lblOldCmtFr = null;
			}

			if (lblNewCmtFr != null) {
				lblNewCmtFr.Dispose ();
				lblNewCmtFr = null;
			}

			if (lblOldCmtBa != null) {
				lblOldCmtBa.Dispose ();
				lblOldCmtBa = null;
			}

			if (lblNewCmtBa != null) {
				lblNewCmtBa.Dispose ();
				lblNewCmtBa = null;
			}
		}
	}
}
