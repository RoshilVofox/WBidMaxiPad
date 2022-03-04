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
    [Register ("bidEditorPrepViewController")]
    partial class bidEditorPrepViewController
    {
        [Outlet]
        UIKit.UIButton btnAvoidance { get; set; }


        [Outlet]
        UIKit.UIButton btnAvoidanceBid { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnChangeEmp { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnHelp { get; set; }


        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UIButton btnStartWithCurrentLine { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceText { get; set; }


        [Outlet]
        UIKit.UIPickerView pckrDomicileSelect { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlVW { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgBidPeriod { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgBidRound { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPosition { get; set; }


        [Outlet]
        UIKit.UITextField txtLineRangeParamOne { get; set; }


        [Outlet]
        UIKit.UITextField txtLineRangeParamTwo { get; set; }


        [Action ("sgPositionTapped:")]
        partial void sgPositionTapped (Foundation.NSObject sender);


        [Action ("sgBidRoundTapped:")]
        partial void sgBidRoundTapped (Foundation.NSObject sender);


        [Action ("sgBidPeriodTapped:")]
        partial void sgBidPeriodTapped (UIKit.UISegmentedControl sender);


        [Action ("btnStartWithCurrentLineTapped:")]
        partial void btnStartWithCurrentLineTapped (UIKit.UIButton sender);


        [Action ("btnOkTapped:")]
        partial void btnOkTapped (UIKit.UIButton sender);


        [Action ("btnChangeEmpTapped:")]
        partial void btnChangeEmpTapped (UIKit.UIButton sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnAvoidanceTapped:")]
        partial void btnAvoidanceTapped (UIKit.UIButton sender);


        [Action ("btnAvoidanceBidTapped:")]
        partial void btnAvoidanceBidTapped (UIKit.UIButton sender);


        [Action ("btnelpTapped:")]
        partial void btnelpTapped (Foundation.NSObject sender);


        [Action ("btnEmployeeNumberTapped:")]
        partial void btnEmployeeNumberTapped (Foundation.NSObject sender);


        [Action ("pckrBidPeriodTapped:")]
        partial void pckrBidPeriodTapped (UIKit.UISegmentedControl sender);


        [Action ("pckrBidRoundTapped:")]
        partial void pckrBidRoundTapped (UIKit.UISegmentedControl sender);


        [Action ("pckrPositionTapped:")]
        partial void pckrPositionTapped (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}