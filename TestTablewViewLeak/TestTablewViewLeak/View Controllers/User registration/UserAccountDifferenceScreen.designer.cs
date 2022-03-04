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
    [Register ("UserAccountDifferenceScreen")]
    partial class UserAccountDifferenceScreen
    {
        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Action ("btnCancelClicked:")]
        partial void btnCancelClicked (Foundation.NSObject sender);


        [Action ("btnUpdateClicked:")]
        partial void btnUpdateClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}