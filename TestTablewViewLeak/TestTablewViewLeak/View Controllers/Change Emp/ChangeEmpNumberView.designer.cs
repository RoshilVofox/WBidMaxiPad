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
    [Register ("ChangeEmpNumberView")]
    partial class ChangeEmpNumberView
    {
        [Outlet]
        UIKit.UITextField txtEmpNo { get; set; }


        [Action ("backaction:")]
        partial void backaction (Foundation.NSObject sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnOKTapped:")]
        partial void btnOKTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}