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
    [Register ("getNewBidPeriodViewController")]
    partial class getNewBidPeriodViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnLogin { get; set; }


        [Outlet]
        UIKit.UIButton [] btnMonthCollection { get; set; }


        [Outlet]
        UIKit.UIButton btnOverlapCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnOverlapCorrectionOk { get; set; }


        [Outlet]
        UIKit.UIButton btnOverlapOK { get; set; }


        [Outlet]
        UIKit.UIButton btnOverlapSkip { get; set; }


        [Outlet]
        UIKit.UIButton btnRefresh { get; set; }


        [Outlet]
        UIKit.UIButton [] btnYear { get; set; }


        [Outlet]
        UIKit.UILabel lblImportLine { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIButton refreshHelpButton { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlVwBaseButton { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlVwPositionButton { get; set; }


        [Outlet]
        UIKit.UIScrollView scrlVwRoundButton { get; set; }


        [Outlet]
        UIKit.UISwitch swImport { get; set; }


        [Outlet]
        UIKit.UISwitch swOverlap { get; set; }


        [Outlet]
        UIKit.UIView viewOverlapAlert { get; set; }


        [Outlet]
        UIKit.UIView vwOverlapPrep { get; set; }


        [Outlet]
        UIKit.UIView vwOverlapPrep2ndRnd { get; set; }


        [Outlet]
        UIKit.UIView vwYear { get; set; }


        [Action ("btnCloseTapped:")]
        partial void btnCloseTapped (Foundation.NSObject sender);


        [Action ("btnLoginTapped:")]
        partial void btnLoginTapped (Foundation.NSObject sender);


        [Action ("btnMonthTapped:")]
        partial void btnMonthTapped (Foundation.NSObject sender);


        [Action ("btnOverlapAlertOkTapped:")]
        partial void btnOverlapAlertOkTapped (Foundation.NSObject sender);


        [Action ("btnOverlapCancelTapped:")]
        partial void btnOverlapCancelTapped (UIKit.UIButton sender);


        [Action ("btnOverlapOkTapped:")]
        partial void btnOverlapOkTapped (Foundation.NSObject sender);


        [Action ("btnOverlapOKTapped:")]
        partial void btnOverlapOKTapped (UIKit.UIButton sender);


        [Action ("btnRefreshTapped:")]
        partial void btnRefreshTapped (UIKit.UIButton sender);


        [Action ("btnYearTapped:")]
        partial void btnYearTapped (UIKit.UIButton sender);


        [Action ("refreshHelpBtnTapped:")]
        partial void refreshHelpBtnTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}