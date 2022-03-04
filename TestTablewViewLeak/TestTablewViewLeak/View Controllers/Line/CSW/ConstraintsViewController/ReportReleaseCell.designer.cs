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
    [Register ("ReportReleaseCell")]
    partial class ReportReleaseCell
    {
        [Outlet]
        UIKit.UIButton btnFirst { get; set; }


        [Outlet]
        UIKit.UIButton btnLast { get; set; }


        [Outlet]
        UIKit.UIButton btnNoMid { get; set; }


        [Outlet]
        UIKit.UIButton btnRelease { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnReport { get; set; }


        [Outlet]
        UIKit.UIView dropView { get; set; }


        [Outlet]
        UIKit.UILabel lblOutline { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgRptType { get; set; }


        [Outlet]
        UIKit.UITextField txtRelease { get; set; }


        [Outlet]
        UIKit.UITextField txtReport { get; set; }


        [Action ("btnCalculate:")]
        partial void btnCalculate (UIKit.UIButton sender);


        [Action ("btnFirstAction:")]
        partial void btnFirstAction (UIKit.UIButton sender);


        [Action ("btnLastAction:")]
        partial void btnLastAction (UIKit.UIButton sender);


        [Action ("btnNoMidAction:")]
        partial void btnNoMidAction (UIKit.UIButton sender);


        [Action ("btnReleaseAction:")]
        partial void btnReleaseAction (UIKit.UIButton sender);


        [Action ("btnRemoveAction:")]
        partial void btnRemoveAction (UIKit.UIButton sender);


        [Action ("btnReportAction:")]
        partial void btnReportAction (UIKit.UIButton sender);


        [Action ("sgTypeValueChanged:")]
        partial void sgTypeValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}