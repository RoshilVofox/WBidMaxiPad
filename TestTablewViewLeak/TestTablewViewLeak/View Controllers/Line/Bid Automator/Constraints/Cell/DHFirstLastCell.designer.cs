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
    [Register ("DHFirstLastCell")]
    partial class DHFirstLastCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnDHConstraint { get; set; }


        [Outlet]
        UIKit.UIButton btnDHValue { get; set; }


        [Outlet]
        UIKit.UIButton btnLessthan { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnDHConstraintEvent:")]
        partial void OnDHConstraintEvent (Foundation.NSObject sender);


        [Action ("OnDHValueEvent:")]
        partial void OnDHValueEvent (Foundation.NSObject sender);


        [Action ("OnLessthanEvent:")]
        partial void OnLessthanEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}