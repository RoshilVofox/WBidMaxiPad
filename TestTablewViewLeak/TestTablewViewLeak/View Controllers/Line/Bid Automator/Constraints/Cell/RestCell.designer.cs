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
    [Register ("RestCell")]
    partial class RestCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnLessthan { get; set; }


        [Outlet]
        UIKit.UIButton btnRestDom { get; set; }


        [Outlet]
        UIKit.UIButton btnRestValue { get; set; }


        [Outlet]
        UIKit.UILabel lbRestName { get; set; }


        [Action ("OnAwayDomEvent:")]
        partial void OnAwayDomEvent (Foundation.NSObject sender);


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnLessThanEvent:")]
        partial void OnLessThanEvent (Foundation.NSObject sender);


        [Action ("OnValueEvent:")]
        partial void OnValueEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}