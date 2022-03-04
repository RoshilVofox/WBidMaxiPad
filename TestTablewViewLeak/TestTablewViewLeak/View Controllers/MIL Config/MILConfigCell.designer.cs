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
    [Register ("MILConfigCell")]
    partial class MILConfigCell
    {
        [Outlet]
        UIKit.UIButton btnClose { get; set; }


        [Outlet]
        UIKit.UIButton btnEnd { get; set; }


        [Outlet]
        UIKit.UIButton btnStart { get; set; }


        [Action ("btnStartTapped:")]
        partial void btnStartTapped (UIKit.UIButton sender);


        [Action ("btnEndTapped:")]
        partial void btnEndTapped (UIKit.UIButton sender);


        [Action ("btnCloseTapped:")]
        partial void btnCloseTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}