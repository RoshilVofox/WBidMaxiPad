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
    [Register ("CommutableTimeViewController")]
    partial class CommutableTimeViewController
    {
        [Outlet]
        UIKit.UILabel lblBase { get; set; }


        [Outlet]
        UIKit.UILabel lblCommuteCity { get; set; }


        [Outlet]
        UIKit.UINavigationBar lblNavTitle { get; set; }


        [Outlet]
        UIKit.UIView Viewcalender { get; set; }


        [Action ("DismissCalendarView:")]
        partial void DismissCalendarView (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}