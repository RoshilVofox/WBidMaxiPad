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
    [Register ("AMPMCell")]
    partial class AMPMCell
    {
        [Outlet]
        UIKit.UIButton btnAm { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnMix { get; set; }


        [Outlet]
        UIKit.UIButton btnPm { get; set; }


        [Outlet]
        UIKit.UILabel lbName { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnAmEvent:")]
        partial void OnAmEvent (Foundation.NSObject sender);


        [Action ("OnMixEvent:")]
        partial void OnMixEvent (Foundation.NSObject sender);


        [Action ("OnPmEvent:")]
        partial void OnPmEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}