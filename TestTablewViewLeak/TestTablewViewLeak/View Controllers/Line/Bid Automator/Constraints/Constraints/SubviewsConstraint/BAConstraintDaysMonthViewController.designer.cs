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
    [Register ("BAConstraintDaysMonthViewController")]
    partial class BAConstraintDaysMonthViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnClear { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnDone { get; set; }


        [Outlet]
        UIKit.UIView viewCalendarShow { get; set; }


        [Action ("ClearButtonClicked:")]
        partial void ClearButtonClicked (Foundation.NSObject sender);


        [Action ("DoneButtonClicked:")]
        partial void DoneButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}