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
    [Register ("ConstraintDaysMonthViewController")]
    partial class ConstraintDaysMonthViewController
    {
        [Outlet]
        UIKit.UIButton btnClear { get; set; }


        [Outlet]
        UIKit.UIButton btnDefault { get; set; }


        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UIButton btnOff { get; set; }


        [Outlet]
        UIKit.UIButton btnOn { get; set; }


        [Outlet]
        UIKit.UIView contentView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ctHeight { get; set; }


        [Outlet]
        UIKit.UIScrollView scrollview { get; set; }


        [Outlet]
        UIKit.UIView viewCalendarShow { get; set; }


        [Action ("OnClearEvent:")]
        partial void OnClearEvent (Foundation.NSObject sender);


        [Action ("OnDoneEvent:")]
        partial void OnDoneEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}