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
    [Register ("CSWViewController")]
    partial class CSWViewController
    {
        [Outlet]
        UIKit.UIButton btnBidStuff { get; set; }


        [Outlet]
        UIKit.UIButton btnHelp { get; set; }


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
        UIKit.UIButton reduButtonView { get; set; }


        [Outlet]
        UIKit.UIToolbar tbTopBar { get; set; }


        [Outlet]
        UIKit.UIButton undoButtonView { get; set; }


        [Outlet]
        UIKit.UIView vwConstraints { get; set; }


        [Outlet]
        UIKit.UIView vwSortAndWeights { get; set; }


        [Action ("btnBidStuffTapped:")]
        partial void btnBidStuffTapped (UIKit.UIButton sender);


        [Action ("btnHelpTapped:")]
        partial void btnHelpTapped (UIKit.UIButton sender);


        [Action ("btnHomeTapped:")]
        partial void btnHomeTapped (UIKit.UIButton sender);


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