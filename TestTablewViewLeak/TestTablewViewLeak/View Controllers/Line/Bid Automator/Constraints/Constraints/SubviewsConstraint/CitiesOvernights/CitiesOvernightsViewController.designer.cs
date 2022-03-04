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
    [Register ("CitiesOvernightsViewController")]
    partial class CitiesOvernightsViewController
    {
        [Outlet]
        UIKit.UIButton btnClear { get; set; }


        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UICollectionView collectionView { get; set; }


        [Action ("OnClearEvent:")]
        partial void OnClearEvent (Foundation.NSObject sender);


        [Action ("OnDoneEvent:")]
        partial void OnDoneEvent (Foundation.NSObject sender);

        [Action ("OnClearEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnClearEvent (UIKit.UIBarButtonItem sender);

        [Action ("OnDoneEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnDoneEvent (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}