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
    [Register ("TestVacationViewController")]
    partial class TestVacationViewController
    {
        [Outlet]
        UIKit.UIButton BtnCancel { get; set; }


        [Outlet]
        UIKit.UIButton [] btnCFVSelect { get; set; }


        [Outlet]
        UIKit.UIButton [] btnCFVStartDate { get; set; }


        [Outlet]
        UIKit.UIButton [] btnEndDate { get; set; }


        [Outlet]
        UIKit.UIButton btnOK { get; set; }


        [Outlet]
        UIKit.UIButton [] btnSelect { get; set; }


        [Outlet]
        UIKit.UIButton [] btnStartDate { get; set; }


        [Outlet]
        UIKit.UIButton [] btnVacationSelect { get; set; }


        [Outlet]
        UIKit.UIButton [] btnVEndDate { get; set; }


        [Outlet]
        UIKit.UIButton [] btnVStartDate { get; set; }


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnCFVSelectTapped:")]
        partial void btnCFVSelectTapped (UIKit.UIButton sender);


        [Action ("btnCFVStartDateTapped:")]
        partial void btnCFVStartDateTapped (UIKit.UIButton sender);


        [Action ("btnEndDateTapped:")]
        partial void btnEndDateTapped (UIKit.UIButton sender);


        [Action ("btnOKTapped:")]
        partial void btnOKTapped (UIKit.UIButton sender);


        [Action ("btnSelectTapped:")]
        partial void btnSelectTapped (UIKit.UIButton sender);


        [Action ("btnStartDateTapped:")]
        partial void btnStartDateTapped (UIKit.UIButton sender);


        [Action ("btnVacationEndDateTapped:")]
        partial void btnVacationEndDateTapped (UIKit.UIButton sender);


        [Action ("btnVacationSelectTapped:")]
        partial void btnVacationSelectTapped (UIKit.UIButton sender);


        [Action ("btnVacationStartDateTapped:")]
        partial void btnVacationStartDateTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}