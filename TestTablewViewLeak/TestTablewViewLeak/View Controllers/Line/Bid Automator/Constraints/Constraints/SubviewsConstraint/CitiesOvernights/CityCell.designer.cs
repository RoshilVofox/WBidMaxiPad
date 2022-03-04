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
    [Register ("CityCell")]
    partial class CityCell
    {
        [Outlet]
        UIKit.UIButton btnCount { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }


        [Action ("OnCellClicked:")]
        partial void OnCellClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}