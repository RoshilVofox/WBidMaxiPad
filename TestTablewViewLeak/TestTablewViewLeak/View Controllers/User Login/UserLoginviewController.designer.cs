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
    [Register ("UserLoginviewController")]
    partial class UserLoginviewController
    {
        [Outlet]
        UIKit.UIButton btnLogIn { get; set; }


        [Outlet]
        UIKit.UIButton DisMissPopPver { get; set; }


        [Outlet]
        UIKit.UITextField txtEmpNo { get; set; }


        [Outlet]
        UIKit.UITextField txtPassword { get; set; }


        [Action ("LogInButtonClicked:")]
        partial void LogInButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}