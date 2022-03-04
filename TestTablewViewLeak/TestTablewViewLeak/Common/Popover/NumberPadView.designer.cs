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
    [Register ("NumberPadView")]
    partial class NumberPadView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UIButton btnClear { get; set; }


        [Outlet]
        UIKit.UIButton btnCLR { get; set; }


        [Outlet]
        UIKit.UIButton btnDot { get; set; }


        [Outlet]
        UIKit.UIButton btnInsert { get; set; }


        [Outlet]
        UIKit.UIButton btnMinus { get; set; }


        [Outlet]
        UIKit.UIButton [] btnNumPad { get; set; }


        [Outlet]
        UIKit.UIButton btnOK { get; set; }


        [Outlet]
        UIKit.UILabel lblNumDisplay { get; set; }


        [Action ("btnAddInsertTapped:")]
        partial void btnAddInsertTapped (UIKit.UIButton sender);


        [Action ("btnClearTapped:")]
        partial void btnClearTapped (UIKit.UIButton sender);


        [Action ("btnNumPadTapped:")]
        partial void btnNumPadTapped (UIKit.UIButton sender);


        [Action ("btnCLRTapped:")]
        partial void btnCLRTapped (UIKit.UIButton sender);


        [Action ("btnOKTapped:")]
        partial void btnOKTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}