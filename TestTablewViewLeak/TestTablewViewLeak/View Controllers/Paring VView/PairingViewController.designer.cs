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
    [Register ("PairingViewController")]
    partial class PairingViewController
    {
        [Outlet]
        UIKit.UIButton btnExport { get; set; }


        [Outlet]
        UIKit.UITableView tblDayView { get; set; }


        [Outlet]
        UIKit.UITableView tblTripPairingView { get; set; }


        [Outlet]
        UIKit.UITableView tblTripView { get; set; }


        [Outlet]
        UIKit.UIToolbar tbTopBar { get; set; }


        [Action ("btnDoneTapped:")]
        partial void btnDoneTapped (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}