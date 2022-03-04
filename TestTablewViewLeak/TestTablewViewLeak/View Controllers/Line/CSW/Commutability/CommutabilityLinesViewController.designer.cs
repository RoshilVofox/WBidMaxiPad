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
    [Register ("CommutabilityLinesViewController")]
    partial class CommutabilityLinesViewController
    {
        [Outlet]
        UIKit.UIButton btnBackToBase { get; set; }


        [Outlet]
        UIKit.UIButton btnCityName { get; set; }


        [Outlet]
        UIKit.UIButton btnDoneSetting { get; set; }


        [Outlet]
        UIKit.UIButton btnInfoBackToBase { get; set; }


        [Outlet]
        UIKit.UIButton btnInfoCmtCity { get; set; }


        [Outlet]
        UIKit.UIButton btnInfoConnectTime { get; set; }


        [Outlet]
        UIKit.UIButton btnInfoPadCheckIn { get; set; }


        [Outlet]
        UIKit.UIButton btnInfoViewCommuteTime { get; set; }


        [Outlet]
        UIKit.UIButton btnTimeCheckIn { get; set; }


        [Outlet]
        UIKit.UIButton btnTimeConnect { get; set; }


        [Outlet]
        UIKit.UIButton btnViewCommuteTime { get; set; }


        [Action ("OnBackToBaseEvent:")]
        partial void OnBackToBaseEvent (Foundation.NSObject sender);


        [Action ("OnDoneSettingEvent:")]
        partial void OnDoneSettingEvent (Foundation.NSObject sender);


        [Action ("OnPadCheckInEvent:")]
        partial void OnPadCheckInEvent (Foundation.NSObject sender);


        [Action ("OnSetCityNameEvent:")]
        partial void OnSetCityNameEvent (Foundation.NSObject sender);


        [Action ("DismissCommuteViewController:")]
        partial void DismissCommuteViewController (Foundation.NSObject sender);


        [Action ("OnSetConnectTimeEvent:")]
        partial void OnSetConnectTimeEvent (Foundation.NSObject sender);


        [Action ("OnViewCommuteTime:")]
        partial void OnViewCommuteTime (Foundation.NSObject sender);


        [Action ("OnInfoCommuteCityEvent:")]
        partial void OnInfoCommuteCityEvent (Foundation.NSObject sender);


        [Action ("OnInfoConnectTimeEvent:")]
        partial void OnInfoConnectTimeEvent (Foundation.NSObject sender);


        [Action ("OnInfoPadCheckInEvent:")]
        partial void OnInfoPadCheckInEvent (Foundation.NSObject sender);


        [Action ("OnInfoBackToBaseEvent:")]
        partial void OnInfoBackToBaseEvent (Foundation.NSObject sender);


        [Action ("OnInfoViewCommuteTimeEvent:")]
        partial void OnInfoViewCommuteTimeEvent (Foundation.NSObject sender);


        [Action ("OnInfoViewCommuteTime:")]
        partial void OnInfoViewCommuteTime (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}