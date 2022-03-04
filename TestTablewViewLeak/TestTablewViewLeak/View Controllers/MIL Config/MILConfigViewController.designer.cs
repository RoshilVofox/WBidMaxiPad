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
    [Register ("MILConfigViewController")]
    partial class MILConfigViewController
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UILabel [] lblEnd { get; set; }


        [Outlet]
        UIKit.UILabel [] lblStart { get; set; }


        [Outlet]
        UIKit.UINavigationBar navBar { get; set; }


        [Outlet]
        UIKit.UITableView tblMILDates { get; set; }


        [Outlet]
        UIKit.UIView vwMILInfo { get; set; }


        [Action ("btnDoneTapped:")]
        partial void btnDoneTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnAddTapped:")]
        partial void btnAddTapped (UIKit.UIButton sender);


        [Action ("btnApplyTapped:")]
        partial void btnApplyTapped (UIKit.UIButton sender);


        [Action ("btnCalculateNewTapped:")]
        partial void btnCalculateNewTapped (UIKit.UIButton sender);


        [Action ("btnCancel1Tapped:")]
        partial void btnCancel1Tapped (UIKit.UIButton sender);


        [Action ("btnCalculateTapped:")]
        partial void btnCalculateTapped (UIKit.UIButton sender);


        [Action ("btnCancel2Tapped:")]
        partial void btnCancel2Tapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}