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
    [Register ("sortAndWeightViewController")]
    partial class sortAndWeightViewController
    {
        [Outlet]
        UIKit.UIButton btnBlankToBottom { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockSort { get; set; }

        [Outlet]
        UIKit.UIButton btnClear { get; set; }

        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }

        [Outlet]
        UIKit.UIButton btnLineNum { get; set; }

        [Outlet]
        UIKit.UIButton btnLinePay { get; set; }

        [Outlet]
        UIKit.UIButton btnLines { get; set; }

        [Outlet]
        UIKit.UIButton btnPlus2 { get; set; }

        [Outlet]
        UIKit.UIButton btnPPDay { get; set; }

        [Outlet]
        UIKit.UIButton btnPPDutyHr { get; set; }

        [Outlet]
        UIKit.UIButton btnPPFDP { get; set; }

        [Outlet]
        UIKit.UIButton btnPPFlightHr { get; set; }

        [Outlet]
        UIKit.UIButton btnPPTimeAway { get; set; }

        [Outlet]
        UIKit.UIButton btnReserveToBottom { get; set; }

        [Outlet]
        UIKit.UIButton btnSelColumn { get; set; }

        [Outlet]
        UIKit.UIButton btnSortByAward { get; set; }

        [Outlet]
        UIKit.UIButton btnSortBySubmittedBid { get; set; }

        [Outlet]
        UIKit.UILabel lblManual { get; set; }

        [Outlet]
        UIKit.UILabel lblSelected { get; set; }

        [Outlet]
        UIKit.UISegmentedControl sgSortnWeights { get; set; }

        [Outlet]
        UIKit.UIToolbar tlbSort { get; set; }

        [Outlet]
        UIKit.UIView vwSort { get; set; }

        [Outlet]
        UIKit.UIView vwSortContainer { get; set; }

        [Outlet]
        UIKit.UIView vwSortList { get; set; }

        [Outlet]
        UIKit.UIView vwWeights { get; set; }

        [Action ("btnAddBlockSort:")]
        partial void btnAddBlockSort (UIKit.UIButton sender);

        [Action ("btnBlankToBotTap:")]
        partial void btnBlankToBotTap (UIKit.UIButton sender);

        [Action ("btnClearTap:")]
        partial void btnClearTap (UIKit.UIButton sender);

        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);

        [Action ("btnLinesTapped:")]
        partial void btnLinesTapped (UIKit.UIButton sender);

        [Action ("btnPlus2Tapped:")]
        partial void btnPlus2Tapped (UIKit.UIButton sender);

        [Action ("btnReserveToBotTap:")]
        partial void btnReserveToBotTap (UIKit.UIButton sender);




        [Action ("btnSortParaTap:")]
        partial void btnSortParaTap (UIKit.UIButton sender);

        [Action ("sgSortnWeightsTapped:")]
        partial void sgSortnWeightsTapped (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}