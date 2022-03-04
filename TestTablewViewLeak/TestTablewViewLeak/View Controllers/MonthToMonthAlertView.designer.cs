// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TestTablewViewLeak.ViewControllers
{
    [Register ("MonthToMonthAlertView")]
    partial class MonthToMonthAlertView
    {
        [Outlet]
        UIKit.UIButton btnLink1 { get; set; }


        [Outlet]
        UIKit.UIButton btnLink2 { get; set; }


        [Outlet]
        UIKit.UILabel lblAlert { get; set; }


        [Action ("btnLink1Tap:")]
        partial void btnLink1Tap (Foundation.NSObject sender);


        [Action ("btnLink2Tap:")]
        partial void btnLink2Tap (Foundation.NSObject sender);


        [Action ("btnOkTap:")]
        partial void btnOkTap (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}