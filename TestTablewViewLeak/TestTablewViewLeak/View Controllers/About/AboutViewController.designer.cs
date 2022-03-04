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
    [Register ("AboutViewController")]
    partial class AboutViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnDone { get; set; }


        [Outlet]
        UIKit.UILabel lblContact { get; set; }


        [Outlet]
        UIKit.UILabel lblVersion { get; set; }


        [Action ("btnDoneTapped:")]
        partial void btnDoneTapped (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}