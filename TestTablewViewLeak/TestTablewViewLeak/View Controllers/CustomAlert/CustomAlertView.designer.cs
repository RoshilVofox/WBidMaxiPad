// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace WBid.WBidiPad.iOS
{
    [Register ("CustomAlertView")]
    partial class CustomAlertView
    {
        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UILabel lblMessageText { get; set; }


        [Outlet]
        UIKit.UINavigationItem navigationTitle { get; set; }


        [Action ("btnOkTapped:")]
        partial void btnOkTapped (Foundation.NSObject sender);


        [Action ("funCloseView:")]
        partial void funCloseView (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}