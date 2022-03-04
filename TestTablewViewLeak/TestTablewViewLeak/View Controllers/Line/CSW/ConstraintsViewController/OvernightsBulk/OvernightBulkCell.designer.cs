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
    [Register ("OvernightBulkCell")]
    partial class OvernightBulkCell
    {
        [Outlet]
        UIKit.UIButton btnClose { get; set; }


        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }


        [Outlet]
        UIKit.UILabel btnNone { get; set; }


        [Outlet]
        UIKit.UILabel lblNo { get; set; }


        [Outlet]
        UIKit.UILabel lblNoOvernights { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UILabel lblYes { get; set; }


        [Outlet]
        UIKit.UIView vwCities { get; set; }


        [Outlet]
        UIKit.UIView vwLabel { get; set; }


        [Outlet]
        UIKit.UIView vwNoOvernights { get; set; }


        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);


        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}