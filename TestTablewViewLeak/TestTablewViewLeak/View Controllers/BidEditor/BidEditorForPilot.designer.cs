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
    [Register ("BidEditorForPilot")]
    partial class BidEditorForPilot
    {
        [Outlet]
        UIKit.UIButton btnAvoidance { get; set; }


        [Outlet]
        UIKit.UIButton btnClearAll { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UIButton btnSubmit { get; set; }


        [Outlet]
        UIKit.UIButton Cancel { get; set; }


        [Outlet]
        UIKit.UILabel lblBidNumber { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        public UIKit.UITableView tblSelectedLInes { get; private set; }


        [Outlet]
        UIKit.UIView vwCollectionContainer { get; set; }


        [Action ("btnSubmitTapped:")]
        partial void btnSubmitTapped (Foundation.NSObject sender);


        [Action ("btnAvoidanceTapped:")]
        partial void btnAvoidanceTapped (Foundation.NSObject sender);


        [Action ("btnEmployeeNumberTapped:")]
        partial void btnEmployeeNumberTapped (Foundation.NSObject sender);


        [Action ("btnDeleteTapped:")]
        partial void btnDeleteTapped (Foundation.NSObject sender);


        [Action ("btnClearAllTapped:")]
        partial void btnClearAllTapped (Foundation.NSObject sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}