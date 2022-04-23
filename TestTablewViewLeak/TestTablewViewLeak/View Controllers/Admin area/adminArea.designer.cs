// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidiPad.iOS
{
	[Register ("adminArea")]
	partial class adminArea
	{
		[Outlet]
		UIKit.UIButton btnReparse { get; set; }

		[Outlet]
		UIKit.UIButton btnSubmit { get; set; }

		[Outlet]
		UIKit.UIButton btnUserIdCheckBox { get; set; }

		[Outlet]
		UIKit.UILabel lblWifiName { get; set; }

		[Outlet]
		UIKit.UISegmentedControl SegIsPaid { get; set; }

		[Outlet]
		UIKit.UISegmentedControl segWifi { get; set; }

		[Outlet]
		UIKit.UISegmentedControl sgMock { get; set; }

		[Outlet]
		UIKit.UISegmentedControl sgQATest { get; set; }

		[Outlet]
		UIKit.UISegmentedControl SgSenList { get; set; }

		[Outlet]
		UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		UIKit.UITextField txtUserId { get; set; }

		[Action ("btnBackTapped:")]
		partial void btnBackTapped (Foundation.NSObject sender);

		[Action ("btnCrash:")]
		partial void btnCrash (Foundation.NSObject sender);

		[Action ("btnReparseTapped:")]
		partial void btnReparseTapped (Foundation.NSObject sender);

		[Action ("btnSelectDomicilesAction:")]
		partial void btnSelectDomicilesAction (UIKit.UIButton sender);

		[Action ("btnService1Tap:")]
		partial void btnService1Tap (Foundation.NSObject sender);

		[Action ("btnService2Tap:")]
		partial void btnService2Tap (Foundation.NSObject sender);

		[Action ("btnService3Tap:")]
		partial void btnService3Tap (Foundation.NSObject sender);

		[Action ("btnService4:")]
		partial void btnService4 (Foundation.NSObject sender);

		[Action ("btnSubmitTap:")]
		partial void btnSubmitTap (UIKit.UIButton sender);

		[Action ("btnSubmitTapped:")]
		partial void btnSubmitTapped (Foundation.NSObject sender);

		[Action ("btnUserIdCheckBoxTapped:")]
		partial void btnUserIdCheckBoxTapped (Foundation.NSObject sender);

		[Action ("SenListValueChanged:")]
		partial void SenListValueChanged (Foundation.NSObject sender);

		[Action ("sgMockTapped:")]
		partial void sgMockTapped (Foundation.NSObject sender);

		[Action ("sgQATestTapped:")]
		partial void sgQATestTapped (Foundation.NSObject sender);

		[Action ("WifiPaidOrNot:")]
		partial void WifiPaidOrNot (Foundation.NSObject sender);

		[Action ("WifiValueChanged:")]
		partial void WifiValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnReparse != null) {
				btnReparse.Dispose ();
				btnReparse = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (btnUserIdCheckBox != null) {
				btnUserIdCheckBox.Dispose ();
				btnUserIdCheckBox = null;
			}

			if (lblWifiName != null) {
				lblWifiName.Dispose ();
				lblWifiName = null;
			}

			if (SegIsPaid != null) {
				SegIsPaid.Dispose ();
				SegIsPaid = null;
			}

			if (segWifi != null) {
				segWifi.Dispose ();
				segWifi = null;
			}

			if (sgMock != null) {
				sgMock.Dispose ();
				sgMock = null;
			}

			if (sgQATest != null) {
				sgQATest.Dispose ();
				sgQATest = null;
			}

			if (SgSenList != null) {
				SgSenList.Dispose ();
				SgSenList = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtUserId != null) {
				txtUserId.Dispose ();
				txtUserId = null;
			}
		}
	}
}
