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
    [Register ("ConfigViewController")]
    partial class ConfigViewController
    {
        [Outlet]
        UIKit.UIButton btnAMArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnAmpmOK { get; set; }


        [Outlet]
        UIKit.UIButton btnAMPush { get; set; }


        [Outlet]
        UIKit.UIButton btnMaxNumber { get; set; }


        [Outlet]
        UIKit.UIButton btnMaxPercentage { get; set; }


        [Outlet]
        UIKit.UIButton btnMaxStartDay { get; set; }


        [Outlet]
        UIKit.UIButton btnNTEArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnNTEPush { get; set; }


        [Outlet]
        UIKit.UIButton btnPMArrive { get; set; }


        [Outlet]
        UIKit.UIButton btnPMPush { get; set; }


        [Outlet]
        UIKit.UIButton btnSecretServerType { get; set; }


        [Outlet]
        UIKit.UIButton btnWeekMaxNumber { get; set; }


        [Outlet]
        UIKit.UIButton btnWeekMaxPercentage { get; set; }


        [Outlet]
        UIKit.UIButton btnWeekOk { get; set; }


        [Outlet]
        UIKit.UIButton btnWeekReset { get; set; }


        [Outlet]
        UIKit.UILabel lblServerType { get; set; }


        [Outlet]
        UIKit.UINavigationBar navBar { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segModernScrollOptions { get; set; }


        [Outlet]
        UIKit.UISegmentedControl SegServerType { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgAMPM { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgHotelList { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgLineParam { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPairSpan { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPairTime { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgWeekendDays { get; set; }


        [Outlet]
        UIKit.UIStepper stprAutoSaveTime { get; set; }


        [Outlet]
        UIKit.UIStepper stpWeek { get; set; }


        [Outlet]
        UIKit.UISwitch swAutoSave { get; set; }


        [Outlet]
        UIKit.UISwitch swBidEmail { get; set; }


        [Outlet]
        UIKit.UISwitch swBorder { get; set; }


        [Outlet]
        UIKit.UISwitch swCheckData { get; set; }


        [Outlet]
        UIKit.UISwitch swCrash { get; set; }


        [Outlet]
        UIKit.UISwitch swGatherTrips { get; set; }


        [Outlet]
        UIKit.UISwitch swMIL { get; set; }


        [Outlet]
        UIKit.UISwitch swMissingData { get; set; }


        [Outlet]
        UIKit.UISwitch swPairInSub { get; set; }


        [Outlet]
        UIKit.UISwitch swShowCover { get; set; }


        [Outlet]
        UIKit.UISwitch swSummaryShade { get; set; }


        [Outlet]
        UIKit.UISwitch swSync { get; set; }


        [Outlet]
        UIKit.UITextField txtAutoSaveTime { get; set; }


        [Outlet]
        UIKit.UIView vwAMPM { get; set; }


        [Outlet]
        UIKit.UIView vwHotels { get; set; }


        [Outlet]
        UIKit.UIView vwMisc { get; set; }


        [Outlet]
        UIKit.UIView vwPairExp { get; set; }


        [Outlet]
        UIKit.UIView vwUser { get; set; }


        [Outlet]
        UIKit.UIView vwWeek { get; set; }


        [Action ("btnAmpmOKTapped:")]
        partial void btnAmpmOKTapped (UIKit.UIButton sender);


        [Action ("btnAMPMResetTapped:")]
        partial void btnAMPMResetTapped (UIKit.UIButton sender);


        [Action ("btnAMPMTimesTapped:")]
        partial void btnAMPMTimesTapped (UIKit.UIButton sender);


        [Action ("btnDoneTapped:")]
        partial void btnDoneTapped (UIKit.UIBarButtonItem sender);


        [Action ("btnMaxInputTapped:")]
        partial void btnMaxInputTapped (UIKit.UIButton sender);


        [Action ("btnMaxStartDayTapped:")]
        partial void btnMaxStartDayTapped (UIKit.UIButton sender);


        [Action ("btnTipTapped:")]
        partial void btnTipTapped (UIKit.UIButton sender);


        [Action ("btnWeekMaxNumberTapped:")]
        partial void btnWeekMaxNumberTapped (UIKit.UIButton sender);


        [Action ("btnWeekMaxPercentageTapped:")]
        partial void btnWeekMaxPercentageTapped (UIKit.UIButton sender);


        [Action ("btnWeekOkTapped:")]
        partial void btnWeekOkTapped (UIKit.UIButton sender);


        [Action ("btnWeekResetTapped:")]
        partial void btnWeekResetTapped (UIKit.UIButton sender);


        [Action ("funBorderChanged:")]
        partial void funBorderChanged (Foundation.NSObject sender);


        [Action ("funModernScrollViewOptionsAction:")]
        partial void funModernScrollViewOptionsAction (Foundation.NSObject sender);


        [Action ("ServerChange:")]
        partial void ServerChange (Foundation.NSObject sender);


        [Action ("sgAMPMChanged:")]
        partial void sgAMPMChanged (UIKit.UISegmentedControl sender);


        [Action ("sgHotelListChanged:")]
        partial void sgHotelListChanged (UIKit.UISegmentedControl sender);


        [Action ("sgLineParamChanged:")]
        partial void sgLineParamChanged (UIKit.UISegmentedControl sender);


        [Action ("sgPairSpanTapped:")]
        partial void sgPairSpanTapped (UIKit.UISegmentedControl sender);


        [Action ("sgPairTimeTapped:")]
        partial void sgPairTimeTapped (UIKit.UISegmentedControl sender);


        [Action ("sgWeekendDaysTapped:")]
        partial void sgWeekendDaysTapped (UIKit.UISegmentedControl sender);


        [Action ("SummaryViewShadeValueChange:")]
        partial void SummaryViewShadeValueChange (Foundation.NSObject sender);


        [Action ("swPairInSubTapped:")]
        partial void swPairInSubTapped (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}