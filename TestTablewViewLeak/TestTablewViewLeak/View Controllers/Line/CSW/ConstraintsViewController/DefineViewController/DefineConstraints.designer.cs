// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WBid.WBidiPad.iOS
{
    [Register ("DefineConstraints")]
    partial class DefineConstraints
    {
        [Outlet]
        UIKit.UIButton btnAmArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnAMPush { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnMaxNumber { get; set; }


        [Outlet]
        UIKit.UIButton btnMaxPercentage { get; set; }


        [Outlet]
        UIKit.UIButton btnNTEArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnNTEPush { get; set; }


        [Outlet]
        UIKit.UIButton btnOK { get; set; }


        [Outlet]
        UIKit.UIButton btnPMArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnPMPush { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgAMPM { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgLineParameter { get; set; }


        [Outlet]
        UIKit.UITextField txtArriveBeforeFirst { get; set; }


        [Outlet]
        UIKit.UITextField txtArriveBeforeSecond { get; set; }


        [Outlet]
        UIKit.UITextField txtArriveBeforeThird { get; set; }


        [Outlet]
        UIKit.UITextField txtFirstAMsPushAfter { get; set; }


        [Outlet]
        UIKit.UITextField txtMaxNumber { get; set; }


        [Outlet]
        UIKit.UITextField txtMaxPercentage { get; set; }


        [Outlet]
        UIKit.UITextField txtNTEPushAfter { get; set; }


        [Outlet]
        UIKit.UITextField txtPMsPushAfter { get; set; }


        [Action ("btnResetTapped:")]
        partial void btnResetTapped (Foundation.NSObject sender);


        [Action ("sgAMPMTapped:")]
        partial void sgAMPMTapped (UIKit.UISegmentedControl sender);


        [Action ("sgLineParamTapped:")]
        partial void sgLineParamTapped (UIKit.UISegmentedControl sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);


        [Action ("btnAMPMTimeTapped:")]
        partial void btnAMPMTimeTapped (UIKit.UIButton sender);


        [Action ("btnMaxInputTapped:")]
        partial void btnMaxInputTapped (UIKit.UIButton sender);


        [Action ("btnOkayTapped:")]
        partial void btnOkayTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}