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
    [Register ("OverlapCorrectionViewController")]
    partial class OverlapCorrectionViewController
    {
        [Outlet]
        UIKit.UIButton [] btnBlockTime { get; set; }


        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnOK { get; set; }


        [Outlet]
        UIKit.UIButton btnRestTime { get; set; }


        [Outlet]
        UIKit.UILabel [] lblBlockTime { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgOverlapChoice { get; set; }


        [Action ("btnBlockTimeTapped:")]
        partial void btnBlockTimeTapped (UIKit.UIButton sender);


        [Action ("btnRestTimeTapped:")]
        partial void btnRestTimeTapped (UIKit.UIButton sender);


        [Action ("sgOverlapChoiceChanged:")]
        partial void sgOverlapChoiceChanged (UIKit.UISegmentedControl sender);


        [Action ("btnOKTapped:")]
        partial void btnOKTapped (UIKit.UIButton sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}