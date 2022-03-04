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
    [Register ("AvoidanceBidVC")]
    partial class AvoidanceBidVC
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoid1 { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoid2 { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoid3 { get; set; }


        [Action ("brnOkTapped:")]
        partial void brnOkTapped (Foundation.NSObject sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}