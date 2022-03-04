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
    [Register ("viewFileViewController")]
    partial class viewFileViewController
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnHelp { get; set; }


        [Outlet]
        UIKit.UIButton btnViewFile { get; set; }


        [Outlet]
        UIKit.UIPickerView pckrDomicileSelect { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgMonthlyFile { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPosition { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgReserveFile { get; set; }


        [Action ("btnSelectDomicileTapped:")]
        partial void btnSelectDomicileTapped (UIKit.UIButton sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);


        [Action ("btnViewFileTapped:")]
        partial void btnViewFileTapped (Foundation.NSObject sender);


        [Action ("sgMonthlyFileTapped:")]
        partial void sgMonthlyFileTapped (UIKit.UISegmentedControl sender);


        [Action ("sgPositionTapped:")]
        partial void sgPositionTapped (UIKit.UISegmentedControl sender);


        [Action ("sgReserveFileTapped:")]
        partial void sgReserveFileTapped (UIKit.UISegmentedControl sender);


        [Action ("btnHelpTapped:")]
        partial void btnHelpTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}