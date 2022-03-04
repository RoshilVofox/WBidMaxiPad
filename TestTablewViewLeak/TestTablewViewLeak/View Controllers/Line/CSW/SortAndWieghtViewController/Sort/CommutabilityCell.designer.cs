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
    [Register ("CommutabilityCell")]
    partial class CommutabilityCell
    {
        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnTitle { get; set; }


        [Outlet]
        UIKit.UIButton btnValue { get; set; }


        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);


        [Action ("btnTitleTapped:")]
        partial void btnTitleTapped (Foundation.NSObject sender);


        [Action ("btnValueTapped:")]
        partial void btnValueTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}