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
    [Register ("LinesTypeCell")]
    partial class LinesTypeCell
    {
        [Outlet]
        UIKit.UIButton btnBlank { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnHard { get; set; }


        [Outlet]
        UIKit.UIButton btnInt { get; set; }


        [Outlet]
        UIKit.UIButton btnNonCon { get; set; }


        [Outlet]
        UIKit.UIButton btnRest { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (UIKit.UIButton sender);


        [Action ("OnNonConButtonEvent:")]
        partial void OnNonConButtonEvent (UIKit.UIButton sender);


        [Action ("OnIntButtonEvent:")]
        partial void OnIntButtonEvent (UIKit.UIButton sender);


        [Action ("OnBlankButtonEvent:")]
        partial void OnBlankButtonEvent (UIKit.UIButton sender);


        [Action ("OnRestButtonEvent:")]
        partial void OnRestButtonEvent (UIKit.UIButton sender);


        [Action ("OnHardButtonEvent:")]
        partial void OnHardButtonEvent (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}