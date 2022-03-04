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
    [Register ("loginViewController")]
    partial class loginViewController
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnLogin { get; set; }


        [Outlet]
        UIKit.UITextField txtPassword { get; set; }


        [Outlet]
        UIKit.UITextField txtUserName { get; set; }


        [Action ("btnLoginTapped:")]
        partial void btnLoginTapped (Foundation.NSObject sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}