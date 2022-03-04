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
	partial class LeakTablewViewControllerCell
	{
		[Outlet]
		UIKit.UIView BaseView { get; set; }

		[Outlet]
		UIKit.UIButton btnLineSelect { get; set; }

		[Outlet]
		UIKit.UIImageView imgCrossIcon { get; set; }

		[Outlet]
		UIKit.UIImageView imgLockIcon { get; set; }

		[Outlet]
		UIKit.UIImageView imgOverlapIcon { get; set; }

		[Outlet]
		UIKit.UIImageView imgVLayer { get; set; }

		[Outlet]
		UIKit.UILabel[] lblCalDate { get; set; }

		[Outlet]
		UIKit.UILabel[] lblCalDay { get; set; }

		[Outlet]
		UIKit.UILabel lblLineNum { get; set; }

		[Outlet]
		UIKit.UILabel lblLinePos { get; set; }

		[Outlet]
		UIKit.UILabel lblLineSlNo { get; set; }

		[Outlet]
		UIKit.UILabel[] lblProperty1 { get; set; }

		[Outlet]
		UIKit.UILabel[] lblProperty2 { get; set; }

		[Outlet]
		UIKit.UILabel[] lblProperty3 { get; set; }

		[Outlet]
		UIKit.UILabel[] lblProperty4 { get; set; }

		[Outlet]
		UIKit.UILabel[] lblProperty5 { get; set; }

		[Outlet]
		UIKit.UILabel[] lblPropName { get; set; }

		[Outlet]
		UIKit.UILabel[] lblPropValue { get; set; }

		[Outlet]
		UIKit.UILabel[] lblTripArrival { get; set; }

		[Outlet]
		UIKit.UILabel[] lblTripName { get; set; }

		[Outlet]
		UIKit.UILabel[] lblVacType { get; set; }

		[Outlet]
		UIKit.UIView vwChild1 { get; set; }

		[Outlet]
		UIKit.UIView vwChild2 { get; set; }

		[Outlet]
		UIKit.UIView[] vwChild3 { get; set; }

		[Action ("btnLineSelectTapped:")]
		partial void btnLineSelectTapped (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BaseView != null) {
				BaseView.Dispose ();
				BaseView = null;
			}

			if (btnLineSelect != null) {
				btnLineSelect.Dispose ();
				btnLineSelect = null;
			}

			if (imgCrossIcon != null) {
				imgCrossIcon.Dispose ();
				imgCrossIcon = null;
			}

			if (imgLockIcon != null) {
				imgLockIcon.Dispose ();
				imgLockIcon = null;
			}

			if (imgOverlapIcon != null) {
				imgOverlapIcon.Dispose ();
				imgOverlapIcon = null;
			}

			if (imgVLayer != null) {
				imgVLayer.Dispose ();
				imgVLayer = null;
			}

			if (lblLineNum != null) {
				lblLineNum.Dispose ();
				lblLineNum = null;
			}

			if (lblLinePos != null) {
				lblLinePos.Dispose ();
				lblLinePos = null;
			}

			if (lblLineSlNo != null) {
				lblLineSlNo.Dispose ();
				lblLineSlNo = null;
			}

			if (vwChild1 != null) {
				vwChild1.Dispose ();
				vwChild1 = null;
			}

			if (vwChild2 != null) {
				vwChild2.Dispose ();
				vwChild2 = null;
			}
		}
	}
}
