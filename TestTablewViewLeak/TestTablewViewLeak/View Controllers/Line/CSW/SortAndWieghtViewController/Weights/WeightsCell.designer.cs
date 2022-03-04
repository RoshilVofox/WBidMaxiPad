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
    [Register ("WeightsCell")]
    partial class WeightsCell
    {
        [Outlet]
        UIKit.UIButton btnCommuteTitleLabel { get; set; }


        [Outlet]
        UIKit.UIButton btnCommuteWeight { get; set; }


        [Outlet]
        UIKit.UIButton [] btnDOWwt { get; set; }


        [Outlet]
        UIKit.UIButton btnFirstCell { get; set; }


        [Outlet]
        UIKit.UIButton btnFri { get; set; }


        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }


        [Outlet]
        UIKit.UIButton btnMon { get; set; }


        [Outlet]
        UIKit.UIButton btnOff { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnSat { get; set; }


        [Outlet]
        UIKit.UIButton btnSecondCell { get; set; }


        [Outlet]
        UIKit.UIButton btnSun { get; set; }


        [Outlet]
        UIKit.UIButton btnThirdCell { get; set; }


        [Outlet]
        UIKit.UIButton btnThu { get; set; }


        [Outlet]
        UIKit.UIButton btnTue { get; set; }


        [Outlet]
        UIKit.UIButton btnWed { get; set; }


        [Outlet]
        UIKit.UIButton btnWeight { get; set; }


        [Outlet]
        UIKit.UIButton btnWork { get; set; }


        [Outlet]
        UIKit.UILabel lblWeightsTitle { get; set; }


        [Outlet]
        UIKit.UIView vwDOW { get; set; }


        [Outlet]
        UIKit.UIView vwWrkOff { get; set; }


        [Action ("btnDOWWeightsTapped:")]
        partial void btnDOWWeightsTapped (UIKit.UIButton sender);


        [Action ("btnFirstCellTapped:")]
        partial void btnFirstCellTapped (UIKit.UIButton sender);


        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);


        [Action ("btnRemoveTapped:")]
        partial void btnRemoveTapped (UIKit.UIButton sender);


        [Action ("btnSecondCellTapped:")]
        partial void btnSecondCellTapped (UIKit.UIButton sender);


        [Action ("btnThirdCellTapped:")]
        partial void btnThirdCellTapped (UIKit.UIButton sender);


        [Action ("btnWeightTapped:")]
        partial void btnWeightTapped (UIKit.UIButton sender);


        [Action ("btnWorkOffTapped:")]
        partial void btnWorkOffTapped (UIKit.UIButton sender);


        [Action ("funCommutabilityLineClicked:")]
        partial void funCommutabilityLineClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}