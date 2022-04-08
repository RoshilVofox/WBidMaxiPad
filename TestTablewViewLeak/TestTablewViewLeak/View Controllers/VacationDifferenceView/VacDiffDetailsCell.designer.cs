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
	[Register ("VacDiffDetailsCell")]
	partial class VacDiffDetailsCell
	{
		[Outlet]
		UIKit.UILabel lblLine { get; set; }

		[Outlet]
		UIKit.UILabel lblNewTotPay { get; set; }

		[Outlet]
		UIKit.UILabel lblNewVpBo { get; set; }

		[Outlet]
		UIKit.UILabel lblNewVPCu { get; set; }

		[Outlet]
		UIKit.UILabel lblNewVpNe { get; set; }

		[Outlet]
		UIKit.UILabel lblOldTotPay { get; set; }

		[Outlet]
		UIKit.UILabel lblOldVpBo { get; set; }

		[Outlet]
		UIKit.UILabel lblOldVpCu { get; set; }

		[Outlet]
		UIKit.UILabel lblOldVpne { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblLine != null) {
				lblLine.Dispose ();
				lblLine = null;
			}

			if (lblNewVpBo != null) {
				lblNewVpBo.Dispose ();
				lblNewVpBo = null;
			}

			if (lblOldVpBo != null) {
				lblOldVpBo.Dispose ();
				lblOldVpBo = null;
			}

			if (lblOldTotPay != null) {
				lblOldTotPay.Dispose ();
				lblOldTotPay = null;
			}

			if (lblNewTotPay != null) {
				lblNewTotPay.Dispose ();
				lblNewTotPay = null;
			}

			if (lblOldVpCu != null) {
				lblOldVpCu.Dispose ();
				lblOldVpCu = null;
			}

			if (lblNewVPCu != null) {
				lblNewVPCu.Dispose ();
				lblNewVPCu = null;
			}

			if (lblOldVpne != null) {
				lblOldVpne.Dispose ();
				lblOldVpne = null;
			}

			if (lblNewVpNe != null) {
				lblNewVpNe.Dispose ();
				lblNewVpNe = null;
			}
		}
	}
}
