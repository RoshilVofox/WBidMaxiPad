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
    [Register ("RetrieveAwardViewController")]
    partial class RetrieveAwardViewController
    {
        [Outlet]
        UIKit.UIButton btnSelectedDomicile { get; set; }


        [Outlet]
        UIKit.UIPickerView pckrDomicilePick { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgAwards { get; set; }


        [Outlet]
        UIKit.UISegmentedControl sgPosition { get; set; }


        [Action ("btnAwardsTapped:")]
        partial void btnAwardsTapped (Foundation.NSObject sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (Foundation.NSObject sender);


        [Action ("btnHelpTapped:")]
        partial void btnHelpTapped (Foundation.NSObject sender);


        [Action ("btnSelectDomicleTapped:")]
        partial void btnSelectDomicleTapped (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}