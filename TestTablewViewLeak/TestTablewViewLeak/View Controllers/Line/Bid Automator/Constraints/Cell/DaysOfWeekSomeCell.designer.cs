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
    [Register ("DaysOfWeekSomeCell")]
    partial class DaysOfWeekSomeCell
    {
        [Outlet]
        UIKit.UIButton btnDaySomeConstraint { get; set; }


        [Outlet]
        UIKit.UIButton btnDaySomeValue { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnLessthan { get; set; }


        [Action ("OnDaySomeConstraintEvent:")]
        partial void OnDaySomeConstraintEvent (Foundation.NSObject sender);


        [Action ("OnDaySomeValueEvent:")]
        partial void OnDaySomeValueEvent (Foundation.NSObject sender);


        [Action ("OnLessthanEvent:")]
        partial void OnLessthanEvent (Foundation.NSObject sender);


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnDelete:")]
        partial void OnDelete (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}