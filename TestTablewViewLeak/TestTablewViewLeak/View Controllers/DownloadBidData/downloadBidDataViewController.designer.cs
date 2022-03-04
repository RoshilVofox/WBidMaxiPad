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
    [Register ("downloadBidDataViewController")]
    partial class downloadBidDataViewController
    {
        [Outlet]
        UIKit.UIButton btnCalculateVACCorrection { get; set; }


        [Outlet]
        UIKit.UIButton btnCheckAuthorization { get; set; }


        [Outlet]
        UIKit.UIButton btnCheckCWACredentials { get; set; }


        [Outlet]
        UIKit.UIButton btnCheckInternetConnection { get; set; }


        [Outlet]
        UIKit.UIButton btnGetDataFiles { get; set; }


        [Outlet]
        UIKit.UIButton btnParseData { get; set; }


        [Outlet]
        UIKit.UIButton btnVacationData { get; set; }


        [Outlet]
        UIKit.UIButton btnVacDone { get; set; }


        [Outlet]
        UIKit.UIButton btnVacLater { get; set; }


        [Outlet]
        UIKit.UILabel lblMessage1 { get; set; }


        [Outlet]
        UIKit.UILabel lblMessage2 { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIProgressView prgrsVw { get; set; }


        [Outlet]
        UIKit.UITextField txtVANumber { get; set; }


        [Outlet]
        UIKit.UIView vwVacOverLap { get; set; }


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}