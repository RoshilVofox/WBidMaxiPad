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
    [Register ("StartDayOfWeekCell")]
    partial class StartDayOfWeekCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnFri { get; set; }


        [Outlet]
        UIKit.UIButton btnMon { get; set; }


        [Outlet]
        UIKit.UIButton btnSat { get; set; }


        [Outlet]
        UIKit.UIButton btnSun { get; set; }


        [Outlet]
        UIKit.UIButton btnThu { get; set; }


        [Outlet]
        UIKit.UIButton btnTue { get; set; }


        [Outlet]
        UIKit.UIButton btnWed { get; set; }


        [Outlet]
        UIKit.UILabel lbStartDayOfWeek { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (UIKit.UIButton sender);


        [Action ("OnSunButtonEvent:")]
        partial void OnSunButtonEvent (UIKit.UIButton sender);


        [Action ("OnMonButtonEvent:")]
        partial void OnMonButtonEvent (UIKit.UIButton sender);


        [Action ("OnTueButtonEvent:")]
        partial void OnTueButtonEvent (UIKit.UIButton sender);


        [Action ("OnWedButtonEvent:")]
        partial void OnWedButtonEvent (UIKit.UIButton sender);


        [Action ("OnThuButtonEvent:")]
        partial void OnThuButtonEvent (UIKit.UIButton sender);


        [Action ("OnFriButtonEvent:")]
        partial void OnFriButtonEvent (UIKit.UIButton sender);


        [Action ("OnSatButtonEvent:")]
        partial void OnSatButtonEvent (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}