// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
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

        [Action ("btnReparseTapped:")]
        partial void btnReparseTapped (Foundation.NSObject sender);

        [Action ("btnSelectDomicilesAction:")]
        partial void btnSelectDomicilesAction (UIKit.UIButton sender);

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

        [Action ("btnCrash:")]
        partial void btnCrash (Foundation.NSObject sender);


        [Action ("sgQATestTapped:")]
        partial void sgQATestTapped (Foundation.NSObject sender);

        [Action ("WifiPaidOrNot:")]
        partial void WifiPaidOrNot (Foundation.NSObject sender);

        [Action ("WifiValueChanged:")]
        partial void WifiValueChanged (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}