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
    [Register ("HelpViewController")]
    partial class HelpViewController
    {
        [Outlet]
        UIKit.UIButton btnFullScrn { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint DetailViewLeftConstraint { get; set; }


        [Outlet]
        UIKit.UINavigationBar navBar { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgDocVid { get; set; }


        [Outlet]
        UIKit.UITableView tblSideView { get; set; }


        [Outlet]
        UIKit.UIWebView wbHelpView { get; set; }


        [Action ("btnDoneTapped:")]
        partial void btnDoneTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnFullScrnTapped:")]
        partial void btnFullScrnTapped (UIKit.UIButton sender);


        [Action ("sgDocVidChanged:")]
        partial void sgDocVidChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}