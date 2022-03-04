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
    [Register ("WalkthroughViewController")]
    partial class WalkthroughViewController
    {
        [Outlet]
        UIKit.UINavigationBar navBar { get; set; }


        [Outlet]
        UIKit.UIPageControl pgCtrlWalkthr { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlWalkthr { get; set; }


        [Action ("btnDoneTap:")]
        partial void btnDoneTap (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}