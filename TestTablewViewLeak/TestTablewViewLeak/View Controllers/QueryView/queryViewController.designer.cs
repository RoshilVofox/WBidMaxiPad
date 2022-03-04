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
    [Register ("queryViewController")]
    partial class queryViewController
    {
        [Outlet]
        UIKit.UIButton btnShowBidChoices { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblAvoidanceText { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddyBidHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblBuddyBidText { get; set; }


        [Outlet]
        UIKit.UILabel lblQueryHeader { get; set; }


        [Outlet]
        UIKit.UITextField txtAvoidance { get; set; }


        [Outlet]
        UIKit.UITextField txtBuddyBid { get; set; }


        [Outlet]
        UIKit.UITextField txtSubmitBid { get; set; }


        [Outlet]
        UIKit.UIView vwAvoidance { get; set; }


        [Outlet]
        UIKit.UIView vwBuddyBid { get; set; }


        [Action ("back:")]
        partial void back (UIKit.UIBarButtonItem sender);


        [Action ("btnCancelTapped:")]
        partial void btnCancelTapped (UIKit.UIButton sender);


        [Action ("btnShowBidChoicesTapped:")]
        partial void btnShowBidChoicesTapped (Foundation.NSObject sender);


        [Action ("btnSubmitTapped:")]
        partial void btnSubmitTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}