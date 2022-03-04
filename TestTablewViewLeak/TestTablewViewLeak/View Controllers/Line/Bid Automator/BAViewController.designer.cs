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
    [Register ("BAViewController")]
    partial class BAViewController
    {
        [Outlet]
        UIKit.UIButton btnBidStuff { get; set; }


        [Outlet]
        UIKit.UIButton btnHome { get; set; }


        [Outlet]
        UIKit.UIButton btnMisc { get; set; }


        [Outlet]
        UIKit.UIButton btnRedo { get; set; }


        [Outlet]
        UIKit.UIButton btnReset { get; set; }


        [Outlet]
        UIKit.UIButton btnSave { get; set; }


        [Outlet]
        UIKit.UIButton btnUndo { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIToolbar tbTopBar { get; set; }


        [Outlet]
        UIKit.UIView vwConstraints { get; set; }


        [Outlet]
        UIKit.UIView vwSortAndWeights { get; set; }


        [Action ("btnHomeTapped:")]
        partial void btnHomeTapped (UIKit.UIButton sender);


        [Action ("BtnHelpClicked:")]
        partial void BtnHelpClicked (Foundation.NSObject sender);


        [Action ("btnBidStuffTapped:")]
        partial void btnBidStuffTapped (UIKit.UIButton sender);


        [Action ("btnMiscTapped:")]
        partial void btnMiscTapped (UIKit.UIButton sender);


        [Action ("btnResetTapped:")]
        partial void btnResetTapped (UIKit.UIButton sender);


        [Action ("btnSaveTapped:")]
        partial void btnSaveTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}