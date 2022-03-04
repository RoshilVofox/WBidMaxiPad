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
    [Register ("SynchSelectionViewController")]
    partial class SynchSelectionViewController
    {
        [Outlet]
        UIKit.UILabel lblLocalUpdatedDate { get; set; }


        [Outlet]
        UIKit.UILabel lblLocalUpdatedTime { get; set; }


        [Outlet]
        UIKit.UILabel lblQuickLocalDate { get; set; }


        [Outlet]
        UIKit.UILabel lblQuickLocalTime { get; set; }


        [Outlet]
        UIKit.UILabel lblQuickServerDate { get; set; }


        [Outlet]
        UIKit.UILabel lblQuickServerTime { get; set; }


        [Outlet]
        UIKit.UILabel lblServerUpdatedDate { get; set; }


        [Outlet]
        UIKit.UILabel lblServerUpdatedTime { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint mainViewHieghtConstraint { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint quicksetConstraint { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentQuickSet { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentState { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint stateConstraint { get; set; }


        [Outlet]
        UIKit.UIView SyncMainView { get; set; }


        [Outlet]
        UIKit.UIView viewQuickSet { get; set; }


        [Outlet]
        UIKit.UIView viewState { get; set; }


        [Action ("CloseAction:")]
        partial void CloseAction (Foundation.NSObject sender);


        [Action ("SyncAction:")]
        partial void SyncAction (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}