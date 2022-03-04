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
    [Register ("BidCollectionCell")]
    partial class BidCollectionCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UILabel lblSubTitle { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Action ("btnDeleteTapped:")]
        partial void btnDeleteTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}