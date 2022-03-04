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
    [Register ("ContraintsViewController")]
    partial class ConstraintsViewController
    {
        [Outlet]
        UIKit.UIButton [] btnABCD { get; set; }


        [Outlet]
        UIKit.UIButton btnAMs { get; set; }


        [Outlet]
        UIKit.UIButton btnBlank { get; set; }


        [Outlet]
        UIKit.UIButton btnClear { get; set; }


        [Outlet]
        UIKit.UIButton [] btnDays { get; set; }


        [Outlet]
        UIKit.UIButton btnETOPS { get; set; }


        [Outlet]
        UIKit.UIButton btnETOPSRES { get; set; }


        [Outlet]
        UIKit.UIButton btnHard { get; set; }


        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }


        [Outlet]
        UIKit.UIButton btnInternational { get; set; }


        [Outlet]
        UIKit.UIButton btnLines { get; set; }


        [Outlet]
        UIKit.UIButton btnMix { get; set; }


        [Outlet]
        UIKit.UIButton btnNonConcus { get; set; }


        [Outlet]
        UIKit.UIButton btnPMs { get; set; }


        [Outlet]
        UIKit.UIButton btnReserve { get; set; }


        [Outlet]
        UIKit.UIButton [] btnTurns { get; set; }


        [Outlet]
        UIKit.UILabel lblLinesNum { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlConstraints { get; set; }


        [Outlet]
        UIKit.UIToolbar tbBottomBar { get; set; }


        [Outlet]
        UIKit.UIView vwAMPMcontainer { get; set; }


        [Outlet]
        UIKit.UIView vwConst { get; set; }


        [Outlet]
        UIKit.UIView vwFirstConstantConstraintContainer { get; set; }


        [Outlet]
        UIKit.UIView vwMainConst { get; set; }


        [Outlet]
        UIKit.UIView vwSecondConstantConstraintContainer { get; set; }


        [Action ("btnABCDTapped:")]
        partial void btnABCDTapped (UIKit.UIButton sender);


        [Action ("btnAMPMMixTapped:")]
        partial void btnAMPMMixTapped (UIKit.UIButton sender);


        [Action ("btnBlankTapped:")]
        partial void btnBlankTapped (UIKit.UIButton sender);


        [Action ("btnClearTapped:")]
        partial void btnClearTapped (UIKit.UIButton sender);


        [Action ("btnDefineTapped:")]
        partial void btnDefineTapped (UIKit.UIButton sender);


        [Action ("btnETOPSRESTapped:")]
        partial void btnETOPSRESTapped (UIKit.UIButton sender);


        [Action ("btnETOPSTapped:")]
        partial void btnETOPSTapped (UIKit.UIButton sender);


        [Action ("btnHardTapped:")]
        partial void btnHardTapped (UIKit.UIButton sender);


        [Action ("BtnInternational_TouchUpInside:")]
        partial void BtnInternational_TouchUpInside (UIKit.UIButton sender);


        [Action ("btnInternationalTapped:")]
        partial void btnInternationalTapped (UIKit.UIButton sender);


        [Action ("btnLinesTapped:")]
        partial void btnLinesTapped (UIKit.UIButton sender);


        [Action ("btnNonConusTapped:")]
        partial void btnNonConusTapped (UIKit.UIButton sender);


        [Action ("btnPlus2Tapped:")]
        partial void btnPlus2Tapped (UIKit.UIButton sender);


        [Action ("btnPlusTapped:")]
        partial void btnPlusTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnReserveTapped:")]
        partial void btnReserveTapped (UIKit.UIButton sender);


        [Action ("btnTurnsTripsTapped:")]
        partial void btnTurnsTripsTapped (UIKit.UIButton sender);


        [Action ("btnWeekDayTapped:")]
        partial void btnWeekDayTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}