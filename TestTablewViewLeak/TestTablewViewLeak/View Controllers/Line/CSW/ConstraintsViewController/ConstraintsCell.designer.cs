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
    [Register ("ConstraintsCell")]
    partial class ConstraintsCell
    {
        [Outlet]
        UIKit.UIButton btnCommutabltytextlbl { get; set; }


        [Outlet]
        UIKit.UIButton btnCompareTerm { get; set; }


        [Outlet]
        UIKit.UIButton btnHelpIcon { get; set; }


        [Outlet]
        UIKit.UIButton btnParam { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UIButton btnSecondCell { get; set; }


        [Outlet]
        UIKit.UIButton btnThirdCell { get; set; }


        [Outlet]
        UIKit.UILabel lblConstraintsName { get; set; }


        [Action ("funCcommutblytextlbl:")]
        partial void funCcommutblytextlbl (Foundation.NSObject sender);


        [Action ("btnRemoveTap:")]
        partial void btnRemoveTap (UIKit.UIButton sender);


        [Action ("btnHelpIconTapped:")]
        partial void btnHelpIconTapped (UIKit.UIButton sender);


        [Action ("btnCompareTaped:")]
        partial void btnCompareTaped (UIKit.UIButton sender);


        [Action ("btnConstraintParamTapped:")]
        partial void btnConstraintParamTapped (UIKit.UIButton sender);


        [Action ("btnThirdCellTap:")]
        partial void btnThirdCellTap (UIKit.UIButton sender);


        [Action ("btnSecondCellTapped:")]
        partial void btnSecondCellTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}