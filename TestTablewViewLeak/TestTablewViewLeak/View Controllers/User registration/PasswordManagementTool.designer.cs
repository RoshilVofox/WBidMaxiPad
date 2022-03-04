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
    [Register ("PasswordManagementTool")]
    partial class PasswordManagementTool
    {
        [Outlet]
        UIKit.UIButton btnClose { get; set; }


        [Outlet]
        UIKit.UIButton btnCreatePassword { get; set; }


        [Outlet]
        UIKit.UIButton btnForgotPassword { get; set; }


        [Outlet]
        UIKit.UITextField CPtxtPassword { get; set; }


        [Outlet]
        UIKit.UITextField CPtxtrepeatPassword { get; set; }


        [Outlet]
        UIKit.UIButton EPbtnSavePassword { get; set; }


        [Outlet]
        UIKit.UITextField EPtxtNewPassword { get; set; }


        [Outlet]
        UIKit.UITextField EPtxtRepeatPassword { get; set; }


        [Outlet]
        UIKit.UIView ViewCP { get; set; }


        [Outlet]
        UIKit.UIView ViewEditPassword { get; set; }


        [Outlet]
        UIKit.UIView ViewForgotPassword { get; set; }


        [Outlet]
        UIKit.UIView ViewVerifyPassword { get; set; }


        [Outlet]
        UIKit.UIButton VPbtnVerifyPassword { get; set; }


        [Outlet]
        UIKit.UITextField VPtxtPassword { get; set; }


        [Action ("EPbtnSavePasswordClicked:")]
        partial void EPbtnSavePasswordClicked (Foundation.NSObject sender);


        [Action ("EPbtnCloseBtnClicked:")]
        partial void EPbtnCloseBtnClicked (Foundation.NSObject sender);


        [Action ("VPbtnVerifyPasswordClicked:")]
        partial void VPbtnVerifyPasswordClicked (Foundation.NSObject sender);


        [Action ("VPbtnForgotPasswordClicked:")]
        partial void VPbtnForgotPasswordClicked (Foundation.NSObject sender);


        [Action ("btnSendViaMailClicked:")]
        partial void btnSendViaMailClicked (Foundation.NSObject sender);


        [Action ("btnSendViaTextButtonClicked:")]
        partial void btnSendViaTextButtonClicked (Foundation.NSObject sender);


        [Action ("btnSendViawncoClicked:")]
        partial void btnSendViawncoClicked (Foundation.NSObject sender);


        [Action ("btnForgotPasswordDismiss:")]
        partial void btnForgotPasswordDismiss (Foundation.NSObject sender);


        [Action ("btnCreatePasswordClicked:")]
        partial void btnCreatePasswordClicked (Foundation.NSObject sender);


        [Action ("btnDismissPopover:")]
        partial void btnDismissPopover (Foundation.NSObject sender);


        [Action ("btnForgotPasswordClicked:")]
        partial void btnForgotPasswordClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}