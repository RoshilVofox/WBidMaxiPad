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
    [Register ("ChangeBuddyViewController")]
    partial class ChangeBuddyViewController
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnOK { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy1 { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddy2 { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddy1 { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddy2 { get; set; }


        [Action ("btnOKTapped:")]
        partial void btnOKTapped (UIKit.UIButton sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnDismiss:")]
        partial void btnDismiss (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}