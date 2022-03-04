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
    [Register ("UserAccountDifferenceCell")]
    partial class UserAccountDifferenceCell
    {
        [Outlet]
        UIKit.UILabel lblEmployeeDetails { get; set; }


        [Outlet]
        UIKit.UISegmentedControl SegSelection { get; set; }


        [Action ("SegmentButtonClicked:")]
        partial void SegmentButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}