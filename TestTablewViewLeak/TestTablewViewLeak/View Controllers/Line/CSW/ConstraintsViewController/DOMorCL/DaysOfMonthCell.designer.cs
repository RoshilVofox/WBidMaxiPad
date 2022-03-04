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
    [Register ("DaysOfMonthCell")]
    partial class DaysOfMonthCell
    {
        [Outlet]
        UIKit.UIButton btnAnyNight { get; set; }


        [Outlet]
        UIKit.UIButton btnBothEnds { get; set; }


        [Outlet]
        UIKit.UIButton btnBothEndsWt { get; set; }


        [Outlet]
        UIKit.UIButton btnCommHome { get; set; }


        [Outlet]
        UIKit.UIButton btnCommWork { get; set; }


        [Outlet]
        UIKit.UIButton btnFriArr { get; set; }


        [Outlet]
        UIKit.UIButton btnFriDep { get; set; }


        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }


        [Outlet]
        UIKit.UIButton btnInDomWt { get; set; }


        [Outlet]
        UIKit.UIButton btnLoadDefaults { get; set; }


        [Outlet]
        UIKit.UIButton btnMonThuArr { get; set; }


        [Outlet]
        UIKit.UIButton btnMonThuDep { get; set; }


        [Outlet]
        UIKit.UIButton btnOff { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnSatArr { get; set; }


        [Outlet]
        UIKit.UIButton btnSatDep { get; set; }


        [Outlet]
        UIKit.UIButton btnSaveDefaults { get; set; }


        [Outlet]
        UIKit.UIButton btnSunArr { get; set; }


        [Outlet]
        UIKit.UIButton btnSunDep { get; set; }


        [Outlet]
        UIKit.UIButton btnWork { get; set; }


        [Outlet]
        UIKit.UILabel lblMonth { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIView vwChildWeight { get; set; }


        [Outlet]
        UIKit.UIView vwComLines { get; set; }


        [Outlet]
        UIKit.UIView vwDayofMonth { get; set; }


        [Outlet]
        UIKit.UIView vwDays { get; set; }


        [Outlet]
        UIKit.UIView vwLabel { get; set; }


        [Action ("btnWorkOffTapped:")]
        partial void btnWorkOffTapped (UIKit.UIButton sender);


        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);


        [Action ("commuteOptionsTapped:")]
        partial void commuteOptionsTapped (UIKit.UIButton sender);


        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);


        [Action ("btnAnyNightBothEndsTapped:")]
        partial void btnAnyNightBothEndsTapped (UIKit.UIButton sender);


        [Action ("btnLoadDefaultsTapped:")]
        partial void btnLoadDefaultsTapped (UIKit.UIButton sender);


        [Action ("btnSaveDefaultsTapped:")]
        partial void btnSaveDefaultsTapped (UIKit.UIButton sender);


        [Action ("btnBothEndsWtTapped:")]
        partial void btnBothEndsWtTapped (UIKit.UIButton sender);


        [Action ("btnInDomWtTapped:")]
        partial void btnInDomWtTapped (UIKit.UIButton sender);


        [Action ("btnTimePopoverTapped:")]
        partial void btnTimePopoverTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}