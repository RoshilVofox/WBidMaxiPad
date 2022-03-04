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
    [Register ("DaysOfWeekAllCell")]
    partial class DaysOfWeekAllCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnFriday { get; set; }


        [Outlet]
        UIKit.UIButton btnMonday { get; set; }


        [Outlet]
        UIKit.UIButton btnSaturday { get; set; }


        [Outlet]
        UIKit.UIButton btnSunday { get; set; }


        [Outlet]
        UIKit.UIButton btnThursday { get; set; }


        [Outlet]
        UIKit.UIButton btnTuesday { get; set; }


        [Outlet]
        UIKit.UIButton btnWednesday { get; set; }


        [Outlet]
        UIKit.UILabel lbDayOfWeekAll { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnFriButtonEvent:")]
        partial void OnFriButtonEvent (Foundation.NSObject sender);


        [Action ("OnMonButtonEvent:")]
        partial void OnMonButtonEvent (Foundation.NSObject sender);


        [Action ("OnSatButtonEvent:")]
        partial void OnSatButtonEvent (Foundation.NSObject sender);


        [Action ("OnSunButtonEvent:")]
        partial void OnSunButtonEvent (Foundation.NSObject sender);


        [Action ("OnThuButtonEvent:")]
        partial void OnThuButtonEvent (Foundation.NSObject sender);


        [Action ("OnTueButtonEvent:")]
        partial void OnTueButtonEvent (Foundation.NSObject sender);


        [Action ("OnWedButtonEvent:")]
        partial void OnWedButtonEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}