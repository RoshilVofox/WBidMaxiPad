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
    [Register ("homeViewController")]
    partial class homeViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnEdit { get; set; }


        [Outlet]
        UIKit.UIButton btnHelp { get; set; }


        [Outlet]
        UIKit.UIButton btnMisc { get; set; }


        [Outlet]
        UIKit.UIButton btnRetrive { get; set; }


        [Outlet]
        UIKit.UINavigationBar NavigationBar { get; set; }


        [Outlet]
        UIKit.UIView vWBidDataCollectionContainer { get; set; }


        [Outlet]
        UIKit.UIView vwSideBar { get; set; }


        [Action ("btnEditTapped:")]
        partial void btnEditTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnMiscTapped:")]
        partial void btnMiscTapped (UIKit.UIButton sender);


        [Action ("btnHelpTapped:")]
        partial void btnHelpTapped (UIKit.UIButton sender);


        [Action ("btnNewBidPeriodTapped:")]
        partial void btnNewBidPeriodTapped (UIKit.UIButton sender);


        [Action ("btnDemoLIneTapped:")]
        partial void btnDemoLIneTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}