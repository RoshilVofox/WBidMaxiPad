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
    [Register ("BADaysOfMonthCell")]
    partial class BADaysOfMonthCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UILabel lbDayOfMonth { get; set; }


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnOpenPickerEvent:")]
        partial void OnOpenPickerEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}