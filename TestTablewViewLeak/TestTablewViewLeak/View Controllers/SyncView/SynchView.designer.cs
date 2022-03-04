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
    [Register ("SynchView")]
    partial class SynchView
    {
        [Outlet]
        UIKit.UIButton btnBoth { get; set; }


        [Outlet]
        UIKit.UIButton btnQuickSet { get; set; }


        [Outlet]
        UIKit.UIButton btnState { get; set; }


        [Action ("ActionBoth:")]
        partial void ActionBoth (UIKit.UIButton sender);


        [Action ("actionCancel:")]
        partial void actionCancel (UIKit.UIButton sender);


        [Action ("ActionOK:")]
        partial void ActionOK (UIKit.UIButton sender);


        [Action ("ActionQuickset:")]
        partial void ActionQuickset (UIKit.UIButton sender);


        [Action ("actionState:")]
        partial void actionState (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}