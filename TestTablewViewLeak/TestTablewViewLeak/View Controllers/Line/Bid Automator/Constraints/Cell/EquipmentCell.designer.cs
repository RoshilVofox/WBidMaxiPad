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
    [Register ("EquipmentCell")]
    partial class EquipmentCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEquipment { get; set; }


        [Outlet]
        UIKit.UIButton btnEquipmentValue { get; set; }


        [Outlet]
        UIKit.UIButton btnLessthan { get; set; }

        [Action ("OnEquipmentDeleteEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnEquipmentDeleteEvent (UIKit.UIButton sender);

        [Action ("OnEquipmentEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnEquipmentEvent (UIKit.UIButton sender);

        [Action ("OnEquipmentValueEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnEquipmentValueEvent (UIKit.UIButton sender);

        [Action ("OnLessthanEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnLessthanEvent (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}