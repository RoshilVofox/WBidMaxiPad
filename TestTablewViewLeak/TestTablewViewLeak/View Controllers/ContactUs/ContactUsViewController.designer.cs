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
    [Register ("ContactUsViewController")]
    partial class ContactUsViewController
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnSend { get; set; }


        [Outlet]
        UIKit.UILabel lblVersion { get; set; }


        [Outlet]
        UIKit.UITextView txtDescription { get; set; }


        [Outlet]
        UIKit.UITextField txtEmailField { get; set; }


        [Outlet]
        UIKit.UITextField txtEmpNum { get; set; }


        [Outlet]
        UIKit.UITextField txtNameField { get; set; }


        [Outlet]
        UIKit.UITextField txtPhoneField { get; set; }


        [Outlet]
        UIKit.UILabel wifiVersion { get; set; }


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnSendTapped:")]
        partial void btnSendTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}